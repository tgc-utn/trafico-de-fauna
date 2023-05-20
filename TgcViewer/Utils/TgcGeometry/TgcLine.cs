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
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Render;

namespace TgcViewer.Utils.TgcGeometry
{
    /// <summary>
    /// Herramienta para crear una línea 3D y renderizarla con color.
    /// </summary>
    public class TgcLine : IRenderObject, IRenderQueueElement
    {

        #region Creacion

        /// <summary>
        /// Crea una línea en base a sus puntos extremos
        /// </summary>
        /// <param name="start">Punto de inicio</param>
        /// <param name="end">Punto de fin</param>
        /// <returns>Línea creada</returns>
        public static TgcLine fromExtremes(Vector3 start, Vector3 end)
        {
            TgcLine line = new TgcLine();
            line.pStart = start;
            line.pEnd = end;
            line.updateValues();
            return line;
        }

        /// <summary>
        /// Crea una línea en base a sus puntos extremos, con el color especificado
        /// </summary>
        /// <param name="start">Punto de inicio</param>
        /// <param name="end">Punto de fin</param>
        /// <param name="color">Color de la línea</param>
        /// <returns>Línea creada</returns>
        public static TgcLine fromExtremes(Vector3 start, Vector3 end, Color color)
        {
            TgcLine line = new TgcLine();
            line.pStart = start;
            line.pEnd = end;
            line.color = color;
            line.updateValues();
            return line;
        }

        #endregion


        CustomVertex.PositionColored[] vertices;

        Vector3 pStart;
        /// <summary>
        /// Punto de inicio de la linea
        /// </summary>
        public Vector3 PStart
        {
            get { return pStart; }
            set { pStart = value; }
        }

        Vector3 pEnd;
        /// <summary>
        /// Punto final de la linea
        /// </summary>
        public Vector3 PEnd
        {
            get { return pEnd; }
            set { pEnd = value; }
        }

        Color color;
        /// <summary>
        /// Color de la linea
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private bool enabled;
        /// <summary>
        /// Indica si la linea esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public Vector3 Position
        {
            //Lo correcto sería calcular el centro, pero con un extremo es suficiente.
            get { return pStart; }
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

        public TgcLine()
        {
            vertices = new CustomVertex.PositionColored[2];
            this.color = Color.White;
            this.enabled = true;
            this.alphaBlendEnable = false;
        }

        /// <summary>
        /// Actualizar parámetros de la línea en base a los valores configurados
        /// </summary>
        public void updateValues()
        {
            int c = color.ToArgb();

            vertices[0] = new CustomVertex.PositionColored(pStart, c);
            vertices[1] = new CustomVertex.PositionColored(pEnd, c);
        }

        /// <summary>
        /// Renderizar la línea
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            GuiController.Instance.RenderQueue.add(this, alphaBlendEnable);
        }
            

        /// <summary>
        /// Método interno, no utilizar.
        /// Renderizar en cola de renderizado.
        /// </summary>
        public void executeRender()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            texturesManager.clear(0);
            d3dDevice.Material = TgcD3dDevice.DEFAULT_MATERIAL;
            d3dDevice.Transform.World = Matrix.Identity;

            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, 1, vertices);
        }

        /// <summary>
        /// Liberar recursos de la línea
        /// </summary>
        public void dispose()
        {
        }


    }
}
