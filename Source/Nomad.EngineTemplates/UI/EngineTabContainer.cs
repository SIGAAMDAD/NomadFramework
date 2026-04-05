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
using Nomad.Core.UI;
using Nomad.EngineTemplates.Attributes;
using Nomad.EngineTemplates.BaseClasses;

namespace Nomad.EngineTemplates.UI
{
    /// <summary>
    /// Declares the engine template for tab container UI elements.
    /// </summary>
    [TemplateClass(Contract = typeof(ITabContainer), GodotBase = "Godot.TabContainer", UnityBase = "UnityEngine.MonoBehaviour")]
    [TemplateNamespace(Name = "UI")]
    [TemplateUIElement]
    [TemplateProperty(
        Name = "SelectedTabIndex",
        Type = typeof(int),
        GodotGetterExpression = "base.CurrentTab",
        GodotSetterExpression = "base.CurrentTab = {{value}}",
        UnityGetterExpression = "(GetUnityComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>() ?? gameObject.AddComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>()).SelectedTabIndex",
        UnitySetterExpression = "(GetUnityComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>() ?? gameObject.AddComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>()).SelectedTabIndex = {{value}}",
        Documentation = "The index of the currently selected tab.")]
    [TemplateProperty(
        Name = "TabCount",
        Type = typeof(int),
        GodotGetterExpression = "base.GetTabCount()",
        UnityGetterExpression = "(GetUnityComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>() ?? gameObject.AddComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>()).TabCount",
        IsReadOnly = true,
        Documentation = "The number of tabs currently contained by this UI element.")]
    [TemplateEvent(
        Name = "TabChanged",
        PayloadType = typeof(int),
        Documentation = "Raised whenever the selected tab changes.",
        GodotHookExpression = "base.TabChanged += (tab) => {{field}}.Publish((int)tab);",
        UnityHookExpression = "(GetUnityComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>() ?? gameObject.AddComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>()).TabChanged += (tab) => {{field}}.Publish(tab);")]
    [TemplateEvent(
        Name = "TabFocused",
        PayloadType = typeof(int),
        Documentation = "Raised whenever a tab is focused/hovered.",
        GodotHookExpression = "base.TabHovered += (tab) => {{field}}.Publish((int)tab);",
        UnityHookExpression = "(GetUnityComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>() ?? gameObject.AddComponent<global::Nomad.EngineUtils.UnityTabContainerAdapter>()).TabFocused += (tab) => {{field}}.Publish(tab);")]
    internal class EngineTabContainer
    {
    }
}
