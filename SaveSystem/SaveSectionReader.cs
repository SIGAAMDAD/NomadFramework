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
using System.IO;
using System.Runtime.CompilerServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	SaveSectionReader
	
	===================================================================================
	*/

	/// <summary>
	/// <para>The dedicated class for reading dictionary style "sections" froms the current savefile.</para>
	/// 
	/// <para>NOTE: reading from loaded save section can be multithreaded since we are loading all values
	/// into RAM from disk before applying gamestate.</para>
	/// 
	/// <para>exceptions are caught in ArchiveSystem.LoadGame.</para>
	/// 
	/// <para>the default stream is ArchiveSystem.SaveReader, but for the sake of testing, it uses
	/// dependency injection in the constructor.</para>
	/// </summary>
	
	internal readonly struct SaveSectionReader {
		private readonly Streams.SaveReaderStream Reader;
		private readonly ConcurrentDictionary<string, SaveField?> FieldList;

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
		public SaveSectionReader( Streams.SaveReaderStream reader ) {
			Reader = reader;

			int fieldCount = Reader.ReadInt32();
			FieldList = new ConcurrentDictionary<string, SaveField?>( 1, fieldCount );

			ConsoleSystem.Console.PrintLine( $"...Got {fieldCount} fields." );

			for ( int i = 0; i < fieldCount; i++ ) {
				string name = Reader.ReadString();
				if ( name.Length <= 0 ) {
					throw new Exception( $"SaveField at offset {Reader.Position} has invalid name (length <= 0)" );
				}
				if ( !FieldList.TryAdd( name, new SaveField( reader ) ) ) {
					throw new InvalidOperationException( $"A duplicate of SaveField {name} was found in savefile, aborting" );
				}
				ConsoleSystem.Console.PrintLine( $"...loaded field \"{name}\"" );
			}
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public readonly T Load<T>( string? name ) {
			ArgumentException.ThrowIfNullOrEmpty( name );

			SaveField? field = LoadField( name, FieldValue.GetFieldType<T>() );
			if ( field.HasValue && field.Value.Value is T value ) {
				return value;
			}

			return default;
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
		public readonly sbyte LoadSByte( string? name ) => Load<sbyte>( name );

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
		public readonly short LoadShort( string? name ) => Load<short>( name );

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
		public readonly int LoadInt( string? name ) => Load<int>( name );

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
		public readonly long LoadLong( string? name ) => Load<long>( name );

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
		public readonly byte LoadByte( string? name ) => Load<byte>( name );

		/*
		===============
		LoadUShort
		===============
		*/
		/// <summary>
		/// Loads a ushort from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly ushort LoadUShort( string? name ) => Load<ushort>( name );

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
		public readonly uint LoadUInt( string? name ) => Load<uint>( name );

		/*
		===============
		LoadULong
		===============
		*/
		/// <summary>
		/// Loads a ulong from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly ulong LoadULong( string? name ) => Load<ulong>( name );

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
		public readonly float LoadFLoat( string? name ) => Load<float>( name );

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
		public readonly double LoadDouble( string? name ) => Load<double>( name );

		/*
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
		public readonly string LoadString( string? name ) => Load<string>( name );

		/*
		===============
		LoadBoolean
		===============
		*/
		/// <summary>
		/// Loads a bool from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly bool LoadBoolean( string? name ) => Load<bool>( name );

		/*
		===============
		LoadVector2
		===============
		*/
		/// <summary>
		/// Loads a Vector2 from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly Vector2 LoadVector2( string? name ) => Load<Vector2>( name );

		/*
		===============
		LoadVector2I
		===============
		*/
		/// <summary>
		/// Loads a Vector2I from the loaded save section
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public readonly Vector2I LoadVector2I( string? name ) => Load<Vector2I>( name );

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
		private readonly SaveField? LoadField( string name, FieldType type ) {
			if ( FieldList.TryGetValue( name, out SaveField? field ) ) {
				if ( !field.HasValue ) {
					ConsoleSystem.Console.PrintError( $"SaveField {name} has a null value in cache!" );
					return null;
				}

				// if the provided type and the loaded type are the same
				return field.Value.Type != type ?
						throw new InvalidCastException( $"SaveField {name} from savefile isn't the same type as {type}" )
					:
						field;
			}

			// not the end of the world, just a missing field, so apply the default value
			ConsoleSystem.Console.PrintError( $"...couldn't find save field {name}" );
			return null;
		}
	};
};