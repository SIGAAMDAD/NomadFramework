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
using Godot;
using NomadCore.Systems.ConsoleSystem;
using Steamworks;
using NomadCore.Systems.Steam;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.LobbySystem {
	/*
	===================================================================================
	
	Lobby
	
	===================================================================================
	*/
	/// <summary>
	/// The base class for the <see cref="ActiveLobby"/> and <see cref="ListedLobby"/> class.
	/// </summary>

	public abstract class Lobby : IDisposable {
		public CSteamID Id { get; protected set; } = CSteamID.Nil;
		public Visibility Visibility { get; protected set; } = Visibility.Public;
		public string? Name { get; protected set; } = "Unnamed";
		public string? Map { get; protected set; } = "Unselected";
		public CSteamID HostId { get; protected set; } = CSteamID.Nil;
		public int MaxPlayers { get; protected set; } = LobbyManager.MAX_LOBBY_MEMBERS;
		public string GameMode { get; protected set; } = "Unselected";

		public bool IsHost => (ulong)HostId == SteamManager.SteamID;
		public int MemberCount => Players.Count;

		public bool IsPublic => Visibility == Visibility.Public;
		public bool IsPrivate => Visibility == Visibility.Private;
		public bool IsFriendsOnly => Visibility == Visibility.FriendsOnly;

		/// <summary>
		/// The currently connected players in the lobby.
		/// </summary>
		public readonly List<User> Players = new List<User>( LobbyManager.MAX_LOBBY_MEMBERS );

		protected readonly ConsoleCommand LobbyInfo = new ConsoleCommand( nameof( LobbyInfo ), OnLobbyInfo );

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public virtual void Dispose() {
		}

		/*
		===============
		TryGetPlayer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool TryGetPlayer( CSteamID userId, out User? user ) {
			for ( int i = 0; i < Players.Count; i++ ) {
				if ( Players[ i ].UserID == userId ) {
					user = Players[ i ];
					return true;
				}
			}
			user = null;
			return false;
		}

		/*
		===============
		GetPlayerIndex
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public int GetPlayerIndex( CSteamID userId ) {
			for ( int i = 0; i < Players.Count; i++ ) {
				if ( Players[ i ].UserID == userId ) {
					return i;
				}
			}
			return -1;
		}

		/*
		===============
		ConnectSignals
		===============
		*/
		/// <summary>
		/// Connects the <see cref="LobbyManager.ClientJoinedLobby"/> and <see cref="LobbyManager.ClientLeftLobby"/> signals.
		/// </summary>
		protected void ConnectSignals() {
			LobbyManager.ClientJoinedLobby.Subscribe( this, OnAddClient );
			LobbyManager.ClientLeftLobby.Subscribe( this, OnRemoveClient );
		}

		/*
		===============
		FindPlayer
		===============
		*/
		/// <summary>
		/// Runs through the <see cref="Players"/> list and locates the player with the matching user id of <paramref name="targetId"/>.
		/// </summary>
		/// <param name="targetId">The user steam id to search for.</param>
		/// <param name="index">The index of the user, -1 if not found.</param>
		/// <returns>True if the user <paramref name="targetId"/> was valid and found in <see cref="Players"/>.</returns>
		protected bool FindPlayer( CSteamID targetId, out int index ) {
			// sanity
			ArgumentNullException.ThrowIfNull( Players );

			for ( int i = 0; i < MemberCount; i++ ) {
				if ( Players[ i ].UserID == targetId ) {
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}

		/*
		===============
		FindPlayer
		===============
		*/
		/// <summary>
		/// Runs through the <see cref="Players"/> list and locates the player with the matching user name of <paramref name="userName"/>.
		/// </summary>
		/// <param name="userName">The user name to search for.</param>
		/// <param name="index">The index of the user, -1 if not found.</param>
		/// <returns>True if the user <paramref name="userName"/> was valid and found in <see cref="Players"/>.</returns>
		protected bool FindPlayer( string? userName, out int index ) {
			// sanity
			ArgumentException.ThrowIfNullOrEmpty( userName );
			ArgumentNullException.ThrowIfNull( Players );

			for ( int i = 0; i < MemberCount; i++ ) {
				if ( Players[ i ].UserName == userName ) {
					index = 0;
					return true;
				}
			}
			index = -1;
			return false;
		}

		/*
		===============
		GetLobbyMembers
		===============
		*/
		/// <summary>
		/// Refreshes the currently held list of players in <see cref="Players"/>.
		/// </summary>
		protected void GetLobbyMembers() {
			ArgumentNullException.ThrowIfNull( Players );

			ConsoleSystem.Console.PrintLine( $"Lobby.GetLobbyMembers: fetching lobby member list..." );

			Players.Clear();
			Players.EnsureCapacity( SteamMatchmaking.GetNumLobbyMembers( Id ) );

			for ( int i = 0; i < MemberCount; i++ ) {
				Players.Add( new User( SteamMatchmaking.GetLobbyMemberByIndex( Id, i ) ) );
				ConsoleSystem.Console.PrintLine( $"Lobby.GetLobbyMembers: LobbyMember ID {Players[ i ].UserID} at index {i}" );
			}
		}

		/*
		===============
		Refresh
		===============
		*/
		/// <summary>
		/// Updates the lobby's information.
		/// </summary>
		protected void Refresh() {
			ConsoleSystem.Console.PrintLine( "Lobby.Refresh: refreshing lobby data..." );

			Visibility = (Visibility)SteamMatchmaking.GetLobbyData( Id, "visibility" ).ToInt();
			Name = SteamMatchmaking.GetLobbyData( Id, "name" );
			Map = SteamMatchmaking.GetLobbyData( Id, "map" );
			GameMode = SteamMatchmaking.GetLobbyData( Id, "mode" );

			HostId = SteamMatchmaking.GetLobbyOwner( Id );
			MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit( Id );

			GetLobbyMembers();
		}

		/*
		===============
		OnLobbyInfo
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		/// <exception cref="InvalidCastException"></exception>
		private static void OnLobbyInfo( in IGameEvent? eventData, in IEventArgs? args ) {
			if ( args is CommandLine.CommandExecutedEventData executed ) {
				ConsoleSystem.Console.PrintLine( "\n======== LOBBY INFO ========" );
				ConsoleSystem.Console.PrintLine( $"Name: {LobbyManager.Current.Name}" );
				ConsoleSystem.Console.PrintLine( $"Map: {LobbyManager.Current.Map}" );
				ConsoleSystem.Console.PrintLine( $"GameMode: {LobbyManager.Current.GameMode}" );
				ConsoleSystem.Console.PrintLine( $"MaxPlayers: {LobbyManager.Current.MaxPlayers}" );
				ConsoleSystem.Console.PrintLine( $"Visibility: {LobbyManager.Current.Visibility}" );
				ConsoleSystem.Console.PrintLine( "============================\n" );
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnAddClient
		===============
		*/
		/// <summary>
		/// Called when we're adding a client to the current lobby.
		/// </summary>
		/// <param name="eventData">The event object.</param>
		/// <param name="args">The event's arguments, should be a <see cref="ClientConnectionChangedEventData"/>.</param>
		private void OnAddClient( in IGameEvent? eventData, in IEventArgs? args ) {
			if ( args is Server.ClientConnectionChangedEventData connectionChange ) {
				if ( !connectionChange.Connected ) {
					ConsoleSystem.Console.PrintError( "Lobby.OnAddClient: called but connection status is false." );
					return;
				}
				if ( FindPlayer( connectionChange.ClientID, out int index ) ) {
					ConsoleSystem.Console.PrintError( $"Lobby.OnAddClient: client '{connectionChange.ClientID}' added twice!" );
					return;
				}
				GetLobbyMembers();
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnRemoveClient
		===============
		*/
		/// <summary>
		/// Called when we're removing a client from the current lobby.
		/// </summary>
		/// <param name="eventData">The event object.</param>
		/// <param name="args">The event's arguments, should be a <see cref="ClientConnectionChangedEventData"/>.</param>
		private void OnRemoveClient( in IGameEvent? eventData, in IEventArgs? args ) {
			if ( args is Server.ClientConnectionChangedEventData connectionChange ) {
				if ( connectionChange.Connected ) {
					ConsoleSystem.Console.PrintError( "Lobby.OnRemoveClient: called but connection status is true." );
					return;
				}
				if ( !FindPlayer( connectionChange.ClientID, out int index ) ) {
					ConsoleSystem.Console.PrintError( $"Lobby.OnRemoveClient: client '{connectionChange.ClientID}' not found!" );
					return;
				}
				GetLobbyMembers();
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}
	};
};