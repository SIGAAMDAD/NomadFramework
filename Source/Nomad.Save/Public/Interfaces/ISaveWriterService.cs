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
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Interfaces
{
    /// <summary>
    /// A service for writing persistent game/save data to disk.
    /// </summary>
    /// <example>
    /// Here is an example of how you would utilize this to write some (for example) player data to a save file:
    /// <code lang="csharp">
    /// <![CDATA[
    /// public class Player {
    ///     
    /// };
    /// ]]>
    /// </code>
    /// </example> 
    public interface ISaveWriterService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        ISaveSectionWriter AddSection(string sectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="gameVersion"></param>
        internal void BeginSave(string filepath, GameVersion gameVersion);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameVersion"></param>
        internal void EndSave(GameVersion gameVersion);
    }
}
