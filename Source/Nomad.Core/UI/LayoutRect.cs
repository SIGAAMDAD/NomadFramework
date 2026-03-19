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

namespace Nomad.Core.UI
{
	/// <summary>
	/// 
	/// </summary>
	public readonly struct LayoutRect
	{
		/// <summary>
		/// 
		/// </summary>
		public float X => _x;
		private readonly float _x;

		/// <summary>
		/// 
		/// </summary>
		public float Y => _y;
		private readonly float _y;

		/// <summary>
		/// 
		/// </summary>
		public float Width => _width;
		private readonly float _width;

		/// <summary>
		/// 
		/// </summary>
		public float Height => _height;
		private readonly float _height;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public LayoutRect(float x, float y, float width, float height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}
	}
}