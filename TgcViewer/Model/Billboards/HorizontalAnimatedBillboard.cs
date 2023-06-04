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
using System.Drawing;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Direct3D;

namespace Examples.TraficoFauna.Billboards
{
    /// <summary>
    /// Herramienta para hacer planos horizontales animados.
    /// Crea un rectangulo 3D en XZ con una textura de sprites animada.
    /// No se mueve ni rota. Está estático.
    /// </summary>
    public class HorizontalAnimatedBillboard : IRenderObject
    {
        VertexBuffer vertexBuffer;
        TgcTexture texture;
        Size frameSize;
        int totalFrames;
        float currentTime;
        float animationTimeLenght;
        int framesPerRow;
        int framesPerColumn;
        float textureWidth;
        float textureHeight;

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

        int currentFrame;
        /// <summary>
        /// Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
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

        private float frameRate;
        /// <summary>
        /// Velocidad de la animacion medida en cuadros por segundo.
        /// </summary>
        public float FrameRate
        {
            get { return frameRate; }
        }

        public HorizontalAnimatedBillboard(Device device, string texturePath, Size frameSize, int totalFrames, float frameRate)
        {
            this.enabled = true;
            this.alphaBlendEnable = true;
            this.currentFrame = 0;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            this.currentTime = 0;
            this.frameRate = frameRate;
            this.playing = true;

            //Crear vertex buffer y textura
            this.texture = TgcTexture.createTexture(device, texturePath);
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionTextured), 6, device,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Calcular valores de frames de la textura
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            framesPerColumn = (int)textureWidth / frameSize.Width;
            framesPerRow = (int)textureHeight / frameSize.Height;
            int realTotalFrames = framesPerRow * framesPerColumn;
            if (realTotalFrames > totalFrames)
            {
                throw new Exception("Error en QuadAnimatedBillboard. No coinciden la cantidad de frames y el tamaño de la textura: " + totalFrames);
            }
            animationTimeLenght = (float)totalFrames / frameRate;
        }

        /// <summary>
        /// Actualizar parámetros del plano en base a los valores configurados
        /// </summary>
        public void updateValues(float elapsedTime)
        {
            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[6];

            //Avanzar tiempo
            if (playing)
            {
                currentTime += elapsedTime;
                if (currentTime > animationTimeLenght)
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                }
            }

            //Obtener cuadro actual
            currentFrame = (int)(currentTime * frameRate);

            //Crear coordenadas UV en base al frame que corresponde dibujar
            float minU = (frameSize.Width * (currentFrame % framesPerRow)) / textureWidth;
            float maxU = (frameSize.Width * (currentFrame % framesPerRow) + frameSize.Width) / textureWidth;
            float minV = (frameSize.Height * (currentFrame % framesPerColumn)) / textureHeight;
            float maxV = (frameSize.Height * (currentFrame % framesPerColumn) + frameSize.Height) / textureHeight;

            //Craer vertices del Quad
            vertices[0] = new CustomVertex.PositionTextured(pMin, minU, maxV);
            vertices[1] = new CustomVertex.PositionTextured(pMin.X, pMin.Y, pMax.Z, minU, minV);
            vertices[2] = new CustomVertex.PositionTextured(pMax, maxU, minV);

            vertices[3] = new CustomVertex.PositionTextured(pMin, minU, maxV);
            vertices[4] = new CustomVertex.PositionTextured(pMax, maxU, minV);
            vertices[5] = new CustomVertex.PositionTextured(pMax.X, pMin.Y, pMin.Z, maxU, maxV);

            //Cargar vertexBuffer
            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }

        /// <summary>
        /// Dibujar Quad
        /// </summary>
        public void render()
        {
            var device = D3DDevice.Instance.Device;

            if (!enabled)
                return;

            //Activar AlphaBlending
            if (this.alphaBlendEnable)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.AlphaTestEnable = true;
            }

            TexturesManager.Instance.set(0, texture);
            device.Material = D3DDevice.DEFAULT_MATERIAL;
            device.Transform.World = Matrix.Identity;

            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, vertexBuffer, 0);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

            //Desactivar AlphaBlending
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
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
