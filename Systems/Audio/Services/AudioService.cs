/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using NomadCore.Abstractions.Services;
using NomadCore.Enums.Audio;
using NomadCore.Enums.ConsoleSystem;
using NomadCore.Infrastructure;
using NomadCore.Interfaces.Audio;
using NomadCore.Interfaces.ConsoleSystem;
using NomadCore.Systems.Audio.Infrastructure.Godot;
using NomadCore.Utilities;

namespace NomadCore.Systems.Audio.Services {
	/*
	===================================================================================
	
	AudioService
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	public sealed class AudioService : IAudioService {
		public ICVar<float> SoundVolume => _soundVolume;
		private readonly ICVar<float> _soundVolume;

		public ICVar<float> MusicVolume => _musicVolume;
		private readonly ICVar<float> _musicVolume;

		private readonly ICVar<bool> SoundOn;
		private readonly ICVar<bool> MusicOn;

		/*
		===============
		AudioService
		===============
		*/
		public AudioService() {
			var cvarSystem = ServiceRegistry.Get<ICVarSystemService>();
			_soundVolume = cvarSystem.Register(
				new CVarCreateInfo<float>(
					name: "audio.SoundVolume",
					defaultValue: 50.0f,
					description: "The volume of in-game sound effects.",
					flags: CVarFlags.Archive
				)
			);
			_musicVolume = cvarSystem.Register(
				new CVarCreateInfo<float>(
					name: "audio.MusicVolume",
					defaultValue: 50.0f,
					description: "The volume of in-game music.",
					flags: CVarFlags.Archive
				)
			);
		}

		/*
		===============
		Initialize
		===============
		*/
		public void Initialize() {
		}

		/*
		===============
		Shutdown
		===============
		*/
		public void Shutdown() {
		}

		/*
		===============
		CreateSource
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IAudioSource CreateSource( SourceType type ) {
			return type switch {
				SourceType.UI or SourceType.Ambience or SourceType.Music => new GodotAudioSource( type ),
				SourceType.Entity => new GodotWorldAudioSource( type )
			};
		}
	};
};