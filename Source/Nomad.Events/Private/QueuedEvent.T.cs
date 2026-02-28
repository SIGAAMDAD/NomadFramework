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

using System.Runtime.CompilerServices;
using Nomad.Core.Events;

namespace Nomad.Events.Private {
	/*
	===================================================================================
	
	QueuedEvent
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class QueuedEvent<TArgs> : QueuedEvent
		where TArgs : struct {
		private readonly IGameEvent<TArgs> _gameEvent;
		private readonly TArgs _args;

		/*
		===============
		QueuedEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameEvent"></param>
		/// <param name="args"></param>
		public QueuedEvent( IGameEvent<TArgs> gameEvent, in TArgs args ) {
			_gameEvent = gameEvent;
			_args = args;
		}

		/*
		===============
		Process
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Process()
			=> _gameEvent.Publish( in _args );
	};
};
