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
using Examples.TraficoFauna.Meshes;
using TgcViewer.Utils.TgcGeometry;
using TraficoFauna.Examples;
using System.Drawing;
using TgcViewer;
using Examples.TraficoFauna.Interpolators;

namespace Examples.TraficoFauna.Escondites
{
    /// <summary>
    /// Modelo en el que el personaje se puede esconder
    /// </summary>
    public class EsconditeFijo : IAlphaObject
    {
        /// <summary>
        /// Radio del bouding sphere de colision
        /// </summary>
        const float BSPHERE_RADIUS = 5;

        /// <summary>
        /// Color para resaltar
        /// </summary>
        static readonly int COLOR_RESALTADO = Color.FromArgb(0, 0, 0, 255).ToArgb();
        static readonly int COLOR_RESALTADO_HIDDEN = Color.FromArgb(0, 255, 0, 0).ToArgb();

        TgcBoundingSphere bSphere;
        public TgcBoundingSphere BSphere
        {
            get { return bSphere; }
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

        TgcMeshShader mesh;
        InterpoladorVaiven interpolador;

        public EsconditeFijo(TgcMeshData meshDataNode, string texturePath, TgcSceneData originalSceneData)
        {
            //Cargar mesh
            mesh = GameUtils.loadMeshShaderFromSceneData(meshDataNode, texturePath, originalSceneData);
            bSphere = new TgcBoundingSphere(mesh.BoundingBox.calculateBoxCenter(), BSPHERE_RADIUS);

            escondido = false;
            interpolador = new InterpoladorVaiven(0, 0.7f, 0.65f);
        }

        public void render()
        {
            if (mesh.AlphaBlendEnable)
            {
                Game.instance.currentLevel.AlphaObjects.Add(this);
            }
            else
            {
                renderWithAlpha();
            }
        }

        public void renderWithAlpha()
        {
            if (escondido)
            {
                Shaders shaders = Game.instance.shaders;
                mesh.Technique = shaders.RESALTADO;
                shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO_HIDDEN);
                shaders.effect.SetValue("scaleFactor", interpolador.update());
                mesh.executeRender();

            }
            else
            {
                //Dibujar si esta en rango visible
                if (Game.instance.mainCharacter.isInProximityRange(bSphere.Center))
                {
                    Shaders shaders = Game.instance.shaders;
                    mesh.Technique = shaders.RESALTADO;
                    shaders.effect.SetValue("color_resaltado", COLOR_RESALTADO);
                    shaders.effect.SetValue("scaleFactor", interpolador.update());
                    mesh.executeRender();
                }
            }
        }



        public void dispose()
        {
            mesh.dispose();
            mesh = null;
            bSphere.dispose();
            bSphere = null;
        }
    }
}
