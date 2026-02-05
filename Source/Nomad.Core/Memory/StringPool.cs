/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Nomad.Core.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nomad.Core.Memory
{
    /// <summary>
    ///
    /// </summary>
    public sealed class StringPool : IDisposable
    {
        private readonly Dictionary<string, int> _stringToIds = new Dictionary<string, int>(2048);
        private readonly Dictionary<int, string> _idToString = new Dictionary<int, string>(2048);

        [ThreadStatic]
        private static StringPool? _currentStringPool;

        private static StringPool _current => _currentStringPool ??= new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Returns the interned string if it exists, null if not found.</returns>
        public static string? FromInterned(in InternString str)
        {
            return _current._idToString.TryGetValue(str.GetHashCode(), out string? value) ? value : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static InternString Intern(ReadOnlySpan<char> str)
        {
            if (str.IsEmpty)
            {
                return InternString.Empty;
            }
            string converted = new string(str);
#if !USE_COMPATIBILITY_EXTENSIONS
            ref int id = ref CollectionsMarshal.GetValueRefOrAddDefault(_current._stringToIds, converted, out bool exists);
            if (!exists)
#else
            if (!_current._stringToIds.TryGetValue(converted, out int id))
#endif
            {
                id = converted.GetHashCode();
                _current._idToString[id] = converted;
            }
            return new InternString(id);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            _stringToIds.Clear();
            _idToString.Clear();
            _currentStringPool = null;
        }
    };
};
