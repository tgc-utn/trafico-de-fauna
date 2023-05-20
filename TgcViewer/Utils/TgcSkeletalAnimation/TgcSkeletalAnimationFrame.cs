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

namespace TgcViewer.Utils.TgcSkeletalAnimation
{
    /// <summary>
    /// Key frame de animacion para un hueso particular
    /// </summary>
    public class TgcSkeletalAnimationFrame
    {
        public TgcSkeletalAnimationFrame(int frame, Vector3 position, Quaternion rotation)
        {
            this.frame = frame;
            this.position = position;
            this.rotation = rotation;
        }

        private int frame;
        /// <summary>
        /// Numero de frame en el cual transcurre esta rotacion y traslacion.
        /// </summary>
        public int Frame
        {
            get { return frame; }
        }

        private Vector3 position;
        /// <summary>
        /// Posicion del hueso para este frame
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }

        private Quaternion rotation;
        /// <summary>
        /// Rotacion del hueso para este frame en Quaternion
        /// </summary>
        public Quaternion Rotation
        {
            get { return rotation; }
        }


    }
}
