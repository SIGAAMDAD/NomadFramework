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
using Godot;
using Nomad.Core.Events;

namespace Nomad.GodotServer.Rendering.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface IEntitySystemService : IDisposable
    {
        IRenderEntity? CreateEntity(CanvasItem item);
        IAnimationEntity? CreateAnimator(CanvasItem item, IGameEventRegistryService eventFactory);
        void Update(float delta);
    }
}
