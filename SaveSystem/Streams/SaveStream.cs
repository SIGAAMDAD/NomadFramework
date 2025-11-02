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

using System;
using System.Runtime.CompilerServices;

namespace SaveSystem.Streams {
	/*
	===================================================================================
	
	SaveStream
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public abstract class SaveStream : IDisposable {
		/// <summary>
		/// The current offset of the buffer. Basically a pointer without the pointer.
		/// </summary>
		public int Position { get; protected set; } = 0;

		/// <summary>
		/// 
		/// </summary>
		public virtual byte[]? Buffer { get; protected set; }

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public virtual void Dispose() {
			GC.SuppressFinalize( this );
		}

		/*
		===============
		Seek
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="origin"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void Seek( int position, System.IO.SeekOrigin origin ) {
			ArgumentOutOfRangeException.ThrowIfLessThan( position, 0 );
			ArgumentNullException.ThrowIfNull( Buffer );

			switch ( origin ) {
				case System.IO.SeekOrigin.Begin:
					ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( position, Buffer.Length );
					Position = position;
					break;
				case System.IO.SeekOrigin.Current:
					ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual( position + Position, Buffer.Length );
					Position += position;
					break;
				case System.IO.SeekOrigin.End:
					ArgumentOutOfRangeException.ThrowIfGreaterThan( position, 0 );
					Position = Buffer.Length - 1;
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof( origin ) );
			}
		}

		/*
		===============
		Reset
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Reset() {
			Position = 0;
		}
	};
};