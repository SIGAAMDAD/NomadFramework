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
using Nomad.Core.Logger;

namespace Nomad.Events.Private {
	/*
	===================================================================================

	SubscriptionCache

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SubscriptionCache<TArgs> : IDisposable
		where TArgs : struct
	{
#if EVENT_DEBUG
		public int DeadCount {
			get {
				int dead = 0;
				for ( int i = 0; i < _subscriptions.Count; i++ ) {
					if ( !_subscriptions[ i ].IsAlive ) {
						dead++;
					}
				}
				return dead;
			}
		}
#endif

		public int Count => _subscriptions.Count;
		public SubscriptionEntry<TArgs> this[ int index ] => _subscriptions[ index ];

		private readonly List<SubscriptionEntry<TArgs>> _subscriptions = new List<SubscriptionEntry<TArgs>>( 16 );

		private int _cleanupCursor = 0;
		private readonly ILoggerService _logger;

		/*
		===============
		SubscriptionCache
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="logger"></param>
		public SubscriptionCache( ILoggerService logger ) {
			_logger = logger;
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
			_subscriptions.Clear();
		}

		/*
		===============
		Add
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="entry"></param>
		public void Add( SubscriptionEntry<TArgs> entry ) {
			_subscriptions.Add( entry );
		}

		/*
		===============
		RemoveAt
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt( int index ) {
			_subscriptions.RemoveAt( index );
		}

		/*
		===============
		RemoveAllOwnedBy
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		public void RemoveAllOwnedBy( object owner ) {
			for ( int i = _subscriptions.Count - 1; i >= 0; i-- ) {
				if ( _subscriptions[ i ].OwnerMatches( owner  ) ) {
					_subscriptions.RemoveAt( i );
				}
			}
		}

		/*
		===============
		CleanupFull
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void CleanupFull() {
			for ( int i = _subscriptions.Count - 1; i >= 0; i-- ) {
				if ( !_subscriptions[ i ].IsAlive ) {
					_subscriptions.RemoveAt( i );
				}
			}
			_cleanupCursor = 0;
		}

		/*
		===============
		CleanupIncremental
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="maxSteps"></param>
		public void CleanupIncremental( int maxSteps ) {
			if ( maxSteps <= 0 ) {
				return;
			}

			int count = _subscriptions.Count;
			for ( int step = 0; step < maxSteps && count > 0; step++ ) {
				if ( _cleanupCursor >= count ) {
					_cleanupCursor = 0;
					break;
				}
				if ( !_subscriptions[ _cleanupCursor ].IsAlive ) {
					_subscriptions.RemoveAt( _cleanupCursor );
					count--;
					continue;
				}

				_cleanupCursor++;
			}
		}
	};
};
