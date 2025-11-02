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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveSectionReader
	
	===================================================================================
	*/

	/// <summary>
	/// <para>The dedicated class for reading dictionary style "sections" froms the current savefile</para>
	/// 
	/// <para>NOTE: reading from loaded save section can be multithreaded since we are loading all values
	/// into RAM from disk before applying gamestate.</para>
	/// 
	/// <para>exceptions are caught in ArchiveSystem.LoadGame</para>
	/// 
	/// <para>the default stream is ArchiveSystem.SaveReader, but for the sake of testing, it uses
	/// dependency injection in the constructor</para>
	/// </summary>
	public readonly struct SaveSectionReader : IDisposable {
		private readonly Streams.SaveReaderStream Reader;
		private readonly ConcurrentDictionary<string, SaveField?> FieldList;
		private readonly IConsoleService Console;

		/*
		===============
		SaveSectionReader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="Exception"></exception>
		public SaveSectionReader( Streams.SaveReaderStream reader, IConsoleService? console ) {
			ArgumentNullException.ThrowIfNull( reader );
			ArgumentNullException.ThrowIfNull( console );

			Console = console;
			Reader = reader;

			int fieldCount = Reader.ReadInt32();
			FieldList = new ConcurrentDictionary<string, SaveField?>( 1, fieldCount );

			Console.PrintLine( $"...Got {fieldCount} fields." );

			for ( int i = 0; i < fieldCount; i++ ) {
				string name = Reader.ReadString();

				if ( name.Length <= 0 ) {
					throw new Exception( $"SaveField at offset {Reader.Position} has invalid name (length <= 0)" );
				}
				if ( !FieldList.TryAdd( name, new SaveField( Reader ) ) ) {
					throw new InvalidOperationException( $"A duplicate of SaveField {name} was found in savefile, aborting" );
				}
				Console.PrintLine( $"...loaded field \"{name}\"" );
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
			FieldList?.Clear();
		}

		/*
		===============
		LoadSByte
		===============
		*/
		/// <summary>
		/// Loads an sbyte from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly sbyte LoadSByte( string? name ) {
			return UnboxField<sbyte>( name, FieldType.Int8 );
		}

		/*
		===============
		LoadShort
		===============
		*/
		/// <summary>
		/// Loads a short from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly short LoadShort( string name ) {
			return UnboxField<short>( name, FieldType.Int16 );
		}

		/*
		===============
		LoadInt
		===============
		*/
		/// <summary>
		/// Loads an int from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly int LoadInt( string? name ) {
			return UnboxField<int>( name, FieldType.Int32 );
		}

		/*
		===============
		LoadLong
		===============
		*/
		/// <summary>
		/// Loads a long from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly long LoadLong( string? name ) {
			return UnboxField<long>( name, FieldType.Int64 );
		}

		/*
		===============
		LoadByte
		===============
		*/
		/// <summary>
		/// Loads a byte from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly byte LoadByte( string? name ) {
			return UnboxField<byte>( name, FieldType.UInt8 );
		}

		/*
		===============
		LoadUShort
		===============
		*/
		/// <summary>
		/// Loads an sbyte from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly ushort LoadUShort( string? name ) {
			return UnboxField<ushort>( name, FieldType.UInt16 );
		}

		/*
		===============
		LoadUInt
		===============
		*/
		/// <summary>
		/// Loads a uint from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly uint LoadUInt( string? name ) {
			return UnboxField<uint>( name, FieldType.UInt32 );
		}

		/*
		===============
		LoadULong
		===============
		*/
		/// <summary>
		/// Loads an sbyte from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly ulong LoadULong( string? name ) {
			return UnboxField<ulong>( name, FieldType.UInt64 );
		}

		/*
		===============
		LoadFloat
		===============
		*/
		/// <summary>
		/// Loads a float from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly float LoadFloat( string? name ) {
			return UnboxField<float>( name, FieldType.Float );
		}

		/*
		===============
		LoadDouble
		===============
		*/
		/// <summary>
		/// Loads a double from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly double LoadDouble( string? name ) {
			return UnboxField<double>( name, FieldType.Double );
		}

		/*  fff
		===============
		LoadString
		===============
		*/
		/// <summary>
		/// Loads a string from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly string LoadString( string? name ) {
			return UnboxField<string>( name, FieldType.String );
		}

		/*
		===============
		LoadBoolean
		===============
		*/
		/// <summary>
		/// Loads a boolean from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly bool LoadBoolean( string? name ) {
			return UnboxField<bool>( name, FieldType.Boolean );
		}

		/*
		===============
		LoadVector2
		===============
		*/
		/// <summary>
		/// :oads a Godot.Vector2 from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly Vector2 LoadVector2( string? name ) {
			return UnboxField<Vector2>( name, FieldType.Vector2 );
		}

		/*
		===============
		LoadVector2I
		===============
		*/
		/// <summary>
		/// Loads a Godot.Vector2I from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly Vector2I LoadVector2I( string? name ) {
			return UnboxField<Vector2I>( name, FieldType.Vector2I );
		}

		/*
		===============
		LoadIntList
		===============
		*/
		/// <summary>
		/// Loads an int[] from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly int[] LoadIntList( string? name ) {
			return UnboxField<int[]>( name, FieldType.IntList );
		}

		/*
		===============
		LoadUIntList
		===============
		*/
		/// <summary>
		/// Loads a uint[] from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly uint[] LoadList( string? name ) {
			return UnboxField<uint[]>( name, FieldType.UIntList );
		}

		/*
		===============
		LoadFloatList
		===============
		*/
		/// <summary>
		/// Loads a float[] from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly float[] LoadFloatList( string? name ) {
			return UnboxField<float[]>( name, FieldType.FloatList );
		}

		/*
		===============
		LoadStringList
		===============
		*/
		/// <summary>
		/// Loads a string[] from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly string[] LoadStringList( string? name ) {
			return UnboxField<string[]>( name, FieldType.StringList );
		}

		/*
		===============
		LoadByteArray
		===============
		*/
		/// <summary>
		/// Loads a byte[] from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly byte[] LoadByteArray( string? name ) {
			return UnboxField<byte[]>( name, FieldType.ByteArray );
		}

		/*
		===============
		LoadArray
		===============
		*/
		/// <summary>
		/// Loads a Godot.Collections.Array from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly Godot.Collections.Array LoadArray( string? name ) {
			return UnboxField<Godot.Collections.Array>( name, FieldType.Array );
		}

		/*
		===============
		LoadDictionary
		===============
		*/
		/// <summary>
		/// Loads a Godot.Collections.Dictionary from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly Godot.Collections.Dictionary LoadDictionary( string? name ) {
			return UnboxField<Godot.Collections.Dictionary>( name, FieldType.Dictionary );
		}

		/*
		===============
		LoadField
		===============
		*/
		/// <summary>
		/// Loads a field's value
		/// </summary>
		/// <param name="name">The name of the field</param>
		/// <param name="type">The type of the field</param>
		/// <returns>The loaded field, or empty if not found</returns>
		/// <exception cref="InvalidCastException">Thrown if <paramref name="type"/> doesn't match what's found in the save data</exception>
		private readonly SaveField? LoadField( string? name, FieldType type ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( FieldList );

			if ( FieldList.TryGetValue( name, out SaveField? field ) ) {
				if ( !field.HasValue ) {
					throw new ArgumentNullException( nameof( field ) );
				}

				// if the provided type and the loaded type are the same
				return field.Value.Type != type ?
						field
					:
						throw new InvalidCastException( $"SaveField {name} from savefile isn't the same type as {type}" );
			}

			// not the end of the world, just a missing field, so apply the default value
			Console.PrintError( $"...couldn't find save field {name}" );
			return new SaveField();
		}

		/*
		===============
		UnboxField
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private readonly T UnboxField<T>( string? name, FieldType type ) {
			SaveField? boxed = LoadField( name, type );
			return boxed != null && boxed.Value.Value is T value ? value : throw new InvalidCastException( $"FieldType {type} doesn't match the save data type!" );
		}
	};
};