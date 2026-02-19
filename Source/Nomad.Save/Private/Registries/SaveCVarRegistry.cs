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

using Nomad.Core.EngineUtils;
using Nomad.CVars.Interfaces;
using Nomad.CVars.ValueObjects;

namespace Nomad.Save.Private.Registries {
	/*
	===================================================================================
	
	SaveCVarRegistry
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal static class SaveCVarRegistry {
		/*
		===============
		RegisterCVars
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="engineService"></param>
		/// <param name="cvarSystem"></param>
		public static void RegisterCVars( IEngineService engineService, ICVarSystemService cvarSystem ) {
			cvarSystem.Register(
				new CVarCreateInfo<string>(
					Name: Constants.CVars.DATA_PATH,
					DefaultValue: $"{engineService.GetStoragePath( StorageScope.UserData )}/SaveData",
					Description: "The directory where all save data is written to.",
					Flags: CVarFlags.Init | CVarFlags.Archive
				)
			);
			cvarSystem.Register(
				new CVarCreateInfo<int>(
					Name: Constants.CVars.MAX_BACKUPS,
					DefaultValue: 3,
					Description: "The maximum number of automatically created backups that are allowed to exist per save file.",
					Flags: CVarFlags.Init | CVarFlags.Archive
				)
			);
			cvarSystem.Register(
				new CVarCreateInfo<bool>(
					Name: Constants.CVars.AUTO_SAVE_ENABLED,
					DefaultValue: true,
					Description: "Enables auto-saving features.",
					Flags: CVarFlags.Archive
				)
			);
		}
	};
};