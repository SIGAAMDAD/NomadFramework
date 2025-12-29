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
using Nomad.Console.Interfaces;
using Nomad.Console.Private.Services;
using Nomad.Core;
using Nomad.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nomad.Console.Private.Godot {
	/*
	===================================================================================
	
	GodotCommandBuilder
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed partial class GodotCommandBuilder : LineEdit, ICommandBuilder {
		public int ArgumentCount => _arguments.Count;

		private readonly IGodotEventBusService _eventBus;

		private readonly List<string> _arguments = new List<string>();
		private readonly StringBuilder _commandBuilder = new StringBuilder();

		public IGameEvent<TextEnteredEventArgs> TextEntered => _textEntered;
		private readonly IGameEvent<TextEnteredEventArgs> _textEntered;

		/*
		===============
		GodotCommandBuilder
		===============
		*/
		public GodotCommandBuilder( IGodotEventBusService eventBus, IGameEventRegistryService eventFactory ) {
			ArgumentNullException.ThrowIfNull( eventBus );
			ArgumentNullException.ThrowIfNull( eventFactory );

			_eventBus = eventBus;

			_textEntered = eventFactory.GetEvent<TextEnteredEventArgs>( Constants.Events.Console.TEXT_ENTERED_EVENT );

			eventFactory.GetEvent<EmptyEventArgs>( Constants.Events.Console.CONSOLE_OPENED_EVENT ).Subscribe( this, OnConsoleOpened );
			eventFactory.GetEvent<EmptyEventArgs>( Constants.Events.Console.CONSOLE_CLOSED_EVENT ).Subscribe( this, OnConsoleClosed );
		}

		/*
		===============
		GetArgumentAt
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public string GetArgumentAt( int index ) {
			return _arguments[ index ];
		}

		/*
		===============
		GetArgs
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public string[] GetArgs() {
			return [ .. _arguments.Skip( 1 ) ];
		}

		/*
		===============
		OnHistoryPrev
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void OnHistoryPrev( in HistoryPrevEventArgs args ) {
			//ResetAutocomplete();
			Text = args.Text;
		}

		/*
		===============
		OnHistoryNext
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void OnHistoryNext( in HistoryNextEventArgs args ) {
			if ( !args.EndReached ) {
				Text = args.Text;
				CaretColumn = Text.Length;
				//ResetAutocomplete();
			} else {
				Clear();
				//ResetAutocomplete();
			}
		}

		/*
		===============
		OnTextEntered
		===============
		*/
		public void OnTextEntered( string newText ) {
			//			ResetAutocomplete();
			Clear();
			CallDeferred( LineEdit.MethodName.Clear );

			if ( newText.Trim().Length > 0 ) {
				ParseLineInput( newText );
				TextEntered.Publish( new TextEnteredEventArgs() );
			}
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
		private void ParseLineInput( string text ) {
			bool inQuotes = false;
			bool escaped = false;

			_arguments.Clear();

			for ( int i = 0; i < text.Length; i++ ) {
				char c = text[ i ];

				if ( escaped ) {
					_commandBuilder.Append( c );
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
							if ( _commandBuilder.Length > 0 ) {
								_arguments.Add( _commandBuilder.ToString() );
								_commandBuilder.Clear();
							}
							continue;
						}
						break;
				}

				_commandBuilder.Append( c );
			}

			if ( _commandBuilder.Length > 0 ) {
				_arguments.Add( _commandBuilder.ToString() );
			}
		}

		/*
		===============
		OnConsoleOpened
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnConsoleOpened( in EmptyEventArgs args ) {
			GrabFocus();
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
		private void OnConsoleClosed( in EmptyEventArgs args ) {
			Clear();
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
			PlaceholderText = String.Empty;

			_eventBus.ConnectSignal( this, LineEdit.SignalName.TextSubmitted, this, Callable.From<string>( OnTextEntered ) );
		}
	};
};