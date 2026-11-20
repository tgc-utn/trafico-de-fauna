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

using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace Trafico.De.Fauna.Model.ItemsAnimal
{
    /// <summary>
    ///     Utilidad para rotar items que flotan en el suelo
    /// </summary>
    public class RotadorDeItems
    {
        private const float LIMIT_Y = 10;
        private const float YAW = 5f;
        private const float Y_SPEED = 50;

        private float acumY;

        private float signY;

        public RotadorDeItems()
        {
            acumY = 0;
            signY = 1;
            YSpeed = Y_SPEED;
            LimitY = LIMIT_Y;
            YawSpeed = YAW;
        }

        /// <summary>
        ///     Velocidad de rotacion
        /// </summary>
        public float YawSpeed { get; set; }

        /// <summary>
        ///     Maximo incremento en Y
        /// </summary>
        public float LimitY { get; set; }

        /// <summary>
        ///     Velocidad con la que se sube el objeto
        /// </summary>
        public float YSpeed { get; set; }

        public void rotate(float elapsedTime, TgcMesh mesh)
        {
            var move = YSpeed * elapsedTime * signY;
            acumY += move;
            if (FastMath.Abs(acumY) > LimitY)
            {
                acumY = 0;
                signY *= -1;
            }
            else
            {
                mesh.Move(0, move, 0);
            }

            mesh.RotateY(YawSpeed * elapsedTime);
        }
    }
}
