/*
===========================================================================
The Nomad MPL Source Code
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
using Godot;
using Nomad.EngineUtils;
using Steamworks;

namespace Nomad.OnlineServices.Steam {
	/*
	===================================================================================

	SteamAchievementInfo

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamAchievementInfo {
		private struct AchievementProgress {
			public float Progress;
			public float MinProgress;
			public float MaxProgress;
		};

		public bool HasProgress => _progress.HasValue;
		public float Progress {
			get {
				return _progress.HasValue ? _progress.Value.Progress : 0.0f;
			}
		}
		public float MinProgress {
			get {
				return _progress.HasValue ? _progress.Value.MinProgress : 0.0f;
			}
		}
		public float MaxProgress {
			get {
				return _progress.HasValue ? _progress.Value.MaxProgress : 0.0f;
			}
		}
		private AchievementProgress? _progress;

		public bool Achieved => _achieved;
		private bool _achieved = false;

		public string Name => _name;
		private readonly string _name;

		/*
		===============
		SteamAchievementInfo
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		public SteamAchievementInfo( string name ) {
			_name = name;
			SteamUserStats.GetAchievementAndUnlockTime( _name, out _achieved, out uint unlockedTime );

			if ( SteamUserStats.GetAchievementProgressLimits( _name, out float minProgress, out float maxProgress ) ) {
				SteamUserStats.GetAchievementAchievedPercent( _name, out float progress );
				_progress = new AchievementProgress {
					MinProgress = minProgress,
					MaxProgress = maxProgress,
					Progress = progress
				};
			}

			// query the api for the achievement's icon
			SteamUserStats.GetAchievementIcon( _name );
		}

		/*
		===============
		SetIcon
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="pAchievement"></param>
		public void SetIcon( UserAchievementIconFetched_t pAchievement, IEngineService service ) {
			SteamUtils.GetImageSize( pAchievement.m_nIconHandle, out uint width, out uint height );

			byte[] imageBuffer = new byte[ width * height * 4 ];
			SteamUtils.GetImageRGBA( pAchievement.m_nIconHandle, imageBuffer, imageBuffer.Length );

			IDisposable texture = service.CreateImage( imageBuffer, (int)width, (int)height );
		}
	};
};
