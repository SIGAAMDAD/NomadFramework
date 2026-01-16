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
using System.Runtime.InteropServices;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================

	FMODChannelStealTracker

	===================================================================================
	*/
	/// <summary>
	/// Tracks channel steals.
	/// </summary>

	internal sealed class FMODChannelStealTracker {
		[StructLayout( LayoutKind.Sequential, Pack = 16 )]
		private struct StolenChannelData {
			public IntPtr Id;
			public int StealCount;
			public float LastStealTime;
		};

		private readonly StolenChannelData[] _buffer;
		private readonly Dictionary<IntPtr, int> _indexMap;
		private int _stealChannelCount = 0;

		/*
		===============
		FMODChannelStealTracker
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="maxChannels"></param>
		public FMODChannelStealTracker( int maxChannels ) {
			_buffer = new StolenChannelData[ maxChannels ];
			_indexMap = new Dictionary<nint, int>( maxChannels );
		}

		/*
		===============
		AddOrUpdate
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="stolenChannelId"></param>
		/// <param name="stealTime"></param>
		public void AddOrUpdate( IntPtr stolenChannelId, float stealTime ) {
			ref int index = ref CollectionsMarshal.GetValueRefOrAddDefault( _indexMap, stolenChannelId, out bool exists );

			if ( !exists ) {

				index = _stealChannelCount++;
				_buffer[ index ].Id = stolenChannelId;
				_buffer[ index ].StealCount = 0;
			}

			ref StolenChannelData channel = ref _buffer[ index ];
			channel.LastStealTime = stealTime;
			channel.StealCount++;
		}
	};
};
