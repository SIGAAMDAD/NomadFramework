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

#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nomad.Core.Events;
using Nomad.Core.Logger;

namespace Nomad.Events.Tests;

/// <summary>
/// Tests for basic subscription and unsubscription functionality
/// </summary>
[TestFixture]
public class SubscriptionTests
{
	private IGameEventRegistryService _registry;
	private ILoggerService _logger;

	[SetUp]
	public void Setup()
	{
		_logger = new MockLogger();
		_registry = new GameEventRegistry(_logger);
	}

	[TearDown]
	public void TearDown()
	{
		_registry?.Dispose();
		_logger?.Dispose();
	}

	#region Basic Subscribe/Publish Tests

	[Test]
	public void Subscribe_WithCallback_CallbackIsInvokedOnPublish()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<TestEventArgs>("Test", "Event");
		bool callbackInvoked = false;
		TestEventArgs receivedArgs = default;

		void Callback(in TestEventArgs args)
		{
			callbackInvoked = true;
			receivedArgs = args;
		}

		var subscriber = this;
		var subscription = gameEvent.Subscribe(subscriber, Callback);

			// Act
			var testArgs = new TestEventArgs { Value = 42, Message = "Test" };
			gameEvent.Publish(in testArgs);

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(callbackInvoked, Is.True);
			Assert.That(receivedArgs.Value, Is.EqualTo(42));
			Assert.That(receivedArgs.Message, Is.EqualTo("Test"));
		}
	}

	[Test]
	public void Subscribe_WithDifferentSubscribers_AllCallbacksInvoked()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		var callStack = new List<string>();

		void Callback1(in EmptyEventArgs args) => callStack.Add("Subscriber1");
		void Callback2(in EmptyEventArgs args) => callStack.Add("Subscriber2");
		void Callback3(in EmptyEventArgs args) => callStack.Add("Subscriber3");

		var subscriber1 = "Sub1";
		var subscriber2 = "Sub2";
		var subscriber3 = "Sub3";

		gameEvent.Subscribe(subscriber1, Callback1);
		gameEvent.Subscribe(subscriber2, Callback2);
		gameEvent.Subscribe(subscriber3, Callback3);

		// Act
		gameEvent.Publish(default);

		// Assert
		Assert.That(callStack, Has.Count.EqualTo(3));
		Assert.That(callStack, Contains.Item("Subscriber1"));
		Assert.That(callStack, Contains.Item("Subscriber2"));
		Assert.That(callStack, Contains.Item("Subscriber3"));
	}

	#endregion

	#region Unsubscribe Tests

	[Test]
	public void Unsubscribe_WithValidSubscription_RemovesCallback()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		bool callbackInvoked = false;

		void Callback(in EmptyEventArgs args)
		{
			callbackInvoked = true;
		}

		var subscriber = new object();
		gameEvent.Subscribe(subscriber, Callback);

		// Act
		gameEvent.Unsubscribe(subscriber, Callback);
		gameEvent.Publish(default); // Second publish

		// Assert
		Assert.That(callbackInvoked, Is.False);
	}

	[Test]
	public void Unsubscribe_WithNonExistentSubscription_DoesNotThrow()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		static void Callback(in EmptyEventArgs args) { }

		// Act & Assert
		Assert.DoesNotThrow(() =>
		{
			gameEvent.Unsubscribe("Subscriber", Callback);
		});
	}

	[Test]
	public void UnsubscribeAll_RemovesAllSubscriptionsForSubscriber()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		var callStack = new List<int>();

		void Callback1(in EmptyEventArgs args) => callStack.Add(1);
		void Callback2(in EmptyEventArgs args) => callStack.Add(2);
		void Callback3(in EmptyEventArgs args) => callStack.Add(3);

		var subscriber = this;
		gameEvent.Subscribe(subscriber, Callback1);
		gameEvent.Subscribe(subscriber, Callback2);
		gameEvent.Subscribe(subscriber, Callback3);

		// Act
		gameEvent.UnsubscribeAll(subscriber);
		gameEvent.Publish(default);

		// Assert
		Assert.That(callStack, Is.Empty);
	}

	[Test]
	public void UnsubscribeAll_OnlyRemovesForSpecificSubscriber()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		var callStack = new List<string>();

		void Callback(in EmptyEventArgs args) => callStack.Add("Called");

		var subscriber1 = "Sub1";
		var subscriber2 = "Sub2";

		gameEvent.Subscribe(subscriber1, Callback);
		gameEvent.Subscribe(subscriber2, Callback);

		// Act
		gameEvent.UnsubscribeAll(subscriber1);
		gameEvent.Publish(default);

		// Assert
		Assert.That(callStack, Has.Count.EqualTo(1));
	}

	#endregion

	#region Null Handling Tests

	[Test]
	public void Subscribe_WithOwnerAndNullCallback_ThrowsArgumentNullException()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() =>
		{
			gameEvent.Subscribe(this, (EventCallback<EmptyEventArgs>)null);
		});
	}

	[Test]
	public void Subscribe_WithNullOwner_ThrowsArgumentNullException()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		static void Callback(in EmptyEventArgs args) { }

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() =>
		{
			gameEvent.Subscribe(null, Callback);
		});
	}

	[Test]
	public void Unsubscribe_WithNullOwner_ThrowsArgumentNullException()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		void Callback(in EmptyEventArgs args) { }

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() =>
		{
			gameEvent.Unsubscribe(null, Callback);
		});
	}

	[Test]
	public void Unsubscribe_WithNullCallback_ThrowsArgumentNullException()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() =>
		{
			gameEvent.Unsubscribe(this, (EventCallback<EmptyEventArgs>)null);
		});
	}

	[Test]
	public void UnsubscribeAll_WithNullOwner_ThrowsArgumentNullException()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() =>
		{
			gameEvent.UnsubscribeAll(null);
		});
	}

	#endregion

	#region Multiple Event Types Tests

	[Test]
	public void Subscribe_WithDifferentEventArgTypes_KeepsSubscriptionsIndependent()
	{
		// Arrange
		var event1 = _registry.GetEvent<EmptyEventArgs>("Test", "Event1");
		var event2 = _registry.GetEvent<TestEventArgs>("Test", "Event2");

		int invocations1 = 0;
		int invocations2 = 0;

		void Callback1(in EmptyEventArgs args) => invocations1++;
		void Callback2(in TestEventArgs args) => invocations2++;

		var subscriber = this;
		event1.Subscribe(subscriber, Callback1);
		event2.Subscribe(subscriber, Callback2);

		// Act
		event1.Publish(default);
		event2.Publish(new TestEventArgs { Value = 42 });

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(invocations1, Is.EqualTo(1));
			Assert.That(invocations2, Is.EqualTo(1));
		}
	}

	#endregion

	#region Order and State Tests

	[Test]
	public void Subscribe_InvocationsOccurInLastSubscribedFirstOrder()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<EmptyEventArgs>("Test", "Event");
		var callStack = new List<int>();

		void Callback1(in EmptyEventArgs args) => callStack.Add(1);
		void Callback2(in EmptyEventArgs args) => callStack.Add(2);
		void Callback3(in EmptyEventArgs args) => callStack.Add(3);

		var subscriber = this;
		gameEvent.Subscribe(subscriber, Callback1);
		gameEvent.Subscribe(subscriber, Callback2);
		gameEvent.Subscribe(subscriber, Callback3);

		// Act
		gameEvent.Publish(default);

		using (Assert.EnterMultipleScope())
		{
			// Assert - Callbacks should be invoked in subscription order
			Assert.That(callStack[0], Is.EqualTo(1));
			Assert.That(callStack[1], Is.EqualTo(2));
			Assert.That(callStack[2], Is.EqualTo(3));
		}
	}

	#endregion

	#region Event Args Preservation Tests

	[Test]
	public void Publish_PreservesEventArgsValues()
	{
		// Arrange
		var gameEvent = _registry.GetEvent<TestEventArgs>("Test", "Event");
		var capturedArgs = new List<TestEventArgs>();

		void Callback(in TestEventArgs args)
		{
			capturedArgs.Add(args);
		}

		var subscriber = this;
		gameEvent.Subscribe(subscriber, Callback);

		// Act
		var args1 = new TestEventArgs { Value = 100, Message = "First" };
		var args2 = new TestEventArgs { Value = 200, Message = "Second" };
		var args3 = new TestEventArgs { Value = 300, Message = "Third" };

		gameEvent.Publish(in args1);
		gameEvent.Publish(in args2);
		gameEvent.Publish(in args3);

		// Assert
		Assert.That(capturedArgs, Has.Count.EqualTo(3));
		using (Assert.EnterMultipleScope())
		{
			Assert.That(capturedArgs[0].Value, Is.EqualTo(100));
			Assert.That(capturedArgs[1].Value, Is.EqualTo(200));
			Assert.That(capturedArgs[2].Value, Is.EqualTo(300));
			Assert.That(capturedArgs[0].Message, Is.EqualTo("First"));
			Assert.That(capturedArgs[1].Message, Is.EqualTo("Second"));
			Assert.That(capturedArgs[2].Message, Is.EqualTo("Third"));
		}
	}

	#endregion
}
#endif
