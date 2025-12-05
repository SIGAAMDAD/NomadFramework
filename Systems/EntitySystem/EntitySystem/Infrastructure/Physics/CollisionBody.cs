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

namespace NomadCore.Systems.EntitySystem.Infrastructure.Physics {
	/*
	===================================================================================
	
	CollisionBody
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class CollisionBody : IDisposable {
		private readonly Rid _bodyRid;
		
		public CollisionBody( StaticBody2D staticBody ) {
			_bodyRid = PhysicsServer2D.BodyCreate();
			PhysicsServer2D.BodySetSpace( _bodyRid, staticBody.GetWorld2D().Space );
			PhysicsServer2D.BodySetCollisionLayer( _bodyRid, staticBody.CollisionLayer );
			PhysicsServer2D.BodySetCollisionMask( _bodyRid, staticBody.CollisionMask );
			PhysicsServer2D.BodySetCollisionPriority( _bodyRid, staticBody.CollisionPriority );
			PhysicsServer2D.BodySetMode( _bodyRid, PhysicsServer2D.BodyMode.Static );

			staticBody.CallDeferred( StaticBody2D.MethodName.QueueFree );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			if ( _bodyRid.IsValid ) {
				PhysicsServer2D.FreeRid( _bodyRid );
			}
		}

		/*
		===============
		CreateShape
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="shape"></param>
		/// <returns></returns>
		private static Rid CreateShape( Shape2D shape ) {
			Rid shapeRid = shape.GetRid();
			if ( !shapeRid.IsValid ) {
				switch ( shape ) {
					case CircleShape2D circleShape:
						shapeRid = PhysicsServer2D.CircleShapeCreate();
						PhysicsServer2D.ShapeSetData( shapeRid, circleShape.Radius );
						break;
					case SegmentShape2D segmentShape:
						shapeRid = PhysicsServer2D.SegmentShapeCreate();
						PhysicsServer2D.ShapeSetData( shapeRid, new Rect2( position: segmentShape.A, size: segmentShape.B ) );
						break;
					case RectangleShape2D rectangleShape:
						shapeRid = PhysicsServer2D.RectangleShapeCreate();
						PhysicsServer2D.ShapeSetData( shapeRid, rectangleShape.Size );
						break;
					case CapsuleShape2D capsuleShape:
						shapeRid = PhysicsServer2D.CapsuleShapeCreate();
						PhysicsServer2D.ShapeSetData( shapeRid, new Vector2( capsuleShape.Radius, capsuleShape.Height ) );
						break;
					case ConcavePolygonShape2D concavePolygonShape:
						shapeRid = PhysicsServer2D.ConcavePolygonShapeCreate();
						PhysicsServer2D.ShapeSetData( shapeRid, concavePolygonShape.Segments );
						break;
					default:
						throw new InvalidCastException( nameof( shape ) );
				}
			}
			return shapeRid;
		}

		/*
		===============
		AddShapesToBody
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bodyRid"></param>
		/// <param name="children"></param>
		private static void AddShapesToBody( Rid bodyRid, Godot.Collections.Array<Node> children ) {
			int count = children.Count;
			for ( int i = 0; i < count; i++ ) {
				if ( children[ i ] is CollisionShape2D collisionShape && collisionShape.Shape != null ) {
					PhysicsServer2D.BodyAddShape( bodyRid, CreateShape( collisionShape.Shape ) );
				} else if ( children[ i ] is CollisionPolygon2D polygonShape && polygonShape.Polygon != null ) {
					ConcavePolygonShape2D shape = new ConcavePolygonShape2D();
					shape.Segments = polygonShape.Polygon;
					PhysicsServer2D.BodyAddShape( bodyRid, CreateShape( shape ) );
				}
			}
		}
	};
};