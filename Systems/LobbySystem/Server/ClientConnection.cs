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

using NomadCore.Systems.EventSystem;
using Steamworks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NomadCore.Systems.Steam;
using System;
using NomadCore.Abstractions.Services;
using NomadCore.Interfaces.EventSystem;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	ClientConnection
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class ClientConnection {
		/// <summary>
		/// The current state of the connection
		/// </summary>
		public ConnectionStatus Status { get; private set; } = ConnectionStatus.None;

		/// <summary>
		/// The current connection handle.
		/// </summary>
		public HSteamNetConnection Connection { get; private set; }

		public delegate void ProcessIncomingPacket( in ReadOnlySpan<byte> buffer, CSteamID senderId );

		private SteamNetworkingIdentity _identity;

		private readonly ILoggerService _logger;

		/*
		===============
		ClientConnection
		===============
		*/
		/// <summary>
		/// Creates a connection with all other lobby members present
		/// </summary>
		public ClientConnection( ILoggerService logger ) {
			ArgumentNullException.ThrowIfNull( logger );

			ConnectToLobbyMembers();
			_logger = logger;

			ConnectionManager.ConnectionStatusChanged.Subscribe( this, OnConnectionStatusChanged );
		}

		/*
		===============
		Close
		===============
		*/
		/// <summary>
		/// Closes the established steam socket connection.
		/// </summary>
		/// <param name="reason">The reason for closing the connection, defaults to "Leaving Lobby".</param>
		public void Close( string? reason = "Leaving Lobby" ) {
			ArgumentException.ThrowIfNullOrEmpty( reason );

			_logger.PrintLine( $"ClientConnection.Close: closing connection for reason of '{reason}'." );
			if ( !SteamNetworkingSockets.CloseConnection( Connection, 0, reason, false ) ) {
				_logger.PrintWarning( $"ClientConnection.Close: SteamNetworkingSockets.CloseConnection returned false." );
			}
		}

		/*
		===============
		PollMessages
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="messages"></param>
		/// <param name="callback"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PollMessages( in IntPtr[] messages, ProcessIncomingPacket? callback ) {
			int messageCount = SteamNetworkingSockets.ReceiveMessagesOnConnection( Connection, messages, messages.Length );

			for ( int i = 0; i < messageCount; i++ ) {
				try {
					ProcessPacketBuffer( out ReadOnlySpan<byte> buffer, in messages[ i ], out CSteamID senderId );
					callback?.Invoke( in buffer, senderId );
				}
				finally {
					SteamNetworkingMessage_t.Release( messages[ i ] );
				}
			}
		}

		/*
		===============
		ConnectToLobbyMembers
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void ConnectToLobbyMembers() {
			for ( int i = 0; i < LobbyManager.Current.MemberCount; i++ ) {
				User user = LobbyManager.Current.Players[ i ];
				if ( user.UserID == (CSteamID)SteamManager.SteamID.Value ) {
					continue; // don't connect to ourselves
				}

				// skip if already connected or connecting
				switch ( Status ) {
					case ConnectionStatus.Connected:
						_logger.PrintLine( $"ClientConnection.ConnectToLobbyMembers: already connected to '{user.UserName}'" );
						break;
					case ConnectionStatus.Pending:
						_logger.PrintLine( $"ClientConnection.ConnectToLobbyMembers: already connecting to '{user.UserName}'" );
						break;
					case ConnectionStatus.None:
						EstablishP2PClientConnection( in user );
						break;
				}
			}
		}

		/*
		===============
		ProcessPacketBuffer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="messagePtr"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private unsafe void ProcessPacketBuffer( out ReadOnlySpan<byte> buffer, in IntPtr messagePtr, out CSteamID id ) {
			SteamNetworkingMessage_t message = Marshal.PtrToStructure<SteamNetworkingMessage_t>( messagePtr );
			id = message.m_identityPeer.GetSteamID();
			buffer = new ReadOnlySpan<byte>( message.m_pData.ToPointer(), message.m_cbSize );
		}

		/*
		===============
		EstablishP2PClientConnection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		private void EstablishP2PClientConnection( in User user ) {
			_identity = new SteamNetworkingIdentity();
			_identity.SetSteamID( user.UserID );

			Connection = SteamNetworkingSockets.ConnectP2P( ref _identity, 0, ConnectionManager.SocketOptions.Length, ConnectionManager.SocketOptions );
			if ( Connection != HSteamNetConnection.Invalid ) {
				Status = ConnectionStatus.Pending;
				_logger.PrintLine( $"ClientConnection.ConnectToLobbyMembers: connecting to {user.UserName}." );
			} else {
				Status = ConnectionStatus.Disconnected;
				_logger.PrintError( $"ClientConnection.ConnectToLobbyMembers: failed to create connection with client {user.UserName}!" );
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
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="InvalidCastException"></exception>
		private void OnConnectionStatusChanged( in ConnectionStatusChangedEventData args ) {
			if ( args is  connection ) {
				string remoteName = SteamFriends.GetFriendPersonaName( args.RemoteID );
				_logger.PrintLine( $"ClientConnection.OnConnectionStatusChanged: client '{remoteName}' connection status changed to '{connection.Status}'" );

				switch ( args.Status ) {
					case ConnectionStatus.Connected:
						HandleConnectionStatusChangedConnected( args.RemoteID, remoteName, in args.Connection );
						break;
					case ConnectionStatus.Pending:
						HandleConnectionStatusChangedConnecting( args.RemoteID, remoteName, in args.Connection );
						break;
					case ConnectionStatus.Disconnected:
						throw new InvalidOperationException( "ConnectionStatus.Disconnected provided but not using the proper event args!" );
					default:
						throw new ArgumentOutOfRangeException( nameof( args.Status ) );
				}
				Status = connection.Status;
			} else if ( args is ConnectionManager.ConnectionClosedEventData closedData ) {

			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		private void OnConnectionClosed( in ConnectionClosed ) {
		}

		/*
		===============
		HandleConnectionStatusChangedConnected
		===============
		*/
		private void HandleConnectionStatusChangedConnected( CSteamID remoteId, string? remoteName, in HSteamNetConnection connection ) {
			_logger.PrintLine( $"ClientConnection.HandleConnectionStatusChangedConnected: successfully connected to peer {remoteName}" );
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
		/// <param name="remoteName"></param>
		/// <param name="connection"></param>
		private void HandleConnectionStatusChangedConnecting( CSteamID remoteId, string? remoteName, in HSteamNetConnection connection ) {
			ArgumentException.ThrowIfNullOrEmpty( remoteName );

			_logger.PrintLine( $"ClientConnection.HandleConnectionStatusChangedConnecting: incoming connection request from {remoteName}" );

			if ( Status != ConnectionStatus.Pending ) {
				if ( SteamNetworkingSockets.AcceptConnection( connection ) == EResult.k_EResultOK ) {
					Status = ConnectionStatus.Connected;
					_logger.PrintLine( "ClientConnection.HandleConnectionStatusChangedConnecting: accepted incoming connection" );
				} else {
					Status = ConnectionStatus.Disconnected;
					_logger.PrintError( "ClientConnection.HandleConnectionStatusChangedConnecting: failed to accept incoming connection!" );
					Close( "Connectection accept failed" );
				}
			} else {
				Status = ConnectionStatus.Disconnected;
				_logger.PrintLine( "[STEAM] Ignoring outgoing connection request (we initiated it)" );
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
		/// <param name="remoteName"></param>
		/// <param name="debugMessage"></param>
		/// <param name="connection"></param>
		private void HandleConnectionStatusChangedDisconnected( string? remoteName, string? debugMessage, in HSteamNetConnection connection ) {
			ArgumentException.ThrowIfNullOrEmpty( remoteName );
			ArgumentException.ThrowIfNullOrEmpty( debugMessage );

			_logger.PrintLine( $"ConnectionManager.HandleConnectionStatusChangedDisconnected: connection closed with {remoteName}" );
			_logger.PrintLine( $"ConnectionManager.HandleConnectionStatusChangedDisconnected: reason: {debugMessage}" );

			if ( Status == ConnectionStatus.Connected ) {
				SteamNetworkingSockets.CloseConnection( connection, 0, debugMessage, false );
			} else {
			}
			Status = ConnectionStatus.Disconnected;
		}
	};
};