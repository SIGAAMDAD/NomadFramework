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
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.ValueObjects {
	internal class SteamNetConnection {
		public SteamConnectionStatus Status => _status;
		private SteamConnectionStatus _status;

		public HSteamNetConnection Connection => _connection;
		private readonly HSteamNetConnection _connection = HSteamNetConnection.Invalid;

		public SteamNetConnection( HSteamNetConnection connection ) {
			_connection = connection;
			_status = SteamConnectionStatus.Opened;
		}

		public void SetStatus( SteamConnectionStatus status ) {
			switch ( status ) {
				case SteamConnectionStatus.Opened: {
						if ( _status != SteamConnectionStatus.Opened ) {
							if ( SteamNetworkingSockets.AcceptConnection( _connection ) != EResult.k_EResultOK ) {
								_status = SteamConnectionStatus.Connected;
							} else {
								Close( "Connection accept failed." );
							}
						} else {

						}
						break;
					}
				case SteamConnectionStatus.Closed:
					if ( _connection != HSteamNetConnection.Invalid ) {
						Close( "Client connection closed." );
					}
					break;
			}
			_status = status;
		}

		/*
		===============
		Close
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		private void Close( string message ) {
			SteamNetworkingSockets.CloseConnection( _connection, 0, message, false );
		}
	};
};
