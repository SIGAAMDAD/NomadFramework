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
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Nomad.OnlineServices.Steam.Private.Services;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
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
		private readonly SteamAppData _appData;

		public OnlinePlatform Platform => OnlinePlatform.Steam;
		public string PlatformName => nameof( OnlinePlatform.Steam );
		public bool IsAvailable => true;

		public IStatsService Stats => _statsService;
		private readonly SteamStatsService _statsService;

		public IAchievementService Achievements => _achievementsService;
		private readonly SteamAchievementService _achievementsService;

		public ICloudStorageService CloudStorage => _cloudStorageService;
		private readonly SteamCloudStorageService _cloudStorageService;

		private bool _isDisposed = false;

		/*
		===============
		SteamService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="fileSystem"></param>
		/// <param name="engineService"></param>
		/// <param name="eventFactory"></param>
		public SteamService( ILoggerService logger, IFileSystem fileSystem, IEngineService engineService, IGameEventRegistryService eventFactory ) {
			ESteamAPIInitResult result = SteamAPI.InitEx( out string errorMessage );
			if ( result != ESteamAPIInitResult.k_ESteamAPIInitResult_OK ) {
				logger.PrintError( $"SteamService: failed to initialize SteamAPI - {result}, {errorMessage}" );
			}

			_userData = new SteamUserData {
				UserID = SteamUser.GetSteamID(),
				UserName = SteamFriends.GetPersonaName()
			};

			_appData = new SteamAppData {
				AppId = SteamUtils.GetAppID()
			};

			_logger = logger;
			_category = logger.CreateCategory( nameof( Nomad.OnlineServices.Steam ), LogLevel.Info, true );

			_statsService = new SteamStatsService( _userData, logger );
			_achievementsService = new SteamAchievementService( _userData, _appData, logger, engineService, eventFactory );
			_cloudStorageService = new SteamCloudStorageService( logger, fileSystem );
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
			if ( !_isDisposed ) {
				_category?.Dispose();
				_statsService?.Dispose();
				_achievementsService?.Dispose();
				_cloudStorageService?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
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
	};
};
