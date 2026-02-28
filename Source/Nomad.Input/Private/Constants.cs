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

namespace Nomad.Input.Private {
	internal static class Constants {
		internal static class CVars {
			public const string NAMESPACE = nameof( Nomad.Input );

			public const string DEFAULTS_PATH = NAMESPACE + ".DefaultsPath";
			public const string INPUT_DELAY_MS = NAMESPACE + ".InputDelayMS";
		};

		public const string KEYBOARD_DEVICE_ID = "Keyboard";
		public const string MOUSE_DEVICE_ID = "Mouse";
		public const string GAMEPAD_DEVICE_ID = "Gamepad";
	};
};