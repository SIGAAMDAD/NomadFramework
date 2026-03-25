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
using Nomad.Core.Engine.Windowing;
using Nomad.Core.Util;
using NUnit.Framework;

namespace Nomad.Core.Tests
{
    [TestFixture]
    [Category("UnitTests")]
    public class WindowResolutionTests
    {
        [Test]
        public void WindowResolution_ToDisplayString_ReturnsExpectedLabels()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That((string)WindowResolution.Res_640x480.ToDisplayString(), Is.EqualTo("640x480"));
                Assert.That((string)WindowResolution.Res_1920x1080.ToDisplayString(), Is.EqualTo("1920x1080"));
                Assert.That((string)WindowResolution.Res_Native.ToDisplayString(), Is.EqualTo("Native Resolution"));
            }
        }

        [Test]
        public void WindowResolution_GetSize_ReturnsExpectedSize()
        {
            var size = WindowResolution.Res_2560x1440.GetSize();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(size.Width, Is.EqualTo(2560));
                Assert.That(size.Height, Is.EqualTo(1440));
            }
        }

        [Test]
        public void WindowResolution_TryParse_ReturnsExpectedResults()
        {
            var parsedKnown = WindowResolutionExtensions.TryParse(new InternString("1920x1080"), out var knownResolution);
            var parsedUnknown = WindowResolutionExtensions.TryParse(new InternString("not-a-resolution"), out var unknownResolution);
            var parsedEmpty = WindowResolutionExtensions.TryParse(InternString.Empty, out var emptyResolution);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsedKnown, Is.True);
                Assert.That(knownResolution, Is.EqualTo(WindowResolution.Res_1920x1080));
                Assert.That(parsedUnknown, Is.False);
                Assert.That(unknownResolution, Is.EqualTo(WindowResolution.Default));
                Assert.That(parsedEmpty, Is.False);
                Assert.That(emptyResolution, Is.EqualTo(WindowResolution.Default));
            }
        }

        [Test]
        public void WindowResolution_Methods_ThrowForUnsupportedValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WindowResolution.Count.ToDisplayString());
            Assert.Throws<ArgumentOutOfRangeException>(() => WindowResolution.Res_Native.GetSize());
            Assert.Throws<ArgumentOutOfRangeException>(() => ((WindowResolution)byte.MaxValue).GetSize());
        }
    }
}
