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
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Nomad.Input.ValueObjects
{
    public readonly struct ButtonBinding
    {
        public ImmutableArray<InputControlId> Modifiers { get; }
        public InputDeviceSlot DeviceId { get; }
        public InputControlId ControlId { get; }

        public ulong ModifierMask0 { get; }
        public ulong ModifierMask1 { get; }
        public ulong ModifierMask2 { get; }
        public ulong ModifierMask3 { get; }

        public int ModifierCount { get; }

        public ButtonBinding(
            InputDeviceSlot deviceId,
            InputControlId controlId,
            ImmutableArray<InputControlId> modifiers
        )
        {
            DeviceId = deviceId;
            ControlId = controlId;
            Modifiers = modifiers.IsDefault ? ImmutableArray<InputControlId>.Empty : modifiers;

            BuildModifierMasks(
                Modifiers,
                out ulong mask0,
                out ulong mask1,
                out ulong mask2,
                out ulong mask3,
                out int modifierCount
            );

            ModifierMask0 = mask0;
            ModifierMask1 = mask1;
            ModifierMask2 = mask2;
            ModifierMask3 = mask3;
            ModifierCount = modifierCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ButtonBinding(
            InputDeviceSlot deviceId,
            InputControlId controlId
        )
            : this(deviceId, controlId, ImmutableArray<InputControlId>.Empty)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildModifierMasks(
            ImmutableArray<InputControlId> modifiers,
            out ulong mask0,
            out ulong mask1,
            out ulong mask2,
            out ulong mask3,
            out int modifierCount
        )
        {
            mask0 = 0UL;
            mask1 = 0UL;
            mask2 = 0UL;
            mask3 = 0UL;
            modifierCount = 0;

            for (int i = 0; i < modifiers.Length; i++)
            {
                int control = (int)modifiers[i];
                int word = control >> 6;
                ulong bit = 1UL << (control & 63);

                switch (word)
                {
                    case 0:
                        mask0 |= bit;
                        break;
                    case 1:
                        mask1 |= bit;
                        break;
                    case 2:
                        mask2 |= bit;
                        break;
                    case 3:
                        mask3 |= bit;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(modifiers),
                            $"Modifier control id {control} exceeds 4x64-bit keyboard mask capacity."
                        );
                }

                modifierCount++;
            }
        }
    }
}
