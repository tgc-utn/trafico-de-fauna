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
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils;
using System.Windows.Forms;
using TgcViewer.Example;
using System.Reflection;
using System.Drawing;
using TgcViewer.Utils.Input;
using System.Globalization;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.Render;

namespace TgcViewer
{
    /// <summary>
    /// Controlador principal de la aplicación
    /// </summary>
    public class GuiController
    {
        #region Singleton
        private static volatile GuiController instance;

        /// <summary>
        /// Permite acceder a una instancia de la clase GuiController desde cualquier parte del codigo.
        /// </summary>
        public static GuiController Instance
        {
            get
            { return instance; }
        }

        /// <summary>
        /// Crear nueva instancia. Solo el MainForm lo hace
        /// </summary>
        internal static void newInstance()
        {
            instance = new GuiController();
        }

        #endregion

        MainForm mainForm;
        Control panel3d;
        TgcD3dDevice tgcD3dDevice;
        string examplesDir;
        string examplesMediaDir;
        string alumnoEjemplosDir;
        string alumnoEjemplosMediaDir;
        ExampleLoader exampleLoader;
        TgcExample currentExample;
        TgcAxisLines axisLines;
        TgcDrawText text3d;
        bool fpsCounterEnable;
        TgcD3dInput tgcD3dInput;
        float elapsedTime;
        TgcFrustum frustum;
        TgcTexture.Pool texturesPool;
        TgcTexture.Manager texturesManager;
        TgcMp3Player mp3Player;
        TgcDirectSound directSound;
        TgcRenderQueue renderQueue;
        TgcRenderQueue defaultRenderQueue;
        TgcCamera currentCamera;




        #region Internal Methods

        private GuiController()
        {
        }

        /// <summary>
        /// Crea todos los modulos necesarios de la aplicacion
        /// </summary>
        internal void initGraphics(MainForm mainForm, Control panel3d)
        {
            this.mainForm = mainForm;
            this.panel3d = panel3d;
            panel3d.Focus();

            //Iniciar graficos
            this.tgcD3dDevice = new TgcD3dDevice(panel3d);
            this.texturesManager = new TgcTexture.Manager();
            this.tgcD3dDevice.OnResetDevice(tgcD3dDevice.D3dDevice, null);

            //Iniciar otras herramientas
            this.texturesPool = new TgcTexture.Pool();
            this.text3d = new TgcDrawText(tgcD3dDevice.D3dDevice);
            this.tgcD3dInput = new TgcD3dInput(mainForm, panel3d);
            this.axisLines = new TgcAxisLines(tgcD3dDevice.D3dDevice);
            this.elapsedTime = -1;
            this.frustum = new TgcFrustum();
            this.mp3Player = new TgcMp3Player();
            this.directSound = new TgcDirectSound();
            this.defaultRenderQueue = new TgcDefaultRenderQueue();
            this.renderQueue = this.defaultRenderQueue;
            

            //toogles
            this.fpsCounterEnable = true;
            this.axisLines.Enable = true;

            //Cargar algoritmos
            exampleLoader = new ExampleLoader();
            examplesDir = System.Environment.CurrentDirectory + "\\" + ExampleLoader.EXAMPLES_DIR + "\\";
            examplesMediaDir = examplesDir + "Media" + "\\";
            alumnoEjemplosDir = System.Environment.CurrentDirectory + "\\" + "AlumnoEjemplos" + "\\";
            alumnoEjemplosMediaDir = alumnoEjemplosDir + "AlumnoMedia" + "\\";
            //exampleLoader.loadExamplesInGui(mainForm.TreeViewExamples, new string[] { examplesDir, alumnoEjemplosDir });
            exampleLoader.loadExamplesInGui(mainForm.TreeViewExamples, new string[] { examplesDir });
        }
       
        internal void render()
        {
            Device d3dDevice = tgcD3dDevice.D3dDevice;
            elapsedTime = HighResolutionTimer.Instance.FrameTime;

            tgcD3dDevice.doClear();

            //Acutalizar input
            tgcD3dInput.update();

            //Actualizar camaras (solo una va a estar activada a la vez)
            this.currentCamera.updateCamera();
            this.currentCamera.updateViewMatrix(d3dDevice);

            //actualizar posicion de pantalla en barra de estado de UI
            //setStatusPosition();

            //actualizar el Frustum
            frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);

            //limpiar texturas
            texturesManager.clearAll();

            //actualizar Listener3D
            directSound.updateListener3d();



            /*
            d3dDevice.BeginScene();
            

            //Actualizar contador de FPS si esta activo
            if (fpsCounterEnable)
            {
                text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            }
            */


            //Ejecutar render del ejemplo
            if (currentExample != null)
            {
                currentExample.render(elapsedTime);
            }

            /*
            //Ejes cartesianos
            if (axisLines.Enable)
            {
                axisLines.render();
            }

            //Renderizar toda la RenderQueue
            this.renderQueue.renderAll();
            this.renderQueue.clearAll();

            d3dDevice.EndScene();

            
            */


            d3dDevice.Present();


            //this.Invalidate();
        }

        /// <summary>
        /// Cuando se selecciona un ejemplo para ejecutar del TreeNode
        /// </summary>
        internal void executeSelectedExample(TreeNode treeNode, bool fullScreenEnable)
        {
            TgcExample example = exampleLoader.getExampleByTreeNode(treeNode);
            executeExample(example);
        }

        /// <summary>
        /// Devuelve el primer TgcExample con el name y category especificados (de los metodos getName y getCategory)
        /// </summary>
        internal TgcExample getExampleByName(string name, string category)
        {
            foreach (TgcExample example in exampleLoader.CurrentExamples)
            {
                if(example.getName() == name && example.getCategory() == category)
                {
                    return example;
                }
            }
            throw new Exception("Example not found. Name: " + name + ", Category: " + category);
        }

        /// <summary>
        /// Arranca a ejecutar un ejemplo.
        /// Para el ejemplo anterior, si hay alguno.
        /// </summary>
        /// <param name="example"></param>
        internal void executeExample(TgcExample example)
        {
            stopCurrentExample();
            resetDefaultConfig();

            //Ejecutar init
            try
            {
                example.init();

                this.currentExample = example;
                panel3d.Focus();
                mainForm.setCurrentExampleStatus("Ejemplo actual: " + example.getName() );
            }
            catch (Exception e)
            {
                throw new Exception("Error en init() de ejemplo: " + example.getName(), e);
            }
        }

        /// <summary>
        /// Deja de ejecutar el ejemplo actual
        /// </summary>
        internal void stopCurrentExample()
        {
            if (currentExample != null)
            {
                currentExample.close();
                tgcD3dDevice.resetWorldTransofrm();
                currentExample = null;
                elapsedTime = -1;
            }
        }

        /// <summary>
        /// Cerrar la aplicacion
        /// </summary>
        public void exitGame()
        {
            mainForm.shutDown();
        }

        /// <summary>
        /// Cuando se cierra la ventana de FullScreen
        /// </summary>
        internal void closeFullScreenPanel()
        {
            mainForm.addPanel3dToMainForm();
            panel3d.Focus();
        }

        /// <summary>
        /// Finaliza la ejecución de la aplicacion
        /// </summary>
        internal void shutDown()
        {
            if (currentExample != null)
            {
                currentExample.close();
            }
            tgcD3dDevice.shutDown();
            texturesPool.clearAll();
        }

        /// <summary>
        /// Termina y vuelve a empezar el ejemplo actual, si hay alguno ejecutando.
        /// </summary>
        internal void resetCurrentExample()
        {
            if (currentExample != null)
            {
                TgcExample exampleBackup = currentExample;
                stopCurrentExample();
                executeExample(exampleBackup);
            }
        }

        /// <summary>
        /// Vuelve la configuracion de render y otras cosas a la configuracion inicial
        /// </summary>
        internal void resetDefaultConfig()
        {
            mainForm.resetMenuOptions();
            this.axisLines.Enable = true;
            this.fpsCounterEnable = true;
            tgcD3dDevice.setDefaultValues();
            this.mp3Player.closeFile();

            //Reset de RenderQueue
            this.renderQueue = this.defaultRenderQueue;
            this.renderQueue.clearAll();
        }

        /// <summary>
        /// Cuando el Direct3D Device se resetea.
        /// Se reinica el ejemplo actual, si hay alguno.
        /// </summary>
        internal void onResetDevice()
        {
            TgcExample exampleBackup = currentExample;
            if (exampleBackup != null)
            {
                stopCurrentExample();
            }
            tgcD3dDevice.doResetDevice();
            if (exampleBackup != null)
            {
                executeExample(exampleBackup);
            }
        }

        /// <summary>
        /// Hace foco en el panel 3D de la aplicacion.
        /// Es util para evitar que el foco quede en otro contro, por ej. un boton,
        /// y que los controles de navegacion respondan mal
        /// </summary>
        internal void focus3dPanel()
        {
            panel3d.Focus();
        }

        /// <summary>
        /// Actualiza en la pantalla principal la posicion actual de la camara
        /// </summary>
        private void setStatusPosition()
        {
            //Actualizar el textbox en todos los cuadros reduce los FPS en algunas PC
            if (mainForm.MostrarPosicionDeCamaraEnable)
            {
                Vector3 pos = currentCamera.getPosition();
                Vector3 lookAt = currentCamera.getLookAt();
                string statusPosition = "Position: [" + TgcParserUtils.printFloat(pos.X) + ", " + TgcParserUtils.printFloat(pos.Y) + ", " + TgcParserUtils.printFloat(pos.Z) + "] " +
                    "- LookAt: [" + TgcParserUtils.printFloat(lookAt.X) + ", " + TgcParserUtils.printFloat(lookAt.Y) + ", " + TgcParserUtils.printFloat(lookAt.Z) + "]";
                mainForm.setStatusPosition(statusPosition);
            }
        }

        



        #endregion



        #region Getters and Setters and Public Methods

        /// <summary>
        /// Direct3D Device
        /// </summary>
        public Device D3dDevice
        {
            get { return tgcD3dDevice.D3dDevice; }
        }

        /// <summary>
        /// Path absoluto del directorio en donde se encuentran las DLL de los ejemplos
        /// </summary>
        public string ExamplesDir
        {
            get { return examplesDir; }
        }

        /// <summary>
        /// Path absoluto de la carpeta Media que contiene todo el contenido visual de los
        /// ejemplos, como texturas, modelos 3D, etc.
        /// </summary>
        public string ExamplesMediaDir
        {
            get { return examplesMediaDir; }
        }

        /// <summary>
        /// Path absoluto del directorio en donde se encuentran las DLL de los ejemplos del alumno
        /// </summary>
        public string AlumnoEjemplosDir
        {
            get { return alumnoEjemplosDir; }
        }

        /// <summary>
        /// Path absoluto de la carpeta AlumnoMedia que contiene todo el contenido visual de los
        /// ejemplos del alumno
        /// </summary>
        public string AlumnoEjemplosMediaDir
        {
            get { return alumnoEjemplosMediaDir; }
        }
        

        /// <summary>
        /// Utilidad para escribir texto dentro de la pantalla 3D
        /// </summary>
        public TgcDrawText Text3d
        {
            get { return text3d; }
        }

        /// <summary>
        /// Habilita o desactiva el contador de FPS
        /// </summary>
        public bool FpsCounterEnable
        {
            get { return fpsCounterEnable; }
            set { fpsCounterEnable = value; }
        }

        /// <summary>
        /// Utilidad para visualizar los ejes cartesianos
        /// </summary>
        public TgcAxisLines AxisLines
        {
            get { return axisLines; }
        }

        /// <summary>
        /// Utilidad para acceder al Input de Teclado y Mouse
        /// </summary>
        public TgcD3dInput D3dInput
        {
            get { return tgcD3dInput; }
        }

        /// <summary>
        /// Tiempo en segundos transcurridos desde el último frame.
        /// Solo puede ser invocado cuando se esta ejecutando un bloque de render() de un TgcExample
        /// </summary>
        public float ElapsedTime
        {
            get { return elapsedTime; }
        }

        /// <summary>
        /// Ventana principal de la aplicacion
        /// </summary>
        public MainForm MainForm
        {
            get { return mainForm; }
        }

        /// <summary>
        /// Control gráfico de .NET utilizado para el panel3D sobre el cual renderiza el
        /// Device de Direct3D
        /// </summary>
        public Control Panel3d
        {
            get { return panel3d; }
        }

        /// <summary>
        /// Frustum que representa el volumen de vision actual
        /// Solo puede ser invocado cuando se esta ejecutando un bloque de render() de un TgcExample
        /// </summary>
        public TgcFrustum Frustum
        {
            get { return frustum; }
        }

        /// <summary>
        /// Pool de texturas
        /// </summary>
        public TgcTexture.Pool TexturesPool
        {
            get { return texturesPool; }
        }

        /// <summary>
        /// Herramienta para configurar texturas en el Device
        /// </summary>
        public TgcTexture.Manager TexturesManager
        {
            get { return texturesManager; }
        }

        /// <summary>
        /// Configura la posicion de la cámara
        /// </summary>
        /// <param name="pos">Posición de la cámara</param>
        /// <param name="lookAt">Punto hacia el cuál se quiere ver</param>
        public void setCamera(Vector3 pos, Vector3 lookAt)
        {
            tgcD3dDevice.D3dDevice.Transform.View = Matrix.LookAtLH(pos, lookAt, new Vector3(0, 1, 0));

            //Imprimir posicion
            string statusPos = "Position: [" + TgcParserUtils.printFloat(pos.X) + ", " + TgcParserUtils.printFloat(pos.Y) + ", " + TgcParserUtils.printFloat(pos.Z) + "] " +
                "- LookAt: [" + TgcParserUtils.printFloat(lookAt.X) + ", " + TgcParserUtils.printFloat(lookAt.Y) + ", " + TgcParserUtils.printFloat(lookAt.Z) + "]";
            mainForm.setStatusPosition(statusPos);
        }

        /// <summary>
        /// Herramienta para reproducir archivos MP3s
        /// </summary>
        public TgcMp3Player Mp3Player
        {
            get { return this.mp3Player; }
        }

        /// <summary>
        /// Herramienta para manipular DirectSound
        /// </summary>
        public TgcDirectSound DirectSound
        {
            get { return this.directSound; }
        }

        /// <summary>
        /// Indica o configura el modo FullScreen para ejecutar ejemplos.
        /// </summary>
        public bool FullScreenEnable
        {
            get { return mainForm.FullScreenEnable; }
            set { mainForm.FullScreenEnable = value; }
        }

        /// <summary>
        /// Color con el que se limpia la pantalla
        /// </summary>
        public Color BackgroundColor
        {
            get { return tgcD3dDevice.ClearColor; }
            set { tgcD3dDevice.ClearColor = value; }
        }

        /// <summary>
        /// Herramienta para renderizar objetos en una cola de renderizado.
        /// Permite cambiar a otra implementación de TgcRenderQueue.
        /// </summary>
        public TgcRenderQueue RenderQueue
        {
            get { return renderQueue; }
            set { renderQueue = value; }
        }

        /// <summary>
        /// Cámara actual que utiliza el framework
        /// </summary>
        public TgcCamera CurrentCamera
        {
            get { return currentCamera; }
            set { currentCamera = value; }
        }

        #endregion

        




        
    }
}
