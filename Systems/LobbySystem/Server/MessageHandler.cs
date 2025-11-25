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

using NomadCore.Systems.EventSystem;
using Godot;
using Steamworks;
using System;
using NomadCore.Systems.LobbySystem.Server.Packets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NomadCore.Systems.Steam;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	MessageHandler
	
	===================================================================================
	*/
	/// <summary>
	/// Handles incoming and outgoing messages with the SteamNetworkingSockets API
	/// </summary>

	public sealed class MessageHandler {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct ClientDataReceivedEventData( CSteamID senderId, in ClientDataPacket packet ) : IEventArgs {
			public readonly ulong SenderId = (ulong)senderId;
			public readonly ClientDataPacket Packet = packet;
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct ServerSyncReceivedEventData( CSteamID senderId, in ServerSyncPacket packet ) : IEventArgs {
			public readonly ulong SenderId = (ulong)senderId;
			public readonly ServerSyncPacket Packet = packet;
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct GameStateSyncReceivedEventData( ulong senderId, int stateObjectID, System.IO.BinaryReader? packetReader ) : IEventArgs {
			public readonly ulong SenderId = senderId;
			public readonly System.IO.BinaryReader? PacketReader = packetReader;
			public readonly int StateObjectID = stateObjectID;
		};
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct ServerCommandReceivedEventData( ulong senderId, CommandType commandType ) : IEventArgs {
			public readonly ulong SenderId = senderId;
			public readonly CommandType CommandType = commandType;
		};
		private readonly struct IncomingMessage( CSteamID senderId, byte[] data, int length, MessageType type ) {
			public readonly CSteamID SenderID = senderId;
			public readonly byte[] Data = data;
			public readonly int Length = length;
			public readonly MessageType Type = type;
		};

		private readonly struct NetworkMessageBufferPool( int bufferSize = 1024, int maxBuffers = 128 ) {
			private readonly ConcurrentBag<byte[]> Pool = new ConcurrentBag<byte[]>();
			private readonly int BufferSize = bufferSize;
			private readonly int MaxBuffers = maxBuffers;

			/*
			===============
			Rent
			===============
			*/
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public readonly byte[] Rent() {
				if ( Pool.TryTake( out byte[]? buffer ) ) {
					return buffer;
				}
				return new byte[ BufferSize ];
			}

			/*
			===============
			Return
			===============
			*/
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public readonly void Return( byte[] buffer ) {
				if ( buffer != null && buffer.Length == BufferSize && Pool.Count < MaxBuffers ) {
					Pool.Add( buffer );
				}
			}
		};

		private const int PACKET_READ_LIMIT = 32;
		private static readonly byte[] HANDSHAKE_PACKET = [ (byte)MessageType.Handshake ];

		private readonly NetworkMessageBufferPool Pool = new NetworkMessageBufferPool();

		private int NetworkRunning = 0;
		private readonly System.Threading.Thread? NetworkThread;
		private readonly object NetworkLock = new object();
		private readonly System.IO.MemoryStream? PacketStream = null;
		private readonly System.IO.BinaryReader? PacketReader = null;

		private readonly Queue<IncomingMessage> MessageQueue = new Queue<IncomingMessage>();

		private readonly IntPtr[] Messages = new IntPtr[ PACKET_READ_LIMIT ];

		public static readonly GameEvent<ClientDataReceivedEventData> ClientDataReceived = new GameEvent<ClientDataReceivedEventData>( nameof( ClientDataReceived ) );
		public static readonly GameEvent<ServerSyncReceivedEventData> ServerSyncReceived = new GameEvent<ServerSyncReceivedEventData>( nameof( ServerSyncReceived ) );
		public static readonly GameEvent<GameStateSyncReceivedEventData> GameStateSyncReceived = new GameEvent<GameStateSyncReceivedEventData>( nameof( GameStateSyncReceived ) );

		/*
		===============
		MessageHandler
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public MessageHandler() {
			NetworkRunning = 1;
			NetworkThread = new System.Threading.Thread( NetworkThreadProcess ) {
				Priority = System.Threading.ThreadPriority.Highest
			};
			NetworkThread.Start();
		}

		/*
		===============
		~MessageHandler
		===============
		*/
		/// <summary>
		/// Cleans up any unmanaged resources and the networking thread
		/// </summary>
		~MessageHandler() {
			System.Threading.Interlocked.Exchange( ref NetworkRunning, 1 );
			if ( NetworkThread.IsAlive ) {
				NetworkThread.Join( 1000 );
			}
		}

		/*
		===============
		SendServerSync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		private static void SendServerSync( in NetworkWriter writer ) {
			int count = LobbyManager.Current.Players.Count;
			for ( int i = 0; i < count; i++ ) {
				User user = LobbyManager.Current.Players[ i ];
				if ( (ulong)user.UserID == SteamManager.SteamID ) {
					continue;
				}
				SendTargetPacket( user, writer.Stream.GetBuffer(), Constants.k_nSteamNetworkingSend_Reliable );
			}
		}

		/*
		===============
		SendServerCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		private static void SendServerCommand( in NetworkWriter writer ) {
			int count = LobbyManager.Current.Players.Count;
			for ( int i = 0; i < count; i++ ) {
				SendTargetPacket( LobbyManager.Current.Players[ i ], writer.Stream.GetBuffer(), Constants.k_nSteamNetworkingSend_Reliable );
			}
		}

		/*
		===============
		SendClientSync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <exception cref="Exception"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static void SendClientSync( in NetworkWriter writer ) {
			if ( !LobbyManager.Current.TryGetPlayer( LobbyManager.Current.HostId, out User? host ) ) {
				throw new Exception( "MessageHandler.SendClientSync: host couldn't be found!" );
			}
			ArgumentNullException.ThrowIfNull( host );
			SendTargetPacket( in host, writer.Stream.GetBuffer(), Constants.k_nSteamNetworkingSend_Reliable );
		}

		/*
		===============
		SendVoiceChat
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static void SendVoiceChat( in NetworkWriter writer ) {
			int count = LobbyManager.Current.Players.Count;
			for ( int i = 0; i < count; i++ ) {
				User user = LobbyManager.Current.Players[ i ];
				if ( (ulong)user.UserID == SteamManager.SteamID ) {
					continue;
				}
				SendTargetPacket( in user, writer.Stream.GetBuffer(), Constants.k_nSteamNetworkingSend_UnreliableNoDelay );
			}
		}

		/*
		===============
		Sync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public static void Sync( in NetworkWriter writer ) {
			switch ( writer.Type ) {
				case MessageType.ClientData:
					SendClientSync( in writer );
					break;
				case MessageType.ServerSync:
					SendServerSync( in writer );
					break;
				case MessageType.GameStateSync:
					break;
				case MessageType.ServerCommand:
					SendServerCommand( in writer );
					break;
				case MessageType.VoiceChat:
					SendVoiceChat( in writer );
					break;
			}
		}

		/*
		===============
		SendHandshake
		===============
		*/
		/// <summary>
		/// Sends a handshake to <paramref name="user"/> to confirm an bind a p2p connection status.
		/// </summary>
		/// <param name="user">The user to shake hands with.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void SendHandshake( in User user ) {
			SendTargetPacket( in user, HANDSHAKE_PACKET, Constants.k_nSteamNetworkingSend_Reliable );
		}

		/*
		===============
		ProcessPacket
		===============
		*/
		/// <summary>
		/// Processes the provided buffer and feeds it into <see cref="PacketStream"/>.
		/// </summary>
		/// <param name="data">The data to write to <see cref="PacketStream"/>.</param>
		/// <param name="length">The size of the packet, not the <paramref name="data"/> buffer.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private bool ProcessPacket( byte[] data, int length ) {
			if ( length == 0 ) {
				return false;
			}
			ArgumentNullException.ThrowIfNull( data );
			ArgumentNullException.ThrowIfNull( PacketStream );

			PacketStream.SetLength( 0 );
			PacketStream.Write( data, 0, length );

			// skip the MessageType byte
			PacketStream.Position = 1;

			return true;
		}

		/*
		===============
		ValidatePacket
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="funcName"></param>
		/// <param name="data"></param>
		/// <param name="length"></param>
		/// <returns>True if we have a valid packet.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private bool ValidatePackage( string? funcName, byte[] data, int length ) {
			if ( data == null || data.Length == 0 ) {
				ConsoleSystem.Console.PrintWarning( $"MessageHandler.{funcName}: invalid data packet (null or empty)" );
				return false;
			}
			if ( length == 0 || length > data.Length ) {
				ConsoleSystem.Console.PrintWarning( $"MessageHandler.{funcName}: invalid packet length (0 or greater than data packet)" );
				return false;
			}
			return true;
		}

		/*
		===============
		ProcessClientData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		/// <param name="packet"></param>
		private void ProcessClientData( in CSteamID senderId, in ClientDataPacket packet ) {
			ClientDataReceived.Publish( new ClientDataReceivedEventData( senderId, packet ) );
		}

		/*
		===============
		ProcessServerSync
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		/// <param name="packet"></param>
		private void ProcessServerSync( in CSteamID senderId, in ServerSyncPacket packet ) {
			ServerSyncReceived.Publish( new ServerSyncReceivedEventData( senderId, in packet ) );
		}

		/*
		===============
		ProcessGameData
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		/// <param name="length"></param>
		/// <param name="data"></param>
		private void ProcessGameData( ulong senderId, int length, byte[] data ) {
			if ( !ValidatePackage( nameof( ProcessGameData ), data, length ) ) {
				return;
			}
			try {
				if ( !ProcessPacket( data, length ) ) {
					return;
				}
				GameStateSyncReceived.Publish( new GameStateSyncReceivedEventData( senderId, PacketReader.ReadInt32(), PacketReader ) );
			}
			finally {
				Pool.Return( data );
			}
		}

		/*
		===============
		ProcessServerCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="senderId"></param>
		/// <param name="data"></param>
		private void ProcessServerCommand( ulong senderId, byte[] data ) {
			if ( !ValidatePackage( nameof( ProcessServerCommand ), data, CommandManager.SERVER_COMMAND_PACKET_SIZE ) ) {
				return;
			}
			try {
				if ( !ProcessPacket( data, sizeof( CommandType ) + sizeof( int ) ) ) {
					return;
				}
				CommandType commandType = (CommandType)PacketReader.ReadByte();
				ConsoleSystem.Console.PrintLine( $"Received ServerCommand {commandType} from user {senderId}" );
				CommandManager.ExecuteCommand( (CSteamID)senderId, commandType );
			}
			finally {
				Pool.Return( data );
			}
		}

		/*
		===============
		PollIncomingMessages
		===============
		*/
		/// <summary>
		/// Pokes the steamworks api to check if we have any pending messages on any of the connections.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void PollIncomingMessages() {
			foreach ( var conn in ConnectionManager.EstablishedConnections.Values ) {
				conn.PollMessages( in Messages, ProcessIncomingMessage );
			}
		}

		/*
		===============
		SendPacketOnConnection
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetId"></param>
		/// <param name="conn"></param>
		/// <param name="data"></param>
		/// <param name="sendType"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static void SendPacketOnConnection( in CSteamID targetId, HSteamNetConnection conn, in byte[] data, int sendType ) {
			unsafe {
				byte[] buffer = PacketSecurity.SecureOutgoingMessage( data, targetId );
				fixed ( byte* pBuffer = buffer ) {
					EResult res = SteamNetworkingSockets.SendMessageToConnection(
						conn,
						(IntPtr)pBuffer,
						(uint)buffer.Length,
						sendType,
						out long _
					);
					if ( res != EResult.k_EResultOK ) {
						ConsoleSystem.Console.PrintError( $"MessageHandler.SendTargetPacket: error sending message to {targetId} - {res}" );
					}
				}
			}
		}

		/*
		===============
		SendTargetPacket
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="buffer"></param>
		/// <param name="sendType"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static void SendTargetPacket( in User target, in byte[] buffer, int sendType ) {
			if ( !ConnectionManager.GetConnection( target, out ClientConnection? conn ) ) {
				ConsoleSystem.Console.PrintError( $"MessageHandler.SendTargetPacket: not a valid connection id {target.UserID}" );
				return;
			}
			SendPacketOnConnection( in target.UserID, target.Connection.Connection, in buffer, sendType );
		}

		/*
		===============
		ProcessIncomingMessage
		===============
		*/
		private void ProcessIncomingMessage( in ReadOnlySpan<byte> data, CSteamID senderId ) {
			if ( !PacketSecurity.ProcessIncomingMessage( in data, senderId ) ) {
				return;
			}
			switch ( (MessageType)data[ 0 ] ) {
				case MessageType.ServerSync:
					ServerSyncPacket serverSync = MemoryMarshal.Read<ServerSyncPacket>( data );
					ServerSyncReceived.Publish( new ServerSyncReceivedEventData( senderId, in serverSync ) );
					break;
				case MessageType.ClientData:
					ClientDataPacket clientData = MemoryMarshal.Read<ClientDataPacket>( data );
					ClientDataReceived.Publish( new ClientDataReceivedEventData( senderId, in clientData ) );
					break;
			}
		}

		/*
		===============
		IsValidSendType
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sendType"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool IsValidSendType( int sendType ) {
			return sendType >= Constants.k_nSteamNetworkingSend_Unreliable && sendType <= Constants.k_nSteamNetworkingSend_ReliableNoNagle;
		}

		/*
		===============
		NetworkThreadProcess
		===============
		*/
		private void NetworkThreadProcess() {
			const int FRAME_LIMIT = 60;
			const double FRAME_TIME_MS = 1000.0f / FRAME_LIMIT;
			Stopwatch frameTimer = new Stopwatch();

			while ( NetworkRunning == 1 ) {
				frameTimer.Restart();

				try {
					PollIncomingMessages();

					SteamNetworkingSockets.RunCallbacks();
				} catch ( Exception e ) {
					ConsoleSystem.Console.PrintError( $"MessageHandler.NetworkThreadProcess: networking thread exception - {e.Message}" );
				}

				double elapsed = frameTimer.Elapsed.TotalMilliseconds;
				double sleepTime = FRAME_TIME_MS - elapsed;
				if ( sleepTime > 0.0f ) {
					System.Threading.Thread.Sleep( (int)sleepTime );
				}
			}
		}
	};
};