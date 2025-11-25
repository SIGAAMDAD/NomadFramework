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

using EventSystem;
using System;
using Godot;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EntitySystem {
	/*
	===================================================================================
	
	StatManager

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public class StatManager {
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public readonly struct StatChangedEventData : IEventArgs {
			public readonly string Name;
			public readonly object Value;

			/*
			===============
			StatChangedEventData
			===============
			*/
			/// <summary>
			/// 
			/// </summary>
			/// <param name="name"></param>
			/// <param name="value"></param>
			public StatChangedEventData( string? name, object value ) {
				ArgumentException.ThrowIfNullOrEmpty( name );

				Name = name;
				Value = value;
			}
		};

		/// <summary>
		/// The maximum natural health of an entity.
		/// </summary>
		public const float BASE_MAX_HEALTH = 100.0f;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		public object this[ string statName ] {
			get {
				return Stats.TryGetValue( statName, out IEntityStat stat ) ? stat.Value : 0.0f;
			}
			set {
				if ( Stats.TryGetValue( statName, out IEntityStat stat ) ) {
					stat.Value = value;
				}
			}
		}

		/// <summary>
		/// The player's current health.
		/// </summary>
		public float Health {
			get => GetStatValue<float>( nameof( Health ) );
			set {
				if ( GetStatValue<float>( nameof( Health ) ) == value ) {
					return;
				}
				SetStatValue( nameof( Health ), value );
				HealthChanged.Publish( new StatChangedEventData( nameof( Health ), value ) );
			}
		}

		private readonly Dictionary<string, IEntityStat> Stats;
		private readonly Entity? Owner;

		public readonly GameEvent StatChanged = new GameEvent( nameof( StatChanged ) );
		public readonly GameEvent HealthChanged = new GameEvent( nameof( HealthChanged ) );
		public readonly GameEvent RageChanged = new GameEvent( nameof( RageChanged ) );
		public readonly GameEvent SanityChanged = new GameEvent( nameof( SanityChanged ) );
		public readonly GameEvent DashBurnoutChanged = new GameEvent( nameof( DashBurnoutChanged ) );

		/*
		===============
		StatManager
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public StatManager( Entity? owner, Dictionary<string, IEntityStat>? stats ) {
			ArgumentNullException.ThrowIfNull( owner );
			ArgumentNullException.ThrowIfNull( stats );

			Owner = owner;
			Stats = stats;
		}

		/*
		===============
		Save
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Save( in IGameEvent eventData, in IEventArgs args ) {
			if ( args is SaveSystem.SaveManager.SaveSectionNodesEventData saveNodes ) {
				Dictionary<string, object> data = new Dictionary<string, object>();
				foreach ( var stat in Stats ) {
					data
				}

				saveNodes.Slot.AddSection( $"StatManager{Owner.GetPath()}", new Dictionary<string, object> {
					{ "StatCount", Stats.Count },
				} );
				int index = 0;
				foreach ( var stat in Stats ) {
					writer.SaveString( $"State{index}Name", stat.Key );
					writer.SaveFloat( $"Stat{index}Min", stat.Value.MinValue );
					writer.SaveFloat( $"Stat{index}Max", stat.Value.MaxValue );
					writer.SaveFloat( $"Stat{index}Value", stat.Value.Value );
					index++;
				}
			} else {
				throw new InvalidCastException( nameof( args ) );
			}
		}

		/*
		===============
		Load
		===============
		*/
			/// <summary>
			/// 
			/// </summary>
			/// <param name="reader"></param>
			/// <exception cref="ArgumentNullException"></exception>
		public void Load( SaveSystem.SaveSectionReader? reader ) {
			if ( !reader.HasValue ) {
				throw new ArgumentNullException( nameof( reader ) );
			}

			int statCount = reader.Value.LoadInt( "StatCount" );
			Dictionary<string, IEntityStat> statCache = new Dictionary<string, IEntityStat>( statCount );

			for ( int i = 0; i < statCount; i++ ) {
				string name = reader.Value.LoadString( $"Stat{i}Name" );
				statCache.Add(
					reader.Value.LoadString( $"Stat{i}Name" ),
					new EntityStat<float>(
						reader.Value.LoadFloat( $"Stat{i}Value" ),
						reader.Value.LoadFloat( $"Stat{i}Min" ),
						reader.Value.LoadFloat( $"Stat{i}Max" )
					)
				);
			}
		}

		/*
		===============
		SetStatValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="statName">The name of statistic.</param>
		/// <param name="newValue"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetStatValue<T>( string? statName, T newValue ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				stat = new EntityStat<T>( newValue, 0, 0 );
				Stats.Add( statName, stat );
			}
			stat.Value = Mathf.Clamp( newValue, stat.MinValue, stat.MaxValue );
			StatChanged.Publish( new StatChangedEventData( statName, newValue ) );
		}

		/*
		===============
		SetStatMaxValue
		===============
		*/
		/// <summary>
		/// Sets the maximum value for <paramref name="statName"/>
		/// </summary>
		/// <param name="statName">The name of the statistic.</param>
		/// <param name="maxValue"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetStatMaxValue<T>( string? statName, T maxValue ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			stat.MaxValue = maxValue;
		}

		/*
		===============
		SetStatMinValue
		===============
		*/
		/// <summary>
		/// Sets the minimum value for <paramref name="statName"/>s
		/// </summary>
		/// <param name="statName">The name of the statistic</param>
		/// <param name="minValue"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void SetStatMinValue<T>( string? statName, T minValue ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			stat.MinValue = minValue;
		}

		/*
		===============
		GetStatValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T GetStatValue<T>( string? statName ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			return (T)stat.Value;
		}

		/*
		===============
		GetStatMaxValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T GetStatMaxValue<T>( string? statName ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			return (T)stat.MaxValue;
		}

		/*
		===============
		GetStatMinValue
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T GetStatMinValue<T>( string? statName ) {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			return (T)stat.MinValue;
		}
	};
};