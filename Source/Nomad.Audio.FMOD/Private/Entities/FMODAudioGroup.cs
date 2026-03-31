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

using Nomad.Audio.Interfaces;

namespace Nomad.Audio.Fmod.Private.Entities {
	/*
	===================================================================================
	
	FMODAudioGroup
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class FMODAudioGroup : IAudioGroup {
		public string Name => _name;
		private readonly string _name;

		public float Volume {
			get => _volume;
			set {
				_volume = value;
				_channelGroup.setVolume( value );
			}
		}
		private float _volume;

		public bool Muted {
			get => _muted;
			set {
				_muted = value;
				_channelGroup.setMute( value );
			}
		}
		private bool _muted;

		private readonly FMOD.ChannelGroup _channelGroup;

		/*
		===============
		FMODAudioGroup
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="system"></param>
		public FMODAudioGroup( string name, FMOD.Studio.System system ) {
			FMODValidator.ValidateCall( system.getBus( name, out var bus ) );
			FMODValidator.ValidateCall( bus.getChannelGroup( out _channelGroup ) );
			
			FMODValidator.ValidateCall( _channelGroup.getVolume( out _volume ) );
			FMODValidator.ValidateCall( _channelGroup.getMute( out _muted ) );
			FMODValidator.ValidateCall( _channelGroup.getName( out _name, 128 ) );
		}
	};
};