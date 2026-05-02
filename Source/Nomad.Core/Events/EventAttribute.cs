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

namespace Nomad.Core.Events
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class EventAttribute : Attribute
    {
        public EventAttribute(string nameSpace)
        {
            NameSpace = nameSpace;
        }

        public string NameSpace { get; }

        /// <summary>
        /// Optional override for the generated event args/payload struct name.
        /// Use the base type name only, for example "CVarValueChangedEventArgs", not "CVarValueChangedEventArgs&lt;T&gt;".
        /// </summary>
        public string? PayloadName { get; set; }

        /// <summary>
        /// Alias for PayloadName when the call site wants to name the generated event payload/args type explicitly.
        /// </summary>
        public string? EventPayloadName { get; set; }

        /// <summary>
        /// Alias for PayloadName when the call site wants to name the generated args type explicitly.
        /// </summary>
        public string? EventArgsName { get; set; }
    }
}
