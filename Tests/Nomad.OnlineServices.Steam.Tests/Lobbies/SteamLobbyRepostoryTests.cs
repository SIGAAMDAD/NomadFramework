/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Steamworks;
using Nomad.Core.Exceptions;
using Nomad.OnlineServices.Steam.Private.Repositories;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Nomad.Events;

// Assumes InternalsVisibleTo is added to the production assembly
namespace Nomad.OnlineServices.Steam.Tests
{
	[TestFixture]
	[Category("Nomad.OnlineServices.Steam")]
	[Category("Lobbies")]
	[Category("Unit")]
	[Category("Steam")]
	public class SteamLobbyRepositoryTests
	{
		private MockCVarSystem _cvarSystem;
		private SteamLobbyRepository _repository;

		[SetUp]
		public void SetUp()
		{
			Environment.SetEnvironmentVariable("SteamAppId", "480");
			Environment.SetEnvironmentVariable("SteamGameId", "480");

			var initResult = SteamAPI.InitEx(out string errorMsg);
			if (initResult != ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
			{
				Assert.Inconclusive($"Could not initialize SteamAPI: {initResult} - {errorMsg}");
			}

			PumpSteamCallbacks(2000);

			var logger = new MockLogger();
			var eventFactory = new GameEventRegistry(logger);
			_cvarSystem = new MockCVarSystem(eventFactory);
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_PURGE_INTERVAL, 60); // default 60 seconds
			_repository = new SteamLobbyRepository(_cvarSystem);
		}

		[TearDown]
		public void TearDown()
		{
			_cvarSystem?.Dispose();
			_repository?.Dispose();
			PumpSteamCallbacks(100); // clean up any pending callbacks

			SteamAPI.Shutdown();
		}

		// Helper to pump Steam callbacks
		private static void PumpSteamCallbacks(int milliseconds)
		{
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			while (stopwatch.ElapsedMilliseconds < milliseconds)
			{
				SteamAPI.RunCallbacks();
				Thread.Sleep(10);
			}
		}

		// Helper to create a test lobby and return its ID
		private static async Task<CSteamID> CreateTestLobby(string name = "TestLobby", int maxPlayers = 4)
		{
			var created = false;
			CSteamID lobbyId = CSteamID.Nil;
			var callResult = CallResult<LobbyCreated_t>.Create((result, failure) =>
			{
				if (result.m_eResult == EResult.k_EResultOK)
				{
					lobbyId = (CSteamID)result.m_ulSteamIDLobby;
					created = true;
				}
			});

			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeInvisible, maxPlayers);
			callResult.Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeInvisible, maxPlayers));

			var timeout = DateTime.UtcNow.AddSeconds(10);
			while (!created && DateTime.UtcNow < timeout)
			{
				PumpSteamCallbacks(100);
				await Task.Delay(100);
			}

			if (!created)
				throw new TimeoutException("Failed to create test lobby.");

			// Set some metadata to make it identifiable
			SteamMatchmaking.SetLobbyData(lobbyId, "name", name);
			return lobbyId;
		}

		// Helper to delete a lobby (leave it, which destroys if owner)
		private static void DeleteTestLobby(CSteamID lobbyId)
		{
			if (lobbyId.IsValid())
			{
				SteamMatchmaking.LeaveLobby(lobbyId);
				PumpSteamCallbacks(500); // allow time for cleanup
			}
		}

		// Reflection helper to set private field LastSeenUtc on SteamLobbyData
		private static void SetLastSeenUtc(SteamLobbyData data, DateTime lastSeen)
		{
			var field = typeof(SteamLobbyData).GetField("_lastSeenTime", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("_lastSeenTime field not found.");
			field.SetValue(data, lastSeen);
		}

		[Test]
		public void Constructor_WithValidCVar_DoesNotThrow()
		{
			Assert.DoesNotThrow(() => new SteamLobbyRepository(_cvarSystem));
		}

		[Test]
		public void Constructor_MissingCVar_ThrowsCVarMissing()
		{
			var logger = new MockLogger();
			var eventFactory = new GameEventRegistry(logger);
			var badCVarSystem = new MockCVarSystem(eventFactory);
			Assert.Throws<CVarMissing>(() => new SteamLobbyRepository(badCVarSystem));
		}

		[Test]
		public async Task AddLobby_NewLobby_AddsToCollection()
		{
			// Arrange
			CSteamID lobbyId = await CreateTestLobby();
			try
			{
				// Act
				_repository.AddLobby(new SteamLobbyKey(lobbyId, Guid.NewGuid()));

				// Assert
				Assert.That(_repository.Lobbies, Has.Count.EqualTo(1));
				var added = _repository.Lobbies.First();
				using (Assert.EnterMultipleScope())
				{
					Assert.That(added.Id, Is.EqualTo(lobbyId));
					Assert.That(added.Name, Is.EqualTo("TestLobby")); // from metadata
				}
			}
			finally
			{
				DeleteTestLobby(lobbyId);
			}
		}

		[Test]
		public async Task AddLobby_ExistingLobby_UpdatesLastSeen()
		{
			// Arrange
			CSteamID lobbyId = await CreateTestLobby();
			try
			{
				var key = new SteamLobbyKey(lobbyId, Guid.NewGuid());
				_repository.AddLobby(key);
				var lobbyData = _repository.Lobbies.First();
				var originalLastSeen = lobbyData.LastSeenUtc;

				// Wait a bit and force an update
				await Task.Delay(100);
				_repository.AddLobby(key); // should update

				// Assert
				var newLastSeen = lobbyData.LastSeenUtc;
				Assert.That(newLastSeen, Is.GreaterThan(originalLastSeen));
			}
			finally
			{
				DeleteTestLobby(lobbyId);
			}
		}

		[Test]
		public async Task RemoveStaleLobbies_RemovesLobbiesOlderThanPurgeInterval()
		{
			// Arrange
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_PURGE_INTERVAL, 10); // 10 seconds
			_repository = new SteamLobbyRepository(_cvarSystem); // recreate with new interval

			CSteamID lobbyId = await CreateTestLobby();
			try
			{
				_repository.AddLobby(new SteamLobbyKey(lobbyId, Guid.NewGuid()));
				Assert.That(_repository.Lobbies, Has.Count.EqualTo(1));

				// Manually set LastSeenUtc to be older than purge interval
				var lobbyData = _repository.Lobbies.First();
				SetLastSeenUtc(lobbyData, DateTime.UtcNow.AddSeconds(-15));

				// Act
				_repository.RemoveStaleLobbies();

				// Assert
				Assert.That(_repository.Lobbies, Is.Empty);
			}
			finally
			{
				DeleteTestLobby(lobbyId);
			}
		}

		[Test]
		public async Task RemoveStaleLobbies_KeepsRecentLobbies()
		{
			// Arrange
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_PURGE_INTERVAL, 10);
			_repository = new SteamLobbyRepository(_cvarSystem);

			CSteamID lobbyId = await CreateTestLobby();
			try
			{
				_repository.AddLobby(new SteamLobbyKey(lobbyId, Guid.NewGuid()));
				// LastSeenUtc is recent (now)

				// Act
				_repository.RemoveStaleLobbies();

				// Assert
				Assert.That(_repository.Lobbies, Has.Count.EqualTo(1));
			}
			finally
			{
				DeleteTestLobby(lobbyId);
			}
		}

		[Test]
		public async Task Timer_TriggersRemoveStaleLobbies()
		{
			// Arrange
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_PURGE_INTERVAL, 2); // 2 seconds
			_repository = new SteamLobbyRepository(_cvarSystem);

			CSteamID lobbyId = await CreateTestLobby();
			try
			{
				_repository.AddLobby(new SteamLobbyKey(lobbyId, Guid.NewGuid()));

				// Force LastSeen to be old
				var lobbyData = _repository.Lobbies.First();
				SetLastSeenUtc(lobbyData, DateTime.UtcNow.AddSeconds(-5));

				// Wait for timer to fire (plus a bit)
				await Task.Delay(2000);
				PumpSteamCallbacks(1000); // allow any Steam callbacks but timer is .NET, not Steam

				// Assert: lobby should be purged
				Assert.That(_repository.Lobbies, Is.Empty);
			}
			finally
			{
				DeleteTestLobby(lobbyId);
			}
		}

		[Test]
		public async Task Dispose_StopsTimer()
		{
			// Arrange
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_PURGE_INTERVAL, 1);
			_repository = new SteamLobbyRepository(_cvarSystem);

			CSteamID lobbyId = await CreateTestLobby();
			try
			{
				_repository.AddLobby(new SteamLobbyKey(lobbyId, Guid.NewGuid()));
				var lobbyData = _repository.Lobbies.First();
				SetLastSeenUtc(lobbyData, DateTime.UtcNow.AddSeconds(-5));

				// Act
				_repository.Dispose();

				// Wait longer than purge interval
				await Task.Delay(3000);

				// Assert: lobby still present because timer was disposed
				Assert.That(_repository.Lobbies, Has.Count.EqualTo(1));
			}
			finally
			{
				// Ensure we clean up the lobby even if test fails
				if (!lobbyId.IsValid())
					DeleteTestLobby(lobbyId);
			}
		}
	}
}
