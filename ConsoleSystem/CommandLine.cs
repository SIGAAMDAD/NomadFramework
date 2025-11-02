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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleSystem {
	/*
	===================================================================================
	
	CommandLine
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed partial class CommandLine : LineEdit {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct TextEnteredEventData : IEventArgs {
			public readonly string Text;
			public TextEnteredEventData( string? text ) {
				ArgumentException.ThrowIfNullOrEmpty( text );
				Text = text;
			}
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct CommandExecutedEventData : IEventArgs {
			public readonly ConsoleCommand Command;
			public readonly string[] Arguments;
			public CommandExecutedEventData( ConsoleCommand command, string[] args ) {
				Command = command;
				Arguments = args;
			}
		};

		public readonly History ConsoleHistory;

		private readonly IGameEventBusService GameEventBus;

		//
		// autocomplete
		//
		private readonly List<string> Suggestions = new List<string>();
		private int CurrentSuggest = 0;
		private bool Suggesting = false;

		public readonly IGameEvent TextEntered;
		public readonly IGameEvent UnknownCommand;
		public readonly IGameEvent CommandExecuted;

		/*
		===============
		CommandLine
		===============
		*/
		public CommandLine( IGameEventBusService? eventBus, IGameEvent autoComplete, IGameEvent historyPrev, IGameEvent historyNext ) {
			ArgumentNullException.ThrowIfNull( eventBus );
			
			TextEntered = eventBus.CreateEvent( nameof( TextEntered ) );
			UnknownCommand = eventBus.CreateEvent( nameof( UnknownCommand ) );
			CommandExecuted = eventBus.CreateEvent( nameof( CommandExecuted ) );

			GameEventBus = eventBus;

			ConsoleHistory = new History( this, eventBus, historyPrev, historyNext );
			ConsoleHistory.HistoryNext.Subscribe( this, OnHistoryNext );
			ConsoleHistory.HistoryPrev.Subscribe( this, OnHistoryPrev );

			//autoComplete.Subscribe( this, OnAutoComplete );

			Console.ConsoleClosed.Subscribe( this, OnConsoleClosed );
		}

		/*
		===============
		ExecuteCommand
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void ExecuteCommand( string? text ) {
			ArgumentException.ThrowIfNullOrEmpty( text );
			OnTextEntered( text );
		}

		/*
		===============
		OnHistoryPrev
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnHistoryPrev( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is History.HistoryPrevEventData prevEvent ) {
				//ResetAutocomplete();
				Text = prevEvent.Text;
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		OnHistoryNext
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnHistoryNext( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is History.HistoryNextEventData nextEvent ) {
				if ( !nextEvent.EndReached ) {
					Text = nextEvent.Text;
					CaretColumn = Text.Length;
					//ResetAutocomplete();
				} else {
					Clear();
					//ResetAutocomplete();
				}
			} else {
				throw new InvalidCastException( nameof( args ) );
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
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnConsoleClosed( in IGameEvent eventData, in IEventArgs args ) {
			//ResetAutocomplete();
		}

		/*
		===============
		OnLineEditTextChanged
		===============
		*/
		private void OnLineEditTextChanged( string newText ) {
			TextEntered.Publish( EmptyEventArgs.Args );
		}

		/*
		===============
		ParseLineInput
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>s
		private string[] ParseLineInput( string text ) {
			List<string> result = new List<string>();
			StringBuilder current = new StringBuilder();
			bool inQuotes = false;
			bool escaped = false;

			for ( int i = 0; i < text.Length; i++ ) {
				char c = text[ i ];

				if ( escaped ) {
					current.Append( c );
					escaped = false;
					continue;
				}

				switch ( c ) {
					case '\\':
						escaped = true;
						continue;
					case '\"':
						inQuotes = !inQuotes;
						continue;
					default:
						if ( char.IsWhiteSpace( c ) && !inQuotes ) {
							if ( current.Length > 0 ) {
								result.Add( current.ToString() );
								current.Clear();
							}
							continue;
						}
						break;
				}

				current.Append( c );
			}

			if ( current.Length > 0 ) {
				result.Add( current.ToString() );
			}

			return [ .. result ];
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
			Suggestions.Clear();
			CurrentSuggest = 0;
			Suggesting = false;
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
		private void OnAutoComplete( in IGameEvent eventData, in IEventArgs args ) {
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
				OnAutoComplete( in eventData, in args );
			} else {
				ResetAutocomplete();
			}
		}
		*/

		/*
		===============
		OnTextEntered
		===============
		*/
		private void OnTextEntered( string newText ) {
			//			ResetAutocomplete();
			Clear();
			CallDeferred( LineEdit.MethodName.Clear );

			if ( newText.Trim().Length > 0 ) {
				TextEntered.Publish( new TextEnteredEventData( newText ) );
				string[] textSplit = ParseLineInput( newText );
				string textCommand = textSplit.Length > 0 ? textSplit[ 0 ] : "";

				if ( !string.IsNullOrEmpty( textCommand ) && Console.TryGetCommand( textCommand, out ConsoleCommand consoleCommand ) ) {
					string[] arguments = [ .. textSplit.Skip( 1 ) ];

					try {
						consoleCommand.Callback.Invoke( CommandExecuted, new CommandExecutedEventData( consoleCommand, arguments ) );
						CommandExecuted.Publish( new CommandExecutedEventData( consoleCommand, arguments ) );
					} catch ( Exception ex ) {
						Console.PrintError( $"Error executing command: {ex.Message}" );
					}
				} else if ( !string.IsNullOrEmpty( textCommand ) ) {
					UnknownCommand.Publish( EmptyEventArgs.Args );
					Console.PrintError( "Command not found." );
				}
			}
		}

		/*
		===============
		_Ready
		===============
		*/
		public override void _Ready() {
			base._Ready();

			Name = nameof( CommandLine );
			AnchorTop = 0.5f;
			AnchorRight = 1.0f;
			AnchorBottom = 0.5f;
			PlaceholderText = "";

			GameEventBus.ConnectSignal( this, LineEdit.SignalName.TextSubmitted, this, Callable.From<string>( OnTextEntered ) );
			GameEventBus.ConnectSignal( this, LineEdit.SignalName.TextChanged, this, Callable.From<string>( OnLineEditTextChanged ) );
		}
	};
};