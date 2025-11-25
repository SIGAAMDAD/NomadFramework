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

using NomadCore.Systems.LobbySystem.Server.Packets;
using Steamworks;
using System;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	VoiceChatPacketHandler
	
	===================================================================================
	*/
	/// <summary>
	/// Processes and decompresses steam voice packets sent through server connections.
	/// </summary>

	public unsafe readonly struct VoiceChatPacketHandler {
		private const int SAMPLE_RATE = 44100;
		private const int DECODE_BUFFER_LENGTH = 1024;
		private const float SCALE = 1.0f / 32768.0f;

		public static readonly int VOICE_PACKET_HEADER_SIZE = sizeof( VoicePacket );

		private readonly byte[] Output = new byte[ SAMPLE_RATE ];
		private readonly byte[] DecodeBuffer = new byte[ DECODE_BUFFER_LENGTH ];

		/*
		===============
		VoiceChatPacketHandler
		===============
		*/
		public VoiceChatPacketHandler() {
		}

		/*
		===============
		Process
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="voice"></param>
		/// <param name="frames"></param>
		/// <param name="input"></param>
		public void Process( in VoiceActivity voice, Godot.Vector2[] frames, in byte[] input ) {
			EVoiceResult result;
			uint written;

			fixed ( byte* pData = input )
			fixed ( byte* pOutput = DecodeBuffer ) {
				int bytesWritten = *(int*)( pData + 1 );
				Buffer.MemoryCopy( pData + 5, pOutput, DecodeBuffer.Length, bytesWritten );
				result = SteamUser.DecompressVoice(
					DecodeBuffer, (uint)bytesWritten, Output, (uint)Output.Length,
					out written, SAMPLE_RATE
				);
			}

			if ( result == EVoiceResult.k_EVoiceResultOK ) {
				int sampleCount = (int)written / 2;
				float highestAmplitude = 0.0f;

				// we need the data as fast as possible, so pin that shit
				fixed ( byte* outputPtr = Output )
				fixed ( Godot.Vector2* framesPtr = frames ) {
					for ( int i = 0; i < sampleCount; i++ ) {
						short s = (short)( outputPtr[ i * 2 ] | ( outputPtr[ i * 2 + 1 ] << 8 ) );
						float amp = s * SCALE;
						framesPtr[ i ].X = amp;
						framesPtr[ i ].Y = amp;

						if ( amp > highestAmplitude ) {
							highestAmplitude = amp;
						}
					}
					if ( highestAmplitude > voice.Volume ) {
						voice.SetActivity( highestAmplitude );
					}
				}
			} else {
				ConsoleSystem.Console.PrintError( $"VoiceChatPacketHandler.Process: error decompressing voice audio packet - {result}" );
			}
		}
	};
};