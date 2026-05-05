using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.Events;

namespace Nomad.Events.Tests
{
    public class MockGameEvent<T> : IGameEvent<T> where T : struct
    {
        private readonly object _lock = new object();
        private readonly List<T> _publishedArgs = new List<T>();
        private readonly Action<T>? _onPublish;

        public MockGameEvent(Action<T>? onPublish = null)
        {
            _onPublish = onPublish;
        }

        public string DebugName { get; set; } = string.Empty;
        public string NameSpace { get; set; } = string.Empty;
        public int Id { get; set; }

        public IReadOnlyList<T> PublishedArgs
        {
            get
            {
                lock (_lock)
                {
                    return _publishedArgs.ToArray();
                }
            }
        }

        public long PublishCount
        {
            get
            {
                lock (_lock)
                {
                    return _publishedArgs.Count;
                }
            }
        }

        public T LastPayload => throw new NotImplementedException();
        public int SubscriberCount => throw new NotImplementedException();
        public DateTime LastPublishTime => throw new NotImplementedException();

        public event EventCallback<T>? OnPublished;
        public event AsyncEventCallback<T>? OnPublishedAsync;

        public void Publish(in T args)
        {
            lock (_lock)
            {
                _publishedArgs.Add(args);
            }

            _onPublish?.Invoke(args);
        }

        public Task PublishAsync(T args, CancellationToken ct = default)
        {
            Publish(args);
            return Task.CompletedTask;
        }

        public ISubscriptionHandle Subscribe(EventCallback<T> callback) => throw new NotImplementedException();
        public ISubscriptionHandle SubscribeAsync(AsyncEventCallback<T> callback) => throw new NotImplementedException();
        public void Unsubscribe(EventCallback<T> callback) => throw new NotImplementedException();
        public void UnsubscribeAsync(AsyncEventCallback<T> callback) => throw new NotImplementedException();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
