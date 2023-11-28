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

using TGC.Core.Camara;
using TGC.Core.Mathematica;

namespace Trafico.De.Fauna.Model.Camera
{
    public class GameThirdPersonCamera : TgcCamera
    {
        static readonly TGCVector3 UP_VECTOR = new TGCVector3(0, 1, 0);

        float offsetHeight;
        /// <summary>
        /// Desplazamiento en altura de la camara respecto del target
        /// </summary>
        public float OffsetHeight
        {
            get { return offsetHeight; }
            set { offsetHeight = value; }
        }

        float offsetForward;
        /// <summary>
        /// Desplazamiento hacia adelante o atras de la camara repecto del target.
        /// Para que sea hacia atras tiene que ser negativo.
        /// </summary>
        public float OffsetForward
        {
            get { return offsetForward; }
            set { offsetForward = value; }
        }

        TGCVector3 targetDisplacement;
        /// <summary>
        /// Desplazamiento final que se le hace al target para acomodar la camara en un cierto
        /// rincon de la pantalla
        /// </summary>
        public TGCVector3 TargetDisplacement
        {
            get { return targetDisplacement; }
            set { targetDisplacement = value; }
        }

        TGCMatrix viewTGCMatrix;

        float rotationY;
        /// <summary>
        /// Rotacion absoluta en Y de la camara
        /// </summary>
        public float RotationY
        {
            get { return rotationY; }
            set { rotationY = value; }
        }

        TGCVector3 target;
        /// <summary>
        /// Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public TGCVector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        TGCVector3 position;
        /// <summary>
        /// Posicion del ojo de la camara
        /// </summary>
        public TGCVector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public GameThirdPersonCamera()
        {
            offsetHeight = 20;
            offsetForward = -120;
            rotationY = 0;
            targetDisplacement = TGCVector3.Empty;
            target = TGCVector3.Empty;
            position = TGCVector3.Empty;
        }

        /// <summary>
        /// Genera la proxima matriz de view, sin actualizar aun los valores internos
        /// </summary>
        /// <param name="pos">Futura posicion de camara generada</param>
        /// <param name="pos">Futuro centro de camara a generada</param>
        /// <returns>Futura matriz de view generada</returns>
        public TGCMatrix generateViewMatrix(out TGCVector3 pos, out TGCVector3 targetCenter)
        {
            //alejarse, luego rotar y lueg ubicar camara en el centro deseado
            targetCenter = TGCVector3.Add(target, targetDisplacement);
            TGCMatrix m = TGCMatrix.Translation(0, offsetHeight, offsetForward) * TGCMatrix.RotationY(rotationY) * TGCMatrix.Translation(targetCenter);

            //Extraer la posicion final de la matriz de transformacion
            pos = new TGCVector3(m.M41, m.M42, m.M43);

            //Obtener ViewTGCMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            return TGCMatrix.LookAtLH(pos, targetCenter, UP_VECTOR);
        }

        public override void UpdateCamera(float elapsedTime)
        {
            TGCVector3 targetCenter;
            viewTGCMatrix = generateViewMatrix(out position, out targetCenter); 
        }

        /// <summary>
        /// Rotar la camara respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            rotationY += angle;
        }

        public TGCVector3 getPosition()
        {
            return position;
        }

        public TGCVector3 getLookAt()
        {
            return target;
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            d3dDevice.Transform.View = viewTGCMatrix;
        }

    }
}
