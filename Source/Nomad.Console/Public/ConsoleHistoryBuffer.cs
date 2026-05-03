using System;
using System.Collections.Generic;
using Nomad.Console.Events;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.Events;

namespace Nomad.Console
{
    public sealed class ConsoleHistoryBuffer : IDisposable
    {
        private const int DEFAULT_CAPACITY = 256;

        private readonly IConsoleHistoryStore _store;

        [Event("Nomad.Console.Events")]
        [EventPayload("Text", typeof(string))]
        public IGameEvent<HistoryPrevEventArgs> HistoryPrev => _historyPreviousEvent;
        private readonly IGameEvent<HistoryPrevEventArgs> _historyPreviousEvent;

        [Event("Nomad.Console.Events")]
        [EventPayload("Text", typeof(string), Order = 1)]
        [EventPayload("EndReached", typeof(bool), Order = 2)]
        public IGameEvent<HistoryNextEventArgs> HistoryNext => _historyNextEvent;
        private readonly IGameEvent<HistoryNextEventArgs> _historyNextEvent;

        private readonly ISubscriptionHandle _historyPreviousRequested;
        private readonly ISubscriptionHandle _historyNextRequested;

        private readonly List<string> _history;

        private int _cursor;
        private string _scratchText = string.Empty;
        private bool _hasScratchText = false;
        private bool _isDisposed = false;

        public int Capacity { get; }
        public int Count => _history.Count;

        public ConsoleHistoryBuffer(
            IGameEventRegistryService eventRegistry,
            IConsoleHistoryStore store,
            int capacity = DEFAULT_CAPACITY
        )
        {
            ArgumentGuard.ThrowIfNull(eventRegistry);

            _store = store ?? throw new ArgumentNullException(nameof(store));
            Capacity = Math.Max(1, capacity);

            _history = new List<string>(Capacity);

            IReadOnlyList<string> loadedHistory = _store.Load();

            for (int i = 0; i < loadedHistory.Count; i++)
            {
                AppendLoadedLine(loadedHistory[i]);
            }

            _cursor = _history.Count;

            _historyPreviousEvent = eventRegistry.GetEvent<HistoryPrevEventArgs>(
                HistoryPrevEventArgs.Name,
                HistoryPrevEventArgs.NameSpace
            );

            _historyNextEvent = eventRegistry.GetEvent<HistoryNextEventArgs>(
                HistoryNextEventArgs.Name,
                HistoryNextEventArgs.NameSpace
            );

            _historyPreviousRequested = eventRegistry
                .GetEvent<HistoryPrevRequestedEventArgs>(
                    HistoryPrevRequestedEventArgs.Name,
                    HistoryPrevRequestedEventArgs.NameSpace
                )
                .Subscribe(OnHistoryPreviousRequested);

            _historyNextRequested = eventRegistry
                .GetEvent<HistoryNextRequestedEventArgs>(
                    HistoryNextRequestedEventArgs.Name,
                    HistoryNextRequestedEventArgs.NameSpace
                )
                .Subscribe(OnHistoryNextRequested);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Save();

                _historyNextRequested?.Dispose();
                _historyPreviousRequested?.Dispose();

                _history.Clear();
                _scratchText = string.Empty;
                _hasScratchText = false;
            }

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        public void Add(string text)
        {
            text = NormalizeHistoryText(text);

            if (text.Length == 0)
            {
                return;
            }

            if (_history.Count > 0 && string.Equals(_history[^1], text, StringComparison.Ordinal))
            {
                ResetCursor();
                return;
            }

            _history.Add(text);

            while (_history.Count > Capacity)
            {
                _history.RemoveAt(0);
            }

            ResetCursor();
            Save();
        }

        public void Clear()
        {
            _history.Clear();
            ResetCursor();
            Save();
        }

        public void Save()
        {
            _store.Save(_history);
        }

        private void OnHistoryPreviousRequested(in HistoryPrevRequestedEventArgs args)
        {
            if (_history.Count == 0)
            {
                return;
            }

            if (!_hasScratchText)
            {
                _scratchText = args.CurrentText ?? string.Empty;
                _hasScratchText = true;
            }

            if (_cursor > 0)
            {
                _cursor--;
            }

            var eventArgs = new HistoryPrevEventArgs(_history[_cursor]);
            _historyPreviousEvent.Publish(in eventArgs);
        }

        private void OnHistoryNextRequested(in HistoryNextRequestedEventArgs args)
        {
            if (_history.Count == 0)
            {
                var emptyArgs = new HistoryNextEventArgs(string.Empty, true);
                _historyNextEvent.Publish(in emptyArgs);
                return;
            }

            if (_cursor < _history.Count)
            {
                _cursor++;
            }

            if (_cursor >= _history.Count)
            {
                string text = _hasScratchText ? _scratchText : string.Empty;

                ResetCursor();

                var endArgs = new HistoryNextEventArgs(text, text.Length == 0);
                _historyNextEvent.Publish(in endArgs);
                return;
            }

            var eventArgs = new HistoryNextEventArgs(_history[_cursor], false);
            _historyNextEvent.Publish(in eventArgs);
        }

        private void AppendLoadedLine(string text)
        {
            text = NormalizeHistoryText(text);

            if (text.Length == 0)
            {
                return;
            }

            if (_history.Count > 0 && string.Equals(_history[^1], text, StringComparison.Ordinal))
            {
                return;
            }

            _history.Add(text);

            while (_history.Count > Capacity)
            {
                _history.RemoveAt(0);
            }
        }

        private void ResetCursor()
        {
            _cursor = _history.Count;
            _scratchText = string.Empty;
            _hasScratchText = false;
        }

        private static string NormalizeHistoryText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();
        }
    }
}
