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

namespace Nomad.EngineUtils
{
    /// <summary>
    ///
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///
        /// </summary>
        public static class CVars
        {
            /// <summary>
            ///
            /// </summary>
            public const string SHADOW_ATLAS_SIZE = "r.Godot.ShadowAtlasSize";

            /// <summary>
            ///
            /// </summary>
            public const string SHADOW_FILTER_TYPE = "r.Godot.ShadowFilterType";

            /// <summary>
            ///
            /// </summary>
            public const string SHADOW_FILTER_SMOOTH = "r.Godot.ShadowFilterSmooth";

            /// <summary>
            ///
            /// </summary>
            public const string FORCE_VERTEX_SHADING = "r.Godot.ForceVertexShading";

            /// <summary>
            ///
            /// </summary>
            public const string LAMBERT_OVER_BURLEY = "r.Godot.LambertOverBurley";

            /// <summary>
            ///
            /// </summary>
            public const string BAKED_LIGHTS = "r.Godot.BakedLights";

            /// <summary>
            ///
            /// </summary>
            public const string SEPARATE_RENDERING_THREAD = "r.Godot.SeparateRenderingThread";
        }

        /// <summary>
        ///
        /// </summary>
        public static class Events
        {
            public const string NAMESPACE = "Nomad.EngineUtils";

            public const string BUTTON_CLICKED = NAMESPACE + ".ButtonClicked";

            public const string SLIDER_VALUE_SET = NAMESPACE + ".SliderValueSet";

            public const string UI_ELEMENT_FOCUSED = NAMESPACE + ".UIElementFocused";
            public const string UI_ELEMENT_UNFOCUSED = NAMESPACE + ".UIElementUnfocused";
        }
    }
}
