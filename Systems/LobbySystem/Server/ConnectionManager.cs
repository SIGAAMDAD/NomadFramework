/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Interfaces;
using NomadCore.Systems.EventSystem;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	ConnectionManager
	
	===================================================================================
	*/
	/// <summary>
	/// Handles the connection and disconnection process between two clients in a multiplayer lobby.
	/// The multiplayer menu system should never have a direct reason to reach into this API unless its for events.
	/// </summary>

	public sealed class ConnectionManager {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct ConnectionClosedEventData : IEventArgs {
			public readonly CSteamID RemoteID;
			public readonly string? Reason;
			public readonly HSteamNetConnection Connection;

			/*
			===============
			ConnectionClosedEventData
			===============
			*/
			public ConnectionClosedEventData( CSteamID remoteId, string? reason, in HSteamNetConnection conn ) {
				ArgumentException.ThrowIfNullOrEmpty( reason );

				RemoteID = remoteId;
				Reason = reason;
				Connection = conn;
			}
		};

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct ConnectionStatusChangedEventData( CSteamID remoteId, ConnectionStatus status, in HSteamNetConnection conn ) : IEventArgs {
			public readonly CSteamID RemoteID = remoteId;
			public readonly ConnectionStatus Status = status;
			public readonly HSteamNetConnection Connection = conn;
		};

		public static readonly Dictionary<User, ClientConnection> EstablishedConnections = new Dictionary<User, ClientConnection>();

		public static readonly SteamNetworkingConfigValue_t[] SocketOptions;

		private HSteamListenSocket ListenSocket;

		private static ConnectionManager Instance;

		private readonly CallResult<SteamNetConnectionStatusChangedCallback_t> ConnectionStatusChangedCallResult;

		public readonly static GameEvent ConnectionStatusChanged = new GameEvent( nameof( ConnectionStatusChanged ) );

		/*
		===============
		ConnectionManager
		===============
		*/
		static ConnectionManager() {
			SocketOptions = [
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 64 * 1024 }
				},
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendRateMin,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 64000 }
				},
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_RecvBufferSize,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 64 * 1024 }
				},
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_NagleTime,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 0 }
				},
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_TimeoutInitial,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 100000 }
				},
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_TimeoutConnected,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 1000000 }
				},
				new SteamNetworkingConfigValue_t {
					m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SymmetricConnect,
					m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
					m_val = new SteamNetworkingConfigValue_t.OptionValue { m_int32 = 1 }
				}
			];
		}

		/*
		===============
		ConnectionManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public ConnectionManager() {
			LobbyManager.LobbyCreated.Subscribe( this, OnCreateListenSocket );
			LobbyManager.LobbyLeft.Subscribe( this, OnCloseConnections );

			ConnectionStatusChangedCallResult = CallResult<SteamNetConnectionStatusChangedCallback_t>.Create( OnConnectionStatusChanged );
		}

		/*
		===============
		GetConnection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool GetConnection( in User user, out ClientConnection? connection ) {
			return EstablishedConnections.TryGetValue( user, out connection );
		}

		/*
		===============
		GetConnectionStatus
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static ConnectionStatus GetConnectionStatus( ESteamNetworkingConnectionState state ) {
			return state switch {
				ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None => ConnectionStatus.None,
				ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting or ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute => ConnectionStatus.Pending,
				ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected => ConnectionStatus.Connected,
				ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer or ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally => ConnectionStatus.Disconnected,
				_ => throw new ArgumentOutOfRangeException( nameof( state ) )
			};
		}

		/*
		===============
		OnCreateListenSocket
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnCreateListenSocket( in LobbyManager.LobbyStatusChangedEventData args ) {
			if ( ListenSocket != HSteamListenSocket.Invalid ) {
				SteamNetworkingSockets.CloseListenSocket( ListenSocket );
			}

			ListenSocket = SteamNetworkingSockets.CreateListenSocketP2P( 0, SocketOptions.Length, SocketOptions );
			if ( ListenSocket == HSteamListenSocket.Invalid ) {
				ConsoleSystem.Console.PrintError( "ConnectionManager.OnCreateListenSocket: failed to create listen socket" );
			} else {
				ConsoleSystem.Console.PrintLine( "ConnectionManager.OnCreateListenSocket: created listen socket successfully" );
			}
		}

		/*
		===============
		OnCloseConnections
		===============
		*/
		/// <summary>
		/// Closes and clears all connection data.
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnCloseConnections( in LobbyManager.LobbyStatusChangedEventData args ) {
			foreach ( var conn in EstablishedConnections ) {
				conn.Value.Close( "Leaving Lobby" );
			}
			CloseListenSocket();
		}

		/*
		===============
		CloseListenSocket
		===============
		*/
		private void CloseListenSocket() {
			if ( ListenSocket != HSteamListenSocket.Invalid ) {
				SteamNetworkingSockets.CloseListenSocket( ListenSocket );
				ListenSocket = HSteamListenSocket.Invalid;
			}
		}

		/*
		===============
		OnConnectionStatusChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="status"></param>
		/// <param name="bIOFailure"></param>
		private void OnConnectionStatusChanged( SteamNetConnectionStatusChangedCallback_t status, bool bIOFailure ) {
			ConsoleSystem.Console.PrintLine( $"ConnectionManager.OnConnectionStatusChanged: status changed - '{status.m_info.m_eState}'" );

			try {
				CSteamID remoteId = status.m_info.m_identityRemote.GetSteamID();
				string remoteName = SteamFriends.GetFriendPersonaName( remoteId );

				switch ( status.m_info.m_eState ) {
					case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
						HandleConnectionStatusChangedConnecting( remoteId, in status.m_hConn );
						break;
					case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
						HandleConnectionStatusChangedConnected( remoteId, in status.m_hConn );
						break;
					case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
					case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
						HandleConnectionStatusChangedDisconnected( remoteId, status.m_info.m_szEndDebug, in status.m_hConn );
						break;
					default:
						ConsoleSystem.Console.PrintWarning( $"ConnectionManager.OnConnectionStatusChanged: unhandled connection state - '{status.m_info.m_eState}'" );
						break;
				}
			} catch ( Exception e ) {
				ConsoleSystem.Console.PrintError( $"ConnectionManager.OnConnectionStatusChanged: error in connection callback - '{e.Message}'" );
			}
		}

		/*
		===============
		HandleConnectionStatusChangedConnecting
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteId"></param>
		/// <param name="connection"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void HandleConnectionStatusChangedConnecting( CSteamID remoteId, in HSteamNetConnection connection ) {
			ConnectionStatusChanged.Publish( new ConnectionStatusChangedEventData( remoteId, ConnectionStatus.Pending, in connection ) );
		}

		/*
		===============
		HandleConnectionStatusChangedConnected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteId"></param>
		/// <param name="connection"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void HandleConnectionStatusChangedConnected( CSteamID remoteId, in HSteamNetConnection connection ) {
			ConnectionStatusChanged.Publish( new ConnectionStatusChangedEventData( remoteId, ConnectionStatus.Connected, in connection ) );

			if ( LobbyManager.Current.TryGetPlayer( remoteId, out User? user ) ) {
				ArgumentNullException.ThrowIfNull( user );

				ConsoleSystem.Console.PrintLine( "ConnectionManager.Sending" );
				MessageHandler.SendHandshake( in user );
			}
		}

		/*
		===============
		HandleConnectionStatusChangedDisconnected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteId"></param>
		/// <param name="debugMessage"></param>
		/// <param name="connection"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void HandleConnectionStatusChangedDisconnected( CSteamID remoteId, string? debugMessage, in HSteamNetConnection connection ) {
			ArgumentException.ThrowIfNullOrEmpty( debugMessage );

			ConnectionStatusChanged.Publish( new ConnectionClosedEventData( remoteId, debugMessage, in connection ) );
		}
	};
};