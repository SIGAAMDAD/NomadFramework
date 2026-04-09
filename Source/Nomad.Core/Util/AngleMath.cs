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
using System.Runtime.CompilerServices;

namespace Nomad.Core.Util
{
	public static class AngleMath
	{
		/// <summary>
		/// Normalizes an angle to the range of [0 &lt;= angle &lt; 360]
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleNormalize360( float angle )
		{
			return (360.0f / 65536) * ((int)(angle * (65536 / 360.0f)) & 65535);
		}

		/// <summary>
		/// Normalizes an angle to the range of [-180 &lt; angle &lt;= 180]
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static float AngleNormalize180( float angle )
		{
			angle = AngleNormalize360( angle );
			if ( angle > 180.0f )
			{
				angle -= 360.0f;
			}
			return angle;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="angle1"></param>
		/// <param name="angle2"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleDelta( float angle1, float angle2 )
		{
			return AngleNormalize180( angle1 - angle2 );
		}

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float RadToDeg( float angle )
		{
			return ( angle * 180.0f ) / MathF.PI;
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DegToRad( float angle )
		{
			return ( angle * MathF.PI ) / 180.0f;
		}
	}
}