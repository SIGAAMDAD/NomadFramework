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
using Nomad.Core.Util;
using Nomad.CVars;
using NomadCore.Systems.ConsoleSystem.Interfaces.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace Nomad.Console.Sinks {
	/*
	===================================================================================
	
	FileSink
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class FileSink : SinkBase {
		private readonly System.IO.StreamWriter? _writer = null;

		/*
		===============
		FileSink
		===============
		*/
		public FileSink( ICVarSystemService cvarSystem ) {
			ICVar<string> logfile = cvarSystem.Register(
				new CVarCreateInfo<string>(
					Name: "console.LogFile",
					DefaultValue: "user://debug.log",
					Description: "The path to the console's logging file.",
					Flags: CVarFlags.Archive | CVarFlags.Developer,
					Validator: file => file.Length > 0
				)
			);

			try {
				using System.IO.FileStream stream = new System.IO.FileStream( FilePath.FromUserPath( logfile.Value ).OSPath, System.IO.FileMode.Create, System.IO.FileAccess.Write );
				_writer = new System.IO.StreamWriter( stream );
			} catch ( Exception e ) {
				_writer?.Close();
				GD.PrintErr( $"FileSink: failed to create log file {logfile.Value} - {e.Message}" );
				throw;
			}
			GD.Print( "Sucessfully opened logfile." );
		}

		/*
		===============
		Print
		===============
		*/
		/// <summary>
		/// Writes a string of characters to the file stream asynchronously.
		/// </summary>
		/// <param name="message"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Print( string message ) {
			_writer?.WriteAsync( message );
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		/// Clears the file stream.
		/// </summary>
		public override void Clear() {
			_writer?.BaseStream.SetLength( 0 );
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// Flushes the contents of the stream writer to the logfile.
		/// </summary>
		public override void Flush() {
			_writer?.Flush();
		}
	};
};