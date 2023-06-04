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
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace Examples.TraficoFauna.ItemsAnimal
{
    /// <summary>
    /// Utilidad para rotar items que flotan en el suelo
    /// </summary>
    public class RotadorDeItems
    {
        const float LIMIT_Y = 10;
        const float YAW = 5f;
        const float Y_SPEED = 50;

        float acumY;
        float signY;

        float yawSpeed;
        /// <summary>
        /// Velocidad de rotacion
        /// </summary>
        public float YawSpeed
        {
            get { return yawSpeed; }
            set { yawSpeed = value; }
        }

        float limitY;
        /// <summary>
        /// Maximo incremento en Y
        /// </summary>
        public float LimitY
        {
            get { return limitY; }
            set { limitY = value; }
        }

        float ySpeed;
        /// <summary>
        /// Velocidad con la que se sube el objeto
        /// </summary>
        public float YSpeed
        {
            get { return ySpeed; }
            set { ySpeed = value; }
        }

        public RotadorDeItems()
        {
            acumY = 0;
            signY = 1;
            ySpeed = Y_SPEED;
            limitY = LIMIT_Y;
            yawSpeed = YAW;
        }

        public void rotate(float elapsedTime, TgcMesh mesh)
        {
            float move = ySpeed * elapsedTime * signY;
            acumY += move;
            if (FastMath.Abs(acumY) > limitY)
            {
                acumY = 0;
                signY *= -1;
            }
            else
            {
                mesh.move(0, move, 0); 
            }
            mesh.rotateY(yawSpeed * elapsedTime);
        }

    }
}
