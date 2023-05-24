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
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TraficoFauna.Examples;
using TgcViewer.Utils.TgcGeometry;
using Examples.TraficoFauna.Meshes;

namespace Examples.TraficoFauna.Enemies
{
    /// <summary>
    /// Utilidad para manejar a los Guardias
    /// </summary>
    public class GuardiasManager
    {
        LevelSector sector;
        public LevelSector Sector
        {
            get { return sector; }
        }

        List<Guardia> guardias;
        public List<Guardia> Guardias
        {
            get { return guardias; }
        }


        public GuardiasManager(LevelSector sector)
        {
            this.sector = sector;
            this.guardias = new List<Guardia>();
        }

        public void addNode(TgcMeshData meshDataNode)
        {
            Vector3 nodeCenter = GameUtils.getBBCenterFromMeshData(meshDataNode);

            //Nombre: Guardia_XXX o Guardia_XXX_1 o Guardia_XXX_2
            string[] split = meshDataNode.name.Split('_');
            string guardiaId = split[0] + "_" + split[1];
            if (split.Length == 2)
            {
                
                Guardia guardia = existGuardia(guardiaId);
                if (guardia == null)
                {
                    //Crear guardia real
                    guardia = new Guardia(guardiaId);
                    guardia.setData(meshDataNode.userProperties);
                    this.guardias.Add(guardia);
                }
                //Actualizar existente
                else
                {
                    guardia.setData(meshDataNode.userProperties);
                }
                //Agregar waypoint inicial
                GuardiaWaypoint waypoint = new GuardiaWaypoint(guardia, nodeCenter, 0, meshDataNode.userProperties);
                guardia.Waypoints.Add(waypoint);
            }
            else if (split.Length == 3)
            {
                int nodeIndex = int.Parse(split[2]);
                Guardia guardia = existGuardia(guardiaId);

                //Crear guardia vacio
                if (guardia == null)
                {
                    guardia = new Guardia(guardiaId);
                    this.guardias.Add(guardia);
                }
                
                //Agregar waypoint
                GuardiaWaypoint waypoint = new GuardiaWaypoint(guardia, nodeCenter, nodeIndex, meshDataNode.userProperties);
                guardia.Waypoints.Add(waypoint);
            }
            else
            {
                throw new Exception("Nombre de Guarida: " + meshDataNode.name + ", incorrecto");
            }
        }


        public void arrangeNodes()
        {
            foreach (Guardia guardia in guardias)
            {
                guardia.arrangeWaypoints();
            }
        }

        public Guardia findGuardia(string guardiaId)
        {
            foreach (Guardia g in guardias)
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
            foreach (Guardia g in guardias)
            {
                if (g.Id == guardiaId)
                {
                    return g;
                }
            }
            return null;
        }

        /// <summary>
        /// Iniciar a los guardias en un sector
        /// </summary>
        public void initGuardias()
        {
            GuardiasPool pool = GameModel.instance.guardiasPool;
            foreach (Guardia g in guardias)
            {
                if (g.Activo)
                {
                    g.init(pool.getAvaliableMesh());
                }
            }
        }

        /// <summary>
        /// Esconder guardias del sector actual
        /// </summary>
        public void hideGuardias()
        {
            GameModel.instance.guardiasPool.freeAllMeshes();
            foreach (Guardia g in guardias)
            {
                g.hide();
            }
        }

        /// <summary>
        /// Actualizar estados de guardias
        /// </summary>
        public void update()
        {
            if (GameModel.instance.debugMode.Enabled && !GameModel.instance.debugMode.GuardiasEnabled)
                return;

            foreach (Guardia g in guardias)
            {
                if (g.Activo)
                {
                    g.update();
                }
            }
        }

        public void render()
        {
            foreach (Guardia g in guardias)
            {
                if (g.Activo)
                {
                    g.render();
                }
            }
        }


        /// <summary>
        /// Alertar a todos los guardias
        /// </summary>
        public void alertAll()
        {
            foreach (Guardia g in guardias)
            {
                if (g.Activo)
                {
                    g.setChasing();
                }
            }
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            GameModel.instance.guardiasPool.freeAllMeshes();
            foreach (Guardia g in guardias)
            {
                g.dispose();
            }
            guardias = null;

            sector = null;
        }
    }
}
