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

using Nomad.Core.Events;

namespace Nomad.Events.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class GameEventExtensions
	{
		/// <summary>
		/// Creates a one-time subscription handle for the provided GameEvent.
		/// </summary>
		/// <typeparam name="TArgs"></typeparam>
		/// <param name="gameEvent"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static ISubscriptionHandle SubscribeOnce<TArgs>(this IGameEvent<TArgs> gameEvent, EventCallback<TArgs> callback)
			where TArgs : struct
		{
			ISubscriptionHandle handle = null;
			EventCallback<TArgs> killAfterPublish = (in TArgs args) => { callback(in args); handle?.Dispose(); };
			handle = gameEvent.Subscribe(killAfterPublish);
			return handle;
		}
	}
}