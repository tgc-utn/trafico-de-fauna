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
    /// Hueso del esqueleto
    /// </summary>
    public class TgcSkeletalBone
    {

        public TgcSkeletalBone(int index, string name, Vector3 startPosition, Quaternion startRotation)
        {
            this.index = index;
            this.name = name;
            this.startPosition = startPosition;
            this.startRotation = startRotation;

            this.matLocal = Matrix.RotationQuaternion(this.startRotation) * Matrix.Translation(this.startPosition);
        }

        private Vector3 startPosition;
        /// <summary>
        /// Posicion inicial del hueso
        /// </summary>
        public Vector3 StartPosition
        {
            get { return startPosition; }
        }

        private Quaternion startRotation;
        /// <summary>
        /// Rotacion inicial del hueso
        /// </summary>
        public Quaternion StartRotation
        {
            get { return startRotation; }
        }

        Matrix matLocal;
        /// <summary>
        /// Matriz local de transformacion
        /// </summary>
        public Matrix MatLocal
        {
            get { return matLocal; }
            set { matLocal = value; }
        }

        Matrix matFinal;
        /// <summary>
        /// Matriz final de transformacion 
        /// </summary>
        public Matrix MatFinal
        {
            get { return matFinal; }
            set { matFinal = value; }
        }

        Matrix matInversePose;
        /// <summary>
        /// Matriz de transformacion inversa de la posicion inicial del hueso, para la animacion actual
        /// </summary>
        public Matrix MatInversePose
        {
            get { return matInversePose; }
            set { matInversePose = value; }
        }

        private int index;
        /// <summary>
        /// Posición del hueso dentro del array de huesos de todo el esqueleto
        /// </summary>
        public int Index
        {
            get { return index; }
        }


        private string name;
        /// <summary>
        /// Nombre del hueso
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        private TgcSkeletalBone parentBone;
        /// <summary>
        /// Hueso padre. Es null si no tiene
        /// </summary>
        public TgcSkeletalBone ParentBone
        {
            get { return parentBone; }
            set { parentBone = value; }
        }


        public override string ToString()
        {
            return "Bone: " + name;
        }

    }
}
