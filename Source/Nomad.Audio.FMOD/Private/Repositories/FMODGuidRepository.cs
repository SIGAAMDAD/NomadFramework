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

using System;
using System.Collections.Generic;
using System.Threading;
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.Fmod.ValueObjects;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;
using Nomad.Core.Abstractions;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODGuidRepository

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODGuidRepository : IGuidRepository<FMODEventId, FMODBankId> {
		private sealed class GUIDCache<TGuid, TId>()
			where TGuid : IValueObject<TGuid>
			where TId : notnull
		{
			private readonly Dictionary<TGuid, TId> _guids = new Dictionary<TGuid, TId>();
			private readonly Dictionary<TId, TGuid> _reverseLookup = new Dictionary<TId, TGuid>();

			public TGuid this[ TId id ] => _reverseLookup[ id ];
			public TId this[ TGuid guid ] => _guids[ guid ];

			/*
			===============
			Add
			===============
			*/
			public void Add( TId path, TGuid guid ) {
				_guids[ guid ] = path;
				_reverseLookup[ path ] = guid;
			}

			/*
			===============
			Clear
			===============
			*/
			public void Clear() {
				_guids.Clear();
				_reverseLookup.Clear();
			}
		};

		private readonly GUIDCache<FMODEventId, string> _eventGuids = new GUIDCache<FMODEventId, string>();
		private readonly GUIDCache<FMODBankId, string> _bankGuids = new GUIDCache<FMODBankId, string>();

		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_eventGuids.Clear();
			_bankGuids.Clear();
		}

		/*
		===============
		AddEventId
		===============
		*/
		/// <summary>
		/// Adds an event's id to the guid cache.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="guid"></param>
		public void AddEventId( string path, FMODEventId guid ) {
			_lock.EnterWriteLock();
			try {
				_eventGuids.Add( path, guid );
			}
			finally {
				_lock.ExitWriteLock();
			}
		}

		/*
		===============
		AddBankId
		===============
		*/
		/// <summary>
		/// Adds a bank's id to the guid cache.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="guid"></param>
		public void AddBankId( string path, FMODBankId guid ) {
			_lock.EnterWriteLock();
			try {
				_bankGuids.Add( path, guid );
			}
			finally {
				_lock.ExitWriteLock();
			}
		}

		/*
		===============
		GetEventId
		===============
		*/
		public string GetEventId( FMODEventId guid ) {
			_lock.EnterUpgradeableReadLock();
			try {
				return _eventGuids[ guid ];
			}
			finally {
				_lock.ExitUpgradeableReadLock();
			}
		}

		/*
		===============
		GetEventGuid
		===============
		*/
		public FMODEventId GetEventGuid( string id ) {
			_lock.EnterUpgradeableReadLock();
			try {
				return _eventGuids[ id ];
			}
			finally {
				_lock.ExitUpgradeableReadLock();
			}
		}

		/*
		===============
		GetBankId
		===============
		*/
		public string GetBankId( FMODBankId guid ) {
			_lock.EnterUpgradeableReadLock();
			try {
				return _bankGuids[ guid ];
			}
			finally {
				_lock.ExitUpgradeableReadLock();
			}
		}

		/*
		===============
		GetBankGuid
		===============
		*/
		public FMODBankId GetBankGuid( string id ) {
			_lock.EnterUpgradeableReadLock();
			try {
				return _bankGuids[ id ];
			}
			finally {
				_lock.ExitUpgradeableReadLock();
			}
		}
	};
};
