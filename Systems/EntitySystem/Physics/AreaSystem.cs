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

using EntitySystem.Events;
using Godot;
using System.Collections.Generic;

namespace EntitySystem.Physics {
	/*
	===================================================================================
	
	AreaSystem
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class AreaSystem : PhysicsSubSystem {
		private sealed class OverlapSet : PoolableObject {
			public readonly HashSet<Entity> Data = new HashSet<Entity>();

			public bool Add( Entity item ) => Data.Add( item );
			public bool Remove( Entity item ) => Data.Remove( item );
			public bool Contains( Entity item ) => Data.Contains( item );
		};
		private readonly Dictionary<Entity, Area> Areas = new Dictionary<Entity, Area>();

		private readonly Dictionary<Entity, OverlapSet> CurrentOverlaps = new Dictionary<Entity, OverlapSet>();
		private readonly ObjectPool<OverlapSet> OverlapPool = new ObjectPool<OverlapSet>( () => new OverlapSet() );

		public AreaSystem( Rid space, EntityComponentSystem system )
			: base( space, system )
		{ }

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public override void Update( float delta ) {
			CheckAreaOverlaps();
		}

		/*
		===============
		CheckAreaOverlaps
		===============
		*/
		private void CheckAreaOverlaps() {
			PhysicsDirectSpaceState2D spaceState = PhysicsServer2D.SpaceGetDirectState( PhysicsSpace );

			foreach ( var ( entity, area ) in Areas ) {
				OverlapSet newOverlaps = QueryAreaOverlaps( spaceState, entity );
				OverlapSet previousOverlaps = CurrentOverlaps[ entity ];

				foreach ( var bodyEntity in newOverlaps.Data ) {
					if ( !previousOverlaps.Contains( bodyEntity ) ) {
						area.AreaEntered.Publish( new AreaEnteredEventData( entity, bodyEntity ) );
					}
				}
				foreach ( var bodyEntity in previousOverlaps.Data ) {
					if ( !newOverlaps.Contains( bodyEntity ) ) {
						area.AreaExited.Publish( new AreaExitedEventData( entity, bodyEntity ) );
					}
				}

				OverlapPool.Return( previousOverlaps );
				CurrentOverlaps[ entity ] = newOverlaps;
			}
		}

		/*
		===============
		QueryAreaOverlaps
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="spaceState"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		private OverlapSet QueryAreaOverlaps( PhysicsDirectSpaceState2D spaceState, Entity entity ) {
			var overlaps = OverlapPool.Rent();
			var areaData = Areas[ entity ];

			int count = areaData.Shapes.Count;
			for ( int i = 0; i < count; i++ ) {
				PhysicsShapeQueryParameters2D queryParams = new PhysicsShapeQueryParameters2D {
					ShapeRid = areaData.Shapes[ i ],
					Transform = areaData.Transform,
					CollisionMask = areaData.CollisionMask,
					CollideWithAreas = true,
					CollideWithBodies = true
				};

				var results = spaceState.IntersectShape( queryParams );
				Variant rid;

				foreach ( var result in results ) {
					if ( result.TryGetValue( nameof( rid ), out rid ) && RidToEntity.TryGetValue( rid.AsRid(), out var bodyEntity ) ) {
						overlaps.Add( bodyEntity );
					}
				}
			}

			return overlaps;
		}
	};
};