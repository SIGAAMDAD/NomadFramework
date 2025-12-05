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
using NomadCore.Systems.EntitySystem.Infrastructure;
using NomadCore.Systems.EntitySystem.Infrastructure.Physics;
using System;

namespace NomadCore.Systems.EntitySystem.Services {
	/*
	===================================================================================
	
	EntityComponentSystem
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class EntityComponentSystem : Node, IEntityComponentSystemService {
		private readonly EntityManager _manager;
		private readonly PhysicsSystem _physics;
		private readonly ComponentManager _componentManager;

		/*
		===============
		EntityComponentSystem
		===============
		*/
		public EntityComponentSystem( Node? rootNode ) {
			ArgumentNullException.ThrowIfNull( rootNode );

			_manager = new EntityManager( this, rootNode );
			_physics = new PhysicsSystem( this, rootNode.GetViewport().World2D.Space, this );
			_componentManager = new ComponentManager();
		}

		public void AddComponents( IEntity entity, Span<IComponent> components ) {
		}

		public IEntity CreateEntity( string name, Area2D area, Sprite2D sprite ) {
			return _manager.Create( name, area, sprite );
		}

		public IEntity CreateEntity( string name, CharacterBody2D body, AnimatedSprite2D animatedSprite ) {
			return _manager.Create( name, body, animatedSprite );
		}

		public void DestroyEntity( IEntity entity, bool immediate = false ) {
			_manager.Destroy( entity );
		}

		public IEntity FindEntityByName( string name ) {
			throw new NotImplementedException();
		}

		public bool HasComponent<T>( IEntity entity ) where T : struct, IComponent {
			return _componentManager.HasComponent<T>( entity );
		}

		public ref T GetOrAddComponent<T>( IEntity entity ) where T : struct, IComponent {
			if ( !_componentManager.HasComponent<T>( entity ) ) {
				_componentManager.AddComponent( entity, new T() );
			}
			return ref _componentManager.GetComponent<T>( entity );
		}

		public ref T GetOrAddComponent<T>( IEntity entity, T defaultValue ) where T : struct, IComponent {
			if ( !_componentManager.HasComponent<T>( entity ) ) {
				_componentManager.AddComponent( entity, defaultValue );
			}
			return ref _componentManager.GetComponent<T>( entity );
		}

		public void Initialize() {
			throw new NotImplementedException();
		}

		public void PostUpdate( float deltaTime ) {
			throw new NotImplementedException();
		}

		public void PreUpdate( float deltaTime ) {
			throw new NotImplementedException();
		}

		public bool TryGetComponent<T>( IEntity entity, out T component ) where T : struct, IComponent {
			throw new NotImplementedException();
		}

		public void Update( float deltaTime ) {
			_physics.Update( deltaTime );
			_manager.Update( deltaTime );
		}
	};
};