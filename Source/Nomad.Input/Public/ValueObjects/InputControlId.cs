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

namespace Nomad.Input.ValueObjects
{
    /// <summary>
    /// Defines all available input controls across different device types.
    /// </summary>
    public enum InputControlId : byte
    {
        /// <summary>
        /// No input control.
        /// </summary>
        None = 0,

        #region Keyboard
        /// <summary>
        /// The 'Q' key.
        /// </summary>
        Q,

        /// <summary>
        /// The 'W' key.
        /// </summary>
        W,

        /// <summary>
        /// The 'E' key.
        /// </summary>
        E,

        /// <summary>
        /// The 'R' key.
        /// </summary>
        R,

        /// <summary>
        /// The 'T' key.
        /// </summary>
        T,

        /// <summary>
        /// The 'Y' key.
        /// </summary>
        Y,

        /// <summary>
        /// The 'U' key.
        /// </summary>
        U,

        /// <summary>
        /// The 'I' key.
        /// </summary>
        I,

        /// <summary>
        /// The 'O' key.
        /// </summary>
        O,

        /// <summary>
        /// The 'P' key.
        /// </summary>
        P,

        /// <summary>
        /// The 'A' key.
        /// </summary>
        A,

        /// <summary>
        /// The 'S' key.
        /// </summary>
        S,

        /// <summary>
        /// The 'D' key.
        /// </summary>
        D,

        /// <summary>
        /// The 'F' key.
        /// </summary>
        F,

        /// <summary>
        /// The 'G' key.
        /// </summary>
        G,

        /// <summary>
        /// The 'H' key.
        /// </summary>
        H,

        /// <summary>
        /// The 'J' key.
        /// </summary>
        J,

        /// <summary>
        /// The 'K' key.
        /// </summary>
        K,

        /// <summary>
        /// The 'L' key.
        /// </summary>
        L,

        /// <summary>
        /// The 'Z' key.
        /// </summary>
        Z,

        /// <summary>
        /// The 'X' key.
        /// </summary>
        X,

        /// <summary>
        /// The 'C' key.
        /// </summary>
        C,

        /// <summary>
        /// The 'V' key.
        /// </summary>
        V,

        /// <summary>
        /// The 'B' key.
        /// </summary>
        B,

        /// <summary>
        /// The 'N' key.
        /// </summary>
        N,

        /// <summary>
        /// The 'M' key.
        /// </summary>
        M,

        /// <summary>
        /// The '1' key (above the letters).
        /// </summary>
        Num1,

        /// <summary>
        /// The '2' key.
        /// </summary>
        Num2,

        /// <summary>
        /// The '3' key.
        /// </summary>
        Num3,

        /// <summary>
        /// The '4' key.
        /// </summary>
        Num4,

        /// <summary>
        /// The '5' key.
        /// </summary>
        Num5,

        /// <summary>
        /// The '6' key.
        /// </summary>
        Num6,

        /// <summary>
        /// The '7' key.
        /// </summary>
        Num7,

        /// <summary>
        /// The '8' key.
        /// </summary>
        Num8,

        /// <summary>
        /// The '9' key.
        /// </summary>
        Num9,

        /// <summary>
        /// The '0' key.
        /// </summary>
        Num0,

        /// <summary>
        /// The '.' key (period/full stop).
        /// </summary>
        Period,

        /// <summary>
        /// The ';' key (semicolon).
        /// </summary>
        SemiColon,

        /// <summary>
        /// The ':' key (colon – often Shift+;).
        /// </summary>
        Colon,

        /// <summary>
        /// The Tab key.
        /// </summary>
        Tab,

        /// <summary>
        /// The grave accent / backtick key (`).
        /// </summary>
        Grave,

        /// <summary>
        /// The Shift key (left or right, treated as a single modifier).
        /// </summary>
        Shift,

        /// <summary>
        /// The Backspace key.
        /// </summary>
        BackSpace,

        /// <summary>
        /// The Ctrl key (left or right, treated as a single modifier).
        /// </summary>
        Ctrl,

        /// <summary>
        /// The Spacebar.
        /// </summary>
        Space,

        /// <summary>
        /// The Alt key (left or right, treated as a single modifier).
        /// </summary>
        Alt,

        /// <summary>
        /// The Left Arrow key.
        /// </summary>
        LeftArrow,

        /// <summary>
        /// The Up Arrow key.
        /// </summary>
        UpArrow,

        /// <summary>
        /// The Down Arrow key.
        /// </summary>
        DownArrow,

        /// <summary>
        /// The Right Arrow key.
        /// </summary>
        RightArrow,

        /// <summary>
        /// The Escape key.
        /// </summary>
        Escape,

        /// <summary>
        /// The Enter key.
        /// </summary>
        Enter,
        #endregion

        #region Mouse Buttons
        /// <summary>
        /// The left mouse button.
        /// </summary>
        Left,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        Right,

        /// <summary>
        /// The middle mouse button (wheel click).
        /// </summary>
        Middle,

        /// <summary>
        /// Mouse wheel scroll down.
        /// </summary>
        WheelDown,

        /// <summary>
        /// Mouse wheel scroll up.
        /// </summary>
        WheelUp,

        /// <summary>
        /// The first extra mouse button (X1).
        /// </summary>
        X1,

        /// <summary>
        /// The second extra mouse button (X2).
        /// </summary>
        X2,
        #endregion

        #region Mouse Analogs
        /// <summary>
        /// Mouse movement delta (relative motion).
        /// </summary>
        Delta,

        /// <summary>
        /// Mouse cursor position (absolute coordinates).
        /// </summary>
        Position,

        /// <summary>
        /// Mouse scroll wheel movement.
        /// </summary>
        Scroll,
        #endregion

        #region Gamepad Buttons
        /// <summary>
        /// Gamepad A button (bottom face button).
        /// </summary>
        GamepadA,

        /// <summary>
        /// Gamepad B button (right face button).
        /// </summary>
        GamepadB,

        /// <summary>
        /// Gamepad X button (left face button).
        /// </summary>
        GamepadX,

        /// <summary>
        /// Gamepad Y button (top face button).
        /// </summary>
        GamepadY,

        /// <summary>
        /// D-Pad up button.
        /// </summary>
        DPadUp,

        /// <summary>
        /// D-Pad down button.
        /// </summary>
        DPadDown,

        /// <summary>
        /// D-Pad left button.
        /// </summary>
        DPadLeft,

        /// <summary>
        /// D-Pad right button.
        /// </summary>
        DPadRight,

        /// <summary>
        /// Left shoulder button (L1/LB).
        /// </summary>
        LeftShoulder,

        /// <summary>
        /// Right shoulder button (R1/RB).
        /// </summary>
        RightShoulder,

        /// <summary>
        /// Left stick button (L3/LS).
        /// </summary>
        LeftStickButton,

        /// <summary>
        /// Right stick button (R3/RS).
        /// </summary>
        RightStickButton,

        /// <summary>
        /// Back/select button.
        /// </summary>
        Back,

        /// <summary>
        /// Start/menu button.
        /// </summary>
        Start,

        /// <summary>
        /// Guide/home button.
        /// </summary>
        Guide,
        #endregion

        #region Gamepad Analogs
        /// <summary>
        /// Left trigger analog input.
        /// </summary>
        LeftTrigger,

        /// <summary>
        /// Right trigger analog input.
        /// </summary>
        RightTrigger,

        /// <summary>
        /// Left analog stick (2D vector).
        /// </summary>
        LeftStick,

        /// <summary>
        /// Right analog stick (2D vector).
        /// </summary>
        RightStick,
        #endregion

        /// <summary>
        /// Sentinel value representing the total number of input controls.
        /// </summary>
        Count
    }
}
