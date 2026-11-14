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
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Levels
{
    /// <summary>
    ///     Conecta un sector con otro.
    /// </summary>
    public class Conector : IAlphaObject
    {
        private readonly TGCVector3 center;

        private readonly string texto;

        public Conector(LevelSector sector, TgcMeshData meshData)
        {
            this.Sector = sector;
            Id = meshData.name;

            //Propiedad: siguienteId=Conector_XXX (si no viene el conector no pasa de sector)
            if (GameUtils.containsUserProp(meshData.userProperties, "siguienteId"))
            {
                SiguienteId = meshData.userProperties["siguienteId"];
            }
            else
            {
                SiguienteId = null;
            }

            //Propiedad: texto=Hola
            if (GameUtils.containsUserProp(meshData.userProperties, "texto"))
            {
                texto = meshData.userProperties["texto"];
            }
            else
            {
                texto = "Siguiente: " + SiguienteId;
            }

            //Propiedad: activo=true (default=true)
            if (GameUtils.containsUserProp(meshData.userProperties, "activo"))
            {
                Activo = bool.Parse(meshData.userProperties["activo"]);
            }
            else
            {
                Activo = true;
            }

            //Propiedad: orientacion=norte (default norte) => norte = +Y, sur = -Y, este = X, oeste = -X
            if (GameUtils.containsUserProp(meshData.userProperties, "orientacion"))
            {
                var orientacionStr = meshData.userProperties["orientacion"].ToUpper();
                switch (orientacionStr)
                {
                    case "NORTE":
                        Orientacion = GameUtils.Orientacion.Norte;
                        break;
                    case "SUR":
                        Orientacion = GameUtils.Orientacion.Sur;
                        break;
                    case "ESTE":
                        Orientacion = GameUtils.Orientacion.Este;
                        break;
                    case "OESTE":
                        Orientacion = GameUtils.Orientacion.Oeste;
                        break;
                    default:
                        throw new Exception("Orientacion incorrecta: " + orientacionStr + ", en Conector: " + Id);
                }
            }
            else
            {
                Orientacion = GameUtils.Orientacion.Norte;
            }

            //Propiedad: changeLevel=true (default false)
            if (GameUtils.containsUserProp(meshData.userProperties, "changeLevel"))
            {
                ChangeLevel = bool.Parse(meshData.userProperties["changeLevel"]);
            }
            else
            {
                ChangeLevel = false;
            }


            //Si no se especifico "siguienteId" y ademas no hay cambio de nivel, se desactiva el conector
            if (SiguienteId == null && !ChangeLevel)
            {
                Activo = false;
            }


            BoundingBox = GameUtils.createBBFromMeshData(meshData);
            center = BoundingBox.calculateBoxCenter();
            center.Y = 0;
        }

        public bool Activo { get; }

        public TGCVector3 Center => center;

        public string Id { get; }

        public string SiguienteId { get; }

        public LevelSector Sector { get; private set; }

        public TgcBoundingAxisAlignBox BoundingBox { get; private set; }

        /// <summary>
        ///     Hacia donde orientar inicialmente al personaje al pasar por este sector
        /// </summary>
        public GameUtils.Orientacion Orientacion { get; }

        /// <summary>
        ///     Indica si este conector produce un cambio de nivel
        /// </summary>
        public bool ChangeLevel { get; }

        public void renderWithAlpha()
        {
            var simpleMessageDialog = GameModel.instance.simpleMessageDialog;
            simpleMessageDialog.setText(texto);
            simpleMessageDialog.render();
        }


        public void dispose()
        {
            Sector = null;
            if (BoundingBox != null)
            {
                BoundingBox.Dispose();
                BoundingBox = null;
            }
        }

        /// <summary>
        ///     Dibujar mensaje indicador de conector
        /// </summary>
        public void render()
        {
            GameModel.instance.currentLevel.AlphaObjects.Add(this);
        }
    }
}
