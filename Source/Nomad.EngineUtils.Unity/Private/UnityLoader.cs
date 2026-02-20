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

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Core.ResourceCache;
using Nomad.Core.Util;
using UnityEngine;

namespace Nomad.EngineUtils.Private {
	/*
	===================================================================================
	
	UnityLoader
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	internal sealed class UnityLoader<Resource> : IResourceLoader<Resource, string>
		where Resource : UnityEngine.Object
	{
		public LoadCallback<Resource, string> Load => LoadResource;
		public LoadAsyncCallback<Resource, string> LoadAsync => LoadResourceAsync;

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private Result<Resource> LoadResource( string path ) {
			Resource resource = Resources.Load<Resource>( path );
			if ( resource == null ) {
				return Result<Resource>.Failure( Error.Create( $"Error loading unity resource '{path}'" ) );
			} else if ( resource is Resource loadedResource ) {
				return Result<Resource>.Success( loadedResource );
			}
			throw new InvalidCastException();
		}

		/*
        ===============
        LoadResourceAsync
        ===============
        */
		/// <summary>
		///
		/// </summary>
		/// <param name="path"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		private async Task<Result<Resource>> LoadResourceAsync( string path, CancellationToken ct = default ) {
			ct.ThrowIfCancellationRequested();

			ResourceRequest request = Resources.LoadAsync( path );
			if ( request == null ) {
				return Result<Resource>.Failure( Error.Create( $"Error loading unity resource '{path}'" ) );
			}
			return Result<Resource>.Success( await request );
		}
	};

	public static class ResourceRequestExtensions {
		public static TaskAwaiter<UnityEngine.Object> GetAwaiter( this ResourceRequest request ) {
			var tcs = new TaskCompletionSource<UnityEngine.Object>();
			var asyncOp = request;
			asyncOp.completed += _ => tcs.SetResult( asyncOp.asset ); // requires Unity 2020.3+?
			return tcs.Task.GetAwaiter();
		}
	};
};
#endif
