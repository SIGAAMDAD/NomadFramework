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
using Steamworks;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NomadCore.Systems.Steam;
using NomadCore.Systems.LobbySystem.Server.Packets;
using NomadCore.Interfaces;

namespace NomadCore.Systems.LobbySystem.Server {
	/*
	===================================================================================
	
	CommandManager
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public sealed class CommandManager {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct ExecuteEventData( CSteamID senderId, CommandType type ) : IEventArgs {
			/// <summary>
			/// Who sent the server command.
			/// </summary>
			public readonly CSteamID SenderId = senderId;

			/// <summary>
			/// What command is being executed.
			/// </summary>
			public readonly CommandType Type = type;
		};

		private static readonly CommandManager Instance;

		public static readonly int SERVER_COMMAND_PACKET_SIZE = Marshal.SizeOf<ServerCommandPacket>();

		/// <summary>
		/// 
		/// </summary>
		private readonly GameEvent ServerCommandExecuted = new GameEvent( nameof( ServerCommandExecuted ) );

		static CommandManager() {
			Instance = new CommandManager();
		}

		/*
		===============
		RegisterCommandCallback
		===============
		*/
		/// <summary>
		/// Registers an event callback for server command <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The command type to hook into.</param>
		/// <param name="callback">The method to call whenever the command is executed.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="type"/> isn't a valid <see cref="CommandType"/>.</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void RegisterCommandCallback( CommandType type, IGameEvent.EventCallback? callback ) {
			if ( type < CommandType.StartGame || type >= CommandType.Count ) {
				throw new ArgumentOutOfRangeException( nameof( type ) );
			}
			ArgumentNullException.ThrowIfNull( callback );

			Instance.ServerCommandExecuted.Subscribe( Instance, callback );
		}

		/*
		===============
		ExecuteCommand
		===============
		*/
		/// <summary>
		/// Executes the server command <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The command to execute.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="type"/> isn't a valid <see cref="CommandType"/>.</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void ExecuteCommand( CommandType type ) {
			if ( type < CommandType.StartGame || type >= CommandType.Count ) {
				throw new ArgumentOutOfRangeException( nameof( type ) );
			}
			Instance.ServerCommandExecuted.Publish( new ExecuteEventData( (CSteamID)SteamManager.SteamID.Value, type ) );
		}

		/*
		===============
		ExecuteCommand
		===============
		*/
		/// <summary>
		/// Executes the server command <paramref name="type"/>.
		/// </summary>
		/// <param name="senderId">The sender of the command.</param>
		/// <param name="type">The command to execute.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void ExecuteCommand( CSteamID senderId, CommandType type ) {
			if ( type < CommandType.StartGame || type >= CommandType.Count ) {
				throw new ArgumentOutOfRangeException( nameof( type ) );
			}
			Instance.ServerCommandExecuted.Publish( new ExecuteEventData( senderId, type ) );
		}

		/*
		===============
		SendCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="type"/> isn't a valid <see cref="CommandType"/>.</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void SendCommand( CommandType type ) {
			if ( type < CommandType.StartGame || type >= CommandType.Count ) {
				throw new ArgumentOutOfRangeException( nameof( type ) );
			}
			using NetworkWriter writer = new NetworkWriter( MessageType.ServerCommand );
			writer.WriteUInt32( (uint)type );
		}
	};
};