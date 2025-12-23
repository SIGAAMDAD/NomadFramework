/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Godot;
using Nomad.Audio.Interfaces;

namespace Nomad.Audio.Fmod.Private.Entities {
	/*
	===================================================================================

	FMODListener

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODListener( FMOD.Studio.System system, int index ) : IListener {
		public Vector2 Position {
			get => _position;
			set {
				if ( _position == value ) {
					return;
				}
				FMOD.ATTRIBUTES_3D attributes = new() {
					position = new FMOD.VECTOR { x = _position.X, y = _position.Y, z = 0.0f }
				};
				_system.setListenerAttributes( _listenerIndex, attributes );
			}
		}
		private Vector2 _position = Vector2.Zero;

		private readonly int _listenerIndex = index;
		private readonly FMOD.Studio.System _system = system;
	};
};
