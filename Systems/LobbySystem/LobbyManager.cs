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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NomadCore.Systems.LobbySystem {
	/*
	===================================================================================
	
	LobbyManager
	
	===================================================================================
	*/
	/// <summary>
	/// Manages the creation and management of steam lobbies.
	/// </summary>

	public sealed class LobbyManager {
		/// <summary>
		/// The type of status update we're sending to the system.
		/// </summary>
		public enum EventType : byte {
			/// <summary>
			/// A client just joined the lobby.
			/// </summary>
			Joined,

			/// <summary>
			/// A client just left the lobby.
			/// </summary>
			Left,

			/// <summary>
			/// We just created a lobby.
			/// </summary>
			Created,

			Count
		};
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="eventType"></param>
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly ref struct LobbyStatusChangedEventData( CSteamID id, EventType eventType ) : IEventArgs {
			public readonly CSteamID Id = id;
			public readonly EventType EventType = eventType;
		};
		public readonly ref struct LobbyRefreshParams( string? map, string? gameMode, LobbyDistanceFilter distanceFilter ) {
			public readonly string? Map = map;
			public readonly string? GameMode = gameMode;
			public readonly LobbyDistanceFilter DistanceFilter = distanceFilter;
		};

		/// <summary>
		/// The maximum amount of members we can handle within a single lobby.
		/// </summary>
		public const int MAX_LOBBY_MEMBERS = 16;

		public static ActiveLobby Current { get; private set; }

		public static readonly List<ListedLobby> LobbyList = new List<ListedLobby>();
		public static readonly ConcurrentDictionary<CSteamID, User> Users = new ConcurrentDictionary<CSteamID, User>();

		private readonly Callback<LobbyEnter_t> LobbyEnterCallback;
		private readonly Callback<LobbyChatUpdate_t> LobbyChatUpdateCallback;

		private readonly CallResult<LobbyCreated_t> LobbyCreatedCallResult;
		private readonly CallResult<LobbyEnter_t>? LobbyEnterCallResult;
		private readonly CallResult<LobbyMatchList_t> LobbyMatchListCallResult;

		private static LobbyManager Instance {
			get {
				_instance ??= new LobbyManager();
				return _instance;
			}
		}
		private static LobbyManager _instance;

		public readonly static GameEvent LobbyListRefreshed = new GameEvent( nameof( LobbyListRefreshed ) );
		public readonly static RefStructEvent<LobbyStatusChangedEventData> LobbyCreated = new RefStructEvent<LobbyStatusChangedEventData>();
		public readonly static RefStructEvent<LobbyStatusChangedEventData> LobbyLeft = new RefStructEvent<LobbyStatusChangedEventData>();
		public readonly static RefStructEvent<LobbyStatusChangedEventData> LobbyJoined = new RefStructEvent<LobbyStatusChangedEventData>();
		public readonly static RefStructEvent<Server.ClientConnectionChangedEventData> ClientLeftLobby = new RefStructEvent<Server.ClientConnectionChangedEventData>();
		public readonly static RefStructEvent<Server.ClientConnectionChangedEventData> ClientJoinedLobby = new RefStructEvent<Server.ClientConnectionChangedEventData>();

		/*
		===============
		LobbyManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public LobbyManager() {
			LobbyEnterCallback = Callback<LobbyEnter_t>.Create( OnLobbyJoined );
			LobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create( OnLobbyChatUpdate );

			LobbyEnterCallResult = CallResult<LobbyEnter_t>.Create( OnLobbyJoined );
			LobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create( OnLobbyCreated );
			LobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create( OnLobbyMatchList );
		}

		/*
		===============
		CreateLobby
		===============
		*/
		/// <summary>
		/// Creates a new lobby with the given metadata provided to this factory function.
		/// </summary>
		/// <param name="visibility">How visible the lobby is to other clients.</param>
		/// <param name="name">The lobby's name.</param>
		/// <param name="maxPlayers">The maximum amount of clients allowed in the lobby.</param>
		/// <param name="map">The lobby's current map name.</param>
		/// <param name="gameMode">The multiplayer mode of the lobby.</param>
		public static void CreateLobby( Visibility visibility, string? name, int maxPlayers, string? map, string? gameMode ) {
			SteamAPICall_t handle = SteamMatchmaking.CreateLobby( GetLobbyType( visibility ), maxPlayers );
			Instance.LobbyCreatedCallResult.Set( handle );
		}

		private static void OnServerResponded( HServerListRequest hRequest, int iServer ) {
		}

		private static void OnServerFailedToRespond( HServerListRequest hRequest, int iServer ) {
		}

		private static void OnRefreshComplete( HServerListRequest hRequest, EMatchMakingServerResponse response ) {
		}

		/*
		===============
		RefreshLobbyList
		===============
		*/
		/// <summary>
		/// Refreshes and clears the current lobby list.
		/// </summary>
		public static void RefreshLobbyList( LobbyRefreshParams refreshParams ) {
			ConsoleSystem.Console.PrintLine( "LobbyManager.RefreshLobbyList: refreshing lobby list..." );

			/*
			MatchMakingKeyValuePair_t[] matchMakingFilters = [
				new MatchMakingKeyValuePair_t{
					m_szKey = "map",
					m_szValue = refreshParams.Map
				}
			];
			ISteamMatchmakingServerListResponse serverListResponse = new ISteamMatchmakingServerListResponse( OnServerResponded, OnServerFailedToRespond, OnRefreshComplete );
			HServerListRequest hServerList = SteamMatchmakingServers.RequestInternetServerList( SteamManager.SteamAppID, matchMakingFilters, (uint)matchMakingFilters.Length, serverListResponse );
			*/
			// TODO: use SteamGameServer & SteamMatchmakingServers for this later on

			SteamMatchmaking.AddRequestLobbyListDistanceFilter( GetLobbyDistanceFilter( refreshParams.DistanceFilter ) );
			if ( refreshParams.Map != null ) {
				SteamMatchmaking.AddRequestLobbyListStringFilter( "map", refreshParams.Map, ELobbyComparison.k_ELobbyComparisonEqual );
			}
			if ( refreshParams.GameMode != null ) {
				SteamMatchmaking.AddRequestLobbyListStringFilter( "gamemode", refreshParams.GameMode, ELobbyComparison.k_ELobbyComparisonEqual );
			}

			SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
			Instance.LobbyMatchListCallResult.Set( handle );
		}

		/*
		===============
		LeaveLobby
		===============
		*/
		/// <summary>
		/// Disposes of the lobby data stored in <see cref="Current"/>.
		/// </summary>
		public static void LeaveLobby() {
			ArgumentNullException.ThrowIfNull( Current );

			LobbyLeft.Publish( new LobbyStatusChangedEventData( Current.Id, EventType.Left ) );

			// kill the current lobby
			Current.Dispose();
			Current = null;

			/*
			if ( ChatBar != null ) {
				GetTree().Root.RemoveChild( ChatBar );
				ChatBar.QueueFree();
			}
			*/
		}

		/*
		===============
		ForEachPlayer
		===============
		*/
		/// <summary>
		/// Loops through all the players currently connected to the active lobby and calls into <paramref name="callback"/>.
		/// </summary>
		/// <param name="callback">The iterator callback.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void ForEachPlayer( Action<User>? callback ) {
			ArgumentNullException.ThrowIfNull( Current );
			ArgumentNullException.ThrowIfNull( callback );

			for ( int i = 0; i < Current.Players.Count; i++ ) {
				callback.Invoke( Current.Players[ i ] );
			}
		}

		/*
		===============
		ForEachPlayerExcludingSelf
		===============
		*/
		/// <summary>
		/// The same as <see cref="ForEachPlayer"/> but excludes our own <see cref="SteamManager.SteamID"/> from the callback.
		/// </summary>
		/// <param name="callback">The iterator callback.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void ForEachPlayerExcludingSelf( Action<User>? callback ) {
			ArgumentNullException.ThrowIfNull( Current );
			ArgumentNullException.ThrowIfNull( callback );

			for ( int i = 0; i < Current.Players.Count; i++ ) {
				User user = Current.Players[ i ];
				if ( (ulong)user.UserID == SteamManager.SteamID ) {
					continue;
				}
				callback.Invoke( user );
			}
		}

		/*
		===============
		GetLobbyType
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="visibility"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static ELobbyType GetLobbyType( Visibility visibility ) {
			return visibility switch {
				Visibility.FriendsOnly => ELobbyType.k_ELobbyTypeFriendsOnly,
				Visibility.Public => ELobbyType.k_ELobbyTypePublic,
				Visibility.Private => ELobbyType.k_ELobbyTypePrivate,
				_ => throw new ArgumentOutOfRangeException( nameof( visibility ) )
			};
		}

		/*
		===============
		GetLobbyDistanceFilter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="distanceFilter"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static ELobbyDistanceFilter GetLobbyDistanceFilter( LobbyDistanceFilter distanceFilter ) {
			return distanceFilter switch {
				LobbyDistanceFilter.LAN => ELobbyDistanceFilter.k_ELobbyDistanceFilterClose,
				LobbyDistanceFilter.Region => ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault,
				LobbyDistanceFilter.Continent => ELobbyDistanceFilter.k_ELobbyDistanceFilterFar,
				LobbyDistanceFilter.Global => ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide,
				_ => throw new ArgumentOutOfRangeException( nameof( distanceFilter ) )
			};
		}

		/*
		===============
		OnLobbyMatchList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		/// <param name="bIOFailure"></param>
		private void OnLobbyMatchList( LobbyMatchList_t pCallback, bool bIOFailure ) {
			LobbyList.Clear();
			for ( int i = 0; i < pCallback.m_nLobbiesMatching; i++ ) {
				LobbyList.Add( new ListedLobby( SteamMatchmaking.GetLobbyByIndex( i ) ) );
			}
			LobbyListRefreshed.Publish( EmptyEventArgs.Args );
		}

		/*
		===============
		OnLobbyChatUpdate
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnLobbyChatUpdate( LobbyChatUpdate_t pCallback ) {
			CSteamID changerId = (CSteamID)pCallback.m_ulSteamIDMakingChange;
			string changerName = SteamFriends.GetFriendPersonaName( changerId );
			EChatMemberStateChange stateChange = (EChatMemberStateChange)pCallback.m_rgfChatMemberStateChange;

			switch ( stateChange ) {
				case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
					ConsoleSystem.Console.PrintLine( $"LobbyManager.OnLobbyChatUpdate: {changerName} has joined the game..." );
					ClientJoinedLobby.Publish( new Server.ClientConnectionChangedEventData( changerId, true ) );
					break;
				case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
				case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
				case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
				case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
					ConsoleSystem.Console.PrintLine( $"LobbyManager.OnLobbyChatUpdate: {changerName} has left the game..." );
					ClientLeftLobby.Publish( new Server.ClientConnectionChangedEventData( changerId, false ) );
					break;
			}
		}

		/*
		===============
		OnLobbyJoined
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnLobbyJoined( LobbyEnter_t pCallback ) {
			OnLobbyJoined( pCallback, false );
		}

		/*
		===============
		OnLobbyJoined
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		/// <param name="bIOFailure"></param>
		private void OnLobbyJoined( LobbyEnter_t pCallback, bool bIOFailure ) {
			if ( pCallback.m_EChatRoomEnterResponse != (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess ) {
				ConsoleSystem.Console.PrintError( $"[STEAM] Error joining lobby {pCallback.m_ulSteamIDLobby}: " +
					$"{(EChatRoomEnterResponse)pCallback.m_EChatRoomEnterResponse}" );
				return;
			}

			ConsoleSystem.Console.PrintLine( $"Joined lobby {pCallback.m_ulSteamIDLobby}" );

			// let it hook to the event before firing the event
			Current = new ActiveLobby( (CSteamID)pCallback.m_ulSteamIDLobby );

			//ChatBar = SceneCache.GetScene( "res://scenes/multiplayer/chat_bar.tscn" ).Instantiate<Chat>();
			//GetTree().Root.AddChild( ChatBar );

			LobbyJoined.Publish( new LobbyStatusChangedEventData( (CSteamID)pCallback.m_ulSteamIDLobby, EventType.Joined ) );
		}

		/*
		===============
		OnLobbyCreated
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		/// <param name="bIOFailure"></param>
		private void OnLobbyCreated( LobbyCreated_t pCallback, bool bIOFailure ) {
			if ( pCallback.m_eResult != EResult.k_EResultOK ) {
				ConsoleSystem.Console.PrintLine( $"[STEAM] Error creating lobby: {pCallback.m_eResult}" );
				return;
			}

			//PlayersReady.EnsureCapacity( MAX_LOBBY_MEMBERS );
			//PlayersReady.TryAdd( SteamManager.SteamID, true );

			ConsoleSystem.Console.PrintLine( $"Created lobby [{pCallback.m_ulSteamIDLobby}] Name: {Current.Name}, MaxMembers: {Current.Players.Count}, GameType: {Current.GameMode}" );
			LobbyCreated.Publish( new LobbyStatusChangedEventData( (CSteamID)pCallback.m_ulSteamIDLobby, EventType.Created ) );
		}
	};
};