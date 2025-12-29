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

using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;

namespace Nomad.Events {
	public static class EventSystemBootstrapper {
		public static void Initialize( IServiceLocator locator, IServiceRegistry registry ) {
			var logger = locator.GetService<ILoggerService>();

			var eventBus = registry.RegisterSingleton<IGodotEventBusService>( new GameEventBus() );
			registry.RegisterSingleton<IGameEventRegistryService>( new GameEventRegistry( logger ) );
		}
	};
};