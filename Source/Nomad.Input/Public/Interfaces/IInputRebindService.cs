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
using Nomad.Input.ValueObjects;

namespace Nomad.Input.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IInputRebindService
	{
		/// <summary>
		/// 
		/// </summary>
		bool IsRebinding { get; }

		/// <summary>
		/// 
		/// </summary>
		InputRebindRequest? CurrentRequest { get; }

		/// <summary>
		/// 
		/// </summary>
		event Action<InputRebindRequest>? RebindStarted;

		/// <summary>
		/// 
		/// </summary>
		event Action<InputRebindRequest>? RebindCanceled;

		/// <summary>
		/// 
		/// </summary>
		event Action<InputRebindResult>? RebindCompleted;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		bool BeginRebind(in InputRebindRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool CancelRebind();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <param name="binding"></param>
		/// <returns></returns>
		bool ApplyBinding(in InputRebindRequest request, in InputBindingDefinition binding);
	}
}
