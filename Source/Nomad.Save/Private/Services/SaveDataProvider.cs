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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.Save.Data;
using Nomad.Save.Events;
using Nomad.Save.Interfaces;
using Nomad.Save.Exceptions;
using Nomad.Save.Services;
using Nomad.Save.ValueObjects;
using Nomad.Save.Private.Entities;
using Nomad.Core.Exceptions;
using Nomad.Save.Private.Registries;
using Nomad.Core.Engine.Services;
using Nomad.Save.Private.Repositories;
using Nomad.Core.CVars;

namespace Nomad.Save.Private.Services {
	/*
	===================================================================================

	SaveDataProvider

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SaveDataProvider : ISaveDataProvider {
		private readonly ISaveWriterService _writerService;
		private readonly ISaveReaderService _readerService;

		private readonly IFileSystem _vfs;

		private readonly ILoggerService _logger;
		private readonly ILoggerCategory _category;

		private readonly SlotRepository _slotRepository;
		private readonly AtomicWriterService _atomicWriter;

		private SaveConfig _config;

		private readonly ISubscriptionHandle _autoSaveChanged;
		private readonly ISubscriptionHandle _autoSaveIntervalChanged;

		private readonly IGameEvent<SaveBeginEventArgs> _saveBegin;
		private readonly IGameEvent<LoadBeginEventArgs> _loadBegin;

		private bool _isDisposed = false;

		/*
		===============
		SaveDataProvider
		===============
		*/
		/// <summary>
		/// Creates a new SaveDataProvider object.
		/// </summary>
		/// <param name="engineService"></param>
		/// <param name="eventFactory"></param>
		/// <param name="cvarSystem"></param>
		/// <param name="fileSystem"></param>
		/// <param name="logger"></param>
		public SaveDataProvider( IEngineService engineService, IGameEventRegistryService eventFactory, ICVarSystemService cvarSystem, IFileSystem fileSystem, ILoggerService logger ) {
			ArgumentGuard.ThrowIfNull( engineService );
			ArgumentGuard.ThrowIfNull( eventFactory );
			ArgumentGuard.ThrowIfNull( fileSystem );
			ArgumentGuard.ThrowIfNull( logger );
			ArgumentGuard.ThrowIfNull( cvarSystem );

			_saveBegin = eventFactory.GetEvent<SaveBeginEventArgs>( EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT );
			_loadBegin = eventFactory.GetEvent<LoadBeginEventArgs>( EventNames.NAMESPACE, EventNames.LOAD_BEGIN_EVENT );

			_vfs = fileSystem;
			_logger = logger;
			_category = logger.CreateCategory( nameof( Nomad.Save ), LogLevel.Info, true );

			_config = InitConfiguration( engineService, cvarSystem );

			var autoSaveEnabled = cvarSystem.GetCVar<bool>( Constants.CVars.AUTO_SAVE_ENABLED ) ?? throw new CVarMissing( Constants.CVars.AUTO_SAVE_ENABLED );
			_autoSaveChanged = autoSaveEnabled.ValueChanged.Subscribe( OnAutoSaveEnabledChanged );

			var autoSaveInterval = cvarSystem.GetCVar<int>( Constants.CVars.AUTO_SAVE_INTERVAL ) ?? throw new CVarMissing( Constants.CVars.AUTO_SAVE_INTERVAL );
			_autoSaveIntervalChanged = autoSaveInterval.ValueChanged.Subscribe( OnAutoSaveIntervalChanged );

			_atomicWriter = new AtomicWriterService( engineService, fileSystem );

			_slotRepository = new SlotRepository( fileSystem, logger, _config );
			_readerService = new SaveReaderService( _config, _slotRepository, _vfs, _logger );
			_writerService = new SaveWriterService( _config, _atomicWriter, _slotRepository, _vfs, _logger );
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
			if ( !_isDisposed ) {
				_readerService?.Dispose();
				_writerService?.Dispose();
				_slotRepository?.Dispose();
				_category?.Dispose();

				_autoSaveChanged?.Dispose();
				_autoSaveIntervalChanged?.Dispose();

				_saveBegin?.Dispose();
				_loadBegin?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		ListSaveFiles
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IReadOnlyList<SaveFileMetadata> ListSaveFiles()
			=> _slotRepository.GetMetadataList();

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task Load( string name ) {
			try {
				_readerService.Load( name );

				_loadBegin.Publish( new LoadBeginEventArgs( _readerService ) );
			} catch ( FieldCorruptException fieldCorrupt ) {
				_logger.PrintError( $"Field corruption: [FieldIndex] {fieldCorrupt.FieldIndex}, [FileOffset] {fieldCorrupt.FileOffset}, [Section] {fieldCorrupt.SectionName} - {fieldCorrupt.Message}" );
			} catch ( Exception e ) {
				_logger.PrintError( $"Exception caught - {e}" );
			}
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <param name="gameVersion"></param>
		/// <returns></returns>
		public async Task Save( string name, GameVersion gameVersion ) {
			try {
				_writerService.BeginSave( name, gameVersion );
				_saveBegin.Publish( new SaveBeginEventArgs( _writerService ) );
				_writerService.EndSave( name, gameVersion );
			} catch ( Exception e ) {
				_logger.PrintError( $"Exception caught - {e}" );
			}
		}

		/*
		===============
		InitConfiguration
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="engineService"></param>
		/// <param name="cvarSystem"></param>
		/// <exception cref="CVarMissing"></exception>
		private static SaveConfig InitConfiguration( IEngineService engineService, ICVarSystemService cvarSystem ) {
			SaveCVarRegistry.RegisterCVars( engineService, cvarSystem );

			var dataPath = cvarSystem.GetCVar<string>( Constants.CVars.DATA_PATH ) ?? throw new CVarMissing( Constants.CVars.DATA_PATH );

			var backupPath = cvarSystem.GetCVar<string>( Constants.CVars.BACKUP_DIRECTORY ) ?? throw new CVarMissing( Constants.CVars.BACKUP_DIRECTORY );
			var maxBackups = cvarSystem.GetCVar<int>( Constants.CVars.MAX_BACKUPS ) ?? throw new CVarMissing( Constants.CVars.MAX_BACKUPS );

			var autoSaveEnabled = cvarSystem.GetCVar<bool>( Constants.CVars.AUTO_SAVE_ENABLED ) ?? throw new CVarMissing( Constants.CVars.AUTO_SAVE_ENABLED );
			var autoSaveInterval = cvarSystem.GetCVar<int>( Constants.CVars.AUTO_SAVE_INTERVAL ) ?? throw new CVarMissing( Constants.CVars.AUTO_SAVE_INTERVAL );

			var checksumsEnabled = cvarSystem.GetCVar<bool>( Constants.CVars.CHECKSUM_ENABLED ) ?? throw new CVarMissing( Constants.CVars.CHECKSUM_ENABLED );
			var verifyAfterWrite = cvarSystem.GetCVar<bool>( Constants.CVars.VERIFY_AFTER_WRITE ) ?? throw new CVarMissing( Constants.CVars.VERIFY_AFTER_WRITE );
			var logSerializationTree = cvarSystem.GetCVar<bool>( Constants.CVars.LOG_SERIALIZATION_TREE ) ?? throw new CVarMissing( Constants.CVars.LOG_SERIALIZATION_TREE );
			var logWriteTimings = cvarSystem.GetCVar<bool>( Constants.CVars.LOG_WRITE_TIMINGS ) ?? throw new CVarMissing( Constants.CVars.LOG_WRITE_TIMINGS );
			var debugLogging = cvarSystem.GetCVar<bool>( Constants.CVars.DEBUG_LOGGING ) ?? throw new CVarMissing( Constants.CVars.DEBUG_LOGGING );

			return new SaveConfig {
				DataPath = dataPath.Value,

				BackupPath = backupPath.Value,
				MaxBackups = maxBackups.Value,

				AutoSaveInterval = autoSaveInterval.Value,
				AutoSave = autoSaveEnabled.Value,

				ChecksumEnabled = checksumsEnabled.Value,
				VerifyAfterWrite = verifyAfterWrite.Value,
				LogSerializationTree = logSerializationTree.Value,
				LogWriteTimings = logWriteTimings.Value,
				DebugLogging = debugLogging.Value
			};
		}

		/*
		===============
		OnAutoSaveIntervalChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnAutoSaveIntervalChanged( in CVarValueChangedEventArgs<int> args ) {
			_config = _config with { AutoSaveInterval = args.NewValue };
		}

		/*
		===============
		OnAutoSaveEnabledChanged
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="args"></param>
		private void OnAutoSaveEnabledChanged( in CVarValueChangedEventArgs<bool> args ) {
			_config = _config with { AutoSave = args.NewValue };
		}
	};
};
