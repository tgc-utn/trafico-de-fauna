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
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TgcViewer.Model;
using TgcViewer.Model.Menu.Error;

namespace TgcViewer.Form
{
    /// <summary>
    /// Formulario principal de la aplicación
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Ejemplo del juego a correr
        /// </summary>
        private TgcExample Modelo { get; set; }

        /// <summary>
        /// Obtener o parar el estado del RenderLoop.
        /// </summary>
        private bool ApplicationRunning { get; set; }

        /// <summary>
        /// Permite manejar el sonido
        /// </summary>
        private TgcDirectSound DirectSound { get; set; }

        /// <summary>
        /// Permite manejar los inputs de la computadora
        /// </summary>
        private TgcD3dInput Input { get; set; }

        public MainForm()
        {
            InitializeComponent();
            //TODO check this.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Iniciar graficos
            InitGraphics();

            //Titulo de la ventana principal
            Text = Modelo.Name + " - " + Modelo.Description;

            //Focus panel3D
            panel3D.Focus();

            //Inicio el ciclo de Render
            InitRenderLoop();
        }

        /// <summary>
        /// Cerrando el formulario
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationRunning)
            {
                ShutDown();
            }
        }

        /// <summary>
        /// Inicio todos los objetos necesarios para cargar el ejemplo y directx
        /// </summary>
        public void InitGraphics()
        {
            //Se inicio la aplicacion
            ApplicationRunning = true;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(panel3D);

            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(this, panel3D);

            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(panel3D);

            //Directorio actual de ejecucion
            var currentDirectory = Environment.CurrentDirectory + "\\";

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(currentDirectory + Game.Default.ShadersDirectory);

            //Juego a ejecutar
            Modelo = new GameModel(currentDirectory + Game.Default.MediaDirectory, currentDirectory + Game.Default.ShadersDirectory);

            //Cargar juego
            ExecuteModel();
        }

        /// <summary>
        /// Comienzo el loop del juego
        /// </summary>
        public void InitRenderLoop()
        {
            try
            {
                while (ApplicationRunning)
                {
                    //Renderizo si es que hay un ejemplo activo
                    if (Modelo != null)
                    {
                        //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios
                        if (ApplicationActive())
                        {
                            Modelo.Update();
                            Modelo.Render();
                        }
                        else
                        {
                            //Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU
                            Thread.Sleep(100);
                        }
                    }
                    // Process application messages
                    Application.DoEvents();
                }
            }
            catch (Exception e)
            {
                //Error Window
                new ErrorWindow().showError(e.Message, e);
                this.ShutDown();
            }
        }

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (ContainsFocus)
            {
                return true;
            }

            foreach (var form in OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Arranca a ejecutar un ejemplo.
        ///     Para el ejemplo anterior, si hay alguno.
        /// </summary>
        public void ExecuteModel()
        {
            //Ejecutar Init
            try
            {
                Modelo.ResetDefaultConfig();
                Modelo.DirectSound = DirectSound;
                Modelo.Input = Input;
                Modelo.Init();
                panel3D.Focus();
            }
            catch (Exception e)
            {
                //Error Window
                new ErrorWindow().showError(e.Message, e);
                this.ShutDown();
            }
        }

        /// <summary>
        ///     Deja de ejecutar el ejemplo actual
        /// </summary>
        public void StopCurrentExample()
        {
            if (Modelo != null)
            {
                Modelo.Dispose();
                Modelo = null;
            }
        }

        /// <summary>
        ///     Finalizar aplicacion
        /// </summary>
        public void ShutDown()
        {
            ApplicationRunning = false;

            if (Modelo != null)
            {
                StopCurrentExample();
            }

            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }
    }
}