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

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Nomad.Audio.Fmod.Private.Services;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.ValueObjects;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODBusRepository

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODBusRepository {
		public ConcurrentDictionary<string, SoundCategory> Categories => _categories;
		private readonly ConcurrentDictionary<string, SoundCategory> _categories = new();

		private readonly FMODDevice _fmodSystem;

		/*
		===============
		FMODBusRepository
		===============
		*/
		/// <summary>
		/// Creates and FMODBusRepository
		/// </summary>
		/// <param name="fmodSystem"></param>
		public FMODBusRepository( FMODDevice fmodSystem ) {
			_fmodSystem = fmodSystem;

			_categories[ "SoundCategory:UI" ] = new SoundCategory(
				Config: new SoundCategoryCreateInfo(
					Name: "SoundCategory:UI",
					MaxSimultaneous: 4,
					PriorityScale: 1.5f,
					StealProtectionTime: 0.2f,
					AllowStealingFromSameCategory: false
				),
				System: fmodSystem.StudioSystem
			);
		}

		/*
		===============
		GetSoundCategory
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public SoundCategory GetSoundCategory( string categoryName ) {
			return _categories[ categoryName ];
		}

		/*
		===============
		CreateCategory
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="createInfo"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void CreateCategory( SoundCategoryCreateInfo createInfo ) {
			_categories[ createInfo.Name ] = new SoundCategory(
				Config: createInfo,
				_fmodSystem.StudioSystem
			);
		}
	};
};
