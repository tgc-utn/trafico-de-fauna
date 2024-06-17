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
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.DirectX.DirectInput;
using TGC.Core;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Trafico.De.Fauna.Model.Levels;
using Trafico.De.Fauna.Model.Menu.Error;

namespace Trafico.De.Fauna.Model.DebugTools
{
    /// <summary>
    ///     Utilidades de Debug
    /// </summary>
    public class DebugMode
    {
        private readonly List<Conector> conectoresList;
        private readonly TgcBoundingSphere radioVisionPersonaje;
        private readonly TGCText2D textDebug;
        private bool showConectorList;

        public DebugMode()
        {
            Enabled = false;
            ShowBoundingVolumes = false;
            showConectorList = false;
            GuardiasEnabled = true;

            //texto debug
            textDebug = new TGCText2D();
            var width = D3DDevice.Instance.Device.Viewport.Width;
            var height = D3DDevice.Instance.Device.Viewport.Height;
            textDebug.Position = new Point(0, height / 2);
            textDebug.Align = TGCText2D.TextAlign.LEFT;
            textDebug.Color = Color.Yellow;
            textDebug.Size = new Size(width, height / 2);

            radioVisionPersonaje = new TgcBoundingSphere();
            conectoresList = new List<Conector>();
        }

        /// <summary>
        ///     Habilitado
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Mostrar Bounding Volumes
        /// </summary>
        public bool ShowBoundingVolumes { get; set; }

        /// <summary>
        ///     Guardias Habilitados
        /// </summary>
        public bool GuardiasEnabled { get; set; }

        public void Render()
        {
            // TODO: axis should be enabled using AxisLinesEnable from TGC.Core, but it doesn't work at this point.
            if (GameModel.instance.AxisLinesEnable)
            {
                GameModel.instance.AxisLines.render();
            }

            //Render de texto debug
            textDebug.Render();

            //Bounding Volumes
            if (ShowBoundingVolumes)
            {
                var CurrentSector = GameModel.instance.currentLevel.CurrentSector;

                //Objetos colsionables
                GameModel.instance.mainCharacter.CharacterSphere.Render();
                var objetosColisionables = CurrentSector.ObjetosColisionables;
                foreach (var obj in objetosColisionables)
                {
                    if (obj.Enabled)
                    {
                        obj.BoundingBox.Render();
                    }
                }

                //conectores
                var conectores = CurrentSector.Conectores;
                foreach (var c in conectores)
                {
                    c.BoundingBox.Render();
                }

                //radioVisionPersonaje
                radioVisionPersonaje.setValues(GameModel.instance.mainCharacter.CurrentPos,
                    GameModel.instance.gameConfig.camara.distanciaVisual);
                radioVisionPersonaje.Render();

                //saltos
                var saltos = CurrentSector.SaltosManager.Saltos;
                foreach (var salto in saltos)
                {
                    salto.renderDebug();
                }

                //CustomEvents
                var events = CurrentSector.CustomEvents;
                foreach (var cEvent in events)
                {
                    cEvent.renderDebug();
                }

                //EsconditesFijos
                var esconditesFijos = CurrentSector.EsconditesFijos;
                foreach (var esc in esconditesFijos)
                {
                    esc.BSphere.Render();
                }

                //Waypoints de guardias
                var guardias = CurrentSector.GuardiasManager.Guardias;
                foreach (var g in guardias)
                foreach (var w in g.Waypoints)
                {
                    w.Bsphere.Render();
                }
            }
        }

        public void Update(TgcD3dInput input, GameModel gameModel)
        {
            //Habilitar modo debug por teclado
            if (input.keyPressed(Key.D))
            {
                if (Enabled)
                {
                    DisableDebugMode();
                }

                Enabled = !Enabled;
                gameModel.FPSText = !gameModel.FPSText;
                gameModel.AxisLinesEnable = !gameModel.AxisLinesEnable;
            }

            if (!Enabled || gameModel.currentLevel == null)
            {
                return;
            }

            //Activar ver BoundingVolume
            if (input.keyPressed(Key.B))
            {
                ShowBoundingVolumes = !ShowBoundingVolumes;
            }

            //Ganar vida
            if (input.keyPressed(Key.V))
            {
                gameModel.mainCharacter.agregarBananas(100);
            }

            //Obtener todas las llaves
            if (input.keyPressed(Key.K))
            {
                gameModel.currentLevel.getLlave(Llave.ColorType.AMARILLA).agarrar();
                gameModel.currentLevel.getLlave(Llave.ColorType.ROJA).agarrar();
                gameModel.currentLevel.getLlave(Llave.ColorType.VERDE).agarrar();
            }

            //Otorgar 1 item animal
            if (input.keyPressed(Key.I))
            {
                gameModel.mainCharacter.agregarItemAnimalLiberado(null);
            }

            //Desactivar guardias
            if (input.keyPressed(Key.G))
            {
                GuardiasEnabled = !GuardiasEnabled;
            }

            //Mostrar lista de conectores
            if (input.keyPressed(Key.S))
            {
                showConectorList = !showConectorList;
                if (showConectorList)
                {
                    var conectores = gameModel.currentLevel.Conectores;
                    var sb = new StringBuilder();
                    sb.AppendLine("Conectores: (Presionar el numero, o Alt + numero si es mayor a 9)");
                    var n = 0;
                    conectoresList.Clear();
                    foreach (var entry in conectores)
                    {
                        sb.AppendLine(n + ") " + entry.Value.Id);
                        conectoresList.Add(entry.Value);
                        n++;
                    }

                    textDebug.Text = sb.ToString();
                }
                else
                {
                    textDebug.Text = "";
                    conectoresList.Clear();
                }
            }

            //Ir hacia un conector especifico de la lista
            if (showConectorList)
            {
                jumpToConector(input);
            }

            printTexturesMemory(input);
        }

        private void DisableDebugMode()
        {
            GuardiasEnabled = true;
            ShowBoundingVolumes = false;
            showConectorList = false;
            textDebug.Text = "";
        }

        private void printTexturesMemory(TgcD3dInput d3dInput)
        {
            //Ver uso de memoria
            if (d3dInput.keyPressed(Key.M))
            {
                var textures = TexturesPool.Instance.GetAllTextures();
                var texturesMem = new List<TextureMemory>();
                foreach (var t in textures)
                {
                    var tm = new TextureMemory();
                    tm.FilePath = t.FilePath;
                    tm.Length = new FileInfo(t.FilePath).Length / 1024f;
                    tm.References = t.References;
                    texturesMem.Add(tm);
                }

                var sb = new StringBuilder();
                texturesMem.Sort();
                texturesMem.Reverse();
                foreach (var tm in texturesMem)
                {
                    sb.Append(tm.FilePath + "; " + tm.Length + "; " + tm.References);
                    sb.AppendLine();
                }

                new ErrorWindow().showError("Memoria de texturas", new Exception(sb.ToString()));
            }
        }

        /// <summary>
        ///     Ir hacia un conector especifico
        /// </summary>
        private void jumpToConector(TgcD3dInput d3dInput)
        {
            //Arriba de 10
            if (d3dInput.keyDown(Key.LeftAlt) || d3dInput.keyDown(Key.RightAlt))
            {
                if (d3dInput.keyPressed(Key.D0))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[10]);
                }

                if (d3dInput.keyPressed(Key.D1))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[11]);
                }

                if (d3dInput.keyPressed(Key.D2))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[12]);
                }

                if (d3dInput.keyPressed(Key.D3))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[13]);
                }

                if (d3dInput.keyPressed(Key.D4))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[14]);
                }

                if (d3dInput.keyPressed(Key.D5))
                {
                    //GameModel.instance.currentLevel.changeSector(conectoresList[15]);
                }

                if (d3dInput.keyPressed(Key.D6))
                {
                    //GameModel.instance.currentLevel.changeSector(conectoresList[16]);
                }

                if (d3dInput.keyPressed(Key.D7))
                {
                    //GameModel.instance.currentLevel.changeSector(conectoresList[17]);
                }

                if (d3dInput.keyPressed(Key.D8))
                {
                    //GameModel.instance.currentLevel.changeSector(conectoresList[18]);
                }

                if (d3dInput.keyPressed(Key.D9))
                {
                    //GameModel.instance.currentLevel.changeSector(conectoresList[19]);
                }
            }
            //Abajo de 10
            else
            {
                if (d3dInput.keyPressed(Key.D0))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[0]);
                }

                if (d3dInput.keyPressed(Key.D1))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[1]);
                }

                if (d3dInput.keyPressed(Key.D2))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[2]);
                }

                if (d3dInput.keyPressed(Key.D3))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[3]);
                }

                if (d3dInput.keyPressed(Key.D4))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[4]);
                }

                if (d3dInput.keyPressed(Key.D5))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[5]);
                }

                if (d3dInput.keyPressed(Key.D6))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[6]);
                }

                if (d3dInput.keyPressed(Key.D7))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[7]);
                }

                if (d3dInput.keyPressed(Key.D8))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[8]);
                }

                if (d3dInput.keyPressed(Key.D9))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[9]);
                }
            }
        }
    }
}
