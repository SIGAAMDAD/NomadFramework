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

#if TEMPLATE
namespace Nomad.EngineUtils.TemplateClasses
{
    [EngineBaseClass("Node", "MonoBehaviour")]
    public abstract partial class EngineNode
    {
        public abstract void Init();
        public abstract void Shutdown();
        public abstract void Update(float delta);
        public abstract void FixedUpdate(float delta);
    }
}
#endif