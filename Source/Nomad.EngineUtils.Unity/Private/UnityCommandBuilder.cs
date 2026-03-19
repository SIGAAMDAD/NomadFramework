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
using System.Linq;
using System.Text;
using Nomad.Core.Console;
using Nomad.Core.Events;

namespace Nomad.EngineUtils.Private {
    /*
    ===================================================================================

    UnityCommandBuilder

    ===================================================================================
    */
    /// <summary>
    ///
    /// </summary>
    internal sealed class UnityCommandBuilder : ICommandBuilder {
        /// <summary>
        ///
        /// </summary>
        public int ArgumentCount => _arguments.Count;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<TextEnteredEventArgs> TextEntered => _textEntered;
        private readonly IGameEvent<TextEnteredEventArgs> _textEntered;

        private readonly List<string> _arguments = new List<string>();
        private readonly StringBuilder _commandBuilder = new StringBuilder( 1024 );

        private bool _isDisposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventFactory"></param>
        public UnityCommandBuilder( IGameEventRegistryService eventFactory ) {
            if ( eventFactory == null ) {
                throw new ArgumentNullException( nameof( eventFactory ) );
            }

            _textEntered = eventFactory.GetEvent<TextEnteredEventArgs>(
                Core.Constants.Events.Console.TEXT_ENTERED_EVENT,
                Core.Constants.Events.Console.NAMESPACE
            );
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose() {
            if ( _isDisposed ) {
                return;
            }

            _textEntered.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize( this );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetArgumentAt( int index ) {
            return _arguments[index];
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string[] GetArgs() {
            return _arguments.Skip( 1 ).ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        public void OnHistoryPrev( in HistoryPrevEventArgs args ) {
            ParseLineInput( args.Text );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        public void OnHistoryNext( in HistoryNextEventArgs args ) {
            if ( !args.EndReached ) {
                ParseLineInput( args.Text );
            } else {
                _arguments.Clear();
                _commandBuilder.Clear();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        internal void Submit( string text ) {
            if ( string.IsNullOrWhiteSpace( text ) ) {
                return;
            }

            ParseLineInput( text );
            _textEntered.Publish( new TextEnteredEventArgs( text ) );
        }

        private void ParseLineInput( string text ) {
            bool inQuotes = false;
            bool escaped = false;

            _arguments.Clear();
            _commandBuilder.Clear();

            for ( int i = 0; i < text.Length; i++ ) {
                char c = text[i];

                if ( escaped ) {
                    _commandBuilder.Append( c );
                    escaped = false;
                    continue;
                }

                switch ( c ) {
                    case '\\':
                        escaped = true;
                        continue;
                    case '"':
                        inQuotes = !inQuotes;
                        continue;
                    default:
                        if ( char.IsWhiteSpace( c ) && !inQuotes ) {
                            if ( _commandBuilder.Length > 0 ) {
                                _arguments.Add( _commandBuilder.ToString() );
                                _commandBuilder.Clear();
                            }
                            continue;
                        }
                        break;
                }

                _commandBuilder.Append( c );
            }

            if ( _commandBuilder.Length > 0 ) {
                _arguments.Add( _commandBuilder.ToString() );
            }
        }
    };
};
