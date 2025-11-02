using CVars;
using EventSystem;
using Godot;
using SaveSystem;

/*
===================================================================================

BackendManager

===================================================================================
*/
/// <summary>
/// Use as 
/// </summary>

public sealed partial class BackendManager : Node {
	private GameEventBus? EventBus;
	private ConsoleSystem.Console? ConsoleSystem;
	private CVarSystem? CvarSystem;
	private SaveManager? SaveSystem;

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

		GD.Print( "Initializing Backend..." );

		EventBus = new GameEventBus();
		ConsoleSystem = new ConsoleSystem.Console( EventBus );
		EventBus.SetConsoleService( ConsoleSystem );
		CvarSystem = new CVarSystem( ConsoleSystem );
		SaveSystem = new SaveManager( ConsoleSystem, EventBus, CvarSystem );
		
		GetTree().Root.CallDeferred( Window.MethodName.AddChild, ConsoleSystem );
	}
};