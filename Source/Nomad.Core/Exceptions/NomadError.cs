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
using System.Reflection;
using System.Runtime.CompilerServices;
using Nomad.Core.Util.Attributes;

namespace Nomad.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NomadError : Exception
    {
        /// <summary>
        /// 
        /// </summary>
		public override string Message => GetMessage();
        
        /// <summary>
        /// The system where the exception was thrown.
        /// </summary>
        public string Module => _module;
        private readonly string _module;

        /// <summary>
        /// The time at which the exception was thrown.
        /// </summary>
        public DateTime TimeStap => _timeStamp;
        private readonly DateTime _timeStamp;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NomadError(string message)
            : base(message)
        {
            _module = GetAssemblyName();
            _timeStamp = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetMessage()
        {
            return $"FROM {_module} AT {_timeStamp}\n{base.Message}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetAssemblyName()
        {
            var assembly = Assembly.GetExecutingAssembly();

            NomadModule module = assembly.GetCustomAttribute<NomadModule>();
            if (module == null)
            {
                return "(UNKNOWN)";
            }
            return module.Name;
        }
    }
}