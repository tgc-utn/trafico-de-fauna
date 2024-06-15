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
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Levels
{
    /// <summary>
    ///     Un nivel del juego
    /// </summary>
    public class Level
    {
        private Llave llaveAmarilla;
        private Llave llaveRoja;
        private Llave llaveVerde;
        private List<LevelSector> sectors;
        private TGCSkyBox skyBox;

        public Level()
        {
            sectors = new List<LevelSector>();
            Conectores = new Dictionary<string, Conector>();
            AlphaObjects = new List<IAlphaObject>();
        }

        /// <summary>
        ///     Numero de nivel
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Nombre del nivel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Conectores de todo el nivel
        /// </summary>
        public Dictionary<string, Conector> Conectores { get; set; }

        /// <summary>
        ///     Sector actual
        /// </summary>
        public LevelSector CurrentSector { get; set; }

        /// <summary>
        ///     Objetos a renderizar con AlphaBlending.
        ///     Se dibujan luego de dibujar todos los opacos
        /// </summary>
        public List<IAlphaObject> AlphaObjects { get; set; }

        /// <summary>
        ///     Cantidad de items animal de todo el nivel
        /// </summary>
        public int TotalItemsAnimal { get; set; }

        /// <summary>
        ///     Cargar nivel y todos sus sectores
        /// </summary>
        public void loadLevel(string levelName, int levelIndex)
        {
            Name = levelName;
            Index = levelIndex;
            var levelPath = GameModel.instance.gameConfig.levelsPath + levelName + "\\";

            //Load sectors
            var sectorsDir = Directory.GetDirectories(levelPath, "Sector-*", SearchOption.TopDirectoryOnly);
            foreach (var sectorDir in sectorsDir)
            {
                var texturePath = sectorDir + "\\";
                var lightmapPath = texturePath;

                var sector = new LevelSector();
                var dirInfo = new DirectoryInfo(sectorDir);
                var sectorName = dirInfo.Name;
                sector.loadScene(this, sectorName, sectorDir + "\\", texturePath, lightmapPath);
                sectors.Add(sector);
            }

            //Calcular cantidad de items animal de todo el nivel
            TotalItemsAnimal = 0;
            foreach (var sector in sectors)
            {
                TotalItemsAnimal += sector.ItemsAnimal.Count;
            }

            //Load skybox
            var skyboxTexturesPath = levelPath + "SkyBox\\";
            skyBox = new TGCSkyBox();
            skyBox.Center = new TGCVector3(0, 0, 0);
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            skyBox.SetFaceTexture(TGCSkyBox.SkyFaces.Up, skyboxTexturesPath + "Up.jpg");
            skyBox.SetFaceTexture(TGCSkyBox.SkyFaces.Down, skyboxTexturesPath + "Down.jpg");
            skyBox.SetFaceTexture(TGCSkyBox.SkyFaces.Left, skyboxTexturesPath + "Left.jpg");
            skyBox.SetFaceTexture(TGCSkyBox.SkyFaces.Right, skyboxTexturesPath + "Right.jpg");
            skyBox.SetFaceTexture(TGCSkyBox.SkyFaces.Front, skyboxTexturesPath + "Back.jpg");
            skyBox.SetFaceTexture(TGCSkyBox.SkyFaces.Back, skyboxTexturesPath + "Front.jpg");
            //skyBox.updateValues();
            skyBox.Init();

            //TODO: Chequear relaciones entre conectores
        }

        /// <summary>
        ///     Arrancar el nive en el primer sector
        /// </summary>
        public void initFirstSector(string firstSectorName, string firstConectorName)
        {
            CurrentSector = findSector(firstSectorName);
            var firstConector = CurrentSector.findSector(firstConectorName);
            var mainCharacter = GameModel.instance.mainCharacter;
            mainCharacter.resetForNewLevel();
            CurrentSector.initSector(mainCharacter, firstConector);

            //Para que todo termine de cargar
            //Thread.Sleep(1000);
        }

        /// <summary>
        ///     Cambia de sector
        /// </summary>
        public void changeSector(Conector nextConector)
        {
            //Transicion de loading
            GameModel.instance.currentGameMode = GameMode.LoadingSector;
            GameModel.instance.transiciones.initLoadingSector();

            //Cargar sector
            CurrentSector.hideSector();
            CurrentSector = nextConector.Sector;
            CurrentSector.initSector(GameModel.instance.mainCharacter, nextConector);
        }

        /// <summary>
        ///     Actualizar estado del nivel
        /// </summary>
        public void update(float elapsedTime)
        {
            CurrentSector.update(elapsedTime);
        }

        /// <summary>
        ///     Renderizar nivel
        /// </summary>
        public void render(float elapsedTime, TgcFrustum frustum)
        {
            //Render SkyBox
            skyBox.Render();

            //Render sector
            CurrentSector.render(elapsedTime, frustum);

            //Renderizar al final los objetos acumulados con alpha
            foreach (var alphaObj in AlphaObjects)
            {
                alphaObj.renderWithAlpha();
            }
        }

        private LevelSector findSector(string sectorName)
        {
            foreach (var sector in sectors)
            {
                if (sector.SectorName == sectorName)
                {
                    return sector;
                }
            }

            throw new Exception("Sector: " + sectorName + " not found");
        }

        /// <summary>
        ///     Devuelve la llave del nivel segun el color pedido
        /// </summary>
        public Llave getLlave(Llave.ColorType colorType)
        {
            switch (colorType)
            {
                case Llave.ColorType.VERDE:
                    return llaveVerde;
                case Llave.ColorType.ROJA:
                    return llaveRoja;
                case Llave.ColorType.AMARILLA:
                    return llaveAmarilla;
            }

            return null;
        }

        /// <summary>
        ///     Cargar una llave en el nivel
        /// </summary>
        public void setLlave(Llave llave)
        {
            var myLlave = getLlave(llave.Color);
            if (myLlave != null)
            {
                throw new Exception("Hay dos llaves del mismo color en el nivel: " + llave);
            }

            switch (llave.Color)
            {
                case Llave.ColorType.VERDE:
                    llaveVerde = llave;
                    break;
                case Llave.ColorType.ROJA:
                    llaveRoja = llave;
                    break;
                case Llave.ColorType.AMARILLA:
                    llaveAmarilla = llave;
                    break;
            }
        }

        /// <summary>
        ///     Cuando el personaje pierde una vida, por diversos motivos.
        ///     Se carga un efecto de perder.
        /// </summary>
        public void removeLifeToPlayer()
        {
            //Restar vida
            var mainCharacter = GameModel.instance.mainCharacter;
            mainCharacter.Vidas--;

            //Poner sonido de perder vida
            GameModel.instance.sonidos.stopAllSounds();
            GameModel.instance.sonidos.personaje_perder_vida.Play();

            //Quitar efecto de modo alarma
            GameModel.instance.postProcessEffects.setAlarmModeEnable(false);

            //Cargar efecto de Game Over
            GameModel.instance.postProcessEffects.setPlayerDeadEffect();
            GameModel.instance.currentGameMode = GameMode.PlayerDead;
        }

        /// <summary>
        ///     Luego de que el jugador perdio, arrancar en el ultimo conector por el que paso.
        ///     Si no le quedan mas vidas, pierde el juego y arranca desde el primer nivel.
        /// </summary>
        public void restoreFromLoseLife()
        {
            var mainCharacter = GameModel.instance.mainCharacter;

            //Arrancar en el ultimo conector por el que paso
            if (mainCharacter.Vidas > 0)
            {
                GameModel.instance.currentGameMode = GameMode.Playing;
                changeSector(GameModel.instance.currentLevel.CurrentSector.CurrentConector);
            }
            //Game over
            else
            {
                //Ocultar sector actual
                CurrentSector.hideSector();

                //Mostrar pantalla de GameOver
                GameModel.instance.transiciones.initGameOver();
                GameModel.instance.currentGameMode = GameMode.GameOver;

                //Arrancar desde el primer nivel del juego
                //Game.instance.LoadFirstLevel();
            }
        }

        /// <summary>
        ///     Pasar al siguiente nivel
        /// </summary>
        public void changeLevel()
        {
            CurrentSector.hideSector();
            var levelData = GameModel.instance.gameConfig.getNextLevel(Index);
            GameModel.instance.LoadLevel(levelData);
        }

        /// <summary>
        ///     Elimina todo contenido del nivel
        /// </summary>
        public void unloadLevel()
        {
            //Eliminar sectores
            foreach (var sector in sectors)
            {
                sector.unload();
            }

            sectors.Clear();
            sectors = null;
            CurrentSector = null;

            //Eliminar skybox
            skyBox.Dispose();
            skyBox = null;

            //Eliminar conectores
            foreach (var item in Conectores)
            {
                item.Value.dispose();
            }

            Conectores.Clear();
            Conectores = null;

            //Llaves
            llaveRoja.dispose();
            llaveVerde.dispose();
            llaveAmarilla.dispose();
            llaveRoja = null;
            llaveVerde = null;
            llaveAmarilla = null;

            //AlphaObjects
            AlphaObjects.Clear();
            AlphaObjects = null;
        }
    }
}
