/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Nomad.Core.Abstractions;

namespace Nomad.Audio.ValueObjects
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="maxSimultaneous"></param>
    /// <param name="priorityScale"></param>
    /// <param name="stealProtectionTime"></param>
    /// <param name="allowStealingFromSameCategory"></param>
    public record SoundCategoryCreateInfo(
        string Name,
        int MaxSimultaneous = 4,
        float PriorityScale = 1.5f,
        float StealProtectionTime = 0.2f, // in seconds
        bool AllowStealingFromSameCategory = false
    ) : IValueObject<SoundCategoryCreateInfo>;
}
