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
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace Trafico.De.Fauna.Model.Menu
{
    /// <summary>
    ///     Utilidad para Sprites 2D animados en forma de tile dentro de una textura
    /// </summary>
    public class AnimatedSprite
    {
        private readonly float animationTimeLenght;

        private float currentTime;

        private Size frameSize;
        private readonly int framesPerColumn;
        private readonly int framesPerRow;

        private readonly float textureHeight;
        private readonly float textureWidth;
        private int totalFrames;

        /// <summary>
        ///     Crear un nuevo Sprite animado
        /// </summary>
        /// <param name="texturePath">path de la textura animada</param>
        /// <param name="frameSize">tamaño de un tile de la animacion</param>
        /// <param name="totalFrames">cantidad de frames que tiene la animacion</param>
        /// <param name="frameRate">velocidad en cuadros por segundo</param>
        public AnimatedSprite(string texturePath, Size frameSize, int totalFrames, float frameRate)
        {
            Enabled = true;
            CurrentFrame = 0;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            currentTime = 0;
            this.FrameRate = frameRate;
            Playing = true;

            //Crear textura
            var d3dDevice = D3DDevice.Instance.Device;
            var texture = TGCTexture.CreateTexture(d3dDevice, texturePath);

            //GameSprite
            Sprite = new GameSprite();
            Sprite.Texture = texture;

            //Calcular valores de frames de la textura
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            framesPerColumn = (int)textureWidth / frameSize.Width;
            framesPerRow = (int)textureHeight / frameSize.Height;
            var realTotalFrames = framesPerRow * framesPerColumn;
            if (realTotalFrames > totalFrames)
            {
                throw new Exception(
                    "Error en AnimatedSprite. No coinciden la cantidad de frames y el tamaño de la textura: " +
                    totalFrames);
            }

            animationTimeLenght = totalFrames / frameRate;
        }

        /// <summary>
        ///     Indica si el sprite esta habilitado para ser renderizada
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Arrancar o parar avance de animacion
        /// </summary>
        public bool Playing { get; set; }

        /// <summary>
        ///     Sprite con toda la textura a animar
        /// </summary>
        public GameSprite Sprite { get; }

        /// <summary>
        ///     Velocidad de la animacion medida en cuadros por segundo.
        /// </summary>
        public float FrameRate { get; }

        /// <summary>
        ///     Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame { get; set; }

        /// <summary>
        ///     Posicion del sprite
        /// </summary>
        public TGCVector2 Position
        {
            get => Sprite.Position;
            set => Sprite.Position = value;
        }

        /// <summary>
        ///     The scaling factors in the x and y axes.
        /// </summary>
        public TGCVector2 Scaling
        {
            get => Sprite.Scaling;
            set => Sprite.Scaling = value;
        }

        /// <summary>
        ///     The angle of rotation in radians.
        /// </summary>
        public float Rotation
        {
            get => Sprite.Rotation;
            set => Sprite.Rotation = value;
        }

        /// <summary>
        ///     Actualizar frame de animacion
        /// </summary>
        public void update(float elapsedTime)
        {
            //Avanzar tiempo
            if (Playing)
            {
                currentTime += elapsedTime;
                if (currentTime > animationTimeLenght)
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                }
            }

            //Obtener cuadro actual
            CurrentFrame = (int)(currentTime * FrameRate);

            //Obtener rectangulo de dibujado de la textura para este frame
            var srcRect = new Rectangle();
            srcRect.Y = frameSize.Width * (CurrentFrame % framesPerRow);
            srcRect.Width = frameSize.Width;
            srcRect.X = frameSize.Height * (CurrentFrame % framesPerColumn);
            srcRect.Height = frameSize.Height;

            Sprite.SrcRect = srcRect;
        }
    }
}
