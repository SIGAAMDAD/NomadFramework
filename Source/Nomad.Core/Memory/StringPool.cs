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

using Nomad.Core.Exceptions;
using Nomad.Core.Util;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Nomad.Core.Memory
{
    /// <summary>
    ///
    /// </summary>
    public sealed class StringPool
    {
        private readonly ConcurrentDictionary<string, ulong> _stringToIds = new ConcurrentDictionary<string, ulong>();
        private readonly ConcurrentDictionary<ulong, string> _idToString = new ConcurrentDictionary<ulong, string>();
        private ulong _nextId = 1; // 0 reserved for empty
        private readonly object _newStringLock = new object();

        private static readonly StringPool _global = new();

        /// <summary>
        /// Retrieves an interned string's UTF-16 representation.
        /// </summary>
        /// <remarks>
        /// If a lookup is attempted on a string that hasn't been interned yet, a <see cref="StringNotInternedException"/> exception will be thrown.
        /// </remarks>
        /// <param name="str">The interned string representation.</param>
        /// <returns>Returns the interned string if it exists</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FromInterned(InternString str)
        {
            return str == InternString.Empty ?
                    string.Empty
                :
                    _global._idToString.TryGetValue(str, out string? s) ? s : throw new StringNotInternedException((ulong)str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetString(InternString str, out string? value)
        {
            if (str == InternString.Empty)
            {
                value = string.Empty;
                return true;
            }
            return _global._idToString.TryGetValue(str, out value);
        }

        /// <summary>
        /// Adds/retrieves an interned string from the global string pool.
        /// </summary>
        /// <param name="str">The string to intern.</param>
        /// <returns>A new intern string or the retrieved cache value.</returns>
        public static InternString Intern(string? str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return InternString.Empty;
            }
            if (_global._stringToIds.TryGetValue(str, out ulong existingId))
            {
                return new InternString(existingId);
            }

            lock (_global._newStringLock)
            {
                if (_global._stringToIds.TryGetValue(str, out existingId))
                {
                    return new InternString(existingId);
                }
                ulong newId = _global._nextId++;
                _global._stringToIds[str] = newId;
                _global._idToString[newId] = str;
                return new InternString(newId);
            }
        }

        /// <summary>
        /// Clears all string pool values and resets all interned strings.
        /// </summary>
        /// <remarks>
        /// All instances of <see cref="InternString"/> become invalidated after calling this, they must be interned again to not throw a <see cref="StringNotInternedException"/>.
        /// </remarks>
        public static void Clear()
        {
            lock (_global._newStringLock)
            {
                _global._idToString.Clear();
                _global._stringToIds.Clear();
                _global._nextId = 1;
            }
        }
    }
}
