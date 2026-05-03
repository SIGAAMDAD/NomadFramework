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

using System.Numerics;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInputSnapshotService
    {
        /// <summary>
        /// 
        /// </summary>
        Vector2 MouseDelta { get; }

        /// <summary>
        /// 
        /// </summary>
        Vector2 MousePosition { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        bool IsPressed(InputDeviceSlot slot, InputControlId control);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        float GetAxis1D(InputDeviceSlot slot, InputControlId control);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        Vector2 GetAxis2D(InputDeviceSlot slot, InputControlId control);
    }
}
