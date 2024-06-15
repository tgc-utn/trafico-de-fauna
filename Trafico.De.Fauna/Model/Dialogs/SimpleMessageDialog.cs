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

using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Menu;
using SystemFont = System.Drawing.Font;

namespace Trafico.De.Fauna.Model.Dialogs
{
    /// <summary>
    ///     Herramienta para mostrar un cuadro de texto simple, con poco texto y sin opciones.
    /// </summary>
    public class SimpleMessageDialog
    {
        private readonly GameSprite background;
        private readonly Drawer2D drawer2D;
        private readonly TGCText2D text2d;

        public SimpleMessageDialog(Device device)
        {
            var width = device.Viewport.Width;
            var height = device.Viewport.Height;
            var dialogPath = GameModel.instance.gameConfig.menuPath + "Dialogs\\";

            //Fondo del cuadro de texto
            drawer2D = new Drawer2D(device);
            background = new GameSprite();
            background.Texture = TGCTexture.CreateTexture(device, dialogPath + "backgroundSimpleMessage.png");
            var textureWidth = background.Texture.Width;
            var textureHeight = background.Texture.Height;
            background.Position = new TGCVector2(width / 2 - textureWidth / 2, height / 2 - textureHeight / 2);

            //Cuadro de texto
            text2d = new TGCText2D();
            text2d.Color = Color.White;
            var font = new SystemFont("JungleFever", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            text2d.changeFont(font);
            text2d.Position = new Point((int)background.Position.X + 10, (int)background.Position.Y + 16);
            text2d.Align = TGCText2D.TextAlign.CENTER;
            text2d.Color = Color.FromArgb(252, 250, 114);
            text2d.Size = new Size(textureWidth - 20, textureHeight - 20);
        }

        /// <summary>
        ///     Configurar un nuevo texto.
        ///     Se divide en paginas segun su longitud
        /// </summary>
        public void setText(string text)
        {
            text2d.Text = text;
        }

        /// <summary>
        ///     Dibujar
        /// </summary>
        public void render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(background);
            drawer2D.EndDrawSprite();

            text2d.Render();
        }

        /// <summary>
        ///     Liberar recursos
        /// </summary>
        public void dispose()
        {
            background.Dispose();
        }
    }
}
