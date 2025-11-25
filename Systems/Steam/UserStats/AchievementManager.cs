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

using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Steam.UserStats {
	/*
	===================================================================================
	
	AchievementManager
	
	===================================================================================
	*/
	/// <summary>
	/// Manages user achievement data
	/// </summary>

	public sealed class AchievementManager : IObservable<AchievementData> {
		private class Unsubscriber(List<IObserver<AchievementData>> observers, IObserver<AchievementData> observer) : IDisposable {
			private readonly List<IObserver<AchievementData>> Observers = observers;
			private readonly IObserver<AchievementData> Observer = observer;

			/*
			===============
			Dispose
			===============
			*/
			public void Dispose() {
				if ( Observer != null && Observers.Contains( Observer ) ) {
					Observers.Remove( Observer );
				}
			}
		};

		private readonly Callback<UserStatsReceived_t> UserStatsReceivedCallback;
		private readonly Callback<UserStatsStored_t> UserStatsStoredCallback;
		private readonly Callback<UserAchievementStored_t> UserAchievementStoredCallback;
		private readonly CallResult<UserStatsReceived_t> UserStatsReceivedCallResult;

		private readonly List<IObserver<AchievementData>> Observers = new List<IObserver<AchievementData>>();
		private readonly ConcurrentDictionary<AchievementID, AchievementData> AchievementCache = new ConcurrentDictionary<AchievementID, AchievementData>();
		private readonly ConcurrentDictionary<StatID, StatData> StatCache = new ConcurrentDictionary<StatID, StatData>();

		private static AchievementManager Instance;

		/*
		===============
		AchievementManager
		===============
		*/
		public AchievementManager() {
			for ( AchievementID id = 0; id < AchievementID.Count; id++ ) {
				AchievementCache.TryAdd( id, new AchievementData( id ) );
			}

			UserStatsReceivedCallback = Callback<UserStatsReceived_t>.Create( OnUserStatsReceived );
			UserStatsStoredCallback = Callback<UserStatsStored_t>.Create( OnUserStatsStored );

			if ( !SteamUserStats.RequestCurrentStats() ) {
				ConsoleSystem.Console.PrintError( "AchievementManager.Init: error fetching steam user stats - SteamUserStats.RequestCurrentStats failed." );
			}
			
			Instance = this;
		}

		/*
		===============
		Subscribe
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		public IDisposable Subscribe( IObserver<AchievementData> observer ) {
			Observers.Add( observer );
			return new Unsubscriber( Observers, observer );
		}

		/*
		===============
		ActivateAchievement
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public static void ActivateAchievement( AchievementID id ) {
			ArgumentNullException.ThrowIfNull( Instance );

			if ( !Instance.AchievementCache.TryGetValue( id, out AchievementData data ) ) {
				ConsoleSystem.Console.PrintError( $"AchievementManager.ActivateAchievement: no achievement cached for '{id}'!" );
				return;
			}
			if ( data.Achieved ) {
				return; // already achieved, just ignore the call
			}
			data.Activate();
		}

		/*
		===============
		SetStatProgress
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="progress"></param>
		public static void SetStatProgress( StatID id, float progress ) {
			ArgumentNullException.ThrowIfNull( Instance );

			if ( !Instance.StatCache.TryGetValue( id, out StatData stat ) ) {
				ConsoleSystem.Console.PrintError( $"AchievementManager.SetStatProgress: no stat cached for '{id}'!" );
				return;
			}
			stat.SetProgress( progress );
		}

		/*
		===============
		OnUserStatsReceived
		===============
		*/
		private void OnUserStatsReceived( UserStatsReceived_t pCallback ) {
			if ( pCallback.m_eResult != EResult.k_EResultOK ) {
				ConsoleSystem.Console.PrintError( $"AchievementManager.OnUserStatsReceived: couldn't retrieve stats - {pCallback.m_eResult}" );
				return;
			}
			if ( pCallback.m_nGameID != (ulong)SteamManager.AppID ) {
				ConsoleSystem.Console.PrintError( $"AchievementManager.OnUserStatsReceived: incorrect appid!" );
				return;
			}

			ConsoleSystem.Console.PrintLine( "AchievementManager.OnUserStatsReceived: successfully fetched user statistics." );

			int count = 0;
			foreach ( var achievement in AchievementCache.Values ) {
				achievement.Update();
				if ( achievement.Achieved ) {
					count++;
				}
			}

			// got all achievements, WELL DONE!
			if ( count == (int)AchievementID.Count ) {
				ActivateAchievement( AchievementID.Master_Of_The_Wastes );
			}
		}

		/*
		===============
		OnUserStatsStored
		===============
		*/
		private void OnUserStatsStored( UserStatsStored_t pCallback ) {
			if ( pCallback.m_eResult != EResult.k_EResultOK ) {
				ConsoleSystem.Console.PrintError( $"AchievementManager.OnUserStatsStored: couldn't store stats - {pCallback.m_eResult}" );
				return;
			}
			if ( pCallback.m_nGameID != (ulong)SteamManager.AppID ) {
				ConsoleSystem.Console.PrintError( $"AchievementManager.OnUserStatsStored: incorrect appid!" );
				return;
			}

			ConsoleSystem.Console.PrintLine( "AchievementManager.OnUserStatsStored: successfully stored user stats." );
		}
	};
};