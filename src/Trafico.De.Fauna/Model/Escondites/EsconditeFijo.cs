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

using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Interpolators;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Escondites
{
    /// <summary>
    ///     Modelo en el que el personaje se puede esconder
    /// </summary>
    public class EsconditeFijo : IAlphaObject
    {
        /// <summary>
        ///     Radio del bouding sphere de colision
        /// </summary>
        private const float BSPHERE_RADIUS = 5;

        /// <summary>
        ///     Color para resaltar
        /// </summary>
        private static readonly int COLOR_RESALTADO = Color.FromArgb(0, 0, 0, 255).ToArgb();

        private static readonly int COLOR_RESALTADO_HIDDEN = Color.FromArgb(0, 255, 0, 0).ToArgb();

        private readonly InterpoladorVaiven interpolador;

        private TgcMeshShader mesh;

        public EsconditeFijo(TgcMeshData meshDataNode, string texturePath, TgcSceneData originalSceneData)
        {
            //Cargar mesh
            mesh = GameUtils.loadMeshShaderFromSceneData(meshDataNode, texturePath, originalSceneData);
            BSphere = new TgcBoundingSphere(mesh.BoundingBox.calculateBoxCenter(), BSPHERE_RADIUS);

            Escondido = false;
            interpolador = new InterpoladorVaiven(0, 0.7f, 0.65f);
        }

        public TgcBoundingSphere BSphere { get; private set; }

        /// <summary>
        ///     Indica si el personaje principal esta escondido ahora en este escondite
        /// </summary>
        public bool Escondido { get; set; }

        public void renderWithAlpha()
        {
            if (Escondido)
            {
                var shaders = GameModel.instance.shaders;
                mesh.Technique = shaders.RESALTADO;
                shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO_HIDDEN);
                shaders.effect.SetValue("scaleFactor", interpolador.update(GameModel.instance.ElapsedTime));
                mesh.executeRender();
            }
            else
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(BSphere.Center))
                {
                    var shaders = GameModel.instance.shaders;
                    mesh.Technique = shaders.RESALTADO;
                    shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO);
                    shaders.effect.SetValue("scaleFactor", interpolador.update(GameModel.instance.ElapsedTime));
                    mesh.executeRender();
                }
            }
        }

        public void render()
        {
            if (mesh.AlphaBlendEnable)
            {
                GameModel.instance.currentLevel.AlphaObjects.Add(this);
            }
            else
            {
                renderWithAlpha();
            }
        }


        public void dispose()
        {
            mesh.Dispose();
            mesh = null;
            BSphere.Dispose();
            BSphere = null;
        }
    }
}
