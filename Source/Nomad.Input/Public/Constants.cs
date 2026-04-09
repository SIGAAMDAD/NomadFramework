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

using Nomad.Core.Input;

namespace Nomad.Input
{
	/// <summary>
	/// 
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// 
		/// </summary>
		public static class Events
		{
			/// <summary>
			/// 
			/// </summary>
			public const string NAMESPACE = "Nomad.Input";

			/// <summary>
			/// 
			/// </summary>
			public const string AXIS_ACTION = NAMESPACE + ".AxisAction";

			/// <summary>
			/// 
			/// </summary>
			public const string FLOAT_ACTION = NAMESPACE + ".FloatAction";

			/// <summary>
			/// 
			/// </summary>
			public const string BUTTON_ACTION = NAMESPACE + ".ButtonAction";

			/// <summary>
			/// Event name for whenever a rebinding process results in a bind collision.
			/// </summary>
			public const string BIND_COLLISION = NAMESPACE + ".BindCollision";
		}
	}
}