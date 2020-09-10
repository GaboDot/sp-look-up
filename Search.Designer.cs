namespace SPLookUp
{
    partial class SearchWindow
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchWindow));
            this.nombreSP = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ipTXT = new System.Windows.Forms.TextBox();
            this.usrTXT = new System.Windows.Forms.TextBox();
            this.pwdTXT = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dbTXT = new System.Windows.Forms.TextBox();
            this.loadingGif = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.loadingGif)).BeginInit();
            this.SuspendLayout();
            // 
            // nombreSP
            // 
            this.nombreSP.Location = new System.Drawing.Point(97, 110);
            this.nombreSP.Name = "nombreSP";
            this.nombreSP.Size = new System.Drawing.Size(162, 20);
            this.nombreSP.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(265, 108);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 23);
            this.button1.TabIndex = 5;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP / Hostname:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Usr:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password:";
            // 
            // ipTXT
            // 
            this.ipTXT.Location = new System.Drawing.Point(97, 6);
            this.ipTXT.Name = "ipTXT";
            this.ipTXT.Size = new System.Drawing.Size(194, 20);
            this.ipTXT.TabIndex = 0;
            // 
            // usrTXT
            // 
            this.usrTXT.Location = new System.Drawing.Point(97, 57);
            this.usrTXT.Name = "usrTXT";
            this.usrTXT.Size = new System.Drawing.Size(194, 20);
            this.usrTXT.TabIndex = 2;
            // 
            // pwdTXT
            // 
            this.pwdTXT.Location = new System.Drawing.Point(97, 83);
            this.pwdTXT.Name = "pwdTXT";
            this.pwdTXT.PasswordChar = '•';
            this.pwdTXT.Size = new System.Drawing.Size(194, 20);
            this.pwdTXT.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "SP:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Database:";
            // 
            // dbTXT
            // 
            this.dbTXT.Location = new System.Drawing.Point(97, 31);
            this.dbTXT.Name = "dbTXT";
            this.dbTXT.Size = new System.Drawing.Size(194, 20);
            this.dbTXT.TabIndex = 1;
            // 
            // loadingGif
            // 
            this.loadingGif.Image = ((System.Drawing.Image)(resources.GetObject("loadingGif.Image")));
            this.loadingGif.Location = new System.Drawing.Point(12, 136);
            this.loadingGif.Name = "loadingGif";
            this.loadingGif.Size = new System.Drawing.Size(279, 34);
            this.loadingGif.TabIndex = 10;
            this.loadingGif.TabStop = false;
            // 
            // SearchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 178);
            this.Controls.Add(this.loadingGif);
            this.Controls.Add(this.dbTXT);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pwdTXT);
            this.Controls.Add(this.usrTXT);
            this.Controls.Add(this.ipTXT);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.nombreSP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar SP Internos";
            ((System.ComponentModel.ISupportInitialize)(this.loadingGif)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nombreSP;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ipTXT;
        private System.Windows.Forms.TextBox usrTXT;
        private System.Windows.Forms.TextBox pwdTXT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox dbTXT;
        private System.Windows.Forms.PictureBox loadingGif;
    }
}

