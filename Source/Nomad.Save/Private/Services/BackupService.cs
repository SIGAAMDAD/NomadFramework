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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nomad.Core.Events;
using Nomad.Core.Exceptions;
using Nomad.Core.FileSystem;
using Nomad.CVars;
using Nomad.Save.Private.ValueObjects;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================
	
	BackupService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class BackupService : IDisposable {
		private readonly List<BackupData> _backups;
		private int _maxBackups = 0;

		private readonly ICVarSystemService _cvarSystem;
		private readonly IFileSystem _fileSystem;

		/*
		===============
		BackupService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cvarSystem"></param>
		/// <param name="fileSystem"></param>
		/// <param name="eventFactory"></param>
		/// <exception cref="CVarMissing"></exception>
		public BackupService( ICVarSystemService cvarSystem, IFileSystem fileSystem, IGameEventRegistryService eventFactory ) {
			var maxBackups = cvarSystem.GetCVar<int>( "Nomad.Save.MaxBackups" ) ?? throw new CVarMissing( "Nomad.Save.MaxBackups" );
			_maxBackups = maxBackups.Value;
			maxBackups.ValueChanged.Subscribe( this, OnMaxBackupsValueChanged );

			_cvarSystem = cvarSystem;
			_fileSystem = fileSystem;
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
		}

		/*
		===============
		CreateBackup
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="header"></param>
		/// <returns></returns>
		public async ValueTask CreateBackup( SaveHeader header ) {
			
		}

		/*
		===============
		OnMaxBackupsValueChanged
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnMaxBackupsValueChanged( in CVarValueChangedEventArgs<int> args ) {
		}
	};
};