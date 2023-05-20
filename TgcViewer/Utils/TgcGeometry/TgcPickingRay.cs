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

namespace TgcViewer.Utils.TgcGeometry
{
    /// <summary>
    /// Utilidad para crear 
    /// </summary>
    public class TgcPickingRay
    {
        private TgcRay ray;
        /// <summary>
        /// Ray que representa la acción de Picking
        /// </summary>
        public TgcRay Ray
        {
            get { return ray; }
        }
        
        public TgcPickingRay()
        {
            ray = new TgcRay();
        }

        /// <summary>
        /// Actualiza el Ray de colisión en base a la posición del mouse en la pantalla
        /// </summary>
        public void updateRay()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Crear Ray en base a coordenadas del mouse
            float sx = GuiController.Instance.D3dInput.Xpos;
            float sy = GuiController.Instance.D3dInput.Ypos;
            int w = d3dDevice.Viewport.Width;
            int h = d3dDevice.Viewport.Height;
            Matrix matProj = d3dDevice.Transform.Projection;

            Vector3 v = new Vector3();
            v.X = (((2.0f * sx) / w) - 1) / matProj.M11;
            v.Y = -(((2.0f * sy) / h) - 1) / matProj.M22;
            v.Z = 1.0f;

            //Transform the screen space pick ray into 3D space
            Matrix m = Matrix.Invert(d3dDevice.Transform.View);
            Vector3 rayDir = new Vector3(
                v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31,
                v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32,
                v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33
                );
            Vector3 rayOrig = new Vector3(m.M41, m.M42, m.M43);


            //Picking Ray creado
            ray.Origin = rayOrig;
            ray.Direction = rayDir;
        }

    }
}
