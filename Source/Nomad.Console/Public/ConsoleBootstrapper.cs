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

using Godot;
using Nomad.Core.Events;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.CVars;
using Nomad.CVars.Private.Services;
using Nomad.Core;
using System;
using Nomad.Console.Private.Godot;
using Nomad.Console.Interfaces;
using Nomad.Console.Private.Services;
using Nomad.Core.Logger;

namespace Nomad.Console
{
    /// <summary>
    ///
    /// </summary>
    public static class ConsoleBootstrapper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <param name="registry"></param>
        /// <param name="rootNode"></param>
        public static void Initialize(IServiceLocator services, IServiceRegistry registry, Node rootNode)
        {
            ArgumentNullException.ThrowIfNull(rootNode);

            var eventFactory = services.GetService<IGameEventRegistryService>();
            var eventBus = services.GetService<IGodotEventBusService>();
            var logger = services.GetService<ILoggerService>();

            var cvarSystem = registry.RegisterSingleton<ICVarSystemService>(new CVarSystem(eventFactory, logger));
            var configFile = cvarSystem.Register(
                new CVarCreateInfo<string>(
                    Name: Constants.CVars.Console.DEFAULT_CONFIG_FILE,
                    DefaultValue: "res://Assets/Config/default.ini",
                    Description: "The default configuration file.",
                    Flags: CVarFlags.Init | CVarFlags.ReadOnly
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

            var commandBuilder = new GodotCommandBuilder(eventBus, eventFactory);
            var commandService = registry.RegisterSingleton<ICommandService>(new CommandCacheService(logger, cvarSystem));

            var console = new GodotConsole(commandBuilder, commandService, logger, eventFactory);
            registry.RegisterSingleton<ICommandLineService>(new CommandLine(commandBuilder, commandService, logger, eventFactory));

            logger.AddSink(new InGameSink(console, eventFactory));

            rootNode.CallDeferred(Node.MethodName.AddChild, console);
        }
    }
}
