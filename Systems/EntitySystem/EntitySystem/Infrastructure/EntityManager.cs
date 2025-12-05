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
using NomadCore.Abstractions.Services;
using NomadCore.Interfaces.EntitySystem;
using NomadCore.Systems.EntitySystem.Common;
using System;

namespace NomadCore.Systems.EntitySystem.Infrastructure {
	/*
	===================================================================================
	
	EntityManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class EntityManager : ComponentSystem {
		private readonly EntityPool EntityPool = new EntityPool();
		private readonly Node2D EntityNode;

		/*
		===============
		EntityManager
		===============
		*/
		public EntityManager( IEntityComponentSystemService ecs, Node? rootNode )
			: base( ecs )
		{
			ArgumentNullException.ThrowIfNull( rootNode );

			EntityNode = new Node2D() {
				Name = nameof( EntityNode )
			};
			rootNode.AddChild( EntityNode );
		}

		/*
		===============
		Update
		===============
		*/
		public override void Update( float delta ) {
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="area"></param>
		/// <param name="sprite"></param>
		/// <returns></returns>
		public IEntity Create( string name, Area2D area, Sprite2D sprite ) {
			return EntityPool.Rent( EntityNode, area, sprite );
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="body"></param>
		/// <param name="animatedSprite"></param>
		/// <returns></returns>
		public IEntity Create( string name, CharacterBody2D body, AnimatedSprite2D animatedSprite ) {
			return EntityPool.Rent( EntityNode, body, animatedSprite );
		}

		/*
		===============
		Destroy
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public void Destroy( IEntity entity ) {
			EntityPool.Return( (Entity)entity );
		}
	};
};