using EventSystem;
using System;

namespace ConsoleSystem {
	/*
	===================================================================================
	
	ConsoleCommand
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public readonly struct ConsoleCommand : IDisposable {
		public readonly string Name;
		public readonly string Description;
		public readonly IGameEvent.EventCallback Callback;

		public ConsoleCommand( string? name, IGameEvent.EventCallback? callback ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentNullException.ThrowIfNull( callback );

			Name = name;
			Callback = callback;
			Description = "";

			Console.AddCommand( this );
		}

		public ConsoleCommand( string? name, IGameEvent.EventCallback? callback, string? description ) {
			ArgumentException.ThrowIfNullOrEmpty( name );
			ArgumentException.ThrowIfNullOrEmpty( description );
			ArgumentNullException.ThrowIfNull( callback );

			Name = name;
			Description = description;
			Callback = callback;

			Console.AddCommand( this );
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// Removes the command from the console system.
		/// </summary>
		public void Dispose() {
			Console.RemoveCommand( this );
		}
	};
};