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
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Repositories {
	/*
	===================================================================================
	
	CompiledBindingRepository
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CompiledBindingRepository {
		public CompiledBindingGraph Current => Volatile.Read( ref _current );
		private CompiledBindingGraph _current = CompiledBindingGraph.Empty;

		/*
		===============
		Replace
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="graph"></param>
		public void Replace( CompiledBindingGraph graph ) {
			Volatile.Write( ref _current, graph );
		}

		/*
		===============
		GetButtonCandidates
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ReadOnlySpan<CompiledBinding> GetButtonCandidates( in ButtonLookupKey key ) {
			if ( Current.ButtonIndex.TryGetValue( key, out var bindings ) ) {
				return bindings.AsSpan();
			}
			return ReadOnlySpan<CompiledBinding>.Empty;
		}

		/*
		===============
		GetAxisCandidates
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ReadOnlySpan<CompiledBinding> GetAxisCandidates( in AxisLookupKey key ) {
			if ( Current.AxisIndex.TryGetValue( key, out var bindings ) ) {
				return bindings.AsSpan();
			}
			return ReadOnlySpan<CompiledBinding>.Empty;
		}

		/*
		===============
		GetDeltaCandidates
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ReadOnlySpan<CompiledBinding> GetDeltaCandidates( in AxisLookupKey key ) {
			if ( Current.DeltaIndex.TryGetValue( key, out var bindings ) ) {
				return bindings.AsSpan();
			}
			return ReadOnlySpan<CompiledBinding>.Empty;
		}

		/*
		===============
		GetComposite1DBindings
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ImmutableArray<CompiledBinding> GetComposite1DBindings()
			=> Current.Composite1D;

		/*
		===============
		GetComposite2DBindings
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ImmutableArray<CompiledBinding> GetComposite2DBindings()
			=> Current.Composite2D;
	};
};