/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Domain.Models.Interfaces;
using NomadCore.GameServices;
using NomadCore.Systems.Audio.Application;
using NomadCore.Systems.Audio.Domain.Interfaces;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Exceptions;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Models.Entities;
using NomadCore.Systems.Audio.Infrastructure.Fmod.Models.ValueObjects;
using System.Collections.Generic;

namespace NomadCore.Systems.Audio.Infrastructure.Fmod.Services {
	/*
	===================================================================================
	
	FMODListenerService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class FMODListenerService : IListenerService {
		public int ListenerCount => _listenerCount;
		private int _listenerCount = 0;

		public IListener ActiveListener => _currentListener;
		private FMODListener _currentListener;

		private readonly FMODListener[] _listeners = new FMODListener[ AudioService.MAX_LISTENERS ];
		private readonly FMODSystemHandle _system;
		private readonly ILoggerService _logger;

		public FMODListenerService( ILoggerService logger, in FMODSystemHandle system ) {
			_logger = logger;
			_system = system;
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			ClearListeners();
		}

		/*
		===============
		AddListener
		===============
		*/
		public IListener AddListener( IGameEntity entity ) {
			if ( _listenerCount >= _listeners.Length ) {
				throw new TooManyListenersException();
			}

			lock ( _listeners ) {
				var listener = new FMODListener( _system.StudioSystem, _listenerCount );
				_listeners[ _listenerCount++ ] = listener;
				_system.StudioSystem.setNumListeners( _listenerCount );

				return listener;
			}
		}

		/*
		===============
		ClearListeners
		===============
		*/
		public void ClearListeners() {
			for ( int i = 0; i < _listenerCount; i++ ) {
				_listeners[ i ] = null;
			}
			_system.StudioSystem.setNumListeners( 0 );
		}

		/*
		===============
		SetActiveListener
		===============
		*/
		public IListener SetActiveListener( IListener listener ) {
			throw new System.NotImplementedException();
		}
	};
};