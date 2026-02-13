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
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;

namespace Nomad.Events.Private {
	/*
	===================================================================================

	SubscriptionEntry

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SubscriptionEntry<TArgs>
		where TArgs : struct
	{
		private readonly WeakReference<object>? _owner;
		private readonly EventCallback<TArgs>? _callback;
		private readonly AsyncEventCallback<TArgs>? _asyncCallback;

		public bool IsAsync => _asyncCallback != null;
		public bool IsAlive => _owner != null && _owner.TryGetTarget( out _ );

		/*
		===============
		SubscriptionEntry
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="callback"></param>
		/// <param name="asyncCallback"></param>
		public SubscriptionEntry( WeakReference<object>? owner, EventCallback<TArgs>? callback, AsyncEventCallback<TArgs>? asyncCallback ) {
			_owner = owner;
			_callback = callback;
			_asyncCallback = asyncCallback;
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static SubscriptionEntry<TArgs> Create( object? owner, EventCallback<TArgs>? callback ) {
			ArgumentGuard.ThrowIfNull( owner );
			ArgumentGuard.ThrowIfNull( callback );
			
			return new SubscriptionEntry<TArgs>( new WeakReference<object>( owner ), callback, null );
		}

		/*
		===============
		CreateAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static SubscriptionEntry<TArgs> CreateAsync( object? owner, AsyncEventCallback<TArgs>? callback ) {
			ArgumentGuard.ThrowIfNull( owner );
			ArgumentGuard.ThrowIfNull( callback );
			
			return new SubscriptionEntry<TArgs>( new WeakReference<object>( owner ), null, callback );
		}

		/*
		===============
		TryGetCallback
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public bool TryGetCallback( out EventCallback<TArgs>? callback ) {
			if ( !_owner.TryGetTarget( out _ ) ) {
				callback = null;
				return false;
			}

			callback = _callback;
			return callback != null;
		}

		/*
		===============
		TryGetAsyncCallback
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public bool TryGetAsyncCallback( out AsyncEventCallback<TArgs>? callback ) {
			if ( !IsAlive ) {
				callback = null;
				return false;
			}

			callback = _asyncCallback;
			return callback != null;
		}

		/*
		===============
		Matches
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public bool Matches( object owner, EventCallback<TArgs> callback ) {
			return _callback == callback &&
				   _owner != null &&
				   _owner.TryGetTarget( out var target ) &&
					ReferenceEquals( target, owner );
		}

		/*
		===============
		MatchesAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public bool MatchesAsync( object owner, AsyncEventCallback<TArgs> callback ) {
			return _asyncCallback == callback &&
				   _owner != null &&
				   _owner.TryGetTarget( out var target ) &&
					target == owner;
		}

		/*
		===============
		OwnerMatches
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public bool OwnerMatches( object owner ) {
			return _owner != null &&
				   _owner.TryGetTarget( out var target ) &&
				   ReferenceEquals( target, owner );
		}
	};
};
