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

using Nomad.Core.Events;

namespace Nomad.Core.Engine.Services
{
	[Event(
		name: "Nomad.Core.Engine.Services.PauseStateChanged",
		nameSpace: "Nomad.Core.Engine.Services"
	)]
	public readonly partial struct PauseStateChangedEventArgs
	{
		public bool Paused { get; }
		
		public PauseStateChangedEventArgs(bool paused)
		{
			Paused = paused;
		}
	}
}