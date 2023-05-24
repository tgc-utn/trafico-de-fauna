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
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using TgcViewer.Utils;
using TgcViewer;
using Microsoft.DirectX;

namespace Examples.TraficoFauna.Billboards
{
    public class PointSpriteBillboard
    {

        TgcTexture texture;
        SpriteVertexVertex[] spriteVertexArray;
        VertexDeclaration vertexDeclaration;

        /// <summary>
        /// Estructura de Vertice para el Sprite
        /// </summary>
        public struct SpriteVertexVertex
        {
            public Vector3 Position;
            public float PointSize;
        }

        /// <summary>
        /// FVF para formato de vertice del Sprite
        /// </summary>
        public static readonly VertexElement[] SpriteVertexVertexElements = new VertexElement[]
        {
            new VertexElement(0, 0, DeclarationType.Float3,
                                    DeclarationMethod.Default,
                                    DeclarationUsage.Position, 0),
                                    
            new VertexElement(0, 12, DeclarationType.Float1,
                                     DeclarationMethod.Default,
                                     DeclarationUsage.PointSize, 0),

            VertexElement.VertexDeclarationEnd 
        };


        int currentFrame;
        /// <summary>
        /// Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        

        public PointSpriteBillboard(TgcTexture texture, Size frameSize, int totalFrames)
        {
            currentFrame = 0;
            SpriteVertexVertex vertex = new SpriteVertexVertex();
            vertex.PointSize = 10;
            vertex.Position = new Vector3(0, 50, 0);
            this.texture = texture;


            Device device = GuiController.Instance.D3dDevice;
            vertexDeclaration = new VertexDeclaration(device, SpriteVertexVertexElements);
            spriteVertexArray = new SpriteVertexVertex[1];
            spriteVertexArray[0] = vertex;
        }

        public void render()
        {
            Device device = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //Cargar VertexDeclaration
            device.VertexDeclaration = vertexDeclaration;

            //Fijo la textura actual de la particula.
            texturesManager.clear(0);
            texturesManager.clear(1);
            texturesManager.set(0, texture);
            device.Material = TgcD3dDevice.DEFAULT_MATERIAL;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.ZBufferWriteEnable = false;
            device.Transform.World = Matrix.Identity;

            device.DrawUserPrimitives(PrimitiveType.PointList, 1, spriteVertexArray);

            //Restaurar valores de RenderState
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.ZBufferWriteEnable = true;
        }

        public void dispose()
        {
            texture.dispose();
            spriteVertexArray = null;
        }


    }
}
