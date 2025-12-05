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

using Godot;
using NomadCore.Abstractions.Services;
using NomadCore.Infrastructure;
using NomadCore.Interfaces.EventSystem;
using NomadCore.Systems.ConsoleSystem.Infrastructure;
using NomadCore.Systems.ConsoleSystem.Infrastructure.Sinks;
using NomadCore.Systems.ConsoleSystem.Interfaces;
using System;

namespace NomadCore.Systems.ConsoleSystem.Services {
	/*
	===================================================================================

	Console

	===================================================================================
	*/
	/// <summary>
	/// The main logger for the game. Use to convey information both boring and important to the
	/// terminal (tty), godot console, the logfile, and the in-game console.
	/// </summary>
	/// <remarks>
	/// This implementation is currently threadsafe.
	/// </remarks>

	public sealed partial class Console : IConsoleService, IConsoleEvents {
		public IGameEvent ConsoleOpened => _consoleOpened;
		private readonly IGameEvent _consoleOpened;

		public IGameEvent ConsoleClosed => _consoleClosed;
		private readonly IGameEvent _consoleClosed;

		public IGameEvent HistoryPrev => _historyPrev;
		private readonly IGameEvent _historyPrev;

		public IGameEvent HistoryNext => _historyNext;
		private readonly IGameEvent _historyNext;

		public IGameEvent AutoComplete => _autoComplete;
		private readonly IGameEvent _autoComplete;

		public IGameEvent PageUp => _pageUp;
		private readonly IGameEvent _pageUp;

		public IGameEvent PageDown => _pageDown;
		private readonly IGameEvent _pageDown;

		/*
		===============
		Console
		===============
		*/
		public Console( Node rootNode, ICVarSystemService cvarSystem, IGameEventBusService eventBus ) {
			ArgumentNullException.ThrowIfNull( rootNode );
			ArgumentNullException.ThrowIfNull( eventBus );

			_consoleOpened = eventBus.CreateEvent( nameof( ConsoleOpened ) );
			_consoleClosed = eventBus.CreateEvent( nameof( ConsoleClosed ) );
			_historyPrev = eventBus.CreateEvent( nameof( HistoryPrev ) );
			_historyNext = eventBus.CreateEvent( nameof( HistoryNext ) );
			_autoComplete = eventBus.CreateEvent( nameof( AutoComplete ) );
			_pageUp = eventBus.CreateEvent( nameof( PageUp ) );
			_pageDown = eventBus.CreateEvent( nameof( PageDown ) );

			var commandBuilder = new GodotCommandBuilder( eventBus, this );
			var commandService = ServiceRegistry.Register<ICommandService>( new CommandCacheService() );
			var console = new GodotConsole( commandBuilder, this );
			var commandLine = ServiceRegistry.Register<ICommandLine>( new CommandLine( commandBuilder, this, eventBus ) );
			var logger = ServiceRegistry.Register<ILoggerService>(
				new LoggerService( commandLine, commandService, cvarSystem, [
					new GodotSink(),
					new FileSink( cvarSystem ),
					new InGameSink( console, commandBuilder, this )
				] )
			);
			var history = new History( commandBuilder, logger, eventBus, this );

			rootNode.CallDeferred( Node.MethodName.AddChild, console );
		}
	};
};