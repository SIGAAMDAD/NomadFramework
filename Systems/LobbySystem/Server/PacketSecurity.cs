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

using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	PacketSecurity
	
	===================================================================================
	*/
	/// <summary>
	/// Handles rate limiting and securing packets
	/// </summary>

	public sealed class PacketSecurity {
		private const bool USE_STEAM_ENCRYPTION = true;

		/// <summary>
		/// The current limit on how many packets we can process per frame.
		/// </summary>
		public const int PACKET_RATE_LIMIT = 425;

		private struct SecurityHeader {
			public uint Timestamp;
			public ushort Sequence;
			public byte Version;
		};
		private struct ConnectionSecurity {
			public uint LastTimestamp = 0;
			public ushort MessageCount = 0;
			public double LastResetTime = 0.0f;

			public ConnectionSecurity() {
			}
		};

		private static readonly Dictionary<CSteamID, ConnectionSecurity> _securityStates = new Dictionary<CSteamID, ConnectionSecurity>();
		private static readonly byte[] HMacKey = new byte[ 16 ];

		public PacketSecurity() {
			using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
			rng.GetBytes( HMacKey );
		}

		/*
		===============
		SecureOutgoingMessage
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="targetId"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static byte[] SecureOutgoingMessage( byte[] data, CSteamID targetId ) {
			return data;
		}

		/*
		===============
		ProcessIncomingMessage
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="senderId"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool ProcessIncomingMessage( in ReadOnlySpan<byte> data, CSteamID senderId ) {
			if ( !_securityStates.TryGetValue( senderId, out ConnectionSecurity state ) ) {
				state = new ConnectionSecurity();
				_securityStates.Add( senderId, state );
			}
			return ExceedsLimit( ref state );
		}

		/*
		===============
		ExceedsLimit
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool ExceedsLimit( ref ConnectionSecurity state ) {
			if ( DateTime.UtcNow.TimeOfDay.TotalSeconds - state.LastResetTime > 1.0f ) {
				state.MessageCount = 0;
				state.LastResetTime = DateTime.UtcNow.TimeOfDay.TotalSeconds;
			}
			return ++state.MessageCount > PACKET_RATE_LIMIT;
		}
	};
};