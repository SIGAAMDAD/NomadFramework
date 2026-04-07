/*
===========================================================================
The Nomad MPLv2 Source Code
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

namespace Nomad.Audio.Fmod.Private.Entities {
	internal sealed class FMODChannelHandle {
		public int ChannelId { get; }
		public int Generation { get; }

		public bool IsValid => _isValid;
		private bool _isValid = false;

		public event Action<FMODChannelHandle>? OnEnded;

		public FMODChannelHandle( int channelId, int generation ) {
			ChannelId = channelId;
			Generation = generation;
			_isValid = true;
			OnEnded = null;
		}

		public void Invalidate() {
			if ( !_isValid ) {
				return;
			}
			_isValid = false;
			OnEnded?.Invoke( this );
			OnEnded = null;
		}
	};
};