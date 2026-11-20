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
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Interpolators;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Escondites
{
    /// <summary>
    ///     Objetos 3D para agarrar y esconderse
    /// </summary>
    public class EsconditeMovil : IAlphaObject
    {
        /// <summary>
        ///     Color para resaltar
        /// </summary>
        private static readonly int COLOR_RESALTADO = Color.FromArgb(0, 0, 0, 255).ToArgb();

        private static readonly int COLOR_RESALTADO_HIDDEN = Color.FromArgb(0, 255, 0, 0).ToArgb();

        private readonly InterpoladorVaiven interpolador;

        private readonly TGCVector3 originalPos;

        public EsconditeMovil(TgcMeshData meshDataNode, string texturePath, TgcSceneData originalSceneData)
        {
            //Cargar mesh
            Mesh = GameUtils.loadMeshShaderFromSceneData(meshDataNode, texturePath, originalSceneData);

            Agarrado = false;
            Escondido = false;
            interpolador = new InterpoladorVaiven(0, 0.7f, 0.65f);
            originalPos = Mesh.Position;
        }

        public TgcBoundingAxisAlignBox BoundingBox => Mesh.BoundingBox;

        /// <summary>
        ///     Indica si el escondite esta siendo cargado por el personaje
        /// </summary>
        public bool Agarrado { get; set; }

        /// <summary>
        ///     Indica si el personaje principal esta escondido ahora en este escondite
        /// </summary>
        public bool Escondido { get; set; }

        /// <summary>
        ///     Mesh del escondite
        /// </summary>
        public TgcMeshShader Mesh { get; private set; }

        public void renderWithAlpha()
        {
            if ( /*agarrado*/ Escondido)
            {
                var shaders = GameModel.instance.shaders;
                Mesh.Technique = shaders.RESALTADO;
                shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO_HIDDEN);
                shaders.effect.SetValue("scaleFactor", interpolador.update(GameModel.instance.ElapsedTime));
                Mesh.executeRender();
            }
            else
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(Mesh.BoundingBox.PMin))
                {
                    var shaders = GameModel.instance.shaders;
                    Mesh.Technique = shaders.RESALTADO;
                    shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO);
                    shaders.effect.SetValue("scaleFactor", interpolador.update(GameModel.instance.ElapsedTime));
                    Mesh.executeRender();
                }
            }
        }

        public void render()
        {
            if (Mesh.AlphaBlendEnable)
            {
                GameModel.instance.currentLevel.AlphaObjects.Add(this);
            }
            else
            {
                renderWithAlpha();
            }
        }


        /// <summary>
        ///     Volver a su posicion inicial
        /// </summary>
        public void restorePosition()
        {
            Mesh.Position = originalPos;
        }

        public void dispose()
        {
            Mesh.Dispose();
            Mesh = null;
        }
    }
}
