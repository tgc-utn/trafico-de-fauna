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
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace Trafico.De.Fauna.Model.Enemies.Ataque
{
    /// <summary>
    ///     Utilidad para manejar todos los Waypoints que se usan para cuando los Guardias atacan
    /// </summary>
    public class WaypointsAtaqueManager
    {
        private List<WaypointAtaqueArista> aristas;


        public WaypointsAtaqueManager()
        {
            Waypoints = new List<WaypointAtaque>();
            aristas = new List<WaypointAtaqueArista>();
        }

        /// <summary>
        ///     Waypoints
        /// </summary>
        public List<WaypointAtaque> Waypoints { get; private set; }

        /// <summary>
        ///     Cargar un nodo de Waypoint de ataque
        /// </summary>
        public void addNode(TgcMeshData meshDataNode)
        {
            //Nombre: Waypont_1 o Waypoint_2
            var split = meshDataNode.name.Split('_');
            if (split.Length != 2)
            {
                throw new Exception("Objeto en layer WaypointsAtaque con nombre icorrecto: " + meshDataNode.name);
            }

            var index = int.Parse(split[1]);
            var nodeCenter = GameUtils.getBBCenterFromMeshData(meshDataNode);
            nodeCenter.Y = 0;

            //Crear waypoint
            var waypoint = new WaypointAtaque(meshDataNode.name, nodeCenter, index);
            Waypoints.Add(waypoint);
        }

        /// <summary>
        ///     Organizar waypoints cargados
        /// </summary>
        public void arrangeNodes()
        {
            if (Waypoints.Count == 1)
            {
                throw new Exception("Hay solo un WaypointAtaque, tiene que haber dos o mas o ninguno.");
            }

            //Ordenar waypoints por indice ascendente
            Waypoints.Sort(new WaypointAtaque.WaypointAtaqueComparer());

            //Reacomodar indices para que queden igual que en el array
            for (var i = 0; i < Waypoints.Count; i++)
            {
                Waypoints[i].Index = i;
            }

            //Crear aristas
            for (var i = 0; i < Waypoints.Count - 1; i++)
            {
                var arista = new WaypointAtaqueArista(Waypoints[i], Waypoints[i + 1]);
                aristas.Add(arista);
            }
        }

        /// <summary>
        ///     Devuelve el waypoint que le sigue en la lista al waypoint especificado
        /// </summary>
        public WaypointAtaque getNextWaypoint(WaypointAtaque w)
        {
            var n = (w.Index + 1) % Waypoints.Count;
            return Waypoints[n];
        }

        /// <summary>
        ///     Devuelve el waypoint anterior en la lista al waypoint especificado
        /// </summary>
        public WaypointAtaque getPreviousWaypoint(WaypointAtaque w)
        {
            var n = w.Index - 1;
            if (n < 0)
            {
                n = Waypoints.Count - 1;
            }

            return Waypoints[n];
        }

        /// <summary>
        ///     Obtener la arista mas cercana al punto indicado.
        ///     Devuelve tambien el punto en esa arista que esta mas cerca.
        /// </summary>
        public WaypointAtaqueArista getAristaMasCercana(TGCVector3 pos, out TGCVector3 aristaPos)
        {
            float t;
            var minQ = TGCVector3.Empty;
            var minDistSq = float.MaxValue;
            WaypointAtaqueArista minArista = null;
            foreach (var arista in aristas)
            {
                var q = TgcCollisionUtils.closestPointSegment(pos, arista.W1.Position, arista.W2.Position, out t);
                var lengthSq = TGCVector3.Subtract(q, pos).LengthSq();
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
            foreach (var w in Waypoints)
            {
                w.dispose();
            }

            Waypoints = null;
            foreach (var a in aristas)
            {
                a.dispose();
            }

            aristas = null;
        }
    }
}
