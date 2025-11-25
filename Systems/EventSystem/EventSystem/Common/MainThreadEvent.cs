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
using NomadCore.Enums.EventSystem;
using NomadCore.Interfaces.EventSystem;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.EventSystem.Common {
	/*
	===================================================================================
	
	MainThreadEvent
	
	===================================================================================
	*/
	/// <summary>
	/// An event that runs on Godot's main thread
	/// </summary>
	
	public class MainThreadEvent<TArgs>( string? name ) : GameEvent<TArgs>( name ) where TArgs : IEventArgs {
		/*
		===============
		Publish
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventArgs"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Publish( TArgs eventArgs ) {
			if ( OS.GetThreadCallerId() == OS.GetMainThreadId() ) {
				base.Publish( eventArgs );
			} else {
				Callable.From( () => base.Publish( eventArgs ) ).CallDeferred();
			}
		}
		
		/*
		===============
		GetDefaultThreadingPolicy
		===============
		*/
		public override EventThreadingPolicy GetDefaultThreadingPolicy() => EventThreadingPolicy.MainThread;
	};
};