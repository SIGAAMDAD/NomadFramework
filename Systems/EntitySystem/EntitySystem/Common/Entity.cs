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
using NomadCore.Infrastructure;
using NomadCore.Interfaces.Audio;
using NomadCore.Enums.Audio;
using NomadCore.Systems.EntitySystem.Infrastructure.Physics;
using NomadCore.Systems.EntitySystem.Interfaces;
using NomadCore.Systems.EntitySystem.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NomadCore.Interfaces.EntitySystem;
using NomadCore.Infrastructure.Rendering;

namespace NomadCore.Systems.EntitySystem.Common {
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

	public partial class Entity : IEntity, IDisposable {
		public int Id => _id;
		private readonly int _id;

		public readonly IAudioSource AudioPlayer;

		public IPhysicsBody PhysicsBody => _body;
		protected readonly IPhysicsBody _body;

		public IRenderEntity RenderEntity => _renderEntity;
		protected readonly IRenderEntity _renderEntity;

		protected readonly EntityComponentSystem _ecs;

		protected readonly IStatService _stats;

		/*
		===============
		Entity
		===============
		*/
		private Entity( EntityComponentSystem ecs, Node2D entityNode, int hashCode ) {
			_id = hashCode;
			_ecs = ecs;
			_stats = new StatManager( this, new Dictionary<string, IEntityStat>() );

			AudioPlayer = ServiceRegistry.Get<IAudioService>().CreateSource( SourceType.Entity );
		}

		/*
		===============
		Entity
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityNode"></param>
		/// <param name="area"></param>
		/// <param name="sprite"></param>
		internal Entity( EntityComponentSystem ecs, Node2D entityNode, Area2D area, Sprite2D sprite )
			: this( ecs, entityNode, area.GetPath().GetHashCode() )
		{
			_body = new Area( area );
			_renderEntity = new ServerSprite( this, sprite );
		}

		/*
		===============
		Entity
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityNode"></param>
		/// <param name="characterBody"></param>
		/// <param name="animatedSprite"></param>
		internal Entity( EntityComponentSystem ecs, Node2D entityNode, CharacterBody2D characterBody, AnimatedSprite2D animatedSprite )
			: this( ecs, entityNode, characterBody.GetPath().GetHashCode() )
		{ }

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			GC.SuppressFinalize( this );
		}
	};
};