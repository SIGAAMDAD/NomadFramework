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

namespace Nomad.Core.Utilities.Tweens
{
    /*
	===================================================================================

	Easing

	===================================================================================
	*/
    /// <summary>
    ///
    /// </summary>

    public static class Easing
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction Linear = new LinearEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction QuadIn = new QuadInEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction QuadOut = new QuadOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction QuadInOut = new QuadInOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction CubicIn = new CubicInEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction CubicOut = new CubicOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction CubicInOut = new CubicInOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction SineIn = new SineInEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction SineOut = new SineOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction SineInOut = new SineInOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction BackIn = new BackInEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction BackOut = new BackOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction BackInOut = new BackInOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction BounceIn = new BounceInEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction BounceOut = new BounceOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction BounceInOut = new BounceInOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction ElasticIn = new ElasticInEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction ElasticOut = new ElasticOutEasing();

        /// <summary>
        /// 
        /// </summary>
        public static readonly IEasingFunction ElasticInOut = default;

        /// <summary>
        /// 
        /// </summary>
        private readonly struct LinearEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => t;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct QuadInEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => t * t;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct QuadOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => 1 - (1 - t) * (1 - t);
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct QuadInOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private readonly struct CubicInEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => t * t * t;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct CubicOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => 1 - MathF.Pow(1 - t, 3);
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct CubicInOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => t < 0.5f ? 4 * t * t * t : 1 - MathF.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct SineInEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => 1 - MathF.Cos(t * MathF.PI / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct SineOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => MathF.Sin(t * MathF.PI / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct SineInOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => -(MathF.Cos(MathF.PI * t) - 1) / 2;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private readonly struct BackInEasing : IEasingFunction
        {
            private const float c1 = 1.70158f;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => (c1 + 1) * t * t * t - c1 * t * t;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private readonly struct BackOutEasing : IEasingFunction
        {
            private const float c1 = 1.70158f;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => 1 + (c1 + 1) * MathF.Pow(t - 1, 3) + c1 * MathF.Pow(t - 1, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        private class BackInOutEasing : IEasingFunction
        {
            private const float c1 = 1.70158f;
            private const float c2 = c1 * 1.525f;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t)
            {
                return t < 0.5f
                    ? MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2) / 2
                    : (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct BounceInEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => 1 - BounceOutEasing.Instance.Calculate(1 - t);
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct BounceOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly BounceOutEasing Instance = new BounceOutEasing();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t)
            {
                const float n1 = 7.5625f;
                const float d1 = 2.75f;

                if (t < 1 / d1) return n1 * t * t;
                if (t < 2 / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
                if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct BounceInOutEasing : IEasingFunction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t) => t < 0.5f
                ? (1 - BounceOutEasing.Instance.Calculate(1 - 2 * t)) / 2
                : (1 + BounceOutEasing.Instance.Calculate(2 * t - 1)) / 2;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct ElasticInEasing : IEasingFunction
        {
            private const float c4 = (2 * MathF.PI) / 3;
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t)
            {
                return t == 0 ? 0 :
                       t == 1 ? 1 :
                       -MathF.Pow(2, 10 * t - 10) * MathF.Sin((t * 10 - 10.75f) * c4);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct ElasticOutEasing : IEasingFunction
        {
            private const float c4 = (2 * MathF.PI) / 3;
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t)
            {
                return t == 0 ? 0 :
                       t == 1 ? 1 :
                       MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * c4) + 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly struct ElasticInOutEasing : IEasingFunction
        {
            private const float c5 = (2 * MathF.PI) / 4.5f;
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public float Calculate(float t)
            {
                return t == 0 ? 0 :
                       t == 1 ? 1 :
                       t < 0.5f
                           ? -(MathF.Pow(2, 20 * t - 10) * MathF.Sin((20 * t - 11.125f) * c5)) / 2
                           : MathF.Pow(2, -20 * t + 10) * MathF.Sin((20 * t - 11.125f) * c5) / 2 + 1;
            }
        }
    }
}
