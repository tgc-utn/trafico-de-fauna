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

namespace Trafico.De.Fauna.Model.Particles
{
    class PilaDeParticulas
    {
        private Particle[] pila = null;
		private int i_cima;

        public PilaDeParticulas(int iMax)
		{
			pila = new Particle[iMax];
			i_cima = 0;
		}

        public bool push(Particle p)
		{
            //Esta llena la pila.
			if (i_cima == pila.Length)
				return false;

			pila[i_cima] = p;
			i_cima++;

			return true;
		}

        public bool pop(out Particle p)
		{
			if (i_cima == 0)
			{
				p = null;
				return false;
			}

			i_cima--;
            p = pila[i_cima];

			return true;
		}
    }
}
