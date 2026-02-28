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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using Nomad.Core.FileSystem;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Services {
	/*
	===================================================================================
	
	BindLoader
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class BindLoader {
		private readonly IFileSystem _fileSystem;

		/*
		===============
		BindLoader
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public BindLoader( IFileSystem fileSystem ) {
			_fileSystem = fileSystem;
		}

		/*
		===============
		LoadBindDatabase
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="binds"></param>
		/// <returns></returns>
		public bool LoadBindDatabase( string filePath, out ImmutableDictionary<BindKey, InputEventData>? binds ) {
			if ( !_fileSystem.FileExists( filePath ) ) {
				binds = null;
				return false;
			}

			var database = JsonSerializer.Deserialize<BindMapping>(
				_fileSystem.LoadFile( filePath )!.Span,
				new JsonSerializerOptions {
					ReadCommentHandling = JsonCommentHandling.Skip,
					AllowTrailingCommas = true,
					PropertyNameCaseInsensitive = true
				}
			);

			var mapping = new Dictionary<BindKey, InputEventData>();
			foreach ( var bind in database.Bindings ) {
				mapping[ bind. ];
			}

			return true;
		}

		/*
		===============
		ParseKeyboardMapping
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private KeyboardEvent ParseKeyboardMapping( InputMapping mapping ) {
			if ( !Enum.TryParse( typeof( KeyNum ), mapping.ButtonId, true, out var keyNum ) ) {
				throw new Exception( $"Invalid InputMapping ButtonId '{mapping.ButtonId}' when loading Keyboard binding, it must be a valid KeyNum!" );
			}

			return new KeyboardEvent( ( KeyNum )keyNum, )
		}

		private InputEventData ParseInputEvent( InputMapping mapping ) {
			InputEventData data;

			switch ( mapping.DeviceId ) {
				case Constants.KEYBOARD_DEVICE_ID:
					data = new InputEventData(
						new KeyboardEvent()
					);
					break;
				case Constants.GAMEPAD_DEVICE_ID:
					break;
				default:
					throw new Exception( $"Invalid DeviceId '{mapping.DeviceId}' in bindings file!" );
			}

			return data;
		}
	};
};
