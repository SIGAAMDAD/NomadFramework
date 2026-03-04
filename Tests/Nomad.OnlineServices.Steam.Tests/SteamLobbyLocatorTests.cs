using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Steamworks;
using Nomad.Core.CVars;
using Nomad.OnlineServices.Steam.Private.Repositories;
using Nomad.OnlineServices.Steam.Private.Services.LobbyServices;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Nomad.Core.Events;
using Nomad.Core.OnlineServices;
using Nomad.Core.Exceptions;

namespace Nomad.OnlineServices.Steam.Tests
{
	[TestFixture]
	public class SteamLobbyLocatorTests
	{
		private SteamLobbyRepository _repository;
		private SteamAppData _appData;
		private MockCVarSystem _cvarSystem;
		private SteamLobbyLocator _locator;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			// Initialize Steam once for all tests
			Environment.SetEnvironmentVariable("SteamAppId", "480");
			Environment.SetEnvironmentVariable("SteamGameId", "480");

			var initResult = SteamAPI.InitEx(out string errorMsg);
			if (initResult != ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
			{
				Assert.Inconclusive($"Could not initialize SteamAPI: {initResult} - {errorMsg}");
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			SteamAPI.Shutdown();
		}

		[SetUp]
		public void SetUp()
		{
			_cvarSystem = new MockCVarSystem();
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_UDDATE_INTERVAL, 5000); // 5 seconds
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_PURGE_INTERVAL, 5000); // 5 seconds
			_repository = new SteamLobbyRepository(_cvarSystem);
			_appData = new SteamAppData { AppId = new AppId_t(480) }; // adjust as needed

			_locator = new SteamLobbyLocator(_repository, _appData, _cvarSystem);
		}

		[TearDown]
		public void TearDown()
		{
			_repository?.Dispose();
			_cvarSystem?.Dispose();
			_locator?.Dispose();
			// Pump any remaining callbacks to clean up
			PumpSteamCallbacks(100);
		}

		// Helper to pump Steam callbacks while waiting for a task
		private static async Task PumpWhileTaskRunning(Task task, int timeoutMs = 10000)
		{
			using var cts = new CancellationTokenSource(timeoutMs);
			var pumpTask = Task.Run(async () =>
			{
				while (!task.IsCompleted && !cts.Token.IsCancellationRequested)
				{
					SteamAPI.RunCallbacks();
					await Task.Delay(10, cts.Token);
				}
			}, cts.Token);

			await Task.WhenAny(task, pumpTask);
			if (!task.IsCompleted)
			{
				throw new TimeoutException("Steam operation timed out.");
			}
			await task; // propagate any exception
		}

		private static void PumpSteamCallbacks(int milliseconds)
		{
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			while (stopwatch.ElapsedMilliseconds < milliseconds)
			{
				SteamAPI.RunCallbacks();
				Thread.Sleep(10);
			}
		}

		[Test]
		public void Constructor_WithValidCVar_DoesNotThrow()
		{
			Assert.DoesNotThrow(() => new SteamLobbyLocator(_repository, _appData, _cvarSystem));
		}

		[Test]
		public void Constructor_MissingCVar_ThrowsCVarMissing()
		{
			var badCVarSystem = new MockCVarSystem(); // no CVar set
			Assert.Throws<CVarMissing>(() => new SteamLobbyLocator(_repository, _appData, badCVarSystem));
		}

		[Test]
		public async Task RequestLobbyListAsync_Default_ReturnsLobbiesAndUpdatesRepository()
		{
			// Act
			var lobbies = await RequestLobbyListWithPump(ServerRange.NoLimit);

			using (Assert.EnterMultipleScope())
			{
				// Assert
				Assert.That(lobbies, Is.Not.Null);
				Assert.That(_repository.Lobbies, Is.EqualTo(lobbies));
			}
			// Verify last fetch time was updated (internal state not exposed, but we can check that subsequent calls use cache)
			// Instead, we'll test cache behavior in another test
		}

		[Test]
		public async Task RequestLobbyListAsync_WithDifferentRanges_SetsDistanceFilter(
			[Values(ServerRange.LAN, ServerRange.Region, ServerRange.Continental, ServerRange.NoLimit)] ServerRange range)
		{
			// Act
			var lobbies = await RequestLobbyListWithPump(range);

			using (Assert.EnterMultipleScope())
			{
				// Assert - we can't directly verify the filter, but ensure no exception and result is a list
				Assert.That(lobbies, Is.Not.Null);
				// Optionally check that the repository was updated
				Assert.That(_repository.Lobbies, Is.EqualTo(lobbies));
			}
		}

		[Test]
		public async Task FindLobbiesWithParams_WhenCacheValid_ReturnsCached()
		{
			// Arrange: perform an initial fetch
			var initialLobbies = await RequestLobbyListWithPump(ServerRange.NoLimit);
			var lastFetchTime = GetPrivateField<DateTime>(_locator, "_lastFetchTime");

			// Wait a bit but less than update interval
			await Task.Delay(1000);

			// Act: call FindLobbiesWithParams with same range
			var info = new MatchMakingInfo { Range = ServerRange.NoLimit };
			var result = await _locator.FindLobbiesWithParams(info, CancellationToken.None);

			// Assert: should return same collection without new fetch
			Assert.That(result, Is.EqualTo(initialLobbies));
			var newFetchTime = GetPrivateField<DateTime>(_locator, "_lastFetchTime");
			Assert.That(newFetchTime, Is.EqualTo(lastFetchTime)); // timestamp unchanged
		}

		[Test]
		public async Task FindLobbiesWithParams_WhenCacheStale_FetchesNew()
		{
			// Arrange: set a very short update interval (e.g., 1ms) to force staleness
			_cvarSystem.SetCVar(Constants.CVars.LOBBY_UDDATE_INTERVAL, 1);
			_locator = new SteamLobbyLocator(_repository, _appData, _cvarSystem);

			var initialLobbies = await RequestLobbyListWithPump(ServerRange.NoLimit);
			var lastFetchTime = GetPrivateField<DateTime>(_locator, "_lastFetchTime");

			// Wait just over the interval
			await Task.Delay(5);

			// Act
			var info = new MatchMakingInfo { Range = ServerRange.NoLimit };
			var result = await FindLobbiesWithParamsWithPump(info);

			// Assert: should fetch again
			Assert.That(result, Is.Not.Null);
			var newFetchTime = GetPrivateField<DateTime>(_locator, "_lastFetchTime");
			Assert.That(newFetchTime, Is.GreaterThan(lastFetchTime));
		}

		[Test]
		public async Task FindLobbiesWithParams_WhenRangeChanged_FetchesNew()
		{
			// Arrange: first fetch with NoLimit
			await RequestLobbyListWithPump(ServerRange.NoLimit);
			var lastFetchTime = GetPrivateField<DateTime>(_locator, "_lastFetchTime");
			var lastRange = GetPrivateField<ServerRange>(_locator, "_lastRange");
			Assert.That(lastRange, Is.EqualTo(ServerRange.NoLimit));

			// Act: request with different range
			var info = new MatchMakingInfo { Range = ServerRange.Region };
			var result = await FindLobbiesWithParamsWithPump(info);

			// Assert: new fetch occurred
			var newFetchTime = GetPrivateField<DateTime>(_locator, "_lastFetchTime");
			Assert.That(newFetchTime, Is.GreaterThan(lastFetchTime));
			var newRange = GetPrivateField<ServerRange>(_locator, "_lastRange");
			Assert.That(newRange, Is.EqualTo(ServerRange.Region));
		}

		[Test]
		public void Cancellation_ThrowsTaskCanceledException()
		{
			var cts = new CancellationTokenSource();
			cts.Cancel();

			Assert.ThrowsAsync<TaskCanceledException>(async () =>
			{
				await _locator.RequestLobbyListAsync(ServerRange.NoLimit, cts.Token);
			});
		}

		// Helper methods to invoke async methods with callback pumping
		private async Task<ICollection<SteamLobbyData>> RequestLobbyListWithPump(ServerRange range, CancellationToken ct = default)
		{
			var task = _locator.RequestLobbyListAsync(range, ct);
			await PumpWhileTaskRunning(task);
			return await task;
		}

		private async Task<ICollection<SteamLobbyData>> FindLobbiesWithParamsWithPump(MatchMakingInfo info, CancellationToken ct = default)
		{
			var task = _locator.FindLobbiesWithParams(info, ct);
			await PumpWhileTaskRunning(task);
			return await task;
		}

		// Reflection helper to read private fields (for validation)
		private static T GetPrivateField<T>(object obj, string fieldName)
		{
			var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return (T)field!.GetValue(obj);
		}

		// Mock CVar system
		private class MockCVarSystem : ICVarSystemService
		{
			private readonly Dictionary<string, object> _cvars = new();

			public void SetCVar<T>(string name, T value) where T : notnull
			{
				_cvars[name] = value!;
			}

			public ICVar<T>? GetCVar<T>(string name) where T : notnull
			{
				if (_cvars.TryGetValue(name, out var val) && val is T tVal)
				{
					return new MockCVar<T>(name, tVal);
				}
				return null;
			}

			public void Register(ICVar cvar)
			{
				throw new NotImplementedException();
			}

			public ICVar<T> Register<T>(in CVarCreateInfo<T> createInfo)
			{
				throw new NotImplementedException();
			}

			public void Unregister(ICVar cvar)
			{
				throw new NotImplementedException();
			}

			public bool CVarExists(string name)
			{
				throw new NotImplementedException();
			}

			public bool CVarExists<T>(string name)
			{
				throw new NotImplementedException();
			}

			public ICVar? GetCVar(string name)
			{
				throw new NotImplementedException();
			}

			public ICVar[]? GetCVars()
			{
				throw new NotImplementedException();
			}

			public ICVar<T>[]? GetCVarsWithValueType<T>()
			{
				throw new NotImplementedException();
			}

			public ICVar[]? GetCVarsInGroup(string groupName)
			{
				throw new NotImplementedException();
			}

			public bool GroupExists(string groupName)
			{
				throw new NotImplementedException();
			}

			public void Restart()
			{
				throw new NotImplementedException();
			}

			public void Load(string configFile)
			{
				throw new NotImplementedException();
			}

			public void Save(string configFile)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}

			private class MockCVar<T> : ICVar<T> where T : notnull
			{
				public string Name { get; }
				public T Value { get; set; }

				public T DefaultValue => throw new NotImplementedException();

				public IGameEvent<CVarValueChangedEventArgs<T>> ValueChanged => throw new NotImplementedException();

				public string Description => throw new NotImplementedException();

				public CVarType Type => throw new NotImplementedException();

				public CVarFlags Flags => throw new NotImplementedException();

				public bool IsSaved => throw new NotImplementedException();

				public bool IsReadOnly => throw new NotImplementedException();

				public bool IsUserCreated => throw new NotImplementedException();

				public bool IsHidden => throw new NotImplementedException();

				public MockCVar(string name, T value) { Name = name; Value = value; }
				public void Reset() { }
				public void SetFromString(string str) { }

				public float GetDecimalValue()
				{
					throw new NotImplementedException();
				}

				public int GetIntegerValue()
				{
					throw new NotImplementedException();
				}

				public uint GetUIntegerValue()
				{
					throw new NotImplementedException();
				}

				public string? GetStringValue()
				{
					throw new NotImplementedException();
				}

				public bool GetBooleanValue()
				{
					throw new NotImplementedException();
				}

				public void SetDecimalValue(float value)
				{
					throw new NotImplementedException();
				}

				public void SetIntegerValue(int value)
				{
					throw new NotImplementedException();
				}

				public void SetUIntegerValue(uint value)
				{
					throw new NotImplementedException();
				}

				public void SetBooleanValue(bool value)
				{
					throw new NotImplementedException();
				}

				public void SetStringValue(string value)
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}