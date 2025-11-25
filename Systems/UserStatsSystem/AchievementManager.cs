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

using NomadCore.Systems.EventSystem;
using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace NomadCore.Systems.UserStatsSystem {
	/*
	===================================================================================
	
	AchievementManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class AchievementManager {
		private static Godot.Collections.Dictionary Achievements;

		public static readonly GameEvent AchievementUnlocked = new GameEvent( nameof( AchievementUnlocked ) );
		public static readonly GameEvent AchievementProgressed = new GameEvent( nameof( AchievementProgressed ) );
		public static readonly GameEvent AchievementReset = new GameEvent( nameof( AchievementReset ) );

		public static readonly GameEvent AchievementsReset = new GameEvent( nameof( AchievementsReset ) );
		public static readonly GameEvent AchievementsLoaded = new GameEvent( nameof( AchievementsLoaded ) );

		/*
		===============
		AchievementManager
		===============
		*/
		static AchievementManager() {
		}

		/*
		===============
		SaveAchievements
		===============
		*/
		private static void SaveAchievements() {
			System.IO.File.WriteAllBytes(
				ProjectSettings.GlobalizePath( "res://achievements.json" ),
				Json.Stringify( Achievements, "\t" ).ToAsciiBuffer()
			);
		}
	};
};