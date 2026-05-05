using System;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Events;

namespace Nomad.Events.Tests
{
    internal sealed class TestGameEventMetadata<T> : IGameEvent<T> where T : struct
    {
        public string DebugName { get; }
        public string NameSpace { get; }
        public int Id { get; }

#if EVENT_DEBUG
        public int SubscriberCount { get; } = 0;
        public long PublishCount { get; } = 0;
        public DateTime LastPublishTime { get; } = default;
        public T LastPayload { get; } = default;
#endif

        public TestGameEventMetadata(string nameSpace, string debugName)
        {
            NameSpace = nameSpace;
            DebugName = debugName;
            Id = HashCode.Combine(nameSpace, debugName);
        }

        public event EventCallback<T> OnPublished
        {
            add { }
            remove { }
        }

        public event AsyncEventCallback<T> OnPublishedAsync
        {
            add { }
            remove { }
        }

        public void Publish(in T eventArgs)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public Task PublishAsync(T eventArgs, CancellationToken ct = default)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public ISubscriptionHandle Subscribe(EventCallback<T> callback)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public ISubscriptionHandle SubscribeAsync(AsyncEventCallback<T> asyncCallback)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public void Unsubscribe(EventCallback<T> callback)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public void UnsubscribeAsync(AsyncEventCallback<T> asyncCallback)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

#if NET7_0_OR_GREATER
        public void operator +=(EventCallback<T> other)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public void operator +=(AsyncEventCallback<T> other)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public void operator -=(EventCallback<T> other)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }

        public void operator -=(AsyncEventCallback<T> other)
        {
            throw new NotSupportedException("Test metadata stub only.");
        }
#endif

        public void Dispose()
        {
        }
    }
}
