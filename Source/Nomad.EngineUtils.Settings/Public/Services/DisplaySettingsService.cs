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

using Nomad.Core.EngineUtils;

namespace Nomad.EngineUtils.Settings.Private.Services
{
    public sealed class DisplaySettingsService : IDisplaySettingsService
    {
        public int MonitorIndex => _monitorIndex;
        private int _monitorIndex;

        public DisplaySettingsService(IEngineService engineService)
        {
        }
    }
}
