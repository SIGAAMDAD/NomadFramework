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

namespace Nomad.Save.ValueObjects
{
    /// <summary>
    /// The metadata of a persistent game state file. Stores relevant information for debugging, listing, and/or sorting.
    /// </summary>
    /// <remarks>
    /// Retrieving the metadata for all the save files currently available can be done with <see cref="Services.ISaveDataProvider.ListSaveFiles"/>.
    /// </remarks>
    /// <param name="FileName">The save file's relative name, includes the ".ngd" extension.</param>
    /// <param name="FileSize">The size of the save file in bytes.</param>
    /// <param name="LastAccessYear">The year the save file was last accessed.</param>
    /// <param name="LastAccessMonth">The month the save file was last accessed.</param>
    /// <param name="LastAccessDay">The day the save file was last accessed.</param>
    public record SaveFileMetadata(
        string FileName,
        long FileSize,
        int LastAccessYear,
        int LastAccessMonth,
        int LastAccessDay
    );
}
