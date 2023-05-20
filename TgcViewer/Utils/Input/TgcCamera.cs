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

namespace TgcViewer.Utils.Input
{
    /// <summary>
    /// Interfaz general para una cámara del Framework
    /// </summary>
    public interface TgcCamera
    {
        /// <summary>
        /// Posición de la cámara
        /// </summary>
        Vector3 getPosition();

        /// <summary>
        /// Posición del punto al que mira la cámara
        /// </summary>
        Vector3 getLookAt();

        /// <summary>
        /// Actualizar el estado interno de la cámara en cada frame
        /// </summary>
        void updateCamera();

        /// <summary>
        /// Actualizar la matriz View en base a los valores de la cámara
        /// </summary>
        void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice);
    }
}
