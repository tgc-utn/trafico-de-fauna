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
using Microsoft.DirectX;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TgcViewer.Model.Dialogs;
using TgcViewer.Model.Meshes;

namespace TgcViewer.Model.Levels
{
    /// <summary>
    /// Conecta un sector con otro.
    /// </summary>
    public class Conector : IAlphaObject
    {
        
        string texto;

        bool activo;
        public bool Activo
        {
            get { return activo; }
        }

        Vector3 center;
        public Vector3 Center
        {
            get { return center; }
        }

        string id;
        public string Id
        {
            get { return id; }
        }
        string siguienteId;
        public string SiguienteId
        {
            get { return siguienteId; }
        }

        LevelSector sector;
        public LevelSector Sector
        {
            get { return sector; }
        }

        TgcBoundingAxisAlignBox boundingBox;
        public TgcBoundingAxisAlignBox BoundingBox
        {
            get { return boundingBox; }
        }

        GameUtils.Orientacion orientacion;
        /// <summary>
        /// Hacia donde orientar inicialmente al personaje al pasar por este sector
        /// </summary>
        public GameUtils.Orientacion Orientacion
        {
            get { return orientacion; }
        }

        bool changeLevel;
        /// <summary>
        /// Indica si este conector produce un cambio de nivel
        /// </summary>
        public bool ChangeLevel
        {
            get { return changeLevel; }
        }

        public Conector(LevelSector sector, TgcMeshData meshData)
        {
            this.sector = sector;
            id = meshData.name;

            //Propiedad: siguienteId=Conector_XXX (si no viene el conector no pasa de sector)
            if (GameUtils.containsUserProp(meshData.userProperties, "siguienteId"))
            {
                siguienteId = meshData.userProperties["siguienteId"];
            }
            else
            {
                siguienteId = null;
            }

            //Propiedad: texto=Hola
            if (GameUtils.containsUserProp(meshData.userProperties, "texto"))
            {
                texto = (string)meshData.userProperties["texto"];
            }
            else
            {
                texto = "Siguiente: " + siguienteId;
            }

            //Propiedad: activo=true (default=true)
            if (GameUtils.containsUserProp(meshData.userProperties, "activo"))
            {
                activo = bool.Parse(meshData.userProperties["activo"]);
            }
            else
            {
                activo = true;
            }
            //Propiedad: orientacion=norte (default norte) => norte = +Y, sur = -Y, este = X, oeste = -X
            if (GameUtils.containsUserProp(meshData.userProperties, "orientacion"))
            {
                string orientacionStr = meshData.userProperties["orientacion"].ToUpper();
                switch (orientacionStr)
                {
                    case "NORTE":
                        orientacion = GameUtils.Orientacion.Norte;
                        break;
                    case "SUR":
                        orientacion = GameUtils.Orientacion.Sur;
                        break;
                    case "ESTE":
                        orientacion = GameUtils.Orientacion.Este;
                        break;
                    case "OESTE":
                        orientacion = GameUtils.Orientacion.Oeste;
                        break;
                    default:
                        throw new Exception("Orientacion incorrecta: " + orientacionStr + ", en Conector: " + id);
                }
            }
            else
            {
                orientacion = GameUtils.Orientacion.Norte;
            }

            //Propiedad: changeLevel=true (default false)
            if (GameUtils.containsUserProp(meshData.userProperties, "changeLevel"))
            {
                changeLevel = bool.Parse(meshData.userProperties["changeLevel"]);
            }
            else
            {
                changeLevel = false;
            }


            //Si no se especifico "siguienteId" y ademas no hay cambio de nivel, se desactiva el conector
            if (siguienteId == null && !changeLevel)
            {
                activo = false;
            }


            boundingBox = GameUtils.createBBFromMeshData(meshData);
            center = boundingBox.calculateBoxCenter();
            center.Y = 0;
        }


        public void dispose()
        {
            sector = null;
            if (boundingBox != null)
            {
                boundingBox.dispose();
                boundingBox = null;
            }
        }

        /// <summary>
        /// Dibujar mensaje indicador de conector
        /// </summary>
        public void render()
        {
            GameModel.instance.currentLevel.AlphaObjects.Add(this);
        }

        public void renderWithAlpha()
        {
            SimpleMessageDialog simpleMessageDialog = GameModel.instance.simpleMessageDialog;
            simpleMessageDialog.setText(this.texto);
            simpleMessageDialog.render();
        }

    }
}
