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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Nomad.Console.Interfaces;
using Nomad.Core.Util;
using Nomad.Console.Events;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Core;

namespace Nomad.Console.Private.Services {
	/*
	===================================================================================
	
	History
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class History : IHistory {
		private const string CONSOLE_HISTORY_FILE = "user://history.txt";
		private const int CONSOLE_HISTORY_MAX = 32;

		private readonly Queue<string> _consoleHistory = new Queue<string>();
		private readonly FilePath _historyPath;
		private int _historyIndex = 0;

		public IGameEvent<HistoryPrevEventArgs> HistoryPrev => _historyPrev;
		private readonly IGameEvent<HistoryPrevEventArgs> _historyPrev;

		public IGameEvent<HistoryNextEventArgs> HistoryNext => _historyNext;
		private readonly IGameEvent<HistoryNextEventArgs> _historyNext;

		private readonly ILoggerService _logger;

		/*
		===============
		History
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="logger"></param>
		/// <param name="eventFactory"></param>
		public History( ICommandBuilder builder, ILoggerService logger, IGameEventRegistryService eventFactory ) {
			ArgumentNullException.ThrowIfNull( builder );
			ArgumentNullException.ThrowIfNull( eventFactory );
			ArgumentNullException.ThrowIfNull( logger );

			_logger = logger;

			builder.TextEntered.Subscribe( this, OnTextEntered );

			eventFactory.GetEvent<EmptyEventArgs>( Constants.Events.Console.HISTORY_PREV_EVENT ).Subscribe( this, OnHistoryPrev );
			eventFactory.GetEvent<EmptyEventArgs>( Constants.Events.Console.HISTORY_NEXT_EVENT ).Subscribe( this, OnHistoryNext );

			_historyPrev = eventFactory.GetEvent<HistoryPrevEventArgs>( Constants.Events.Console.HISTORY_PREV_EVENT );
			_historyNext = eventFactory.GetEvent<HistoryNextEventArgs>( Constants.Events.Console.HISTORY_NEXT_EVENT );

			_historyPath = new FilePath( CONSOLE_HISTORY_FILE, PathType.User );

			LoadHistory();
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		/// Clears the console history buffer and deletes the history file.
		/// </summary>
		public void Clear() {
			_consoleHistory.Clear();
			_historyIndex = 0;
			File.Delete( _historyPath.OSPath );
		}

		/*
		===============
		OnTextEntered
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void OnTextEntered( in TextEnteredEventArgs args ) {
			AddInputHistory( args.Text );
			SaveHistory();
		}

		/*
		===============
		SaveHistory
		===============
		*/
		/// <summary>
		/// Saves the console history list to <see cref="CONSOLE_HISTORY_FILE"/>.
		/// </summary>
		private void SaveHistory() {
			_logger?.PrintLine( "History.SaveHistory: writing command line history to disk..." );
			try {
				using StreamWriter writer = new StreamWriter( _historyPath.OSPath );
				foreach ( var history in _consoleHistory ) {
					writer.WriteLine( history );
				}
			} catch ( Exception e ) {
				_logger?.PrintError( $"History.SaveHistory: couldn't write console command history data to file '{_historyPath}'! Exception: {e}" );
				throw;
			}
		}

		/*
		===============
		OnHistoryPrev
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		private void OnHistoryPrev( in EmptyEventArgs args ) {
			if ( _historyIndex > 0 ) {
				_historyIndex--;
				if ( _historyIndex >= 0 ) {
					HistoryPrev.Publish( new HistoryPrevEventArgs( _consoleHistory.ElementAt( _historyIndex ) ) );
				}
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
		private void OnHistoryNext( in EmptyEventArgs args ) {
			if ( _historyIndex < _consoleHistory.Count ) {
				_historyIndex++;
				HistoryNext.Publish( new HistoryNextEventArgs( _historyIndex < _consoleHistory.Count, _consoleHistory.ElementAt( _historyIndex ) ) );
			}
		}

		/*
		===============
		LoadHistory
		===============
		*/
		private void LoadHistory() {
			try {
				using StreamReader reader = new StreamReader( FilePath.FromUserPath( CONSOLE_HISTORY_FILE ).OSPath );

				string? text;
				while ( ( text = reader.ReadLine() ) != null ) {
					AddInputHistory( text );
				}
			} catch ( Exception ) {
				throw;
			}
		}

		/*
		===============
		AddInputHistory
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		private void AddInputHistory( string text ) {
			if ( _consoleHistory.Count >= CONSOLE_HISTORY_MAX ) {
				_consoleHistory.Dequeue();
			}
			_consoleHistory.Enqueue( text );
			_historyIndex = _consoleHistory.Count;
		}
	};
};