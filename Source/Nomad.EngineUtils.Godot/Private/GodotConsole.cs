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

using System;
using Godot;
using Nomad.Core.Console;
using Nomad.Core.Engine.Services;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Nomad.EngineUtils.Godot.Private {
	/*
	===================================================================================

	GodotConsole

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class GodotConsole : IConsoleObject {
		public ICommandBuilder CommandBuilder => _commandBuilder;
		private readonly GodotCommandBuilder _commandBuilder;

		private readonly ILoggerService _logger;

		private readonly CanvasLayer _node;
		private readonly InGameSink _godotSink;

		private readonly ISubscriptionHandle _consoleOpened;
		private readonly ISubscriptionHandle _consoleClosed;

		private bool _isDisposed = false;

		/*
		===============
		GodotConsole
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="node"></param>
		/// <param name="logger"></param>
		/// <param name="eventFactory"></param>
		public GodotConsole( Node node, ILoggerService logger, IGameEventRegistryService eventFactory ) {
			_logger = logger;

			_commandBuilder = new GodotCommandBuilder( eventFactory );

			_consoleOpened = eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.Console.CONSOLE_OPENED_EVENT, Core.Constants.Events.Console.NAMESPACE ).Subscribe( OnConsoleOpened );
			_consoleClosed = eventFactory.GetEvent<EmptyEventArgs>( Core.Constants.Events.Console.CONSOLE_CLOSED_EVENT, Core.Constants.Events.Console.NAMESPACE ).Subscribe( OnConsoleClosed );

			_node = new CanvasLayer() { Visible = false, ProcessMode = Node.ProcessModeEnum.Always };
			_godotSink = new InGameSink( _node, _commandBuilder, node, eventFactory );
			_logger.AddSink( _godotSink );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			if ( !_isDisposed ) {
				_commandBuilder?.Dispose();
				_godotSink?.Dispose();
				_node?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Clear() {
			_godotSink.Clear();
		}

		/*
		===============
		PrintString
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		public static void PrintString( in string message ) {
		}

		/*
		===============
		Show
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Show() {
			_node.Show();
		}

		/*
		===============
		Hide
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Hide() {
			_node.Hide();
		}

		/*
		===============
		OnConsoleClosed
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnConsoleClosed( in EmptyEventArgs args ) {
			Hide();
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
			Show();
		}
	};
};
