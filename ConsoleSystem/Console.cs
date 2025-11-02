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

using EventSystem;
using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConsoleSystem {
	/*
	===================================================================================

	Console

	===================================================================================
	*/
	/// <summary>
	/// The main logger for the game. Use to convey information both boring and important to the
	/// terminal (tty), godot console, the logfile, and the in-game console.
	/// </summary>
	/// <remarks>
	/// This implementation is currently threadsafe.
	/// </remarks>

	public sealed partial class Console : Control, IConsoleService {
		private readonly ConsoleCommand Quit;
		private readonly ConsoleCommand Exit;
		private readonly ConsoleCommand Clear;
		private readonly ConsoleCommand DeleteHistory;
		private readonly ConsoleCommand ListCommands;
		private readonly ConsoleCommand Execute;
		private readonly ConsoleCommand Echo;

		public static readonly string LOG_FILE = "user://debug.log";

		[Export]
		public bool Enabled { get; private set; } = true;
		[Export]
		public bool EnableOnReleaseBuild { get; private set; } = true;
		[Export]
		public bool PauseEnabled { get; private set; } = false;

		private readonly Logger Logger;
		private readonly CommandLine CommandLine;

		private static readonly ConcurrentDictionary<string, ConsoleCommand> CommandCache = new ConcurrentDictionary<string, ConsoleCommand>();

//		private readonly Dictionary<string, string[]> CommandParameters = new Dictionary<string, string[]>();
		private bool WasPausedAlready = false;
		private readonly object LockObject = new object();

		private static Console Instance;

		public static IGameEvent ConsoleOpened { get; private set; }
		public static IGameEvent ConsoleClosed { get; private set; }

		private readonly IGameEvent HistoryPrev;
		private readonly IGameEvent HistoryNext;
		private readonly IGameEvent AutoComplete;
		private readonly IGameEvent PageUp;
		private readonly IGameEvent PageDown;

		/*
		===============
		Console
		===============
		*/
		public Console( IGameEventBusService? eventBus ) {
			ArgumentNullException.ThrowIfNull( eventBus );

			Instance = this;

			ConsoleOpened = eventBus.CreateEvent( nameof( ConsoleOpened ) );
			ConsoleClosed = eventBus.CreateEvent( nameof( ConsoleClosed ) );
			AutoComplete = eventBus.CreateEvent( nameof( AutoComplete ) );
			HistoryPrev = eventBus.CreateEvent( nameof( HistoryPrev ) );
			HistoryNext = eventBus.CreateEvent( nameof( HistoryNext ) );
			PageDown = eventBus.CreateEvent( nameof( PageDown ) );
			PageUp = eventBus.CreateEvent( nameof( PageUp ) );

			CommandLine = new CommandLine( eventBus, AutoComplete, HistoryPrev, HistoryNext );
			Logger = new Logger( this, CommandLine, ProjectSettings.GlobalizePath( LOG_FILE ), PageDown, PageUp );

			Quit = new ConsoleCommand( "quit", OnQuit, "Quits the game." );
			Exit = new ConsoleCommand( "quit", OnQuit, "Quits the game." );
			Clear = new ConsoleCommand( "clear", OnClear, "Clears the console." );
			DeleteHistory = new ConsoleCommand( "delete_history", OnDeleteHistory, "Clears the console's history data." );
			ListCommands = new ConsoleCommand( "cmdlist", OnListCommands, "Lists all commands." );
			Execute = new ConsoleCommand( "exec", OnExec, "Executes the provided script." );
			Echo = new ConsoleCommand( "echo", OnEcho, "Prints a string to the console." );
		}

		/*
		===============
		GetCommands
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static ConsoleCommand[] GetCommands() {
			return CommandCache.Values.ToArray();
		}

		/*
		===============
		TryGetCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="commandName"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool TryGetCommand( string? commandName, out ConsoleCommand cmd ) {
			ArgumentException.ThrowIfNullOrEmpty( commandName );
			return CommandCache.TryGetValue( commandName, out cmd );
		}

		/*
		===============
		AddCommand
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void AddCommand( ConsoleCommand command ) {
			if ( CommandCache.ContainsKey( command.Name ) ) {
				return;
			}
			CommandCache.TryAdd( command.Name, command );
		}

		/*
		===============
		RemoveCommand
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void RemoveCommand( ConsoleCommand command ) {
			if ( !CommandCache.ContainsKey( command.Name ) ) {
				return;
			}
			CommandCache.TryRemove( new KeyValuePair<string, ConsoleCommand>( command.Name, command ) );
		}

		/*
		===============
		SetEnableOnReleaseBuild
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="enable"></param>
		public static void SetEnableOnReleaseBuild( bool enable ) {
			Instance.EnableOnReleaseBuild = enable;
			if ( !Instance.EnableOnReleaseBuild && !OS.IsDebugBuild() ) {
				Instance.Disable();
			}
		}

		/*
		===============
		Disable
		===============
		*/
		/// <summary>
		/// Disables console processing
		/// </summary>
		public void Disable() {
			Enabled = false;
			ToggleConsole();
		}

		/*
		===============
		Enable
		===============
		*/
		/// <summary>
		/// Enables console processing
		/// </summary>
		public void Enable() {
			Enabled = true;
		}

		/*
		===============
		ToggleConsole
		===============
		*/
		/// <summary>
		/// Handles internal processing mode flip-flopping
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
				CommandLine.GrabFocus();
				ConsoleOpened.Publish( EmptyEventArgs.Args );
			} else {
				AnchorBottom = 1.0f;
				if ( PauseEnabled && !WasPausedAlready ) {
					GetTree().Paused = false;
				}
				ConsoleClosed.Publish( EmptyEventArgs.Args );
			}
		}

		/*
		===============
		AddCommandAutocompleteList
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="commandName"></param>
		/// <param name="paramList"></param>
		/*
		public static void AddCommandAutocompleteList( string? commandName, string[]? paramList ) {
			ArgumentException.ThrowIfNullOrEmpty( commandName );
			ArgumentNullException.ThrowIfNull( paramList );

			Instance.CommandParameters.TryAdd( commandName, paramList );
		}
		*/

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
						HistoryPrev.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Down ) {
						GetViewport().SetInputAsHandled();
						HistoryNext.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Pageup ) {
						GetViewport().SetInputAsHandled();
						PageUp.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Pagedown ) {
						GetViewport().SetInputAsHandled();
						PageDown.Publish( EmptyEventArgs.Args );
					} else if ( keyEvent.GetPhysicalKeycodeWithModifiers() == Key.Tab ) {
						GetViewport().SetInputAsHandled();
						AutoComplete.Publish( EmptyEventArgs.Args );
					}
				}
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

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintLine<TString>( TString message ) => Logger.PrintLine( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintDebug<TString>( TString message ) => Logger.PrintDebug( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintError<TString>( TString message ) => Logger.PrintError( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintWarning<TString>( TString message ) => Logger.PrintWarning( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void ExecuteCommandStringInternal( string text ) => CommandLine.ExecuteCommand( text );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintError( string message ) => Instance?.CallDeferred( MethodName.PrintError, message );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintWarning( string message ) => Instance?.CallDeferred( MethodName.PrintWarning, message );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintLine( string message ) => Instance?.CallDeferred( MethodName.PrintLine, message );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintDebug( string message ) => Instance?.CallDeferred( MethodName.PrintDebug, message );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void ExecuteCommandString( string text ) => Instance?.CallDeferred( MethodName.ExecuteCommandStringInternal, text );

		/*
		===============
		_EnterTree
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public override void _EnterTree() {
			Name = nameof( Console );
			AnchorBottom = 1.0f;
			AnchorRight = 1.0f;
			Visible = false;
			TopLevel = true;

			ProcessMode = ProcessModeEnum.Always;

			AddChild( CommandLine );
		}

		/*
		===============
		Quit
		===============
		*/
		/// <summary>
		/// Closes the game.
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnQuit( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecute ) {
				GetTree().Quit();
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		/// Clears the console text.
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnClear( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecute ) {
				Logger.RichLabel.Clear();
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnDeleteHistory
		===============
		*/
		/// <summary>
		/// Clears the console's history buffer, resets the index, and deletes the history file
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnDeleteHistory( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecuted ) {
				CommandLine.ConsoleHistory.Clear();
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnListCommands
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		/// <exception cref="InvalidCastException"></exception>
		private void OnListCommands( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecuted ) {
				ConsoleCommand[] commandList = GetCommands();
				for ( int i = 0; i < commandList.Length; i++ ) {
					PrintLine( $"{commandList[ i ].Name}: {commandList[ i ].Description}" );
				}
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnEcho
		===============
		*/
		/// <summary>
		/// Prints the string found in the arguments to the console.
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		/// <exception cref="InvalidCastException"></exception>
		private void OnEcho( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecuted ) {
				PrintLine( commandExecuted.Arguments[ 0 ] );
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnExec
		===============
		*/
		/// <summary>
		/// Executes the commands found in the given file
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnExec( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecuted ) {
				string? filename = commandExecuted.Arguments[ 0 ];
				ArgumentException.ThrowIfNullOrEmpty( filename );

				string path = $"user://{filename}";

				using Godot.FileAccess file = Godot.FileAccess.Open( path, Godot.FileAccess.ModeFlags.Read );
				if ( file != null ) {
					while ( !file.EofReached() ) {
						ExecuteCommandString( file.GetLine() );
					}
				} else {
					PrintError( $"Error opening file at path {path}" );
				}
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}
	};
};