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
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;

namespace Trafico.De.Fauna.Utils.Render
{
    /// <summary>
    ///     Implementación Default del Framework para la cola de renderizado.
    /// </summary>
    public class TgcDefaultRenderQueue : TgcRenderQueue
    {
        private readonly AlphaElementComparer alphaComparer;
        private readonly List<IRenderQueueElement> alphaList;

        private readonly Dictionary<IRenderQueueElement, float> elementsViewDistance =
            new Dictionary<IRenderQueueElement, float>();

        private readonly List<IRenderQueueElement> opaqueList;

        public TgcDefaultRenderQueue()
        {
            opaqueList = new List<IRenderQueueElement>();
            alphaList = new List<IRenderQueueElement>();
            alphaComparer = new AlphaElementComparer();
        }

        public void clearAll()
        {
            opaqueList.Clear();
            alphaList.Clear();
        }

        /// <summary>
        ///     Agregar un elemento a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento a renderizar</param>
        /// <param name="hasAlpha">True si tiene translucidez</param>
        public void add(IRenderQueueElement element, bool hasAlpha)
        {
            if (!hasAlpha)
            {
                addOpaque(element);
            }
            else
            {
                addAlpha(element);
            }
        }

        /// <summary>
        ///     Agregar un elemento opaco a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento opaco a renderizar</param>
        public void addOpaque(IRenderQueueElement element)
        {
            //opaqueList.Add(element);
            throw new Exception("No usar RenderQueue: " + element);
        }

        /// <summary>
        ///     Agregar un elemento con translucidez a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento translucido a renderizar</param>
        public void addAlpha(IRenderQueueElement element)
        {
            alphaList.Add(element);
        }

        /// <summary>
        ///     Renderizar todos los elementos de la cola de renderizado
        /// </summary>
        public void renderAll(Device d3dDevice, TGCVector3 cameraPos)
        {
            renderAlphaElements(d3dDevice, cameraPos);
            renderOpaqueElements(d3dDevice);
        }

        /// <summary>
        ///     Renderizar elementos con alpha
        /// </summary>
        private void renderAlphaElements(Device d3dDevice, TGCVector3 cameraPos)
        {
            if (alphaList.Count == 0)
            {
                return;
            }

            //Ordenar por view-distance
            sortAlphaList(cameraPos);

            //Activar AlphaBlending
            d3dDevice.RenderState.AlphaBlendEnable = true;
            d3dDevice.RenderState.AlphaTestEnable = true;

            //Renderizar objetos con alpha, en orden
            foreach (var element in alphaList)
            {
                element.executeRender();
            }

            //Desactivar AlphaBlending
            d3dDevice.RenderState.AlphaBlendEnable = false;
            d3dDevice.RenderState.AlphaTestEnable = false;
        }

        /// <summary>
        ///     Ordenar elementos con Alpha según view-distance
        /// </summary>
        private void sortAlphaList(TGCVector3 cameraPos)
        {
            elementsViewDistance.Clear();

            //Calcular la distancia cuadrada de cada elemento hacia la cámara
            foreach (var element in alphaList)
            {
                var v = cameraPos - element.Position;
                elementsViewDistance.Add(element, v.LengthSq());
            }

            //Ordenar ascendentemente por view-distance
            alphaComparer.elementsViewDistance = elementsViewDistance;
            alphaList.Sort(alphaComparer);
        }

        /// <summary>
        ///     Renderizar elementos opacos
        /// </summary>
        private void renderOpaqueElements(Device d3dDevice)
        {
            foreach (var element in opaqueList)
            {
                element.executeRender();
            }
        }

        /// <summary>
        ///     Comparardor de view-distance para elementos con Alpha.
        /// </summary>
        private class AlphaElementComparer : Comparer<IRenderQueueElement>
        {
            public Dictionary<IRenderQueueElement, float> elementsViewDistance;

            public override int Compare(IRenderQueueElement x, IRenderQueueElement y)
            {
                var distX = elementsViewDistance[x];
                var distY = elementsViewDistance[y];
                return distX.CompareTo(distY);
            }
        }
    }
}
