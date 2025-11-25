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

using Godot;
using NomadCore.Systems.EntitySystem.Common;
using System;
using System.Collections.Concurrent;

namespace NomadCore.Systems.EntitySystem.Infrastructure {
	/*
	===================================================================================

	EntityPool

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class EntityPool {
		public int AvailableCount => AvailableObjects.Count;

		public int TotalCount => _currentSize;
		private int _currentSize = 0;

		public int ActiveObjectCount => _currentSize - AvailableObjects.Count;

		private readonly ConcurrentBag<Entity> AvailableObjects = new ConcurrentBag<Entity>();

		private readonly int MaxSize;
		private bool IsDisposed;

		/*
		===============
		EntityPool
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSize"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public EntityPool( int maxSize = int.MaxValue ) {
			AvailableObjects = new ConcurrentBag<Entity>();
			MaxSize = maxSize;
		}

		/*
		===============
		Rent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityNode"></param>
		/// <param name="area"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Entity Rent( Node2D entityNode, Area2D area ) {
			ObjectDisposedException.ThrowIf( IsDisposed, this );

			if ( AvailableObjects.TryTake( out Entity? obj ) ) {
				ArgumentNullException.ThrowIfNull( obj );
				return obj;
			}

			if ( _currentSize < MaxSize ) {
				_currentSize++;
				return new Entity( entityNode, area );
			}
			throw new InvalidOperationException( "Object pool exhausted and maximum size reached" );
		}

		/*
		===============
		Rent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityNode"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public Entity Rent( Node2D entityNode, CharacterBody2D body ) {
			ObjectDisposedException.ThrowIf( IsDisposed, this );

			if ( AvailableObjects.TryTake( out Entity? obj ) ) {
				ArgumentNullException.ThrowIfNull( obj );
				return obj;
			}

			if ( _currentSize < MaxSize ) {
				_currentSize++;
				return new Entity( entityNode, body );
			}
			throw new InvalidOperationException( "Object pool exhausted and maximum size reached" );
		}

		/*
		===============
		Return
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void Return( in Entity obj ) {
			if ( IsDisposed ) {
				obj.Dispose();
				return;
			}

			ArgumentNullException.ThrowIfNull( obj );

			if ( AvailableObjects.Count < MaxSize ) {
				AvailableObjects.Add( obj );
			} else {
				obj.Dispose();
				_currentSize--;
			}
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			if ( IsDisposed ) {
				return;
			}

			IsDisposed = true;
			while ( AvailableObjects.TryTake( out Entity? obj ) ) {
				ArgumentNullException.ThrowIfNull( obj );
				obj.Dispose();
			}
			_currentSize = 0;
		}
	};
};