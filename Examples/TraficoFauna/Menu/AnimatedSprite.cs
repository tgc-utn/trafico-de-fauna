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
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;

namespace Examples.TraficoFauna.Menu
{
    /// <summary>
    /// Utilidad para Sprites 2D animados en forma de tile dentro de una textura
    /// </summary>
    public class AnimatedSprite
    {
        Size frameSize;
        int totalFrames;
        float currentTime;
        float animationTimeLenght;
        int framesPerRow;
        int framesPerColumn;
        float textureWidth;
        float textureHeight;

        private bool enabled;
        /// <summary>
        /// Indica si el sprite esta habilitado para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private bool playing;
        /// <summary>
        /// Arrancar o parar avance de animacion
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        GameSprite sprite;
        /// <summary>
        /// Sprite con toda la textura a animar
        /// </summary>
        public GameSprite Sprite
        {
            get { return sprite; }
        }

        private float frameRate;
        /// <summary>
        /// Velocidad de la animacion medida en cuadros por segundo.
        /// </summary>
        public float FrameRate
        {
            get { return frameRate; }
        }

        int currentFrame;
        /// <summary>
        /// Frame actual de la textura animada
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        /// <summary>
        /// Posicion del sprite
        /// </summary>
        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        /// <summary>
        /// The scaling factors in the x and y axes.
        /// </summary>
        public Vector2 Scaling
        {
            get { return sprite.Scaling; }
            set { sprite.Scaling = value; }
        }

        /// <summary>
        /// The angle of rotation in radians.
        /// </summary>
        public float Rotation
        {
            get { return sprite.Rotation; }
            set { sprite.Rotation = value; }
        }

        /// <summary>
        /// Crear un nuevo Sprite animado
        /// </summary>
        /// <param name="texturePath">path de la textura animada</param>
        /// <param name="frameSize">tamaño de un tile de la animacion</param>
        /// <param name="totalFrames">cantidad de frames que tiene la animacion</param>
        /// <param name="frameRate">velocidad en cuadros por segundo</param>
        public AnimatedSprite(string texturePath, Size frameSize, int totalFrames, float frameRate)
        {
            this.enabled = true;
            this.currentFrame = 0;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            this.currentTime = 0;
            this.frameRate = frameRate;
            this.playing = true;

            //Crear textura
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture texture = TgcTexture.createTexture(d3dDevice, texturePath);

            //GameSprite
            sprite = new GameSprite();
            sprite.Texture = texture;

            //Calcular valores de frames de la textura
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            framesPerColumn = (int)textureWidth / frameSize.Width;
            framesPerRow = (int)textureHeight / frameSize.Height;
            int realTotalFrames = framesPerRow * framesPerColumn;
            if (realTotalFrames > totalFrames)
            {
                throw new Exception("Error en AnimatedSprite. No coinciden la cantidad de frames y el tamaño de la textura: " + totalFrames);
            }
            animationTimeLenght = (float)totalFrames / frameRate;
        }

        /// <summary>
        /// Actualizar frame de animacion
        /// </summary>
        public void update()
        {
            //Avanzar tiempo
            if (playing)
            {
                currentTime += GuiController.Instance.ElapsedTime;
                if (currentTime > animationTimeLenght)
                {
                    //Reiniciar al llegar al final
                    currentTime = 0;
                }
            }

            //Obtener cuadro actual
            currentFrame = (int)(currentTime * frameRate);

            //Obtener rectangulo de dibujado de la textura para este frame
            Rectangle srcRect = new Rectangle();
            srcRect.Y = frameSize.Width * (currentFrame % framesPerRow);
            srcRect.Width = frameSize.Width;
            srcRect.X = frameSize.Height * (currentFrame % framesPerColumn);
            srcRect.Height = frameSize.Height;

            sprite.SrcRect = srcRect;
        }

    }
}
