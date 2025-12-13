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

using NomadCore.GameServices;
using NomadCore.Interfaces.Common;
using NomadCore.Infrastructure.ServiceRegistry.Interfaces;
using NomadCore.Infrastructure.ServiceRegistry.Services;
using NomadCore.Systems.EventSystem.Services;
using NomadCore.Systems.EventSystem.Domain;
using NomadCore.Interfaces;

namespace NomadCore.Systems.EventSystem.Infrastructure.Startup {
	public sealed class EventSystemBootstrapper : IBootstrapper {
		private readonly IServiceRegistry _services;
		private readonly IServiceLocator _locator;

		public EventSystemBootstrapper( ILoggerService logger ) {
			var services = new ServiceCollection();
			_locator = new ServiceLocator( services );

			var eventBus = new GameEventBus();

			services.RegisterSingleton<IGameEventBusService>( eventBus );
			services.RegisterSingleton<IGameService>( new GameEventFactory( eventBus, logger ) );
		}

		public void Initialize() {
		}
		
		public void Start() {
		}

		public void Shutdown() {
		}
	};
};