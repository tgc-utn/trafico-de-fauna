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
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Input;
using TraficoFauna.Examples;
using TgcViewer;
using TgcViewer.Utils._2D;
using System.Drawing;
using TgcViewer.Utils;
using Examples.TraficoFauna.Meshes;
using TgcViewer.Utils.TgcGeometry;
using Examples.TraficoFauna.Saltos;
using Examples.TraficoFauna.CustomEvents;
using Examples.TraficoFauna.Escondites;
using Examples.TraficoFauna.Enemies;
using TgcViewer.Utils.TgcSceneLoader;
using System.IO;
using System.Collections;

namespace Examples.TraficoFauna.Debug
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

        TgcText2d textDebug;
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
            textDebug = new TgcText2d();
            int width = GuiController.Instance.Panel3d.Width;
            int height = GuiController.Instance.Panel3d.Height;
            textDebug.Position = new System.Drawing.Point(0, height / 2);
            textDebug.Align = TgcText2d.TextAlign.LEFT;
            textDebug.Color = Color.Yellow;
            textDebug.Size = new System.Drawing.Size(width, height / 2);

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

            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            /*
            //Habilitar modo debug por teclado
            if (d3dInput.keyPressed(Key.D))
            {
                this.enabled = !this.enabled;
            }
            */


            //Activar ver BoundingVolume
            if (d3dInput.keyPressed(Key.B))
            {
                showBoundingVolumes = !showBoundingVolumes;
            }


            //Ganar vida
            if (d3dInput.keyPressed(Key.V))
            {
                Game.instance.mainCharacter.agregarBananas(100);
            }

            //Obtener todas las llaves
            if (d3dInput.keyPressed(Key.K))
            {
                Game.instance.currentLevel.getLlave(Examples.TraficoFauna.ItemsAnimal.Llave.ColorType.AMARILLA).agarrar();
                Game.instance.currentLevel.getLlave(Examples.TraficoFauna.ItemsAnimal.Llave.ColorType.ROJA).agarrar();
                Game.instance.currentLevel.getLlave(Examples.TraficoFauna.ItemsAnimal.Llave.ColorType.VERDE).agarrar();
            }

            //Otorgar 1 item animal
            if (d3dInput.keyPressed(Key.I))
            {
                Game.instance.mainCharacter.agregarItemAnimalLiberado(null);
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
                    Dictionary<string, Conector> conectores = Game.instance.currentLevel.Conectores;
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
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            GuiController.Instance.AxisLines.render();

            //Render de texto debug
            textDebug.render();

            //Bounding Volumes
            if (showBoundingVolumes)
            {
                LevelSector CurrentSector = Game.instance.currentLevel.CurrentSector;

                //Objetos colsionables
                Game.instance.mainCharacter.CharacterSphere.render();
                List<ColliderObject> objetosColisionables = CurrentSector.ObjetosColisionables;
                foreach (ColliderObject obj in objetosColisionables)
                {
                    if (obj.Enabled)
                    {
                        obj.BoundingBox.render();
                    }
                }

                //conectores
                List<Conector> conectores = CurrentSector.Conectores;
                foreach (Conector c in conectores)
                {
                    c.BoundingBox.render();
                }

                //radioVisionPersonaje
                radioVisionPersonaje.setValues(Game.instance.mainCharacter.CurrentPos, Game.instance.gameConfig.camara.distanciaVisual);
                radioVisionPersonaje.render();

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
                    esc.BSphere.render();
                }

                //Waypoints de guardias
                List<Guardia> guardias =  CurrentSector.GuardiasManager.Guardias;
                foreach (Guardia g in guardias)
                {
                    foreach (GuardiaWaypoint w in g.Waypoints)
                    {
                        w.Bsphere.render();
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
                List<TgcTexture.Pool.PoolItem> textures = GuiController.Instance.TexturesPool.getAllTextures();
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

                Game.instance.errorWindow.showError("Memoria de texturas", new Exception(sb.ToString()));
            }
        }

        class TextureMemory : IComparable
        {
            public string FilePath;
            public float Length;
            public int References;

            public int CompareTo(object obj)
            {
                return Length.CompareTo(((TextureMemory)obj).Length);
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
                    Game.instance.currentLevel.changeSector(conectoresList[10]);
                }
                if (d3dInput.keyPressed(Key.D1))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[11]);
                }
                if (d3dInput.keyPressed(Key.D2))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[12]);
                }
                if (d3dInput.keyPressed(Key.D3))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[13]);
                }
                if (d3dInput.keyPressed(Key.D4))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[14]);
                }
                if (d3dInput.keyPressed(Key.D5))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[15]);
                }
                if (d3dInput.keyPressed(Key.D6))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[16]);
                }
                if (d3dInput.keyPressed(Key.D7))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[17]);
                }
                if (d3dInput.keyPressed(Key.D8))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[18]);
                }
                if (d3dInput.keyPressed(Key.D9))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[19]);
                }
            }
            //Abajo de 10
            else
            {
                if (d3dInput.keyPressed(Key.D0))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[0]);
                }
                if (d3dInput.keyPressed(Key.D1))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[1]);
                }
                if (d3dInput.keyPressed(Key.D2))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[2]);
                }
                if (d3dInput.keyPressed(Key.D3))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[3]);
                }
                if (d3dInput.keyPressed(Key.D4))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[4]);
                }
                if (d3dInput.keyPressed(Key.D5))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[5]);
                }
                if (d3dInput.keyPressed(Key.D6))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[6]);
                }
                if (d3dInput.keyPressed(Key.D7))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[7]);
                }
                if (d3dInput.keyPressed(Key.D8))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[8]);
                }
                if (d3dInput.keyPressed(Key.D9))
                {
                    Game.instance.currentLevel.changeSector(conectoresList[9]);
                }
            }
        }


    }
}
