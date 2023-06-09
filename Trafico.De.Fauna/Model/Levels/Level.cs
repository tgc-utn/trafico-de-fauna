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
using System.IO;
using Microsoft.DirectX;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TgcViewer.Model.ItemsAnimal;
using TgcViewer.Model.MainCharacters;
using TgcViewer.Model.Meshes;

namespace TgcViewer.Model.Levels
{
    /// <summary>
    /// Un nivel del juego
    /// </summary>
    public class Level
    {

        List<LevelSector> sectors;
        TgcSkyBox skyBox;

        int index;
        /// <summary>
        /// Numero de nivel
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        string name;
        /// <summary>
        /// Nombre del nivel
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        Dictionary<string, Conector> conectores;
        /// <summary>
        /// Conectores de todo el nivel
        /// </summary>
        public Dictionary<string, Conector> Conectores
        {
            get { return conectores; }
        }

        LevelSector currentSector;
        /// <summary>
        /// Sector actual
        /// </summary>
        public LevelSector CurrentSector
        {
            get { return currentSector; }
        }

        List<IAlphaObject> alphaObjects;
        /// <summary>
        /// Objetos a renderizar con AlphaBlending.
        /// Se dibujan luego de dibujar todos los opacos
        /// </summary>
        public List<IAlphaObject> AlphaObjects
        {
            get { return alphaObjects; }
        }

        int totalItemsAnimal;
        /// <summary>
        /// Cantidad de items animal de todo el nivel
        /// </summary>
        public int TotalItemsAnimal
        {
            get { return totalItemsAnimal; }
        }

        Llave llaveRoja;
        Llave llaveVerde;
        Llave llaveAmarilla;

        public Level()
        {
            sectors = new List<LevelSector>();
            conectores = new Dictionary<string, Conector>();
            alphaObjects = new List<IAlphaObject>();
        }

        /// <summary>
        /// Cargar nivel y todos sus sectores
        /// </summary>
        public void loadLevel(string levelName, int levelIndex)
        {
            this.name = levelName;
            this.index = levelIndex;
            string levelPath = GameModel.instance.gameConfig.levelsPath + levelName + "\\";

            //Load sectors
            string[] sectorsDir = Directory.GetDirectories(levelPath, "Sector-*", SearchOption.TopDirectoryOnly);
            foreach (string sectorDir in sectorsDir)
            {
                //TODO Texture path (en modo produccion salen todas las texturas de un solo lugar para cada nivel)
                string texturePath = sectorDir + "\\";
                string lightmapPath = texturePath;
                if (!GameModel.instance.debugMode.Enabled)
                {
                    texturePath = levelPath + "\\";
                }

                LevelSector sector = new LevelSector();
                DirectoryInfo dirInfo = new DirectoryInfo(sectorDir);
                string sectorName = dirInfo.Name;
                sector.loadScene(this, sectorName, sectorDir + "\\", texturePath, lightmapPath);
                sectors.Add(sector);
            }

            //Calcular cantidad de items animal de todo el nivel
            totalItemsAnimal = 0;
            foreach (LevelSector sector in sectors)
            {
                totalItemsAnimal += sector.ItemsAnimal.Count;
            }

            //Load skybox
            string skyboxTexturesPath = levelPath + "SkyBox\\";
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 0, 0);
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, skyboxTexturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, skyboxTexturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, skyboxTexturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, skyboxTexturesPath + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, skyboxTexturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, skyboxTexturesPath + "Front.jpg");
            //skyBox.updateValues();
            skyBox.Init();



            //TODO: Chequear relaciones entre conectores
        }

        /// <summary>
        /// Arrancar el nive en el primer sector
        /// </summary>
        public void initFirstSector(string firstSectorName, string firstConectorName)
        {
            currentSector = findSector(firstSectorName);
            Conector firstConector = currentSector.findSector(firstConectorName);
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;
            mainCharacter.resetForNewLevel();
            currentSector.initSector(mainCharacter, firstConector);

            //Para que todo termine de cargar
            //Thread.Sleep(1000);
        }

        /// <summary>
        /// Cambia de sector
        /// </summary>
        public void changeSector(Conector nextConector)
        {
            //Transicion de loading
            GameModel.instance.currentGameMode = GameModel.GameMode.LoadingSector;
            GameModel.instance.transiciones.initLoadingSector();

            //Cargar sector
            currentSector.hideSector();
            currentSector = nextConector.Sector;
            currentSector.initSector(GameModel.instance.mainCharacter, nextConector);
        }

        /// <summary>
        /// Actualizar estado del nivel
        /// </summary>
        public void update(float elapsedTime)
        {
            currentSector.update(elapsedTime);
        }

        /// <summary>
        /// Renderizar nivel
        /// </summary>
        public void render(float elapsedTime, TgcFrustum frustum)
        {
            //Render SkyBox
            skyBox.Render();

            //render sector
            currentSector.render(elapsedTime, frustum);

            //Renderizar al final los objetos acumulados con alpha
            foreach (IAlphaObject alphaObj in alphaObjects)
            {
                alphaObj.renderWithAlpha();
            }
        }

        private LevelSector findSector(string sectorName)
        {
            foreach (LevelSector sector in sectors)
            {
                if (sector.SectorName == sectorName)
                {
                    return sector;
                }
            }
            throw new Exception("Sector: " + sectorName + " not found");
        }

        /// <summary>
        /// Devuelve la llave del nivel segun el color pedido
        /// </summary>
        public Llave getLlave(Llave.ColorType colorType)
        {
            switch (colorType)
            {
                case Llave.ColorType.VERDE:
                    return this.llaveVerde;
                case Llave.ColorType.ROJA:
                    return this.llaveRoja;
                case Llave.ColorType.AMARILLA:
                    return this.llaveAmarilla;
            }
            return null;
        }

        /// <summary>
        /// Cargar una llave en el nivel
        /// </summary>
        public void setLlave(Llave llave)
        {
            Llave myLlave = getLlave(llave.Color);
            if (myLlave != null)
            {
                throw new Exception("Hay dos llaves del mismo color en el nivel: " + llave.ToString());
            }
            switch (llave.Color)
            {
                case Llave.ColorType.VERDE:
                    this.llaveVerde = llave;
                    break;
                case Llave.ColorType.ROJA:
                    this.llaveRoja = llave;
                    break;
                case Llave.ColorType.AMARILLA:
                    this.llaveAmarilla = llave;
                    break;
            }
        }


        /// <summary>
        /// Cuando el personaje pierde una vida, por diversos motivos.
        /// Se carga un efecto de perder.
        /// </summary>
        public void removeLifeToPlayer()
        {
            //Restar vida
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;
            mainCharacter.Vidas--;

            //Poner sonido de perder vida
            GameModel.instance.sonidos.stopAllSounds();
            GameModel.instance.sonidos.personaje_perder_vida.play();

            //Quitar efecto de modo alarma
            GameModel.instance.postProcessEffects.setAlarmModeEnable(false);

            //Cargar efecto de Game Over
            GameModel.instance.postProcessEffects.setPlayerDeadEffect();
            GameModel.instance.currentGameMode = GameModel.GameMode.PlayerDead;
        }

        /// <summary>
        /// Luego de que el jugador perdio, arrancar en el ultimo conector por el que paso.
        /// Si no le quedan mas vidas, pierde el juego y arranca desde el primer nivel.
        /// </summary>
        public void restoreFromLoseLife()
        {
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;
            
            //Arrancar en el ultimo conector por el que paso
            if (mainCharacter.Vidas > 0)
            {
                GameModel.instance.currentGameMode = GameModel.GameMode.Playing;
                changeSector(GameModel.instance.currentLevel.CurrentSector.CurrentConector);
            }
            //Game over
            else
            {
                //Ocultar sector actual
                currentSector.hideSector();

                //Mostrar pantalla de GameOver
                GameModel.instance.transiciones.initGameOver();
                GameModel.instance.currentGameMode = GameModel.GameMode.GameOver;
                

                //Arrancar desde el primer nivel del juego
                //Game.instance.loadFirstLevel();
            }
        }

        /// <summary>
        /// Pasar al siguiente nivel
        /// </summary>
        public void changeLevel()
        {
            currentSector.hideSector();
            GameConfig.Level levelData = GameModel.instance.gameConfig.getNextLevel(this.index);
            GameModel.instance.loadLevel(levelData);
        }

        /// <summary>
        /// Elimina todo contenido del nivel
        /// </summary>
        public void unloadLevel()
        {
            //Eliminar sectores
            foreach (LevelSector sector in sectors)
            {
                sector.unload();
            }
            sectors.Clear();
            sectors = null;
            currentSector = null;

            //Eliminar skybox
            skyBox.Dispose();
            skyBox = null;

            //Eliminar conectores
            foreach (KeyValuePair<string, Conector> item in conectores)
            {
                item.Value.dispose();
            }
            conectores.Clear();
            conectores = null;

            //Llaves
            llaveRoja.dispose();
            llaveVerde.dispose();
            llaveAmarilla.dispose();
            llaveRoja = null;
            llaveVerde = null;
            llaveAmarilla = null;

            //AlphaObjects
            alphaObjects.Clear();
            alphaObjects = null;
        }

    }
}
