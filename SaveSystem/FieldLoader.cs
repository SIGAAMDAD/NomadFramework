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

	internal static class FieldLoader {
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
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2 LoadVector2( Streams.SaveReaderStream reader ) {
			return new Vector2( reader.ReadFloat(), reader.ReadFloat() );
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
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2I LoadVector2I( Streams.SaveReaderStream reader ) {
			return new Vector2I( reader.ReadInt32(), reader.ReadInt32() );
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
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static byte[] LoadByteArray( Streams.SaveReaderStream reader ) {
			byte[] buffer = new byte[ reader.ReadInt32() ];
			reader.ReadExactly( buffer, 0, buffer.Length );
			return buffer;
		}
	};
};