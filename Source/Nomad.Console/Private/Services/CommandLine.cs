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

using Nomad.Console.Events;
using Nomad.Console.Interfaces;
using Nomad.Console.ValueObjects;
using Nomad.Core;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Nomad.Console.Private.Services {
	/*
	===================================================================================
	
	CommandLine
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class CommandLine : ICommandLineService {
		public int ArgumentCount => _commandBuilder.ArgumentCount;

		private readonly ICommandBuilder _commandBuilder;
		private readonly IHistory _history;

		//
		// autocomplete
		//
		private readonly List<string> _suggestions = new List<string>();
		private int _currentSuggest = 0;
		private bool _suggesting = false;

		private readonly ICommandService _commandService;
		private readonly ILoggerService _logger;

		public IGameEvent<TextEnteredEventArgs> TextEntered => _textEntered;
		private readonly IGameEvent<TextEnteredEventArgs> _textEntered;

		public IGameEvent<CommandExecutedEventArgs> UnknownCommand => _unknownCommand;
		private readonly IGameEvent<CommandExecutedEventArgs> _unknownCommand;

		public IGameEvent<CommandExecutedEventArgs> CommandExecuted => _commandExecuted;
		private readonly IGameEvent<CommandExecutedEventArgs> _commandExecuted;

		/*
		===============
		CommandLine
		===============
		*/
		public CommandLine( ICommandBuilder builder, ICommandService commandService, ILoggerService logger, IGameEventRegistryService eventFactory ) {
			ArgumentNullException.ThrowIfNull( builder );
			ArgumentNullException.ThrowIfNull( eventFactory );

			_commandBuilder = builder;
			_commandService = commandService;
			_logger = logger;
			_history = new History( builder, logger, eventFactory );

			_textEntered = eventFactory.GetEvent<TextEnteredEventArgs>( Constants.Events.Console.TEXT_ENTERED_EVENT );
			_unknownCommand = eventFactory.GetEvent<CommandExecutedEventArgs>( Constants.Events.Console.UNKNOWN_COMMAND_EVENT );
			_commandExecuted = eventFactory.GetEvent<CommandExecutedEventArgs>( Constants.Events.Console.COMMAND_EXECUTED_EVENT );

			eventFactory.GetEvent<EmptyEventArgs>( Constants.Events.Console.CONSOLE_CLOSED_EVENT ).Subscribe( this, OnConsoleClosed );

			builder.TextEntered.Subscribe( this, OnCommandExecuted );
		}

		/*
		===============
		Dispose
		===============
		*/
		public void Dispose() {
			_textEntered?.Dispose();
			_unknownCommand?.Dispose();
			_commandExecuted?.Dispose();
		}

		/*
		===============
		ExecuteCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void ExecuteCommand( string text ) {
			ArgumentException.ThrowIfNullOrEmpty( text );
		}

		/*
		===============
		GetArgumentAt
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public string GetArgumentAt( int index ) {
			return _commandBuilder.GetArgumentAt( index );
		}

		/*
		===============
		GetArguments
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string[] GetArguments() {
			return _commandBuilder.GetArgs();
		}

		/*
		===============
		OnCommandExecuted
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="textEntered"></param>
		private void OnCommandExecuted( in TextEnteredEventArgs textEntered ) {
			string textCommand = _commandBuilder.ArgumentCount > 0 ? _commandBuilder.GetArgumentAt( 0 ) : String.Empty;

			if ( !string.IsNullOrEmpty( textCommand ) && _commandService.TryGetCommand( textCommand, out ConsoleCommand consoleCommand ) ) {
				var arguments = _commandBuilder.GetArgs();

				try {
					_commandExecuted.Publish( new CommandExecutedEventArgs( consoleCommand, arguments.Length ) );
				} catch ( Exception ex ) {
					_logger.PrintError( $"CommandLine.OnCommandExecuted: error executing command - {ex.Message}" );
					throw;
				}
			} else if ( !string.IsNullOrEmpty( textCommand ) ) {
				_unknownCommand.Publish( new CommandExecutedEventArgs() );
				_logger.PrintWarning( $"CommandLine.OnCommandExecuted: command '{textCommand}' not found." );
			}
		}

		/*
		===============
		OnConsoleClosed
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventArgsCommandExecutedEventArgs"></param>
		/// <param name="args"></param>
		private void OnConsoleClosed( in EmptyEventArgs args ) {
			//ResetAutocomplete();
		}

		/*
		===============
		ResetAutocomplete
		===============
		*/
		/// <summary>
		/// Clears the autocomplete buffer
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void ResetAutocomplete() {
			_suggestions.Clear();
			_currentSuggest = 0;
			_suggesting = false;
		}

		/*
		===============
		Autocomplete
		===============
		*/
		/// <summary>
		/// I feel like this is self-explanatory
		/// </summary>
		/*
		private void OnAutoComplete( in IGameEvent eventArgsCommandExecutedEventArgs, in IEventArgs args ) {
			if ( Suggesting ) {
				if ( CurrentSuggest < Suggestions.Count ) {
					Text = Suggestions[ CurrentSuggest ];
					CaretColumn = Text.Length;
					CurrentSuggest = ( CurrentSuggest + 1 ) % Suggestions.Count;
				}
				return;
			}

			Suggesting = true;

			if ( Text.Contains( ' ' ) ) {
				string[] splitText = ParseLineInput( Text );
				if ( splitText.Length > 1 ) {
					string command = splitText[ 0 ];
					string paramInput = splitText[ 1 ];
					if ( CommandParameters.TryGetValue( command, out var parameters ) ) {
						for ( int i = 0; i < parameters.Length; i++ ) {
							if ( parameters[ i ].Contains( paramInput ) ) {
								Suggestions.Add( $"{command} {parameters[ i ]}" );
							}
						}
					}
				}
			} else {
				List<string>? sortedCommands = [ .. ConsoleCommands
					.Where( c => !c.Value.Hidden )
					.Select( c => c.Key )
					.OrderBy( c => c ) ];

				for ( int i = 0; i < sortedCommands.Count; i++ ) {
					if ( string.IsNullOrEmpty( Text ) || sortedCommands[ i ].Contains( Text ) ) {
						Suggestions.Add( sortedCommands[ i ] );
					}
				}
			}

			if ( Suggestions.Count > 0 ) {
				OnAutoComplete( in eventArgsCommandExecutedEventArgs, in args );
			} else {
				ResetAutocomplete();
			}
		}
		*/
	};
};