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
using NomadCore.Systems.Steam;
using System;
using NomadCore.Systems.LobbySystem.Server;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.LobbySystem {
	/*
	===================================================================================
	
	ActiveLobby
		
	===================================================================================
	*/
	/// <summary>
	/// Holds a lobby's data
	/// </summary>

	public sealed class ActiveLobby : Lobby {
		private ServerManager ServerManager;

		/*
		===============
		ActiveLobby
		===============
		*/
		/// <summary>
		/// Creates a lobby that was pulled from the lobby matchmaking list.
		/// </summary>
		/// <param name="id">The <see cref="CSteamID"/> of the lobby.</param>
		public ActiveLobby( CSteamID id ) {
			ServerManager = new ServerManager();
			Id = id;

			Refresh();
		}

		/*
		===============
		ActiveLobby
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="visibility">The visibility of the lobby.</param>
		/// <param name="maxPlayers"></param>
		/// <param name="name"></param>
		/// <param name="map"></param>
		/// <param name="gameMode"></param>
		public ActiveLobby( Visibility visibility, int maxPlayers, string? name, string? map, string? gameMode ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentException.ThrowIfNullOrEmpty( map );
			ArgumentException.ThrowIfNullOrEmpty( gameMode );

			ServerManager = new ServerManager();

			Visibility = visibility;
			MaxPlayers = maxPlayers;
			GameMode = gameMode;
			Name = name;
			Map = map;

			LobbyManager.LobbyCreated.Subscribe( this, OnCreated );
			LobbyManager.LobbyJoined.Subscribe( this, OnConnected );
			LobbyManager.LobbyLeft.Subscribe( this, Leave );
			LobbyManager.ClientLeftLobby.Subscribe( this, OnCheckOwnership );
		}

		/*
		===============
		OnConnected
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnConnected( in LobbyManager.LobbyStatusChangedEventData args ) {
			if ( IsHost ) {
				return;
			}

			ConsoleSystem.Console.PrintLine( "Lobby.OnConnected: sending joined command to server..." );
			CommandManager.SendCommand( CommandType.ConnectedToLobby );
		}

		/*
		===============
		Leave
		===============
		*/
		/// <summary>
		/// Leaves the lobby
		/// </summary>
		private void Leave( in LobbyManager.LobbyStatusChangedEventData args ) {
			ArgumentNullException.ThrowIfNull( Players );

			if ( !Id.IsValid() ) {
				ConsoleSystem.Console.PrintWarning( $"Lobby.Leave: lobby {Id} isn't valid!" );
				return;
			}

			//PlayersReady.Clear();

			ConsoleSystem.Console.PrintLine( $"Lobby.Leave: leaving lobby {Id}..." );

			if ( IsHostTransferValid() ) {
				for ( int i = 0; i < MemberCount; i++ ) {
					if ( Players[ i ].UserID.IsValid() && (ulong)Players[ i ].UserID != SteamManager.SteamID ) {
						AssignNewLeadership( Players[ i ].UserID );
						break;
					}
				}
			}

			// clear the data
			SteamMatchmaking.LeaveLobby( Id );
			Id = CSteamID.Nil;

			Dispose();
		}

		/*
		===============
		IsHostTransferValid
		===============
		*/
		/// <summary>
		/// Returns if the transfer of ownership is valid or not.
		/// </summary>
		/// <returns>True if we're the host and there's more than one member in the lobby.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private bool IsHostTransferValid() {
			return Id.IsValid() && Id.IsLobby() && MemberCount > 1 && IsHost;
		}

		/*
		===============
		CmdTransferOwnership
		===============
		*/
		/// <summary>
		/// Transfers lobby host status to the user provided with <paramref name="userName"/>.
		/// </summary>
		/// <param name="userName">The name of the user to give host status to.</param>
		private void CmdTransferOwnership( string? userName ) {
			ArgumentException.ThrowIfNullOrEmpty( userName );

			if ( !FindPlayer( userName, out int index ) ) {
				ConsoleSystem.Console.PrintLine( $"ActiveLobby.CmdTransferOwnership: no user found for name '{userName}' in current lobby." );
				return;
			}

			AssignNewLeadership( Players[ index ].UserID );
		}

		/*
		===============
		CmdDumpLobbyInfo
		===============
		*/
		private void CmdDumpLobbyInfo() {
			ArgumentNullException.ThrowIfNull( Players );

			ConsoleSystem.Console.PrintLine( "======== LOBBY INFORMATION ========" );
			ConsoleSystem.Console.PrintLine( $"Name: {Name}" );
			ConsoleSystem.Console.PrintLine( $"Map: {Map}" );
			ConsoleSystem.Console.PrintLine( $"GameMode: {GameMode}" );
			ConsoleSystem.Console.PrintLine( $"Host: " );
			ConsoleSystem.Console.PrintLine( $"MaxMembers: {MaxPlayers}" );
			ConsoleSystem.Console.PrintLine( $"MemberCount: {MemberCount}" );
			ConsoleSystem.Console.PrintLine( $"Members:" );
			for ( int i = 0; i < MemberCount; i++ ) {
				ConsoleSystem.Console.PrintLine( $"\t[{i}]: {Players[ i ].UserName}" );
			}
			ConsoleSystem.Console.PrintLine( "===================================" );
		}

		/*
		===============
		OnCheckOwnership
		===============
		*/
		/// <summary>
		/// Checks the current ownership of the lobby if we have more than one member and the host just left.
		/// </summary>
		private void OnCheckOwnership( in ClientConnectionChangedEventData args ) {
			if ( !IsHost || ( IsHost && (ulong)args.ClientID != SteamManager.SteamID ) ) {
				return;
			}

			ConsoleSystem.Console.PrintLine( $"Lobby.OnCheckOwnership: transferring ownership of lobby '{Id}' to " );
		}

		/*
		===============
		OnCreated
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnCreated( in LobbyManager.LobbyStatusChangedEventData args ) {
			if ( args.EventType != LobbyManager.EventType.Created ) {
				throw new InvalidOperationException( "Lobby.OnCreated called with an event that wasn't LobbyManager.EventType.Created" );
			}
			Id = args.Id;

			SteamMatchmaking.SetLobbyJoinable( Id, true );
			SteamMatchmaking.SetLobbyType( Id, LobbyManager.GetLobbyType( Visibility ) );
			SteamMatchmaking.SetLobbyData( Id, "name", Name );
			SteamMatchmaking.SetLobbyData( Id, "map", Map );
			SteamMatchmaking.SetLobbyData( Id, "mode", GameMode.ToString() );
		}

		/*
		===============
		AssignNewLeadership
		===============
		*/
		/// <summary>
		/// Assigns lobby host status to the provided user.
		/// </summary>
		/// <param name="newOwnerId">The user to give host status to.</param>
		private void AssignNewLeadership( CSteamID newOwnerId ) {
			ArgumentNullException.ThrowIfNull( Players );

			ConsoleSystem.Console.PrintLine( $"ActiveLobby.AssignNewLeadership: assinging lobby host status to steam user '{newOwnerId}'..." );
			SteamMatchmaking.SetLobbyOwner( Id, newOwnerId );
			CommandManager.SendCommand( CommandType.OwnershipChanged );

			// let steam process the request
			System.Threading.Thread.Sleep( 750 );
		}
	};
};