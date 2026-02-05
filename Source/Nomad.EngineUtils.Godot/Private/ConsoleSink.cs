/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026s Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Godot;
using System.Runtime.CompilerServices;
using Nomad.Core.Logger;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================

	ConsoleSink

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class ConsoleSink : SinkBase {
		/*
		===============
		Print
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Print( in string message ) {
			GD.PrintRich( message );
		}

		/*
		===============
		Clear
		===============
		*/
		/// <summary>
		///
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Clear() {
			System.Console.Clear();
		}

		/*
		===============
		Flush
		===============
		*/
		/// <summary>
		///
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public override void Flush() {
		}
	};
};
