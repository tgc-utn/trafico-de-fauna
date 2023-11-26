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
using System.Text;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Text;
using Trafico.De.Fauna.Model.CustomEvents;
using Trafico.De.Fauna.Model.Enemies;
using Trafico.De.Fauna.Model.Escondites;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Trafico.De.Fauna.Model.Levels;
using Trafico.De.Fauna.Model.Meshes;
using Trafico.De.Fauna.Model.Saltos;

namespace Trafico.De.Fauna.Model.Debug
{
    /// <summary>
    /// Utilidades de Debug
    /// </summary>
    public class DebugMode
    {
        bool enabled;
        /// <summary>
        /// Habilitado
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        bool showBoundingVolumes;
        /// <summary>
        /// Mostrar Bounding Volumes
        /// </summary>
        public bool ShowBoundingVolumes
        {
            get { return showBoundingVolumes; }
            set { showBoundingVolumes = value; }
        }

        bool guardiasEnabled;
        /// <summary>
        /// Guardias Habilitados
        /// </summary>
        public bool GuardiasEnabled
        {
            get { return guardiasEnabled; }
            set { guardiasEnabled = value; }
        }

        TgcText2D textDebug;
        TgcBoundingSphere radioVisionPersonaje;
        bool showConectorList;
        List<Conector> conectoresList;

        public DebugMode()
        {
            enabled = false;
            showBoundingVolumes = false;
            showConectorList = false;
            guardiasEnabled = true;

            //texto debug
            textDebug = new TgcText2D();
            int width = D3DDevice.Instance.Device.Viewport.Width;
            int height = D3DDevice.Instance.Device.Viewport.Height;
            textDebug.Position = new Point(0, height / 2);
            textDebug.Align = TgcText2D.TextAlign.LEFT;
            textDebug.Color = Color.Yellow;
            textDebug.Size = new Size(width, height / 2);

            radioVisionPersonaje = new TgcBoundingSphere();
            conectoresList = new List<Conector>();
        }

        /// <summary>
        /// Texto debug
        /// </summary>
        public void setText(string text)
        {
            textDebug.Text = text;
        }

        public void render()
        {
            if (!enabled)
            {
                return;
            }

            TgcD3dInput d3dInput = GameModel.instance.Input;

            //Habilitar modo debug por teclado
            if (d3dInput.keyPressed(Key.D))
            {
                this.enabled = !this.enabled;
            }

            //Activar ver BoundingVolume
            if (d3dInput.keyPressed(Key.B))
            {
                showBoundingVolumes = !showBoundingVolumes;
            }

            //Ganar vida
            if (d3dInput.keyPressed(Key.V))
            {
                GameModel.instance.mainCharacter.agregarBananas(100);
            }

            //Obtener todas las llaves
            if (d3dInput.keyPressed(Key.K))
            {
                GameModel.instance.currentLevel.getLlave(Llave.ColorType.AMARILLA).agarrar();
                GameModel.instance.currentLevel.getLlave(Llave.ColorType.ROJA).agarrar();
                GameModel.instance.currentLevel.getLlave(Llave.ColorType.VERDE).agarrar();
            }

            //Otorgar 1 item animal
            if (d3dInput.keyPressed(Key.I))
            {
                GameModel.instance.mainCharacter.agregarItemAnimalLiberado(null);
            }

            //Desactivar guardias
            if (d3dInput.keyPressed(Key.G))
            {
                guardiasEnabled = !guardiasEnabled;
            }

            //Mostrar lista de conectores
            if (d3dInput.keyPressed(Key.S))
            {
                showConectorList = !showConectorList;
                if (showConectorList)
                {
                    Dictionary<string, Conector> conectores = GameModel.instance.currentLevel.Conectores;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Conectores: (Presionar el numero, o Alt + numero si es mayor a 9)");
                    int n = 0;
                    conectoresList.Clear();
                    foreach (KeyValuePair<string, Conector> entry in conectores)
                    {
                        sb.AppendLine(n + ") " + entry.Value.Id);
                        conectoresList.Add(entry.Value);
                        n++;
                    }
                    setText(sb.ToString());
                }
                else
                {
                    setText("");
                }
            }

            //Ir hacia un conector especifico de la lista
            jumpToConector(d3dInput);

            //Render

            //Hay que renderizar devuelta estas cosas porque se pierden con el RenderTarget
            //TODO Render de Axis and FPS.
            //GuiController.Instance.Text3d.drawText("FPS: " + GameModel.instance.HighResolutionTimer.FramesPerSecond, 0, 0, Color.Yellow);
            //GuiController.Instance.AxisLines.render();

            //Render de texto debug
            textDebug.render();

            //Bounding Volumes
            if (showBoundingVolumes)
            {
                LevelSector CurrentSector = GameModel.instance.currentLevel.CurrentSector;

                //Objetos colsionables
                GameModel.instance.mainCharacter.CharacterSphere.Render();
                List<ColliderObject> objetosColisionables = CurrentSector.ObjetosColisionables;
                foreach (ColliderObject obj in objetosColisionables)
                {
                    if (obj.Enabled)
                    {
                        obj.BoundingBox.Render();
                    }
                }

                //conectores
                List<Conector> conectores = CurrentSector.Conectores;
                foreach (Conector c in conectores)
                {
                    c.BoundingBox.Render();
                }

                //radioVisionPersonaje
                radioVisionPersonaje.setValues(GameModel.instance.mainCharacter.CurrentPos, GameModel.instance.gameConfig.camara.distanciaVisual);
                radioVisionPersonaje.Render();

                //saltos
                List<Salto> saltos = CurrentSector.SaltosManager.Saltos;
                foreach (Salto salto in saltos)
                {
                    salto.renderDebug();
                }

                //CustomEvents
                List<CustomEvent> events = CurrentSector.CustomEvents;
                foreach (CustomEvent cEvent in events)
                {
                    cEvent.renderDebug();
                }

                //EsconditesFijos
                List<EsconditeFijo> esconditesFijos = CurrentSector.EsconditesFijos;
                foreach (EsconditeFijo esc in esconditesFijos)
                {
                    esc.BSphere.Render();
                }

                //Waypoints de guardias
                List<Guardia> guardias = CurrentSector.GuardiasManager.Guardias;
                foreach (Guardia g in guardias)
                {
                    foreach (GuardiaWaypoint w in g.Waypoints)
                    {
                        w.Bsphere.Render();
                    }
                }
            }

            printTexturesMemory(d3dInput);
        }

        private void printTexturesMemory(TgcD3dInput d3dInput)
        {
            //Ver uso de memoria
            if (d3dInput.keyPressed(Key.M))
            {
                /*List<TgcTexture.Pool.PoolItem> textures =TexturesPool.getAllTextures();
                List<TextureMemory> texturesMem = new List<TextureMemory>();
                foreach (TgcTexture.Pool.PoolItem t in textures)
                {
                    TextureMemory tm = new TextureMemory();
                    tm.FilePath = t.FilePath;
                    tm.Length = new FileInfo(t.FilePath).Length / 1024f;
                    tm.References = t.References;
                    texturesMem.Add(tm);
                }

                StringBuilder sb = new StringBuilder();
                texturesMem.Sort();
                texturesMem.Reverse();
                foreach (TextureMemory tm in texturesMem)
                {
                    sb.Append(tm.FilePath + "; " + tm.Length + "; " + tm.References);
                    sb.AppendLine();
                }

                MessageBox.Show(sb.ToString(), "Memoria de texturas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // TODO check this.
                //GameModel.instance.errorWindow.showError(, new Exception(sb.ToString()));*/
            }
        }

        /// <summary>
        /// Ir hacia un conector especifico
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
                    GameModel.instance.currentLevel.changeSector(conectoresList[15]);
                }
                if (d3dInput.keyPressed(Key.D6))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[16]);
                }
                if (d3dInput.keyPressed(Key.D7))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[17]);
                }
                if (d3dInput.keyPressed(Key.D8))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[18]);
                }
                if (d3dInput.keyPressed(Key.D9))
                {
                    GameModel.instance.currentLevel.changeSector(conectoresList[19]);
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
