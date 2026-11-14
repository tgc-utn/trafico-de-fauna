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
using System.Drawing;
using System.Text;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Menu;
using Font = System.Drawing.Font;

namespace Trafico.De.Fauna.Model.Dialogs
{
    /// <summary>
    ///     Herramienta para mostrar un cuadro de dialogo con varias paginas de texto que se
    ///     pueden ir pasando.
    /// </summary>
    public class TextDialog
    {
        private const int WORDS_PER_PAGE = 50;
        private readonly GameSprite background;
        private readonly GameSprite closeIcon;
        private int currentPage;

        private readonly Drawer2D drawer2D;
        private readonly GameSprite nextPageArrow;
        private readonly List<string> pages;
        private readonly TGCText2D text2d;


        public TextDialog(Device device)
        {
            pages = new List<string>();

            var width = device.Viewport.Width;
            var height = device.Viewport.Height;
            var dialogPath = GameModel.instance.gameConfig.menuPath + "Dialogs\\";

            //Fondo del cuadro de texto
            drawer2D = new Drawer2D(device);
            background = new GameSprite();
            background.Texture = TGCTexture.CreateTexture(device, dialogPath + "background.png");
            var textureWidth = background.Texture.Width;
            var textureHeight = background.Texture.Height;
            background.Position = new TGCVector2(width / 2 - textureWidth / 2, height / 2 - textureHeight / 2);

            //Cuadro de texto
            text2d = new TGCText2D();
            text2d.Color = Color.White;
            var font = GameFonts.Spatha(20);
            text2d.changeFont(font);
            text2d.Position = new Point((int)background.Position.X + 10, (int)background.Position.Y + 10);
            text2d.Align = TGCText2D.TextAlign.LEFT;
            text2d.Color = Color.FromArgb(252, 250, 114);
            text2d.Size = new Size(textureWidth - 20, textureHeight - 20);

            //Flecha de nextPage
            float arrowOffset = 12;
            nextPageArrow = new GameSprite();
            nextPageArrow.Texture = TGCTexture.CreateTexture(device, dialogPath + "nextPage.png");
            nextPageArrow.Position = new TGCVector2(
                background.Position.X + textureWidth - nextPageArrow.Texture.Width - arrowOffset,
                background.Position.Y + textureHeight - nextPageArrow.Texture.Height - arrowOffset);

            //Icono de cerrar
            closeIcon = new GameSprite();
            closeIcon.Texture = TGCTexture.CreateTexture(device, dialogPath + "close.png");
            closeIcon.Position = new TGCVector2(
                background.Position.X + textureWidth - nextPageArrow.Texture.Width - arrowOffset,
                background.Position.Y + textureHeight - nextPageArrow.Texture.Height - arrowOffset);
        }

        /// <summary>
        ///     Configurar un nuevo texto.
        ///     Se divide en paginas segun su longitud
        /// </summary>
        public void setText(string text)
        {
            pages.Clear();
            var words = text.Split(' ');
            var currentWords = 0;
            while (currentWords < words.Length)
            {
                var sb = new StringBuilder();
                var totalLeft = words.Length - currentWords >= WORDS_PER_PAGE
                    ? WORDS_PER_PAGE
                    : words.Length - currentWords;
                for (var i = 0; i < totalLeft; i++)
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
        ///     Pasar a la siguiente pagina
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
        ///     Vuelve a la pagina inicial
        /// </summary>
        public void resetPage()
        {
            currentPage = 0;
        }

        /// <summary>
        ///     Actualizar estado
        /// </summary>
        public bool update()
        {
            //Input para pasar de pagina de texto
            var input = GameModel.instance.Input;
            var keys = GameModel.instance.gameConfig.keys;
            var finished = false;
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
        ///     Dibujar
        /// </summary>
        public void render()
        {
            drawer2D.BeginDrawSprite();

            drawer2D.DrawSprite(background);
            drawer2D.DrawSprite(nextPageArrow);
            drawer2D.DrawSprite(closeIcon);

            drawer2D.EndDrawSprite();

            text2d.Render();
        }

        /// <summary>
        ///     Liberar recursos
        /// </summary>
        public void dispose()
        {
            background.Dispose();
            nextPageArrow.Dispose();
            closeIcon.Dispose();
        }
    }
}
