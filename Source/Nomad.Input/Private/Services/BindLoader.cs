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
using System.Text.Json;
using Nomad.Core.FileSystem;
using Nomad.Core.Input;
using Nomad.Core.Input.ValueObjects;
using Nomad.Core.Util;
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
		public bool LoadBindDatabase( string filePath, out Dictionary<BindKey, InputEventData>? binds ) {
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

			binds = new Dictionary<BindKey, InputEventData>();
			foreach ( var mapping in database.Bindings ) {
				var key = new BindKey( new InternString( mapping.Key ), mapping.Value.);
				binds[mapping.Key] = new Dictionary<BindKey, InputEventData>();
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
		private static KeyboardEvent ParseKeyboardMapping( InputMapping mapping ) {
			if ( !Enum.TryParse( typeof( KeyNum ), mapping.ButtonId, true, out var keyNum ) ) {
				throw new Exception( $"Invalid InputMapping ButtonId '{mapping.ButtonId}' when loading Keyboard binding, it must be a valid KeyNum!" );
			}
			return new KeyboardEvent( (KeyNum)keyNum, 0, mapping.Value == 1.0f );
		}

		private static MouseButtonEvent ParseMouseButtonMapping( InputMapping mapping ) {
			if ( !Enum.TryParse( typeof( MouseButton ), mapping.ButtonId, true, out var mouseButton ) ) {
				throw new Exception( $"Invalid InputMapping ButtonId '{mapping.ButtonId}' when loading MouseButton binding, it must be a valid MouseButton!" );
			}
			return new MouseButtonEvent( (MouseButton)mouseButton, 0, mapping.Value == 1.0f );
		}

		private static InputEventData ParseInputEvent( InputMapping mapping ) {
			InputEventData data;

			if ( mapping.DeviceId.Equals( Constants.KEYBOARD_DEVICE_ID, StringComparison.InvariantCulture ) ) {
				data = new InputEventData( ParseKeyboardMapping( mapping ) );
			} else if ( mapping.DeviceId.Equals( Constants.MOUSE_DEVICE_ID, StringComparison.InvariantCulture ) ) {
				data = new InputEventData();
			} else if ( mapping.DeviceId.Equals( Constants.GAMEPAD_DEVICE_ID, StringComparison.InvariantCulture ) ) {
				
			} else {
				throw new Exception( $"Invalid DeviceId '{mapping.DeviceId}' in bindings file!" );
			}

			return data;
		}
	};
};
