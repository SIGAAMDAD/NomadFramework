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
using Nomad.Core.Engine.Services;
using Nomad.Core.Scene.GameObjects;
using UnityEngine;

namespace Nomad.EngineUtils.Private.Services {
    internal sealed class UnitySplitScreenService : ISplitScreenService {
        public void Apply( IGameObject worldRoot, IReadOnlyList<IGameObject> participants ) {
            ArgumentNullException.ThrowIfNull( worldRoot );
            ArgumentNullException.ThrowIfNull( participants );

            int playerCount = Mathf.Clamp( participants.Count, 0, 4 );
            var layout = GetLayout( playerCount );

            for ( int i = 0; i < playerCount; i++ ) {
                var camera = ResolveParticipantCamera( participants[ i ] );
                if ( camera == null ) {
                    continue;
                }

                camera.enabled = true;
                camera.rect = layout[ i ];
            }

            for ( int i = playerCount; i < participants.Count; i++ ) {
                var camera = ResolveParticipantCamera( participants[ i ] );
                if ( camera != null ) {
                    camera.enabled = false;
                }
            }
        }

        public void Clear() {
        }

        public void Dispose() {
            Clear();
            GC.SuppressFinalize( this );
        }

        private static Camera? ResolveParticipantCamera( IGameObject participant ) {
            if ( participant is Component component ) {
                return component.GetComponentInChildren<Camera>( true );
            }

            if ( participant is GameObject gameObject ) {
                return gameObject.GetComponentInChildren<Camera>( true );
            }

            return null;
        }

        private static Rect[] GetLayout( int playerCount ) {
            return playerCount switch {
                0 => Array.Empty<Rect>(),
                1 => [ new Rect( 0.0f, 0.0f, 1.0f, 1.0f ) ],
                2 => [
                    new Rect( 0.0f, 0.0f, 0.5f, 1.0f ),
                    new Rect( 0.5f, 0.0f, 0.5f, 1.0f )
                ],
                3 => [
                    new Rect( 0.0f, 0.5f, 1.0f, 0.5f ),
                    new Rect( 0.0f, 0.0f, 0.5f, 0.5f ),
                    new Rect( 0.5f, 0.0f, 0.5f, 0.5f )
                ],
                _ => [
                    new Rect( 0.0f, 0.5f, 0.5f, 0.5f ),
                    new Rect( 0.5f, 0.5f, 0.5f, 0.5f ),
                    new Rect( 0.0f, 0.0f, 0.5f, 0.5f ),
                    new Rect( 0.5f, 0.0f, 0.5f, 0.5f )
                ]
            };
        }
    }
}
