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

using ConsoleSystem;
using CVars;
using EventSystem;
using Steamworks;
using System;

namespace Steam {
	/*
	===================================================================================
	
	SteamManager
	
	===================================================================================
	*/
	/// <summary>
	/// Manages steam initialization and operations
	/// </summary>

	public sealed class SteamManager {
		public static readonly CSteamID VIP_ID = (CSteamID)76561199403850315;

		public static readonly CVar<ulong> SteamID = new CVar<ulong>(
			name: "steam.UserID",
			defaultValue: 0,
			description: "The user's steam id.",
			flags: CVarFlags.Hidden
		);
		public static readonly CVar<string> UserName = new CVar<string>(
			name: "steam.UserName",
			defaultValue: String.Empty,
			description: "The user's steam name.",
			flags: CVarFlags.ReadOnly
		);
		public static readonly AppId_t AppID;
		public static readonly bool Initialized = false;

		//public static LobbyManager LobbyManager = null;
		//public static ServerManager ServerManager = null;

		private static readonly SteamAPIWarningMessageHook_t? DebugMessageHook = null;
		private static readonly ConsoleCommand DumpInfo = new ConsoleCommand( nameof( DumpInfo ), OnDumpSteamInfo );

		/*
		===============
		SteamManager
		===============
		*/
		static SteamManager() {
			ESteamAPIInitResult result = SteamAPI.InitEx( out string errMessage );
			if ( result != ESteamAPIInitResult.k_ESteamAPIInitResult_OK ) {
				ConsoleSystem.Console.PrintError( $"SteamAPI.InitEx failed - {errMessage}" );
				return;
			}

			Initialized = true;
			SteamID.Value = (ulong)SteamUser.GetSteamID();
			UserName.Value = SteamFriends.GetPersonaName();
			AppID = SteamUtils.GetAppID();

			DebugMessageHook = new SteamAPIWarningMessageHook_t( SteamAPIDebugTextCallback );
			SteamClient.SetWarningMessageHook( DebugMessageHook );

			OnDumpSteamInfo( null, null );
		}

		/*
		===============
		OnDumpSteamInfo
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private static void OnDumpSteamInfo( in IGameEvent eventData, in IEventArgs args ) {
			ConsoleSystem.Console.PrintLine( "\n[STEAM INFO]" );
			ConsoleSystem.Console.PrintLine( $"...SteamID: {SteamID}" );
			ConsoleSystem.Console.PrintLine( $"...AppID: {AppID}" );
			ConsoleSystem.Console.PrintLine( $"...UserName: {UserName}" );
		}

		/*
		===============
		SteamAPIDebugTextCallback
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="debugText"></param>
		private static void SteamAPIDebugTextCallback( int severity, System.Text.StringBuilder debugText ) {
			if ( severity >= 1 ) {
				ConsoleSystem.Console.PrintWarning( $"Steam: {debugText}" );
			} else {
				ConsoleSystem.Console.PrintLine( $"Steam: {debugText}" );
			}
		}
	};
};