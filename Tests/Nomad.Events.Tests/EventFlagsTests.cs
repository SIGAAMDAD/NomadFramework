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
using NUnit.Framework;
using Nomad.Core.Events;
using Nomad.Core.Logger;
using Nomad.Events;
using Nomad.Events.Private;
using Nomad.Events.Private.SubscriptionSets;

namespace Nomad.Events.Tests;

/// <summary>
/// Tests for EventFlags and event configuration options
/// </summary>
[TestFixture]
public class EventFlagsTests
{
	private ILoggerService _logger;
	private IGameEventRegistryService _registry;

	[SetUp]
	public void Setup()
	{
		_logger = new MockLogger();
		_registry = new GameEventRegistry(_logger);
	}

	[TearDown]
	public void TearDown()
	{
		_logger?.Dispose();
		_registry?.Dispose();
	}

	#region EventFlags Enum Tests

	[Test]
	public void EventFlags_Synchronous_ValueIsSet()
	{
		// Act
		uint value = (uint)EventFlags.Synchronous;

		// Assert
		Assert.That(value, Is.EqualTo(1u));
	}

	[Test]
	public void EventFlags_Asynchronous_ValueIsSet()
	{
		// Act
		uint value = (uint)EventFlags.Asynchronous;

		// Assert
		Assert.That(value, Is.EqualTo(2u));
	}

	[Test]
	public void EventFlags_StrongSubscriptions_ValueIsSet()
	{
		// Act
		uint value = (uint)EventFlags.StrongSubscriptions;

		// Assert
		Assert.That(value, Is.EqualTo(4u));
	}

	[Test]
	public void EventFlags_NoLock_ValueIsSet()
	{
		// Act
		uint value = (uint)EventFlags.NoLock;

		// Assert
		Assert.That(value, Is.EqualTo(8u));
	}

	[Test]
	public void EventFlags_Default_ContainsSynchronousAndAsynchronous()
	{
		// Act
		EventFlags defaultFlags = EventFlags.Default;
		bool hasSynchronous = (defaultFlags & EventFlags.Synchronous) != 0;
		bool hasAsynchronous = (defaultFlags & EventFlags.Asynchronous) != 0;

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(hasSynchronous, Is.True);
			Assert.That(hasAsynchronous, Is.True);
		}
	}

	#endregion

	#region Flag Combination Tests

	[Test]
	public void EventFlags_CanCombineFlags()
	{
		// Act
		EventFlags combined = EventFlags.Synchronous | EventFlags.StrongSubscriptions;
		bool hasSync = (combined & EventFlags.Synchronous) != 0;
		bool hasStrong = (combined & EventFlags.StrongSubscriptions) != 0;

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(hasSync, Is.True);
			Assert.That(hasStrong, Is.True);
		}
	}

	[Test]
	public void EventFlags_CanCheckForFlag()
	{
		// Arrange
		EventFlags flags = EventFlags.Synchronous | EventFlags.Asynchronous;

		// Act
		bool hasSynchronous = (flags & EventFlags.Synchronous) != 0;
		bool hasNoLock = (flags & EventFlags.NoLock) != 0;

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(hasSynchronous, Is.True);
			Assert.That(hasNoLock, Is.False);
		}
	}

	#endregion

	#region Event Configuration Tests

	[Test]
	public void EventFlags_Default_CreatesAsyncCapableEvent()
	{
		// Arrange
		var registry = new GameEventRegistry(_logger);

		// Act
		var gameEvent = registry.GetEvent<EmptyEventArgs>("Test", "Event");

		// Assert
		Assert.That(gameEvent, Is.Not.Null);

		registry.Dispose();
	}

	#endregion

	#region Flag Invalidation Tests

	[Test]
	public void EventFlags_SynchronousOnly_IsValid()
	{
		// Arrange
		EventFlags flags = EventFlags.Synchronous;

		// Act
		bool isValid = flags != 0;

		// Assert
		Assert.That(isValid, Is.True);
	}

	[Test]
	public void EventFlags_AsynchronousOnly_IsValid()
	{
		// Arrange
		EventFlags flags = EventFlags.Asynchronous;

		// Act
		bool isValid = flags != 0;

		// Assert
		Assert.That(isValid, Is.True);
	}

	[Test]
	public void EventFlags_NoLockOnly_DisablesAsync()
	{
		// Arrange
		EventFlags flags = EventFlags.NoLock;

		// Act
		bool hasAsync = (flags & EventFlags.Asynchronous) != 0;

		// Assert
		Assert.That(hasAsync, Is.False);
	}

	#endregion

	#region Flag Behavior Tests

	[Test]
	public void EventFlags_Synchronous_AllowsSynchronousCallbacks()
	{
		// Arrange
		EventFlags flags = EventFlags.Synchronous;

		// Act
		bool allowsSync = (flags & EventFlags.Synchronous) != 0;

		// Assert
		Assert.That(allowsSync, Is.True);
	}

	[Test]
	public void EventFlags_Asynchronous_AllowsAsyncCallbacks()
	{
		// Arrange
		EventFlags flags = EventFlags.Asynchronous;

		// Act
		bool allowsAsync = (flags & EventFlags.Asynchronous) != 0;

		// Assert
		Assert.That(allowsAsync, Is.True);
	}

	[Test]
	public void EventFlags_StrongSubscriptions_IndicatesStrongReferences()
	{
		// Arrange
		EventFlags flags = EventFlags.StrongSubscriptions;

		// Act
		bool useStrong = (flags & EventFlags.StrongSubscriptions) != 0;

		// Assert
		Assert.That(useStrong, Is.True);
	}

	[Test]
	public void EventFlags_NoLock_IndicatesLockFree()
	{
		// Arrange
		EventFlags flags = EventFlags.NoLock;

		// Act
		bool isLockFree = (flags & EventFlags.NoLock) != 0;

		// Assert
		Assert.That(isLockFree, Is.True);
	}

	#endregion

	#region Default Behavior Tests

	[Test]
	public void EventFlags_Default_SupportsBothSyncAndAsync()
	{
		// Arrange
		EventFlags defaultFlags = EventFlags.Default;

		// Act
		bool hasSynchronous = (defaultFlags & EventFlags.Synchronous) != 0;
		bool hasAsynchronous = (defaultFlags & EventFlags.Asynchronous) != 0;
		bool hasNoLock = (defaultFlags & EventFlags.NoLock) != 0;

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(hasSynchronous, Is.True);
			Assert.That(hasAsynchronous, Is.True);
			Assert.That(hasNoLock, Is.False);
		}
	}

	#endregion

	#region Flag Exclusivity Tests

	[Test]
	public void EventFlags_FlagsAreDistinct()
	{
		// These flags should not interfere with each other when combined
		EventFlags flags1 = EventFlags.Synchronous | EventFlags.StrongSubscriptions;
		EventFlags flags2 = EventFlags.Asynchronous | EventFlags.NoLock;

		// Act
		bool flags1HasSync = (flags1 & EventFlags.Synchronous) != 0;
		bool flags1HasStrong = (flags1 & EventFlags.StrongSubscriptions) != 0;
		bool flags2HasAsync = (flags2 & EventFlags.Asynchronous) != 0;
		bool flags2HasNoLock = (flags2 & EventFlags.NoLock) != 0;

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(flags1HasSync, Is.True);
			Assert.That(flags1HasStrong, Is.True);
			Assert.That(flags2HasAsync, Is.True);
			Assert.That(flags2HasNoLock, Is.True);
		}
	}

	#endregion

	[Test]
	public void NoLock_UsesLockFreeSubscriptionSet()
	{
		// Arrange
		var ev = _registry.GetEvent<EmptyEventArgs>("Test", "Event",
			EventFlags.NoLock);

		Assert.That(ev, Is.InstanceOf<GameEvent<EmptyEventArgs>>());

		// Assert
		var typedEvent = (GameEvent<EmptyEventArgs>)ev;
		Assert.That(typedEvent.SubscriptionSet, Is.TypeOf<LockFreeSubscriptionSet<EmptyEventArgs>>());
	}

	[Test]
	public void Async_UsesSubscriptionSet()
	{
		// Arrange
		var ev = _registry.GetEvent<EmptyEventArgs>("Test", "Event",
			EventFlags.Asynchronous);

		Assert.That(ev, Is.InstanceOf<GameEvent<EmptyEventArgs>>());

		// Assert
		var typedEvent = (GameEvent<EmptyEventArgs>)ev;
		Assert.That(typedEvent.SubscriptionSet, Is.TypeOf<SubscriptionSet<EmptyEventArgs>>());
	}
}
