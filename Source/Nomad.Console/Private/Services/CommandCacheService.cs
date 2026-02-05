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

using Nomad.Console.Interfaces;
using Nomad.Core.Compatibility;
using Nomad.Core.Console;
using Nomad.Core.Logger;
using Nomad.Core.Util;
using Nomad.CVars;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nomad.Console.Private.Services {
	/*
	===================================================================================

	CommandCacheService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class CommandCacheService : ICommandService {
		private readonly ConcurrentDictionary<InternString, ConsoleCommand> _commands = new ConcurrentDictionary<InternString, ConsoleCommand>();
		private readonly ILoggerService _logger;
		private readonly ICVarSystemService _cvarSystem;

		/*
		===============
		CommandCacheServices
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="cvarSystem"></param>
		public CommandCacheService( ILoggerService logger, ICVarSystemService cvarSystem ) {
			_logger = logger;
			_cvarSystem = cvarSystem;

			RegisterCommand(
				new ConsoleCommand {
					Name = "cmdlist",
					Callback = OnListCommands,
					Description = "prints all available commands to the console."
				}
			);
			RegisterCommand(
				new ConsoleCommand {
					Name = "cvarlist",
					Callback = OnListCVars,
					Description = "prints all currently stored cvars."
				}
			);
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
			_commands.Clear();
		}

		/*
		===============
		RegisterCommand
		===============
		*/
		/// <summary>
		/// Registers a command into the global cache.
		/// </summary>
		/// <param name="command">The command that's being added to the global cache.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void RegisterCommand( ConsoleCommand command ) {
			_commands[ new( command.Name ) ] = command;
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
		public ConsoleCommand GetCommand( string command ) {
			ExceptionCompat.ThrowIfNullOrEmpty( command );
			return _commands[ new( command ) ];
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
		public bool TryGetCommand( string name, out ConsoleCommand command ) {
			ExceptionCompat.ThrowIfNullOrEmpty( name );
			return _commands.TryGetValue( new( name ), out command );
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
			ExceptionCompat.ThrowIfNullOrEmpty( command );
			return _commands.ContainsKey( new( command ) );
		}

		/*
		===============
		OnListCommands
		===============
		*/
		/// <summary>
		/// Lists all _commands currently stored in <see cref="_commands"/>.
		/// </summary>
		/// <param name="args"></param>
		private void OnListCommands( in CommandExecutedEventArgs args ) {
			ConsoleCommand[] commandList = _commands.Values.ToArray();

			for ( int i = 0; i < commandList.Length; i++ ) {
				_logger.PrintLine( $"{commandList[ i ].Name}: {commandList[ i ].Description}" );
			}
		}

		/*
		===============
		OnListCVars
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		private void OnListCVars( in CommandExecutedEventArgs args ) {
			var cvars = _cvarSystem.GetCVars();
			var flagsSb = new StringBuilder( 128 );

			_logger.PrintLine( "\n[CVARS]" );
			for ( int i = 0; i < cvars.Length; i++ ) {
				var cvar = cvars[ i ];

				flagsSb.Clear();
				flagsSb.Append( String.Empty );
				if ( cvar.IsReadOnly ) {
					flagsSb.Append( " ReadOnly" );
				}
				if ( cvar.IsSaved ) {
					flagsSb.Append( " Archive" );
				}
				if ( cvar.IsUserCreated ) {
					flagsSb.Append( " UserCreated" );
				}

				_logger.PrintLine( $"{cvar.Name,20}{flagsSb,32}" );
			}
		}
	};
};
