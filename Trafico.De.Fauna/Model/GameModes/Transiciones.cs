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

using Microsoft.DirectX.Direct3D;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Interpolators;
using Trafico.De.Fauna.Model.Menu;

namespace Trafico.De.Fauna.Model.GameModes
{
    /// <summary>
    ///     Utilidad para manejar transiciones especiales del juego: loading de nivel, loading de sector, etc
    /// </summary>
    public class Transiciones
    {
        private readonly GameSprite backgroundGameOver;
        private readonly GameSprite backgroundGameWin;
        private readonly Drawer2D drawer2D;
        private readonly Temporizador temporizadorGameOver;
        private readonly Temporizador temporizadorGameWin;
        private readonly Temporizador temporizadorLoadingSector;

        public Transiciones(Device device)
        {
            drawer2D = new Drawer2D(device);
            temporizadorLoadingSector = new Temporizador();
            temporizadorGameOver = new Temporizador();
            temporizadorGameWin = new Temporizador();

            var menuPath = GameModel.instance.gameConfig.menuPath;

            //Background game over
            backgroundGameOver = new GameSprite();
            var d3dTextureGameOver = TextureLoader.FromFile(device, menuPath + "gameover_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundGameOver.Texture = TGCTexture.CreateTexture(device, "gameover_background",
                menuPath + "gameover_background.jpg", d3dTextureGameOver);

            //Background game win
            backgroundGameWin = new GameSprite();
            var d3dTextureGameWin = TextureLoader.FromFile(device, menuPath + "gamewin_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundGameWin.Texture = TGCTexture.CreateTexture(device, "gamewin_background",
                menuPath + "gamewin_background.jpg", d3dTextureGameWin);
        }

        public void initLoadingSector()
        {
            //temporizadorLoadingSector.setValues(0.2f);
            temporizadorLoadingSector.setValues(0);
        }

        public void renderLoadingSector()
        {
            if (temporizadorLoadingSector.update(GameModel.instance.ElapsedTime))
            {
                GameModel.instance.currentGameMode = GameMode.Playing;
            }
            else
            {
                drawer2D.DrawSpriteAtomic(GameModel.instance.menuPrincipal.BackgroundLoading);
            }
        }

        public void initGameOver()
        {
            temporizadorGameOver.setValues(5f);
            GameModel.instance.sonidos.game_over.Play();
        }

        public void renderGameOver()
        {
            if (temporizadorGameOver.update(GameModel.instance.ElapsedTime))
            {
                //Arrancar desde el primer nivel del juego
                GameModel.instance.LoadFirstLevel();
            }
            else
            {
                drawer2D.DrawSpriteAtomic(backgroundGameOver);
            }
        }

        public void initGameWin()
        {
            temporizadorGameWin.setValues(10f);
            GameModel.instance.sonidos.game_win.Play();
        }

        public void renderGameWin()
        {
            if (temporizadorGameWin.update(GameModel.instance.ElapsedTime))
            {
                //Arrancar desde el primer nivel del juego
                GameModel.instance.LoadFirstLevel();
            }
            else
            {
                drawer2D.DrawSpriteAtomic(backgroundGameWin);
            }
        }
    }
}
