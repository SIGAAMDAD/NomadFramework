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

using Nomad.Audio.Fmod.ValueObjects;
using Nomad.CVars;
using Nomad.Core;

namespace Nomad.Audio.Fmod.Private.Registries {
	/*
	===================================================================================

	FMODCVarRegistry

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public static class FMODCVarRegistry {
		/*
		===============
		Register
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="cvarSystem"></param>
		public static void Register( ICVarSystemService cvarSystem ) {
			cvarSystem.Register(
				new CVarCreateInfo<int>(
					Name: Constants.CVars.Audio.FMOD.STREAM_BUFFER_SIZE,
					DefaultValue: 12,
					Description: "The size of FMOD's stream buffer in milliseconds.",
					Flags: CVarFlags.Archive | CVarFlags.Init,
					Validator: value => value > 100 && value <= 5000
				)
			);
			cvarSystem.Register(
				new CVarCreateInfo<int>(
					Name: Constants.CVars.Audio.FMOD.DSP_BUFFER_SIZE,
					DefaultValue: 12,
					Description: "The size of FMOD's dsp buffer in MB.",
					Flags: CVarFlags.Archive | CVarFlags.Init,
					Validator: value => value > 10 && value < 48
				)
			);
			cvarSystem.Register(
				new CVarCreateInfo<bool>(
					Name: Constants.CVars.Audio.FMOD.LOGGING,
#if DEBUG
					DefaultValue: true,
#else
					DefaultValue: false,
#endif
					Description: "Enables a dedicated FMOD debug log.",
					Flags: CVarFlags.Developer | CVarFlags.ReadOnly
				)
			);
			cvarSystem.Register(
				new CVarCreateInfo<FMODBankLoadingStrategy>(
					Name: Constants.CVars.Audio.FMOD.BANK_LOADING_STRATEGY,
					DefaultValue: FMODBankLoadingStrategy.Streaming,
					Description: "Sets the loading policy for how FMOD banks are handled in memory.",
					Flags: CVarFlags.ReadOnly | CVarFlags.Archive,
					Validator: value => value >= FMODBankLoadingStrategy.Streaming && value < FMODBankLoadingStrategy.Compressed
				)
			);
		}
	};
};
