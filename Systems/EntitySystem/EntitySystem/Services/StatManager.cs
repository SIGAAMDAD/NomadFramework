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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NomadCore.Systems.EntitySystem.Interfaces;
using NomadCore.Systems.EntitySystem.Common;
using NomadCore.Infrastructure;
using NomadCore.Systems.EntitySystem.Events;

namespace NomadCore.Systems.EntitySystem.Services {
	/*
	===================================================================================
	
	StatManager

	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>

	public class StatManager : IStatService {
		/// <summary>
		/// The maximum natural health of an entity.
		/// </summary>
		public const float BASE_MAX_HEALTH = 100.0f;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		public Any this[ string statName ] {
			get {
				return Stats.TryGetValue( statName, out IEntityStat stat ) ? stat.Value : Any.From( 0 );
			}
			set {
				if ( Stats.TryGetValue( statName, out IEntityStat stat ) ) {
					stat.Value = value;
				}
			}
		}

		private readonly Dictionary<string, IEntityStat> Stats;		private readonly Entity? Owner;

		public readonly StatChanged StatChanged = new StatChanged();

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
		/// <param name="args"></param>
		public void Save( in SaveSystem.Events.SaveStartedEventData args ) {
		}

		/*
		===============
		Load
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void Load( in SaveSystem.Events.LoadStartedEventData args ) {
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
		public void SetStatValue<T>( string statName, T newValue ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				stat = new EntityStat<T>( newValue, default, default );
				Stats.Add( statName, stat );
			}
			stat.Value = Any.From( newValue );
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
		public void SetStatMaxValue<T>( string statName, T maxValue ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			stat.MaxValue = Any.From( maxValue );
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
		public void SetStatMinValue<T>( string statName, T minValue ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			stat.MinValue = Any.From( minValue );
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
			return stat.Value.GetValue<T>();
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
			return stat.MaxValue.GetValue<T>();
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
		public T GetStatMinValue<T>( string? statName ) where T : unmanaged {
			ArgumentException.ThrowIfNullOrEmpty( statName );

			if ( !Stats.TryGetValue( statName, out IEntityStat? stat ) ) {
				throw new KeyNotFoundException( statName );
			}
			return stat.MinValue.GetValue<T>();
		}
	};
};