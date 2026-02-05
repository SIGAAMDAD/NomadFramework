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

using System.Runtime.InteropServices;
using Nomad.Core.Util;

namespace Nomad.CVars.Private {
	/*
	===================================================================================

	CVarMetadata

	===================================================================================
	*/
	/// <summary>
	/// Holds all a <see cref="ICVar"/>'s metadata.
	/// </summary>

	[StructLayout( LayoutKind.Explicit, Pack = 1, Size = 14 )]
	internal readonly struct CVarMetadata {
		[FieldOffset( 0 )] public readonly InternString Name;
		[FieldOffset( 4 )] public readonly InternString Description;
		[FieldOffset( 8 )] public readonly CVarFlags Flags;
		[FieldOffset( 12 )] public readonly CVarType Type;

		public bool IsReadOnly => ( Flags & CVarFlags.ReadOnly ) != 0;
		public bool IsHidden => ( Flags & CVarFlags.Hidden ) != 0;
		public bool IsSaved => ( Flags & CVarFlags.Archive ) != 0;
		public bool IsUserCreated => ( Flags & CVarFlags.UserCreated ) != 0;

		/*
		===============
		CVarMetadata
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="flags"></param>
		/// <param name="type"></param>
		public CVarMetadata( string name, string description, CVarFlags flags, CVarType type ) {
			Name = new( name );
			Description = new( description );
			Flags = flags;
			Type = type;
		}
	};
};
