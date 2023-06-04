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
using Examples.TraficoFauna.Menu;
using TraficoFauna.Examples;
using Microsoft.DirectX;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using System.Drawing.Text;
using TGC.Core.Textures;
using TGC.Core.Text;

namespace Examples.TraficoFauna.Dialogs
{
    /// <summary>
    /// Herramienta para mostrar un cuadro de texto simple, con poco texto y sin opciones.
    /// </summary>
    public class SimpleMessageDialog
    {
        Drawer2D drawer2D;
        GameSprite background;
        TgcText2D text2d;

        public SimpleMessageDialog(Device device)
        {
            int width = device.Viewport.Width;
            int height = device.Viewport.Height;
            string dialogPath = GameModel.instance.gameConfig.menuPath + "Dialogs\\";

            //Fondo del cuadro de texto
            drawer2D = new Drawer2D(device);
            background = new GameSprite();
            background.Texture = TgcTexture.createTexture(device, dialogPath + "backgroundSimpleMessage.png");
            int textureWidth = background.Texture.Width;
            int textureHeight = background.Texture.Height;
            background.Position = new Vector2(width / 2 - textureWidth / 2, height / 2 - textureHeight / 2);

            //Cuadro de texto
            text2d = new TgcText2D();
            text2d.Color = Color.White;
            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile(GameModel.instance.gameConfig.basePath + "Fonts\\JUNGLEFE.TTF");
            FontFamily[] families = privateFontCollection.Families;
            System.Drawing.Font font = new System.Drawing.Font(families[0], 20, FontStyle.Regular, GraphicsUnit.Pixel);
            text2d.changeFont(font);
            
            text2d.Position = new Point((int)background.Position.X + 10, (int)background.Position.Y + 16);
            text2d.Align = TgcText2D.TextAlign.CENTER;
            text2d.Color = Color.FromArgb(252, 250, 114);
            text2d.Size = new Size(textureWidth - 20, textureHeight - 20);
        }

        /// <summary>
        /// Configurar un nuevo texto.
        /// Se divide en paginas segun su longitud
        /// </summary>
        public void setText(string text)
        {
            text2d.Text = text;
        }

        /// <summary>
        /// Dibujar
        /// </summary>
        public void render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(background);
            drawer2D.EndDrawSprite();

            text2d.render();
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            background.dispose();
        }

    }
}
