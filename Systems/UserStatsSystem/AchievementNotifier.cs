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
using System.Collections.Generic;

namespace NomadCore.Systems.UserStatsSystem {
	[Icon( "res://addons/milestone/icons/editor/icon-x16.svg" )]
	public sealed partial class AchievementNotifier : Node {
		private enum ScreenPosition : int {
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		};

		private const int MAX_STACKED_NOTIFICATIONS = 4;
		private const int NOTIFICATION_SPACING = 0;
		private const int MARGIN = 0;
		private const float ON_SCREEN_DURATION = 5.0f;

		private static readonly string BUS_NAME = "Master";

		[Export]
		private ScreenPosition ScreenCorner = ScreenPosition.BottomRight;
		[Export]
		private Node UserInterface;
		[Export]
		private PackedScene NotificationComponent = ResourceLoader.Load<PackedScene>( "res://addons/milestone/components/achievement_notification.tscn" );
		[Export]
		private float AnimationDuration = 0.2f;

		private readonly Queue<Resource> Queue = new Queue<Resource>();
		private readonly Dictionary<StringName, Timer> NotificationTimers = new Dictionary<StringName, Timer>();
		private readonly Dictionary<StringName, Tween> NotificationTweens = new Dictionary<StringName, Tween>();

		private readonly AudioStream UnlockedSound;
		private readonly AudioStream ProgressSound;

		public override void _Ready() {
			base._Ready();
		}

		public override void _ExitTree() {
			base._ExitTree();
		}
	};
};