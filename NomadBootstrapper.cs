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
/// 
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
		
		ICvarSystemService cvarSystem = ServiceRegistry.Register<ICVarSystemService>( new CVarSystemService() );
		IGameEventBusService eventBus = ServiceRegistry.Register<IGameEventBusService>( new GameEventBus() );
		IConsoleService console = ServiceRegistry.Register<IConsoleService>( new Console( GetTree().Root, cvarSystem, eventBus ) );

		ServiceRegistry.InitializeAll();
	}
};