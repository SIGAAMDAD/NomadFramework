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
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public sealed class EventPayloadAttribute : Attribute
	{
		public string Name { get; }
		public Type? Type { get; }
		public string? TypeName { get; init; }
		public int Order { get; init; }

		public EventPayloadAttribute(string name, Type type)
		{
			Name = name;
			Type = type;
		}
		
		public EventPayloadAttribute(string name, string typeName)
		{
			Name = name;
			TypeName = typeName;
		}
	}
}