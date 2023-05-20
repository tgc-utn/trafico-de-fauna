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
using TgcViewer.Utils.TgcSceneLoader;

namespace TgcViewer.Utils.TgcGeometry
{
    /// <summary>
    /// Representa un Ray 3D con un origen y una direccion, de la forma: r = p + td
    /// </summary>
    public class TgcRay
    {

        public TgcRay()
        {
        }

        /// <summary>
        /// Se normaliza la direccion
        /// </summary>
        public TgcRay(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
            this.direction.Normalize();
        }

        Vector3 origin;
        /// <summary>
        /// Punto de origen del Ray
        /// </summary>
        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        Vector3 direction;
        /// <summary>
        /// Direccion del Ray
        /// </summary>
        public Vector3 Direction
        {
            get { return direction; }
            set { 
                direction = value;
                direction.Normalize();
            }
        }

        public override string ToString()
        {
            return "Origin[" + TgcParserUtils.printFloat(origin.X) + ", " + TgcParserUtils.printFloat(origin.Y) + ", " + TgcParserUtils.printFloat(origin.Z) + "]" +
                " Direction[" + TgcParserUtils.printFloat(direction.X) + ", " + TgcParserUtils.printFloat(direction.Y) + ", " + TgcParserUtils.printFloat(direction.Z) + "]";
        }

    }
}
