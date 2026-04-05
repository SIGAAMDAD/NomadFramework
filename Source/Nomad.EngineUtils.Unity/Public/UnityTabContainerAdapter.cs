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
using UnityEngine;

namespace Nomad.EngineUtils
{
    /// <summary>
    /// Provides basic tab-container behavior for Unity-backed engine templates.
    /// Child transforms are treated as tab pages and only the selected page remains active.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnityTabContainerAdapter : MonoBehaviour
    {
        [SerializeField]
        private int _selectedTabIndex;

        [SerializeField]
        private bool _hideInactiveTabs = true;

        public event Action<int>? TabChanged;
        public event Action<int>? TabFocused;

        /// <summary>
        /// Gets or sets the currently selected tab index.
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetSelectedTab(value);
        }

        /// <summary>
        /// Gets the number of child pages currently available as tabs.
        /// </summary>
        public int TabCount => transform.childCount;

        private void Awake()
        {
            SynchronizeChildVisibility();
        }

        private void OnValidate()
        {
            SynchronizeChildVisibility();
        }

        private void OnTransformChildrenChanged()
        {
            SynchronizeChildVisibility();
        }

        /// <summary>
        /// Selects the requested tab and updates page visibility.
        /// </summary>
        public void SetSelectedTab(int index)
        {
            if (TabCount == 0)
            {
                _selectedTabIndex = 0;
                return;
            }

            int clampedIndex = Mathf.Clamp(index, 0, TabCount - 1);
            bool changed = _selectedTabIndex != clampedIndex;
            _selectedTabIndex = clampedIndex;

            SynchronizeChildVisibility();

            if (changed)
            {
                TabChanged?.Invoke(_selectedTabIndex);
            }
        }

        /// <summary>
        /// Raises a focus notification for an individual tab.
        /// External Unity UI controls can call this when a tab receives hover/focus.
        /// </summary>
        public void FocusTab(int index)
        {
            if (TabCount == 0)
            {
                return;
            }

            int clampedIndex = Mathf.Clamp(index, 0, TabCount - 1);
            TabFocused?.Invoke(clampedIndex);
        }

        private void SynchronizeChildVisibility()
        {
            if (TabCount == 0)
            {
                _selectedTabIndex = 0;
                return;
            }

            _selectedTabIndex = Mathf.Clamp(_selectedTabIndex, 0, TabCount - 1);

            if (!_hideInactiveTabs)
            {
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                bool shouldBeActive = i == _selectedTabIndex;

                if (child.gameObject.activeSelf != shouldBeActive)
                {
                    child.gameObject.SetActive(shouldBeActive);
                }
            }
        }
    }
}
