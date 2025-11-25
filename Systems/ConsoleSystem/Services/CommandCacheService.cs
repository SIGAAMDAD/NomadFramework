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

using NomadCore.Abstractions.Services;
using NomadCore.Infrastructure;
using NomadCore.Interfaces;
using NomadCore.Systems.ConsoleSystem.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace NomadCore.Systems.ConsoleSystem.Services {
	/*
	===================================================================================
	
	CommandCacheService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class CommandCacheService : ICommandService {
		private readonly ConcurrentDictionary<string, IConsoleCommand> Commands = new ConcurrentDictionary<string, IConsoleCommand>();

		/*
		===============
		CommandCacheServices
		===============
		*/
		public CommandCacheService() {
			RegisterCommand( new ConsoleCommand(
				name: "cmdlist",
				callback: OnListCommands,
				description: "Prints all available commands to the console."
			) );
		}
		
		/*
		===============
		Initialize
		===============
		*/
		public void Initialize() {
		}

		/*
		===============
		Shutdown
		===============
		*/
		public void Shutdown() {
		}

		/*
		===============
		RegisterCommand
		===============
		*/
		/// <summary>
		/// Registers a command into the global cache
		/// </summary>
		/// <param name="command">The command that's being added to the global cache.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void RegisterCommand( IConsoleCommand command ) {
			Commands[ command.Name ] = command;
		}

		/*
		===============
		GetCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IConsoleCommand GetCommand( string command ) {
			ArgumentException.ThrowIfNullOrEmpty( command );
			return Commands[ command ];
		}

		/*
		===============
		TryGetCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="command"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryGetCommand( string name, out IConsoleCommand command ) {
			return Commands.TryGetValue( name, out command );
		}

		/*
		===============
		CommandExists
		===============
		*/
		/// <summary>
		/// Checks if a command by the name of <paramref name="command"/> exists in the cache.
		/// </summary>
		/// <param name="command">The command to check for.</param>
		/// <returns>True if <paramref name="command"/> has been registered.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool CommandExists( string command ) {
			ArgumentException.ThrowIfNullOrEmpty( command );
			return Commands.ContainsKey( command );
		}

		/*
		===============
		OnListCommands
		===============
		*/
		/// <summary>
		/// Lists all commands currently stored in <see cref="Commands"/>.
		/// </summary>
		/// <param name="args"></param>
		private void OnListCommands( in ICommandExecutedEventData args ) {
			IConsoleCommand[] commandList = [ .. Commands.Values ];
			
			var logger = ServiceRegistry.Get<ILoggerService>();
			for ( int i = 0; i < commandList.Length; i++ ) {
				logger.PrintLine( $"{commandList[ i ].Name}: {commandList[ i ].Description}" );
			}
		}
	};
};