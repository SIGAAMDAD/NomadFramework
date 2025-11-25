/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EntitySystem {
	/*
	===================================================================================
	
	Entity
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	/// <param name="id"></param>

	public abstract class Entity( int id ) : PoolableObject {
		public readonly int Id = id;

		protected readonly Dictionary<Type, IComponent> Components = new Dictionary<Type, IComponent>();

		/*
		===============
		AddComponent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public virtual T AddComponent<T>() where T : struct, IComponent {
			Type type = typeof( T );
			if ( Components.TryGetValue( type, out var component ) ) {
				return (T)component;
			}
			component = new T();
			Components[ type ] = component;
			return (T)component;
		}

		/*
		===============
		RemoveComponent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="KeyNotFoundException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public virtual void RemoveComponent<T>() where T : struct, IComponent {
			Type type = typeof( T );
			if ( !Components.ContainsKey( type ) ) {
				throw new KeyNotFoundException( $"Component {type} doesn't exist" );
			}
			Components.Remove( type );
		}

		/*
		===============
		GetComponent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public virtual T GetComponent<T>() where T : struct, IComponent {
			return Components.TryGetValue( typeof( T ), out var component ) ?
					(T)component
				:
					throw new KeyNotFoundException( $"Component {typeof( T )} doesn't exist" );
		}
		
		/*
		===============
		TryGetComponent
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryGetComponent<T>( out T? component ) where T : struct, IComponent {
			if ( Components.TryGetValue( typeof( T ), out var value ) ) {
				component = (T)value;
				return true;
			}
			component = null;
			return false;
		}

		/*
		===============
		GetComponentCount
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public virtual int GetComponentCount() {
			return Components.Count;
		}
		
		/*
		===============
		GetComponentAtIndex
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="index"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public virtual T GetComponentAtIndex<T>( int index ) where T : struct, IComponent {
			return (T)Components.ElementAt( index ).Value;
		}

		/*
		===============
		GetComponents
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual T[] GetComponents<T>() where T : struct, IComponent {
			List<T> components = new List<T>();
			Type type = typeof( T );
			foreach ( var component in Components ) {
				if ( component.Key == type ) {
					components.Add( (T)component.Value );
				}
			}
			return [ .. components ];
		}
	};
};