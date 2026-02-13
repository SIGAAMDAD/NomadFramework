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
using System.Collections.Generic;
using System.Diagnostics;

namespace Nomad.Core.Utilities.Tweens
{
    /*
	===================================================================================

	Tween

	===================================================================================
	*/
    /// <summary>
    ///
    /// </summary>

    public class Tween : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public float ElapsedTime => (float)_stopwatch.Elapsed.TotalSeconds;

        /// <summary>
        /// 
        /// </summary>
        public float Progress => Math.Clamp((ElapsedTime - _delay) / _duration, 0f, 1f);

        /// <summary>
        /// 
        /// </summary>
        public bool IsPlaying => _isPlaying;

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleted => Progress >= 1f;

        private readonly List<ITweenProperty> _properties = new List<ITweenProperty>();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private float _duration;
        private IEasingFunction _easing = Easing.Linear;
        private Action _onCompleted;
        private bool _isPlaying;
        private float _delay;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _properties.Clear();
            _easing = null;
            _onCompleted = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="easing"></param>
        /// <returns></returns>
        public Tween SetEase(IEasingFunction easing)
        {
            _easing = easing;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public Tween SetDelay(float delay)
        {
            _delay = Math.Max(0, delay);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Tween OnComplete(Action action)
        {
            _onCompleted = action;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            if (_isPlaying)
            {
                return;
            }

            _isPlaying = true;
            _stopwatch.Restart();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _stopwatch.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (!_isPlaying || ElapsedTime < _delay)
            {
                return;
            }

            float t = _easing.Calculate(Progress);

            for (int i = 0; i < _properties.Count; i++)
            {
                _properties[i].Update(t);
            }

            if (IsCompleted)
            {
                _onCompleted?.Invoke();
                Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public Tween TweenProperty<T>(Action<T> setter, T start, T end, float duration)
        {
            _duration = Math.Max(_duration, duration);

            if (typeof(T) == typeof(float))
            {
                _properties.Add(new FloatTweenProperty(
                    setter as Action<float>,
                    Convert.ToSingle(start),
                    Convert.ToSingle(end),
                    duration
                ));
            }
            else if (typeof(T) == typeof(int))
            {
                _properties.Add(new IntTweenProperty(
                    setter as Action<int>,
                    Convert.ToInt32(start),
                    Convert.ToInt32(end),
                    duration
                ));
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        private interface ITweenProperty
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="t"></param>
            void Update(float t);
        }

        private sealed class FloatTweenProperty : ITweenProperty
        {
            private readonly Action<float> _setter;
            private readonly float _start;
            private readonly float _end;
            private readonly float _duration;

            public FloatTweenProperty(Action<float> setter, float start, float end, float duration)
            {
                _setter = setter;
                _start = start;
                _end = end;
                _duration = duration;
            }

            /*
			===============
			Update
			===============
			*/
            public void Update(float t)
            {
                float value = _start + (_end - _start) * t;
                _setter?.Invoke(value);
            }
        }

        private sealed class IntTweenProperty : ITweenProperty
        {
            private readonly Action<int> _setter;
            private readonly int _start;
            private readonly int _end;
            private readonly float _duration;

            public IntTweenProperty(Action<int> setter, int start, int end, float duration)
            {
                _setter = setter;
                _start = start;
                _end = end;
                _duration = duration;
            }

            /*
			===============
			Update
			===============
			*/
            public void Update(float t)
            {
                int value = (int)(_start + (_end - _start) * t);
                _setter?.Invoke(value);
            }
        }
    }
}
