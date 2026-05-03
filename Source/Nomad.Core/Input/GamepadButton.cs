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

namespace Nomad.Core.Input
{
    /// <summary>
    /// Represents a virtual gamepad button id.
    /// </summary>
    public enum GamepadButton : int
    {
        /// <summary>
        /// Invalid game controller button id.
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// Game controller SDL button A. Corresponds to the bottom action button: Sony Cross,
        /// Xbox A, Nintendo B.
        /// </summary>
        A,

        /// <summary>
        /// Game controller SDL button B. Corresponds to the right action button: Sony Circle,
        /// Xbox B, Nintendo A.
        /// </summary>
        B,

        /// <summary>
        /// Game controller SDL button X. Corresponds to the left action button: Sony Square,
        /// Xbox X, Nintendo Y.
        /// </summary>
        X,

        /// <summary>
        /// Game controller SDL button Y. Corresponds to the top action button: Sony Triangle,
        /// Xbox Y, Nintendo X.
        /// </summary>
        Y,

        /// <summary>
        /// Game controller SDL back button. Corresponds to the Sony Select, Xbox Back, Nintendo
        /// - button.
        /// </summary>
        Back,

        /// <summary>
        /// Game controller SDL guide button. Corresponds to the Sony PS, Xbox Home button.
        /// </summary>
        Guide,

        /// <summary>
        /// Game controller SDL start button. Corresponds to the Sony Options, Xbox Menu,
        /// Nintendo + button.
        /// </summary>
        Start,

        /// <summary>
        /// Game controller SDL left stick button. Corresponds to the Sony L3, Xbox L/LS
        /// button.
        /// </summary>
        LeftStick,

        /// <summary>
        /// Game controller SDL right stick button. Corresponds to the Sony R3, Xbox R/RS
        /// button.
        /// </summary>
        RightStick,

        /// <summary>
        /// Game controller SDL left shoulder button. Corresponds to the Sony L1, Xbox LB
        /// button.
        /// </summary>
        LeftShoulder,

        /// <summary>
        /// Game controller SDL right shoulder button. Corresponds to the Sony R1, Xbox RB
        /// button.
        /// </summary>
        RightShoulder,

        /// <summary>
        /// Game controller D-Pad up button.
        /// </summary>
        DPadUp,

        /// <summary>
        /// Game controller D-Pad down button.
        /// </summary>
        DPadDown,

        /// <summary>
        /// Game controller D-Pad left button.
        /// </summary>
        DPadLeft,

        /// <summary>
        /// Game controller D-Pad right button.
        /// </summary>
        DPadRight,

        /// <summary>
        /// Sentinel value representing the total number of gamepad buttons.
        /// </summary>
        Count
    }
}
