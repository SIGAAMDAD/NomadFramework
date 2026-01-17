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
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Save.Private.Services;
using Nomad.Save.Services;

namespace Nomad.Save
{
    /*
    ===================================================================================

    SaveBootstrapper

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>

    public static class SaveBootstrapper
    {
        /*
        ===============
        Initialize
        ===============
        */
        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceFactory"></param>
        /// <param name="locator"></param>
        public static void Initialize(IServiceRegistry serviceFactory, IServiceLocator locator)
        {
            var logger = locator.GetService<ILoggerService>();
            var eventFactory = locator.GetService<IGameEventRegistryService>();

            serviceFactory.RegisterSingleton<ISaveDataProvider>(new SaveDataProvider(eventFactory, logger));
        }
    }
}
