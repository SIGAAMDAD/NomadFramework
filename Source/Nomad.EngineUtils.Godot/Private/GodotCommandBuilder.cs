/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Godot;
using Nomad.Core.Console;
using Nomad.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nomad.EngineUtils.Godot.Private {
	/*
	===================================================================================

	GodotCommandBuilder

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed partial class GodotCommandBuilder : LineEdit, ICommandBuilder {
		/// <summary>
		/// The number of arguments currently available from the command line.
		/// </summary>
		public int ArgumentCount => _arguments.Count;

		private readonly List<string> _arguments = new List<string>();
		private readonly StringBuilder _commandBuilder = new StringBuilder( 1024 );
		
		// it ain't pretty but it works.
		private bool _isOpen = false;

		private readonly IGameEventRegistryService _eventFactory;

		/// <summary>
		/// 
		/// </summary>
		public IGameEvent<TextEnteredEventArgs> TextEntered => _textEntered;
		private readonly IGameEvent<TextEnteredEventArgs> _textEntered;

		private bool _isDisposed = false;

		/*
		===============
		GodotCommandBuilder
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="eventFactory"></param>
		public GodotCommandBuilder( IGameEventRegistryService eventFactory ) {
			ArgumentNullException.ThrowIfNull( eventFactory );

			_textEntered = eventFactory.GetEvent<TextEnteredEventArgs>( Core.Constants.Events.Console.TEXT_ENTERED_EVENT, Core.Constants.Events.Console.NAMESPACE );

			_eventFactory = eventFactory;
			eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.Console.CONSOLE_OPENED_EVENT, Core.Constants.Events.Console.NAMESPACE );
			eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.Console.CONSOLE_CLOSED_EVENT, Core.Constants.Events.Console.NAMESPACE );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if ( !_isDisposed ) {
				_textEntered?.Dispose();
			}
			base.Dispose( disposing );
			_isDisposed = true;
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
		public string GetArgumentAt( int index ) {
			return _arguments[index];
		}

		/*
		===============
		GetArgs
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string[] GetArgs() {
			return [.. _arguments.Skip( 1 )];
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newText"></param>
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
			_commandBuilder.Clear();

			for ( int i = 0; i < text.Length; i++ ) {
				char c = text[i];

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
		private void OnConsoleOpened() {
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
		private void OnConsoleClosed() {
			Clear();
		}

		/*
		===============
		_Ready
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public override void _Ready() {
			base._Ready();

			Name = nameof( GodotCommandBuilder );
			AnchorTop = 0.5f;
			AnchorRight = 1.0f;
			AnchorBottom = 0.5f;
			PlaceholderText = string.Empty;

			Connect( LineEdit.SignalName.TextSubmitted, Callable.From<string>( OnTextEntered ) );
		}

		/*
		===============
		_UnhandledInput
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="event"></param>
		public override void _UnhandledInput( InputEvent @event ) {
			base._UnhandledInput( @event );

			if ( @event is InputEventKey key && key.GetPhysicalKeycodeWithModifiers() == Key.Quoteleft && key.Pressed ) {
				if ( !_isOpen ) {
					_eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.Console.CONSOLE_OPENED_EVENT, Core.Constants.Events.Console.NAMESPACE ).Publish( default );
					OnConsoleOpened();
				} else {
					_eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.Console.CONSOLE_CLOSED_EVENT, Core.Constants.Events.Console.NAMESPACE ).Publish( default );
					OnConsoleClosed();
				}
				_isOpen = !_isOpen;
			}
		}
	};
};
