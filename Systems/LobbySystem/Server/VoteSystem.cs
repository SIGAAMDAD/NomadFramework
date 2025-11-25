/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Interfaces;
using NomadCore.Systems.EventSystem;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace NomadCore.Systems.LobbySystem.Server {
	/// <summary>
	/// A voting poll's type
	/// </summary>
	public enum VoteType : byte {
		/// <summary>
		/// We're force-kicking someone from the lobby.
		/// </summary>
		Kick,

		/// <summary>
		/// We're force-starting the game.
		/// </summary>
		StartGame,

		/// <summary>
		/// We're force-changing the map.
		/// </summary>
		ChangeMap,

		/// <summary>
		/// We're force-changing the gamemode.
		/// </summary>
		ChangeMode,

		Count
	};

	/*
	===================================================================================
	
	VoteSystem
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Constructs a voting system
	/// </remarks>
	/// <param name="manager">The <see cref="ServerManager"/> owner of the VoteSystem.</param>

	internal sealed class VoteSystem( ServerManager manager ) {
		public readonly struct VoteResult( CSteamID userId, bool result ) {
			public readonly CSteamID UserID = userId;
			public readonly bool Result = result;
		};
		private interface IVoteInfo {
			public VoteType Type { get; }
			public CSteamID InititatorID { get; }
			public ConcurrentDictionary<User, VoteResult> Results { get; }
		};
		private sealed class VoteKickInfo( CSteamID initiatorId, CSteamID targetId ) {
			public readonly CSteamID InitiatorId = initiatorId;
			public readonly CSteamID TargetId = targetId;
			public readonly ConcurrentDictionary<User, VoteResult> Results = new ConcurrentDictionary<User, VoteResult>();

			public readonly GameEvent End = new GameEvent( nameof( End ) );
		};
		private sealed class VoteInfo {
			public readonly CSteamID InitiatorID;
			public readonly object? Data;
			public readonly ConcurrentDictionary<LobbySystem.User, VoteResult> Results;
			public readonly VoteType Type;
			public readonly System.Threading.Thread PollThread;

			public readonly GameEvent End = new GameEvent( nameof( End ) );

			/*
			===============
			VoteInfo
			===============
			*/
			/// <summary>
			/// 
			/// </summary>
			/// <param name="initiatorId"></param>
			/// <param name="data"></param>
			/// <param name="type"></param>
			public VoteInfo( CSteamID initiatorId, object? data, VoteType type ) {
				InitiatorID = initiatorId;
				Data = data;
				Results = new ConcurrentDictionary<User, VoteResult>();
				Type = type;
				PollThread = new System.Threading.Thread( PollVotesProcess );
				PollThread.Start();
			}

			/*
			===============
			PollVoteProcess
			===============
			*/
			private void PollVotesProcess() {
				Stopwatch timer = new Stopwatch();
				while ( timer.Elapsed.Seconds < VOTE_DURATION || Results.Count < LobbySystem.LobbyManager.Current.MemberCount ) {
				}
			}
		};

		private const float VOTE_COOLDOWN = 60.0f;
		private const float VOTE_DURATION = 30.0f;

		private IVoteInfo ActivePoll;
		private readonly Dictionary<CSteamID, float> LastVoteStartTimes = new Dictionary<CSteamID, float>();

		private readonly ServerManager Manager = manager;

		/*
		===============
		InitiateVoteGameStart
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="initiatorId"></param>
		public void InitiateVoteGameStart( CSteamID initiatorId ) {
			/*
			if ( ActivePolls.ContainsKey( initiatorId ) ) {
				Console.PrintLine( $"VoteSystem.InitiateVoteGameStart: another vote is already active for user '{initiatorId}'" );
				return;
			}
			if ( LastVoteStartTimes.TryGetValue( initiatorId, out float startTime ) ) {
				float timeSince = startTime - Time.GetTicksMsec() / 1000.0f;
				if ( timeSince < VOTE_COOLDOWN ) {
					Console.PrintLine( "VoteSystem.InitiateVoteGameStart: You must wait another 60 seconds to initiate a new vote." );
					return;
				}
			}

			Console.PrintLine( $"VoteSystem.InitiateVoteGameStart: starting a StartGame voting poll (triggered by {initiatorId})..." );
			LastVoteStartTimes[ initiatorId ] = Time.GetTicksMsec() / 1000.0f;
			ActivePolls[ initiatorId ] = new VoteInfo( initiatorId, null, VoteType.StartGame );
			*/
		}

		/*
		===============
		InitiateVoteKick
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="initiatorId"></param>
		/// <param name="targetId"></param>
		public void InitiateVoteKick( CSteamID initiatorId, CSteamID targetId ) {
			/*
			if ( ActivePolls.ContainsKey( initiatorId ) ) {
				Console.PrintLine( $"VoteSystem.InitiateVoteKick: another vote is already active for user '{initiatorId}'" );
				return;
			}
			if ( LastVoteStartTimes.TryGetValue( initiatorId, out float startTime ) ) {
				float timeSince = startTime - Time.GetTicksMsec() / 1000.0f;
				if ( timeSince < VOTE_COOLDOWN ) {
					Console.PrintLine( "VoteSystem.InitiateVoteKick: You must wait another 60 seconds to initiate a new vote." );
					return;
				}
			}

			Console.PrintLine( $"VoteSystem.InitiateVoteKick: starting a votekick (triggered by {initiatorId}, targeting '{targetId}')..." );
			*/
		}

		/*
		===============
		CreatePoll
		===============
		*/
		private void CreatePoll( CSteamID initiatorId, object? data, VoteType type ) {
			/*
			LastVoteStartTimes[ initiatorId ] = Time.GetTicksMsec() / 1000.0f;
			ActivePolls[ initiatorId ] = new VoteInfo( initiatorId, data, type );
			ActivePolls[ initiatorId ].End.Subscribe( this, OnVotingPollEnded );
			*/
		}

		/*
		===============
		OnVotingPollEnded
		===============
		*/
		private void OnVotingPollEnded( in IGameEvent eventData, in IEventArgs args ) {
		}
	};
};