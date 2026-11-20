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
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace Trafico.De.Fauna.Model.Billboards
{
    public class PointSpriteBillboard
    {
        /// <summary>
        ///     FVF para formato de vertice del Sprite
        /// </summary>
        public static readonly VertexElement[] SpriteVertexVertexElements =
        {
            new VertexElement(0, 0, DeclarationType.Float3,
                DeclarationMethod.Default,
                DeclarationUsage.Position, 0),

            new VertexElement(0, 12, DeclarationType.Float1,
                DeclarationMethod.Default,
                DeclarationUsage.PointSize, 0),

            VertexElement.VertexDeclarationEnd
        };


        private SpriteVertexVertex[] spriteVertexArray;

        private readonly TGCTexture texture;
        private readonly VertexDeclaration vertexDeclaration;


        public PointSpriteBillboard(TGCTexture texture, Size frameSize, int totalFrames)
        {
            CurrentFrame = 0;
            var vertex = new SpriteVertexVertex();
            vertex.PointSize = 10;
            vertex.Position = new TGCVector3(0, 50, 0);
            this.texture = texture;


            var device = D3DDevice.Instance.Device;
            vertexDeclaration = new VertexDeclaration(device, SpriteVertexVertexElements);
            spriteVertexArray = new SpriteVertexVertex[1];
            spriteVertexArray[0] = vertex;
        }

        /// <summary>
        ///     Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame { get; set; }

        public void render(Device device)
        {
            //Cargar VertexDeclaration
            device.VertexDeclaration = vertexDeclaration;

            //Fijo la textura actual de la particula.
            TexturesManager.Instance.clear(0);
            TexturesManager.Instance.clear(1);
            TexturesManager.Instance.set(0, texture);
            device.Material = D3DDevice.DEFAULT_MATERIAL;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.ZBufferWriteEnable = false;
            device.Transform.World = TGCMatrix.Identity;

            device.DrawUserPrimitives(PrimitiveType.PointList, 1, spriteVertexArray);

            //Restaurar valores de RenderState
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.ZBufferWriteEnable = true;
        }

        public void dispose()
        {
            texture.Dispose();
            spriteVertexArray = null;
        }

        /// <summary>
        ///     Estructura de Vertice para el Sprite
        /// </summary>
        public struct SpriteVertexVertex
        {
            public TGCVector3 Position;
            public float PointSize;
        }
    }
}
