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
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TgcViewer.Model.Bananas;
using TgcViewer.Model.Camera;
using TgcViewer.Model.CustomEvents;
using TgcViewer.Model.Debug;
using TgcViewer.Model.Dialogs;
using TgcViewer.Model.Enemies;
using TgcViewer.Model.GameModes;
using TgcViewer.Model.Levels;
using TgcViewer.Model.MainCharacters;
using TgcViewer.Model.Menu;
using TgcViewer.Model.PostProcess;
using TgcViewer.Model.Sonidos;

namespace TgcViewer.Model
{
    /// <summary>
    /// Clase principal del juego
    /// </summary>
    public class GameModel : TgcExample
    {
        public static GameModel instance;
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

        /// <summary>
        /// Constructor del juego
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            //Device de DirectX para crear primitivas
            var d3dDevice = D3DDevice.Instance.Device;

            //BackgroundColor = Color.Black; this is the way in the last version.
            //BackgroundColor
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //Singleton trucho
            GameModel.instance = this;

            //Cargar gameConfig
            gameConfig = new GameConfig(MediaDir, "config.xml", ShadersDir);

            //Debug Mode
            debugMode = new DebugMode();
            debugMode.Enabled = gameConfig.debugModeEnabled;

            //Sonidos
            sonidos = new GameSounds(DirectSound);

            //Shaders
            shaders = new Shaders(d3dDevice);

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
            postProcessEffects = new PostProcessEffects(d3dDevice);
            transiciones = new Transiciones(d3dDevice);

            //HUD
            hud = new HUD(d3dDevice);

            //Menu
            menuPrincipal = new MenuPrincipal(d3dDevice);

            //CustomEventsManager
            customEventsManager = new CustomEventsManager();

            //Dialogs
            textDialog = new TextDialog(d3dDevice);
            simpleMessageDialog = new SimpleMessageDialog(d3dDevice);

            //Arrancar con el menu
            menuPrincipal.init();
            currentGameMode = GameMode.Menu;
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

        public override void Update()
        {
            PreUpdate();
            //TODO
            PostUpdate();
            ((GameThirdPersonCamera)this.Camara).updateViewMatrix(D3DDevice.Instance.Device);


        }

        /// <summary>
        /// Render principal del juego
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena
            PreRender();

            //Device de DirectX para crear primitivas
            var d3dDevice = D3DDevice.Instance.Device;
            d3dDevice.EndScene();

            switch (currentGameMode)
            {
                case GameMode.Menu:
                    doMenu(d3dDevice, Input);
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
                    doPlaying(d3dDevice, ElapsedTime);
                    break;
                case GameMode.PlayerDead:
                    doPlayerDead(ElapsedTime);
                    break;
                case GameMode.GameOver:
                    doGameOver(ElapsedTime);
                    break;
                case GameMode.GameWin:
                    doGameWin(ElapsedTime);
                    break;
                default:
                    break;
            }

            d3dDevice.BeginScene();
            //Finaliza el render
            PostRender();
        }

        private void doMenu(Device device, TgcD3dInput input)
        {
            menuPrincipal.render(device, input);
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
        private void doPlaying(Device device, float elapsedTime)
        {
            //Renderizar todo al RenderTarget secundario
            postProcessEffects.set2DRenderTarget();

            //update
            mainCharacter.update(elapsedTime);
            currentLevel.update(elapsedTime);
            hud.update(elapsedTime);

            //render
            currentLevel.AlphaObjects.Clear();
            mainCharacter.render(elapsedTime);
            currentLevel.render(elapsedTime, Frustum);
            hud.render(device);

            //Modo debug
            debugMode.render();

            postProcessEffects.renderAndRestoreTarget();

            //Si termino el juego y ya cambido de modo, esconder sector actual
            if (currentGameMode == GameMode.GameWin)
            {
               currentLevel.CurrentSector.hideSector();
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

        public override void Dispose()
        {
            //TODO: hacer dispose de todo

            this.currentLevel.unloadLevel();
            this.textDialog.dispose();
        }
    }
}