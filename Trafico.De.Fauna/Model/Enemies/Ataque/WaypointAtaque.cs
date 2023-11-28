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

using System.Collections.Generic;
using TGC.Core.Mathematica;

namespace Trafico.De.Fauna.Model.Enemies.Ataque
{
    /// <summary>
    /// Waypoint para que el Guardia navegue por el sector en modo ataque
    /// </summary>
    public class WaypointAtaque
    {
        string name;
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        TGCVector3 position;
        /// <summary>
        /// Posicion del Waypoint
        /// </summary>
        public TGCVector3 Position
        {
            get { return position; }
        }

        int index;
        /// <summary>
        /// Indice del waypoint
        /// </summary>
        public int Index
        {
            set { index = value; }
            get { return index; }
        }

        public WaypointAtaque(string name, TGCVector3 position, int index)
        {
            this.name = name;
            this.position = position;
            this.index = index;
        }

        /// <summary>
        /// Comparador para ordenar por index
        /// </summary>
        public class WaypointAtaqueComparer : IComparer<WaypointAtaque>
        {
            public int Compare(WaypointAtaque x, WaypointAtaque y)
            {
                return x.index.CompareTo(y.index);
            }
        }

        public override string ToString()
        {
            return "name: " + name + ", index: " + index;
        }


        public void dispose()
        {
            
        }
    }
}
