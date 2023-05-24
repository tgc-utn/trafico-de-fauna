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
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TraficoFauna.SphereCollision;
using Examples.TraficoFauna;
using Examples.TraficoFauna.Enemies;
using Examples.TraficoFauna.Debug;
using Examples.TraficoFauna.PostProcess;
using TgcViewer.Utils;
using Examples.TraficoFauna.GameModes;
using Examples.TraficoFauna.Menu;
using Examples.TraficoFauna.CustomEvents;
using Examples.TraficoFauna.Dialogs;
using Examples.TraficoFauna.Bananas;
using System.Windows.Forms;
using Examples.TraficoFauna.Sonidos;

namespace TraficoFauna.Examples
{
    /// <summary>
    /// Clase principal del juego
    /// </summary>
    public class Game : TgcExample
    {

        public static Game instance;
        public GameConfig gameConfig;
        public Level currentLevel;
        public MainCharacter mainCharacter;
        public GuardiasPool guardiasPool;
        public DebugMode debugMode;
        public PostProcessEffects postProcessEffects;
        public HUD hud;
        public GameSounds sonidos;
        public Shaders shaders;
        public GameMode currentGameMode;
        public Transiciones transiciones;
        public MenuPrincipal menuPrincipal;
        public CustomEventsManager customEventsManager;
        public TextDialog textDialog;
        public SimpleMessageDialog simpleMessageDialog;
        public BananasPool bananasPool;
        public CajasBananasPool cajasBananasPool;
        public ErrorWindow errorWindow;



        /// <summary>
        /// Modos en los que se encuentra el juego
        /// </summary>
        public enum GameMode
        {
            Playing,
            Menu,
            Intro,
            PlayerDead,
            LoadingLevel,
            LoadingSector,
            GameOver,
            GameWin,
        }


        public override string getCategory()
        {
            return "TraficoFauna";
        }

        public override string getName()
        {
            return "TraficoFauna";
        }

        public override string getDescription()
        {
            return "TraficoFauna";
        }

        public override void init()
        {
            try
            {
                Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
                GuiController.Instance.BackgroundColor = Color.Black;

                //Singleton trucho
                Game.instance = this;

                //Error Window
                errorWindow = new ErrorWindow();

                //Cargar gameConfig
                string basePath = GuiController.Instance.ExamplesDir + "..\\..\\..\\TraficoFauna\\Media\\";
                gameConfig = new GameConfig(basePath, "config.xml");

                //Debug Mode
                debugMode = new DebugMode();
                debugMode.Enabled = gameConfig.debugModeEnabled;

                //Sonidos
                sonidos = new GameSounds();

                //Shaders
                shaders = new Shaders();

                //Cargar personaje principal
                mainCharacter = new MainCharacter();
                mainCharacter.loadModel();

                //Cargar pool de enemigos guardias
                guardiasPool = new GuardiasPool();
                guardiasPool.loadModels();

                //Cargar pool de bananas y cajas de bananas
                bananasPool = new BananasPool();
                bananasPool.loadModels();
                cajasBananasPool = new CajasBananasPool();
                cajasBananasPool.loadModels();

                //Efectos 2D de PostProcesado y transiciones
                postProcessEffects = new PostProcessEffects();
                transiciones = new Transiciones();

                //HUD
                hud = new HUD();

                //Menu
                menuPrincipal = new MenuPrincipal();

                //CustomEventsManager
                customEventsManager = new CustomEventsManager();

                //Dialogs
                textDialog = new TextDialog();
                simpleMessageDialog = new SimpleMessageDialog();


                //Arrancar con el menu
                menuPrincipal.init();
                currentGameMode = GameMode.Menu;
            }
            catch (Exception e)
            {
                errorWindow.showError(e.Message, e);
                //throw e;
                GuiController.Instance.exitGame();
            }
            
        }

        /// <summary>
        /// Cargar primer nivel de todo el juego
        /// </summary>
        public void loadFirstLevel()
        {
            mainCharacter.resetForFirstLevel();
            sonidos.stopAllSounds();
            GameConfig.Level levelData = gameConfig.getFirstLevel();
            loadLevel(levelData);
        }

        /// <summary>
        /// Iniciar la carga de un nuevo nivel
        /// </summary>
        public void loadLevel(GameConfig.Level levelData)
        {
            //Quitar nivel anterior
            if (currentLevel != null)
            {
                currentLevel.unloadLevel();
                currentLevel = null;

                //Limpiar memoria
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
    

            //Cargar nuevo nivel
            Level level = new Level();
            this.currentLevel = level;
            this.currentGameMode = GameMode.LoadingLevel;
            this.currentLevel.loadLevel(levelData.name, levelData.index);

            System.GC.Collect();

            //Cargar primer sector
            this.currentGameMode = GameMode.LoadingSector;
            currentLevel.initFirstSector(levelData.sectorInicial, levelData.conectorInicial);
            this.currentGameMode = GameMode.Playing;
        }




        

        /// <summary>
        /// Render principal del juego
        /// </summary>
        public override void render(float elapsedTime)
        {
            try
            {
                switch (currentGameMode)
                {
                    case GameMode.Menu:
                        doMenu();
                        break;
                    case GameMode.Intro:
                        doIntro();
                        break;
                    case GameMode.LoadingLevel:
                        doLoadingLevel();
                        break;
                    case GameMode.LoadingSector:
                        doLoadingSector();
                        break;
                    case GameMode.Playing:
                        doPlaying(elapsedTime);
                        break;
                    case GameMode.PlayerDead:
                        doPlayerDead(elapsedTime);
                        break;
                    case GameMode.GameOver:
                        doGameOver(elapsedTime);
                        break;
                    case GameMode.GameWin:
                        doGameWin(elapsedTime);
                        break;
                    default:
                        break;
                } 
            }
            catch (Exception e)
            {
                errorWindow.showError(e.Message, e);
                //throw e;
                GuiController.Instance.exitGame();
            }
        }

        private void doMenu()
        {
            menuPrincipal.render();
        }

        private void doIntro()
        {

        }

        private void doLoadingLevel()
        {
            
        }

        private void doLoadingSector()
        {
            transiciones.renderLoadingSector();
        }


        /// <summary>
        /// Estado normal, en el que el jugador esta jugando (99% del tiempo)
        /// </summary>
        private void doPlaying(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Renderizar todo al RenderTarget secundario
            postProcessEffects.set2DRenderTarget();

            //update
            mainCharacter.update();
            currentLevel.update();
            hud.update();

            //render
            currentLevel.AlphaObjects.Clear();
            mainCharacter.render();
            currentLevel.render();
            hud.render();


            //Modo debug
            debugMode.render();
            

            postProcessEffects.renderAndRestoreTarget();

            
            //Si termino el juego y ya cambido de modo, esconder sector actual
            if (currentGameMode == GameMode.GameWin)
            {
                Game.instance.currentLevel.CurrentSector.hideSector();
            }
        }


        /// <summary>
        /// Estado cuando el jugador fue atrapado y pierde
        /// </summary>
        private void doPlayerDead(float elapsedTime)
        {
            postProcessEffects.renderOldFrame();
        }

        /// <summary>
        /// Estado cuando el jugador perdio todas sus vidas
        /// </summary>
        private void doGameOver(float elapsedTime)
        {
            transiciones.renderGameOver();
        }

        /// <summary>
        /// Estado cuando el jugador gano todo el juego
        /// </summary>
        private void doGameWin(float elapsedTime)
        {
            transiciones.renderGameWin();
        }


        public override void close()
        {
            //TODO: hacer dispose de todo

            this.currentLevel.unloadLevel();
            this.textDialog.dispose();
        }

    }
}
