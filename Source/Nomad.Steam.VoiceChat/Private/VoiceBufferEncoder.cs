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

using Steamworks;

namespace Nomad.Steam.VoiceChat {
	/*
	===================================================================================

	VoiceBufferEncoder

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal readonly struct VoiceBufferEncoder {
		private const int MAX_COMPRESSED_SIZE = 1024;
		private const int MAX_PACKET_SIZE = 1029;
		private const uint SAMPLE_RATE = 44100;
		private const float VOICE_THRESHOLD = 0.05f;

		private readonly byte[] _destBuffer = new byte[ 44100 ];

		/*
		===============
		VoiceBufferEncoder
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public VoiceBufferEncoder() {
			uint bufferSize = SteamUser.GetVoiceOptimalSampleRate();
			_destBuffer = new byte[ bufferSize ];
		}

		/*
		===============
		CaptureVoice
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool CaptureVoice() {
			EVoiceResult result = SteamUser.GetAvailableVoice( out uint compressedSize );
			if ( result != EVoiceResult.k_EVoiceResultOK ) {
				return false;
			}

			result = SteamUser.GetVoice( true, _destBuffer, compressedSize, out uint bytesWriten );
			if ( result != EVoiceResult.k_EVoiceResultOK ) {
				return false;
			}

			return true;
		}
	};
};
