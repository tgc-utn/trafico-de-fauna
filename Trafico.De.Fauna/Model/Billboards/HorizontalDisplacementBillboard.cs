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

using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace Trafico.De.Fauna.Model.Billboards
{
    /// <summary>
    /// Herramienta para hacer planos horizontales con textura y que
    /// posee una animacion de sus coordenadas UV.
    /// Las coordenadas UV varian segun un vector de velocidad especificado
    /// </summary>
    public class HorizontalDisplacementBillboard : IRenderObject
    {
        VertexBuffer vertexBuffer;
        TgcTexture texture;
        TGCVector2 currentUVsum;


        private bool enabled;
        /// <summary>
        /// Indica si la pared esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private bool playing;
        /// <summary>
        /// Arrancar o parar avance de animacion
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        TGCVector3 pMin;
        /// <summary>
        /// Punto minimo del Billboard
        /// </summary>
        public TGCVector3 PMin
        {
            get { return pMin; }
            set { pMin = value; }
        }

        TGCVector3 pMax;
        /// <summary>
        /// Punto maximo del Billboard.
        /// El valor de Y no se considera. Se toma siempre el de PMin.
        /// </summary>
        public TGCVector3 PMax
        {
            get { return pMax; }
            set { pMax = value; }
        }

        private bool alphaBlendEnable;
        /// <summary>
        /// Habilita el renderizado con AlphaBlending para los modelos
        /// con textura o colores por vértice de canal Alpha.
        /// Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable
        {
            get { return alphaBlendEnable; }
            set { alphaBlendEnable = value; }
        }

        private TGCVector2 speed;
        /// <summary>
        /// Velocidad de desplazamiento de las coordenadas UV
        /// </summary>
        public TGCVector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public HorizontalDisplacementBillboard(Device device, string texturePath, TGCVector3 pMin, TGCVector3 pMax)
        {
            this.enabled = true;
            this.alphaBlendEnable = true;
            this.speed = new TGCVector2(0, 1);
            this.playing = true;
            this.pMin = pMin;
            this.pMax = pMax;
            currentUVsum = new TGCVector2(0, 0);

            //Cargar textura
            this.texture = TgcTexture.createTexture(device, texturePath);

            //Crear vertexBuffer
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionTextured), 6, device,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[6];

            //Craer vertices del Quad
            vertices[0] = new CustomVertex.PositionTextured(pMin, 0, 1);
            vertices[1] = new CustomVertex.PositionTextured(pMin.X, pMin.Y, pMax.Z, 0, 0);
            vertices[2] = new CustomVertex.PositionTextured(pMax, 1, 0);

            vertices[3] = new CustomVertex.PositionTextured(pMin, 0, 1);
            vertices[4] = new CustomVertex.PositionTextured(pMax, 1, 0);
            vertices[5] = new CustomVertex.PositionTextured(pMax.X, pMin.Y, pMin.Z, 1, 1);

            //Cargar vertexBuffer
            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }

        public void update(float elapsedTime)
        {
            //Actualizar incremento de UV
            if (playing)
            {
                TGCVector2 addVec = TGCVector2.Scale(speed, elapsedTime);
                currentUVsum += addVec;
                currentUVsum.X %= 1;
                currentUVsum.Y %= 1;
            }
        }

        /// <summary>
        /// Dibujar Quad
        /// </summary>
        public void Render()
        {
            var device = D3DDevice.Instance.Device;

            if (!enabled)
                return;

            //Dibujar
            Shaders shaders = GameModel.instance.shaders;
            Effect effect = shaders.effect;

            effect.Technique = GameModel.instance.shaders.ANIMATE_UV;
            effect.SetValue("animateUV", TGCVector2.Vector2ToFloat2Array(currentUVsum));
            TGCMatrix matWorldView = TGCMatrix.Identity * TGCMatrix.FromMatrix(device.Transform.View);
            TGCMatrix matWorldViewProj = matWorldView * TGCMatrix.FromMatrix(device.Transform.Projection);
            effect.SetValue("matWorld", TGCMatrix.Identity);
            effect.SetValue("matWorldView", matWorldView);
            effect.SetValue("matWorldViewProj", matWorldViewProj);

            //Activar AlphaBlending
            if (this.alphaBlendEnable)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.AlphaTestEnable = true;
            }

            device.Material = D3DDevice.DEFAULT_MATERIAL;
            TexturesManager.Instance.shaderSet(effect, "diffuseMap_Tex", texture);
            device.VertexFormat = CustomVertex.PositionTextured.Format;

            int numPasses = effect.Begin(0);
            for (int n = 0; n < numPasses; n++)
            {
                //Dibujar cada el unico subset que hay
                effect.BeginPass(n);
                device.SetStreamSource(0, vertexBuffer, 0);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                effect.EndPass();
            }

            //Finalizar shader
            effect.End();

            //Desactivar AlphaBlending
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
        }

        public void Dispose()
        {
            texture.dispose();
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
        }
    }
}
