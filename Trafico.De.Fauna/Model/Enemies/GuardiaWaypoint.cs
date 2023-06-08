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
using Microsoft.DirectX;
using TGC.Core.BoundingVolumes;

namespace TgcViewer.Model.Enemies
{
    public class GuardiaWaypoint
    {
        Guardia guardia;
        public Guardia Guardia
        {
            get { return guardia; }
        }

        Vector3 center;
        public Vector3 Center
        {
            get { return center; }
        }

        int index;
        public int Index
        {
            get { return index; }
        }

        bool vigilar;
        public bool Vigilar
        {
            get { return vigilar; }
        }

        TgcBoundingSphere bsphere;
        /// <summary>
        /// BSphere debug
        /// </summary>
        public TgcBoundingSphere Bsphere
        {
            get { return bsphere; }
            set { bsphere = value; }
        }

        public GuardiaWaypoint(Guardia guardia, Vector3 center, int index, Dictionary<string, string> userProperties)
        {
            this.guardia = guardia;
            this.center = center;
            this.index = index;
            bsphere = new TgcBoundingSphere(center, 15);

            if (GameUtils.containsUserProp(userProperties, "vigilar"))
            {
                vigilar = bool.Parse(userProperties["vigilar"]);
            }
            else
            {
                vigilar = false;
            }
        }

        public void setCenterY(float y)
        {
            this.center.Y = y;
        }

        public class WaypointComparer : IComparer<GuardiaWaypoint>
        {
            public int Compare(GuardiaWaypoint x, GuardiaWaypoint y)
            {
                return x.index.CompareTo(y.index);
            }
        }

        public void dispose()
        {
            bsphere.dispose();
        }

    }
}
