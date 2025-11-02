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

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveField
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public readonly struct SaveField {
		/// <summary>
		/// The field's type.
		/// </summary>
		public readonly FieldType Type { get; }

		/// <summary>
		/// 
		/// </summary>
		public readonly object? Value { get; } = null;

		/*
		===============
		SaveField
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader">The stream to read from</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if the loaded <see cref="FieldType"/> isn't valid</exception>
		public SaveField( Streams.SaveReaderStream reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			Type = (FieldType)reader.ReadUInt32();
			Value = Type switch {
				FieldType.Int8 => reader.ReadInt8(),
				FieldType.Int16 => reader.ReadInt16(),
				FieldType.Int32 => reader.ReadInt32(),
				FieldType.Int64 => reader.ReadInt64(),
				FieldType.UInt8 => reader.ReadUInt8(),
				FieldType.UInt16 => reader.ReadUInt16(),
				FieldType.UInt32 => reader.ReadUInt32(),
				FieldType.UInt64 => reader.ReadUInt64(),
				FieldType.Boolean => reader.ReadBoolean(),
				FieldType.Float => reader.ReadFloat(),
				FieldType.Double => reader.ReadDouble(),
				FieldType.Vector2 => FieldLoader.LoadVector2( reader ),
				FieldType.Vector2I => FieldLoader.LoadVector2I( reader ),
				FieldType.IntList => FieldLoader.LoadArray<int>( reader, value => reader.ReadInt32() ),
				FieldType.UIntList => FieldLoader.LoadArray<uint>( reader, value => reader.ReadUInt32() ),
				FieldType.FloatList => FieldLoader.LoadArray<float>( reader, value => reader.ReadFloat() ),
				FieldType.StringList => FieldLoader.LoadArray<string>( reader, value => reader.ReadString() ),
				FieldType.ByteArray => FieldLoader.LoadByteArray( reader ),
				FieldType.Array => FieldLoader.LoadGodotArray( reader ),
				FieldType.Dictionary => FieldLoader.LoadGodotDictionary( reader ),
				_ => throw new IndexOutOfRangeException( $"invalid FieldType in savefile - {Type}" )
			};
		}

		/*
		===============
		SaveField
		===============
		*/
		/// <summary>
		/// Only called when the field isn't found, so we provide a default value of 0/empty
		/// </summary>
		/// <param name="type">Type of the field</param>
		public SaveField( FieldType type ) {
			Type = type;
			Value = Type switch {
				FieldType.Int8 => (sbyte)0,
				FieldType.Int16 => (short)0,
				FieldType.Int32 => (int)0,
				FieldType.Int64 => (long)0,
				FieldType.UInt8 => (byte)0,
				FieldType.UInt16 => (ushort)0,
				FieldType.UInt32 => (uint)0,
				FieldType.UInt64 => (ulong)0,
				FieldType.Float => (float)0.0f,
				FieldType.Double => (double)0.0f,
				FieldType.String => "",
				FieldType.Vector2 => Vector2.Zero,
				FieldType.Vector2I => Vector2I.Zero,
				FieldType.Boolean => false,
				FieldType.IntList => Array.Empty<int>(),
				FieldType.FloatList => Array.Empty<float>(),
				FieldType.ByteArray => Array.Empty<byte>(),
				FieldType.Array => new Godot.Collections.Array(),
				FieldType.Dictionary => new Godot.Collections.Dictionary(),
				_ => throw new IndexOutOfRangeException( $"invalid FieldType in savefile - {Type}" )
			};
		}
	};
};