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
	/// A Unity-focused resource loader.
	/// </summary>
	internal sealed class UnityLoader : IResourceLoader {
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="TResource"></typeparam>
		/// <typeparam name="TId"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public Result<TResource> Load<TResource, TId>( TId id ) {
			if ( id is not string path ) {
				throw new InvalidCastException();
			}

			UnityEngine.Object resource = Resources.Load( path, typeof( TResource ) );
			if ( resource == null ) {
				return Result<TResource>.Failure( InternalError.Create( "Error loading unity resource '" + path + "'" ) );
			}

			if ( resource is TResource loadedResource ) {
				return Result<TResource>.Success( loadedResource );
			}

			throw new InvalidCastException();
		}

		/*
        ===============
        LoadAsync
        ===============
        */
		/// <summary>
		///
		/// </summary>
		/// <typeparam name="TResource"></typeparam>
		/// <typeparam name="TId"></typeparam>
		/// <param name="id"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<Result<TResource>> LoadAsync<TResource, TId>( TId id, CancellationToken ct = default ) {
			ct.ThrowIfCancellationRequested();

			if ( id is not string path ) {
				throw new InvalidCastException();
			}

			ResourceRequest request = Resources.LoadAsync( path, typeof( TResource ) );
			if ( request == null ) {
				return Result<TResource>.Failure( InternalError.Create( "Error loading unity resource '" + path + "'" ) );
			}

			UnityEngine.Object resource = await request;
			if ( resource is TResource loadedResource ) {
				return Result<TResource>.Success( loadedResource );
			}

			throw new InvalidCastException();
		}
	};

	/// <summary>
	///
	/// </summary>
	internal static class ResourceRequestExtensions {
		/// <summary>
		///
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static TaskAwaiter<UnityEngine.Object> GetAwaiter( this ResourceRequest request ) {
			var tcs = new TaskCompletionSource<UnityEngine.Object>();
			var asyncOperation = request;
			asyncOperation.completed += delegate { tcs.SetResult( asyncOperation.asset ); };
			return tcs.Task.GetAwaiter();
		}
	};
};
