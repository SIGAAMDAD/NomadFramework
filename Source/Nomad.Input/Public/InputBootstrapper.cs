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

using Nomad.Core.Abstractions;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Input;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.Input.Private.Services;

namespace Nomad.Input
{
    /// <summary>
    /// Initializes the Input subsystem.
    /// </summary>
    public sealed class InputBootstrapper : IBootstrapper
    {
		private IInputSystem? _inputSystem;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		/// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
		{
			ArgumentGuard.ThrowIfNull(registry);
			ArgumentGuard.ThrowIfNull(locator);

			_inputSystem = new InputSystem(
				locator.GetService<IFileSystem>(),
				locator.GetService<ICVarSystemService>(),
				locator.GetService<IGameEventRegistryService>()
			);
			registry.AddSingleton(_inputSystem);
		}

		/// <summary>
		/// 
		/// </summary>
        public void Shutdown()
		{
			_inputSystem?.Dispose();
		}
    }
}