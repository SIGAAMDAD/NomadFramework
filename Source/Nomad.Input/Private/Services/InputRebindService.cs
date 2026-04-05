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
using System.Collections.Immutable;
using System.Numerics;
using Nomad.Core.Events;
using Nomad.Core.Input;
using Nomad.Input.Extensions;
using Nomad.Input.Interfaces;
using Nomad.Input.Private.Extensions;
using Nomad.Input.Private.Repositories;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Private.Services
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class InputRebindService : IInputRebindService, IDisposable
	{
		private const float AXIS_CAPTURE_THRESHOLD = 0.5f;

		public bool IsRebinding => _currentRequest.HasValue;
		public InputRebindRequest? CurrentRequest => _currentRequest;

		public event Action<InputRebindRequest>? RebindStarted;
		public event Action<InputRebindRequest>? RebindCanceled;
		public event Action<InputRebindResult>? RebindCompleted;

		private readonly BindRepository _repository;
		private readonly BindingCompilerService _compilerService;

		private readonly ISubscriptionHandle _keyboardEvent;
		private readonly ISubscriptionHandle _mouseButtonEvent;
		private readonly ISubscriptionHandle _mouseMotionEvent;
		private readonly ISubscriptionHandle _gamepadButtonEvent;
		private readonly ISubscriptionHandle _gamepadAxisEvent;

		private InputRebindRequest? _currentRequest;

		public InputRebindService(BindRepository repository, BindingCompilerService compilerService, IGameEventRegistryService eventRegistry)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_compilerService = compilerService ?? throw new ArgumentNullException(nameof(compilerService));

			var keyboardEvent = eventRegistry.GetEvent<KeyboardEventArgs>(Core.Constants.Events.Input.KEYBOARD_EVENT, Core.Constants.Events.Input.NAMESPACE);
			_keyboardEvent = keyboardEvent.Subscribe(OnKeyboardEventTriggered);

			var mouseButtonEvent = eventRegistry.GetEvent<MouseButtonEventArgs>(Core.Constants.Events.Input.MOUSE_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE);
			_mouseButtonEvent = mouseButtonEvent.Subscribe(OnMouseButtonEventTriggered);

			var mouseMotionEvent = eventRegistry.GetEvent<MouseMotionEventArgs>(Core.Constants.Events.Input.MOUSE_MOTION_EVENT, Core.Constants.Events.Input.NAMESPACE);
			_mouseMotionEvent = mouseMotionEvent.Subscribe(OnMouseMotionEventTriggered);

			var gamepadButtonEvent = eventRegistry.GetEvent<GamepadButtonEventArgs>(Core.Constants.Events.Input.GAMEPAD_BUTTON_EVENT, Core.Constants.Events.Input.NAMESPACE);
			_gamepadButtonEvent = gamepadButtonEvent.Subscribe(OnGamepadButtonEventTriggered);

			var gamepadAxisEvent = eventRegistry.GetEvent<GamepadAxisEventArgs>(Core.Constants.Events.Input.GAMEPAD_AXIS_EVENT, Core.Constants.Events.Input.NAMESPACE);
			_gamepadAxisEvent = gamepadAxisEvent.Subscribe(OnGamepadAxisEventTriggered);
		}

		public void Dispose()
		{
			_keyboardEvent?.Dispose();
			_mouseButtonEvent?.Dispose();
			_mouseMotionEvent?.Dispose();
			_gamepadButtonEvent?.Dispose();
			_gamepadAxisEvent?.Dispose();
		}

		public bool BeginRebind(in InputRebindRequest request)
		{
			if (IsRebinding || !TryGetBinding(request, out var binding))
			{
				return false;
			}

			if (!IsSupported(binding.Kind, request.Part))
			{
				return false;
			}

			_currentRequest = request;
			RebindStarted?.Invoke(request);
			return true;
		}

		public bool CancelRebind()
		{
			if (!_currentRequest.HasValue)
			{
				return false;
			}

			InputRebindRequest request = _currentRequest.Value;
			_currentRequest = null;
			RebindCanceled?.Invoke(request);
			return true;
		}

		public bool ApplyBinding(in InputRebindRequest request, InputBindingDefinition binding)
		{
			if (!_repository.TryGetBindMapping(request.MappingName, out var actions))
			{
				return false;
			}

			int actionIndex = FindActionIndex(actions, request.ActionId);
			if (actionIndex < 0 || (uint)request.BindingIndex >= (uint)actions[actionIndex].Bindings.Length)
			{
				return false;
			}

			var bindingsBuilder = actions[actionIndex].Bindings.Clone().ToBuilder();
			bindingsBuilder[request.BindingIndex] = binding.Clone();

			if (!_repository.SetActionBindings(request.MappingName, request.ActionId, bindingsBuilder.ToImmutable()))
			{
				return false;
			}

			_compilerService.CompileIntoRepository(_repository.GetAllBindings());
			_currentRequest = null;
			RebindCompleted?.Invoke(new InputRebindResult(request, binding.Clone()));
			return true;
		}

		private void OnKeyboardEventTriggered(in KeyboardEventArgs args)
		{
			if (!_currentRequest.HasValue || !args.Pressed)
			{
				return;
			}

			ApplyCapturedButton(InputDeviceSlot.Keyboard, args.KeyNum.ToControlId());
		}

		private void OnMouseButtonEventTriggered(in MouseButtonEventArgs args)
		{
			if (!_currentRequest.HasValue || !args.Pressed)
			{
				return;
			}

			ApplyCapturedButton(InputDeviceSlot.Mouse, args.Button.ToControlId());
		}

		private void OnMouseMotionEventTriggered(in MouseMotionEventArgs args)
		{
			if (!_currentRequest.HasValue)
			{
				return;
			}

			if (args.RelativeX == 0 && args.RelativeY == 0)
			{
				return;
			}

			var request = _currentRequest.Value;
			if (!TryGetBinding(request, out var existing) || existing.Kind != InputBindingKind.Delta2D || request.Part != InputRebindPart.Whole)
			{
				return;
			}

			var updated = existing.Clone();
			updated.Delta2D.DeviceId = InputDeviceSlot.Mouse;
			updated.Delta2D.ControlId = InputControlId.Delta;

			ApplyBinding(request, updated);
		}

		private void OnGamepadButtonEventTriggered(in GamepadButtonEventArgs args)
		{
			if (!_currentRequest.HasValue || !args.Pressed)
			{
				return;
			}

			ApplyCapturedButton(GetGamepadDeviceSlot(args.DeviceId), args.Button.ToControlId());
		}

		private void OnGamepadAxisEventTriggered(in GamepadAxisEventArgs args)
		{
			if (!_currentRequest.HasValue || args.Value.LengthSquared() < AXIS_CAPTURE_THRESHOLD * AXIS_CAPTURE_THRESHOLD)
			{
				return;
			}

			var request = _currentRequest.Value;
			if (!TryGetBinding(request, out var existing) || existing.Kind != InputBindingKind.Axis2D || request.Part != InputRebindPart.Whole)
			{
				return;
			}

			var updated = existing.Clone();
			updated.Axis2D.DeviceId = GetGamepadDeviceSlot(args.DeviceId);
			updated.Axis2D.ControlId = args.Stick.ToControlId();

			ApplyBinding(request, updated);
		}

		private void ApplyCapturedButton(InputDeviceSlot deviceId, InputControlId controlId)
		{
			if (!_currentRequest.HasValue)
			{
				return;
			}

			InputRebindRequest request = _currentRequest.Value;
			if (!TryGetBinding(request, out var existing))
			{
				return;
			}

			InputBindingDefinition updated = existing.Clone();
			switch (updated.Kind)
			{
				case InputBindingKind.Button:
					if (request.Part != InputRebindPart.Whole)
					{
						return;
					}

					updated.Button.DeviceId = deviceId;
					updated.Button.ControlId = controlId;
					updated.Button.Modifiers = ImmutableArray<InputControlId>.Empty;
					break;
				case InputBindingKind.Axis1DComposite:
					switch (request.Part)
					{
						case InputRebindPart.Negative:
							updated.Axis1DComposite.Negative = controlId;
							break;
						case InputRebindPart.Positive:
							updated.Axis1DComposite.Positive = controlId;
							break;
						default:
							return;
					}
					break;
				case InputBindingKind.Axis2DComposite:
					switch (request.Part)
					{
						case InputRebindPart.Up:
							updated.Axis2DComposite.Up = controlId;
							break;
						case InputRebindPart.Down:
							updated.Axis2DComposite.Down = controlId;
							break;
						case InputRebindPart.Left:
							updated.Axis2DComposite.Left = controlId;
							break;
						case InputRebindPart.Right:
							updated.Axis2DComposite.Right = controlId;
							break;
						default:
							return;
					}
					break;
				default:
					return;
			}

			ApplyBinding(request, updated);
		}

		private bool TryGetBinding(InputRebindRequest request, out InputBindingDefinition binding)
		{
			binding = null;
			if (!_repository.TryGetBindMapping(request.MappingName, out var actions))
			{
				return false;
			}

			int actionIndex = FindActionIndex(actions, request.ActionId);
			if (actionIndex < 0 || (uint)request.BindingIndex >= (uint)actions[actionIndex].Bindings.Length)
			{
				return false;
			}

			binding = actions[actionIndex].Bindings[request.BindingIndex];
			return binding != null;
		}

		private static int FindActionIndex(ImmutableArray<InputActionDefinition> actions, string actionId)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				if (actions[i].Name.Equals(actionId, StringComparison.Ordinal))
				{
					return i;
				}
			}

			return -1;
		}

		private static bool IsSupported(InputBindingKind kind, InputRebindPart part)
		{
			return kind switch
			{
				InputBindingKind.Button => part == InputRebindPart.Whole,
				InputBindingKind.Axis1D => part == InputRebindPart.Whole,
				InputBindingKind.Axis2D => part == InputRebindPart.Whole,
				InputBindingKind.Delta2D => part == InputRebindPart.Whole,
				InputBindingKind.Axis1DComposite => part == InputRebindPart.Negative || part == InputRebindPart.Positive,
				InputBindingKind.Axis2DComposite => part == InputRebindPart.Up || part == InputRebindPart.Down || part == InputRebindPart.Left || part == InputRebindPart.Right,
				_ => false
			};
		}

		private static InputDeviceSlot GetGamepadDeviceSlot(int deviceId)
		{
			return deviceId switch
			{
				0 => InputDeviceSlot.Gamepad0,
				1 => InputDeviceSlot.Gamepad1,
				2 => InputDeviceSlot.Gamepad2,
				3 => InputDeviceSlot.Gamepad3,
				_ => throw new ArgumentOutOfRangeException(nameof(deviceId))
			};
		}
	}
}
