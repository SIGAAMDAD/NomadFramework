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

namespace Nomad.Audio.Fmod.Private.ValueObjects {
	/*
	===================================================================================

	FMODBankResource

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal readonly struct FMODBankResource : IAudioResource {
		private readonly FMOD.Studio.Bank _instance;

		public readonly bool IsValid => _instance.isValid();

		/*
		===============
		FMODBankResource
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		public FMODBankResource( FMOD.Studio.Bank instance ) {
			_instance = instance;
		}

		/*
		===============
		Unload
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Unload() {
			if ( _instance.isValid() ) {
				FMODValidator.ValidateCall( _instance.unloadSampleData() );
				FMODValidator.ValidateCall( _instance.unload() );
				_instance.clearHandle();
			}
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
			Unload();
		}

		public static implicit operator FMOD.Studio.Bank( FMODBankResource bank ) => bank._instance;
		public static bool operator ==( FMODBankResource a, FMODBankResource b ) => a._instance.handle == b._instance.handle;
		public static bool operator !=( FMODBankResource a, FMODBankResource b ) => a._instance.handle != b._instance.handle;
	};
};
