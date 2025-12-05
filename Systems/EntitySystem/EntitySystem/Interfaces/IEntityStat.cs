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

namespace NomadCore.Systems.EntitySystem.Interfaces {
	public interface IEntityStat : IComponent {
		public Any GetValue();
		public Any GetMinValue();
		public Any GetMaxValue();
		public Any GetBaseValue();
	};

	public interface IEntityStat<T> where T : unmanaged {
		public T Value { get; set; }
		public T MaxValue { get; set; }
		public T MinValue { get; set; }
		public T BaseValue { get; }
	};
};