/*
===========================================================================
The Nomad MPLv2 Source Code
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

namespace Nomad.Core.Engine.Windowing
{
	/// <summary>
	/// 
	/// </summary>
	public readonly struct AspectRatioValue
	{
		/// <summary>
		/// 
		/// </summary>
		public AspectRatio Ratio => _ratio;
		private readonly AspectRatio _ratio;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ratio"></param>
		public AspectRatioValue(AspectRatio ratio)
		{
			_ratio = ratio;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ratio"></param>
		public AspectRatioValue(float ratio)
		{
			_ratio = AspectRatio.Aspect_Automatic;
			if (ratio == 4.0f / 3.0f)
			{
				_ratio = AspectRatio.Aspect_4_3;
			}
			else if (ratio == 16.0f / 9.0f)
			{
				_ratio = AspectRatio.Aspect_16_9;
			}
			else if (ratio == 16.0f / 10.0f)
			{
				_ratio = AspectRatio.Aspect_16_10;
			}
			else if (ratio == 21.0f / 9.0f)
			{
				_ratio = AspectRatio.Aspect_21_9;
			}
		}

		public static explicit operator float(AspectRatioValue ratio)
			=> ratio._ratio.GetRatio();
	}
}