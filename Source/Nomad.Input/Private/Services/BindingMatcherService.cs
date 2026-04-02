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
using System.Numerics;
using System.Runtime.CompilerServices;
using Nomad.Core.Input;
using Nomad.Input.Private.Extensions;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	BindingMatcherService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class BindingMatcherService {
		private readonly CompiledBindingRepository _compiledBindings;
		private readonly InputStateService _stateService;

		/*
		===============
		BindingMatcherService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiledBindings"></param>
		/// <param name="stateService"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public BindingMatcherService( CompiledBindingRepository compiledBindings, InputStateService stateService ) {
			_compiledBindings = compiledBindings ?? throw new ArgumentNullException( nameof( compiledBindings ) );
			_stateService = stateService ?? throw new ArgumentNullException( nameof( stateService ) );
		}

		/*
		===============
		MatchKeyboard
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		public ReadOnlyMemory<BindingMatch> MatchKeyboard( in KeyboardEventArgs evt, uint activeContextMask, InputScheme? activeScheme ) {
			var key = new ButtonLookupKey( InputDeviceSlot.Keyboard, evt.KeyNum.ToControlId(), evt.Pressed );
			return MatchButtons( _compiledBindings.GetButtonCandidates( key ), activeContextMask, activeScheme );
		}

		/*
		===============
		MatchMouseButton
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		public ReadOnlyMemory<BindingMatch> MatchMouseButton( in MouseButtonEventArgs evt, uint activeContextMask, InputScheme? activeScheme ) {
			var key = new ButtonLookupKey( InputDeviceSlot.Mouse, evt.Button.ToControlId(), evt.Pressed );
			return MatchButtons( _compiledBindings.GetButtonCandidates( key ), activeContextMask, activeScheme );
		}

		/*
		===============
		MatchGamepadButton
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		public ReadOnlyMemory<BindingMatch> MatchGamepadButton( in GamepadButtonEventArgs evt, uint activeContextMask, InputScheme? activeScheme ) {
			var key = new ButtonLookupKey( GetGamepadDeviceSlot( evt.DeviceId ), evt.Button.ToControlId(), evt.Pressed );
			return MatchButtons( _compiledBindings.GetButtonCandidates( key ), activeContextMask, activeScheme );
		}

		/*
		===============
		MatchGamepadAxis
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="control"></param>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		public ReadOnlyMemory<BindingMatch> MatchGamepadAxis( InputDeviceSlot device, InputControlId control, uint activeContextMask, InputScheme? activeScheme ) {
			var key = new AxisLookupKey( device, control );
			return MatchAxes( _compiledBindings.GetAxisCandidates( key ), activeContextMask, activeScheme );
		}

		/*
		===============
		MatchMouseDelta
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		public ReadOnlyMemory<BindingMatch> MatchMouseDelta( uint activeContextMask, InputScheme? activeScheme ) {
			if ( _stateService.MouseDelta == Vector2.Zero ) {
				return Array.Empty<BindingMatch>();
			}
			var key = new AxisLookupKey( InputDeviceSlot.Mouse, InputControlId.Delta );
			return MatchAxes( _compiledBindings.GetDeltaCandidates( key ), activeContextMask, activeScheme );
		}

		/*
		===============
		MatchButtons
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="candidates"></param>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		private ReadOnlyMemory<BindingMatch> MatchButtons( ReadOnlySpan<CompiledBinding> candidates, uint activeContextMask, InputScheme? activeScheme ) {
			var matches = new BindingMatch[ candidates.Length ];
			int matchCount = 0;

			for ( int i = 0; i < candidates.Length; i++ ) {
				ref readonly var binding = ref candidates[i];

				if ( !ContextMatches( binding.ContextMask, activeContextMask ) ) {
					continue;
				}
				if ( activeScheme.HasValue && binding.Scheme != activeScheme.Value ) {
					continue;
				}
				if ( !ModifiersSatisfied( binding ) ) {
					continue;
				}
				matches[ matchCount++ ] = new BindingMatch { Binding = binding, Score = Score( binding, modifierCount: binding.Button.Modifiers.Length, exactScheme: true ) };
			}
			return matches.AsMemory( 0, matchCount );
		}

		/*
		===============
		MatchAxes
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="candidates"></param>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <returns></returns>
		private static ReadOnlyMemory<BindingMatch> MatchAxes( ReadOnlySpan<CompiledBinding> candidates, uint activeContextMask, InputScheme? activeScheme ) {
			var matches = new BindingMatch[ candidates.Length ];
			int matchCount = 0;

			for ( int i = 0; i < candidates.Length; i++ ) {
				ref readonly var binding = ref candidates[i];

				if ( !ContextMatches( binding.ContextMask, activeContextMask ) ) {
					continue;
				}
				if ( activeScheme.HasValue && binding.Scheme != activeScheme.Value ) {
					continue;
				}
				matches[ matchCount++ ] = new BindingMatch { Binding = binding, Score = Score( binding, modifierCount: 0, exactScheme: true ) };
			}

			return matches.AsMemory( 0, matchCount );
		}

		/*
		===============
		ContextMatches
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindingMask"></param>
		/// <param name="activeMask"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool ContextMatches( uint bindingMask, uint activeMask ) {
			return bindingMask == 0 || (bindingMask & activeMask) != 0;
		}

		/*
		===============
		ModifiersSatisfied
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		private bool ModifiersSatisfied( CompiledBinding binding ) {
			if ( binding.Kind != InputBindingKind.Button ) {
				return true;
			}
			foreach ( var modifier in binding.Button.Modifiers ) {
				if ( !_stateService.IsPressed( InputDeviceSlot.Keyboard, modifier ) ) {
					return false;
				}
			}
			return true;
		}
		
		/*
		===============
		Score
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binding"></param>
		/// <param name="modifierCount"></param>
		/// <param name="exactScheme"></param>
		/// <returns></returns>
		private static int Score( CompiledBinding binding, int modifierCount, bool exactScheme ) {
			int score = 0;
			score += binding.Priority * 100;
			score += modifierCount * 25;
			score += exactScheme ? 10 : 0;
			score += binding.ConsumesInput ? 1 : 0;
			return score;
		}

		/*
		===============
		GetGamepadDeviceSlot
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private static InputDeviceSlot GetGamepadDeviceSlot( int deviceId ) {
			return deviceId switch {
				0 => InputDeviceSlot.Gamepad0,
				1 => InputDeviceSlot.Gamepad1,
				2 => InputDeviceSlot.Gamepad2,
				3 => InputDeviceSlot.Gamepad3,
				_ => throw new ArgumentOutOfRangeException( nameof( deviceId ) )
			};
		}
	};
};
