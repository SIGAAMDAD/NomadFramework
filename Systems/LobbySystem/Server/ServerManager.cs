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

using Godot;
using Steamworks;
using System;
using NomadCore.Systems.Steam;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	ServerManager
	
	===================================================================================
	*/
	/// <summary>
	/// Manages all server data processing. The server system middleman.
	/// </summary>

	public sealed partial class ServerManager : Node {
		internal readonly EntityManager EntityManager;
		internal readonly MessageHandler MessageHandler;
		internal readonly VoiceChat ProximityChat;
		internal readonly VoteSystem VoteSystem;

		/// <summary>
		/// The primary synchronization thread that we are handling ready commands on
		/// </summary>
		private System.Threading.Thread WaitThread;

		/*
		===============
		ServerManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public ServerManager() {
			EntityManager = new EntityManager();
			ProximityChat = new VoiceChat( this );
			MessageHandler = new MessageHandler();
			VoteSystem = new VoteSystem( this );
		}

		/*
		===============
		InitializeSteamNetworking
		===============
		*/
		private static void InitializeSteamNetworking() {
			try {
				SteamNetworkingUtils.InitRelayNetworkAccess();
				ConsoleSystem.Console.PrintLine( "ServerManager.InitializeSteamNetworking: relay network access initialized" );

				SteamNetworkingUtils.SetDebugOutputFunction(
					ESteamNetworkingSocketsDebugOutputType.k_ESteamNetworkingSocketsDebugOutputType_Verbose,
					( type, message ) => ConsoleSystem.Console.PrintLine( $"ServerManager.DebugMessage: {type} - {message}" )
				);

				SteamNetworkingIdentity localIdentity = new SteamNetworkingIdentity();
				localIdentity.SetSteamID( (CSteamID)SteamManager.SteamID.Value );

				SteamNetworkingSockets.ResetIdentity( ref localIdentity );
				ConsoleSystem.Console.PrintLine( $"ServerManager.InitializeSteamNetworking: set local identity: {SteamUser.GetSteamID()}" );

				ESteamNetworkingAvailability status = SteamNetworkingUtils.GetRelayNetworkStatus( out SteamRelayNetworkStatus_t relayStatus );
				ConsoleSystem.Console.PrintLine( $"ServerManager.InitializeSteamNetworking: relay network status - {status}" );
			} catch ( Exception e ) {
				ConsoleSystem.Console.PrintError( $"ServerManager.InitializeSteamNetworking: steam networking initialization failed - {e.Message}" );
			}
		}

		/*
		===============
		_Ready
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _Ready() {
			base._Ready();

			InitializeSteamNetworking();
		}

		/*
		===============
		_Process
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process( double delta ) {
			base._Process( delta );

			ProximityChat.CaptureVoice( (float)delta );
			EntityManager.SendPackets();
		}
	};
};