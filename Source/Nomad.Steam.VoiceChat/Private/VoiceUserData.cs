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

using Nomad.Core;

namespace Nomad.Steam.VoiceChat {
	internal sealed class VoiceUserProcessor {
		private struct VoiceUser {
			public float Volume;
			public bool Muted;
			public bool Active;
		};

		private const float VOICE_DECAY_RATE = 0.1f;

		private readonly VoiceUser[] _voices = new VoiceUser[ Constants.Multiplayer.MAX_PLAYERS ];
		private int _numVoices = 0;

		/*
		===============
		VoiceUserProcessor
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public VoiceUserProcessor() {
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
			for ( int i = 0; i < _numVoices; i++ ) {
				var voice = _voices[ i ];
				if ( voice.Muted ) {
					continue;
				}
				float amount = voice.Volume;
				if ( amount > 0.0f ) {
					amount -= VOICE_DECAY_RATE * delta;
					if ( amount < 0.0f ) {
						amount = 0.0f;
					}
					voice.Volume = amount;
				}
				voice.Active = false;
			}
		}
	};
};
