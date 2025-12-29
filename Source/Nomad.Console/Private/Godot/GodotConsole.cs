/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Godot;
using Nomad.Console.Events;
using Nomad.Core.Events;
using Nomad.Core;
using System;
using System.Runtime.CompilerServices;
using Nomad.Console.Interfaces;
using Nomad.Console.ValueObjects;
using Nomad.Core.Logger;

namespace Nomad.Console.Private.Godot {
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

		private readonly GodotCommandBuilder _commandBuilder;
		private readonly ILoggerService _logger;

		private bool _wasPausedAlready = false;

		private readonly IGameEvent<EmptyEventArgs> _consoleOpened;
		private readonly IGameEvent<EmptyEventArgs> _consoleClosed;
		private readonly IGameEvent<HistoryPrevEventArgs> _historyPrev;
		private readonly IGameEvent<HistoryNextEventArgs> _historyNext;
		private readonly IGameEvent<EmptyEventArgs> _pageDown;
		private readonly IGameEvent<EmptyEventArgs> _pageUp;

		/*
		===============
		GodotConsole
		===============
		*/
		public GodotConsole( ICommandBuilder builder, ICommandService commands, ILoggerService logger, IGameEventRegistryService eventRegistry ) {
			ArgumentNullException.ThrowIfNull( builder );

			_logger = logger;

			_consoleClosed = eventRegistry.GetEvent<EmptyEventArgs>( Constants.Events.Console.CONSOLE_CLOSED_EVENT );
			_consoleOpened = eventRegistry.GetEvent<EmptyEventArgs>( Constants.Events.Console.CONSOLE_OPENED_EVENT );
			_historyNext = eventRegistry.GetEvent<HistoryNextEventArgs>( Constants.Events.Console.HISTORY_NEXT_EVENT );
			_historyPrev = eventRegistry.GetEvent<HistoryPrevEventArgs>( Constants.Events.Console.HISTORY_PREV_EVENT );
			_pageDown = eventRegistry.GetEvent<EmptyEventArgs>( Constants.Events.Console.PAGE_DOWN_EVENT );
			_pageUp = eventRegistry.GetEvent<EmptyEventArgs>( Constants.Events.Console.PAGE_UP_EVENT );
			
			_commandBuilder = builder as GodotCommandBuilder ?? throw new InvalidOperationException( "Cannot create a GodotConsole without a GodotCommandBuilder!" );
			_commandBuilder.TextEntered.Subscribe( this, OnTextEntered );

			commands.RegisterCommand( new ConsoleCommand( Constants.Commands.Console.QUIT_COMMAND, OnQuit, "Closes the game application." ) );
			commands.RegisterCommand( new ConsoleCommand( Constants.Commands.Console.EXIT_COMMAND, OnQuit, "Exits the running application." ) );
			commands.RegisterCommand( new ConsoleCommand( Constants.Commands.Console.ECHO_COMMAND, OnEcho ) );
			commands.RegisterCommand( new ConsoleCommand( Constants.Commands.Console.CLEAR_COMMAND, OnClear ) );

			AddThemeFontOverride( "SourceCodePro-ExtraLight", ResourceLoader.Load<Font>( "res://Assets/Fonts/SourceCodePro-ExtraLight.ttf" ) );
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
				_wasPausedAlready = GetTree().Paused;
				GetTree().Paused = _wasPausedAlready || PauseEnabled;
				_consoleOpened.Publish( EmptyEventArgs.Args );
			} else {
				AnchorBottom = 1.0f;
				if ( PauseEnabled && !_wasPausedAlready ) {
					GetTree().Paused = false;
				}
				_consoleClosed.Publish( EmptyEventArgs.Args );
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
		private void OnQuit( in CommandExecutedEventArgs args ) {
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

			AddChild( _commandBuilder );
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
						_historyPrev.Publish( new HistoryPrevEventArgs() );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Down ) {
						GetViewport().SetInputAsHandled();
						_historyNext.Publish( new HistoryNextEventArgs() );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Pageup ) {
						GetViewport().SetInputAsHandled();
						_pageUp.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Pagedown ) {
						GetViewport().SetInputAsHandled();
						_pageDown.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Tab ) {
						GetViewport().SetInputAsHandled();
						//Events.AutoComplete.Publish( EmptyEventArgs.Args );
					}
				}
			}
		}

		/*
		===============
		OnClear
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void OnClear( in CommandExecutedEventArgs args ) {
			_logger.Clear();
		}

		/*
		===============
		OnEcho
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void OnEcho( in CommandExecutedEventArgs args ) {
			_logger.PrintLine( in _commandBuilder.GetArgs()[ 0 ] );
		}

		/*
		===============
		OnTextEntered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void OnTextEntered( in TextEnteredEventArgs args ) {
			_logger.PrintLine( $"] {args.Text}" );
		}
	};
};