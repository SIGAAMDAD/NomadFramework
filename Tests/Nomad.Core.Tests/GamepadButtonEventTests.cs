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

using Nomad.Core.Input;
using NUnit.Framework;

namespace Nomad.Core.Tests
{
    [TestFixture]
    [Category("UnitTests")]
    public class GamepadButtonEventTests
    {
        [Test]
        public void GamepadButtonEvent_Constructor_ExposesAssignedValues()
        {
            var buttonEvent = new GamepadButtonEvent(GamepadButton.A, 4, 321L, true);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(buttonEvent.Button, Is.EqualTo(GamepadButton.A));
                Assert.That(buttonEvent.DeviceId, Is.EqualTo(4));
                Assert.That(buttonEvent.TimeStamp, Is.EqualTo(321L));
                Assert.That(buttonEvent.Pressed, Is.True);
            }
        }

        [Test]
        public void GamepadButtonEvent_Equals_UsesButtonAndPressedOnly()
        {
            var pressed = new GamepadButtonEvent(GamepadButton.B, 1, 100L, true);
            var sameMeaning = new GamepadButtonEvent(GamepadButton.B, 2, 200L, true);
            var differentState = new GamepadButtonEvent(GamepadButton.B, 1, 100L, false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(pressed.Equals(sameMeaning), Is.True);
                Assert.That(pressed.Equals(differentState), Is.False);
            }
        }
    }
}
