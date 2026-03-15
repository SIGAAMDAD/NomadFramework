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

using System;

namespace Nomad.EngineTemplates.Attributes
{
    /// <summary>
    /// Describes how to translate a framework type to and from engine-native types.
    /// Expressions should use <c>{{value}}</c> as the placeholder for the source value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TemplateTypeConversion : Attribute
    {
        /// <summary>
        /// The agnostic framework type.
        /// </summary>
        public Type AgnosticType { get; set; }

        /// <summary>
        /// The Godot engine-side type.
        /// </summary>
        public Type GodotType { get; set; }

        /// <summary>
        /// Converts a Godot value to the agnostic type.
        /// </summary>
        public string GodotToAgnosticExpression { get; set; } = null;

        /// <summary>
        /// Converts an agnostic value to the Godot type.
        /// </summary>
        public string AgnosticToGodotExpression { get; set; } = null;

        /// <summary>
        /// The Unity engine-side type.
        /// </summary>
        public Type UnityType { get; set; }

        /// <summary>
        /// Converts a Unity value to the agnostic type.
        /// </summary>
        public string UnityToAgnosticExpression { get; set; } = null;

        /// <summary>
        /// Converts an agnostic value to the Unity type.
        /// </summary>
        public string AgnosticToUnityExpression { get; set; } = null;
    }
}
