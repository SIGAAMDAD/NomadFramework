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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nomad.Core.ServiceRegistry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceLocator : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        IServiceRegistry Collection { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService GetService<TService>()
            where TService : class;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        bool TryGetService<TService>([NotNullWhen(true)] out TService? service)
            where TService : class;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        IEnumerable<TService> GetServices<TService>()
            where TService : class;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService CreateInstance<TService>()
            where TService : class;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IServiceScope CreateScope();
    }
}
