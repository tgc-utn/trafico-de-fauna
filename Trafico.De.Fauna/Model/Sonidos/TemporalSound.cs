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

using TGC.Core.Sound;

namespace Trafico.De.Fauna.Model.Sonidos
{
    /// <summary>
    ///     Sonido sin loop que se ejecuta con una cierta frecuencia
    /// </summary>
    public class TemporalSound
    {
        private float acumTime;

        private int currentTimes;

        public TemporalSound()
        {
            Frecuency = 1;
            acumTime = 0;
            Enabled = true;
            Amounts = -1;
            currentTimes = 0;
            GameModel.instance.sonidos.addTemporalSound(this);
        }

        /// <summary>
        ///     Sonido a reproducir.
        ///     Se reproduce siempre sin loop
        /// </summary>
        public TGCStaticSound Sound { get; set; }

        /// <summary>
        ///     Frecuencia en segundos en la que se va a reproducir el sonido
        /// </summary>
        public float Frecuency { get; set; }

        /// <summary>
        ///     Cantidad total de veces que se debe reproducir el sonido.
        ///     Es una vez cada vez que se cumple la frecuencia.
        ///     En -1 indica que es infinita cantidad de veces.
        /// </summary>
        public int Amounts { get; set; }

        /// <summary>
        ///     Habilita la reproduccion
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Iniciar de nuevo
        /// </summary>
        public void reset()
        {
            acumTime = 0;
            currentTimes = 0;
        }

        /// <summary>
        ///     Actualizar estado y reproducir sonido si corresponde
        /// </summary>
        public void update(float elapsedTime)
        {
            if (!Enabled)
            {
                return;
            }

            acumTime += elapsedTime;
            if (acumTime > Frecuency)
            {
                acumTime = 0;

                //Cantidad ilimitada
                if (Amounts < 0)
                {
                    Sound.Play(false);
                }

                //Cantidad limitada
                else
                {
                    currentTimes++;
                    if (currentTimes < Amounts)
                    {
                        Sound.Play(false);
                    }
                    else
                    {
                        currentTimes = Amounts;
                    }
                }
            }
        }

        /// <summary>
        ///     Indica si ya termino la cantidad de reproducciones que debia hacer
        /// </summary>
        public bool isEnd()
        {
            return currentTimes == Amounts;
        }
    }
}
