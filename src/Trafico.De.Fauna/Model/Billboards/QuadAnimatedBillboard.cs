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
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Billboards
{
    /// <summary>
    ///     Herramienta para crear Billboards animados.
    ///     Crea un rectangulo 3D en XY con una textura de sprites animada en forma de tiles.
    /// </summary>
    public class QuadAnimatedBillboard : IRenderObject, IAlphaObject
    {
        private readonly float animationTimeLenght;

        private float currentTime;


        private Size frameSize;
        private readonly int framesPerColumn;
        private readonly int framesPerRow;


        private TGCVector2 size;
        private readonly TGCTexture texture;
        private readonly float textureHeight;
        private readonly float textureWidth;
        private int totalFrames;
        private readonly VertexBuffer vertexBuffer;

        /// <summary>
        ///     Crear un nuevo Quad animado
        /// </summary>
        /// <param name="texturePath">path de la textura animada</param>
        /// <param name="frameSize">tamaño de un tile de la animacion</param>
        /// <param name="totalFrames">cantidad de frames que tiene la animacion</param>
        /// <param name="frameRate">velocidad en cuadros por segundo</param>
        public QuadAnimatedBillboard(string texturePath, Size frameSize, int totalFrames, float frameRate)
        {
            Enabled = true;
            AlphaBlendEnable = true;
            CurrentFrame = 0;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            currentTime = 0;
            this.FrameRate = frameRate;
            RotationY = 0;
            Playing = true;

            //Crear vertex buffer y textura
            var d3dDevice = D3DDevice.Instance.Device;
            texture = TGCTexture.CreateTexture(d3dDevice, texturePath);
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionTextured), 6, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Calcular valores de frames de la textura
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            framesPerColumn = (int)textureWidth / frameSize.Width;
            framesPerRow = (int)textureHeight / frameSize.Height;
            var realTotalFrames = framesPerRow * framesPerColumn;
            if (realTotalFrames > totalFrames)
            {
                throw new Exception(
                    "Error en QuadAnimatedBillboard. No coinciden la cantidad de frames y el tamaño de la textura: " +
                    totalFrames);
            }

            animationTimeLenght = totalFrames / frameRate;
        }

        /// <summary>
        ///     Indica si la pared esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Arrancar o parar avance de animacion
        /// </summary>
        public bool Playing { get; set; }

        /// <summary>
        ///     Posicion del billboard.
        ///     El eje Y se toma desde abajo
        /// </summary>
        public TGCVector3 Position { get; set; }

        /// <summary>
        ///     Rotacion Y del Billboard
        /// </summary>
        public float RotationY { get; set; }

        /// <summary>
        ///     Tamaño del billboard en XY
        /// </summary>
        public TGCVector2 Size
        {
            get => size;
            set => size = value;
        }

        /// <summary>
        ///     Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame { get; set; }

        /// <summary>
        ///     Velocidad de la animacion medida en cuadros por segundo.
        /// </summary>
        public float FrameRate { get; }

        /// <summary>
        ///     Dibujar con Alpha
        /// </summary>
        public void renderWithAlpha()
        {
            if (!Enabled)
            {
                return;
            }

            Render();
        }

        /// <summary>
        ///     Habilita el renderizado con AlphaBlending para los modelos
        ///     con textura o colores por vértice de canal Alpha.
        ///     Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable { get; set; }

        /// <summary>
        ///     Dibujar Quad
        /// </summary>
        public void Render()
        {
            var device = D3DDevice.Instance.Device;

            if (!Enabled)
            {
                return;
            }

            //Activar AlphaBlending
            if (AlphaBlendEnable)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.AlphaTestEnable = true;
            }

            TexturesManager.Instance.set(0, texture);
            device.Material = D3DDevice.DEFAULT_MATERIAL;
            device.Transform.World = TGCMatrix.RotationY(RotationY) * TGCMatrix.Translation(Position);

            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, vertexBuffer, 0);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

            //Desactivar AlphaBlending
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
        }

        public void Dispose()
        {
            texture.Dispose();
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
        }

        /// <summary>
        ///     Actualizar parámetros del plano en base a los valores configurados
        /// </summary>
        public void updateValues(float elapsedTime)
        {
            var vertices = new CustomVertex.PositionTextured[6];

            //Crear un Quad con dos triángulos sobre XY
            var halfSize = TGCVector2.Scale(size, 0.5f);
            var min = new TGCVector3(-halfSize.X, 0, 0);
            var max = new TGCVector3(halfSize.X, size.Y, 0);

            //Avanzar tiempo
            if (Playing)
            {
                currentTime += elapsedTime;
                if (currentTime > animationTimeLenght)
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                }
            }

            //Obtener cuadro actual
            CurrentFrame = (int)(currentTime * FrameRate);

            //Crear coordenadas UV en base al frame que corresponde dibujar
            var minU = frameSize.Width * (CurrentFrame % framesPerRow) / textureWidth;
            var maxU = (frameSize.Width * (CurrentFrame % framesPerRow) + frameSize.Width) / textureWidth;
            var minV = frameSize.Height * (CurrentFrame % framesPerColumn) / textureHeight;
            var maxV = (frameSize.Height * (CurrentFrame % framesPerColumn) + frameSize.Height) / textureHeight;

            //Craer vertices del Quad
            vertices[0] = new CustomVertex.PositionTextured(min, minU, maxV);
            vertices[1] = new CustomVertex.PositionTextured(min.X, max.Y, 0, minU, minV);
            vertices[2] = new CustomVertex.PositionTextured(max, maxU, minV);

            vertices[3] = new CustomVertex.PositionTextured(min, minU, maxV);
            vertices[4] = new CustomVertex.PositionTextured(max, maxU, minV);
            vertices[5] = new CustomVertex.PositionTextured(max.X, min.Y, 0, maxU, maxV);

            //Cargar vertexBuffer
            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }
    }
}
