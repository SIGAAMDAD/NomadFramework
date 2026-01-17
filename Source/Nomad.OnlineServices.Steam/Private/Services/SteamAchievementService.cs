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

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Nomad.Core.EngineUtils;
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam {
	/*
	===================================================================================

	SteamAchievementService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamAchievementService : IAchievementService {
		private record AchievementInfo(
			string Id,
			bool Achieved,
			float Progress,
			float MaxProgress
		);

		public bool SupportsAchievements => true;

		private readonly ConcurrentDictionary<string, SteamAchievementInfo> _achievements;

		private readonly IEngineService _engineService;

		private readonly CallResult<UserAchievementIconFetched_t> _userAchievementIconFetched;
		private readonly CallResult<UserAchievementStored_t> _userAchievementStored;

		/*
		===============
		SteamAchievementService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="engineService"></param>
		public SteamAchievementService( IEngineService engineService ) {
			_engineService = engineService;

			int numAchievements = (int)SteamUserStats.GetNumAchievements();
			_achievements = new ConcurrentDictionary<string, SteamAchievementInfo>( numAchievements, numAchievements );

			_userAchievementIconFetched = CallResult<UserAchievementIconFetched_t>.Create( OnAchievementIconFetched );

			for ( uint i = 0; i < numAchievements; i++ ) {
				string name = SteamUserStats.GetAchievementName( i );
				_achievements[ name ] = new SteamAchievementInfo( name );
			}

			SteamUserStats.RequestCurrentStats();
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

		private void OnUserAchievementStored( UserAchievementStored_t pCallback ) {

		}

		public ValueTask LockAchievement( string achievementId ) {
			throw new System.NotImplementedException();
		}

		public ValueTask SetAchievementProgress( string achievementId, float current, float max ) {
			throw new System.NotImplementedException();
		}

		public ValueTask UnlockAchievement( string achievementId ) {
			throw new System.NotImplementedException();
		}
	};
};
