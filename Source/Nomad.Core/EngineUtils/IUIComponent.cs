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
using System.Numerics;

namespace Nomad.Core.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public interface IUIComponent : IDisposable
    {
        /// <summary>
        /// Whether the ui component is visible or not.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// The ui component's position on the screen.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// The ui component's scaling factor.
        /// </summary>
        Vector2 Scale { get; set; }

        /// <summary>
        /// Enables the ui component completely, toggles it visible and allows it to process events.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the ui component completely, toggles it invisible and forces it to no longer receive any events.
        /// </summary>
        void Disable();
    }
}
