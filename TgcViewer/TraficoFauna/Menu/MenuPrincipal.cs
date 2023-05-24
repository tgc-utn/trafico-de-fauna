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
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TraficoFauna.Examples;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using System.Windows.Forms;
using Examples.TraficoFauna.Interpolators;

namespace Examples.TraficoFauna.Menu
{
    /// <summary>
    /// Menu principal del juego
    /// </summary>
    public class MenuPrincipal
    {
        static readonly Vector2 ITEM_JUGAR_POSITION = new Vector2(390, 412 - 15);
        static readonly Vector2 ITEM_OPCIONES_POSITION = new Vector2(390, 469 - 15);
        static readonly Vector2 ITEM_SALIR_POSITION = new Vector2(390, 525 - 15);

        static readonly Vector2[] ITEMS_POSITION = new Vector2[]{
            ITEM_JUGAR_POSITION,
            ITEM_OPCIONES_POSITION,
            ITEM_SALIR_POSITION
        };

        Drawer2D drawer2D;
        GameSprite background;
        GameSprite backgroundControls;
        GameSprite selectedItem;
        int currentOption;
        bool gameStarted;
        bool loadingFistLevel;
        bool showingControls;
        Temporizador temporizadorLoading;

        GameSprite backgroundLoading;
        /// <summary>
        /// Background de Loading
        /// </summary>
        public GameSprite BackgroundLoading
        {
            get { return backgroundLoading; }
        }


        public MenuPrincipal()
        {
            drawer2D = new Drawer2D();
            gameStarted = false;
            loadingFistLevel = false;
            showingControls = false;

            int width = GuiController.Instance.Panel3d.Width;
            int height = GuiController.Instance.Panel3d.Height;
            string menuPath = Game.instance.gameConfig.menuPath;

            //Ocultar cursor del mouse
            Cursor.Hide();

            //background
            background = new GameSprite();
            Texture d3dTexture = TextureLoader.FromFile(
                GuiController.Instance.D3dDevice, menuPath + "menu_background.jpg", 1024, 600, 1, 
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            background.Texture = TgcTexture.createTexture(GuiController.Instance.D3dDevice, "menu_background", menuPath + "menu_background.jpg", d3dTexture);

            //background loading
            backgroundLoading = new GameSprite();
            Texture d3dTextureLoading = TextureLoader.FromFile(
                GuiController.Instance.D3dDevice, menuPath + "loading_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundLoading.Texture = TgcTexture.createTexture(GuiController.Instance.D3dDevice, "loading_background", menuPath + "loading_background.jpg", d3dTextureLoading);

            //background controls
            backgroundControls = new GameSprite();
            Texture d3dTextureControls = TextureLoader.FromFile(
                GuiController.Instance.D3dDevice, menuPath + "controls_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundControls.Texture = TgcTexture.createTexture(GuiController.Instance.D3dDevice, "controls_background", menuPath + "controls_background.jpg", d3dTextureControls);


            //selectedItem
            selectedItem = new GameSprite();
            selectedItem.Texture = TgcTexture.createTexture(GuiController.Instance.D3dDevice, menuPath + "menu_selected_item.png");
            currentOption = 0;
            selectedItem.Position = ITEMS_POSITION[currentOption];

            temporizadorLoading = new Temporizador();
            temporizadorLoading.setValues(0.5f);
        }

        /// <summary>
        /// Arrancar menu
        /// </summary>
        public void init()
        {
            Game.instance.sonidos.playMusic(Game.instance.sonidos.music_menu);
        }

        /// <summary>
        /// Dibujar menu
        /// </summary>
        public void render()
        {
            //Capturar input
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            //Cargando la primera vez
            if (loadingFistLevel)
            {
                if(temporizadorLoading.update())
                {
                    //Cargar nivel
                    loadingFistLevel = false;
                    gameStarted = true;
                    Game.instance.loadFirstLevel();
                }
                else
                {
                    //dibujar
                    d3dDevice.BeginScene();
                    drawer2D.DrawSpriteAtomic(backgroundLoading);
                    d3dDevice.EndScene();
                }
            }
            //Controles
            else if (showingControls)
            {
                //Volver al menu
                if (d3dInput.keyPressed(Key.Return) || d3dInput.keyPressed(Key.Space) || d3dInput.keyPressed(Key.Escape))
                {
                    showingControls = false;
                }
                //Dibujar
                else
                {
                    //dibujar
                    d3dDevice.BeginScene();
                    drawer2D.DrawSpriteAtomic(backgroundControls);
                    d3dDevice.EndScene();
                }
            }
            //Normal
            else
            {
                //Quitar almenu y volver al juego
                if (gameStarted && d3dInput.keyPressed(Key.Escape))
                {
                    returnToGame();
                }

                //Seleccionar opcion
                if (d3dInput.keyPressed(Key.Up))
                {
                    currentOption--;
                    if (currentOption < 0)
                    {
                        currentOption = 0;
                    }
                }
                else if (d3dInput.keyPressed(Key.Down))
                {
                    currentOption++;
                    currentOption = currentOption % ITEMS_POSITION.Length;
                }
                selectedItem.Position = ITEMS_POSITION[currentOption];

                //Seleccionar opcion
                if (d3dInput.keyPressed(Key.Return) || d3dInput.keyPressed(Key.Space))
                {
                    //Cargar primer nivel
                    if (currentOption == 0)
                    {
                        //Si el juego ya empezo, solo ocultar el menu y volver al juego
                        if (gameStarted)
                        {
                            returnToGame();
                        }
                        //Si no empezo, cargar primer nivel
                        else
                        {
                            showingControls = false;
                            loadingFistLevel = true;
                        }

                    }
                    //Opciones
                    else if (currentOption == 1)
                    {
                        showingControls = true;
                    }
                    //Salir
                    else if (currentOption == 2)
                    {
                        GuiController.Instance.exitGame();
                    }
                }



                //dibujar
                d3dDevice.BeginScene();
                drawer2D.BeginDrawSprite();

                drawer2D.DrawSprite(background);
                drawer2D.DrawSprite(selectedItem);

                drawer2D.EndDrawSprite();
                d3dDevice.EndScene();
            }
        }

        /// <summary>
        /// Continuar con el juego ya arrancado
        /// </summary>
        private void returnToGame()
        {
            showingControls = false;
            Game.instance.currentGameMode = Game.GameMode.Playing;
        }

    }
}
