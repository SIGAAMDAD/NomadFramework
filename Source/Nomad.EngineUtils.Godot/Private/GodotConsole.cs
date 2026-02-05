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
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================
	
	GodotConsole
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class GodotConsole : IConsoleObject {
		private readonly ILoggerService _logger;

		private readonly CanvasLayer _node;
		private readonly InGameSink _godotSink;

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

			_node = new CanvasLayer();
			_godotSink = new InGameSink( _node, node, eventFactory );
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
			_godotSink.Dispose();
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
		public void PrintString( in string message ) {
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
	};
};