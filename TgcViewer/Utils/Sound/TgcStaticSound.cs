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
using Microsoft.DirectX.DirectSound;

namespace TgcViewer.Utils.Sound
{
    /// <summary>
    /// Herramienta para reproducir un sonido WAV estático
    /// </summary>
    public class TgcStaticSound
    {
        private SecondaryBuffer soundBuffer;
        /// <summary>
        /// Buffer con la información del sonido cargado
        /// </summary>
        public SecondaryBuffer SoundBuffer
        {
            get { return soundBuffer; }
        }


        public TgcStaticSound()
        {
        }

        /// <summary>
        /// Carga un archivo WAV de audio, indicando el volumen del mismo
        /// </summary>
        /// <param name="soundPath">Path del archivo WAV</param>
        /// <param name="volume">Volumen del mismo</param>
        public void loadSound(string soundPath, int volume)
        {
            try
            {
                dispose();

                BufferDescription bufferDescription = new BufferDescription();
                if (volume != -1)
                {
                    bufferDescription.ControlVolume = true;
                }

                soundBuffer = new SecondaryBuffer(soundPath, bufferDescription, GuiController.Instance.DirectSound.DsDevice);

                if (volume != -1)
                {
                    soundBuffer.Volume = volume;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar sonido estático WAV: " + soundPath, ex);
            }
        }

        /// <summary>
        /// Carga un archivo WAV de audio, con el volumen default del mismo
        /// </summary>
        /// <param name="soundPath">Path del archivo WAV</param>
        public void loadSound(string soundPath)
        {
            loadSound(soundPath, -1);
        }

        /// <summary>
        /// Reproduce el sonido, indicando si se hace con Loop.
        /// Si ya se está reproduciedo, no vuelve a empezar.
        /// </summary>
        /// <param name="playLoop">TRUE para reproducir en loop</param>
        public void play(bool playLoop)
        {
            soundBuffer.Play(0, playLoop ? BufferPlayFlags.Looping : BufferPlayFlags.Default);
        }

        /// <summary>
        /// Reproduce el sonido, sin Loop.
        /// Si ya se está reproduciedo, no vuelve a empezar.
        /// </summary>
        public void play()
        {
            play(false);
        }

        /// <summary>
        /// Pausa la ejecución del sonido.
        /// Si el sonido no se estaba ejecutando, no hace nada.
        /// Si se hace stop() y luego play(), el sonido continua desde donde había dejado la última vez.
        /// </summary>
        public void stop()
        {
            soundBuffer.Stop();
        }

        /// <summary>
        /// Liberar recursos del sonido
        /// </summary>
        public void dispose()
        {
            if (soundBuffer != null && !soundBuffer.Disposed)
            {
                soundBuffer.Dispose();
                soundBuffer = null;
            }
        }

    }
}
