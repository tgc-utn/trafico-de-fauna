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

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TgcViewer.Utils.Render
{
    /// <summary>
    /// Herramienta para administrar todos los elementos que se quieren renderizar
    /// a traves de una cola de renderizado.
    /// Interfaz genérica para implementar la cola de renderizado.
    /// </summary>
    public interface TgcRenderQueue
    {
        /// <summary>
        /// Limpiar toda la cola de renderizado
        /// </summary>
        void clearAll();

        /// <summary>
        /// Agregar un elemento a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento a renderizar</param>
        /// <param name="hasAlpha">True si tiene translucidez</param>
        void add(IRenderQueueElement element, bool hasAlpha);

        /// <summary>
        /// Agregar un elemento opaco a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento opaco a renderizar</param>
        void addOpaque(IRenderQueueElement element);

        /// <summary>
        /// Agregar un elemento con translucidez a renderizar a la cola de renderizado
        /// </summary>
        /// <param name="element">Elemento translucido a renderizar</param>
        void addAlpha(IRenderQueueElement element);

        /// <summary>
        /// Renderizar todos los elementos de la cola de renderizado
        /// </summary>
        void renderAll(Device d3dDevice, Vector3 cameraPos);
    }

    /// <summary>
    /// Elemento a ser renderizado en una cola de renderizado
    /// </summary>
    public interface IRenderQueueElement
    {
        /// <summary>
        /// Ejecutar el renderizado físico final del elemento
        /// </summary>
        void executeRender();

        /// <summary>
        /// Posicion absoluta del elemento en el universo.
        /// Tiene que indicar de alguna forma donde se encuentra el elemento.
        /// Puede ser el centro del objeto o algún punto relevante del elemento.
        /// </summary>
        Vector3 Position
        {
            get;
        }
    }
}
