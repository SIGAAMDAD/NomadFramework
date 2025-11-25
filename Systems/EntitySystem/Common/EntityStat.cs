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

using NomadCore.Infrastructure;
using NomadCore.Systems.EntitySystem.Interfaces;

namespace NomadCore.Systems.EntitySystem.Common {
	/*
	===================================================================================
	
	EntityStat
	
	===================================================================================
	*/
	/// <summary>
	/// For storing entity stats such as max health, max rage, etc.
	/// made this way to allow for rune and boon buffs/debuffs
	/// </summary>
	/// <typeparam name="T">The type of stat, must be a primitive.</typeparam>
	/// <remarks>
	/// 
	/// </remarks>
	/// <param name="value"></param>
	/// <param name="maxValue"></param>
	/// <param name="minValue"></param>

	internal struct EntityStat<T>( T value, T minValue, T maxValue ) : IEntityStat where T : unmanaged {
		/// <summary>
		/// The maximum value allowed for the stat
		/// </summary>
		public Any MaxValue {
			readonly get => _maxValue;
			set => _maxValue = value;
		}
		private Any _maxValue = Any.From( maxValue );

		/// <summary>
		/// The maximum value allowed for the stats
		/// </summary>
		public Any MinValue {
			readonly get => _minValue;
			set => _minValue = value;
		}
		private Any _minValue = Any.From( minValue );

		/// <summary>
		/// The base value loaded or determined at compile time
		/// </summary>
		public readonly Any BaseValue => _baseValue;
		private readonly Any _baseValue = Any.From( value );

		/// <summary>
		/// The value that can be changed and the value that is read at runtime
		/// </summary>
		public Any Value {
			readonly get => _value;
			set => _value = value;
		}
		private Any _value = Any.From( value );

		/*
		===============
		Reset
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		public void Reset() {
			_value = _baseValue;
		}
	};
};