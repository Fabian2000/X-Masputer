namespace X_Masputer
{
    partial class Preview
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
            this.AlwaysOnTop = new System.Windows.Forms.Timer(this.components);
            this.LightPB = new System.Windows.Forms.PictureBox();
            this.ColorChanger = new System.Windows.Forms.Timer(this.components);
            this.WindowDetection = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.LightPB)).BeginInit();
            this.SuspendLayout();
            // 
            // AlwaysOnTop
            // 
            this.AlwaysOnTop.Interval = 1;
            this.AlwaysOnTop.Tick += new System.EventHandler(this.AlwaysOnTop_Tick);
            // 
            // LightPB
            // 
            this.LightPB.BackColor = System.Drawing.Color.DimGray;
            this.LightPB.Image = ((System.Drawing.Image)(resources.GetObject("LightPB.Image")));
            this.LightPB.Location = new System.Drawing.Point(0, 0);
            this.LightPB.Name = "LightPB";
            this.LightPB.Size = new System.Drawing.Size(25, 20);
            this.LightPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.LightPB.TabIndex = 0;
            this.LightPB.TabStop = false;
            // 
            // ColorChanger
            // 
            this.ColorChanger.Enabled = true;
            this.ColorChanger.Interval = 1000;
            this.ColorChanger.Tick += new System.EventHandler(this.ColorChanger_Tick);
            // 
            // WindowDetection
            // 
            this.WindowDetection.Enabled = true;
            this.WindowDetection.Interval = 2500;
            this.WindowDetection.Tick += new System.EventHandler(this.WindowDetection_Tick);
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(188, 37);
            this.ControlBox = false;
            this.Controls.Add(this.LightPB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.DimGray;
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.LightPB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox LightPB;
        private System.Windows.Forms.Timer ColorChanger;
        private System.Windows.Forms.Timer AlwaysOnTop;
        private System.Windows.Forms.Timer WindowDetection;
    }
}

