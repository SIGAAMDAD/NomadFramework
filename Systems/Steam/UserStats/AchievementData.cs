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

using Godot;
using Steamworks;
using System;

namespace Steam.UserStats {
	/*
	===================================================================================
	
	AchievementData
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public struct AchievementData {
		/// <summary>
		/// The achievement's api name
		/// </summary>
		public readonly string? Name;

		/// <summary>
		/// The achievement's api description
		/// </summary>
		public readonly string? Description;

		/// <summary>
		/// Whether it's hidden or not
		/// </summary>
		public readonly bool Hidden;

		/// <summary>
		/// The achievement's api icon
		/// </summary>
		public readonly Texture2D Icon;

		/// <summary>
		/// Whether the achievment has been... achieved
		/// </summary>
		public readonly bool Achieved => _achieved;
		private bool _achieved = false;

		/// <summary>
		/// How much progress the achievement has
		/// </summary>
		public readonly float Progress => _progress;
		private float _progress = 0.0f;

		/// <summary>
		/// When the achievement reached completed status
		/// </summary>
		public readonly uint AchievedTime => _achievedTime;
		private uint _achievedTime = 0;

		/*
		===============
		AchievementData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">The achievement api id.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="id"/> isn't a valid <see cref="AchievementID"/>.</exception>
		public AchievementData( AchievementID id ) {
			if ( id < AchievementID.R_U_Cheating || id >= AchievementID.Count ) {
				throw new ArgumentOutOfRangeException( nameof( id ) );
			}

			Name = SteamUserStats.GetAchievementName( (uint)id );
			Description = SteamUserStats.GetAchievementDisplayAttribute( Name, "desc" );
			Hidden = Convert.ToBoolean( SteamUserStats.GetAchievementDisplayAttribute( Name, "hidden" ).ToInt() );
			Icon = GetIconData();
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// Updates the current achievement progress
		/// </summary>
		public void Update() {
			if ( !SteamUserStats.GetAchievementAndUnlockTime( Name, out _achieved, out _achievedTime ) ) {
				ConsoleSystem.Console.PrintError( $"AchievementData.Update: SteamUserStats.GetAchievementAndUnlockTime returned false for {Name}!" );
			}
			if ( !SteamUserStats.GetAchievementAchievedPercent( Name, out _progress ) ) {
				ConsoleSystem.Console.PrintError( $"AchievementData.Update: SteamUserStats.GetAchievementAchievedPercent returned false for {Name}!" );
			}
		}

		/*
		===============
		Activate
		===============
		*/
		/// <summary>
		/// Activates a steam achievement
		/// </summary>
		public void Activate() {
			if ( !SteamUserStats.SetAchievement( Name ) ) {
				ConsoleSystem.Console.PrintError( $"AchievementData.Activate: SteamUserStats.SetAchievement failed on '{Name}'" );
			}
		}

		/*
		===============
		GetIconData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns>The generated godot image icon.</returns>
		private Texture2D GetIconData() {
			int imageHandle = SteamUserStats.GetAchievementIcon( Name );
			if ( imageHandle == 0 ) {
			}

			SteamUtils.GetImageSize( imageHandle, out uint width, out uint height );

			byte[] imageData = new byte[ (int)width * (int)height * 4 ];
			SteamUtils.GetImageRGBA( imageHandle, imageData, imageData.Length );

			Image image = new Image();
			image.SetData( (int)width, (int)height, false, Image.Format.Rgba8, imageData );

			return ImageTexture.CreateFromImage( image );
		}
	};
};