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
using System.Threading;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.OnlineServices;
using Nomad.CVars;
using Nomad.OnlineServices.Steam.Private.Services.LobbyServices;
using Nomad.OnlineServices.Steam.Private.ValueObjects;
using Steamworks;

namespace Nomad.OnlineServices.Steam.Private.Entities {
	/*
	===================================================================================

	SteamLobbyInstance

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class SteamLobbyInstance : IDisposable {
		public HashSet<ulong> Members => _memberService.Members;

		public LobbyInfo Info => _info.Info;
		private readonly SteamLobbyData _info;

		private readonly SteamLobbyMemberService _memberService;
		private readonly Timer _updateTimer;

		private bool _isDisposed = false;

		/*
		===============
		SteamLobbyInstance
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="info"></param>
		/// <param name="cvarSystem"></param>
		/// <param name="eventFactory"></param>
		public SteamLobbyInstance( SteamLobbyData info, ICVarSystemService cvarSystem, IGameEventRegistryService eventFactory ) {
			_info = info;
			_memberService = new SteamLobbyMemberService( eventFactory );

			var updateInterval = cvarSystem.GetCVarOrThrow<int>( Constants.CVars.LOBBY_METADATA_FETCH_INTERVAL );
			_updateTimer = new Timer( OnUpdateTimerTimeout, null, TimeSpan.FromSeconds( updateInterval.Value ), TimeSpan.FromSeconds( updateInterval.Value ) );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			if ( !_isDisposed ) {
				_memberService?.Dispose();
				_updateTimer?.Dispose();
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		/*
		===============
		Leave
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Leave() {
			SteamMatchmaking.LeaveLobby( _info.Id );
		}

		/*
		===============
		OnUpdateTimerTimeout
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="state"></param>
		private void OnUpdateTimerTimeout( object? state ) {
			lock ( _info ) {
				_info.Update();
			}
		}
	};
};
