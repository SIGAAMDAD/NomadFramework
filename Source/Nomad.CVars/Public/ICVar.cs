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

using Nomad.Core.Events;

namespace Nomad.CVars
{
    /// <summary>
    ///
    /// </summary>
    public interface ICVar
    {
        /// <summary>
        ///
        /// </summary>
        string Name { get; }

        /// <summary>
        ///
        /// </summary>
        string Description { get; }

        /// <summary>
        ///
        /// </summary>
        CVarType Type { get; }

        /// <summary>
        ///
        /// </summary>
        CVarFlags Flags { get; }

        /// <summary>
        /// <c>true</c> if the cvar will be saved and loaded from the configuration file.
        /// </summary>
        bool IsSaved { get; }

        /// <summary>
        /// <c>true</c> if the cvar's value cannot be changed after creation.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// <c>true</c> if the cvar was created from the console or by the user.
        /// </summary>
        bool IsUserCreated { get; }

        /// <summary>
        /// <c>true</c> if the cvar cannot be shown in the console.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        float GetDecimalValue();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        int GetIntegerValue();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        uint GetUIntegerValue();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        string? GetStringValue();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        bool GetBooleanValue();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetValue<T>();

        /// <summary>
        ///
        /// </summary>
        void Reset();

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        void SetFromString(string value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        void SetDecimalValue(float value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        void SetIntegerValue(int value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        void SetUIntegerValue(uint value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        void SetBooleanValue(bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        void SetStringValue(string value);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICVar<T> : ICVar
    {
        /// <summary>
        ///
        /// </summary>
        T Value { get; set; }

        /// <summary>
        ///
        /// </summary>
        T DefaultValue { get; }

        /// <summary>
        ///
        /// </summary>
        IGameEvent<CVarValueChangedEventArgs<T>> ValueChanged { get; }
    }
}
