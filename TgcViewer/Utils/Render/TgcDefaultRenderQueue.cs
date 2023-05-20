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
using Microsoft.DirectX;
using System.Collections;

namespace TgcViewer.Utils.Render
{
    /// <summary>
    /// Implementación Default del Framework para la cola de renderizado.
    /// </summary>
    public class TgcDefaultRenderQueue : TgcRenderQueue
    {
        List<IRenderQueueElement> opaqueList;
        List<IRenderQueueElement> alphaList;
        Dictionary<IRenderQueueElement, float> elementsViewDistance = new Dictionary<IRenderQueueElement, float>();
        AlphaElementComparer alphaComparer;

        public TgcDefaultRenderQueue()
        {
            opaqueList = new List<IRenderQueueElement>();
            alphaList = new List<IRenderQueueElement>();
            alphaComparer = new AlphaElementComparer();
        }

        /// <summary>
        /// Agregar un elemento a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento a renderizar</param>
        /// <param name="hasAlpa">True si tiene translucidez</param>
        public void clearAll()
        {
            opaqueList.Clear();
            alphaList.Clear();
        }

        /// <summary>
        /// Agregar un elemento a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento a renderizar</param>
        /// <param name="hasAlpa">True si tiene translucidez</param>
        public void add(IRenderQueueElement element, bool hasAlpa)
        {
            if (!hasAlpa)
            {
                addOpaque(element);
            }
            else
            {
                addAlpha(element);
            }  
        }

        /// <summary>
        /// Agregar un elemento opaco a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento opaco a renderizar</param>
        public void addOpaque(IRenderQueueElement element)
        {
            //opaqueList.Add(element);
            throw new Exception("No usar RenderQueue: " + element.ToString());
        }

        /// <summary>
        /// Agregar un elemento con translucidez a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento translucido a renderizar</param>
        public void addAlpha(IRenderQueueElement element)
        {
            alphaList.Add(element);
        }

        /// <summary>
        /// Renderizar todos los elementos de la cola de renderizado
        /// </summary>
        public void renderAll()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            renderAlphaElements(d3dDevice);
            renderOpaqueElements(d3dDevice);
        }

        /// <summary>
        /// Renderizar elementos con alpha
        /// </summary>
        private void renderAlphaElements(Device d3dDevice)
        {
            if (alphaList.Count == 0)
                return;


            //Ordenar por view-distance
            sortAlphaList();

            //Activar AlphaBlending
            d3dDevice.RenderState.AlphaBlendEnable = true;
            d3dDevice.RenderState.AlphaTestEnable = true;

            //Renderizar objetos con alpha, en orden
            foreach (IRenderQueueElement element in alphaList)
            {
                element.executeRender();
            }

            //Desactivar AlphaBlending
            d3dDevice.RenderState.AlphaBlendEnable = false;
            d3dDevice.RenderState.AlphaTestEnable = false;
        }

        
        /// <summary>
        /// Ordenar elementos con Alpha según view-distance
        /// </summary>
        private void sortAlphaList()
        {
            this.elementsViewDistance.Clear();

            //Calcular la distancia cuadrada de cada elemento hacia la cámara
            Vector3 cameraPos = GuiController.Instance.CurrentCamera.getPosition();
            foreach (IRenderQueueElement element in alphaList)
            {
                Vector3 v = cameraPos - element.Position;
                elementsViewDistance.Add(element, v.LengthSq());
            }

            //Ordenar ascendentemente por view-distance
            alphaComparer.elementsViewDistance = this.elementsViewDistance;
            alphaList.Sort(alphaComparer);
        }

        /// <summary>
        /// Renderizar elementos opacos
        /// </summary>
        private void renderOpaqueElements(Device d3dDevice)
        {
            foreach (IRenderQueueElement element in opaqueList)
            {
                element.executeRender();
            }
        }


        /// <summary>
        /// Comparardor de view-distance para elementos con Alpha.
        /// </summary>
        private class AlphaElementComparer : Comparer<IRenderQueueElement>
        {
            public Dictionary<IRenderQueueElement, float> elementsViewDistance;

            public override int Compare(IRenderQueueElement x, IRenderQueueElement y)
            {
                float distX = elementsViewDistance[x];
                float distY = elementsViewDistance[y];
                return distX.CompareTo(distY);
            }
        }
        


    }


}
