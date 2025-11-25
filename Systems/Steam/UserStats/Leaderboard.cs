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
using System.Collections.Generic;
using System.Threading;

namespace Steam.UserStats {
	/*
	===================================================================================
	
	Leaderboard
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class Leaderboard {
		public readonly struct Entry( LeaderboardEntry_t entry, int[] details ) {
			public readonly CSteamID UserId = entry.m_steamIDUser;
			public readonly int Score = entry.m_nScore;
			public readonly int Ranking = entry.m_nGlobalRank;
		};

		public readonly Dictionary<int, Entry> Entries = new Dictionary<int, Entry>();

		private SteamLeaderboard_t Handle;
		private SteamLeaderboardEntries_t LeaderboardEntries;

		private static readonly CallResult<LeaderboardFindResult_t> OnLeaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create( OnFindLeaderboard );
		private static readonly CallResult<LeaderboardScoreUploaded_t> OnLeaderboardScoreUploaded = CallResult<LeaderboardScoreUploaded_t>.Create( OnScoreUploaded );
		private static readonly CallResult<LeaderboardScoresDownloaded_t> OnLeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create( OnScoreDownloaded );

		private static readonly object LeaderboardFound = new object();
		private static readonly object LeaderboardReady = new object();

		/*
		===============
		Leaderboard
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public Leaderboard( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			SteamAPICall_t handle = SteamUserStats.FindLeaderboard( name );
			OnLeaderboardFindResult.Set( handle );
		}

		public static Leaderboard Create( string? name ) {
			Leaderboard leaderboard = new Leaderboard( name );
			return leaderboard;
		}

		/*
		===============
		OnFindLeaderboard
		===============
		*/
		private static void OnFindLeaderboard( LeaderboardFindResult_t pCallback, bool bIOFailure ) {

		}

		/*
		===============
		OnScoreUploaded
		===============
		*/
		private static void OnScoreUploaded( LeaderboardScoreUploaded_t pCallback, bool bIOFailure ) {
			if ( pCallback.m_bSuccess == 0 ) {
				ConsoleSystem.Console.PrintError( "[STEAM] Error uploading stats to global leaderboards!" );
				return;
			}
		}

		/*
		===============
		OnScoreDownloaded
		===============
		*/
		private static void OnScoreDownloaded( LeaderboardScoresDownloaded_t pCallback, bool bIOFailure ) {
			int[] details = new int[ 4 ];
			SteamLeaderboardEntries_t entries = pCallback.m_hSteamLeaderboardEntries;
			/*

			for ( int i = 0; i < pCallback.m_cEntryCount; i++ ) {
				if ( !SteamUserStats.GetDownloadedLeaderboardEntry( LeaderboardEntries, i, out LeaderboardEntry_t entry, details, details.Length ) ) {
					ConsoleSystem.Console.PrintError( "[STEAM] Error fetching downloaded leaderboard entry!" );
					continue;
				}
				Entries.Add( entry.m_nGlobalRank, new LeaderboardEntry( entry, details ) );
			}
			Monitor.Pulse( LeaderboardReady );
			*/
		}
	};
};