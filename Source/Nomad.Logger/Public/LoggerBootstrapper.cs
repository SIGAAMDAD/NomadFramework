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

using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.CVars;
using Nomad.Logger.Private.Sinks;

namespace Nomad.Logger
{
    /// <summary>
    ///
    /// </summary>
    public static class LoggerBootstrapper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <param name="locator"></param>
        public static void Initialize(IServiceRegistry serviceRegistry, IServiceLocator locator)
        {
            var logger = serviceRegistry.RegisterSingleton<ILoggerService>(new LoggerService());
            logger.AddSink(new FileSink(locator.GetService<ICVarSystemService>()));
            logger.AddSink(new ConsoleSink());
        }
    }
}
