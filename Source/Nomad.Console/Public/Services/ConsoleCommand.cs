/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;

namespace NomadCore.Systems.ConsoleSystem.Infrastructure
{
	/*
	===================================================================================
	
	ConsoleCommand
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public record ConsoleCommand : IConsoleCommand
	{
		public InternString Name => _name;
		private readonly InternString _name;

		public InternString Description => _description;
		private readonly InternString _description;

		public IGameEvent<CommandExecutedEventData>.EventCallback Callback => _callback;
		private readonly IGameEvent<CommandExecutedEventData>.EventCallback _callback;

		/*
		===============
		ConsoleCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		public ConsoleCommand(string name, IGameEvent<CommandExecutedEventData>.EventCallback callback)
		{
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentNullException.ThrowIfNull(callback);

			_name = new(name);
			_callback = callback;
			_description = InternString.Empty;
		}

		/*
		===============
		ConsoleCommand
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		/// <param name="description"></param>
		public ConsoleCommand(string name, IGameEvent<CommandExecutedEventData>.EventCallback callback, string description)
			: this(name, callback)
		{
			ArgumentException.ThrowIfNullOrEmpty(description);
			_description = StringPool.Intern(description);
		}
	};
};