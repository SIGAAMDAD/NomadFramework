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

using System.Runtime.CompilerServices;

namespace NomadCore.Systems.LobbySystem {
	/*
	===================================================================================
	
	VoiceActivity
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public struct VoiceActivity {
		private const float VOICE_DECAY_RATE = 0.1f;

		/// <summary>
		/// Does this source have any activity?
		/// </summary>
		public float Volume { get; private set; }

		/// <summary>
		/// Is this voice source active?
		/// </summary>
		public bool Active { get; private set; }

		/// <summary>
		/// Is this voice source muted?
		/// </summary>
		public bool Muted {
			readonly get => _muted;
			set {
				if ( value ) {
					Active = false;
					Volume = 0.0f;
				}
				_muted = value;
			}
		}
		private bool _muted;

		/*
		===============
		VoiceActivity
		===============
		*/
		public VoiceActivity() {
			_muted = false;
			Active = false;
			Volume = 0.0f;
		}

		/*
		===============
		SetActivity
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="volume"></param>
		[MethodImpl( MethodImplOptions.AggressiveOptimization )]
		public void SetActivity( float volume ) {
			if ( _muted ) {
				return;
			}
			Volume = volume;
			Active = true;
		}

		/*
		===============
		CheckDecay
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void CheckDecay( float delta ) {
			if ( Volume > 0.0f ) {
				Volume -= VOICE_DECAY_RATE * (float)delta;
				if ( Volume < 0.0f ) {
					Volume = 0.0f;
				}
				Active = true;
			}
			Active = false;
		}
	};
};