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

namespace ConsoleSystem {
	/*
	===================================================================================
	
	Logger
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class Logger {
		public readonly RichTextLabel RichLabel;

		private readonly System.IO.StreamWriter? Stream;
		private readonly Control Node;

		/*
		===============
		Logger
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="commandLine"></param>
		/// <param name="filePath"></param>
		/// <param name="pageDown"></param>
		/// <param name="pageUp"></param>
		public Logger( Control? node, CommandLine? commandLine, string? filePath, IGameEvent pageDown, IGameEvent pageUp ) {
			ArgumentException.ThrowIfNullOrEmpty( filePath );
			ArgumentNullException.ThrowIfNull( commandLine );
			ArgumentNullException.ThrowIfNull( node );
			ArgumentNullException.ThrowIfNull( pageDown );
			ArgumentNullException.ThrowIfNull( pageUp );

			Node = node;

			try {
				Stream = new System.IO.StreamWriter( filePath, false );
			} catch ( Exception e ) {
				GD.PushWarning( $"Couldn't create logfile {filePath}! Exception: {e.Message}" );
			}

			RichLabel = new RichTextLabel() {
				Name = nameof( RichLabel ),
				SelectionEnabled = true,
				ContextMenuEnabled = true,
				BbcodeEnabled = true,
				ScrollFollowing = true,
				AnchorRight = 1.0f,
				AnchorBottom = 0.5f
			};
			RichLabel.CallDeferred( RichTextLabel.MethodName.AddThemeStyleboxOverride, "normal", new StyleBoxFlat() { BgColor = new Color( 0.0f, 0.0f, 0.0f, 0.84f ) } );
			node.CallDeferred( Control.MethodName.AddChild, RichLabel );

			pageDown.Subscribe( this, OnPageDown );
			pageUp.Subscribe( this, OnPageUp );

			commandLine.TextEntered.Subscribe( this, OnTextEntered );

			Console.ConsoleClosed.Subscribe( this, OnScrollToBottom );
		}

		/*
		===============
		OnPageDown
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnPageDown( in IGameEvent eventData, in IEventArgs args ) {
			VScrollBar scroll = RichLabel.GetVScrollBar();
			Tween tween = RichLabel.CreateTween();
			tween.TweenProperty( scroll, "value", scroll.Value - ( scroll.Page - scroll.Page * 0.1f ), 0.1f );
		}

		/*
		===============
		OnPageUp
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnPageUp( in IGameEvent eventData, in IEventArgs args ) {
			VScrollBar scroll = RichLabel.GetVScrollBar();
			Tween tween = RichLabel.CreateTween();
			tween.TweenProperty( scroll, "value", scroll.Value + ( scroll.Page - scroll.Page * 0.1f ), 0.1f );
		}

		/*
		===============
		PrintLine
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public void PrintLine( string? message ) {
			ArgumentException.ThrowIfNullOrEmpty( message );

			RichLabel.CallDeferred( RichTextLabel.MethodName.AppendText, $"{message}\n" );
			GD.PrintRich( message );
			Stream?.WriteLine( message );
		}

		public void PrintDebug( string? message ) {
			ArgumentException.ThrowIfNullOrEmpty( message );

			RichLabel.CallDeferred( RichTextLabel.MethodName.AppendText, $"[color=light_blue]DEBUG: {message}[/color]\n" );
			GD.PrintRich( $"[color=light_blue]DEBUG: {message}[/color]\n" );
			Stream?.WriteLine( $"DEBUG: {message}" );
		}

		/*
		===============
		PrintWarning
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public void PrintWarning( string? message ) {
			ArgumentException.ThrowIfNullOrEmpty( message );

			RichLabel.CallDeferred( RichTextLabel.MethodName.AppendText, $"[color=gold]WARNING: {message}[/color]\n" );
			GD.PushWarning( message );
			Stream?.WriteLine( $"WARNING: {message}" );
		}

		/*
		===============
		PrintError
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public void PrintError( string? message ) {
			ArgumentException.ThrowIfNullOrEmpty( message );

			RichLabel.CallDeferred( RichTextLabel.MethodName.AppendText, $"[color=red]ERROR: {message}[/color]\n" );
			GD.PushError( message );
			Stream?.WriteLine( $"ERROR: {message}" );
		}

		/*
		===============
		OnScrollToBottom
		===============
		*/
		private void OnScrollToBottom( in IGameEvent eventData, in IEventArgs args ) {
			VScrollBar scroll = RichLabel.GetVScrollBar();
			scroll.Value = scroll.MaxValue - scroll.Page;
		}

		/*
		===============
		OnTextEntered
		===============
		*/
		private void OnTextEntered( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.TextEnteredEventData textEntered ) {
				OnScrollToBottom( in eventData, in args );
				PrintLine( $"> {textEntered.Text}" );
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}
	};
};