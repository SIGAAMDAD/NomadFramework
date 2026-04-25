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
using Nomad.Core.Exceptions;

namespace Nomad.Events
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EventPublishException : NomadException
    {
        public string EventName { get; }
        public Type ArgsType { get; }
        public IReadOnlyList<EventHandlerException> HandlerExceptions { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="argsType"></param>
        /// <param name="handlerExceptions"></param>
        public EventPublishException(string eventName, Type argsType, IReadOnlyList<EventHandlerException> handlerExceptions)
            : base(CreateMessage(eventName, argsType, handlerExceptions))
        {
        }

        private static string CreateMessage(string eventName, Type argsType, IReadOnlyList<EventHandlerException> failures)
        {
            return $"Event '{eventName}' published with {failures.Count} subscriber exception(s). ArgsType={argsType.Name}";
        }
    }
}