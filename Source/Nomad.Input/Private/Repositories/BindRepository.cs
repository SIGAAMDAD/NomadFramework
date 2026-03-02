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
using Nomad.Core.CVars;
using Nomad.Core.Exceptions;
using Nomad.Core.FileSystem;
using Nomad.Input.Interfaces;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Private.Repositories {
	/*
	===================================================================================
	
	BindRepository
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class BindRepository : IDisposable {
		private readonly Dictionary<BindKey, IInputAction> _bindings;
		private readonly Dictionary<InputEventData, BindKey> _bindTriggers;

		private readonly ImmutableArray<InputEventData, IInputBinding> _defaultMap;
		private readonly BindLoader _loader;

		private bool _isDisposed = false;

		/*
		===============
		BindRepository
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <param name="cvarSystem"></param>
		public BindRepository( IFileSystem fileSystem, ICVarSystemService cvarSystem ) {
			_loader = new BindLoader( fileSystem );

			var defaultsPath = cvarSystem.GetCVar<string>( Constants.CVars.DEFAULTS_PATH ) ?? throw new CVarMissing( Constants.CVars.DEFAULTS_PATH );
			_loader.LoadBindDatabase( defaultsPath.Value, out var defaults );
			if ( defaults == null ) {
				// TODO: throw error
			}
		}

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			if ( !_isDisposed ) {
			}
			GC.SuppressFinalize( this );
			_isDisposed = true;
		}

		public static IInputAction? MapInput( InputEventData eventData ) {
			switch ( eventData.Type ) {
				case InputType.Keyboard:

					break;
				case InputType.GamepadButton:
					break;
				case InputType.GamepadMotion:
					break;
				case InputType.MouseButton:
					break;
				case InputType.MouseMotion:
					break;
			}
			return null;
		}
	};
};
