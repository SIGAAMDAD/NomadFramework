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

namespace Nomad.Events.Private {
	/*
	===================================================================================

	WeakSubscription

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class WeakSubscription<TArgs, TCallback>
		where TArgs : struct
		where TCallback : class
	{
		public readonly WeakReference<object> Subscriber;
		public readonly TCallback Callback;
		
		public bool IsAlive => Subscriber.TryGetTarget( out _ );

		public WeakSubscription( object subscriber, TCallback callback ) {
			Subscriber = new WeakReference<object>( Subscriber );
			Callback = callback;
		}
	};
};
