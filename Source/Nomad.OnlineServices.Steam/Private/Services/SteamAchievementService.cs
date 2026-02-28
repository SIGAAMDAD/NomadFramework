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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Nomad.Core.Util;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Services {
	/*
	===================================================================================

	SteamAchievementService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamAchievementService : IAchievementService {
		public bool SupportsAchievements => true;

		public int NumAchievements => _achievements.Count;
		private readonly ConcurrentDictionary<string, SteamAchievementInfo> _achievements;

		private readonly SteamAppData _appData;
		private readonly IEngineService _engineService;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly CallResult<UserAchievementIconFetched_t> _userAchievementIconFetched;
		private readonly Callback<UserAchievementStored_t> _userAchievementStored;

		private bool _isDisposed = false;

		public IGameEvent<AchievementUnlockedEventArgs> Unlocked => _unlocked;
		private readonly IGameEvent<AchievementUnlockedEventArgs> _unlocked;

		/*
		===============
		SteamAchievementService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="appData"></param>
		/// <param name="logger"></param>
		/// <param name="engineService"></param>
		/// <param name="eventFactory"></param>
		public SteamAchievementService( SteamUserData userData, SteamAppData appData, ILoggerService logger, IEngineService engineService, IGameEventRegistryService eventFactory ) {
			_engineService = engineService;
			_appData = appData;

			int numAchievements = (int)SteamUserStats.GetNumAchievements();
			_achievements = new ConcurrentDictionary<string, SteamAchievementInfo>();

			_logger = logger;
			_category = _logger.CreateCategory( nameof( SteamAchievementService ), LogLevel.Info, true );

			_userAchievementIconFetched = CallResult<UserAchievementIconFetched_t>.Create( OnAchievementIconFetched );
			_userAchievementStored = Callback<UserAchievementStored_t>.Create( OnUserAchievementStored );

			for ( uint i = 0; i < numAchievements; i++ ) {
				string name = SteamUserStats.GetAchievementName( i );
				_achievements[name] = new SteamAchievementInfo( name );
			}

			SteamAPICall_t hCallback = SteamUserStats.RequestUserStats( userData.UserID );

			_unlocked = eventFactory.GetEvent<AchievementUnlockedEventArgs>( Constants.Events.ACHIEVEMENT_UNLOCKED, Constants.Events.NAMESPACE );
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
				_userAchievementIconFetched?.Dispose();
				_userAchievementStored?.Dispose();

				_category?.Dispose();
				_unlocked?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		OnAchievementIconFetched
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="pCallback"></param>
		/// <param name="bIOFailure"></param>
		private void OnAchievementIconFetched( UserAchievementIconFetched_t pCallback, bool bIOFailure ) {
			if ( !_achievements.TryGetValue( pCallback.m_rgchAchievementName, out var achievementInfo ) ) {
				_logger.PrintError( in _category, $"SteamAchievementService.OnAchievementIconFetched: no such achievement '{pCallback.m_rgchAchievementName}'" );
				return;
			}
			achievementInfo.SetIcon( pCallback, _engineService );
		}

		/*
		===============
		OnUserAchievementStored
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCallback"></param>
		private void OnUserAchievementStored( UserAchievementStored_t pCallback ) {
			if ( !_achievements.TryGetValue( pCallback.m_rgchAchievementName, out var achievementInfo ) ) {
				_logger.PrintError( in _category, $"SteamAchievementService.OnUserAchievementStored: no such achievement '{pCallback.m_rgchAchievementName}'" );
				return;
			}
			_logger.PrintError( in _category, $"SteamAchievementService.OnAchievementIconFetched: updating achievement storage for '{pCallback.m_rgchAchievementName}'" );
			if ( pCallback.m_nCurProgress == pCallback.m_nMaxProgress ) {
				_unlocked.Publish( new AchievementUnlockedEventArgs( new InternString( achievementInfo.Id ) ) );
			}
		}

		/*
		===============
		LockAchievement
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="achievementId"></param>
		public async Task LockAchievement( string achievementId ) {
			if ( SteamUserStats.ClearAchievement( achievementId ) ) {
				SteamUserStats.StoreStats();
			}
		}

		/*
		===============
		SetAchievementProgress
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="achievementId"></param>
		/// <param name="current"></param>
		public async Task SetAchievementProgress( string achievementId, float current ) {
			if ( !_achievements.TryGetValue( achievementId, out var achievementInfo ) ) {
				return;
			}

			achievementInfo.SetAchievementProgress( current );
		}

		/*
		===============
		UnlockAchievement
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="achievementId"></param>
		public async Task UnlockAchievement( string achievementId ) {
			if ( SteamUserStats.SetAchievement( achievementId ) ) {
				SteamUserStats.StoreStats();
			}
		}

		/*
		===============
		GetAchievementInfo
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="achievementId"></param>
		/// <returns></returns>
		public IAchievementInfo? GetAchievementInfo( string achievementId ) {
			if ( !_achievements.TryGetValue( achievementId, out var achievementInfo ) ) {
				return null;
			}
			return achievementInfo;
		}
	};
};
