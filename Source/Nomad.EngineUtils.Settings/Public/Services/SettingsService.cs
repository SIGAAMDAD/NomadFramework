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
using Nomad.Core.CVars;

namespace Nomad.EngineUtils.Settings.Services
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    /// <typeparam name="IConfig"></typeparam>
    public abstract class SettingsService<TConfig, IConfig>
        where TConfig : IConfig, IEquatable<TConfig>
    {
        /// <summary>
        ///
        /// </summary>
        public IConfig Config => config;

        /// <summary>
        ///
        /// </summary>
        protected TConfig config;

        /// <summary>
        ///
        /// </summary>
        public bool IsModified => !_initial.Equals(config);

        /// <summary>
        ///
        /// </summary>
        protected readonly ICVarSystemService cvarSystem;

        private TConfig _initial;
        private readonly TConfig _default;

        /// <summary>
        ///
        /// </summary>
        /// <param name="cvarSystem"></param>
        public SettingsService(ICVarSystemService cvarSystem)
        {
            this.cvarSystem = cvarSystem;
            Load();
            _initial = config;
            _default = CreateDefault();
        }

        /// <summary>
        ///
        /// </summary>
        public void ResetToDefault()
        {
            config = _default;
        }

        /// <summary>
        ///
        /// </summary>
        public void Reset()
        {
            config = _initial;
        }

        /// <summary>
        ///
        /// </summary>
        public void Save()
        {
            SaveInternal();
            _initial = config;
        }

        /// <summary>
        ///
        /// </summary>
        public void Load()
        {
            config = CreateConfig();
        }

        /// <summary>
        ///
        /// </summary>
        protected abstract void SaveInternal();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected abstract TConfig CreateConfig();

        /// <summary>
        ///
        /// </summary>
        protected abstract TConfig CreateDefault();
    }
}
