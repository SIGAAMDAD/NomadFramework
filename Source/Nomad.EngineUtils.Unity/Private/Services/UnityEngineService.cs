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
using System.IO;
using Nomad.Core.Compatibility.Guards;
using Nomad.Core.CVars;
using Nomad.Core.Engine.Globals;
using Nomad.Core.Engine.SceneManagement;
using Nomad.Core.Engine.Services;
using Nomad.Core.Events;
using Nomad.Core.Input;
using Nomad.Core.Logger;
using Nomad.Core.ResourceCache;
using Nomad.Core.ServiceRegistry.Interfaces;
using Nomad.EngineUtils.Private;
using Nomad.EngineUtils.Private.SceneManagement;
using Nomad.EngineUtils.Private.Services;
using UnityEngine;
using UnityEngine.Windows;

namespace Nomad.EngineUtils {
	/// <summary>
	///
	/// </summary>
	internal sealed class UnityEngineService : MonoBehaviour, IEngineService {
		private readonly UnityLoader _loader;
		private readonly UnityInputPump _inputPump;

		private readonly IWindowService _windowService;
		private readonly ILocalizationService _localizationService;
		private readonly IDisplayService _displayService;
		private readonly ISceneManager _sceneManager;
		
		private readonly ILoggerService _logger;
		private readonly IGameEventRegistryService _eventFactory;

		private bool _isDisposed = false;

		/// <summary>
		///
		/// </summary>
		/// <param name="serviceFactory"></param>
		/// <param name="locator"></param>
		public UnityEngineService( IServiceRegistry serviceFactory, IServiceLocator locator ) {
			ArgumentGuard.ThrowIfNull( serviceFactory );
			ArgumentGuard.ThrowIfNull( locator );

			if ( locator.TryGetService<IInputSystem>( out var inputSystem ) ) {
				_inputPump = gameObject.AddComponent<UnityInputPump>();
				_inputPump.Initialize( inputSystem );
			}

			_logger = locator.GetService<ILoggerService>();
			_eventFactory = locator.GetService<IGameEventRegistryService>();
			ICVarSystemService cvarSystem = locator.GetService<ICVarSystemService>();

			_loader = new UnityLoader();

			_windowService = new UnityWindowService( _eventFactory );
			serviceFactory.AddSingleton( _windowService );
			_windowService.CloseRequested.Subscribe( OnWindowCloseRequested );

			_localizationService = new UnityLocalizationService();
			serviceFactory.AddSingleton( _localizationService );

			_sceneManager = new UnitySceneManager();
			serviceFactory.AddSingleton( _sceneManager );

			_displayService = new UnityDisplayService( _windowService, cvarSystem );
			serviceFactory.AddSingleton( _displayService );
			serviceFactory.AddSingleton<ISplitScreenService>( new UnitySplitScreenService() );

			_logger.AddSink( new UnityDebugSink() );

			EngineService.Initialize( this );
		}

		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			if ( _isDisposed ) {
				return;
			}

			_windowService.Dispose();
			_sceneManager.Dispose();

			_isDisposed = true;
			GC.SuppressFinalize( this );
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool IsApplicationFocused() {
			return _windowService.IsFocused;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string GetApplicationVersion() {
			return Application.version;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string GetEngineVersion() {
			return Application.unityVersion;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="localpath"></param>
		/// <returns></returns>
		public string GetOSPath( string localpath ) {
			if ( string.IsNullOrWhiteSpace( localpath ) ) {
				return string.Empty;
			}

			string normalizedPath = NormalizePath( localpath );
			if ( Path.IsPathRooted( normalizedPath ) ) {
				return normalizedPath;
			}

			if ( normalizedPath.StartsWith( "Assets/StreamingAssets", StringComparison.OrdinalIgnoreCase ) ) {
				string relativePath = normalizedPath.Substring( "Assets/StreamingAssets".Length ).TrimStart( '/' );
				return NormalizePath( Path.Combine( Application.streamingAssetsPath, relativePath ) );
			}

			if ( normalizedPath.StartsWith( "Assets", StringComparison.OrdinalIgnoreCase ) ) {
				string projectRoot = System.IO.Directory.GetParent( Application.dataPath ) != null
					? System.IO.Directory.GetParent( Application.dataPath ).FullName
					: Application.dataPath;
				return NormalizePath( Path.Combine( projectRoot, normalizedPath ) );
			}

			return NormalizePath( Path.GetFullPath( normalizedPath ) );
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="ospath"></param>
		/// <returns></returns>
		public string GetLocalPath( string ospath ) {
			if ( string.IsNullOrWhiteSpace( ospath ) ) {
				return string.Empty;
			}

			string normalizedPath = NormalizePath( ospath );
			string normalizedStreamingAssetsPath = NormalizePath( Application.streamingAssetsPath );
			if ( normalizedPath.StartsWith( normalizedStreamingAssetsPath, StringComparison.OrdinalIgnoreCase ) ) {
				return "Assets/StreamingAssets" + normalizedPath.Substring( normalizedStreamingAssetsPath.Length );
			}

			string normalizedDataPath = NormalizePath( Application.dataPath );
			if ( normalizedPath.StartsWith( normalizedDataPath, StringComparison.OrdinalIgnoreCase ) ) {
				return "Assets" + normalizedPath.Substring( normalizedDataPath.Length );
			}

			return normalizedPath;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string GetStoragePath( StorageScope scope ) => scope switch {
			StorageScope.Install => NormalizePath( System.IO.Directory.GetParent( Application.dataPath ) != null ? System.IO.Directory.GetParent( Application.dataPath ).FullName : Application.dataPath ),
			StorageScope.StreamingAssets => NormalizePath( Application.streamingAssetsPath ),
			StorageScope.UserData => NormalizePath( Application.persistentDataPath ),
			StorageScope.Documents => NormalizePath( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) ),
			StorageScope.Temporary => NormalizePath( Application.temporaryCachePath ),
			_ => throw new ArgumentOutOfRangeException( nameof( scope ) ),
		};

		/// <summary>
		///
		/// </summary>
		/// <param name="relativePath"></param>
		/// <param name="scope"></param>
		/// <returns></returns>
		public string GetStoragePath( string relativePath, StorageScope scope ) {
			return NormalizePath( Path.Combine( GetStoragePath( scope ), relativePath ) );
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string GetSystemRegion() {
			return System.Globalization.CultureInfo.CurrentCulture.Name;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="image"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public IDisposable CreateImageRGBA( byte[] image, int width, int height ) {
			var texture = new Texture2D( width, height, TextureFormat.RGBA32, false );
			texture.LoadRawTextureData( image );
			texture.Apply( false, false );
			return new UnityImageHandle( texture );
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IConsoleObject CreateConsoleObject() {
			return new UnityConsole( _logger, _eventFactory );
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IResourceLoader GetResourceLoader() {
			return _loader;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="exitCode"></param>
		public void Quit( int exitCode = 0 ) {
			Application.Quit();
		}

		private void OnWindowCloseRequested( in EmptyEventArgs args ) {
			Quit();
		}

		private static string NormalizePath( string path ) {
			return path.Replace( '\\', '/' );
		}

		private sealed class UnityImageHandle : IDisposable {
			private readonly Texture2D _texture;

			private bool _disposed;

			public UnityImageHandle( Texture2D texture ) {
				_texture = texture;
			}

			public void Dispose() {
				if ( _disposed ) {
					return;
				}

				if ( _texture != null ) {
					UnityEngine.Object.Destroy( _texture );
				}

				_disposed = true;
				GC.SuppressFinalize( this );
			}
		}
	}
}
