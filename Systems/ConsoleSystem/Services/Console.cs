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
using NomadCore.Interfaces;
using NomadCore.Systems.ConsoleSystem.Events;
using NomadCore.Systems.ConsoleSystem.Infrastructure;
using NomadCore.Systems.ConsoleSystem.Infrastructure.Sinks;
using NomadCore.Systems.ConsoleSystem.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.ConsoleSystem.Services {
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

	public sealed partial class Console : IConsoleService, IConsoleEvents {
		private readonly ILoggerService Logger;
		private readonly IHistory History;
		private readonly ICommandLine CommandLine;

		private static readonly ConcurrentDictionary<string, ConsoleCommand> CommandCache = new ConcurrentDictionary<string, ConsoleCommand>();

		private bool WasPausedAlready = false;

		private static Console Instance;

		internal readonly ConsoleCommand Quit;
		internal readonly ConsoleCommand Exit;
		internal readonly ConsoleCommand Clear;
		internal readonly ConsoleCommand DeleteHistory;
		internal readonly ConsoleCommand ListCommands;
		internal readonly ConsoleCommand Execute;
		internal readonly ConsoleCommand Echo;

		public IGameEvent ConsoleOpened => _consoleOpened;
		private readonly IGameEvent _consoleOpened;

		public IGameEvent ConsoleClosed => _consoleClosed;
		private readonly IGameEvent _consoleClosed;

		public IGameEvent HistoryPrev => _historyPrev;
		private readonly IGameEvent _historyPrev;

		public IGameEvent HistoryNext => _historyNext;
		private readonly IGameEvent _historyNext;

		public IGameEvent AutoComplete => _autoComplete;
		private readonly IGameEvent _autoComplete;

		public IGameEvent PageUp => _pageUp;
		private readonly IGameEvent _pageUp;

		public IGameEvent PageDown => _pageDown;
		private readonly IGameEvent _pageDown;

		/*
		===============
		Console
		===============
		*/
		public Console( Node? rootNode, IGameEventBusService? eventBus ) {
			ArgumentNullException.ThrowIfNull( rootNode );
			ArgumentNullException.ThrowIfNull( eventBus );

			Instance = this;

			_consoleOpened = eventBus.CreateEvent( nameof( ConsoleOpened ) );
			_consoleClosed = eventBus.CreateEvent( nameof( ConsoleClosed ) );
			_historyPrev = eventBus.CreateEvent( nameof( HistoryPrev ) );
			_historyNext = eventBus.CreateEvent( nameof( HistoryNext ) );
			_autoComplete = eventBus.CreateEvent( nameof( AutoComplete) );
			_pageUp = eventBus.CreateEvent( nameof( PageUp ) );
			_pageDown = eventBus.CreateEvent( nameof( PageDown ) );

			ICommandBuilder commandBuilder = new GodotCommandBuilder( eventBus, this );
			GodotConsole console = new GodotConsole( commandBuilder, this );
			History = new History( commandBuilder, eventBus, this, this );
			CommandLine = new CommandLine( commandBuilder, this, eventBus );
			Logger = new LoggerService( CommandLine, [
				new GodotSink(),
				new FileSink(),
				new InGameSink( console, commandBuilder, this )
			] );

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
			return [ .. CommandCache.Values ];
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

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintLine<TString>( TString message ) where TString : notnull => Logger.PrintLine( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintDebug<TString>( TString message ) where TString : notnull => Logger.PrintDebug( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintError<TString>( TString message ) where TString : notnull => Logger.PrintError( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void PrintWarning<TString>( TString message ) where TString : notnull => Logger.PrintWarning( message as string );
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void ExecuteCommand<TString>( TString text ) where TString : notnull => CommandLine.ExecuteCommand( text as string );

		/*
		===============
		PrintError
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintError( string message ) => Instance?.CallDeferred( MethodName.PrintError, message );

		/*
		===============
		PrintWarning
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintWarning( string message ) => Instance?.CallDeferred( MethodName.PrintWarning, message );

		/*
		===============
		PrintLine
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintLine( string message ) => Instance?.CallDeferred( MethodName.PrintLine, message );

		/*
		===============
		PrintDebug
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void PrintDebug( string message ) => Instance?.CallDeferred( MethodName.PrintDebug, message );

		/*
		===============
		ExecuteCommand
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void ExecuteCommand( string text ) => Instance?.CallDeferred( MethodName.ExecuteCommand, text );

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		/// Clears the console text.
		/// </summary>
		/// <param name="args"></param>
		private void OnClear( in CommandExecutedEventData args ) {
			Logger.Clear();
		}

		/*
		===============
		OnDeleteHistory
		===============
		*/
		/// <summary>
		/// Clears the console's history buffer, resets the index, and deletes the history file
		/// </summary>
		/// <param name="args"></param>
		private void OnDeleteHistory( in CommandExecutedEventData args ) {
			CommandLine.ConsoleHistory.Clear();
		}

		/*
		===============
		OnListCommands
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="InvalidCastException"></exception>
		private void OnListCommands( in CommandExecutedEventData args ) {
			ConsoleCommand[] commandList = GetCommands();
			for ( int i = 0; i < commandList.Length; i++ ) {
				PrintLine( $"{commandList[ i ].Name}: {commandList[ i ].Description}" );
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
		/// <param name="args"></param>
		/// <exception cref="InvalidCastException"></exception>
		private void OnEcho( in CommandExecutedEventData args ) {
			PrintLine( args.Arguments[ 0 ] );
		}

		/*
		===============
		OnExec
		===============
		*/
		/// <summary>
		/// Executes the commands found in the given file.
		/// </summary>
		/// <param name="args"></param>
		private void OnExec( in CommandExecutedEventData args ) {
			string? filename = args.Arguments[ 0 ];
			ArgumentException.ThrowIfNullOrEmpty( filename );

			string path = $"user://{filename}";

			using FileAccess file = FileAccess.Open( path, FileAccess.ModeFlags.Read );
			if ( file != null ) {
				while ( !file.EofReached() ) {
					ExecuteCommand( file.GetLine() );
				}
			} else {
				PrintError( $"Error opening file at path {path}" );
			}
		}
	};
};