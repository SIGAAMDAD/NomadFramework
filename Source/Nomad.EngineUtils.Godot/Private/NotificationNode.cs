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

using System;
using Godot;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================

	NotificationNode

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed partial class NotificationNode : GodotObject {
		public event Action<bool> FocusChanged;

		/*
		===============
		_Notification
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="what"></param>
		public override void _Notification( int what ) {
			switch ( ( long )what ) {
				case Node.NotificationWMWindowFocusIn:
					FocusChanged?.Invoke( true );
					break;
				case Node.NotificationWMWindowFocusOut:
					FocusChanged?.Invoke( false );
					break;
			}
		}
	};
};
