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
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.CVars;
using Nomad.Core;
using Nomad.Console.Interfaces;
using Nomad.Console.Private.Services;
using Nomad.Core.Logger;
using Nomad.Core.Compatibility;
using Nomad.Core.Abstractions;
using Nomad.Core.EngineUtils;

namespace Nomad.Console
{
    /// <summary>
    ///
    /// </summary>
    public class ConsoleBootstrapper : IBootstrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            ExceptionCompat.ThrowIfNull(registry);
            ExceptionCompat.ThrowIfNull(locator);

            var eventFactory = locator.GetService<IGameEventRegistryService>();
            var logger = locator.GetService<ILoggerService>();
            var engineService = locator.GetService<IEngineService>();

            var cvarSystem = locator.GetService<ICVarSystemService>();
            var configFile = cvarSystem.Register(
                new CVarCreateInfo<string>(
                    name: Constants.CVars.Console.DEFAULT_CONFIG_FILE,
                    defaultValue: "res://Assets/Config/default.ini",
                    description: "The default configuration file.",
                    flags: CVarFlags.Init | CVarFlags.ReadOnly
                )
            );
            cvarSystem.Load(configFile.Value);

            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.CONSOLE_OPENED_EVENT);
            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.CONSOLE_CLOSED_EVENT);
            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.HISTORY_PREV_EVENT);
            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.HISTORY_NEXT_EVENT);
            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.AUTOCOMPLETE_EVENT);
            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.PAGE_UP_EVENT);
            eventFactory.GetEvent<EmptyEventArgs>(Constants.Events.Console.NAMESPACE, Constants.Events.Console.PAGE_DOWN_EVENT);

            var consoleObject = engineService.CreateConsoleObject();

            var commandService = registry.RegisterSingleton<ICommandService>(new CommandCacheService(logger, cvarSystem));
            registry.RegisterSingleton<ICommandLineService>(new CommandLine(commandService, logger, eventFactory));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
        }
    }
}
