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
using System.Runtime.CompilerServices;

namespace SaveSystem {
	/*
	===================================================================================
	
	FieldLoader
	
	===================================================================================
	*/
	/// <summary>
	/// The general purpose middle-man between <see cref="SaveField"/> and <see cref="VariantLoader"/>.
	/// A global class that simply loads from a <see cref="Streams.SaveReaderStream"/> stream
	/// </summary>
	/// <remarks>
	/// Meant strictly to reduce repetitive code and cross-referencing between <see cref="SaveField"/> and <see cref="VariantLoader"/>
	/// </remarks>

	public static class FieldLoader {
		/*
		===============
		LoadColor
		===============
		*/
		/// <summary>
		/// Reads a 4-part floating point <see cref="Color"/> from the provided stream reader
		/// </summary>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The <see cref="Color"/> processed from the stream</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Color LoadColor( Streams.SaveReaderStream reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			return new Color(
				reader.ReadFloat(),
				reader.ReadFloat(),
				reader.ReadFloat(),
				reader.ReadFloat()
			);
		}

		/*
		===============
		LoadVector2
		===============
		*/
		/// <summary>
		/// Loads a <see cref="Godot.Vector2"/> from the provided stream
		/// </summary>
		/// <remarks>
		/// Any exceptions thrown here are the responsibility of the caller
		/// </remarks>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The loaded <see cref="Vector2"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2 LoadVector2( Streams.SaveReaderStream reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			return new Vector2(
				reader.ReadFloat(),
				reader.ReadFloat()
			);
		}

		/*
		===============
		LoadVector2I
		===============
		*/
		/// <summary>
		/// Loads a <see cref="Vector2I"/> from the provided stream
		/// </summary>
		/// <remarks>
		/// Any exceptions thrown here are the responsibility of the caller
		/// </remarks>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The loaded <see cref="Vector2I"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2I LoadVector2I( Streams.SaveReaderStream reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			return new Vector2I(
				reader.ReadInt32(),
				reader.ReadInt32()
			);
		}

		/*
		===============
		LoadArray
		===============
		*/
		/// <summary>
		/// Loads a C# fixed-size array from the provided stream
		/// </summary>
		/// <remarks>
		/// <para>The <paramref name="reader"/> doesn't need to be given to the <paramref name="loadCallback"/> because we already
		/// have it in the LoadVariant.</para>
		/// <para>Any exceptions thrown here are the responsibility of the caller.</para>
		/// </remarks>
		/// <typeparam name="T">The type of the array</typeparam>
		/// <param name="reader">The stream to read from</param>
		/// <param name="loadCallback">The callback to processing the input</param>
		/// <returns>The loaded fixed-size array</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T[] LoadArray<T>( Streams.SaveReaderStream reader, Action<T>? loadCallback ) {
			ArgumentNullException.ThrowIfNull( reader );
			ArgumentNullException.ThrowIfNull( loadCallback );

			T[] values = new T[ reader.ReadInt32() ];
			for ( int i = 0; i < values.Length; i++ ) {
				loadCallback.Invoke( values[ i ] );
			}
			return values;
		}

		/*
		===============
		LoadByteArray
		===============
		*/
		/// <summary>
		/// <para>Loads a C# fixed-size byte[] array from the provided stream.</para>
		/// <para>This is a separate function from <see cref="LoadArray"/> for performance's sake</para>
		/// </summary>
		/// <remarks>
		/// Any exceptions thrown here are the responsibility of the caller.
		/// </remarks>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The loaded fixed-size byte[] array</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static byte[] LoadByteArray( Streams.SaveReaderStream reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			byte[] buffer = new byte[ reader.ReadInt32() ];
			reader.ReadExactly( buffer, 0, buffer.Length );
			return buffer;
		}

		/*
		===============
		LoadGodotArray
		===============
		*/
		/// <summary>
		/// Loads a <see cref="Godot.Collections.Array"/> from the provided stream
		/// </summary>
		/// <remarks>
		/// Any exceptions thrown here are the responsibility of the caller
		/// </remarks>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The loaded array</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Godot.Collections.Array LoadGodotArray( Streams.SaveReaderStream reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			Godot.Collections.Array value = new Godot.Collections.Array();

			int count = reader.ReadInt32();
			value.Resize( count );
			for ( int i = 0; i < count; i++ ) {
				value[ i ] = LoadVariant( reader );
			}
			return value;
		}

		/*
		===============
		LoadGodotDictionary
		===============
		*/
		/// <summary>
		/// Loads a <see cref="Godot.Collections.Dictionary"/> from the provided stream
		/// </summary>
		/// <remarks>
		/// Any exceptions thrown here are the responsibility of the caller
		/// </remarks>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The loaded dictionary</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Godot.Collections.Dictionary LoadGodotDictionary( Streams.SaveReaderStream? reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			Godot.Collections.Dictionary value = new Godot.Collections.Dictionary();

			int count = reader.ReadInt32();
			for ( int i = 0; i < count; i++ ) {
				value.Add( LoadVariant( reader ), LoadVariant( reader ) );
			}
			return value;
		}

		/*
		===============
		LoadVariant
		===============
		*/
		/// <summary>
		/// Loads a Godot.Variant from the stream
		/// </summary>
		/// <remarks>
		/// Any exceptions thrown here are the responsibility of the caller
		/// </remarks>
		/// <param name="reader">The stream to read from</param>
		/// <returns>The loaded variant</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/> is null</exception>
		/// <exception cref="IndexOutOfRangeException">Thrown if the variant type isn't supported</exception>
		public static Variant LoadVariant( Streams.SaveReaderStream? reader ) {
			ArgumentNullException.ThrowIfNull( reader );

			// type is only really here for the exception message
			Variant.Type type = (Variant.Type)reader.ReadUInt32();
			return type switch {
				Variant.Type.Bool => Variant.From( reader.ReadBoolean() ),
				Variant.Type.Int => Variant.From( reader.ReadInt32() ),
				Variant.Type.Float => Variant.From( reader.ReadFloat() ),
				Variant.Type.String or Variant.Type.StringName or Variant.Type.NodePath => Variant.From( reader.ReadString() ),
				Variant.Type.Color => LoadColor( reader ),
				Variant.Type.Vector2 => LoadVector2( reader ),
				Variant.Type.Vector2I => LoadVector2I( reader ),
				Variant.Type.PackedColorArray => LoadArray<Color>( reader, ( color ) => color = LoadColor( reader ) ),
				Variant.Type.PackedInt32Array => LoadArray<int>( reader, ( value ) => value = reader.ReadInt32() ),
				Variant.Type.PackedFloat64Array => LoadArray<long>( reader, ( value ) => value = reader.ReadInt64() ),
				Variant.Type.Array => LoadGodotArray( reader ),
				Variant.Type.Dictionary => LoadGodotDictionary( reader ),
				_ => throw new IndexOutOfRangeException( $"invalid godot variant type found in savefile - {type}" )
			};
		}
	};
};