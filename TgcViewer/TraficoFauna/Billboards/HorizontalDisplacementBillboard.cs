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
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils;
using TraficoFauna.Examples;

namespace Examples.TraficoFauna.Billboards
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
        Vector2 currentUVsum;


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

        Vector3 pMin;
        /// <summary>
        /// Punto minimo del Billboard
        /// </summary>
        public Vector3 PMin
        {
            get { return pMin; }
            set { pMin = value; }
        }

        Vector3 pMax;
        /// <summary>
        /// Punto maximo del Billboard.
        /// El valor de Y no se considera. Se toma siempre el de PMin.
        /// </summary>
        public Vector3 PMax
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

        private Vector2 speed;
        /// <summary>
        /// Velocidad de desplazamiento de las coordenadas UV
        /// </summary>
        public Vector2 Speed
        {
          get { return speed; }
          set { speed = value; }
        }



        public HorizontalDisplacementBillboard(string texturePath, Vector3 pMin, Vector3 pMax)
        {
            this.enabled = true;
            this.alphaBlendEnable = true;
            this.speed = new Vector2(0, 1);
            this.playing = true;
            this.pMin = pMin;
            this.pMax = pMax;
            currentUVsum = new Vector2(0, 0);

            //Cargar textura
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.texture = TgcTexture.createTexture(d3dDevice, texturePath);

            //Crear vertexBuffer
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionTextured), 6, d3dDevice,
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

        /// <summary>
        /// Dibujar Quad
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            //Actualizar incremento de UV
            if (playing)
            {
                Vector2 addVec = Vector2.Scale(speed, GuiController.Instance.ElapsedTime);
                currentUVsum += addVec;
                currentUVsum.X %= 1;
                currentUVsum.Y %= 1;
            }

            //Dibujar
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;
            Shaders shaders = Game.instance.shaders;
            Effect effect = shaders.effect;

            effect.Technique = Game.instance.shaders.ANIMATE_UV;
            effect.SetValue("animateUV", TgcParserUtils.vector2ToFloat2Array(currentUVsum));
            Matrix matWorldView = Matrix.Identity * d3dDevice.Transform.View;
            Matrix matWorldViewProj = matWorldView * d3dDevice.Transform.Projection;
            effect.SetValue("matWorld", Matrix.Identity);
            effect.SetValue("matWorldView", matWorldView);
            effect.SetValue("matWorldViewProj", matWorldViewProj);

            //Activar AlphaBlending
            if (this.alphaBlendEnable)
            {
                d3dDevice.RenderState.AlphaBlendEnable = true;
                d3dDevice.RenderState.AlphaTestEnable = true;
            }

            d3dDevice.Material = TgcD3dDevice.DEFAULT_MATERIAL;
            texturesManager.shaderSet(effect, "diffuseMap_Tex", texture);
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;

            int numPasses = effect.Begin(0);
            for (int n = 0; n < numPasses; n++)
            {
                //Dibujar cada el unico subset que hay
                effect.BeginPass(n);
                d3dDevice.SetStreamSource(0, vertexBuffer, 0);
                d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                effect.EndPass();
            }

            //Finalizar shader
            effect.End();

            

            //Desactivar AlphaBlending
            d3dDevice.RenderState.AlphaBlendEnable = false;
            d3dDevice.RenderState.AlphaTestEnable = false;
        }

        public void dispose()
        {
            texture.dispose();
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
        }

        

    }
}
