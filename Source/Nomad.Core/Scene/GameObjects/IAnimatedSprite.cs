/*
===========================================================================
The Nomad MPLv2 Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Nomad.Core.Engine.Assets;

namespace Nomad.Core.Scene.GameObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnimatedSprite : IGameObject
    {
        /// <summary>
        /// The current animation.
        /// </summary>
        IAsset? Animation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        AnimationState State { get; }

        /// <summary>
        /// 
        /// </summary>
        int CurrentFrame { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float Speed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        void Stop();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        void Play(IAsset? animation);

        /// <summary>
        /// 
        /// </summary>
        void Pause();
    }
}
