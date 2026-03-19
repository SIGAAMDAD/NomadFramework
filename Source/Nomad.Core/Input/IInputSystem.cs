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

namespace Nomad.Core.Input
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInputSystem : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        InputMode Mode { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyEvent"></param>
        void PushKeyboardEvent(in KeyboardEvent keyEvent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseMotionEvent"></param>
        void PushMouseMotionEvent(in MouseMotionEvent mouseMotionEvent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseButtonEvent"></param>
        void PushMouseButtonEvent(in MouseButtonEvent mouseButtonEvent);

        /// <summary>
        /// 
        /// </summary>
        void PushGamepadAxisEvent(in GamepadAxisEvent gamepadAxisEvent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gamepadButtonEvent"></param>
        void PushGamepadButtonEvent(in GamepadButtonEvent gamepadButtonEvent);
    }
}
