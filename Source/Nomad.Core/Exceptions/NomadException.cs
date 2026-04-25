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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Nomad.Core.Util.Attributes;

namespace Nomad.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class NomadException : Exception
    {
        /// <summary>
        /// UTC timestamp for when the exception object was created.
        /// </summary>
        public DateTimeOffset TimestampUtc { get; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// The system where the exception was thrown.
        /// </summary>
        public string Module { get; }

        /// <summary>
        /// Optional stable error identifier.
        /// </summary>
        public virtual string? ErrorCode => null;

        private string? DebuggerDisplay => $"{GetType().Name}: {Message} [{Module}]";

        /// <summary>
        /// 
        /// </summary>
        public NomadException()
            : base($"A NomadFramework exception has occurred.")
        {
            Module = ResolveNomadModuleInfo() ?? "(UNKNOWN)";

            InitializeMetadata();
        }

        public NomadException(string message)
            : base(message, null)
        {
            Module = ResolveNomadModuleInfo() ?? "(UNKNOWN)";

            InitializeMetadata();
        }

        public NomadException(string message, Exception? innerException)
            : base(message, innerException)
        {
            Module = ResolveNomadModuleInfo() ?? "(UNKNOWN)";

            InitializeMetadata();
        }

        public override string ToString()
        {
            var sb = new StringBuilder(256);

            sb.Append("FROM ");
            sb.Append(Module);
            sb.Append(" AT ");
            sb.Append(TimestampUtc.ToString("O"));
            sb.AppendLine();

            if (ErrorCode != null)
            {
                sb.Append("[ERROR CODE]: ").Append(ErrorCode).AppendLine();
            }

            sb.Append("[EXCEPTION TYPE]: ").Append(GetType().FullName).AppendLine();
            sb.Append("[MESSAGE]: ").Append(base.Message).AppendLine();

            if (InnerException != null)
            {
                sb.AppendLine("[INNER EXCEPTION]:");
                sb.AppendLine(InnerException.ToString());
            }

            sb.AppendLine("[STACKTRACE]:");
            sb.AppendLine(StackTrace ?? "(stack trace unavailable; exception has not been thrown yet)");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string? ResolveNomadModuleInfo()
        {
            var thisAssembly = typeof(NomadException).Assembly;

            // skip 1 to move past this method's own fame
            var st = new StackTrace(skipFrames: 1, fNeedFileInfo: true);
            var frames = st.GetFrames();
            if (frames == null)
            {
                return null;
            }

            for (int i = 0; i < frames.Length; i++)
            {
                var method = frames[i].GetMethod();
                if (method == null)
                {
                    continue;
                }

                var asm = method.Module.Assembly;
                if (asm == thisAssembly)
                {
                    continue;
                }

                var attribute = asm.GetCustomAttribute<NomadModule>();
                if (attribute != null)
                {
                    return attribute.Name;
                }
            }
            return null;
        }

        private void InitializeMetadata()
        {
            Data["Nomad.Module"] = Module;
            Data["Nomad.TimestampUtc"] = TimestampUtc.ToString("O");
            Data["Nomad.ExceptionType"] = GetType().FullName ?? GetType().Name;

            if (ErrorCode != null)
            {
                Data["Nomad.ErrorCode"] = ErrorCode;
            }
        }
    }
}
