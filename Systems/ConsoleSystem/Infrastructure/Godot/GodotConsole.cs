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
using NomadCore.Infrastructure;
using NomadCore.Systems.ConsoleSystem.Events;
using NomadCore.Systems.ConsoleSystem.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.ConsoleSystem.Infrastructure {
	/*
	===================================================================================
	
	GodotConsole
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class GodotConsole : Control {
		[Export]
		public bool Enabled { get; private set; } = true;
		[Export]
		public bool EnableOnReleaseBuild { get; private set; } = true;
		[Export]
		public bool PauseEnabled { get; private set; } = false;

		private readonly ConsoleCommand Quit;
		private readonly ConsoleCommand Exit;

		private readonly IConsoleEvents Events;
		private readonly GodotCommandBuilder CommandBuilder;

		private bool WasPausedAlready = false;

		/*
		===============
		GodotConsole
		===============
		*/
		public GodotConsole( ICommandBuilder? builder, IConsoleEvents? events ) {
			ArgumentNullException.ThrowIfNull( builder );
			ArgumentNullException.ThrowIfNull( events );

			Events = events;
			CommandBuilder = (GodotCommandBuilder)builder;

			Quit = new ConsoleCommand( "quit", OnQuit, "Closes the game application." );
			Exit = new ConsoleCommand( "exit", OnQuit, "Exits the running application." );
		}

		/*
		===============
		ToggleConsole
		===============
		*/
		/// <summary>
		/// Handles internal processing mode flip-flopping.
		/// </summary>
		public void ToggleConsole() {
			if ( Enabled ) {
				Visible = !Visible;
			} else {
				Visible = false;
			}

			if ( Visible ) {
				WasPausedAlready = GetTree().Paused;
				GetTree().Paused = WasPausedAlready || PauseEnabled;
				Events.ConsoleOpened.Publish( EmptyEventArgs.Args );
			} else {
				AnchorBottom = 1.0f;
				if ( PauseEnabled && !WasPausedAlready ) {
					GetTree().Paused = false;
				}
				Events.ConsoleClosed.Publish( EmptyEventArgs.Args );
			}
		}

		/*
		===============
		ToggleSize
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void ToggleSize() {
			AnchorBottom = AnchorBottom == 1.0f ? 1.9f : 1.0f;
		}

		/*
		===============
		HandleOpenConsole
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyEvent"></param>
		private void HandleToggleConsole( InputEventKey keyEvent ) {
			if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Quoteleft ) {
				if ( keyEvent.IsCommandOrControlPressed() ) {
					if ( Visible ) {
						ToggleSize();
					} else {
						ToggleConsole();
						ToggleSize();
					}
					GetViewport().SetInputAsHandled();
				} else {
					ToggleConsole();
					GetViewport().SetInputAsHandled();
				}
			} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Escape && Visible ) {
				ToggleConsole();
				GetViewport().SetInputAsHandled();
			}
		}

		/*
		===============
		OnQuit
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnQuit( in CommandExecutedEventData args ) {
			GetTree().Quit();
		}

		/*
		===============
		_EnterTree
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _EnterTree() {
			base._EnterTree();

			Name = nameof( GodotConsole );
			AnchorBottom = 1.0f;
			AnchorRight = 1.0f;
			Visible = false;
			TopLevel = true;

			ProcessMode = ProcessModeEnum.Always;

			AddChild( CommandBuilder );
		}

		/*
		===============
		_Input
		===============
		*/
		/// <summary>
		/// Handles input for the console
		/// </summary>
		/// <param name="event"></param>
		public override void _Input( InputEvent @event ) {
			if ( @event is InputEventKey keyEvent && keyEvent != null && keyEvent.Pressed ) {
				// ~ or ESC key
				HandleToggleConsole( keyEvent );

				if ( Visible ) {
					if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Up ) {
						GetViewport().SetInputAsHandled();
						Events.HistoryPrev.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Down ) {
						GetViewport().SetInputAsHandled();
						Events.HistoryNext.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Pageup ) {
						GetViewport().SetInputAsHandled();
						Events.PageUp.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Pagedown ) {
						GetViewport().SetInputAsHandled();
						Events.PageDown.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Tab ) {
						GetViewport().SetInputAsHandled();
						//Events.AutoComplete.Publish( EmptyEventArgs.Args );
					}
				}
			}
		}
	};
};