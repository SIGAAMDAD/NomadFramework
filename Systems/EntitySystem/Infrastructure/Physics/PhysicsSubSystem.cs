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
using NomadCore.Systems.EntitySystem.Interfaces;
using NomadCore.Systems.EntitySystem.Services;
using System.Collections.Generic;

namespace NomadCore.Systems.EntitySystem.Infrastructure.Physics {
	/*
	===================================================================================
	
	PhysicsSubSystem
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal abstract class PhysicsSubSystem : IPhysicsSubSystem {
		protected readonly Rid PhysicsSpace;
		protected readonly EntityComponentSystem EntitySystem;
		protected readonly Dictionary<Entity, Rid> EntityToRid = new Dictionary<Entity, Rid>();
		protected readonly Dictionary<Rid, Entity> RidToEntity = new Dictionary<Rid, Entity>();

		/*
		===============
		PhysicsSubSystem
		===============
		*/
		public PhysicsSubSystem( Rid space, EntityComponentSystem system ) {
			PhysicsSpace = space;
			EntitySystem = system;
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
		public abstract void Update( float delta );
		
		/*
		===============
		Shutdown
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public virtual void Shutdown() {
			foreach ( var rid in EntityToRid ) {
				PhysicsServer2D.FreeRid( rid.Value );
			}
			EntityToRid.Clear();
			RidToEntity.Clear();
		}

		/*
		===============
		RegisterEntity
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="rid"></param>
		protected void RegisterEntity( Entity entity, Rid rid ) {
			EntityToRid[ entity ] = rid;
			RidToEntity[ rid ] = entity;
		}

		/*
		===============
		UnregisterEntity
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		protected void UnregisterEntity( Entity entity ) {
			if ( EntityToRid.TryGetValue( entity, out Rid rid ) ) {
				EntityToRid.Remove( entity );
				RidToEntity.Remove( rid );
				PhysicsServer2D.FreeRid( rid );
			}
		}
	};
};