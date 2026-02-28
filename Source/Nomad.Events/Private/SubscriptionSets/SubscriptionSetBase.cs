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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Nomad.Events.Private.SubscriptionSets {
	/*
	===================================================================================

	SubscriptionSetBase

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>
	/// TODO: finish this as the base class for all subscription sets, we have duplicated behaviors

	internal abstract class SubscriptionSetBase<TArgs> : IDisposable
		where TArgs : struct {
		protected readonly ILoggerService logger;
		protected readonly IGameEvent<TArgs> eventData;

		protected readonly SubscriptionCache<TArgs, EventCallback<TArgs>> genericSubscriptions;
		protected readonly SubscriptionCache<TArgs, AsyncEventCallback<TArgs>>? asyncSubscriptions;
		protected readonly HashSet<WeakReference<IGameEvent>> friends = new HashSet<WeakReference<IGameEvent>>();

		protected bool isDisposed = false;

		protected virtual bool SupportsAsync => false;

		/*
		===============
		SubscriptionSetBase
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="logger"></param>
		protected SubscriptionSetBase( IGameEvent<TArgs> eventData, ILoggerService logger ) {
			ArgumentGuard.ThrowIfNull( eventData );
			ArgumentGuard.ThrowIfNull( logger );

			this.eventData = eventData;
			this.logger = logger;
		}

		#region Locking hooks
		protected virtual void EnterRead() { }
		protected virtual void ExitRead() { }
		protected virtual void EnterWrite() { }
		#endregion

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose( bool disposing ) {
			if ( isDisposed ) {
				return;
			}
			if ( disposing ) {
				genericSubscriptions?.Dispose();
				asyncSubscriptions?.Dispose();
			}
			isDisposed = true;
		}
	}
}
