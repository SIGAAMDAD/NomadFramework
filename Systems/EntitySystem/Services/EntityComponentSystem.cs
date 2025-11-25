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

	public sealed partial class EntityComponentSystem : Node, IEntityService {
		private readonly EntityManager Manager;
		private readonly PhysicsSystem Physics;

		/*
		===============
		EntityComponentSystem
		===============
		*/
		public EntityComponentSystem( Node? rootNode ) {
			ArgumentNullException.ThrowIfNull( rootNode );

			Manager = new EntityManager( rootNode );
			Physics = new PhysicsSystem( rootNode.GetViewport().World2D.Space, this );
		}

		/*
		===============
		Initialize
		===============
		*/
		public void Initialize() {
		}

		/*
		===============
		Shutdown
		===============
		*/
		public void Shutdown() {
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public IEntity Create( Area2D owner ) {
			return Manager.Create( owner );
		}

		/*
		===============
		Create
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public IEntity Create( CharacterBody2D owner ) {
			return Manager.Create( owner );
		}

		/*
		===============
		GetEntities
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEntity[] GetEntities() {
			return null;
		}

		/*
		===============
		_Process
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process( double delta ) {
			base._Process( delta );

			float deltaTime = (float)delta;
			Manager.Update( deltaTime );
			Physics.Update( deltaTime );
		}
	};
};