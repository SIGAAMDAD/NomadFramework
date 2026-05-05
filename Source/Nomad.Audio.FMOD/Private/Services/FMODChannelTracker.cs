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

#if false
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Nomad.Audio.Fmod.Private.Entities;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Audio.Fmod.Private.Repositories {
	/*
	===================================================================================
	
	FMODChannelTracker
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class FMODChannelTracker {
		private const int INVALID_INDEX = -1;

		private readonly FMODChannel?[] _channelsById;
		private readonly int[] _freeIds;
		private int _freeCount;

		private readonly int[] _activeIds;
		private readonly int[] _activeIndexByChannelId;
		private int _activeCount;

		private readonly InstanceHandleMap _instanceToHandle;
		private readonly StringCounterMap _categoryCounts;
		private readonly ConcurrentQueue<nint> _finishedInstances = new ConcurrentQueue<nint>();

		public FMODChannelTracker( int capacity ) {
			RangeGuard.ThrowIfNegativeOrZero( capacity, nameof( capacity ) );
			if ( capacity <= 0 ) {
				throw new ArgumentOutOfRangeException( nameof( capacity ) );
			}

			_channelsById = new FMODChannel?[capacity];
			_freeIds = new int[capacity];
			_activeIds = new int[capacity];
			_activeIndexByChannelId = new int[capacity];

			Array.Fill( _activeIndexByChannelId, INVALID_INDEX );

			// stack, not queue: lower overhead and trivially cache-friendly
			for ( int i = 0; i < capacity; i++ ) {
				_freeIds[i] = capacity - 1 - i;
			}
			_freeCount = capacity;

			_instanceToHandle = new InstanceHandleMap( capacity * 2 );
			_categoryCounts = new StringCounterMap( Math.Max( 8, capacity ) );
		}

		public int Capacity {
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			get => _channelsById.Length;
		}

		public int ActiveCount {
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			get => _activeCount;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryAllocateChannelId( out int channelId ) {
			if ( _freeCount == 0 ) {
				channelId = -1;
				return false;
			}

			channelId = _freeIds[--_freeCount];
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Register( FMODChannel channel, FMODChannelHandle handle ) {
			int channelId = channel.ChannelId;

			_channelsById[channelId] = channel;
			_activeIndexByChannelId[channelId] = _activeCount;
			_activeIds[_activeCount++] = channelId;

			_instanceToHandle.Set( (nint)channel.Instance, handle );
			_categoryCounts.Increment( channel.Category.Config.Name );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void Unregister( FMODChannel channel ) {
			int channelId = channel.ChannelId;
			int activeIndex = _activeIndexByChannelId[channelId];

			_instanceToHandle.Remove( (nint)channel.Instance );

			if ( activeIndex != INVALID_INDEX ) {
				int lastIndex = --_activeCount;
				int movedChannelId = _activeIds[lastIndex];

				if ( activeIndex != lastIndex ) {
					_activeIds[activeIndex] = movedChannelId;
					_activeIndexByChannelId[movedChannelId] = activeIndex;
				}

				_activeIds[lastIndex] = 0;
				_activeIndexByChannelId[channelId] = INVALID_INDEX;
			}

			_channelsById[channelId] = null;
			_freeIds[_freeCount++] = channelId;

			_categoryCounts.Decrement( channel.Category.Config.Name );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryGetChannel( FMODChannelHandle? handle, out FMODChannel channel ) {
			channel = null!;

			if ( handle == null || !handle.IsValid ) {
				return false;
			}

			int channelId = handle.ChannelId;
			if ( (uint)channelId >= (uint)_channelsById.Length ) {
				return false;
			}

			FMODChannel? existing = _channelsById[channelId];
			if ( existing == null ) {
				return false;
			}

			if ( existing.Generation != handle.Generation ) {
				return false;
			}

			if ( !existing.Instance.IsValid ) {
				return false;
			}

			channel = existing;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryGetChannelByInstance( nint instanceHandle, out FMODChannel channel ) {
			channel = null!;

			if ( !_instanceToHandle.TryGet( instanceHandle, out FMODChannelHandle? handle ) || handle == null ) {
				return false;
			}

			return TryGetChannel( handle, out channel );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public FMODChannel GetActiveChannel( int activeIndex ) {
			return _channelsById[_activeIds[activeIndex]]!;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int CountSoundsInCategory( string categoryName ) {
			return _categoryCounts.GetValueOrDefault( categoryName );
		}

		public bool TryFindLowestPriorityInCategory( string categoryName, out FMODChannel? lowestPriorityChannel ) {
			lowestPriorityChannel = null;
			float lowestPriority = float.MaxValue;

			for ( int i = 0; i < _activeCount; i++ ) {
				FMODChannel channel = _channelsById[_activeIds[i]]!;

				if ( !channel.IsPlaying || channel.IsEssential ) {
					continue;
				}

				if ( channel.Category.Config.Name != categoryName ) {
					continue;
				}

				if ( channel.CurrentPriority < lowestPriority ) {
					lowestPriority = channel.CurrentPriority;
					lowestPriorityChannel = channel;
				}
			}

			return lowestPriorityChannel != null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void EnqueueFinishedInstance( nint instanceHandle ) {
			_finishedInstances.Enqueue( instanceHandle );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool TryDequeueFinishedInstance( out nint instanceHandle ) {
			return _finishedInstances.TryDequeue( out instanceHandle );
		}

		private sealed class InstanceHandleMap {
			private nint[] _keys;
			private FMODChannelHandle?[] _values;
			private byte[] _states; // 0 = empty, 1 = used, 2 = tombstone
			private int _count;
			private int _mask;

			public InstanceHandleMap( int capacity ) {
				int size = NextPowerOfTwo( Math.Max( 8, capacity ) );
				_keys = new nint[size];
				_values = new FMODChannelHandle?[size];
				_states = new byte[size];
				_mask = size - 1;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public bool TryGet( nint key, out FMODChannelHandle? value ) {
				int index = FindIndex( key );
				if ( index < 0 ) {
					value = null;
					return false;
				}

				value = _values[index];
				return true;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public void Set( nint key, FMODChannelHandle value ) {
				if ( ( _count + 1 ) * 2 >= _keys.Length ) {
					Resize( _keys.Length << 1 );
				}

				InsertOrUpdate( key, value );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public void Remove( nint key ) {
				int index = FindIndex( key );
				if ( index < 0 ) {
					return;
				}

				_states[index] = 2;
				_values[index] = null;
				_keys[index] = 0;
				_count--;
			}

			private void InsertOrUpdate( nint key, FMODChannelHandle value ) {
				int index = Hash( key ) & _mask;
				int firstTombstone = -1;

				while ( true ) {
					byte state = _states[index];

					if ( state == 0 ) {
						int target = firstTombstone >= 0 ? firstTombstone : index;
						_states[target] = 1;
						_keys[target] = key;
						_values[target] = value;
						_count++;
						return;
					}

					if ( state == 2 ) {
						if ( firstTombstone < 0 ) {
							firstTombstone = index;
						}
					} else if ( _keys[index] == key ) {
						_values[index] = value;
						return;
					}

					index = ( index + 1 ) & _mask;
				}
			}

			private int FindIndex( nint key ) {
				int index = Hash( key ) & _mask;

				while ( true ) {
					byte state = _states[index];
					if ( state == 0 ) {
						return -1;
					}
					if ( state == 1 && _keys[index] == key ) {
						return index;
					}
					index = ( index + 1 ) & _mask;
				}
			}

			private void Resize( int newSize ) {
				nint[] oldKeys = _keys;
				FMODChannelHandle?[] oldValues = _values;
				byte[] oldStates = _states;

				_keys = new nint[newSize];
				_values = new FMODChannelHandle?[newSize];
				_states = new byte[newSize];
				_mask = newSize - 1;
				_count = 0;

				for ( int i = 0; i < oldKeys.Length; i++ ) {
					if ( oldStates[i] == 1 && oldValues[i] != null ) {
						InsertOrUpdate( oldKeys[i], oldValues[i]! );
					}
				}
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			private static int Hash( nint value ) {
				return value.GetHashCode();
			}
		}

		private sealed class StringCounterMap {
			private string?[] _keys;
			private int[] _values;
			private int _mask;
			private int _count;

			public StringCounterMap( int capacity ) {
				int size = NextPowerOfTwo( Math.Max( 8, capacity ) );
				_keys = new string?[size];
				_values = new int[size];
				_mask = size - 1;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public int GetValueOrDefault( string key ) {
				int index = FindIndex( key );
				return index >= 0 ? _values[index] : 0;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public void Increment( string key ) {
				int index = GetOrCreateIndex( key );
				_values[index]++;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public void Decrement( string key ) {
				int index = FindIndex( key );
				if ( index >= 0 && _values[index] > 0 ) {
					_values[index]--;
				}
			}

			private int GetOrCreateIndex( string key ) {
				if ( ( _count + 1 ) * 2 >= _keys.Length ) {
					Resize( _keys.Length << 1 );
				}

				int index = Hash( key ) & _mask;
				while ( true ) {
					string? existing = _keys[index];

					if ( existing == null ) {
						_keys[index] = key;
						_values[index] = 0;
						_count++;
						return index;
					}

					if ( existing == key || StringComparer.Ordinal.Equals( existing, key ) ) {
						return index;
					}

					index = ( index + 1 ) & _mask;
				}
			}

			private int FindIndex( string key ) {
				int index = Hash( key ) & _mask;
				while ( true ) {
					string? existing = _keys[index];
					if ( existing == null ) {
						return -1;
					}

					if ( existing == key || StringComparer.Ordinal.Equals( existing, key ) ) {
						return index;
					}

					index = ( index + 1 ) & _mask;
				}
			}

			private void Resize( int newSize ) {
				string?[] oldKeys = _keys;
				int[] oldValues = _values;

				_keys = new string?[newSize];
				_values = new int[newSize];
				_mask = newSize - 1;
				_count = 0;

				for ( int i = 0; i < oldKeys.Length; i++ ) {
					string? key = oldKeys[i];
					if ( key == null ) {
						continue;
					}

					int index = GetOrCreateIndex( key );
					_values[index] = oldValues[i];
				}
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			private static int Hash( string key ) {
				return StringComparer.Ordinal.GetHashCode( key );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private static int NextPowerOfTwo( int value ) {
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value++;
			return value < 8 ? 8 : value;
		}
	}
}
#endif
