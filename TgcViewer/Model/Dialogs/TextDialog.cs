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
using System.Collections.Generic;
using System.Text;
using Examples.TraficoFauna.Menu;
using TraficoFauna.Examples;
using Microsoft.DirectX;
using System.Drawing;
using System.Drawing.Text;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.Core.Text;

namespace Examples.TraficoFauna.Dialogs
{
    /// <summary>
    /// Herramienta para mostrar un cuadro de dialogo con varias paginas de texto que se
    /// pueden ir pasando.
    /// </summary>
    public class TextDialog
    {
        const int WORDS_PER_PAGE = 50;

        Drawer2D drawer2D;
        GameSprite background;
        TgcText2D text2d;
        GameSprite nextPageArrow;
        GameSprite closeIcon;
        List<string> pages;
        int currentPage;


        public TextDialog(Microsoft.DirectX.Direct3D.Device device)
        {
            pages = new List<string>();

            int width = device.Viewport.Width;
            int height = device.Viewport.Height;
            string dialogPath = GameModel.instance.gameConfig.menuPath + "Dialogs\\";

            //Fondo del cuadro de texto
            drawer2D = new Drawer2D(device);
            background = new GameSprite();
            background.Texture = TgcTexture.createTexture(device, dialogPath + "background.png");
            int textureWidth = background.Texture.Width;
            int textureHeight = background.Texture.Height;
            background.Position = new Vector2(width / 2 - textureWidth / 2, height / 2 - textureHeight / 2);

            //Cuadro de texto
            text2d = new TgcText2D();
            text2d.Color = Color.White;
            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile(GameModel.instance.gameConfig.basePath + "Fonts\\spathasans.ttf");
            FontFamily[] families = privateFontCollection.Families;
            System.Drawing.Font font = new System.Drawing.Font(families[0], 20, FontStyle.Regular, GraphicsUnit.Pixel);
            text2d.changeFont(font);
            text2d.Position = new Point((int)background.Position.X + 10, (int)background.Position.Y + 10);
            text2d.Align = TgcText2D.TextAlign.LEFT;
            text2d.Color = Color.FromArgb(252, 250, 114);
            text2d.Size = new Size(textureWidth - 20, textureHeight - 20);
            
            //Flecha de nextPage
            float arrowOffset = 12;
            nextPageArrow = new GameSprite();
            nextPageArrow.Texture = TgcTexture.createTexture(device, dialogPath + "nextPage.png");
            nextPageArrow.Position = new Vector2(
                background.Position.X + textureWidth - nextPageArrow.Texture.Width - arrowOffset,
                background.Position.Y + textureHeight - nextPageArrow.Texture.Height - arrowOffset);

            //Icono de cerrar
            closeIcon = new GameSprite();
            closeIcon.Texture = TgcTexture.createTexture(device, dialogPath + "close.png");
            closeIcon.Position = new Vector2(
                background.Position.X + textureWidth - nextPageArrow.Texture.Width - arrowOffset,
                background.Position.Y + textureHeight - nextPageArrow.Texture.Height - arrowOffset);

        }

        /// <summary>
        /// Configurar un nuevo texto.
        /// Se divide en paginas segun su longitud
        /// </summary>
        public void setText(string text)
        {
            pages.Clear();
            string[] words = text.Split(' ');
            int currentWords = 0;
            while (currentWords < words.Length)
            {
                StringBuilder sb = new StringBuilder();
                int totalLeft = words.Length - currentWords >= WORDS_PER_PAGE ? WORDS_PER_PAGE : words.Length - currentWords;
                for (int i = 0; i < totalLeft; i++)
                {
                    sb.Append(words[currentWords + i]);
                    sb.Append(" ");
                }
                pages.Add(sb.ToString());
                currentWords += WORDS_PER_PAGE;
            }


            currentPage = 0;
            text2d.Text = pages[currentPage];
        }

        /// <summary>
        /// Pasar a la siguiente pagina
        /// </summary>
        public bool nextPage()
        {
            currentPage++;
            if (currentPage >= pages.Count)
            {
                currentPage--;
                return false;
            }
            text2d.Text = pages[currentPage];
            return true;
        }

        /// <summary>
        /// Vuelve a la pagina inicial
        /// </summary>
        public void resetPage()
        {
            currentPage = 0;
        }

        /// <summary>
        /// Actualizar estado
        /// </summary>
        public bool update()
        {
            //Input para pasar de pagina de texto
            TgcD3dInput input = GameModel.instance.Input;
            GameConfig.Keys keys = GameModel.instance.gameConfig.keys;
            bool finished = false;
            if (input.keyPressed(keys.enter) || input.keyPressed(keys.action))
            {
                finished = !nextPage();
            }

            //Activar o desactivar flechas
            nextPageArrow.Enabled = currentPage < pages.Count - 1 ? true : false;
            closeIcon.Enabled = currentPage == pages.Count - 1 ? true : false;

            return finished;
        }

        /// <summary>
        /// Dibujar
        /// </summary>
        public void render()
        {
            drawer2D.BeginDrawSprite();

            drawer2D.DrawSprite(background);
            drawer2D.DrawSprite(nextPageArrow);
            drawer2D.DrawSprite(closeIcon);

            drawer2D.EndDrawSprite();

            text2d.render();
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            background.dispose();
            nextPageArrow.dispose();
            closeIcon.dispose();
        }

    }
}
