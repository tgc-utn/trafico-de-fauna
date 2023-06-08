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
using Microsoft.DirectX;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TgcViewer.Model.Interpolators;
using TgcViewer.Model.Meshes;

namespace TgcViewer.Model.Escondites
{
    /// <summary>
    /// Objetos 3D para agarrar y esconderse
    /// </summary>
    public class EsconditeMovil : IAlphaObject
    {

        /// <summary>
        /// Color para resaltar
        /// </summary>
        static readonly int COLOR_RESALTADO = Color.FromArgb(0, 0, 0, 255).ToArgb();
        static readonly int COLOR_RESALTADO_HIDDEN = Color.FromArgb(0, 255, 0, 0).ToArgb();

        public TgcBoundingAxisAlignBox BoundingBox
        {
            get { return mesh.BoundingBox; }
        }

        bool agarrado;
        /// <summary>
        /// Indica si el escondite esta siendo cargado por el personaje
        /// </summary>
        public bool Agarrado
        {
            get { return agarrado; }
            set { agarrado = value; }
        }

        bool escondido;
        /// <summary>
        /// Indica si el personaje principal esta escondido ahora en este escondite
        /// </summary>
        public bool Escondido
        {
            get { return escondido; }
            set { escondido = value; }
        }

        Vector3 originalPos;
        InterpoladorVaiven interpolador;

        TgcMeshShader mesh;
        /// <summary>
        /// Mesh del escondite
        /// </summary>
        public TgcMeshShader Mesh
        {
            get { return mesh; }
        }

        public EsconditeMovil(TgcMeshData meshDataNode, string texturePath, TgcSceneData originalSceneData)
        {

            //Cargar mesh
            mesh = GameUtils.loadMeshShaderFromSceneData(meshDataNode, texturePath, originalSceneData);

            agarrado = false;
            escondido = false;
            interpolador = new InterpoladorVaiven(0, 0.7f, 0.65f);
            this.originalPos = mesh.Position;
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

        public void renderWithAlpha()
        {
            if (/*agarrado*/ escondido)
            {
                Shaders shaders = GameModel.instance.shaders;
                mesh.Technique = shaders.RESALTADO;
                shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO_HIDDEN);
                shaders.effect.SetValue("scaleFactor", interpolador.update(GameModel.instance.ElapsedTime));
                mesh.executeRender();
            }
            else
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.BoundingBox.PMin))
                {
                    Shaders shaders = GameModel.instance.shaders;
                    mesh.Technique = shaders.RESALTADO;
                    shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO);
                    shaders.effect.SetValue("scaleFactor", interpolador.update(GameModel.instance.ElapsedTime));
                    mesh.executeRender();
                }
            }
        }


        /// <summary>
        /// Volver a su posicion inicial
        /// </summary>
        public void restorePosition()
        {
            mesh.Position = originalPos;
        }

        public void dispose()
        {
            mesh.dispose();
            mesh = null;
        }
    }


}
