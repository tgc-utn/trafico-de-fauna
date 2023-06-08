namespace TgcViewer.Model.Menu.Error
{
    partial class ErrorWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.descripcionLabel = new System.Windows.Forms.Label();
            this.descripcion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.exception = new System.Windows.Forms.TextBox();
            this.buttonCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // descripcionLabel
            // 
            this.descripcionLabel.AutoSize = true;
            this.descripcionLabel.Location = new System.Drawing.Point(12, 9);
            this.descripcionLabel.Name = "descripcionLabel";
            this.descripcionLabel.Size = new System.Drawing.Size(66, 13);
            this.descripcionLabel.TabIndex = 0;
            this.descripcionLabel.Text = "Descripcion:";
            // 
            // descripcion
            // 
            this.descripcion.Location = new System.Drawing.Point(12, 25);
            this.descripcion.Multiline = true;
            this.descripcion.Name = "descripcion";
            this.descripcion.ReadOnly = true;
            this.descripcion.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descripcion.Size = new System.Drawing.Size(568, 65);
            this.descripcion.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Exception:";
            // 
            // exception
            // 
            this.exception.Location = new System.Drawing.Point(12, 118);
            this.exception.Multiline = true;
            this.exception.Name = "exception";
            this.exception.ReadOnly = true;
            this.exception.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.exception.Size = new System.Drawing.Size(568, 316);
            this.exception.TabIndex = 3;
            // 
            // buttonCerrar
            // 
            this.buttonCerrar.Location = new System.Drawing.Point(505, 440);
            this.buttonCerrar.Name = "buttonCerrar";
            this.buttonCerrar.Size = new System.Drawing.Size(75, 23);
            this.buttonCerrar.TabIndex = 4;
            this.buttonCerrar.Text = "Cerrar";
            this.buttonCerrar.UseVisualStyleBackColor = true;
            this.buttonCerrar.Click += new System.EventHandler(this.buttonCerrar_Click);
            // 
            // ErrorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 466);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCerrar);
            this.Controls.Add(this.exception);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.descripcion);
            this.Controls.Add(this.descripcionLabel);
            this.Name = "ErrorWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label descripcionLabel;
        private System.Windows.Forms.TextBox descripcion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox exception;
        private System.Windows.Forms.Button buttonCerrar;
    }
}