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
using Nomad.Audio.Fmod.Private.ValueObjects;
using Nomad.Audio.ValueObjects;
using Nomad.Core.Logger;
using Nomad.Core.Util;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODEventRepository

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class FMODEventRepository( ILoggerService logger, FMOD.Studio.System system ) : IDisposable {
		private readonly Dictionary<EventHandle, FMODEventResource> _eventCache = new();

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			_eventCache.Clear();
		}

		/*
		===============
		CreateEvent
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="eventHandle"></param>
		/// <returns></returns>
		public AudioResult CreateEvent( string assetPath, out EventHandle eventHandle ) {
			eventHandle = new( assetPath.HashFileName() );
			if ( _eventCache.TryGetValue( eventHandle, out _ ) ) {
				return AudioResult.Success;
			}

			FMODValidator.ValidateCall( system.getEvent( assetPath, out var eventDescription ) );
			_eventCache.Add( eventHandle, new( eventDescription ) );
			return AudioResult.Success;
		}

		/*
		===============
		GetEventDescription
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventHandle"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		public AudioResult GetEventDescription( EventHandle eventHandle, out FMODEventResource description ) {
			if ( !_eventCache.TryGetValue( eventHandle, out description ) ) {
				return AudioResult.Error_ResourceNotFound;
			}
			return AudioResult.Success;
		}

		/*
		===============
		GetEventDescription
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		public AudioResult GetEventDescription( string assetPath, out FMODEventResource description ) {
			EventHandle eventHandle = new( assetPath.HashFileName() );
			if ( !_eventCache.TryGetValue( eventHandle, out description ) ) {
				return AudioResult.Error_ResourceNotFound;
			}
			return AudioResult.Success;
		}
	};
};
