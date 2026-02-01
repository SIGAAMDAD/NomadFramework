using System;
using Godot;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================

	NotificationNode

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed partial class NotificationNode : GodotObject {
		public event Action<bool> FocusChanged;

		/*
		===============
		_Notification
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="what"></param>
		public override void _Notification( int what ) {
			switch ( ( long )what ) {
				case Node.NotificationWMWindowFocusIn:
					FocusChanged?.Invoke( true );
					break;
				case Node.NotificationWMWindowFocusOut:
					FocusChanged?.Invoke( false );
					break;
			}
		}
	};
};
