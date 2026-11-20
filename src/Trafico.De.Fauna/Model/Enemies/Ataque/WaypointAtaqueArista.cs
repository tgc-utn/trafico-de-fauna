/*
Copyright 2012 Educ.ar S.E.

This file is part of "Trafico de Fauna".

"Trafico de Fauna" is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

"Trafico de Fauna" is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with "Trafico de Fauna".  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Trafico.De.Fauna.Model.Enemies.Ataque
{
    /// <summary>
    ///     Arista que uno dos Waypoints
    /// </summary>
    public class WaypointAtaqueArista
    {
        public WaypointAtaqueArista(WaypointAtaque w1, WaypointAtaque w2)
        {
            this.W1 = w1;
            this.W2 = w2;
        }

        /// <summary>
        ///     Waypoint 1
        /// </summary>
        public WaypointAtaque W1 { get; private set; }

        /// <summary>
        ///     Waypoint 2
        /// </summary>
        public WaypointAtaque W2 { get; private set; }

        public void dispose()
        {
            W1 = null;
            W2 = null;
        }
    }
}
