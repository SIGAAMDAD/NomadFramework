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
using Nomad.Events.Private;
using NUnit.Framework;

namespace Nomad.Events.Tests
{
	[TestFixture]
	public class EventQueueTests
	{
		private EventQueue _queue;
		private IGameEventRegistryService _registry;

		[SetUp]
		public void SetUp()
		{
			var logger = new MockLogger();
			_registry = new GameEventRegistry(logger);
			_queue = new EventQueue();
		}

		[TearDown]
		public void TearDown()
		{
			_registry?.Dispose();
		}
	}
}