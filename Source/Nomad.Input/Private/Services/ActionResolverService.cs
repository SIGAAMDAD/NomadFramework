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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Nomad.Input.Private.ValueObjects;
using Nomad.Input.ValueObjects;
using Nomad.Input.Private.Repositories;
using Nomad.Core.Input;
using Nomad.Core.Util;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	ActionResolverService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class ActionResolverService {
		private readonly CompiledBindingRepository _compiledBindings;
		private readonly InputStateService _stateService;
		private readonly Dictionary<InternString, bool> _actionActiveStates = new();
		private readonly Dictionary<InternString, BestResolvedAction> _bestByAction = new();
		private ResolvedAction[] _resolvedActionBuffer = Array.Empty<ResolvedAction>();

		/*
		===============
		ActionResolverService
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiledBindings"></param>
		/// <param name="stateService"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public ActionResolverService( CompiledBindingRepository compiledBindings, InputStateService stateService ) {
			_compiledBindings = compiledBindings ?? throw new ArgumentNullException( nameof( compiledBindings ) );
			_stateService = stateService ?? throw new ArgumentNullException( nameof( stateService ) );
		}

		/*
		===============
		ResolveMatches
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		public ResolvedAction[] ResolveMatches( ref ReadOnlySpan<BindingMatch> matches, long timeStamp ) {
			ReadOnlySpan<ResolvedAction> actions = ResolveMatchesNonAlloc( matches, timeStamp );
			return actions.ToArray();
		}

		public ReadOnlySpan<ResolvedAction> ResolveMatchesNonAlloc( ReadOnlySpan<BindingMatch> matches, long timeStamp ) {
			_bestByAction.Clear();
			for ( int i = 0; i < matches.Length; i++ ) {
				ref readonly var match = ref matches[i];
				if ( !TryResolve( match.Binding, timeStamp, out var action ) ) {
					continue;
				}
				float magnitude = GetMagnitude( action );

				ref BestResolvedAction current = ref CollectionsMarshal.GetValueRefOrAddDefault( _bestByAction, action.ActionId, out bool exists );
				if ( !exists || ShouldReplace( current.Match, match, current.Magnitude, magnitude ) ) {
					current = new BestResolvedAction( match, action, magnitude );
				}
			}

			EnsureResolvedActionCapacity( _bestByAction.Count );
			int actionCount = 0;
			foreach ( var pair in _bestByAction ) {
				_resolvedActionBuffer[actionCount++] = pair.Value.Action;
			}

			return _resolvedActionBuffer.AsSpan( 0, actionCount );
		}

		/*
		===============
		ResolveComposites
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="activeContextMask"></param>
		/// <param name="activeScheme"></param>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		public ResolvedAction[] ResolveComposites( uint activeContextMask, InputScheme? activeScheme, long timeStamp ) {
			ReadOnlySpan<ResolvedAction> actions = ResolveCompositesNonAlloc( activeContextMask, activeScheme, timeStamp );
			return actions.ToArray();
		}

		public ReadOnlySpan<ResolvedAction> ResolveCompositesNonAlloc( uint activeContextMask, InputScheme? activeScheme, long timeStamp ) {
			var composite1D = _compiledBindings.GetComposite1DBindings();
			var composite2D = _compiledBindings.GetComposite2DBindings();

			EnsureResolvedActionCapacity( composite1D.Length + composite2D.Length );
			int actionCount = 0;
			bool hasActiveScheme = activeScheme.HasValue;
			InputScheme activeSchemeValue = activeScheme.GetValueOrDefault();

			for ( int i = 0; i < composite1D.Length; i++ ) {
				ref readonly var binding = ref composite1D[i];
				if ( !ContextMatches( binding.ContextMask, activeContextMask ) ) {
					continue;
				}
				if ( hasActiveScheme && binding.Scheme != activeSchemeValue ) {
					continue;
				}

				float value = ResolveComposite1D( binding.Axis1DComposite );
				var phase = ResolvePhase( binding.ActionId, MathF.Abs( value ) > 0.0001f );

				_resolvedActionBuffer[actionCount++] = new ResolvedAction(
					actionId: binding.ActionId,
					valueType: InputValueType.Float,
					phase: phase,
					timeStamp: timeStamp,
					floatValue: value
				);
			}

			for ( int i = 0; i < composite2D.Length; i++ ) {
				ref readonly var binding = ref composite2D[i];
				if ( !ContextMatches( binding.ContextMask, activeContextMask ) ) {
					continue;
				}
				if ( hasActiveScheme && binding.Scheme != activeSchemeValue ) {
					continue;
				}

				Vector2 value = ResolveComposite2D( binding.Axis2DComposite );
				var phase = ResolvePhase( binding.ActionId, value.LengthSquared() > 0.0001f );

				_resolvedActionBuffer[actionCount++] = new ResolvedAction(
					actionId: binding.ActionId,
					valueType: InputValueType.Vector2,
					phase: phase,
					timeStamp: timeStamp,
					vector2Value: value
				);
			}

			return _resolvedActionBuffer.AsSpan( 0, actionCount );
		}

		/*
		===============
		TryResolve
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binding"></param>
		/// <param name="timeStamp"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private bool TryResolve( CompiledBinding binding, long timeStamp, out ResolvedAction action ) {
			switch ( binding.Kind ) {
				case InputBindingKind.Button: {
						bool pressed = _stateService.IsPressed( binding.Button.DeviceId, binding.Button.ControlId );
						action = new ResolvedAction(
							actionId: binding.ActionId,
							valueType: InputValueType.Button,
							phase: ResolvePhase( binding.ActionId, pressed ),
							timeStamp: timeStamp,
							buttonValue: pressed
						);
						return true;
					}
				case InputBindingKind.Axis1D: {
						float value = ApplyAxis1DProcessors(
							_stateService.GetAxis1DUnchecked( binding.Axis1D.DeviceId, binding.Axis1D.ControlId ),
							binding.Axis1D
						);
						action = new ResolvedAction(
							actionId: binding.ActionId,
							valueType: InputValueType.Float,
							phase: ResolvePhase( binding.ActionId, MathF.Abs( value ) > 0.0001f ),
							timeStamp: timeStamp,
							floatValue: value
						);
						return true;
					}
				case InputBindingKind.Axis2D: {
						Vector2 value = ApplyAxis2DProcessors(
							_stateService.GetAxis2DUnchecked( binding.Axis2D.DeviceId, binding.Axis2D.ControlId ),
							binding.Axis2D
						);
						action = new ResolvedAction(
							actionId: binding.ActionId,
							valueType: InputValueType.Vector2,
							phase: ResolvePhase( binding.ActionId, value.LengthSquared() > 0.0001f ),
							timeStamp: timeStamp,
							vector2Value: value
						);
						return true;
					}
				case InputBindingKind.Delta2D: {
						Vector2 value = _stateService.MouseDelta;
						value = ApplyDelta2DProcessors( value, binding.Delta2D );

						action = new ResolvedAction(
							actionId: binding.ActionId,
							valueType: InputValueType.Vector2,
							phase: ResolvePhase( binding.ActionId, value.LengthSquared() > 0.0001f ),
							timeStamp: timeStamp,
							vector2Value: value
						);
						return true;
					}
				default:
					action = default;
					return false;
			}
		}

		/*
		===============
		ResolveComposite1D
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		private float ResolveComposite1D( Axis1DCompositeBinding binding ) {
			float negative = _stateService.IsPressed( InputDeviceSlot.Keyboard, binding.Negative ) ? 1.0f : 0.0f;
			float positive = _stateService.IsPressed( InputDeviceSlot.Keyboard, binding.Positive ) ? 1.0f : 0.0f;
			float value = (positive - negative) * binding.Scale;

			if ( binding.Normalize ) {
				value = Math.Clamp( value, -1.0f, 1.0f );
			}

			return value;
		}

		/*
		===============
		ResolveComposite2D
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		private Vector2 ResolveComposite2D( Axis2DCompositeBinding binding ) {
			float up = _stateService.IsPressed( InputDeviceSlot.Keyboard, binding.Up ) ? 1.0f : 0.0f;
			float down = _stateService.IsPressed( InputDeviceSlot.Keyboard, binding.Down ) ? 1.0f : 0.0f;
			float left = _stateService.IsPressed( InputDeviceSlot.Keyboard, binding.Left ) ? 1.0f : 0.0f;
			float right = _stateService.IsPressed( InputDeviceSlot.Keyboard, binding.Right ) ? 1.0f : 0.0f;

			Vector2 value = new(
				(right - left) * binding.ScaleX,
				(up - down) * binding.ScaleY
			);

			if ( binding.Normalize && value != Vector2.Zero ) {
				value = Vector2.Normalize( value );
			}

			return value;
		}

		/*
		===============
		ApplyAxis1DProcessors
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="binding"></param>
		/// <returns></returns>
		private static float ApplyAxis1DProcessors( float value, Axis1DBinding binding ) {
			if ( MathF.Abs( value ) < binding.Deadzone ) {
				value = 0.0f;
			}
			if ( binding.Invert ) {
				value = -value;
			}

			value *= binding.Sensitivity;
			value *= binding.Scale;

			return value;
		}

		/*
		===============
		ApplyAxis2DProcessors
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="binding"></param>
		/// <returns></returns>
		private static Vector2 ApplyAxis2DProcessors( Vector2 value, Axis2DBinding binding ) {
			float deadzone = binding.Deadzone;
			if ( value.LengthSquared() < deadzone * deadzone ) {
				return Vector2.Zero;
			}
			if ( binding.InvertX ) {
				value.X = -value.X;
			}
			if ( binding.InvertY ) {
				value.Y = -value.Y;
			}

			value.X *= binding.ScaleX * binding.Sensitivity;
			value.Y *= binding.ScaleY * binding.Sensitivity;

			return value;
		}

		/*
		===============
		ApplyDelta2DProcessors
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="binding"></param>
		/// <returns></returns>
		private static Vector2 ApplyDelta2DProcessors( Vector2 value, Delta2DBinding binding ) {
			if ( binding.InvertX ) {
				value.X = -value.X;
			}
			if ( binding.InvertY ) {
				value.Y = -value.Y;
			}

			value.X *= binding.ScaleX * binding.Sensitivity;
			value.Y *= binding.ScaleY * binding.Sensitivity;

			return value;
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
		ResolvePhase
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionId"></param>
		/// <param name="isActive"></param>
		/// <returns></returns>
		private InputActionPhase ResolvePhase( InternString actionId, bool isActive ) {
			ref bool active = ref CollectionsMarshal.GetValueRefOrAddDefault( _actionActiveStates, actionId, out bool exists );
			bool wasActive = exists && active;
			active = isActive;

			if ( !isActive ) {
				return InputActionPhase.Canceled;
			}

			return wasActive ? InputActionPhase.Performed : InputActionPhase.Started;
		}

		/*
		===============
		GetMagnitude
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static float GetMagnitude( ResolvedAction action ) {
			return action.ValueType switch {
				InputValueType.Button => action.ButtonValue ? 1.0f : 0.0f,
				InputValueType.Float => MathF.Abs( action.FloatValue ),
				InputValueType.Vector2 => action.Vector2Value.LengthSquared(),
				_ => 0.0f
			};
		}

		/*
		===============
		ShouldReplace
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		/// <param name="challenger"></param>
		/// <param name="currentMagnitude"></param>
		/// <param name="challengerMagnitude"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static bool ShouldReplace( BindingMatch current, BindingMatch challenger, float currentMagnitude, float challengerMagnitude ) {
			if ( challenger.Score != current.Score ) {
				return challenger.Score > current.Score;
			}
			return challengerMagnitude > currentMagnitude;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private void EnsureResolvedActionCapacity( int requiredCapacity ) {
			if ( _resolvedActionBuffer.Length >= requiredCapacity ) {
				return;
			}

			_resolvedActionBuffer = new ResolvedAction[Math.Max( requiredCapacity, _resolvedActionBuffer.Length == 0 ? 8 : _resolvedActionBuffer.Length * 2 )];
		}

		private readonly record struct BestResolvedAction( BindingMatch Match, ResolvedAction Action, float Magnitude );
	}
}
