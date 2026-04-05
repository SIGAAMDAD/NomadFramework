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

using System.Collections.Immutable;
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class InputBindingDefinitionExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bindings"></param>
		/// <returns></returns>
		public static ImmutableArray<InputBindingDefinition> Clone(this ImmutableArray<InputBindingDefinition> bindings)
		{
			if (bindings.IsDefault)
			{
				return default;
			}

			var builder = ImmutableArray.CreateBuilder<InputBindingDefinition>(bindings.Length);
			for (int i = 0; i < bindings.Length; i++)
			{
				builder.Add(bindings[i].Clone());
			}

			return builder.ToImmutable();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		public static InputBindingDefinition Clone(this InputBindingDefinition binding)
		{
			return new InputBindingDefinition
			{
				Scheme = binding.Scheme,
				Kind = binding.Kind,
				Button = binding.Button.Clone(),
				Axis1D = binding.Axis1D.Clone(),
				Axis1DComposite = binding.Axis1DComposite.Clone(),
				Axis2D = binding.Axis2D.Clone(),
				Axis2DComposite = binding.Axis2DComposite.Clone(),
				Delta2D = binding.Delta2D.Clone()
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool ContentEquals(this InputBindingDefinition left, InputBindingDefinition right)
		{
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			if (left is null || right is null)
			{
				return false;
			}

			if (left.Scheme != right.Scheme || left.Kind != right.Kind)
			{
				return false;
			}

			return left.Kind switch
			{
				InputBindingKind.Button => ButtonEquals(left.Button, right.Button),
				InputBindingKind.Axis1D => Axis1DEquals(left.Axis1D, right.Axis1D),
				InputBindingKind.Axis1DComposite => Axis1DCompositeEquals(left.Axis1DComposite, right.Axis1DComposite),
				InputBindingKind.Axis2D => Axis2DEquals(left.Axis2D, right.Axis2D),
				InputBindingKind.Axis2DComposite => Axis2DCompositeEquals(left.Axis2DComposite, right.Axis2DComposite),
				InputBindingKind.Delta2D => Delta2DEquals(left.Delta2D, right.Delta2D),
				_ => false
			};
		}

		private static ButtonBinding Clone(this ButtonBinding binding)
		{
			return binding is null
				? new ButtonBinding()
				: new ButtonBinding
				{
					DeviceId = binding.DeviceId,
					ControlId = binding.ControlId,
					Modifiers = binding.Modifiers.IsDefault ? default : binding.Modifiers.ToImmutableArray()
				};
		}

		private static Axis1DBinding Clone(this Axis1DBinding binding)
		{
			return binding is null
				? new Axis1DBinding()
				: new Axis1DBinding
				{
					DeviceId = binding.DeviceId,
					ControlId = binding.ControlId,
					Deadzone = binding.Deadzone,
					Sensitivity = binding.Sensitivity,
					Scale = binding.Scale,
					Invert = binding.Invert,
					ResponseCurve = binding.ResponseCurve
				};
		}

		private static Axis1DCompositeBinding Clone(this Axis1DCompositeBinding binding)
		{
			return binding is null
				? new Axis1DCompositeBinding()
				: new Axis1DCompositeBinding
				{
					Negative = binding.Negative,
					Positive = binding.Positive,
					Scale = binding.Scale,
					Normalize = binding.Normalize
				};
		}

		private static Axis2DBinding Clone(this Axis2DBinding binding)
		{
			return binding is null
				? new Axis2DBinding()
				: new Axis2DBinding
				{
					DeviceId = binding.DeviceId,
					ControlId = binding.ControlId,
					Deadzone = binding.Deadzone,
					Sensitivity = binding.Sensitivity,
					ScaleX = binding.ScaleX,
					ScaleY = binding.ScaleY,
					InvertX = binding.InvertX,
					InvertY = binding.InvertY,
					ResponseCurve = binding.ResponseCurve
				};
		}

		private static Axis2DCompositeBinding Clone(this Axis2DCompositeBinding binding)
		{
			return binding is null
				? new Axis2DCompositeBinding()
				: new Axis2DCompositeBinding
				{
					Up = binding.Up,
					Down = binding.Down,
					Left = binding.Left,
					Right = binding.Right,
					Normalize = binding.Normalize,
					ScaleX = binding.ScaleX,
					ScaleY = binding.ScaleY
				};
		}

		private static Delta2DBinding Clone(this Delta2DBinding binding)
		{
			return binding is null
				? new Delta2DBinding()
				: new Delta2DBinding
				{
					DeviceId = binding.DeviceId,
					ControlId = binding.ControlId,
					Sensitivity = binding.Sensitivity,
					ScaleX = binding.ScaleX,
					ScaleY = binding.ScaleY,
					InvertX = binding.InvertX,
					InvertY = binding.InvertY
				};
		}

		private static bool ButtonEquals(ButtonBinding left, ButtonBinding right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}

			if (left.DeviceId != right.DeviceId || left.ControlId != right.ControlId)
			{
				return false;
			}

			int leftLength = left.Modifiers.IsDefault ? 0 : left.Modifiers.Length;
			int rightLength = right.Modifiers.IsDefault ? 0 : right.Modifiers.Length;
			if (leftLength != rightLength)
			{
				return false;
			}

			for (int i = 0; i < leftLength; i++)
			{
				if (left.Modifiers[i] != right.Modifiers[i])
				{
					return false;
				}
			}

			return true;
		}

		private static bool Axis1DEquals(Axis1DBinding left, Axis1DBinding right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}

			return left.DeviceId == right.DeviceId
				&& left.ControlId == right.ControlId
				&& left.Deadzone == right.Deadzone
				&& left.Sensitivity == right.Sensitivity
				&& left.Scale == right.Scale
				&& left.Invert == right.Invert
				&& left.ResponseCurve == right.ResponseCurve;
		}

		private static bool Axis1DCompositeEquals(Axis1DCompositeBinding left, Axis1DCompositeBinding right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}

			return left.Negative == right.Negative
				&& left.Positive == right.Positive
				&& left.Scale == right.Scale
				&& left.Normalize == right.Normalize;
		}

		private static bool Axis2DEquals(Axis2DBinding left, Axis2DBinding right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}

			return left.DeviceId == right.DeviceId
				&& left.ControlId == right.ControlId
				&& left.Deadzone == right.Deadzone
				&& left.Sensitivity == right.Sensitivity
				&& left.ScaleX == right.ScaleX
				&& left.ScaleY == right.ScaleY
				&& left.InvertX == right.InvertX
				&& left.InvertY == right.InvertY
				&& left.ResponseCurve == right.ResponseCurve;
		}

		private static bool Axis2DCompositeEquals(Axis2DCompositeBinding left, Axis2DCompositeBinding right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}

			return left.Up == right.Up
				&& left.Down == right.Down
				&& left.Left == right.Left
				&& left.Right == right.Right
				&& left.Normalize == right.Normalize
				&& left.ScaleX == right.ScaleX
				&& left.ScaleY == right.ScaleY;
		}

		private static bool Delta2DEquals(Delta2DBinding left, Delta2DBinding right)
		{
			if (left is null || right is null)
			{
				return left is null && right is null;
			}

			return left.DeviceId == right.DeviceId
				&& left.ControlId == right.ControlId
				&& left.Sensitivity == right.Sensitivity
				&& left.ScaleX == right.ScaleX
				&& left.ScaleY == right.ScaleY
				&& left.InvertX == right.InvertX
				&& left.InvertY == right.InvertY;
		}
	}
}
