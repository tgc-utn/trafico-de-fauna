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

using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Interpolators;
using Device = Microsoft.DirectX.Direct3D.Device;

namespace Trafico.De.Fauna.Model.Menu
{
    /// <summary>
    ///     Menu principal del juego
    /// </summary>
    public class MenuPrincipal
    {
        private static readonly TGCVector2 ITEM_JUGAR_POSITION = new TGCVector2(390, 412 - 15);
        private static readonly TGCVector2 ITEM_OPCIONES_POSITION = new TGCVector2(390, 469 - 15);
        private static readonly TGCVector2 ITEM_SALIR_POSITION = new TGCVector2(390, 525 - 15);

        private static readonly TGCVector2[] ITEMS_POSITION =
        {
            ITEM_JUGAR_POSITION,
            ITEM_OPCIONES_POSITION,
            ITEM_SALIR_POSITION
        };

        private readonly GameSprite background;
        private readonly GameSprite backgroundControls;
        private readonly Drawer2D drawer2D;
        private readonly GameSprite selectedItem;
        private readonly Temporizador temporizadorLoading;
        private int currentOption;
        private bool gameStarted;
        private bool loadingFistLevel;
        private bool showingControls;

        public MenuPrincipal(Device d3dDevice)
        {
            drawer2D = new Drawer2D(d3dDevice);
            gameStarted = false;
            loadingFistLevel = false;
            showingControls = false;

            var width = d3dDevice.Viewport.Width;
            var height = d3dDevice.Viewport.Height;
            var menuPath = GameModel.instance.gameConfig.menuPath;

            //Ocultar cursor del mouse
            Cursor.Hide();

            //background
            background = new GameSprite();
            var d3dTexture = TextureLoader.FromFile(d3dDevice, menuPath + "menu_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            background.Texture = TGCTexture.CreateTexture(d3dDevice, "menu_background",
                menuPath + "menu_background.jpg", d3dTexture);

            //background loading
            BackgroundLoading = new GameSprite();
            var d3dTextureLoading = TextureLoader.FromFile(d3dDevice, menuPath + "loading_background.jpg", 1024, 600, 1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            BackgroundLoading.Texture = TGCTexture.CreateTexture(d3dDevice, "loading_background",
                menuPath + "loading_background.jpg", d3dTextureLoading);

            //background controls
            backgroundControls = new GameSprite();
            var d3dTextureControls = TextureLoader.FromFile(d3dDevice, menuPath + "controls_background.jpg", 1024, 600,
                1,
                Usage.None, Format.R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
            backgroundControls.Texture = TGCTexture.CreateTexture(d3dDevice, "controls_background",
                menuPath + "controls_background.jpg", d3dTextureControls);

            //selectedItem
            selectedItem = new GameSprite();
            selectedItem.Texture = TGCTexture.CreateTexture(d3dDevice, menuPath + "menu_selected_item.png");
            currentOption = 0;
            selectedItem.Position = ITEMS_POSITION[currentOption];

            temporizadorLoading = new Temporizador();
            temporizadorLoading.setValues(0.5f);
        }

        /// <summary>
        ///     Background de Loading
        /// </summary>
        public GameSprite BackgroundLoading { get; }

        /// <summary>
        ///     Arrancar menu
        /// </summary>
        public void init()
        {
            GameModel.instance.sonidos.playMusic(GameModel.instance.sonidos.music_menu);
        }

        /// <summary>
        ///     Dibujar menu
        /// </summary>
        public void render(Device device, TgcD3dInput input)
        {
            //Cargando la primera vez
            if (loadingFistLevel)
            {
                if (temporizadorLoading.update(GameModel.instance.ElapsedTime))
                {
                    //Cargar nivel
                    loadingFistLevel = false;
                    gameStarted = true;
                    GameModel.instance.LoadFirstLevel();
                }
                else
                {
                    //dibujar
                    device.BeginScene();
                    drawer2D.DrawSpriteAtomic(BackgroundLoading);
                    device.EndScene();
                }
            }
            //Controles
            else if (showingControls)
            {
                //Volver al menu
                if (input.keyPressed(Key.Return) || input.keyPressed(Key.Space) || input.keyPressed(Key.Escape))
                {
                    showingControls = false;
                }
                //Dibujar
                else
                {
                    //dibujar
                    device.BeginScene();
                    drawer2D.DrawSpriteAtomic(backgroundControls);
                    device.EndScene();
                }
            }
            //Normal
            else
            {
                //Quitar almenu y volver al juego
                if (gameStarted && input.keyPressed(Key.Escape))
                {
                    returnToGame();
                }

                //Seleccionar opcion
                if (input.keyPressed(Key.Up))
                {
                    currentOption--;
                    if (currentOption < 0)
                    {
                        currentOption = 0;
                    }
                }
                else if (input.keyPressed(Key.Down))
                {
                    currentOption++;
                    currentOption = currentOption % ITEMS_POSITION.Length;
                }

                selectedItem.Position = ITEMS_POSITION[currentOption];

                //Seleccionar opcion
                if (input.keyPressed(Key.Return) || input.keyPressed(Key.Space))
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
                        Application.Exit();
                        return;
                    }
                }

                //dibujar
                device.BeginScene();
                drawer2D.BeginDrawSprite();

                drawer2D.DrawSprite(background);
                drawer2D.DrawSprite(selectedItem);

                drawer2D.EndDrawSprite();
                device.EndScene();
            }
        }

        /// <summary>
        ///     Continuar con el juego ya arrancado
        /// </summary>
        private void returnToGame()
        {
            showingControls = false;
            GameModel.instance.currentGameMode = GameMode.Playing;
        }
    }
}
