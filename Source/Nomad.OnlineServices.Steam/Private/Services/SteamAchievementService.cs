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
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
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

		private readonly ConcurrentDictionary<string, SteamAchievementInfo> _achievements;

		private readonly SteamAppData _appData;
		private readonly IEngineService _engineService;

		private readonly CallResult<UserAchievementIconFetched_t> _userAchievementIconFetched;
		private readonly CallResult<UserAchievementStored_t> _userAchievementStored;

		private bool _isDisposed = false;

		/*
		===============
		SteamAchievementService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appData"></param>
		/// <param name="logger"></param>
		/// <param name="engineService"></param>
		public SteamAchievementService( SteamAppData appData, ILoggerService logger, IEngineService engineService ) {
			_engineService = engineService;
			_appData = appData;

			int numAchievements = (int)SteamUserStats.GetNumAchievements();
			_achievements = new ConcurrentDictionary<string, SteamAchievementInfo>( numAchievements, numAchievements );

			_userAchievementIconFetched = CallResult<UserAchievementIconFetched_t>.Create( OnAchievementIconFetched );
			_userAchievementStored = CallResult<UserAchievementStored_t>.Create( OnUserAchievementStored );

			for ( uint i = 0; i < numAchievements; i++ ) {
				string name = SteamUserStats.GetAchievementName( i );
				_achievements[name] = new SteamAchievementInfo( name );
			}

			SteamUserStats.RequestCurrentStats();
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
		/// <param name="bIOFailure"></param>
		private void OnUserAchievementStored( UserAchievementStored_t pCallback, bool bIOFailure ) {
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
		/// <returns></returns>
		public async ValueTask LockAchievement( string achievementId ) {
			if ( !SteamUserStats.ClearAchievement( achievementId ) ) {
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
		/// <returns></returns>
		public async ValueTask SetAchievementProgress( string achievementId, float current ) {
			if ( !_achievements.TryGetValue( achievementId, out var achievementInfo ) ) {
				return;
			}

			achievementInfo.SetAchievementProgress( current );
		}

		public async ValueTask UnlockAchievement( string achievementId ) {
			if ( SteamUserStats.SetAchievement( achievementId ) ) {
				
			}
		}
	};
};
