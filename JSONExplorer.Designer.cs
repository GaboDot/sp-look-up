namespace SPLookUp
{
    partial class JSONWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JSONWindow));
            this.jsonExplorer = new System.Windows.Forms.TreeView();
            this.excel = new System.Windows.Forms.Button();
            this.labTotal = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.jsonExport = new System.Windows.Forms.Button();
            this.saveJson = new System.Windows.Forms.SaveFileDialog();
            this.saveExcel = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // jsonExplorer
            // 
            this.jsonExplorer.Dock = System.Windows.Forms.DockStyle.Left;
            this.jsonExplorer.Location = new System.Drawing.Point(0, 0);
            this.jsonExplorer.Name = "jsonExplorer";
            this.jsonExplorer.Size = new System.Drawing.Size(599, 641);
            this.jsonExplorer.TabIndex = 0;
            // 
            // excel
            // 
            this.excel.Image = ((System.Drawing.Image)(resources.GetObject("excel.Image")));
            this.excel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.excel.Location = new System.Drawing.Point(605, 12);
            this.excel.Name = "excel";
            this.excel.Size = new System.Drawing.Size(117, 23);
            this.excel.TabIndex = 1;
            this.excel.Text = "Excel";
            this.excel.UseVisualStyleBackColor = true;
            this.excel.Click += new System.EventHandler(this.excel_Click);
            // 
            // labTotal
            // 
            this.labTotal.AutoSize = true;
            this.labTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labTotal.Location = new System.Drawing.Point(605, 76);
            this.labTotal.Name = "labTotal";
            this.labTotal.Size = new System.Drawing.Size(58, 13);
            this.labTotal.TabIndex = 2;
            this.labTotal.Text = "Total SP\'s:";
            // 
            // labTime
            // 
            this.labTime.AutoSize = true;
            this.labTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labTime.Location = new System.Drawing.Point(605, 95);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(33, 13);
            this.labTime.TabIndex = 3;
            this.labTime.Text = "Time:";
            // 
            // jsonExport
            // 
            this.jsonExport.Image = ((System.Drawing.Image)(resources.GetObject("jsonExport.Image")));
            this.jsonExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.jsonExport.Location = new System.Drawing.Point(608, 42);
            this.jsonExport.Name = "jsonExport";
            this.jsonExport.Size = new System.Drawing.Size(114, 23);
            this.jsonExport.TabIndex = 4;
            this.jsonExport.Text = "JSON";
            this.jsonExport.UseVisualStyleBackColor = true;
            this.jsonExport.Click += new System.EventHandler(this.jsonExport_Click);
            // 
            // saveJson
            // 
            this.saveJson.DefaultExt = "json";
            this.saveJson.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFile_FileOk);
            // 
            // saveExcel
            // 
            this.saveExcel.DefaultExt = "xlsx";
            this.saveExcel.FileOk += new System.ComponentModel.CancelEventHandler(this.saveExcel_FileOk);
            // 
            // JSONWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 641);
            this.Controls.Add(this.jsonExport);
            this.Controls.Add(this.labTime);
            this.Controls.Add(this.labTotal);
            this.Controls.Add(this.excel);
            this.Controls.Add(this.jsonExplorer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "JSONWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JSON Explorer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView jsonExplorer;
        private System.Windows.Forms.Button excel;
        private System.Windows.Forms.Label labTotal;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Button jsonExport;
        private System.Windows.Forms.SaveFileDialog saveJson;
        private System.Windows.Forms.SaveFileDialog saveExcel;
    }
}