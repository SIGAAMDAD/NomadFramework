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

namespace Steam.UserStats {
	/// <summary>
	/// Various steam api stats that are tracked in-game
	/// </summary>
	public enum StatID : uint {
		/// <summary>
		/// The number of kills in the user's multiplayer career
		/// </summary>
		LifetimeKills,

		/// <summary>
		/// The number of detahs in the user's multiplayer career
		/// </summary>
		LifetimeDeaths,

		/// <summary>
		/// The number of multiplayer games that the user has played
		/// </summary>
		LifetimeMatches,

		/// <summary>
		/// The number of wins in the user's multiplayer career
		/// </summary>
		LifetimeWins,

		/// <summary>
		/// The number of losses in the user's multiplayer career
		/// </summary>
		LifetimeLosses,
		
		/// <summary>
		/// The total amount of renown accrued across all time
		/// </summary>
		Renown,

		/// <summary>
		/// ...
		/// </summary>
		WeebCounter,

		/// <summary>
		/// The total amount of warcrimes that the user has commmited
		/// </summary>
		WarcrimeCounter,

		Count
	};
};