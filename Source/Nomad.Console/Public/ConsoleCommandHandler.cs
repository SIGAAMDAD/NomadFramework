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
using System.Collections.Generic;
using Nomad.Console.Events;
using Nomad.Core.Events;

namespace Nomad.Console
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConsoleCommandHandler : IDisposable
    {
        private const int MAX_QUEUED_LINES = 64;

        private readonly IGameEventRegistryService _eventRegistry;
        private readonly IConsoleCommandParser _parser;
        private readonly ConsoleHistoryBuffer _history;

        [Event("Nomad.Console.Events", PayloadName = "ConsoleCommandEventArgs")]
        [EventPayload("Command", typeof(string), Order = 1)]
        [EventPayload("RawText", typeof(string), Order = 2)]
        [EventPayload("Arguments", typeof(string[]), Order = 3)]
        [EventPayload("CommandIndex", typeof(int), Order = 4)]
        public IGameEvent<ConsoleCommandEventArgs> CommandDispatched => _commandDispatchedEvent;
        private readonly IGameEvent<ConsoleCommandEventArgs> _commandDispatchedEvent;

        [Event("Nomad.Console.Events", PayloadName = "ConsoleCommandParseErrorEventArgs")]
        [EventPayload("Text", typeof(string), Order = 1)]
        [EventPayload("Error", typeof(string), Order = 2)]
        public IGameEvent<ConsoleCommandParseErrorEventArgs> CommandParseError => _commandParseErrorEvent;
        private readonly IGameEvent<ConsoleCommandParseErrorEventArgs> _commandParseErrorEvent;

        private readonly ISubscriptionHandle _textSubmitted;

        private readonly Queue<string> _lineQueue = new Queue<string>(MAX_QUEUED_LINES);
        private readonly List<ConsoleCommandInvocation> _commandBuffer = new List<ConsoleCommandInvocation>(64);

        private bool _isDispatching = false;
        private bool _isDisposed = false;

        public ConsoleCommandHandler(
            IGameEventRegistryService eventRegistry,
            IConsoleCommandParser parser,
            ConsoleHistoryBuffer history
        )
        {
            _eventRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _history = history ?? throw new ArgumentNullException(nameof(history));

            _commandDispatchedEvent = _eventRegistry.GetEvent<ConsoleCommandEventArgs>(
                ConsoleCommandEventArgs.Name,
                ConsoleCommandEventArgs.NameSpace
            );

            _commandParseErrorEvent = _eventRegistry.GetEvent<ConsoleCommandParseErrorEventArgs>(
                ConsoleCommandParseErrorEventArgs.Name,
                ConsoleCommandParseErrorEventArgs.NameSpace
            );

            _textSubmitted = _eventRegistry
                .GetEvent<TextEnteredEventArgs>(
                    TextEnteredEventArgs.Name,
                    TextEnteredEventArgs.NameSpace
                )
                .Subscribe(OnTextSubmitted);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _textSubmitted?.Dispose();

                _lineQueue.Clear();
                _commandBuffer.Clear();
            }

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        public void ExecuteText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            text = text.Trim();

            _history.Add(text);

            if (_lineQueue.Count >= MAX_QUEUED_LINES)
            {
                PublishParseError(text, "Console command queue overflow.");
                return;
            }

            _lineQueue.Enqueue(text);

            if (!_isDispatching)
            {
                DrainQueue();
            }
        }

        private void OnTextSubmitted(in TextEnteredEventArgs args)
        {
            ExecuteText(args.Text);
        }

        private void DrainQueue()
        {
            _isDispatching = true;

            try
            {
                while (_lineQueue.Count > 0)
                {
                    string text = _lineQueue.Dequeue();

                    if (!_parser.TryParse(text, _commandBuffer, out string error))
                    {
                        PublishParseError(text, error);
                        continue;
                    }

                    for (int i = 0; i < _commandBuffer.Count; i++)
                    {
                        var command = _commandBuffer[i];
                        var eventArgs = new ConsoleCommandEventArgs(
                            command.Name,
                            command.RawText,
                            command.Arguments,
                            command.CommandIndex
                        );

                        _eventRegistry
                            .GetEvent<ConsoleCommandEventArgs>(
                                command.Name,
                                ConsoleCommandEventArgs.NameSpace
                            )
                            .Publish(in eventArgs);

                        _commandDispatchedEvent.Publish(in eventArgs);
                    }
                }
            }
            finally
            {
                _isDispatching = false;
            }
        }

        private void PublishParseError(string text, string error)
        {
            var eventArgs = new ConsoleCommandParseErrorEventArgs(text, error);
            _commandParseErrorEvent.Publish(in eventArgs);
        }
    };
};
