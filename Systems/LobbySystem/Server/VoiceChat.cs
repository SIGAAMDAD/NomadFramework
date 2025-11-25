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

using NomadCore.Systems.ConsoleSystem.CVars;
using NomadCore.Systems.EventSystem;
using Godot;
using NomadCore.Systems.LobbySystem.Server.Packets;
using Steamworks;
using System;
using System.Collections.Generic;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	VoiceChat
	
	===================================================================================
	*/
	/// <summary>
	/// Handles the voice chat.
	/// </summary>

	public sealed class VoiceChat {
		public const int MAX_COMPRESSED_SIZE = 1024;
		public const int MAX_PACKET_SIZE = 1029;
		private const int SAMPLE_RATE = 44100;
		private const float VOICE_THRESHOLD = 0.05f;

		private readonly AudioStreamGeneratorPlayback Playback;
		private bool RecordingVoice = false;

		private readonly Dictionary<ulong, VoiceActivity> Voices = new Dictionary<ulong, VoiceActivity>();
		private readonly VoiceChatPacketHandler PacketHandler = new VoiceChatPacketHandler();

		private readonly Vector2[] Frames = new Vector2[ SAMPLE_RATE ];
		private readonly byte[] Packet = new byte[ MAX_PACKET_SIZE ];
		private readonly byte[] RecordBuffer = new byte[ MAX_COMPRESSED_SIZE ];
		private int CaptureBusIndex = 0;
		private AudioEffectCapture? CaptureEffect;

		/*
		===============
		VoiceChat
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="voiceChatOverlay"></param>
		public VoiceChat( Node? voiceChatOverlay ) {
			ArgumentNullException.ThrowIfNull( voiceChatOverlay );

			AudioStreamPlayer audioPlayer = voiceChatOverlay.GetNode<AudioStreamPlayer>( "Output" );
			ArgumentNullException.ThrowIfNull( audioPlayer );

			Playback = (AudioStreamGeneratorPlayback)audioPlayer.GetStreamPlayback();

			LobbyManager.LobbyJoined.Subscribe( this, OnActivateVoiceChat );
			LobbyManager.LobbyLeft.Subscribe( this, OnDeactivateVoiceChat );
			LobbyManager.ClientLeftLobby.Subscribe( this, OnRemoveCachedVoice );
		}

		/*
		===============
		CaptureVoice
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delta"></param>
		public void CaptureVoice( float delta ) {
			foreach ( var voice in Voices.Values ) {
				if ( voice.Muted ) {
					continue;
				}
				voice.CheckDecay( delta );
			}
			ProcessInputBuffer();
		}

		/*
		===============
		ProcessInputBuffer
		===============
		*/
		private void ProcessInputBuffer() {
			EVoiceResult result = SteamUser.GetAvailableVoice( out uint compressedSize );
			if ( result == EVoiceResult.k_EVoiceResultOK && compressedSize > 0 ) {
				if ( SteamUser.GetVoice( true, RecordBuffer, compressedSize, out uint bytesWritten ) == EVoiceResult.k_EVoiceResultOK ) {
					NetworkWriter writer = new NetworkWriter( MessageType.VoiceChat );
					writer.WriteUInt32( bytesWritten );
					writer.WriteByteArray( RecordBuffer );
					MessageHandler.Sync( in writer );
				}
			}
		}

		/*
		===============
		ProcessIncomingVoice
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		/// <param name="data"></param>
		public void ProcessIncomingVoice( ulong senderId, byte[] data ) {
			if ( data == null || data.Length == 0 || data.Length > MAX_PACKET_SIZE ) {
				ConsoleSystem.Console.PrintError( $"VoiceChat.ProcessIncomingVoice: invalid data packet!" );
				return;
			}
			if ( !Voices.TryGetValue( senderId, out VoiceActivity activity ) ) {
				if ( !LobbyManager.Current.TryGetPlayer( (CSteamID)senderId, out User? user ) ) {
					return;
				}
				ArgumentNullException.ThrowIfNull( user );
				Voices.TryAdd( senderId, user.Voice );
			}
			if ( activity.Muted ) {
				return;
			}

			PacketHandler.Process( in activity, Frames, data );
			Playback.CallDeferred( AudioStreamGeneratorPlayback.MethodName.PushBuffer, Frames );
		}

		/*
		===============
		OnActivateVoiceChat
		===============
		*/
		private void OnActivateVoiceChat( in LobbyManager.LobbyStatusChangedEventData args ) {
			if ( !CVarSystem.GetCVar<bool>( "CODLobbies" ) ) {
				return;
			}
			ConsoleSystem.Console.PrintLine( "VoiceChat.OnActivateVoiceChat: enabling voice chat" );
			SteamUser.StartVoiceRecording();
			RecordingVoice = true;
		}

		/*
		===============
		OnDeactivateVoiceChat
		===============
		*/
		private void OnDeactivateVoiceChat( in LobbyManager.LobbyStatusChangedEventData args ) {
			ConsoleSystem.Console.PrintLine( "VoiceChat.OnActivateVoiceChat: disabling voice chat" );

			Voices.Clear();

			SteamUser.StopVoiceRecording();
			RecordingVoice = false;
		}

		/*
		===============
		OnRemoveCachedVoice
		===============
		*/
		private void OnRemoveCachedVoice( in ClientConnectionChangedEventData args ) {
			if ( Voices.ContainsKey( (ulong)args.ClientID ) ) {
				Voices.Remove( (ulong)args.ClientID );
			}
		}
	};
};