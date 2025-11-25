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
using NomadCore.Systems.EntitySystem.Events;
using NomadCore.Systems.EventSystem.Common;
using System;
using System.Collections.Generic;

namespace NomadCore.Systems.EntitySystem.Infrastructure.Physics {
	/*
	===================================================================================
	
	Area
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class Area {
		private static readonly Rid Space;

		public uint CollisionLayer {
			get => _collisionLayer;
			set {
				if ( _collisionLayer == value ) {
					return;
				}
				_collisionLayer = value;
				PhysicsServer2D.AreaSetCollisionLayer( AreaRID, _collisionLayer );
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
				PhysicsServer2D.AreaSetCollisionMask( AreaRID, _collisionMask );
			}
		}
		private uint _collisionMask;

		public Rid AreaRID { get; private set; }
		public HashSet<Rid> OverlappingBodies { get; private set; }
		public HashSet<Rid> OverlappingAreas { get; private set; }

		public IReadOnlyList<Rid> Shapes => _shapes;
		private readonly List<Rid> _shapes = new List<Rid>();

		public Transform2D Transform => _transform;
		private Transform2D _transform;

		public readonly GameEvent<AreaEnteredEventData> AreaEntered = new GameEvent<AreaEnteredEventData>( nameof( AreaEntered ) );
		public readonly GameEvent<AreaExitedEventData> AreaExited = new GameEvent<AreaExitedEventData>( nameof( AreaExited ) );

		/*
		===============
		Area
		===============
		*/
		private Area( Rid areaRid ) {
			AreaRID = areaRid;
			
			_collisionLayer = PhysicsServer2D.AreaGetCollisionLayer( areaRid );
			_collisionMask = PhysicsServer2D.AreaGetCollisionMask( areaRid );
			_transform = PhysicsServer2D.AreaGetTransform( areaRid );
			_shapes = GetAreaShapes( areaRid );
		}

		/*
		===============
		Convert
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		public static Area Convert( Area2D area ) {
			Rid areaRid = PhysicsServer2D.AreaCreate();

			uint collisionLayer = area.CollisionLayer;
			uint collisionMask = area.CollisionMask;

			PhysicsServer2D.AreaSetSpace( areaRid, area.GetWorld2D().Space );
			PhysicsServer2D.AreaSetCollisionLayer( areaRid, collisionLayer );
			PhysicsServer2D.AreaSetCollisionMask( areaRid, collisionMask );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.AngularDamp, area.AngularDamp );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.AngularDampOverrideMode, (long)area.AngularDampSpaceOverride );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.Gravity, area.Gravity );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.GravityIsPoint, area.GravityPoint );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.GravityOverrideMode, (long)area.GravitySpaceOverride );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.GravityPointUnitDistance, area.GravityPointUnitDistance );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.GravityVector, area.GravityDirection );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.LinearDamp, area.LinearDamp );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.LinearDampOverrideMode, (long)area.LinearDampSpaceOverride );
			PhysicsServer2D.AreaSetParam( areaRid, PhysicsServer2D.AreaParameter.Priority, area.CollisionPriority );
			PhysicsServer2D.AreaSetMonitorable( areaRid, area.Monitorable );
			PhysicsServer2D.AreaSetTransform( areaRid, area.Transform );

			Godot.Collections.Array<Node> children = area.GetChildren();
			AddShapesToArea( areaRid, children );

			area.CallDeferred( Area2D.MethodName.QueueFree );

			return new Area( areaRid );
		}

		/*
		===============
		GetAreaShapes
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="areaRid"></param>
		/// <returns></returns>
		private static List<Rid> GetAreaShapes( Rid areaRid ) {
			int shapeCount = PhysicsServer2D.AreaGetShapeCount( areaRid );
			List<Rid> shapes = new List<Rid>( shapeCount );
			
			for ( int i = 0; i < shapeCount; i++ ) {
				shapes.Add( PhysicsServer2D.AreaGetShape( areaRid, i ) );
			}

			return shapes;
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
		AddShapesToArea
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="children"></param>
		private static void AddShapesToArea( Rid areaRid, Godot.Collections.Array<Node> children ) {
			int count = children.Count;
			for ( int i = 0; i < count; i++ ) {
				if ( children[ i ] is CollisionShape2D collisionShape && collisionShape.Shape != null ) {
					PhysicsServer2D.AreaAddShape( areaRid, CreateShape( collisionShape.Shape ) );
				} else if ( children[ i ] is CollisionPolygon2D polygonShape && polygonShape.Polygon != null ) {
					ConcavePolygonShape2D shape = new ConcavePolygonShape2D();
					shape.Segments = polygonShape.Polygon;
					PhysicsServer2D.AreaAddShape( areaRid, CreateShape( shape ) );
				}
			}
		}
	};
};