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

using System;
using System.Collections.Generic;
using Nomad.Core.Events;
using Nomad.Core.Util;
using Nomad.Input.Events;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	InputDispatchService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class InputDispatchService {
		private readonly Dictionary<string, IGameEvent<ButtonActionEventArgs>> _buttonEvents = new();
		private readonly Dictionary<string, IGameEvent<FloatActionEventArgs>> _floatEvents = new();
		private readonly Dictionary<string, IGameEvent<AxisActionEventArgs>> _axisEvents = new();
		private readonly IGameEventRegistryService _eventFactory;

		/*
		===============
		InputDispatchService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFactory"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public InputDispatchService( IGameEventRegistryService eventFactory ) {
			_eventFactory = eventFactory ?? throw new ArgumentNullException( nameof( eventFactory ) );
		}

		/*
		===============
		Dispatch
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		public void Dispatch( in ResolvedAction action ) {
			switch ( action.ValueType ) {
				case InputValueType.Button:
					GetButtonEvent( action.ActionId ).Publish( new ButtonActionEventArgs( action.ActionId, action.Phase, action.ButtonValue, action.TimeStamp ) );
					break;
				case InputValueType.Float:
					GetFloatEvent( action.ActionId ).Publish( new FloatActionEventArgs( action.ActionId, action.Phase, action.FloatValue, action.TimeStamp ) );
					break;
				case InputValueType.Vector2:
					GetAxisEvent( action.ActionId ).Publish( new AxisActionEventArgs( action.ActionId, action.Phase, action.Vector2Value, action.TimeStamp ) );
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof( action ) );
			}
		}

		/*
		===============
		GetButtonEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionId"></param>
		/// <returns></returns>
		private IGameEvent<ButtonActionEventArgs> GetButtonEvent( InternString actionId ) {
			if ( _buttonEvents.TryGetValue( actionId, out var gameEvent ) ) {
				return gameEvent;
			}
			gameEvent = _eventFactory.GetEvent<ButtonActionEventArgs>( $"{(string)actionId}:{Input.Constants.Events.BUTTON_ACTION}", Input.Constants.Events.NAMESPACE );
			_buttonEvents[ actionId ] = gameEvent;
			return gameEvent;
		}

		/*
		===============
		GetFloatEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionId"></param>
		/// <returns></returns>
		private IGameEvent<FloatActionEventArgs> GetFloatEvent( InternString actionId ) {
			if ( _floatEvents.TryGetValue( actionId, out var gameEvent ) ) {
				return gameEvent;
			}
			gameEvent = _eventFactory.GetEvent<FloatActionEventArgs>( $"{(string)actionId}:{Input.Constants.Events.FLOAT_ACTION}", Input.Constants.Events.NAMESPACE );
			_floatEvents[ actionId ] = gameEvent;
			return gameEvent;
		}

		/*
		===============
		GetAxisEvent
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionId"></param>
		/// <returns></returns>
		private IGameEvent<AxisActionEventArgs> GetAxisEvent( InternString actionId ) {
			if ( _axisEvents.TryGetValue( actionId, out var gameEvent ) ) {
				return gameEvent;
			}
			gameEvent = _eventFactory.GetEvent<AxisActionEventArgs>( $"{(string)actionId}:{Input.Constants.Events.AXIS_ACTION}", Input.Constants.Events.NAMESPACE );
			_axisEvents[ actionId ] = gameEvent;
			return gameEvent;
		}
	};
};
