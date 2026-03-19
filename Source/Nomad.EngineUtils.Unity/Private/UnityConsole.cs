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
using Nomad.Core.Console;
using Nomad.Core.Engine.Services;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Nomad.EngineUtils.Private {
    /*
    ===================================================================================

    UnityConsole

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>

    internal sealed class UnityConsole : IConsoleObject {
        /// <summary>
        ///
        /// </summary>
        public ICommandBuilder CommandBuilder => _commandBuilder;
        private readonly UnityCommandBuilder _commandBuilder;

        private readonly ILoggerService _logger;

        /// <summary>
        ///
        /// </summary>
        public bool IsVisible => _visible;
        private bool _visible;
        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventFactory"></param>
        public UnityConsole( ILoggerService logger, IGameEventRegistryService eventFactory ) {
            _logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
            _commandBuilder = new UnityCommandBuilder( eventFactory );
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose() {
            if ( _isDisposed ) {
                return;
            }

            _commandBuilder.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize( this );
        }

        /// <summary>
        ///
        /// </summary>
        public void Clear() {
            _logger.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        public void Show() {
            _visible = true;
        }

        /// <summary>
        ///
        /// </summary>
        public void Hide() {
            _visible = false;
        }
    };
};
