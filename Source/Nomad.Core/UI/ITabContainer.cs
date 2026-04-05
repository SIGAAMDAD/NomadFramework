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

namespace Nomad.Core.UI
{
    /// <summary>
    /// Represents a UI container that organizes child content into tabs.
    /// </summary>
    public interface ITabContainer : IUIElement
    {
        /// <summary>
        /// The index of the currently selected tab.
        /// </summary>
        int SelectedTabIndex { get; set; }

        /// <summary>
        /// The number of tabs currently contained by this UI element.
        /// </summary>
        int TabCount { get; }

        /// <summary>
        /// Raised whenever the selected tab changes.
        /// </summary>
        IGameEvent<int> TabChanged { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameEvent<int> TabFocused { get; }
    }
}
