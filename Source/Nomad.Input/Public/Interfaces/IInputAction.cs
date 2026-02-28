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
using Nomad.Core.Events;
using Nomad.Input.Events;

namespace Nomad.Input.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInputAction : IDisposable
    {
        /// <summary>
        /// Unique name of the action (e.g., "Jump").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Current value of the action (0–1 for buttons, –1..1 for axes).
        /// </summary>
        float Value { get; }

        /// <summary>
        /// True while the action is being held (button down, axis non‑zero).
        /// </summary>
        bool IsPressed { get; }

        /// <summary>
        /// Fires once when the action is first pressed (button down, axis crosses threshold).
        /// </summary>
        IGameEvent<BindPressedEventArgs> Pressed { get; }

        /// <summary>
        /// Fires once when the action is released (button up, axis returns to rest).
        /// </summary>
        IGameEvent<BindReleasedEventArgs> Released { get; }

        /// <summary>
        /// Fires every frame while the action is held (button held, axis non‑zero).
        /// </summary>
        IGameEvent<EmptyEventArgs> Held { get; }

        /// <summary>
        /// Enables or disables the action (events stop firing when disabled).
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets all bindings currently associated with this action.
        /// </summary>
        IReadOnlyList<IInputBinding> Bindings { get; }
    }
}
