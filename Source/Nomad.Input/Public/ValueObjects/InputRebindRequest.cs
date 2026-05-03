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

namespace Nomad.Input.ValueObjects
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct InputRebindRequest
    {
        public string MappingName { get; }
        public string ActionId { get; }
        public int BindingIndex { get; }
        public InputRebindPart Part { get; }

        public InputRebindRequest(
            string mappingName,
            string actionId,
            int bindingIndex,
            InputRebindPart part = InputRebindPart.Whole)
        {
            MappingName = mappingName;
            ActionId = actionId;
            BindingIndex = bindingIndex;
            Part = part;
        }
    }
}
