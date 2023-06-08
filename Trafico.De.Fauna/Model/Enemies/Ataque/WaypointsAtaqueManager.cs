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

using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;

namespace TgcViewer.Model.Enemies.Ataque
{
    /// <summary>
    /// Utilidad para manejar todos los Waypoints que se usan para cuando los Guardias atacan
    /// </summary>
    public class WaypointsAtaqueManager
    {

        List<WaypointAtaque> waypoints;
        /// <summary>
        /// Waypoints
        /// </summary>
        public List<WaypointAtaque> Waypoints
        {
            get { return waypoints; }
        }


        List<WaypointAtaqueArista> aristas;


        public WaypointsAtaqueManager()
        {
            waypoints = new List<WaypointAtaque>();
            aristas = new List<WaypointAtaqueArista>();
        }

        /// <summary>
        /// Cargar un nodo de Waypoint de ataque
        /// </summary>
        public void addNode(TgcMeshData meshDataNode)
        {
            //Nombre: Waypont_1 o Waypoint_2
            string[] split = meshDataNode.name.Split('_');
            if (split.Length != 2)
            {
                throw new Exception("Objeto en layer WaypointsAtaque con nombre icorrecto: " + meshDataNode.name);
            }

            int index = int.Parse(split[1]);
            Vector3 nodeCenter = GameUtils.getBBCenterFromMeshData(meshDataNode);
            nodeCenter.Y = 0;

            //Crear waypoint
            WaypointAtaque waypoint = new WaypointAtaque(meshDataNode.name, nodeCenter, index);
            this.waypoints.Add(waypoint);
        }

        /// <summary>
        /// Organizar waypoints cargados
        /// </summary>
        public void arrangeNodes()
        {
            if (waypoints.Count == 1)
            {
                throw new Exception("Hay solo un WaypointAtaque, tiene que haber dos o mas o ninguno.");
            }

            //Ordenar waypoints por indice ascendente
            waypoints.Sort(new WaypointAtaque.WaypointAtaqueComparer());

            //Reacomodar indices para que queden igual que en el array
            for (int i = 0; i < waypoints.Count; i++)
            {
                waypoints[i].Index = i;
            }

            //Crear aristas
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                WaypointAtaqueArista arista = new WaypointAtaqueArista(waypoints[i], waypoints[i + 1]);
                aristas.Add(arista);
            }
        }

        /// <summary>
        /// Devuelve el waypoint que le sigue en la lista al waypoint especificado
        /// </summary>
        public WaypointAtaque getNextWaypoint(WaypointAtaque w)
        {
            int n = (w.Index + 1) % waypoints.Count;
            return waypoints[n];
        }

        /// <summary>
        /// Devuelve el waypoint anterior en la lista al waypoint especificado
        /// </summary>
        public WaypointAtaque getPreviousWaypoint(WaypointAtaque w)
        {
            int n = w.Index - 1;
            if (n < 0)
            {
                n = waypoints.Count - 1;
            }
            return waypoints[n];
        }

        /// <summary>
        /// Obtener la arista mas cercana al punto indicado.
        /// Devuelve tambien el punto en esa arista que esta mas cerca.
        /// </summary>
        public WaypointAtaqueArista getAristaMasCercana(Vector3 pos, out Vector3 aristaPos)
        {
            float t;
            Vector3 minQ = Vector3.Empty;
            float minDistSq = float.MaxValue;
            WaypointAtaqueArista minArista = null;
            foreach (WaypointAtaqueArista arista in aristas)
            {
                Vector3 q = TgcCollisionUtils.closestPointSegment(pos, arista.W1.Position, arista.W2.Position, out t);
                float lengthSq = Vector3.Subtract(q, pos).LengthSq();
                if (lengthSq < minDistSq)
                {
                    minDistSq = lengthSq;
                    minQ = q;
                    minArista = arista;
                }
            }

            aristaPos = minQ;
            return minArista;
        }
         


        public void dispose()
        {
            foreach (WaypointAtaque w in waypoints)
            {
                w.dispose();
            }
            waypoints = null;
            foreach (WaypointAtaqueArista a in aristas)
            {
                a.dispose();
            }
            aristas = null;
        }
    }
}
