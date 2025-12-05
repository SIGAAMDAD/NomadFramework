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

using NomadCore.Abstractions.Services;
using Steamworks;
using System.Collections.Generic;

namespace NomadCore.Systems.Steam.Infrastructure {
	/*
	===================================================================================
	
	SteamDataRepository
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class SteamDataRepository {
		public string UserName => _userName;
		private readonly string _userName;

		public AppId_t AppID => _appId;
		private readonly AppId_t _appId;
		
		public CSteamID SteamID => _steamId;
		private readonly CSteamID _steamId;

		public SteamDataRepository( ILoggerService logger ) {
			ESteamAPIInitResult result = SteamAPI.InitEx( out string errorMessage );
			if ( result != ESteamAPIInitResult.k_ESteamAPIInitResult_OK ) {
				logger.PrintError( $"SteamDataRepository: failed to initialize SteamAPI - {result}" );
			}

			_userName = SteamFriends.GetPersonaName();
			_appId = SteamUtils.GetAppID();
			_steamId = SteamUser.GetSteamID();
		}
	};
};