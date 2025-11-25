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
using NomadCore.Interfaces.EventSystem;
using NomadCore.Systems.ConsoleSystem.Interfaces;
using NomadCore.Systems.ConsoleSystem.Interfaces.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.ConsoleSystem.Infrastructure {
	/*
	===================================================================================
	
	InGameSink
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class InGameSink : SinkBase {
		private readonly RichTextLabel RichLabel;

		/*
		===============
		InGameSink
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="builder"></param>
		/// <param name="events"></param>
		public InGameSink( Node? node, ICommandBuilder? builder, IConsoleEvents? events ) {
			ArgumentNullException.ThrowIfNull( node );
			ArgumentNullException.ThrowIfNull( builder );
			ArgumentNullException.ThrowIfNull( events );

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

			builder.TextEntered.Subscribe( this, OnScrollToBottom );
			events.ConsoleOpened.Subscribe( this, OnScrollToBottom );
			events.PageUp.Subscribe( this, OnPageUp );
			events.PageDown.Subscribe( this, OnPageDown );
		}

		/*
		===============
		Print
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Print( string message ) {
			RichLabel.CallDeferred( RichTextLabel.MethodName.AppendText, message );
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Clear() {
			RichLabel.CallDeferred( RichTextLabel.MethodName.Clear );
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Flush() {
		}

		/*
		===============
		OnScrollToBottom
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		private void OnScrollToBottom( in IGameEvent eventData, in IEventArgs args ) {
			VScrollBar scroll = RichLabel.GetVScrollBar();
			scroll.Value = scroll.MaxValue - scroll.Page;
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
			tween.TweenProperty( scroll, "value", scroll.Value - ( scroll.Page - scroll.Page * 0.1f ), 0.1f );
			RichLabel.GetViewport().SetInputAsHandled();
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
			tween.TweenProperty( scroll, "value", scroll.Value + ( scroll.Page - scroll.Page * 0.1f ), 0.1f );
			RichLabel.GetViewport().SetInputAsHandled();
		}
	};
};