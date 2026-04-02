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

using System.Collections.Generic;
using System.Collections.Immutable;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBindResolver
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mappingName"></param>
		/// <returns></returns>
		IReadOnlyList<InputActionDefinition> GetBindMapping(string mappingName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mappingName"></param>
		/// <param name="actionId"></param>
		/// <param name="bindings"></param>
		void SetActionBindings( string mappingName, string actionId, ImmutableArray<InputBindingDefinition> bindings );
	}
}