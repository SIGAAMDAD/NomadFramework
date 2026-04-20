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

namespace Nomad.Core.Input
{
    /// <summary>
    /// Interface for the input system that handles various input events from different devices.
    /// </summary>
    public interface IInputSystem : IDisposable
    {
        /// <summary>
        /// Gets the current input scheme being used.
        /// </summary>
        InputScheme Mode { get; }

        /// <summary>
        /// Gets the context mask for filtering input events.
        /// </summary>
        uint ContextMask { get; }

        /// <summary>
        /// Pushes a keyboard event into the input system.
        /// </summary>
        /// <param name="keyEvent">The keyboard event arguments.</param>
        void PushKeyboardEvent(in KeyboardEventArgs keyEvent);

        /// <summary>
        /// Pushes a mouse motion event into the input system.
        /// </summary>
        /// <param name="mouseMotionEvent">The mouse motion event arguments.</param>
        void PushMouseMotionEvent(in MouseMotionEventArgs mouseMotionEvent);

        /// <summary>
        /// Pushes a mouse button event into the input system.
        /// </summary>
        /// <param name="mouseButtonEvent">The mouse button event arguments.</param>
        void PushMouseButtonEvent(in MouseButtonEventArgs mouseButtonEvent);

        /// <summary>
        /// Pushes a gamepad axis event into the input system.
        /// </summary>
        /// <param name="gamepadAxisEvent">The gamepad axis event arguments.</param>
        void PushGamepadAxisEvent(in GamepadAxisEventArgs gamepadAxisEvent);

        /// <summary>
        /// Pushes a gamepad button event into the input system.
        /// </summary>
        /// <param name="gamepadButtonEvent">The gamepad button event arguments.</param>
        void PushGamepadButtonEvent(in GamepadButtonEventArgs gamepadButtonEvent);

        /// <summary>
        /// Updates the input system with the given time delta.
        /// </summary>
        /// <param name="delta">The time delta since the last update.</param>
        void Update(float delta);
    }
}
