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
using TgcViewer;

namespace Examples.TraficoFauna.Interpolators
{
    /// <summary>
    /// Utilidad para controlar el avance del tiempo hasta un tope determinado (como una cuenta regresiva)
    /// </summary>
    public class Temporizador
    {
        float current;
        float stopSegs;

        public Temporizador()
        {
        }

        public void setValues(float stopSegs)
        {
            this.stopSegs = stopSegs;
            this.current = 0;
        }

        /// <summary>
        /// Avanzar el tiempo
        /// </summary>
        /// <returns>True si llego a su fin</returns>
        public bool update()
        {
            current += GuiController.Instance.ElapsedTime;
            if (current > stopSegs)
            {
                current = stopSegs;
                return true;
            }
            return false;
        }

    }
}
