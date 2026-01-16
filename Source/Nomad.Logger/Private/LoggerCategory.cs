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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Nomad.Core.Collections;
using Nomad.Core.Logger;

namespace Nomad.Logger.Private {
	/*
	===================================================================================

	LoggerCategory

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class LoggerCategory : ILoggerCategory {
		public string Name => _name;
		private readonly string _name;

		public LogLevel Level => _level;
		private readonly LogLevel _level;

		public bool Enabled {
			get => _enabled;
			set => _enabled = value;
		}
		private bool _enabled;

		private long _quitFlag = 0;

		private readonly List<ILoggerSink> _sinks = new List<ILoggerSink>();
		private readonly LockFreePooledQueue<string> _messageQueue = new LockFreePooledQueue<string>( 256 );

		/*
		===============
		LoggerCategory
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <param name="level"></param>
		/// <param name="enabled"></param>
		public LoggerCategory( string name, LogLevel level, bool enabled ) {
			_name = name;
			_level = level;
			_enabled = enabled;

			var printThread = new Thread( LoggerThreadAsync );
			printThread.IsBackground = true;
			printThread.Start();
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
			_sinks.Clear();
		}

		/*
		===============
		AddSink
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="sink"></param>
		public void AddSink( in ILoggerSink sink ) {
			_sinks.Add( sink );
		}

		/*
		===============
		QueueMessage
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		public void QueueMessage( in string message ) {
			_messageQueue.TryEnqueue( in message );
		}

		/*
		===============
		LoggerThreadAsync
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		private async void LoggerThreadAsync() {
			try {
				while ( true ) {
					while ( _messageQueue.TryDequeue( out var message ) ) {
						for ( int i = 0; i < _sinks.Count; i++ ) {
							_sinks[ i ].Print( in message );
						}
					}
					await Task.Delay( 500 );
				}
			} catch ( Exception e ) {
				GD.PrintErr( $"Exception caught: {e}" );
			}
		}
	};
};
