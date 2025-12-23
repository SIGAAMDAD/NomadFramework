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

using Nomad.Audio.Interfaces;

namespace Nomad.Audio.Fmod.ValueObjects
{
    /*
	===================================================================================

	FMODEventResource

	===================================================================================
	*/
    /// <summary>
    ///
    /// </summary>

    internal sealed record FMODEventResource(FMOD.Studio.EventDescription handle) : IEventResource
    {
        public bool IsValid => Handle.isValid();

        public readonly FMOD.Studio.EventDescription Handle = handle;

        /*
		===============
		Dispose
		===============
		*/
        public void Dispose()
        {
            Unload();
        }

        /*
		===============
		Unload
		===============
		*/
        /// <summary>
        /// Unloads and deallocates all event instances bound to this event description.
        /// </summary>
        public void Unload()
        {
            if (Handle.isValid())
            {
                Handle.unloadSampleData();
                Handle.releaseAllInstances();
                Handle.clearHandle();
            }
        }
    };
};
