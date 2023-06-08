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
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;
using TgcViewer.Utils.Render;

namespace TgcViewer.Model.Meshes
{
    /// <summary>
    /// Mesh que permite que se animen sus coordenadas UV en base a una velocidad especificada.
    /// Solo permite meshes con formato de Textura y de Textura con Lightmaps (los lightmaps se ignoran).
    /// No puede haber meshes sin coordenadas uv.
    /// </summary>
    public class MeshAnimatedTexture : TgcMeshShader, IRenderQueueElement, IAlphaObject
    {

        Vector2 currentUVsum;

        private bool playing;
        /// <summary>
        /// Arrancar o parar avance de animacion de la textura
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        private Vector2 speed;
        /// <summary>
        /// Velocidad de desplazamiento de las coordenadas UV
        /// </summary>
        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }


        public MeshAnimatedTexture(Mesh mesh, string name, MeshRenderType renderType)
            : base(mesh, name, renderType)
        {
            if (renderType == MeshRenderType.VERTEX_COLOR)
            {
                throw new Exception("El mesh no tiene texturas.");
            }

            this.speed = new Vector2(0, 1);
            currentUVsum = new Vector2(0, 0);
        }

        public MeshAnimatedTexture(string name, TgcMesh parentInstance, Vector3 translation, Vector3 rotation, Vector3 scale)
            : base(name, parentInstance, translation, rotation, scale)
        {
            throw new Exception("Instancia no soportada");
        }


        /// <summary>
        /// Se redefine este método.
        /// Si tiene Alpha habilitado no renderiza directo, sino que lo manda a una lista para renderizar
        /// luego de todos los objetos opacos.
        /// </summary>
        public new void render()
        {
            if (!enabled)
                return;

            if (alphaBlendEnable)
            {
                GameModel.instance.currentLevel.AlphaObjects.Add(this);
            }
            else
            {
                executeRender();
            }
        }

        /// <summary>
        /// Render directo
        /// </summary>
        public new void renderWithAlpha()
        {
            executeRender();
        }


        /// <summary>
        /// Se redefine este método para agregar seteo de shader y animacion de uv
        /// </summary>
        public new void executeRender(float elapsedTime)
        {
            Effect effect = GameModel.instance.shaders.effect;
            this.Technique = GameModel.instance.shaders.ANIMATE_UV;


            //Actualizar incremento de UV
            if (playing)
            {
                Vector2 addVec = Vector2.Scale(speed, elapsedTime);
                currentUVsum += addVec;
                currentUVsum.X %= 1;
                currentUVsum.Y %= 1;
            }

            effect.SetValue("animateUV", TgcParserUtils.vector2ToFloat2Array(currentUVsum));

            //Ejecutar render de TgcMeshShader
            base.executeRender();
        }


        /// <summary>
        /// Factory customizado 
        /// </summary>
        public class MeshAnimatedTextureFactory : TgcSceneLoader.IMeshFactory
        {
            public TgcMesh createNewMesh(Mesh d3dMesh, string meshName, TgcMesh.MeshRenderType renderType)
            {
                return new MeshAnimatedTexture(d3dMesh, meshName, renderType);
            }

            public TgcMesh createNewMeshInstance(string meshName, TgcMesh originalMesh, Vector3 translation, Vector3 rotation, Vector3 scale)
            {
                return new MeshAnimatedTexture(meshName, originalMesh, translation, rotation, scale);
            }
        }

    }
}
