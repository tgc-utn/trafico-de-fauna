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
using TgcViewer.Utils.Sound;
using TgcViewer;
using TraficoFauna.Examples;

namespace Examples.TraficoFauna.Sonidos
{
    /// <summary>
    /// Sonido sin loop que se ejecuta con una cierta frecuencia
    /// </summary>
    public class TemporalSound
    {
        float acumTime;
        int currentTimes;

        TgcStaticSound sound;
        /// <summary>
        /// Sonido a reproducir.
        /// Se reproduce siempre sin loop
        /// </summary>
        public TgcStaticSound Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        float frecuency;
        /// <summary>
        /// Frecuencia en segundos en la que se va a reproducir el sonido
        /// </summary>
        public float Frecuency
        {
            get { return frecuency; }
            set { frecuency = value; }
        }

        int amounts;
        /// <summary>
        /// Cantidad total de veces que se debe reproducir el sonido.
        /// Es una vez cada vez que se cumple la frecuencia.
        /// En -1 indica que es infinita cantidad de veces.
        /// </summary>
        public int Amounts
        {
            get { return amounts; }
            set { amounts = value; }
        }

        bool enabled;
        /// <summary>
        /// Habilita la reproduccion
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public TemporalSound()
        {
            frecuency = 1;
            acumTime = 0;
            enabled = true;
            amounts = -1;
            currentTimes = 0;
            Game.instance.sonidos.addTemporalSound(this);
        }

        /// <summary>
        /// Iniciar de nuevo
        /// </summary>
        public void reset()
        {
            acumTime = 0;
            currentTimes = 0;
        }

        /// <summary>
        /// Actualizar estado y reproducir sonido si corresponde
        /// </summary>
        public void update()
        {
            if (!enabled)
            {
                return;
            }

            acumTime += GuiController.Instance.ElapsedTime;
            if (acumTime > frecuency)
            {
                acumTime = 0;

                //Cantidad ilimitada
                if (amounts < 0)
                {
                    sound.play(false);
                }

                //Cantidad limitada
                else
                {
                    currentTimes++;
                    if (currentTimes < amounts)
                    {
                        sound.play(false);
                    }
                    else
                    {
                        currentTimes = amounts;
                    }
                }
            }
        }

        /// <summary>
        /// Indica si ya termino la cantidad de reproducciones que debia hacer
        /// </summary>
        public bool isEnd()
        {
            return currentTimes == amounts;
        }


    }
}
