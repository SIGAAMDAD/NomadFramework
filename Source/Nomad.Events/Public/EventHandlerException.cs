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
using Nomad.Core.Exceptions;

namespace Nomad.Events
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EventHandlerException : NomadException
    {
        public string EventName { get; }
        public Type ArgsType { get; }
        public string HandlerName { get; }
        public int HandlerIndex { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="argsType"></param>
        /// <param name="handlerName"></param>
        /// <param name="handlerIndex"></param>
        /// <param name="innerException"></param>
        public EventHandlerException(
            string eventName,
            Type argsType,
            string handlerName,
            int handlerIndex,
            Exception innerException
        )
            : base($"Subscriber '{handlerName}' failed while handling event '{eventName}'", innerException)
        {
            EventName = eventName;
            ArgsType = argsType;
            HandlerName = handlerName;
            HandlerIndex = handlerIndex;
        }
    }
}
