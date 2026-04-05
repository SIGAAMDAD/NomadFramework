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
    /// Declares an event member that should be generated for an engine template.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class TemplateEvent : Attribute
    {
        /// <summary>
        /// The event's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The data type of what we're sending the event's payload.
        /// </summary>
        public Type PayloadType { get; set; }

        /// <summary>
        /// Optional documentation for the generated event member.
        /// </summary>
        public string Documentation { get; set; } = null;

        /// <summary>
        /// Optional Godot-specific hook expression used to translate engine signals into the generated event.
        /// Use <c>{{field}}</c> as a placeholder for the generated backing event field.
        /// </summary>
        public string GodotHookExpression { get; set; } = null;

        /// <summary>
        /// Optional Unity-specific hook expression used to translate engine callbacks into the generated event.
        /// Use <c>{{field}}</c> as a placeholder for the generated backing event field.
        /// </summary>
        public string UnityHookExpression { get; set; } = null;
    }
}
