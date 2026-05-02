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
using Nomad.Core.Events;

namespace Nomad.Core.CVars
{
    /// <summary>
    /// The generic interface for a Configurable-Variable, or CVar. Originates from the Quake engine brought up to .NET standards to make runtime testing much easier for all game developers.
    /// </summary>
    public interface ICVar
    {
        /// <summary>
        /// The CVar's name. This is a required element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The CVar's description. This is a required element.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The CVar's internal data type. This cannot be changed after creation.
        /// </summary>
        CVarType Type { get; }

        /// <summary>
        /// The CVar's permission structure. Various flags assigned at creation carry different requirements/responsibilities.
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
        /// Retrieves the CVar's 32-bit floating point value. If the CVar was not created as a 32-bit floating point value, an <see cref="InvalidCastException"/> will be thrown.
        /// </summary>
        /// <returns>The CVar's internal 32-bit floating point value.</returns>
        float GetDecimalValue();

        /// <summary>
        /// Retrieves the CVar's 32-bit signed integer value. If the CVar was not created as a signed 32-bit integer value, an <see cref="InvalidCastException"/> will be thrown.
        /// </summary>
        /// <returns>The CVar's internal 32-bit signed integer value.</returns>
        int GetIntegerValue();

        /// <summary>
        /// Retrieves the CVar's 32-bit unsigned integer value. If the CVar was not created as an unsigned 32-bit integer value, an <see cref="InvalidCastException"/> will be thrown.
        /// </summary>
        /// <returns>The CVar's internal 32-bit unsigned integer value.</returns>
        uint GetUIntegerValue();

        /// <summary>
        /// Retrieves the CVar's null-terminated string value. If the CVar was not created as null-terminated string value, an <see cref="InvalidCastException"/> will be thrown.
        /// </summary>
        /// <returns>The CVar's internal null-terminated string value.</returns>
        string? GetStringValue();

        /// <summary>
        /// Retrieves the CVar's boolean value. If the CVar was not created as a boolean value, an <see cref="InvalidCastException"/> will be thrown.
        /// </summary>
        /// <returns>returns the CVar's internal boolean value.</returns>
        bool GetBooleanValue();

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
    /// A typed generic CVar interface. Used for referencing CVars in code as seen in the examples provided below.
    /// <para>A CVar's type can only be one of the following:</para>
    /// <list type="bullet">
    ///     <item>
    ///         <term>Int32</term>
    ///         <description>A 32 bit signed integer value.</description>
    ///     </item>
    ///     <item>
    ///         <term>UInt32</term>
    ///         <description>A 32 bit unsigned integer value.</description>
    ///     </item>
    ///     <item>
    ///         <term>Boolean</term>
    ///         <description>An 8 bit boolean true-false value.</description>
    ///     </item>
    ///     <item>
    ///         <term>Decimal</term>
    ///         <description>A 32 bit floating-point value.</description>
    ///     </item>
    ///     <item>
    ///         <term>String</term>
    ///         <description>A null-terminated string value.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">The CVar's internal type is limited to the types listed in the <see cref="ICVar{T}"/>'s documentation.</typeparam>
    public interface ICVar<T> : ICVar
    {
        /// <summary>
        /// The CVar's current internal value. This can only hold values within the type restrictions of <see cref="CVarType"/>,
        /// and whenever this variable changes, the <see cref="ValueChanged"/> event is published.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// The value that the CVar was created with. This cannot change.
        /// </summary>
        T DefaultValue { get; }

        /// <summary>
        /// Event that triggers whenever <see cref="Value"/> changes. Contains the new value and the previous value.
        /// </summary>
        [Event("Nomad.Core.CVars", EventPayloadName = "CVarValueChangedEventArgs")]
        [EventPayload("OldValue", "T", Order = 1)]
        [EventPayload("NewValue", "T", Order = 2)]
        IGameEvent<CVarValueChangedEventArgs<T>> ValueChanged { get; }
    }
}
