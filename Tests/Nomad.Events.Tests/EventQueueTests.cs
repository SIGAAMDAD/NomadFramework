using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Nomad.Events;
using Nomad.Events.Private;
using Nomad.Core.Events; // Contains EventQueue

namespace Nomad.Events.Tests
{
    // Mock implementation of IGameEvent<T> that records when Publish is called.
    public class MockGameEvent<T> : IGameEvent<T> where T : struct
    {
        public string DebugName { get; set; }
        public string NameSpace { get; set; }
        public int Id { get; set; }

        // For tracking calls
        public List<T> PublishedArgs { get; } = new List<T>();
        public int PublishCount => PublishedArgs.Count;

		public T LastPayload => throw new NotImplementedException();

		public int SubscriberCount => throw new NotImplementedException();

		long IGameEvent.PublishCount => PublishCount;

		public DateTime LastPublishTime => throw new NotImplementedException();

		public event EventCallback<T> OnPublished;
		public event AsyncEventCallback<T> OnPublishedAsync;

		public void Publish(in T args)
        {
            PublishedArgs.Add(args);
        }

        // Other interface members can be stubbed as needed
        public void Dispose() { GC.SuppressFinalize( this ); }
        public ISubscriptionHandle Subscribe(EventCallback<T> callback) => throw new NotImplementedException();
        public ISubscriptionHandle SubscribeAsync(AsyncEventCallback<T> callback) => throw new NotImplementedException();
        public void Unsubscribe(EventCallback<T> callback) => throw new NotImplementedException();
        public void UnsubscribeAsync(AsyncEventCallback<T> callback) => throw new NotImplementedException();
        public Task PublishAsync(T args, CancellationToken ct = default) => throw new NotImplementedException();

        // Also implement the non-generic IGameEvent if needed (but we only need generic)
    }

    // Test structs for event arguments
    public struct TestArgs
    {
        public int Value { get; set; }
    }

    public struct OtherArgs
    {
        public string Text { get; set; }
    }

    [TestFixture]
    public class EventQueueTests
    {
        private EventQueue _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new EventQueue();
        }

        [Test]
        public void Constructor_InitializesQueuesForAllPriorities()
        {
            // Count should be zero initially
            Assert.That(_queue.Count, Is.Zero);
        }

        [Test]
        public void Enqueue_SingleEvent_CountIncrements()
        {
            var mockEvent = new MockGameEvent<TestArgs>();
            var args = new TestArgs { Value = 42 };

            _queue.Enqueue(mockEvent, args, EventPriority.Normal);

            Assert.That(_queue.Count, Is.EqualTo(1));
        }

        [Test]
        public void Enqueue_MultipleEvents_CountMatches()
        {
            var mockEvent1 = new MockGameEvent<TestArgs>();
            var mockEvent2 = new MockGameEvent<TestArgs>();
            var args1 = new TestArgs { Value = 1 };
            var args2 = new TestArgs { Value = 2 };

            _queue.Enqueue(mockEvent1, args1, EventPriority.Low);
            _queue.Enqueue(mockEvent2, args2, EventPriority.High);

            Assert.That(_queue.Count, Is.EqualTo(2));
        }

        [Test]
        public void Enqueue_DifferentArgumentTypes_Works()
        {
            var mockEvent1 = new MockGameEvent<TestArgs>();
            var mockEvent2 = new MockGameEvent<OtherArgs>();
            var args1 = new TestArgs { Value = 10 };
            var args2 = new OtherArgs { Text = "Hello" };

            _queue.Enqueue(mockEvent1, args1, EventPriority.Normal);
            _queue.Enqueue(mockEvent2, args2, EventPriority.Normal);

            Assert.That(_queue.Count, Is.EqualTo(2));
        }

        [Test]
        public void ProcessAll_EmptyQueue_DoesNothing()
        {
            _queue.ProcessAll();
            Assert.That(_queue.Count, Is.Zero);
        }

        [Test]
        public void ProcessAll_ProcessesEventsInPriorityOrder()
        {
            var processedOrder = new List<(string EventName, EventPriority Priority, int Value)>();
            var criticalEvent = CreateTrackingEvent<TestArgs>("Critical", (args) => processedOrder.Add(("Critical", EventPriority.Critical, args.Value)));
            var highEvent = CreateTrackingEvent<TestArgs>("High", (args) => processedOrder.Add(("High", EventPriority.High, args.Value)));
            var normalEvent = CreateTrackingEvent<TestArgs>("Normal", (args) => processedOrder.Add(("Normal", EventPriority.Normal, args.Value)));
            var lowEvent = CreateTrackingEvent<TestArgs>("Low", (args) => processedOrder.Add(("Low", EventPriority.Low, args.Value)));
            var veryLowEvent = CreateTrackingEvent<TestArgs>("VeryLow", (args) => processedOrder.Add(("VeryLow", EventPriority.VeryLow, args.Value)));

            // Enqueue in mixed order
            _queue.Enqueue(normalEvent, new TestArgs { Value = 3 }, EventPriority.Normal);
            _queue.Enqueue(veryLowEvent, new TestArgs { Value = 5 }, EventPriority.VeryLow);
            _queue.Enqueue(criticalEvent, new TestArgs { Value = 1 }, EventPriority.Critical);
            _queue.Enqueue(lowEvent, new TestArgs { Value = 4 }, EventPriority.Low);
            _queue.Enqueue(highEvent, new TestArgs { Value = 2 }, EventPriority.High);

            _queue.ProcessAll();

            // Expected order: Critical (1), High (2), Normal (3), Low (4), VeryLow (5)
            Assert.That(processedOrder, Has.Count.EqualTo(5));
			using (Assert.EnterMultipleScope())
			{
				Assert.That(processedOrder[0].Priority, Is.EqualTo(EventPriority.Critical));
				Assert.That(processedOrder[0].Value, Is.EqualTo(1));
				Assert.That(processedOrder[1].Priority, Is.EqualTo(EventPriority.High));
				Assert.That(processedOrder[1].Value, Is.EqualTo(2));
				Assert.That(processedOrder[2].Priority, Is.EqualTo(EventPriority.Normal));
				Assert.That(processedOrder[2].Value, Is.EqualTo(3));
				Assert.That(processedOrder[3].Priority, Is.EqualTo(EventPriority.Low));
				Assert.That(processedOrder[3].Value, Is.EqualTo(4));
				Assert.That(processedOrder[4].Priority, Is.EqualTo(EventPriority.VeryLow));
				Assert.That(processedOrder[4].Value, Is.EqualTo(5));
			}
		}

        [Test]
        public void ProcessAll_SamePriority_FIFO()
        {
            var processedOrder = new List<int>();
            var mockEvent = CreateTrackingEvent<TestArgs>("Test", (args) => processedOrder.Add(args.Value));

            // Enqueue three events with same priority
            _queue.Enqueue(mockEvent, new TestArgs { Value = 1 }, EventPriority.Normal);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 2 }, EventPriority.Normal);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 3 }, EventPriority.Normal);

            _queue.ProcessAll();

            Assert.That(processedOrder, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void ProcessNext_ReturnsTrueAndProcessesHighestPriority()
        {
            var processed = new List<int>();
            var lowEvent = CreateTrackingEvent<TestArgs>("Low", (args) => processed.Add(args.Value));
            var highEvent = CreateTrackingEvent<TestArgs>("High", (args) => processed.Add(args.Value));

            _queue.Enqueue(lowEvent, new TestArgs { Value = 10 }, EventPriority.Low);
            _queue.Enqueue(highEvent, new TestArgs { Value = 20 }, EventPriority.High);

            bool result = _queue.ProcessNext();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.True);
				Assert.That(processed, Is.EqualTo(new[] { 20 })); // High priority processed first
				Assert.That(_queue.Count, Is.EqualTo(1)); // Low still remains
			}
		}

        [Test]
        public void ProcessNext_EmptyQueue_ReturnsFalse()
        {
            bool result = _queue.ProcessNext();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ProcessNext_ProcessesOnlyOneEvent()
        {
            var processed = new List<int>();
            var mockEvent = CreateTrackingEvent<TestArgs>("Test", (args) => processed.Add(args.Value));

            _queue.Enqueue(mockEvent, new TestArgs { Value = 1 }, EventPriority.Normal);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 2 }, EventPriority.Normal);

            bool result1 = _queue.ProcessNext();
            bool result2 = _queue.ProcessNext();
            bool result3 = _queue.ProcessNext();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result1, Is.True);
				Assert.That(result2, Is.True);
				Assert.That(result3, Is.False);
				Assert.That(processed, Is.EqualTo(new[] { 1, 2 }));
				Assert.That(_queue.Count, Is.Zero);
			}
		}

        [Test]
        public void Clear_RemovesAllEvents()
        {
            var mockEvent = new MockGameEvent<TestArgs>();
            _queue.Enqueue(mockEvent, new TestArgs { Value = 1 }, EventPriority.Normal);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 2 }, EventPriority.High);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 3 }, EventPriority.Low);

            Assert.That(_queue.Count, Is.EqualTo(3));

            _queue.Clear();

            Assert.That(_queue.Count, Is.Zero);

            // ProcessAll should do nothing
            _queue.ProcessAll();
            Assert.That(mockEvent.PublishCount, Is.Zero);
        }

        [Test]
        public void Clear_AfterPartialProcessing_StillClears()
        {
            var processed = new List<int>();
            var mockEvent = CreateTrackingEvent<TestArgs>("Test", (args) => processed.Add(args.Value));

            _queue.Enqueue(mockEvent, new TestArgs { Value = 1 }, EventPriority.High);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 2 }, EventPriority.Normal);
            _queue.Enqueue(mockEvent, new TestArgs { Value = 3 }, EventPriority.Low);

            _queue.ProcessNext(); // processes High (1)

            _queue.Clear();

            _queue.ProcessAll(); // should process nothing

			using (Assert.EnterMultipleScope())
			{
				Assert.That(processed, Is.EqualTo(new[] { 1 }));
				Assert.That(_queue.Count, Is.Zero);
			}
		}

        [Test]
        public void Count_ReflectsQueuedItemsAcrossPriorities()
        {
            Assert.That(_queue.Count, Is.Zero);

            var mockEvent = new MockGameEvent<TestArgs>();
            _queue.Enqueue(mockEvent, new TestArgs(), EventPriority.VeryLow);
            Assert.That(_queue.Count, Is.EqualTo(1));

            _queue.Enqueue(mockEvent, new TestArgs(), EventPriority.VeryHigh);
            Assert.That(_queue.Count, Is.EqualTo(2));

            _queue.Enqueue(mockEvent, new TestArgs(), EventPriority.Critical);
            Assert.That(_queue.Count, Is.EqualTo(3));

            _queue.ProcessNext(); // removes Critical
            Assert.That(_queue.Count, Is.EqualTo(2));
        }

        // Helper to create an event that invokes an action when published.
        private static IGameEvent<T> CreateTrackingEvent<T>(string name, Action<T> onPublish) where T : struct
        {
            var mock = new MockGameEvent<T> { DebugName = name };
            // We need to override Publish to call the action.
            // Since we can't easily override, we can modify MockGameEvent to accept a callback.
            // Let's enhance MockGameEvent to allow a callback.
            return new CallbackGameEvent<T>(onPublish);
        }

        // A more flexible mock that executes a callback on publish.
        private class CallbackGameEvent<T> : IGameEvent<T> where T : struct
        {
            private readonly Action<T> _callback;

			public event EventCallback<T> OnPublished;
			public event AsyncEventCallback<T> OnPublishedAsync;

			public CallbackGameEvent(Action<T> callback)
            {
                _callback = callback;
            }

            public string DebugName { get; set; }
            public string NameSpace { get; set; }
            public int Id { get; set; }

			public T LastPayload => throw new NotImplementedException();

			public int SubscriberCount => throw new NotImplementedException();

			public long PublishCount => throw new NotImplementedException();

			public DateTime LastPublishTime => throw new NotImplementedException();

			public void Publish(in T args)
            {
                _callback(args);
            }

            public void Dispose() { }
            public ISubscriptionHandle Subscribe(EventCallback<T> callback) => throw new NotImplementedException();
            public ISubscriptionHandle SubscribeAsync(AsyncEventCallback<T> callback) => throw new NotImplementedException();
            public void Unsubscribe(EventCallback<T> callback) => throw new NotImplementedException();
            public void UnsubscribeAsync(AsyncEventCallback<T> callback) => throw new NotImplementedException();
            public Task PublishAsync(T args, CancellationToken ct = default) => throw new NotImplementedException();
        }
    }
}