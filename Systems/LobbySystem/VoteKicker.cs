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

using NomadCore.Systems.ConsoleSystem;
using NomadCore.Systems.EventSystem;
using Godot;
using NomadCore.Systems.Steam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NomadCore.Interfaces;

namespace NomadCore.Systems.LobbySystem {
	/*
	===================================================================================
	
	VoteKicker
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class VoteKicker {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct VoteKickStartedEventData( CSteamID targetId, CSteamID initiatorId ) : IEventArgs {
			public readonly ulong TargetID = (ulong)targetId;
			public readonly ulong InitiatorID = (ulong)initiatorId;
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		private readonly struct VoteKickResult( CSteamID voterId, bool voteResult ) : IEventArgs {
			public readonly ulong VoterID = (ulong)voterId;
			public readonly bool VoteResult = voteResult;
		};
		private readonly struct KickData {
			public readonly float StartTime = 0.0f;
			public readonly string? Reason;
			public readonly CSteamID TargetID = CSteamID.Nil;
			public readonly CSteamID InitiatorID = CSteamID.Nil;

			/*
			===============
			KickData
			===============
			*/
			public KickData( string? reason, CSteamID targetId, CSteamID initiatorId ) {
				ArgumentException.ThrowIfNullOrEmpty( reason );

				Reason = reason;
				TargetID = targetId;
				InitiatorID = initiatorId;
				StartTime = Time.GetTicksMsec() / 1000.0f;
			}
		};

		public const float VOTE_KICK_DURATION = 30.0f;
		public const float VOTE_KICK_COOLDOWN = 300.0f;

		private KickData Internal;

		public bool Completed = false;
		public readonly Dictionary<CSteamID, bool> Votes = new Dictionary<CSteamID, bool>( LobbyManager.MAX_LOBBY_MEMBERS );

		private readonly Dictionary<CSteamID, float> VoteKickCooldowns = new Dictionary<CSteamID, float>();

		private static readonly ConsoleCommand VoteKick = new ConsoleCommand( nameof( VoteKick ), OnVoteKickStarted, "Initiates a vote kick of the provided target." );

		public static readonly GameEvent VoteKickStarted = new GameEvent( nameof( VoteKickStarted ) );

		/*
		===============
		OnVoteKickStarted
		===============
		*/
		private static void OnVoteKickStarted( in CommandLine.CommandExecutedEventData args ) {
			if ( !LobbyManager.Current.Id.IsLobby() ) {
				ConsoleSystem.Console.PrintError( $"VoteKicker.OnVoteKickStarted: not in a valid lobby!" );
				return;
			}
			if ( args.Arguments.Length < 2 ) {
				ConsoleSystem.Console.PrintError( $"VoteKicker.OnVoteKickStarted: you must provide a targetUser and the reason for kicking." );
				return;
			}

			string targetUser = args.Arguments[ 0 ];
			if ( targetUser == null || targetUser.Length == 0 ) {
				ConsoleSystem.Console.PrintError( $"VoteKicker.OnVoteKickStarted: you must provide a valid targetUser to kick!" );
				return;
			}
			CSteamID targetId = CSteamID.Nil;
			for ( int i = 0; i < LobbyManager.Current.MemberCount; i++ ) {
				if ( LobbyManager.Current.Players[ i ].UserName == targetUser ) {
					targetId = LobbyManager.Current.Players[ i ].UserID;
					break;
				}
			}
			if ( targetId == CSteamID.Nil ) {
				ConsoleSystem.Console.PrintLine( $"VoteKicker.StartVoteKick: no user named '{targetUser}' in current lobby." );
				return;
			}
		}

		/*
		===============
		StartVoteKick
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetUser"></param>
		/// <param name="reason"></param>
		private void StartVoteKick( string? targetUser, string? reason ) {
			ArgumentException.ThrowIfNullOrEmpty( targetUser );
			ArgumentException.ThrowIfNullOrEmpty( reason );
			ArgumentNullException.ThrowIfNull( LobbyManager.Current );

			if ( !LobbyManager.Current.Id.IsLobby() ) {
				ConsoleSystem.Console.PrintError( $"VoteKicker.StartVoteKick: not in a valid lobby!" );
				return;
			}

			CSteamID targetId = CSteamID.Nil;
			for ( int i = 0; i < LobbyManager.Current.MemberCount; i++ ) {
				if ( LobbyManager.Current.Players[ i ].UserName == targetUser ) {
					targetId = LobbyManager.Current.Players[ i ].UserID;
					break;
				}
			}
			if ( targetId == CSteamID.Nil ) {
				ConsoleSystem.Console.PrintLine( $"VoteKicker.StartVoteKick: no user named '{targetUser}' in current lobby." );
				return;
			}

			// can't vote yours truly out
			if ( targetId == SteamManager.VIP_ID ) {
				ConsoleSystem.Console.PrintError( "The AUDACITY of this BIATCH!" );
				return;
			}

			ConsoleSystem.Console.PrintLine( $"VoteKicker.StartVoteKick: vote kick initiated by '{SteamManager.UserName}' targeting '{targetUser}' because of '{reason}'." );

			Internal = new KickData( reason, (CSteamID)(ulong)SteamManager.SteamID, targetId );
			Votes[ (CSteamID)(ulong)SteamManager.SteamID ] = true;
			VoteKickCooldowns[ (CSteamID)(ulong)SteamManager.SteamID ] = Internal.StartTime;

			BroadcastVoteKickStart();
		}

		/*
		===============
		CastVote
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		/// <param name="vote"></param>
		public void CastVote( CSteamID senderId, bool vote ) {
			// can't vote for yourself
			if ( SteamManager.SteamID == (ulong)senderId ) {
				return;
			}
			// already voted
			if ( !Votes.ContainsKey( (CSteamID)(ulong)SteamManager.SteamID ) ) {
				return;
			}

			Votes[ (CSteamID)(ulong)SteamManager.SteamID ] = vote;

			if ( vote ) {
				Server.CommandManager.SendCommand( Server.CommandType.VoteKickResponse_Yes );
			} else {
				Server.CommandManager.SendCommand( Server.CommandType.VoteKickResponse_No );
			}
		}

		/*
		===============
		BroadcastVoteKickStart
		===============
		*/
		private void BroadcastVoteKickStart() {
			Server.CommandManager.SendCommand( Server.CommandType.StartVoteKick );

			VoteKickStarted.Publish( new VoteKickStartedEventData( Internal.TargetID, Internal.InitiatorID ) );
		}
	};
};