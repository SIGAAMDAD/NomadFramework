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

namespace Nomad.Core.EngineUtils.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    public interface IButton : IUIElement
    {
        /// <summary>
        ///
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Event that triggers whenever this button is pressed.
        /// </summary>
        IGameEvent<EmptyEventArgs> Pressed { get; }

        /// <summary>
        /// Event that triggers whenever this button is focused.
        /// </summary>
        IGameEvent<EmptyEventArgs> Focused { get; }

        /// <summary>
        /// Event that triggers whenever this button is unfocused.
        /// </summary>
        IGameEvent<EmptyEventArgs> Unfocused { get; }
    }
}
