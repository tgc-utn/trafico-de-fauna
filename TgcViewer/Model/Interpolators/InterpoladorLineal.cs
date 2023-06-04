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

namespace Examples.TraficoFauna.Interpolators
{
    /// <summary>
    /// Utilidad para interpolar un valor que arranca en INIT y va hasta END, a una velocidad determinada.
    /// El metodo isEnd() indica si termino.
    /// </summary>
    public class InterpoladorLineal
    {
        float current;
        float end;
        float speed;

        public InterpoladorLineal()
        {
        }

        public InterpoladorLineal(float init, float end, float speed)
        {
            setValues(init, end, speed);
        }

        public void setValues(float init, float end, float speed)
        {
            this.current = init;
            this.end = end;
            this.speed = speed;
        }

        /// <summary>
        /// Interpolar y devolver incremento
        /// </summary>
        public float update(float elapsedTime)
        {
            float n = speed * elapsedTime;
            current += n;
            if (speed > 0)
            {
                if (current > end)
                {
                    current = end;
                }
            }
            else
            {
                if (current < end)
                {
                    current = end;
                }
            }

            return current;
        }

        /// <summary>
        /// Indica si el interpolador llego al final
        /// </summary>
        public bool isEnd()
        {
            return current == end;
        }

    }
}
