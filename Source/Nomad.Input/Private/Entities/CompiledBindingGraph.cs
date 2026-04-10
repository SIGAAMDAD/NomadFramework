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

namespace Nomad.Input.Private.ValueObjects {
	/*
	===================================================================================
	
	CompiledBindingGraph
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CompiledBindingGraph {
		public Dictionary<ButtonLookupKey, CompiledBinding[]> ButtonIndex { get; init; }
		public Dictionary<AxisLookupKey, CompiledBinding[]> AxisIndex { get; init; }
		public Dictionary<AxisLookupKey, CompiledBinding[]> DeltaIndex { get; init; }

		public CompiledBinding[] Composite1D { get; init; }
		public CompiledBinding[] Composite2D { get; init; }

		public static CompiledBindingGraph Empty { get; } = new CompiledBindingGraph {
			ButtonIndex = new Dictionary<ButtonLookupKey, CompiledBinding[]>(),
			AxisIndex = new Dictionary<AxisLookupKey, CompiledBinding[]>(),
			DeltaIndex = new Dictionary<AxisLookupKey, CompiledBinding[]>(),
			Composite1D = System.Array.Empty<CompiledBinding>(),
			Composite2D = System.Array.Empty<CompiledBinding>()
		};
	};
};
