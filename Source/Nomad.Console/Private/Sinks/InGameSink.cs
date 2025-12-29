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
using System;
using System.Runtime.CompilerServices;

namespace Nomad.Console.Private.Sinks {
	/*
	===================================================================================
	
	InGameSink
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class InGameSink : SinkBase {
		private readonly RichTextLabel _richLabel;

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
		/// <param name="eventRegistry"></param>
		public InGameSink( Node? node, ICommandBuilder? builder, IGameEventRegistryService eventRegistry ) {
			ArgumentNullException.ThrowIfNull( node );
			ArgumentNullException.ThrowIfNull( builder );
			ArgumentNullException.ThrowIfNull( eventRegistry );

			_richLabel = new RichTextLabel() {
				Name = nameof( _richLabel ),
				SelectionEnabled = true,
				ContextMenuEnabled = true,
				BbcodeEnabled = true,
				ScrollFollowing = true,
				AnchorRight = 1.0f,
				AnchorBottom = 0.5f,
			};
			_richLabel.CallDeferred( RichTextLabel.MethodName.AddThemeStyleboxOverride, "normal", new StyleBoxFlat() { BgColor = new Color( 0.0f, 0.0f, 0.0f, 0.84f ) } );
			node.CallDeferred( Control.MethodName.AddChild, _richLabel );

			builder.TextEntered.Subscribe( this, OnScrollToBottom );
			eventRegistry.GetEvent<EmptyEventArgs>( StringPool.Intern( "ConsoleOpened" ) ).Subscribe( this, OnConsoleOpened );
			eventRegistry.GetEvent<EmptyEventArgs>( StringPool.Intern( "PageUp" ) ).Subscribe( this, OnPageUp );
			eventRegistry.GetEvent<EmptyEventArgs>( StringPool.Intern( "PageDown" ) ).Subscribe( this, OnPageDown );
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
			_richLabel.CallDeferred( RichTextLabel.MethodName.AppendText, message );
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
			_richLabel.CallDeferred( RichTextLabel.MethodName.Clear );
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
		private void OnScrollToBottom( in TextEnteredEventData args ) {
			VScrollBar scroll = _richLabel.GetVScrollBar();
			scroll.Value = scroll.MaxValue - scroll.Page;
		}

		/*
		===============
		OnConsoleOpened
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnConsoleOpened( in EmptyEventArgs args ) {
			OnScrollToBottom( new TextEnteredEventData() );
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
		private void OnPageUp( in EmptyEventArgs args ) {
			VScrollBar scroll = _richLabel.GetVScrollBar();
			Tween tween = _richLabel.CreateTween();
			tween.TweenProperty( scroll, "value", scroll.Value - ( scroll.Page - scroll.Page * 0.1f ), 0.1f );
			_richLabel.GetViewport().SetInputAsHandled();
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
		private void OnPageDown( in EmptyEventArgs args ) {
			VScrollBar scroll = _richLabel.GetVScrollBar();
			Tween tween = _richLabel.CreateTween();
			tween.TweenProperty( scroll, "value", scroll.Value + ( scroll.Page - scroll.Page * 0.1f ), 0.1f );
			_richLabel.GetViewport().SetInputAsHandled();
		}
	};
};