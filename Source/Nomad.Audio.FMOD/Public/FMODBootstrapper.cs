/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using Nomad.Audio.Fmod.Private.Registries;
using Nomad.Audio.Fmod.Private.Services;
using Nomad.Audio.Interfaces;
using Nomad.Core.Abstractions;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;
#if NET5_0_OR_GREATER
using Nomad.Core.Util;
#endif

namespace Nomad.Audio.Fmod
{
    /// <summary>
    /// Initializes FMOD.
    /// </summary>
    public class FMODBootstrapper : IBootstrapper
    {
        private FMODDevice _device;
        private FMODListenerService _listenerService;
        private FMODChannelService _channelService;

        /// <summary>
        /// Initializes the FMOD audio backend.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="locator"></param>
        public void Initialize(IServiceRegistry registry, IServiceLocator locator)
        {
            ArgumentGuard.ThrowIfNull(registry);
            ArgumentGuard.ThrowIfNull(locator);

            var logger = locator.GetService<ILoggerService>();
            var cvarSystem = locator.GetService<ICVarSystemService>();

#if NET5_0_OR_GREATER
            InteropAssemblyResolver.Hook(typeof(FMOD.Channel).Assembly, FMOD.VERSION.dll, FMOD.VERSION.dll, FMOD.VERSION.dll);
            InteropAssemblyResolver.Hook(typeof(FMOD.Studio.Bank).Assembly, FMOD.Studio.STUDIO_VERSION.dll, FMOD.Studio.STUDIO_VERSION.dll, FMOD.Studio.STUDIO_VERSION.dll);
#endif
            AudioCVars.Register(cvarSystem);

            _device = new FMODDevice(locator, registry);
            _listenerService = new FMODListenerService(logger, _device);
            _channelService = new FMODChannelService(logger, cvarSystem, _listenerService, _device);

            registry.AddSingleton<IAudioDevice>(_device);
            registry.AddSingleton<IListenerService>(_listenerService);
            registry.AddSingleton<IChannelRepository>(_channelService);
            registry.AddSingleton<IMusicService>(new FMODMusicService(_device.EventRepository, cvarSystem));
            registry.AddSingleton<IEmitterFactory>(new FMODEmitterFactory(_channelService, _channelService.BusRepository));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            _device?.Dispose();
            _listenerService?.Dispose();
            _channelService?.Dispose();
        }
    }
}
