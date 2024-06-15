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

namespace Trafico.De.Fauna.Model.Interpolators
{
    /// <summary>
    ///     Utilidad para interpolar un valor entre un MIN y un MAX, variando en forma de vaiven (subi y baja)
    ///     a una velocidad determinada
    /// </summary>
    public class InterpoladorVaiven
    {
        private float current;
        private readonly float max;
        private readonly float min;
        private float speed;

        public InterpoladorVaiven(float min, float max, float speed)
        {
            this.min = min;
            this.max = max;
            this.speed = speed;
            current = 0;
        }

        /// <summary>
        ///     Interpolar y devolver incremento
        /// </summary>
        public float update(float elapsedTime)
        {
            var n = speed * elapsedTime;
            current += n;
            if (current > max)
            {
                speed *= -1;
                current = max;
            }
            else if (current < min)
            {
                speed *= -1;
                current = min;
            }

            return current;
        }
    }
}
