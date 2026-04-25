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

namespace Nomad.Core.Util
{
    /// <summary>
    /// Represents an error that occurred during a load or initialization operation.
    /// </summary>
    /// <remarks>
    /// This record implements the <see cref="IError"/> interface and is specifically designed for errors that occur
    /// when loading resources, assets, configurations, or other data. Use the <see cref="Create"/> factory method to instantiate errors.
    /// </remarks>
    /// <param name="Message">A descriptive message explaining what failed to load.</param>
    /// <param name="Type">The classification of the error type. Defaults to <see cref="ErrorType.NotFound"/>.</param>
    public record LoadError(
        string Message,
        ErrorType Type = ErrorType.NotFound
    ) : IError
    {
        /// <summary>
        /// Creates a new instance of <see cref="LoadError"/> with the specified message and type.
        /// </summary>
        /// <param name="message">A descriptive message explaining what failed to load.</param>
        /// <param name="type">The classification of the error type. Defaults to <see cref="ErrorType.NotFound"/>.</param>
        /// <returns>A new <see cref="LoadError"/> instance.</returns>
        public static LoadError Create(string message, ErrorType type = ErrorType.NotFound)
        {
            return new LoadError(message, type);
        }
    }
}
