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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveSectionWriter

	the dedicated class for writing dictionary style "sections" into the current savefile
	
	===================================================================================
	*/
	/// <summary>
	/// Handles writing of savefile "sections" into a binary format
	/// </summary>
	/// <remarks>
	/// Should ONLY be used in a dedicated Save() function.
	/// Exceptions are caught in ArchiveSystem.SaveGame
	/// </remarks>
	/// <seealso cref="SaveManager"/>
	/// <seealso cref="SaveSectionReader"/>

	internal ref struct SaveSectionWriter {
		/// <summary>
		/// Realistically someone shouldn't be writing more than 4 MB of contigious data to disk in a go
		/// </summary>
		private const int MAX_ARRAY_SIZE = 4 * 1024 * 1024;

		/// <summary>
		/// The maximum amount of recursion allowed while writing an array to disk. Meant to avoid stack overflows
		/// </summary>
		private const int MAX_ARCHIVE_DEPTH = 32;

		private readonly int BeginPosition = 0;
		private readonly Streams.SaveWriterStream Writer;

		private int FieldCount = 0;

		/*
		===============
		SaveSectionWriter
		===============
		*/
		/// <summary>
		/// constructs a SaveSectionWriter object
		/// </summary>
		/// <param name="name">name of the section</param>
		/// <param name="writer">the stream to write to, if null, ArchiveSystem.SaveWriter is used by default</param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public SaveSectionWriter( string? name, Streams.SaveWriterStream? writer ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( writer );

			Writer = writer;

			Writer.Write( name );
			BeginPosition = Writer.Position;
			Writer.Write( FieldCount );
		}

		/*
		===============
		SaveSectionWriter
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fields"></param>
		/// <param name="writer"></param>
		public SaveSectionWriter( string? name, in IReadOnlyList<SaveField>? fields, Streams.SaveWriterStream? writer ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( fields );
			ArgumentNullException.ThrowIfNull( writer );

			Writer = writer;

			Writer.Write( name );
			BeginPosition = Writer.Position;
			Writer.Write( FieldCount );

			foreach ( var field in fields ) {
				object value = field.Value;
			}
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
			// exceptions are already being caught in Flush()
			Flush();
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		/// finalizes a save section. Called automatically when the SaveSectionWriter is disposed.
		/// </summary>
		/// <remarks>
		/// There is very little reason to call this explicitly.
		/// </remarks>
		public void Flush() {
			if ( Writer == null ) {
				return;
			}
			int position = Writer.Position;

			Writer.Seek( BeginPosition, System.IO.SeekOrigin.Begin );
			Writer.Write( FieldCount );
			Writer.Seek( position, System.IO.SeekOrigin.Begin );
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void Save<T>( string? name, T value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			var serializer = FieldSerializers.FieldSerializerRegistry.GetSerializer<T>();
			WriteFieldHeader( name, serializer.FieldType );
			serializer.Serialize( Writer, value );
			FieldCount++;
		}

		/*
		===============
		WriteFieldHeader
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private readonly void WriteFieldHeader( string name, FieldType type ) {
			Writer.Write( name );
			Writer.Write( (byte)type );
		}
	};
};