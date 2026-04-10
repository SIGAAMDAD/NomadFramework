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
using System.Collections.Generic;
using System.Collections.Immutable;
using Nomad.Core.Util;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	BindingCompilerService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class BindingCompilerService {
		private readonly CompiledBindingRepository _compiledBindings;

		/*
		===============
		BindingCompilerService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiledBindings"></param>
		public BindingCompilerService( CompiledBindingRepository compiledBindings ) {
			_compiledBindings = compiledBindings ?? throw new ArgumentNullException( nameof( compiledBindings ) );
		}

		/*
		===============
		CompileIntoRepository
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actions"></param>
		public void CompileIntoRepository( ImmutableArray<InputActionDefinition> actions ) {
			_compiledBindings.Replace( Compile( actions ) );
		}

		/*
		===============
		Compile
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actions"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static CompiledBindingGraph Compile( ImmutableArray<InputActionDefinition> actions ) {
			var buttonIndex = new Dictionary<ButtonLookupKey, List<CompiledBinding>>();
			var axisIndex = new Dictionary<AxisLookupKey, List<CompiledBinding>>();
			var deltaIndex = new Dictionary<AxisLookupKey, List<CompiledBinding>>();
			var composite1D = new List<CompiledBinding>();
			var composite2D = new List<CompiledBinding>();

			foreach ( var action in actions ) {
				foreach ( var bindingDef in action.Bindings ) {
					var binding = new CompiledBinding(
						actionId: new InternString( action.Name ),
						valueType: action.ValueType,
						kind: bindingDef.Kind,
						scheme: bindingDef.Scheme,
						priority: 0,
						consumesInput: false,
						contextMask: 0xFFFFFFFF,
						button: bindingDef.Button,
						axis1D: bindingDef.Axis1D,
						axis1DComposite: bindingDef.Axis1DComposite,
						axis2D: bindingDef.Axis2D,
						axis2DComposite: bindingDef.Axis2DComposite,
						delta2D: bindingDef.Delta2D
					);

					switch ( binding.Kind ) {
						case InputBindingKind.Button:
							Add(
								buttonIndex,
								new ButtonLookupKey( binding.Button.DeviceId, binding.Button.ControlId, true ),
								binding
							);
							Add(
								buttonIndex,
								new ButtonLookupKey( binding.Button.DeviceId, binding.Button.ControlId, false ),
								binding
							);
							break;
						case InputBindingKind.Axis1D:
							Add(
								axisIndex,
								new AxisLookupKey( binding.Axis1D.DeviceId, binding.Axis1D.ControlId ),
								binding
							);
							break;
						case InputBindingKind.Axis2D:
							Add(
								axisIndex,
								new AxisLookupKey( binding.Axis2D.DeviceId, binding.Axis2D.ControlId ),
								binding
							);
							break;
						case InputBindingKind.Delta2D:
							Add(
								deltaIndex,
								new AxisLookupKey( binding.Delta2D.DeviceId, binding.Delta2D.ControlId ),
								binding
							);
							break;
						case InputBindingKind.Axis1DComposite:
							composite1D.Add( binding );
							break;
						case InputBindingKind.Axis2DComposite:
							composite2D.Add( binding );
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			return new CompiledBindingGraph {
				ButtonIndex = ToCompiledBindingArrayIndex( buttonIndex ),
				AxisIndex = ToCompiledBindingArrayIndex( axisIndex ),
				DeltaIndex = ToCompiledBindingArrayIndex( deltaIndex ),
				Composite1D = composite1D.ToArray(),
				Composite2D = composite2D.ToArray()
			};
		}

		/*
		===============
		Add
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="index"></param>
		/// <param name="key"></param>
		/// <param name="binding"></param>
		private static void Add<TKey>( Dictionary<TKey, List<CompiledBinding>> index, TKey key, in CompiledBinding binding )
			where TKey : notnull
		{
			if ( !index.TryGetValue( key, out var list ) ) {
				list = new List<CompiledBinding>();
				index[key] = list;
			}
			list.Add( binding );
		}

		/*
		===============
		ToImmutable
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		private static Dictionary<TKey, CompiledBinding[]> ToCompiledBindingArrayIndex<TKey>( Dictionary<TKey, List<CompiledBinding>> source )
			where TKey : notnull
		{
			var builder = new Dictionary<TKey, CompiledBinding[]>( source.Count );
			foreach ( var pair in source ) {
				builder[pair.Key] = pair.Value.ToArray();
			}
			return builder;
		}
	}
};
