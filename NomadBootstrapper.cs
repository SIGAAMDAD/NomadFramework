using NomadCore.Abstractions.Services;
using NomadCore.Infrastructure;
using NomadCore.Systems.EventSystem.Services;
using NomadCore.Systems.ConsoleSystem.Services;
using NomadCore.Systems.ConsoleSystem.CVars.Services;

/*
===================================================================================

NomadBootstrapper

===================================================================================
*/
/// <summary>
/// Use as 
/// </summary>

public sealed partial class NomadBootstrapper : Node {
	/*
	===============
	_Ready
	===============
	*/
	/// <summary>
	/// 
	/// </summary>
	public override void _Ready() {
		base._Ready();
		
		IConsoleService console = ServiceRegistry.Register<IConsoleService>( new Console() );
		IGameEventBusService eventBus = ServiceRegistry.Register<IGameEventBusService>( new GameEventBus() );

		ServiceRegistry.InitializeAll();
	}
};