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
using Examples.TraficoFauna.Interpolators;
using TraficoFauna.Examples;
using TgcViewer.Utils._2D;
using System.Drawing;
using Examples.TraficoFauna.Menu;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace Examples.TraficoFauna.GameModes
{
    /// <summary>
    /// Utilidad para manejar transiciones especiales del juego: loading de nivel, loading de sector, etc
    /// </summary>
    public class Transiciones
    {

        Temporizador temporizadorLoadingSector;
        Temporizador temporizadorGameOver;
        Temporizador temporizadorGameWin;
        Drawer2D drawer2D;
        GameSprite backgroundGameOver;
        GameSprite backgroundGameWin;
        

        public Transiciones()
        {
            drawer2D = new Drawer2D();
            temporizadorLoadingSector = new Temporizador();
            temporizadorGameOver = new Temporizador();
            temporizadorGameWin = new Temporizador();

            string menuPath = GameModel.instance.gameConfig.menuPath;

            //Background game over
            backgroundGameOver = new GameSprite();
            Texture d3dTextureGameOver = TextureLoader.FromFile(
                GuiController.Instance.D3dDevice, menuPath + "gameover_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundGameOver.Texture = TgcTexture.createTexture(GuiController.Instance.D3dDevice, "gameover_background", menuPath + "gameover_background.jpg", d3dTextureGameOver);

            //Background game win
            backgroundGameWin = new GameSprite();
            Texture d3dTextureGameWin = TextureLoader.FromFile(
                GuiController.Instance.D3dDevice, menuPath + "gamewin_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundGameWin.Texture = TgcTexture.createTexture(GuiController.Instance.D3dDevice, "gamewin_background", menuPath + "gamewin_background.jpg", d3dTextureGameWin);

        }

        public void initLoadingSector()
        {
            //temporizadorLoadingSector.setValues(0.2f);
            temporizadorLoadingSector.setValues(0);
        }

        public void renderLoadingSector()
        {
            if (temporizadorLoadingSector.update())
            {
                GameModel.instance.currentGameMode = GameModel.GameMode.Playing;
            }
            else
            {
                drawer2D.DrawSpriteAtomic(GameModel.instance.menuPrincipal.BackgroundLoading);
            }
        }


        public void initGameOver()
        {
            temporizadorGameOver.setValues(5f);
            GameModel.instance.sonidos.game_over.play();
        }

        public void renderGameOver()
        {
            if (temporizadorGameOver.update())
            {
                //Arrancar desde el primer nivel del juego
                GameModel.instance.loadFirstLevel();
            }
            else
            {
                drawer2D.DrawSpriteAtomic(backgroundGameOver);
            }
        }

        public void initGameWin()
        {
            temporizadorGameWin.setValues(10f);
            GameModel.instance.sonidos.game_win.play();
        }

        public void renderGameWin()
        {
            if (temporizadorGameWin.update())
            {
                //Arrancar desde el primer nivel del juego
                GameModel.instance.loadFirstLevel();
            }
            else
            {
                drawer2D.DrawSpriteAtomic(backgroundGameWin);
            }
        }

    }
}
