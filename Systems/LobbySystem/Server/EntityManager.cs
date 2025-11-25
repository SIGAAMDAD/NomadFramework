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
using NomadCore.Systems.LobbySystem.Server.Packets;
using Steamworks;
using System;
using System.Collections.Generic;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	EntityManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class EntityManager {
		public readonly struct NetworkNode( Node node, Action send, Action<INetworkPacket> receive ) {
			internal readonly Node Node = node;
			internal readonly Action Send = send;
			internal readonly Action<INetworkPacket> Receive = receive;
		};

		public readonly struct PlayerNetworkNode( Node node, Action send, Action<CSteamID, INetworkPacket> receive ) {
			internal readonly Node Node = node;
			internal readonly Action Send = send;
			internal readonly Action<CSteamID, INetworkPacket> Receive = receive;
		};

		private readonly Dictionary<int, NetworkNode> NodeCache = new Dictionary<int, NetworkNode>();
		private readonly Dictionary<CSteamID, PlayerNetworkNode> PlayerCache = new Dictionary<CSteamID, PlayerNetworkNode>();
		private readonly Dictionary<CSteamID, System.IO.MemoryStream> Batches = new Dictionary<CSteamID, System.IO.MemoryStream>( LobbyManager.MAX_LOBBY_MEMBERS );

		/*
		===============
		EntityManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public EntityManager() {
			MessageHandler.ClientDataReceived.Subscribe( this, OnClientDataReceived );
			MessageHandler.ServerSyncReceived.Subscribe( this, OnServerSyncReceived );
			LobbyManager.ClientJoinedLobby.Subscribe( this, OnLobbyStatusChanged );
			LobbyManager.ClientLeftLobby.Subscribe( this, OnLobbyStatusChanged );
		}

		/*
		===============
		AddPlayer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="callbacks"></param>
		public void AddPlayer( CSteamID userId, PlayerNetworkNode callbacks ) {
			ConsoleSystem.Console.PrintLine( $"EntityManager.AddPlayer: added player with hash {userId} to network sync cache." );
			PlayerCache.TryAdd( userId, callbacks );
		}

		/*
		===============
		RemovePlayer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		public void RemovePlayer( CSteamID userId ) {
			PlayerCache.Remove( userId );
		}

		/*
		===============
		GetPlayer
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public INetworkPlayer GetPlayer( CSteamID userId ) {
			return PlayerCache[ userId ].Node as INetworkPlayer;
		}

		/*
		===============
		AddNetworkNode
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="callbacks"></param>
		public void AddNetworkNode( NodePath node, NetworkNode callbacks ) {
			ConsoleSystem.Console.PrintLine( $"Added node with hash {node.GetHashCode()} to network sync cache." );
			NodeCache.TryAdd( node.GetHashCode(), callbacks );
		}

		/*
		===============
		RemoveNetworkNode
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		public void RemoveNetworkNode( NodePath node ) {
			if ( NodeCache.ContainsKey( node.GetHashCode() ) ) {
				NodeCache.Remove( node.GetHashCode() );
			}
		}

		/*
		===============
		GetNetworkNode
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public Node? GetNetworkNode( NodePath node ) {
			if ( NodeCache.TryGetValue( node.GetHashCode(), out NetworkNode value ) ) {
				return value.Node;
			}
			ConsoleSystem.Console.PrintError( $"SteamLobby.GetNetworkNode: invalid network node {node.GetHashCode()}!" );
			return null;
		}

		/*
		===============
		SendPackets
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void SendPackets() {
			foreach ( var node in NodeCache.Values ) {
				node.Send?.Invoke();
			}
			foreach ( var player in PlayerCache.Values ) {
				player.Send?.Invoke();
			}
			SendBatches();
		}

		/*
		===============
		OnLobbyStatusChanged
		===============
		*/
		/// <summary>
		/// Clears both <see cref="NodeCache"/> and <see cref="PlayerCache"/> whenever the current lobby's connection status changes.
		/// </summary>
		private void OnLobbyStatusChanged( in ClientConnectionChangedEventData args ) {
			NodeCache.Clear();
			PlayerCache.Clear();
		}

		/*
		===============
		OnClientDataReceived
		===============
		*/
		private void OnClientDataReceived( in MessageHandler.ClientDataReceivedEventData args ) {
			if ( !PlayerCache.TryGetValue( (CSteamID)args.SenderId, out PlayerNetworkNode player ) ) {
				ConsoleSystem.Console.PrintError( $"" );
				return;
			}
			player.Receive.Invoke( (CSteamID)args.SenderId, args.Packet );
		}

		/*
		===============
		OnServerSyncReceived
		===============
		*/
		private void OnServerSyncReceived( in MessageHandler.ServerSyncReceivedEventData args ) {
		}

		/*
		===============
		SendBatches
		===============
		*/
		private void SendBatches() {
			foreach ( var batch in Batches ) {
				if ( batch.Value.Length > 0 ) {
					//MessageHandler.SendTargetPacket( batch.Key, batch.Value.GetBuffer(), Steamworks.Constants.k_nSteamNetworkingSend_Unreliable, false, 0 );
					batch.Value.SetLength( 0 );
				}
			}
		}
	};
};