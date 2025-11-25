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
using System;

namespace EntitySystem.Physics {
	/*
	===================================================================================
	
	Body
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class Body {
		public uint CollisionLayer {
			get => _collisionLayer;
			set {
				if ( _collisionLayer == value ) {
					return;
				}
				_collisionLayer = value;
				PhysicsServer2D.BodySetCollisionLayer( BodyRID, _collisionLayer );
			}
		}
		private uint _collisionLayer;

		public uint CollisionMask {
			get => _collisionMask;
			set {
				if ( _collisionMask == value ) {
					return;
				}
				_collisionMask = value;
				PhysicsServer2D.BodySetCollisionMask( BodyRID, _collisionMask );
			}
		}
		private uint _collisionMask;

		private readonly Entity Owner;
		private readonly Rid BodyRID;

		/*
		===============
		Body
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="bodyRid"></param>
		public Body( Entity? owner, Rid bodyRid, uint collisionLayer, uint collisionMask ) {
			ArgumentNullException.ThrowIfNull( owner );
			
			Owner = owner;
			BodyRID = bodyRid;

			_collisionLayer = collisionLayer;
			_collisionMask = collisionMask;

			PhysicsServer2D.BodySetStateSyncCallback( BodyRID, Callable.From<float>( Update ) );
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public void Update( float delta ) {
		}

		/*
		===============
		Convert
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="characterBody2D"></param>
		/// <returns></returns>
		public static Body Convert( Entity owner, CharacterBody2D characterBody2D ) {
			Rid bodyRid = PhysicsServer2D.BodyCreate();

			uint collisionLayer = characterBody2D.CollisionLayer;
			uint collisionMask = characterBody2D.CollisionMask;

			PhysicsServer2D.BodySetCollisionLayer( bodyRid, collisionLayer );
			PhysicsServer2D.BodySetCollisionMask( bodyRid, collisionMask );

			return new Body( owner, bodyRid, collisionLayer, collisionMask );
		}
	};
};