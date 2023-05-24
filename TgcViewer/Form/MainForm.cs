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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TgcViewer.Utils;
using System.Diagnostics;
using TgcViewer.Example;

namespace TgcViewer
{
    /// <summary>
    /// Formulario principal de la aplicación
    /// </summary>
    public partial class MainForm : Form
    {
        static bool applicationRunning;
        /// <summary>
        /// Obtener o parar el estado del RenderLoop.
        /// </summary>
        public static bool ApplicationRunning
        {
            get { return MainForm.applicationRunning; }
            set { MainForm.applicationRunning = value; }
        }

        //Variables por compatibilidad
        RichTextBox logConsole;
        TreeView treeViewExamples;
        DataGridView dataGridUserVars;
        Panel flowLayoutPanelModifiers;
        
        public MainForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            //Variables por compatibilidad
            logConsole = new RichTextBox();
            treeViewExamples = new TreeView();
            dataGridUserVars = new DataGridView();
            flowLayoutPanelModifiers = new Panel();
        }

        /// <summary>
        /// Ejecutar ejemplo fijo
        /// </summary>
        private void startExample(GuiController guiController)
        {
            TgcExample example = guiController.getExampleByName("TraficoFauna", "TraficoFauna");
            guiController.executeExample(example);
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            //Show the App before we init
            this.Show();
            this.panel3d.Focus();

            MainForm.ApplicationRunning = true;
            GuiController.newInstance();
            GuiController guiController = GuiController.Instance;
            guiController.initGraphics(this, panel3d);
            this.resetMenuOptions();

            startExample(guiController);

            while (MainForm.ApplicationRunning)
            {
                //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios
                if (this.ContainsFocus)
                {
                    guiController.render();
                }
                else
                {
                    //Si no tenemos el foco, dormir cada tanto para no consumir gran cantida de CPU
                    System.Threading.Thread.Sleep(100); 
                }
                
                // Process application messages
                Application.DoEvents();
            }

            //shutDown();
        }

        /// <summary>
        /// Finalizar aplicacion
        /// </summary>
        internal void shutDown()
        {
            MainForm.ApplicationRunning = false;
            //GuiController.Instance.shutDown();
            //Application.Exit();

            //Matar proceso principal a la fuerza
            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.Kill();
        }

        /// <summary>
        /// Cerrando el formulario
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            shutDown();
        }

        /// <summary>
        /// Consola de Log
        /// </summary>
        public RichTextBox LogConsole
        {
            get { return logConsole; }
        }

        /// <summary>
        /// TreeView de ejemplos
        /// </summary>
        public TreeView TreeViewExamples
        {
            get { return treeViewExamples; }
        }

        

        /// <summary>
        /// Texto de ToolStripStatusPosition
        /// </summary>
        public void setStatusPosition(string text)
        {
        }

        /// <summary>
        /// Texto de ToolStripStatusCurrentExample
        /// </summary>
        /// <param name="text"></param>
        public void setCurrentExampleStatus(string text)
        {
        }

        /// <summary>
        /// Tabla de variables de usuario
        /// </summary>
        /// <returns></returns>
        public DataGridView getDataGridUserVars()
        {
            return dataGridUserVars;
        }

        /// <summary>
        /// Panel de Modifiers
        /// </summary>
        /// <returns></returns>
        internal Panel getModifiersPanel()
        {
            return flowLayoutPanelModifiers;
        }

        


        private void treeViewExamples_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void treeViewExamples_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeViewExamples.SelectedNode != null)
            {
                TreeNode selectedNode = treeViewExamples.SelectedNode;
                if (selectedNode.Nodes.Count == 0)
                {
                    GuiController.Instance.executeSelectedExample(selectedNode, FullScreenEnable);
                }
            }
        }

        

        /// <summary>
        /// Setea los valores default de las opciones del menu
        /// </summary>
        internal void resetMenuOptions()
        {

        }

        /// <summary>
        /// Modo FullScreen
        /// </summary>
        internal bool FullScreenEnable
        {
            get { return false; }
            set {  }
        }

        internal void removePanel3dFromMainForm()
        {
        }

        internal void addPanel3dToMainForm()
        {
        }


        /// <summary>
        /// Mostrar posicion de camara
        /// </summary>
        internal bool MostrarPosicionDeCamaraEnable
        {
            get { return false; }
            set {  }
        }




        
    }
}