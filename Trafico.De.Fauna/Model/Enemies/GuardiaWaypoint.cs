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
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;

namespace Trafico.De.Fauna.Model.Enemies
{
    public class GuardiaWaypoint
    {
        private TGCVector3 center;

        public GuardiaWaypoint(Guardia guardia, TGCVector3 center, int index, Dictionary<string, string> userProperties)
        {
            Guardia = guardia;
            this.center = center;
            Index = index;
            Bsphere = new TgcBoundingSphere(center, 15);

            if (GameUtils.containsUserProp(userProperties, "vigilar"))
            {
                Vigilar = bool.Parse(userProperties["vigilar"]);
            }
            else
            {
                Vigilar = false;
            }
        }

        public Guardia Guardia { get; }

        public TGCVector3 Center => center;

        public int Index { get; }

        public bool Vigilar { get; }

        /// <summary>
        ///     BSphere debug
        /// </summary>
        public TgcBoundingSphere Bsphere { get; set; }

        public void setCenterY(float y)
        {
            center.Y = y;
        }

        public void dispose()
        {
            Bsphere.Dispose();
        }

        public class WaypointComparer : IComparer<GuardiaWaypoint>
        {
            public int Compare(GuardiaWaypoint x, GuardiaWaypoint y)
            {
                return x.Index.CompareTo(y.Index);
            }
        }
    }
}
