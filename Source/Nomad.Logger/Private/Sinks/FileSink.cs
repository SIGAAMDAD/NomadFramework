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

using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.CVars;
using System;
using System.Runtime.CompilerServices;

namespace Nomad.Logger.Private.Sinks {
	/*
	===================================================================================

	FileSink

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	public sealed class FileSink : SinkBase {
		private readonly IFileWriteStream? _writer = null;

		/*
		===============
		FileSink
		===============
		*/
		public FileSink( ICVarSystemService cvarSystem, IFileSystem fileSystem ) {
			ICVar<string> logfile = cvarSystem.Register(
				new CVarCreateInfo<string>(
					name: "console.LogFile",
					defaultValue: "debug.log",
					description: "The path to the console's logging file.",
					flags: CVarFlags.Archive | CVarFlags.Developer,
					validator: file => file.Length > 0
				)
			);

			try {
				_writer = fileSystem.OpenWrite( $"{fileSystem.GetUserDataPath()}/{logfile.Value}" ) as IFileWriteStream;
			} catch ( Exception e ) {
				_writer?.Close();
				Console.WriteLine( $"FileSink: failed to create log file {logfile.Value} - {e.Message}" );
				throw;
			}
			Console.WriteLine( $"Sucessfully opened logfile at {DateTime.Now}." );
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
		public override void Print( in string message ) {
			_writer?.WriteLineAsync( message );
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
//			_writer.Length = 0;
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
