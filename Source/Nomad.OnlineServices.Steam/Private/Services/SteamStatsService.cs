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

using System.Threading.Tasks;
using Nomad.Core.Logger;
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam {
	/*
	===================================================================================

	SteamStatsService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamStatsService : IStatsService {
		public bool SupportsLeaderboards => true;

		private readonly CallResult<UserStatsReceived_t> _userStatsRecieved;
		private readonly CallResult<UserStatsStored_t> _userStatsStored;
		private readonly CallResult<UserStatsUnloaded_t> _userStatsUnloaded;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		/*
		===============
		SteamStatsService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public SteamStatsService( ILoggerService logger ) {
			_userStatsRecieved = CallResult<UserStatsReceived_t>.Create( OnUserStatsReceived );
			_userStatsStored = CallResult<UserStatsStored_t>.Create( OnUserStatsStored );
			_userStatsUnloaded = CallResult<UserStatsUnloaded_t>.Create( OnUserStatsUnloaded );

			_logger = logger;
			_category = _logger.CreateCategory( nameof( SteamStatsService ), LogLevel.Info, true );

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
			_userStatsRecieved.Dispose();
			_userStatsStored.Dispose();
			_userStatsUnloaded.Dispose();
		}

		/*
		===============
		GetStatFloat
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		public async ValueTask<float> GetStatFloat( string statName ) {
			SteamUserStats.GetStat( statName, out float statValue );
			return statValue;
		}

		/*
		===============
		GetStatInt
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		public async ValueTask<int> GetStatInt( string statName ) {
			SteamUserStats.GetStat( statName, out int statValue );
			return statValue;
		}

		/*
		===============
		SetStatFloat
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="statName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public async ValueTask SetStatFloat( string statName, float value ) {
			SteamUserStats.SetStat( statName, value );
		}

		/*
		===============
		SetStatInt
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="statName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public async ValueTask SetStatInt( string statName, int value ) {
			SteamUserStats.SetStat( statName, value );
		}

		/*
		===============
		StoreStats
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public async ValueTask<bool> StoreStats() {
			return SteamUserStats.StoreStats();
		}

		/*
		===============
		OnUserStatsReceived
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="pCallback"></param>
		/// <param name="bIOFailure"></param>
		private void OnUserStatsReceived( UserStatsReceived_t pCallback, bool bIOFailure ) {
			if ( pCallback.m_eResult != EResult.k_EResultOK ) {
				return;
			}
		}

		/*
		===============
		OnUserStatsUnloaded
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="param"></param>
		/// <param name="bIOFailure"></param>
		private void OnUserStatsUnloaded( UserStatsUnloaded_t param, bool bIOFailure ) {
			if ( bIOFailure ) {
				return;
			}
		}

		/*
		===============
		OnUserStatsStored
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="param"></param>
		/// <param name="bIOFailure"></param>
		private void OnUserStatsStored( UserStatsStored_t param, bool bIOFailure ) {
			if ( param.m_eResult != EResult.k_EResultOK ) {
				return;
			}
		}
	};
};
