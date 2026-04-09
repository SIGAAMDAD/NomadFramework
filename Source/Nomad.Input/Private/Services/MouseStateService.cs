/*
===========================================================================
The Nomad MPLv2 Source Code
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
using System.Numerics;
using Nomad.Core.Input;
using Nomad.Input.Interfaces;

namespace Nomad.Input.Private.Services {
	internal sealed class MouseStateService : IMouseService {
		public Vector2 Position => _position;
		private Vector2 _position;

		private readonly IInputSnapshotService _snapshotService;

		public MouseStateService( IInputSnapshotService snapshotService ) {
			_snapshotService = snapshotService ?? throw new ArgumentNullException( nameof( snapshotService ) );
		}

		public bool IsPressed( MouseButton mouseButton ) {
			throw new NotImplementedException();
		}
	};
};