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

using Nomad.Multiplayer;
using Steamworks;

namespace Nomad.Steam {
	/*
	===================================================================================

	SteamVoiceService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamVoiceService : IVoiceChatService {
		public bool Enabled {
			get => _enabled;
			set {
				_enabled = value;
				SetEnabled( value );
			}
		}
		private bool _enabled;

		/*
		===============
		SteamVoiceService
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public SteamVoiceService() {
			SetEnabled( true );
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
			SetEnabled( false );
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="delta"></param>
		public void Update( float delta ) {
			SteamUser.GetVoice( true, );
		}

		/*
		===============
		SetEnabled
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="enabled"></param>
		private void SetEnabled( bool enabled ) {
		}
	};
};
