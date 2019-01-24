namespace PowerediOXDailySales
{
    partial class SplashScreen
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
            this.ProgressLoader = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.ProgressLabel = new DevComponents.DotNetBar.LabelX();
            this.DummyProgress = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.SuspendLayout();
            // 
            // ProgressLoader
            // 
            // 
            // 
            // 
            this.ProgressLoader.BackgroundStyle.CornerDiameter = 20;
            this.ProgressLoader.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.ProgressLoader.BackgroundStyle.CornerTypeBottomLeft = DevComponents.DotNetBar.eCornerType.Rounded;
            this.ProgressLoader.BackgroundStyle.CornerTypeBottomRight = DevComponents.DotNetBar.eCornerType.Rounded;
            this.ProgressLoader.BackgroundStyle.CornerTypeTopLeft = DevComponents.DotNetBar.eCornerType.Rounded;
            this.ProgressLoader.BackgroundStyle.CornerTypeTopRight = DevComponents.DotNetBar.eCornerType.Rounded;
            this.ProgressLoader.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ProgressLoader.Location = new System.Drawing.Point(0, 398);
            this.ProgressLoader.Name = "ProgressLoader";
            this.ProgressLoader.Size = new System.Drawing.Size(647, 23);
            this.ProgressLoader.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ProgressLoader.TabIndex = 0;
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelX1.Location = new System.Drawing.Point(496, 0);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(151, 28);
            this.labelX1.Symbol = "";
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "JJETSystems";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelX2.Location = new System.Drawing.Point(0, 0);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(146, 28);
            this.labelX2.Symbol = "";
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "Powered iOX";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelX3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelX3.Location = new System.Drawing.Point(223, 310);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(200, 28);
            this.labelX3.Symbol = "";
            this.labelX3.TabIndex = 4;
            this.labelX3.Text = "Daily Sales System";
            this.labelX3.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            // 
            // 
            // 
            this.ProgressLabel.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ProgressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgressLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ProgressLabel.Location = new System.Drawing.Point(35, 370);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(41, 26);
            this.ProgressLabel.TabIndex = 6;
            this.ProgressLabel.Text = "Test";
            // 
            // DummyProgress
            // 
            this.DummyProgress.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.DummyProgress.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.DummyProgress.Location = new System.Drawing.Point(0, 370);
            this.DummyProgress.MaximumSize = new System.Drawing.Size(50, 50);
            this.DummyProgress.Name = "DummyProgress";
            this.DummyProgress.ProgressBarType = DevComponents.DotNetBar.eCircularProgressType.Donut;
            this.DummyProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.DummyProgress.Size = new System.Drawing.Size(40, 28);
            this.DummyProgress.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.DummyProgress.TabIndex = 7;
            // 
            // SplashScreen
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BackgroundImage = global::PowerediOXDailySales.Properties.Resources._12767268_10205415957863888_1665001525_n;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BottomLeftCornerSize = 5;
            this.BottomRightCornerSize = 5;
            this.ClientSize = new System.Drawing.Size(647, 421);
            this.ControlBox = false;
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.DummyProgress);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.ProgressLoader);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ProgressBarX ProgressLoader;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX ProgressLabel;
        private DevComponents.DotNetBar.Controls.CircularProgress DummyProgress;
    }
}