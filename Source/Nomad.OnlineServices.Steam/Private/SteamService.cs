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
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Nomad.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private {
	/*
	===================================================================================

	SteamService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamService : IOnlinePlatformService {
		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly SteamUserData _userData;
		private readonly AppId_t _appId;

		public OnlinePlatform Platform => OnlinePlatform.Steam;
		public string PlatformName => nameof( OnlinePlatform.Steam );
		public bool IsAvailable => true;

		public IStatsService Stats => _statsService;
		private readonly SteamStatsService _statsService;

		public IAchievementService Achievements {
			get {
				throw new NotImplementedException();
			}
		}

		public IMultiplayerService Multiplayer {
			get {
				throw new NotImplementedException();
			}
		}

		public ICloudStorageService CloudStorage {
			get {
				throw new NotImplementedException();
			}
		}

		/*
		===============
		SteamService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="logger"></param>
		public SteamService( ILoggerService logger ) {
			ESteamAPIInitResult result = SteamAPI.InitEx( out string errorMessage );
			if ( result != ESteamAPIInitResult.k_ESteamAPIInitResult_OK ) {
				logger.PrintError( $"SteamService: failed to initialize SteamAPI - {result}, {errorMessage}" );
			}

			_userData = new SteamUserData(
				SteamUser.GetSteamID(),
				SteamFriends.GetPersonaName()
			);

			_appId = SteamUtils.GetAppID();

			_logger = logger;
			_category = logger.CreateCategory( "Steam", LogLevel.Info, true );
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
			_category.Dispose();
		}

		/*
		===============
		Initialize
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Initialize() {
			ESteamAPIInitResult result = SteamAPI.InitEx( out string errorMessage );
			if ( result != ESteamAPIInitResult.k_ESteamAPIInitResult_OK ) {

			}

		}

		/*
		===============
		RunCallbacks
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void RunCallbacks() {
			SteamAPI.RunCallbacks();
		}

		public void Shutdown() {
			throw new NotImplementedException();
		}
	};
};
