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

using NomadCore.Infrastructure.Collections;

namespace NomadCore.Systems.EntitySystem.Domain.Events {
	public static class EventConstants {
		public static readonly InternString ANIMATION_CHANGED_EVENT = new( "EntitySystem:AnimationChanged" );
		public static readonly InternString ANIMATION_FINISHED_EVENT = new( "EntitySystem:AnimationFinished" );
		public static readonly InternString ANIMATION_LOOP_EVENT = new( "EntitySystem:AnimationLoop" );
		public static readonly InternString AREA_ENTERED_EVENT = new( "EntitySystem:AreaEntered" );
		public static readonly InternString AREA_EXITED_EVENT = new( "EntitySystem:AreaExited" ) ;
		public static readonly InternString ENTITY_VISIBILITY_CHANGED_EVENT = new( "EntitySystem:EntityVisibilityChanged" );
		public static readonly InternString FRAME_CHANGED_EVENT = new( "EntitySystem:FrameChanged" );
		public static readonly InternString NAVIGATION_DESTINATION_REACHED_EVENT = new( "EntitySystem:NavigationDestinationReached" );
		public static readonly InternString STAT_CHANGED_EVENT = new( "EntitySystem:StatChanged" );
	};
};