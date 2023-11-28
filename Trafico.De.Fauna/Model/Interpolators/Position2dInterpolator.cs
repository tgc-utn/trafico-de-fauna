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
    /// Interpolador lineal de posicion en 2D.
    /// </summary>
    public class Position2dInterpolator
    {
        TGCVector2 current;
        TGCVector2 dir;
        float distanceToTravel;

        float speed;
        /// <summary>
        /// Velocidad de desplazamiento
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        TGCVector2 init;
        /// <summary>
        /// Posicion inicial
        /// </summary>
        public TGCVector2 Init
        {
            get { return init; }
            set { init = value; }
        }

        TGCVector2 end;
        /// <summary>
        /// Posicion final
        /// </summary>
        public TGCVector2 End
        {
            get { return end; }
            set { end = value; }
        }

        public Position2dInterpolator()
        {
        }

        public void reset()
        {
            dir = end - init;
            distanceToTravel = dir.Length();
            dir.Normalize(); 
            current = init;
        }

        /// <summary>
        /// Actualizar estado del interpolador
        /// </summary>
        /// <returns>Nueva posicion</returns>
        public TGCVector2 update(float elapsedTime)
        {
            float movement = speed * elapsedTime;
            distanceToTravel -= movement;
            if (distanceToTravel < 0)
            {
                distanceToTravel = 0;
                current = end;
            }
            else
            {
                current += TGCVector2.Scale(dir, movement);
            }
            return current;
        }

        /// <summary>
        /// Indica si el interpolador llego al final
        /// </summary>
        public bool isEnd()
        {
            return distanceToTravel == 0;
        } 

    }
}
