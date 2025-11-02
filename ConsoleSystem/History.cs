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
using System.Runtime.InteropServices;

namespace ConsoleSystem {
	/*
	===================================================================================
	
	History
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class History {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct HistoryNextEventData : IEventArgs {
			public readonly bool EndReached;
			public readonly string Text;
			public HistoryNextEventData( bool endReached, string? text ) {
				ArgumentNullException.ThrowIfNull( text );

				EndReached = endReached;
				Text = text;
			}
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct HistoryPrevEventData : IEventArgs {
			public readonly string Text;
			public HistoryPrevEventData( string? text ) {
				ArgumentException.ThrowIfNullOrEmpty( text );

				Text = text;
			}
		};

		private static readonly string CONSOLE_HISTORY_FILE = "user://history.txt";
		private const int CONSOLE_HISTORY_MAX = 32;

		private readonly Queue<string> ConsoleHistory = new Queue<string>();
		private readonly string HistoryPath;
		private int HistoryIndex = 0;

		public readonly IGameEvent HistoryPrev;
		public readonly IGameEvent HistoryNext;

		/*
		===============
		History
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="commandLine"></param>
		/// <param name="historyPrev"></param>
		/// <param name="historyNext"></param>
		public History( CommandLine? commandLine, IGameEventBusService eventBus, IGameEvent? historyPrev, IGameEvent? historyNext ) {
			ArgumentNullException.ThrowIfNull( commandLine );
			ArgumentNullException.ThrowIfNull( historyPrev );
			ArgumentNullException.ThrowIfNull( historyNext );

			commandLine.TextEntered.Subscribe( this, OnTextEntered );
			commandLine.CommandExecuted.Subscribe( this, OnSaveHistory );

			historyPrev.Subscribe( this, OnHistoryPrev );
			historyNext.Subscribe( this, OnHistoryNext );

			HistoryPrev = eventBus.CreateEvent( nameof( HistoryPrev ) );
			HistoryNext = eventBus.CreateEvent( nameof( HistoryNext ) );

			HistoryPath = ProjectSettings.GlobalizePath( CONSOLE_HISTORY_FILE );

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
			ConsoleHistory.Clear();
			HistoryIndex = 0;
			System.IO.File.Delete( HistoryPath );
		}

		/*
		===============
		OnSaveHistory
		===============
		*/
		/// <summary>
		/// Saves the console history list to <see cref="CONSOLE_HISTORY_FILE"/>.
		/// </summary>
		private void OnSaveHistory( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.CommandExecutedEventData commandExecuted ) {
				try {
					using System.IO.StreamWriter writer = new System.IO.StreamWriter( HistoryPath );
					foreach ( var history in ConsoleHistory ) {
						writer.WriteLine( history );
					}
				} catch ( Exception e ) {
					Console.PrintError( $"History.OnSaveHistory: couldn't write console command history data to file '{HistoryPath}'! Exception: {e}" );
				}
			} else {
				throw new InvalidCastException( nameof( args ) );
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
		private void OnHistoryPrev( in IGameEvent eventData, in IEventArgs args ) {
			if ( HistoryIndex > 0 ) {
				HistoryIndex--;
				if ( HistoryIndex >= 0 ) {
					HistoryPrev.Publish( new HistoryPrevEventData( ConsoleHistory.ElementAt( HistoryIndex ) ) );
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
		private void OnHistoryNext( in IGameEvent eventData, in IEventArgs args ) {
			if ( HistoryIndex < ConsoleHistory.Count ) {
				HistoryIndex++;
				HistoryNext.Publish( new HistoryNextEventData( HistoryIndex < ConsoleHistory.Count, ConsoleHistory.ElementAt( HistoryIndex ) ) );
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
		/// <param name="eventData"></param>
		/// <param name="args"></param>
		/// <exception cref="InvalidCastException"></exception>
		private void OnTextEntered( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is CommandLine.TextEnteredEventData textEntered ) {
				AddInputHistory( textEntered.Text );
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		LoadHistory
		===============
		*/
		private void LoadHistory() {
			try {
				using System.IO.StreamReader reader = new System.IO.StreamReader( CONSOLE_HISTORY_FILE );

				string? text;
				while ( ( text = reader.ReadLine() ) != null ) {
					AddInputHistory( text );
				}
			} catch ( Exception ) {
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
			if ( ConsoleHistory.Count >= CONSOLE_HISTORY_MAX ) {
				ConsoleHistory.Dequeue();
			}
			ConsoleHistory.Enqueue( text );
			HistoryIndex = ConsoleHistory.Count;
		}
	};
};