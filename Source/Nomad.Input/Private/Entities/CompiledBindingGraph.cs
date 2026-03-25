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

using System.Collections.Immutable;

namespace Nomad.Input.Private.ValueObjects {
	/*
	===================================================================================
	
	CompiledBindingGraph
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal record CompiledBindingGraph {
		public ImmutableDictionary<ButtonLookupKey, ImmutableArray<CompiledBinding>> ButtonIndex { get; init; }
		public ImmutableDictionary<AxisLookupKey, ImmutableArray<CompiledBinding>> AxisIndex { get; init; }
		public ImmutableDictionary<AxisLookupKey, ImmutableArray<CompiledBinding>> DeltaIndex { get; init; }

		public ImmutableArray<CompiledBinding> Composite1D { get; init; }
		public ImmutableArray<CompiledBinding> Composite2D { get; init; }

		public static CompiledBindingGraph Empty => new CompiledBindingGraph {
			ButtonIndex = ImmutableDictionary<ButtonLookupKey, ImmutableArray<CompiledBinding>>.Empty,
			AxisIndex = ImmutableDictionary<AxisLookupKey, ImmutableArray<CompiledBinding>>.Empty,
			DeltaIndex = ImmutableDictionary<AxisLookupKey, ImmutableArray<CompiledBinding>>.Empty,
			Composite1D = ImmutableArray<CompiledBinding>.Empty,
			Composite2D = ImmutableArray<CompiledBinding>.Empty
		};
	};
};