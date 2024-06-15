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
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using Trafico.De.Fauna.Model.Bananas;
using Trafico.De.Fauna.Model.Camera;
using Trafico.De.Fauna.Model.CustomEvents;
using Trafico.De.Fauna.Model.DebugTools;
using Trafico.De.Fauna.Model.Dialogs;
using Trafico.De.Fauna.Model.Enemies;
using Trafico.De.Fauna.Model.GameModes;
using Trafico.De.Fauna.Model.Levels;
using Trafico.De.Fauna.Model.MainCharacters;
using Trafico.De.Fauna.Model.Menu;
using Trafico.De.Fauna.Model.PostProcess;
using Trafico.De.Fauna.Model.Sonidos;

namespace Trafico.De.Fauna.Model
{
    /// <summary>
    ///     Clase principal del juego
    /// </summary>
    public class GameModel : TGCExample
    {
        public static GameModel instance;
        public BananasPool bananasPool;
        public CajasBananasPool cajasBananasPool;
        public GameMode currentGameMode;
        public Level currentLevel;
        public CustomEventsManager customEventsManager;
        public DebugMode debugMode;
        public GameConfig gameConfig;
        public GuardiasPool guardiasPool;
        public HUD hud;
        public MainCharacter mainCharacter;
        public MenuPrincipal menuPrincipal;
        public PostProcessEffects postProcessEffects;
        public Shaders shaders;
        public SimpleMessageDialog simpleMessageDialog;
        public GameSounds sonidos;
        public TextDialog textDialog;
        public Transiciones transiciones;

        /// <summary>
        ///     Constructor del juego
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

            //Utilizando esta propiedad puedo activar el update/Render a intervalos constantes.
            FixedTickEnable = true;

            //BackgroundColor
            BackgroundColor = Color.Black;

            //Singleton trucho
            instance = this;

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
        ///     Cargar primer nivel de todo el juego
        /// </summary>
        public void LoadFirstLevel()
        {
            mainCharacter.resetForFirstLevel();
            sonidos.stopAllSounds();
            var levelData = gameConfig.getFirstLevel();
            LoadLevel(levelData);
        }

        /// <summary>
        ///     Iniciar la carga de un nuevo nivel
        /// </summary>
        public void LoadLevel(GameConfig.Level levelData)
        {
            //Quitar nivel anterior
            if (currentLevel != null)
            {
                currentLevel.unloadLevel();
                currentLevel = null;

                //Limpiar memoria
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            //Cargar nuevo nivel
            var level = new Level();
            currentLevel = level;
            currentGameMode = GameMode.LoadingLevel;
            currentLevel.loadLevel(levelData.name, levelData.index);

            GC.Collect();

            //Cargar primer sector
            currentGameMode = GameMode.LoadingSector;
            currentLevel.initFirstSector(levelData.sectorInicial, levelData.conectorInicial);
            currentGameMode = GameMode.Playing;
        }

        public override void Update()
        {
            debugMode.Update(Input, this);
        }

        /// <summary>
        ///     Render principal del juego
        /// </summary>
        public override void Render()
        {
            // TODO remove this from the Render method.
            ((GameThirdPersonCamera)Camera).updateViewMatrix(D3DDevice.Instance.Device);

            //Inicio el Render de la escena
            PreRender();

            //Device de DirectX para crear primitivas
            var d3dDevice = D3DDevice.Instance.Device;
            d3dDevice.EndScene();

            switch (currentGameMode)
            {
                case GameMode.Menu:
                    DoMenu(d3dDevice, Input);
                    break;
                case GameMode.Intro:
                    DoIntro();
                    break;
                case GameMode.LoadingLevel:
                    DoLoadingLevel();
                    break;
                case GameMode.LoadingSector:
                    DoLoadingSector();
                    break;
                case GameMode.Playing:
                    DoPlaying(d3dDevice, ElapsedTime);
                    break;
                case GameMode.PlayerDead:
                    DoPlayerDead(ElapsedTime);
                    break;
                case GameMode.GameOver:
                    DoGameOver(ElapsedTime);
                    break;
                case GameMode.GameWin:
                    DoGameWin(ElapsedTime);
                    break;
            }

            BeginScene();

            //Finaliza el Render
            PostRender();
        }

        private void DoMenu(Device device, TgcD3dInput input)
        {
            menuPrincipal.render(device, input);
        }

        private void DoIntro()
        {
        }

        private void DoLoadingLevel()
        {
        }

        private void DoLoadingSector()
        {
            transiciones.renderLoadingSector();
        }

        /// <summary>
        ///     Estado normal, en el que el jugador esta jugando (99% del tiempo)
        /// </summary>
        private void DoPlaying(Device device, float elapsedTime)
        {
            //Renderizar todo al RenderTarget secundario
            postProcessEffects.set2DRenderTarget();

            //update
            mainCharacter.update(elapsedTime);
            currentLevel.update(elapsedTime);
            hud.update(elapsedTime);

            //Render
            currentLevel.AlphaObjects.Clear();
            mainCharacter.render(elapsedTime);
            currentLevel.render(elapsedTime, Frustum);
            hud.render(device);

            //Modo debug
            debugMode.Render();

            postProcessEffects.renderAndRestoreTarget();

            //Si termino el juego y ya cambido de modo, esconder sector actual
            if (currentGameMode == GameMode.GameWin)
            {
                currentLevel.CurrentSector.hideSector();
            }
        }

        /// <summary>
        ///     Estado cuando el jugador fue atrapado y pierde
        /// </summary>
        private void DoPlayerDead(float elapsedTime)
        {
            postProcessEffects.renderOldFrame();
        }

        /// <summary>
        ///     Estado cuando el jugador perdio todas sus vidas
        /// </summary>
        private void DoGameOver(float elapsedTime)
        {
            transiciones.renderGameOver();
        }

        /// <summary>
        ///     Estado cuando el jugador gano todo el juego
        /// </summary>
        private void DoGameWin(float elapsedTime)
        {
            transiciones.renderGameWin();
        }

        public override void Dispose()
        {
            //TODO: hacer Dispose de todo

            currentLevel?.unloadLevel();
            textDialog.dispose();
        }
    }
}
