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
using Nomad.Core.OnlineServices;
using Steamworks;

namespace Nomad.OnlineServices.Steam {
	internal sealed class SteamStatsService : IStatsService {
		public bool SupportsLeaderboards => true;

		private readonly CallResult<UserStatsReceived_t> _userStatsRecieved;

		public SteamStatsService() {
			_userStatsRecieved = CallResult<UserStatsReceived_t>.Create( OnUserStatsReceived );

			SteamUserStats.RequestCurrentStats();
		}

		private void OnUserStatsReceived( UserStatsReceived_t pCallback, bool bIOFailure ) {
		}

		public ValueTask<float> GetStatFloat( string statName ) {
			throw new System.NotImplementedException();
		}

		public ValueTask<int> GetStatInt( string statName ) {
			throw new System.NotImplementedException();
		}

		public ValueTask SetStatFloat( string statName, float value ) {
			throw new System.NotImplementedException();
		}

		public ValueTask SetStatInt( string statName, int value ) {
			throw new System.NotImplementedException();
		}

		public ValueTask<bool> StoreStats() {
			throw new System.NotImplementedException();
		}
	};
};
