namespace Trafico.De.Fauna.Model.Menu.Error
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
            this.descripcion = new System.Windows.Forms.TextBox();
            this.exception = new System.Windows.Forms.TextBox();
            this.buttonCerrar = new System.Windows.Forms.Button();
            this.panelClose = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBoxDescription = new System.Windows.Forms.GroupBox();
            this.groupBoxException = new System.Windows.Forms.GroupBox();
            this.panelClose.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxDescription.SuspendLayout();
            this.groupBoxException.SuspendLayout();
            this.SuspendLayout();
            // 
            // descripcion
            // 
            this.descripcion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descripcion.Location = new System.Drawing.Point(3, 22);
            this.descripcion.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.descripcion.Multiline = true;
            this.descripcion.Name = "descripcion";
            this.descripcion.ReadOnly = true;
            this.descripcion.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descripcion.Size = new System.Drawing.Size(752, 205);
            this.descripcion.TabIndex = 1;
            // 
            // exception
            // 
            this.exception.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exception.Location = new System.Drawing.Point(3, 22);
            this.exception.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.exception.Multiline = true;
            this.exception.Name = "exception";
            this.exception.ReadOnly = true;
            this.exception.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.exception.Size = new System.Drawing.Size(752, 201);
            this.exception.TabIndex = 3;
            // 
            // buttonCerrar
            // 
            this.buttonCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCerrar.Location = new System.Drawing.Point(633, 15);
            this.buttonCerrar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCerrar.Name = "buttonCerrar";
            this.buttonCerrar.Size = new System.Drawing.Size(112, 35);
            this.buttonCerrar.TabIndex = 4;
            this.buttonCerrar.Text = "Cerrar";
            this.buttonCerrar.UseVisualStyleBackColor = true;
            this.buttonCerrar.Click += new System.EventHandler(this.buttonCerrar_Click);
            // 
            // panelClose
            // 
            this.panelClose.Controls.Add(this.buttonCerrar);
            this.panelClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelClose.Location = new System.Drawing.Point(10, 470);
            this.panelClose.Name = "panelClose";
            this.panelClose.Size = new System.Drawing.Size(758, 64);
            this.panelClose.TabIndex = 5;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(10, 10);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBoxDescription);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBoxException);
            this.splitContainer.Size = new System.Drawing.Size(758, 460);
            this.splitContainer.SplitterDistance = 230;
            this.splitContainer.TabIndex = 6;
            // 
            // groupBoxDescription
            // 
            this.groupBoxDescription.Controls.Add(this.descripcion);
            this.groupBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDescription.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDescription.Name = "groupBoxDescription";
            this.groupBoxDescription.Size = new System.Drawing.Size(758, 230);
            this.groupBoxDescription.TabIndex = 2;
            this.groupBoxDescription.TabStop = false;
            this.groupBoxDescription.Text = "Descripcion";
            // 
            // groupBoxException
            // 
            this.groupBoxException.Controls.Add(this.exception);
            this.groupBoxException.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxException.Location = new System.Drawing.Point(0, 0);
            this.groupBoxException.Name = "groupBoxException";
            this.groupBoxException.Size = new System.Drawing.Size(758, 226);
            this.groupBoxException.TabIndex = 4;
            this.groupBoxException.TabStop = false;
            this.groupBoxException.Text = "Exception";
            // 
            // ErrorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 544);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panelClose);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "ErrorWindow";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            this.panelClose.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxDescription.ResumeLayout(false);
            this.groupBoxDescription.PerformLayout();
            this.groupBoxException.ResumeLayout(false);
            this.groupBoxException.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox descripcion;
        private System.Windows.Forms.TextBox exception;
        private System.Windows.Forms.Button buttonCerrar;
        private System.Windows.Forms.Panel panelClose;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBoxDescription;
        private System.Windows.Forms.GroupBox groupBoxException;
    }
}