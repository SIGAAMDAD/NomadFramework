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
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.Exceptions;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Nomad.CVars;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services {
	/*
	===================================================================================

	SteamLobbyFactory

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamLobbyFactory : IDisposable {
		private readonly SteamAsyncCallbackDispatcher<LobbyCreated_t, SteamLobbyData> _lobbyCreated;

		private readonly ILoggerCategory _category;

		private readonly SteamUserData _userData;

		private readonly int _maxPlayers;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyFactory
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="logger"></param>
		/// <param name="cvarSystem"></param>
		public SteamLobbyFactory( SteamUserData userData, ILoggerService logger, ICVarSystemService cvarSystem ) {
			ArgumentGuard.ThrowIfNull( logger );

			_userData = userData;
			_category = logger.CreateCategory( nameof( SteamLobbyFactory ), LogLevel.Info, true );

			var maxPlayers = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.LOBBY_MAX_CLIENTS );
			_maxPlayers = maxPlayers.Value;

			_lobbyCreated = new SteamAsyncCallbackDispatcher<LobbyCreated_t, SteamLobbyData>();
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			if ( !_isDisposed ) {
				_lobbyCreated?.Dispose();

				_category?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		CreateLobby
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<SteamLobbyData?> CreateLobby( LobbyInfo info, CancellationToken ct = default ) {
			if ( info.MaxPlayers < 1 || info.MaxPlayers > _maxPlayers ) {
				throw new ArgumentOutOfRangeException( nameof( info ), "LobbyInfo.MaxPlayers is less than 1 or greater than MaxPlayers!" );
			}

			ELobbyType type = info.Visibility switch {
				LobbyVisibility.Private => ELobbyType.k_ELobbyTypePrivate,
				LobbyVisibility.Public => ELobbyType.k_ELobbyTypePublic,
				LobbyVisibility.FriendsOnly => ELobbyType.k_ELobbyTypeFriendsOnly,
				_ => throw new ArgumentOutOfRangeException( nameof( info ) )
			};

			return await _lobbyCreated.Invoke(
				result => {
					if ( result.m_eResult != EResult.k_EResultOK ) {
						_category.PrintError( $"SteamLobbyFactory.OnLobbyCreated: error creating lobby - {result.m_eResult}" );
						return null;
					}
					_category.PrintLine( $"SteamLobbyFactory.OnLobbyFactory: created new lobby with CSteamID '{result.m_ulSteamIDLobby}'" );

					CSteamID id = (CSteamID)result.m_ulSteamIDLobby;

					// setup default metadata
					SteamMatchmaking.SetLobbyOwner( id, _userData.UserID );
					SteamMatchmaking.SetLobbyMemberLimit( id, info.MaxPlayers );
					SteamMatchmaking.SetLobbyJoinable( id, true );

					SteamMatchmaking.SetLobbyData( id, nameof( LobbyInfo.Name ), info.Name );
					SteamMatchmaking.SetLobbyData( id, nameof( LobbyInfo.Map ), info.Map );
					SteamMatchmaking.SetLobbyData( id, nameof( LobbyInfo.GameMode ), info.GameMode );
					SteamMatchmaking.SetLobbyData( id, nameof( LobbyInfo.Visibility ), info.Visibility.ToString() );

					foreach ( var metadata in info.Metadata ) {
						SteamMatchmaking.SetLobbyData( id, metadata.Key, metadata.Value );
					}

					var data = new SteamLobbyData( id, info, Guid.NewGuid() );
					data.Update();

					return data;
				},
				() => SteamMatchmaking.CreateLobby( type, info.MaxPlayers ),
				ct
			);
		}
	};
};