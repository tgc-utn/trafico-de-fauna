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
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Levels;

namespace Trafico.De.Fauna.Model.Enemies
{
    /// <summary>
    ///     Utilidad para manejar a los Guardias
    /// </summary>
    public class GuardiasManager
    {
        public GuardiasManager(LevelSector sector)
        {
            this.Sector = sector;
            Guardias = new List<Guardia>();
        }

        public LevelSector Sector { get; private set; }

        public List<Guardia> Guardias { get; private set; }

        public void addNode(TgcMeshData meshDataNode)
        {
            var nodeCenter = GameUtils.getBBCenterFromMeshData(meshDataNode);

            //Nombre: Guardia_XXX o Guardia_XXX_1 o Guardia_XXX_2
            var split = meshDataNode.name.Split('_');
            var guardiaId = split[0] + "_" + split[1];
            if (split.Length == 2)
            {
                var guardia = existGuardia(guardiaId);
                if (guardia == null)
                {
                    //Crear guardia real
                    guardia = new Guardia(guardiaId);
                    guardia.setData(meshDataNode.userProperties);
                    Guardias.Add(guardia);
                }
                //Actualizar existente
                else
                {
                    guardia.setData(meshDataNode.userProperties);
                }

                //Agregar waypoint inicial
                var waypoint = new GuardiaWaypoint(guardia, nodeCenter, 0, meshDataNode.userProperties);
                guardia.Waypoints.Add(waypoint);
            }
            else if (split.Length == 3)
            {
                var nodeIndex = int.Parse(split[2]);
                var guardia = existGuardia(guardiaId);

                //Crear guardia vacio
                if (guardia == null)
                {
                    guardia = new Guardia(guardiaId);
                    Guardias.Add(guardia);
                }

                //Agregar waypoint
                var waypoint = new GuardiaWaypoint(guardia, nodeCenter, nodeIndex, meshDataNode.userProperties);
                guardia.Waypoints.Add(waypoint);
            }
            else
            {
                throw new Exception("Nombre de Guarida: " + meshDataNode.name + ", incorrecto");
            }
        }


        public void arrangeNodes()
        {
            foreach (var guardia in Guardias)
            {
                guardia.arrangeWaypoints();
            }
        }

        public Guardia findGuardia(string guardiaId)
        {
            foreach (var g in Guardias)
            {
                if (g.Id == guardiaId)
                {
                    return g;
                }
            }

            throw new Exception("Guardia: " + guardiaId + " no encontrado");
        }

        public Guardia existGuardia(string guardiaId)
        {
            foreach (var g in Guardias)
            {
                if (g.Id == guardiaId)
                {
                    return g;
                }
            }

            return null;
        }

        /// <summary>
        ///     Iniciar a los guardias en un sector
        /// </summary>
        public void initGuardias()
        {
            var pool = GameModel.instance.guardiasPool;
            foreach (var g in Guardias)
            {
                if (g.Activo)
                {
                    g.init(pool.getAvaliableMesh());
                }
            }
        }

        /// <summary>
        ///     Esconder guardias del sector actual
        /// </summary>
        public void hideGuardias()
        {
            GameModel.instance.guardiasPool.freeAllMeshes();
            foreach (var g in Guardias)
            {
                g.hide();
            }
        }

        /// <summary>
        ///     Actualizar estados de guardias
        /// </summary>
        public void update(float elapsedTime)
        {
            if (GameModel.instance.debugMode.Enabled && !GameModel.instance.debugMode.GuardiasEnabled)
            {
                return;
            }

            foreach (var g in Guardias)
            {
                if (g.Activo)
                {
                    g.update(elapsedTime);
                }
            }
        }

        public void render(float elapsedTime)
        {
            foreach (var g in Guardias)
            {
                if (g.Activo)
                {
                    g.render(elapsedTime);
                }
            }
        }


        /// <summary>
        ///     Alertar a todos los guardias
        /// </summary>
        public void alertAll()
        {
            foreach (var g in Guardias)
            {
                if (g.Activo)
                {
                    g.setChasing();
                }
            }
        }

        /// <summary>
        ///     Liberar recursos
        /// </summary>
        public void dispose()
        {
            GameModel.instance.guardiasPool.freeAllMeshes();
            foreach (var g in Guardias)
            {
                g.dispose();
            }

            Guardias = null;

            Sector = null;
        }
    }
}
