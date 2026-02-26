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
using Nomad.Core.Util;
using Nomad.Save.Exceptions;

namespace Nomad.Save.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface ISaveSectionWriter : IDisposable
    {
        /// <summary>
        /// The section's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The number of fields currently added to the section.
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Adds a new field with the data <paramref name="fieldId"/> and <paramref name="value"/>.
        /// </summary>
        /// <remarks>
        /// If a similar field was added to this with the same name and type, a <see cref="DuplicateFieldException"/> exception will be thrown.
        /// </remarks>
        /// <example>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldId">The field's unique id.</param>
        /// <param name="value">The field's value. Must be a valid <see cref="AnyType"/> to avoid an </param>
        void AddField<T>(string fieldId, T value);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        bool HasField<T>(string fieldId);
    }
}
