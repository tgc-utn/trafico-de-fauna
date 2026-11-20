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

namespace Trafico.De.Fauna.Model.Interpolators
{
    /// <summary>
    ///     Interpolador lineal de posicion en 2D.
    /// </summary>
    public class Position2dInterpolator
    {
        private TGCVector2 current;
        private TGCVector2 dir;
        private float distanceToTravel;

        /// <summary>
        ///     Velocidad de desplazamiento
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Posicion inicial
        /// </summary>
        public TGCVector2 Init { get; set; }

        /// <summary>
        ///     Posicion final
        /// </summary>
        public TGCVector2 End { get; set; }

        public void reset()
        {
            dir = End - Init;
            distanceToTravel = dir.Length();
            dir.Normalize();
            current = Init;
        }

        /// <summary>
        ///     Actualizar estado del interpolador
        /// </summary>
        /// <returns>Nueva posicion</returns>
        public TGCVector2 update(float elapsedTime)
        {
            var movement = Speed * elapsedTime;
            distanceToTravel -= movement;
            if (distanceToTravel < 0)
            {
                distanceToTravel = 0;
                current = End;
            }
            else
            {
                current += TGCVector2.Scale(dir, movement);
            }

            return current;
        }

        /// <summary>
        ///     Indica si el interpolador llego al final
        /// </summary>
        public bool isEnd()
        {
            return distanceToTravel == 0;
        }
    }
}
