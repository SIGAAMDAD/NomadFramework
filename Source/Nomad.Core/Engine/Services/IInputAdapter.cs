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
using Nomad.Core.Input;

namespace Nomad.Core.Engine.Services
{
    /// <summary>
    ///
    /// </summary>
    public interface IInputAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        IGameEvent<KeyboardEventArgs> KeyboardEvent { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameEvent<MouseButtonEventArgs> MouseButtonEvent { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameEvent<MouseMotionEventArgs> MouseMotionEvent { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameEvent<GamepadAxisEventArgs> GamepadAxisEvent { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameEvent<GamepadButtonEventArgs> GamepadButtonEvent { get; }
    }
}
