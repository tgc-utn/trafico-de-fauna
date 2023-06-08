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
using System.Text;
using System.Windows.Forms;

namespace TgcViewer.Model.Menu.Error
{
    /// <summary>
    /// Ventana de errores
    /// </summary>
    public partial class ErrorWindow : System.Windows.Forms.Form
    {
        public ErrorWindow()
        {
            InitializeComponent();
            buttonCerrar.TabIndex = 0;
        }

        public void showError(string descripcion, Exception exception)
        {
            this.descripcion.Text = descripcion;
            if (exception != null)
            {
                StringBuilder sb = new StringBuilder();
                Exception e = exception;
                while (e != null)
                {
                    sb.AppendLine(e.Message);
                    sb.AppendLine(e.Source);
                    sb.AppendLine(e.StackTrace);
                    e = e.InnerException;
                }
                this.exception.Text = sb.ToString();
            }

            Cursor.Show();
            this.ShowDialog();
            buttonCerrar.Focus();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
