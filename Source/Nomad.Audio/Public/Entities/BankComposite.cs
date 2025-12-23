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
using System;
using Nomad.Audio.Interfaces;
using Nomad.Audio.ValueObjects;
using Nomad.Core.Abstractions;

namespace Nomad.Audio.Entities
{
    /// <summary>
    ///
    /// </summary>

    public sealed class BankComposite(IEventCollection events, IBankResource resource, IBankMetadata metadata) : IAudioBank
    {
        public BankId Id => metadata.Id;
        public DateTime CreatedAt => metadata.CreatedAt;
        public DateTime? ModifiedAt => metadata.ModifiedAt;
        public int Version => metadata.Version;

        /// <summary>
        /// The bank's current state.
        /// </summary>
        public BankState Status
        {
            get => _status;
            set
            {
                if (value < BankState.Unloaded || value >= BankState.Count)
                {
                    return;
                }
                _status = value;
            }
        }
        private BankState _status = BankState.Unloaded;

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            resource.Dispose();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsEventInBank(EventId id)
        {
            return events.ContainsEvent(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IEntity<BankId>? other)
        {
            return other?.Id == Id;
        }
    };
};
