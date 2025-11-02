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

	NOTE: it is a programming error if this is called without explicit ArchiveSystem SaveGame()
	setup or called in a multithreaded context. SO DO NOT MULTITHREAD IT!

	the default stream is ArchiveSystem.SaveWriter, but for the sake of testing, it uses
	dependency injection in the contructor
	
	===================================================================================
	*/
	/// <summary>
	/// Handles writing of savefile "sections" into a binary format
	/// </summary>
	/// <remarks>
	/// Should ONLY be used in a dedicated Save() function.
	/// Exceptions are caught in ArchiveSystem.SaveGame
	/// </remarks>
	/// <seealso cref="ArchiveSystem"/>
	/// <seealso cref="SaveSectionReader"/>

	public sealed class SaveSectionWriter : IDisposable {
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
		public SaveSectionWriter( string? name, Dictionary<string, object>? fields, Streams.SaveWriterStream? writer ) {
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
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <exception cref="InvalidCastException"></exception>
		public void Save( string? name, object? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( value );

			if ( value is sbyte int8 ) {
				SaveSByte( name, int8 );
			} else if ( value is short int16 ) {
				SaveShort( name, int16 );
			} else if ( value is int int32 ) {
				SaveInt( name, int32 );
			} else if ( value is long int64 ) {
				SaveLong( name, int64 );
			} else if ( value is byte uint8 ) {
				SaveByte( name, uint8 );
			} else if ( value is ushort uint16 ) {
				SaveUShort( name, uint16 );
			} else if ( value is uint uint32 ) {
				SaveUInt( name, uint32 );
			} else if ( value is ulong uint64 ) {
				SaveULong( name, uint64 );
			} else if ( value is string str ) {
				SaveString( name, str );
			} else if ( value is Vector2 vec2 ) {
				SaveVector2( name, vec2 );
			} else if ( value is Vector2I vec2i ) {
				SaveVector2I( name, vec2i );
			} else {
				throw new InvalidCastException( nameof( value ) );
			}
		}

		/*
		===============
		SaveSByte
		===============
		*/
		/// <summary>
		/// Writes an sbyte (int8) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveSByte( string? name, sbyte value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Int8, value );
		}

		/*
		===============
		SaveShort
		===============
		*/
		/// <summary>
		/// Writes a short (int16) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveShort( string? name, short value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Int16, value );
		}

		/*
		===============
		SaveInt
		===============
		*/
		/// <summary>
		/// Writes an int (int32) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveInt( string? name, int value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Int32, value );
		}

		/*
		===============
		SaveLong
		===============
		*/
		/// <summary>
		/// Writes a long (int64) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveLong( string? name, long value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Int64, value );
		}

		/*
		===============
		SaveByte
		===============
		*/
		/// <summary>
		/// Writes a byte (uint8) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveByte( string? name, byte value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.UInt8, value );
		}

		/*
		===============
		SaveUShort
		===============
		*/
		/// <summary>
		/// Writes a ushort (uint16) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveUShort( string? name, ushort value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.UInt16, value );
		}

		/*
		===============
		SaveUInt
		===============
		*/
		/// <summary>
		/// Writes a uint (uint32) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveUInt( string? name, uint value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.UInt32, value );
		}

		/*
		===============
		SaveULong
		===============
		*/
		/// <summary>
		/// Writes a ulong (uint64) value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveULong( string? name, ulong value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.UInt64, value );
		}

		/*
		===============
		SaveFloat
		===============
		*/
		/// <summary>
		/// Writes a float value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveFloat( string? name, float value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Float, value );
		}

		/*
		===============
		SaveDouble
		===============
		*/
		/// <summary>
		/// Writes a double value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveDouble( string? name, double value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Double, value );
		}

		/*
		===============
		SaveString
		===============
		*/
		/// <summary>
		/// Writes a string value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveString( string? name, string? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( value );

			WriteFieldHeader( name, FieldType.String );
			Writer.Write( value );
		}

		/*
		===============
		SaveBool
		===============
		*/
		/// <summary>
		/// Writes a boolean value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SaveBool( string? name, bool value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			WritePrimitive( name, FieldType.Boolean, value );
		}

		/*
		===============
		SaveVector2
		===============
		*/
		/// <summary>
		/// Writes a Godot.Vector2 value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveVector2( string? name, Vector2 value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );

			WritePrimitive( name, FieldType.Vector2, value );
		}

		/*
		===============
		SaveVector2I
		===============
		*/
		/// <summary>
		/// Writes a Godot.Vector2I value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveVector2I( string? name, Vector2I value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );

			WriteFieldHeader( name, FieldType.Vector2I );
			SaveVector2I( in value );

			FieldCount++;
		}

		/*
		===============
		SaveArray
		===============
		*/
		/// <summary>
		/// Writes a int[] value to the current savefile section
		/// </summary>
		/// <remarks>
		/// Calls <see cref="SaveIntArray"/> internally.
		/// </remarks>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveArray( string? name, in IReadOnlyList<int>? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );

			WriteFieldHeader( name, FieldType.IntList );
			SaveIntArray( in value );
		}

		/*
		===============
		SaveArray
		===============
		*/
		/// <summary>
		/// Writes a uint[] value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveArray( string? name, in IReadOnlyList<uint>? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );

			WriteFieldHeader( name, FieldType.UIntList );
			SaveUIntArray( value );

			FieldCount++;
		}

		/*
		===============
		SaveArray
		===============
		*/
		/// <summary>
		/// Writes a float[] value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveArray( string? name, in IReadOnlyList<float>? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );


			WriteFieldHeader( name, FieldType.FloatList );
			SaveFloatArray( in value );

			FieldCount++;
		}

		/*
		===============
		SaveArray
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SaveArray( string? name, in IReadOnlyList<string>? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );

			WriteFieldHeader( name, FieldType.StringList );
			SaveStringArray( in value );

			FieldCount++;
		}

		/*
		===============
		SaveArray
		===============
		*/
		/// <summary>
		/// Writes a Godot.Collections.Array value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveArray( string? name, Godot.Collections.Array? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );

			WriteFieldHeader( name, FieldType.Array );
			Writer.Write( value.Count );
			for ( int i = 0; i < value.Count; i++ ) {
				WriteVariant( value[ i ] );
			}
			FieldCount++;
		}

		/*
		===============
		SaveDictionary
		===============
		*/
		/// <summary>
		/// Writes a Godot.Collections.Dictionary value to the current savefile section
		/// </summary>
		/// <param name="name">The unique identifier for this field</param>
		/// <param name="value">The value to be saved</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		/// <exception cref="InvalidOperationException">Thrown if called outside of a save operation</exception>
		public void SaveDictionary( string? name, Godot.Collections.Dictionary? value ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( value );

			WriteFieldHeader( name, FieldType.Dictionary );
			Writer.Write( value.Count );
			foreach ( var it in value ) {
				WriteVariant( it.Key );
				WriteVariant( it.Value );
			}

			FieldCount++;
		}

		/*
		===============
		SaveIntArray
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveIntArray( in IReadOnlyList<int> value ) {
			Writer.Write( value.Count );
			for ( int i = 0; i < value.Count; i++ ) {
				Writer.Write( value[ i ] );
			}
		}

		/*
		===============
		SaveUIntArray
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveUIntArray( in IReadOnlyList<uint> value ) {
			Writer.Write( value.Count );
			for ( int i = 0; i < value.Count; i++ ) {
				Writer.Write( value[ i ] );
			}
		}

		/*
		===============
		SaveFloatArray
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveFloatArray( in IReadOnlyList<float> value ) {
			Writer.Write( value.Count );
			for ( int i = 0; i < value.Count; i++ ) {
				Writer.Write( value[ i ] );
			}
		}

		/*
		===============
		SaveStringArray
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveStringArray( in IReadOnlyList<string> value ) {
			Writer.Write( value.Count );
			for ( int i = 0; i < value.Count; i++ ) {
				Writer.Write( value[ i ] );
			}
		}

		/*
		===============
		WriteFieldHeader
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void WriteFieldHeader( string name, FieldType type ) {
			Writer.Write( name );
			Writer.Write( (byte)type );
		}

		/*
		===============
		WritePrimitiveField
		===============
		*/
		/// <summary>
		/// Writes a primitive save field to the binary stream.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private unsafe void WritePrimitive<T>( string name, FieldType type, T value ) where T : unmanaged {
			WriteFieldHeader( name, type );
			Writer.Write( MemoryMarshal.AsBytes( MemoryMarshal.CreateReadOnlySpan( ref value, Marshal.SizeOf( typeof( T ) ) ) ) );
			FieldCount++;
		}

		/*
		===============
		SaveVector2
		===============
		*/
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveVector2( in Vector2 value ) {
			Writer.Write( value.X );
			Writer.Write( value.Y );
		}

		/*
		===============
		SaveVector2I
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveVector2I( in Vector2I value ) {
			Writer.Write( value.X );
			Writer.Write( value.Y );
		}

		/*
		===============
		SaveColor
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void SaveColor( in Color color ) {
			Writer.Write( color.R8 );
			Writer.Write( color.G8 );
			Writer.Write( color.B8 );
			Writer.Write( color.A8 );
		}

		/*
		===============
		WriteVariant
		===============
		*/
		/// <summary>
		/// Writes a Godot variant's type and value to the current savefile section.
		/// </summary>
		/// <remarks>
		/// Should only be called to from SaveArray or SaveDictionary, strictly for archiving Godot .NET collections
		/// If you want to archive a primitive, use the provided functions like SaveInt.
		/// this function only supports the following variant types: Boolean, Int, Float, String, StringName, NodePath,
		/// Vector2, Vector2I, Color, PackedColorArray, PackedByteArray, PackedInt32Array, PackedInt64Array, PackedFloat32Array
		/// PackedVector2Array, PackedStringArray
		/// </remarks>
		/// <param name="value">The Variant object</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported VariantType is being saved</exception>
		private void WriteVariant( Variant? value ) {
			ArgumentNullException.ThrowIfNull( Writer );
			if ( !value.HasValue ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			Writer.Write( (uint)value.Value.VariantType );
			switch ( value.Value.VariantType ) {
				case Variant.Type.Bool:
					Writer.Write( value.Value.AsBool() );
					break;
				case Variant.Type.Int:
					Writer.Write( value.Value.AsInt32() );
					break;
				case Variant.Type.Float:
					Writer.Write( value.Value.AsSingle() );
					break;
				case Variant.Type.String:
					Writer.Write( value.Value.AsString() );
					break;
				case Variant.Type.StringName:
					Writer.Write( value.Value.AsStringName() );
					break;
				case Variant.Type.NodePath:
					Writer.Write( value.Value.AsNodePath() );
					break;
				case Variant.Type.Color:
					SaveColor( value.Value.AsColor() );
					break;
				case Variant.Type.Vector2:
					SaveVector2( value.Value.AsVector2() );
					break;
				case Variant.Type.Vector2I:
					SaveVector2I( value.Value.AsVector2I() );
					break;
				case Variant.Type.PackedColorArray: {
						Color[] arr = value.Value.AsColorArray();
						Writer.Write( arr.Length );
						for ( int i = 0; i < arr.Length; i++ ) {
							SaveColor( arr[ i ] );
						}
						break;
					}
				case Variant.Type.PackedInt32Array:
					SaveIntArray( value.Value.AsInt32Array() );
					break;
				case Variant.Type.PackedInt64Array: {
						long[] arr = value.Value.AsInt64Array();
						Writer.Write( arr.Length );
						for ( int i = 0; i < arr.Length; i++ ) {
							Writer.Write( arr[ i ] );
						}
						break;
					}
				case Variant.Type.PackedFloat32Array:
					SaveFloatArray( value.Value.AsFloat32Array() );
					break;
				case Variant.Type.PackedStringArray:
					SaveStringArray( value.Value.AsStringArray() );
					break;
				case Variant.Type.PackedVector2Array: {
						Vector2[] arr = value.Value.AsVector2Array();
						Writer.Write( arr.Length );
						for ( int i = 0; i < arr.Length; i++ ) {
							SaveVector2( in arr[ i ] );
						}
						break;
					}
				case Variant.Type.Array:
					SaveArrayInternal( value.Value.AsGodotArray() );
					break;
				case Variant.Type.Dictionary:
					SaveDictionaryInternal( value.Value.AsGodotDictionary() );
					break;
				default:
					throw new ArgumentOutOfRangeException( $"invalid Godot.VariantType: {value.Value.VariantType}" );
			}
		}

		/*
		===============
		SaveDictionaryInternal
		===============
		*/
		/// <summary>
		/// SaveDictionary, but its for recursive dictionaries found in the base dictionary
		/// given to the section writer.
		/// </summary>
		/// <remarks>
		/// Shouldn't be called outside of this class
		/// </remarks>
		/// <param name="value"></param>
		/// <param name="depth"></param>
		/// <exception cref="InvalidOperationException">Thrown if the depth of a dictionary reference exceeds MAX_ARCHIVE_DEPTH</exception>
		private void SaveDictionaryInternal( Godot.Collections.Dictionary? value, int depth = 0 ) {
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );

			if ( depth++ > MAX_ARCHIVE_DEPTH ) {
				throw new InvalidOperationException( $"Dictionary depth exceeded maximum depth of {MAX_ARCHIVE_DEPTH}" );
			}

			Writer.Write( value.Count );
			foreach ( var it in value ) {
				WriteVariant( it.Key );
				WriteVariant( it.Value );
			}
		}

		/*
		===============
		SaveArrayInternal
		===============
		*/
		/// <summary>
		/// SaveArray, but its for recursive arrays found in the base array
		/// given to the section writer.
		/// </summary>
		/// <remarks>
		/// Shouldn't be called outside of this class
		/// </remarks>
		/// <param name="value"></param>
		/// <param name="depth"></param>
		/// <exception cref="InvalidOperationException">Thrown if the depth of a array reference exceeds MAX_ARCHIVE_DEPTH</exception>
		private void SaveArrayInternal( Godot.Collections.Array? value, int depth = 0 ) {
			ArgumentNullException.ThrowIfNull( Writer );
			ArgumentNullException.ThrowIfNull( value );
			ArgumentOutOfRangeException.ThrowIfLessThan( depth, 0 );

			if ( depth++ > MAX_ARCHIVE_DEPTH ) {
				throw new InvalidOperationException( $"Array depth exceeded maximum depth of {MAX_ARCHIVE_DEPTH}" );
			}

			Writer.Write( value.Count );
			for ( int i = 0; i < value.Count; i++ ) {
				WriteVariant( value[ i ] );
			}
		}
	};
};