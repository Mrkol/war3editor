namespace Editor.GUI
{
    partial class EnvironmentEditor
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.sizeTrackBar = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.shapeButton5 = new System.Windows.Forms.Button();
            this.shapeButton4 = new System.Windows.Forms.Button();
            this.shapeButton3 = new System.Windows.Forms.Button();
            this.shapeButton2 = new System.Windows.Forms.Button();
            this.shapeButton1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.envDisplay = new Editor.GUI.EnvironmentDisplayControl();
            this.label2 = new System.Windows.Forms.Label();
            this.intensityTrackBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.valueTrackBar = new System.Windows.Forms.TrackBar();
            this.leftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.intensityTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 659);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1264, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.valueTrackBar);
            this.leftPanel.Controls.Add(this.label3);
            this.leftPanel.Controls.Add(this.intensityTrackBar);
            this.leftPanel.Controls.Add(this.label2);
            this.leftPanel.Controls.Add(this.label1);
            this.leftPanel.Controls.Add(this.sizeTrackBar);
            this.leftPanel.Controls.Add(this.panel1);
            this.leftPanel.Location = new System.Drawing.Point(0, 27);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(200, 631);
            this.leftPanel.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label1.Location = new System.Drawing.Point(3, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Size";
            // 
            // sizeTrackBar
            // 
            this.sizeTrackBar.Location = new System.Drawing.Point(3, 65);
            this.sizeTrackBar.Name = "sizeTrackBar";
            this.sizeTrackBar.Size = new System.Drawing.Size(194, 45);
            this.sizeTrackBar.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.shapeButton5);
            this.panel1.Controls.Add(this.shapeButton4);
            this.panel1.Controls.Add(this.shapeButton3);
            this.panel1.Controls.Add(this.shapeButton2);
            this.panel1.Controls.Add(this.shapeButton1);
            this.panel1.Location = new System.Drawing.Point(10, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 36);
            this.panel1.TabIndex = 0;
            // 
            // shapeButton5
            // 
            this.shapeButton5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.shapeButton5.Location = new System.Drawing.Point(147, 3);
            this.shapeButton5.Name = "shapeButton5";
            this.shapeButton5.Size = new System.Drawing.Size(30, 30);
            this.shapeButton5.TabIndex = 4;
            this.shapeButton5.Text = "/";
            this.shapeButton5.UseVisualStyleBackColor = true;
            // 
            // shapeButton4
            // 
            this.shapeButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.shapeButton4.Location = new System.Drawing.Point(111, 3);
            this.shapeButton4.Name = "shapeButton4";
            this.shapeButton4.Size = new System.Drawing.Size(30, 30);
            this.shapeButton4.TabIndex = 3;
            this.shapeButton4.Text = "R";
            this.shapeButton4.UseVisualStyleBackColor = true;
            // 
            // shapeButton3
            // 
            this.shapeButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.shapeButton3.Location = new System.Drawing.Point(75, 3);
            this.shapeButton3.Name = "shapeButton3";
            this.shapeButton3.Size = new System.Drawing.Size(30, 30);
            this.shapeButton3.TabIndex = 2;
            this.shapeButton3.Text = "◆";
            this.shapeButton3.UseVisualStyleBackColor = true;
            // 
            // shapeButton2
            // 
            this.shapeButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.shapeButton2.Location = new System.Drawing.Point(39, 3);
            this.shapeButton2.Name = "shapeButton2";
            this.shapeButton2.Size = new System.Drawing.Size(30, 30);
            this.shapeButton2.TabIndex = 1;
            this.shapeButton2.Text = "⬤";
            this.shapeButton2.UseVisualStyleBackColor = true;
            // 
            // shapeButton1
            // 
            this.shapeButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.shapeButton1.Location = new System.Drawing.Point(3, 3);
            this.shapeButton1.Name = "shapeButton1";
            this.shapeButton1.Size = new System.Drawing.Size(30, 30);
            this.shapeButton1.TabIndex = 0;
            this.shapeButton1.Text = "■";
            this.shapeButton1.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // envDisplay
            // 
            this.envDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.envDisplay.Cursor = System.Windows.Forms.Cursors.Default;
            this.envDisplay.Location = new System.Drawing.Point(200, 27);
            this.envDisplay.Name = "envDisplay";
            this.envDisplay.Size = new System.Drawing.Size(880, 631);
            this.envDisplay.TabIndex = 10;
            this.envDisplay.Text = "environmentDisplayControl1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label2.Location = new System.Drawing.Point(3, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Intensity";
            // 
            // intensityTrackBar
            // 
            this.intensityTrackBar.Location = new System.Drawing.Point(3, 136);
            this.intensityTrackBar.Name = "intensityTrackBar";
            this.intensityTrackBar.Size = new System.Drawing.Size(194, 45);
            this.intensityTrackBar.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label3.Location = new System.Drawing.Point(3, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Value";
            // 
            // valueTrackBar
            // 
            this.valueTrackBar.Location = new System.Drawing.Point(3, 207);
            this.valueTrackBar.Name = "valueTrackBar";
            this.valueTrackBar.Size = new System.Drawing.Size(194, 45);
            this.valueTrackBar.TabIndex = 6;
            // 
            // EnvironmentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.envDisplay);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EnvironmentEditor";
            this.Text = "EnvironmentEditor";
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.intensityTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private EnvironmentDisplayControl envDisplay;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button shapeButton5;
        private System.Windows.Forms.Button shapeButton4;
        private System.Windows.Forms.Button shapeButton3;
        private System.Windows.Forms.Button shapeButton2;
        private System.Windows.Forms.Button shapeButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar sizeTrackBar;
        private System.Windows.Forms.TrackBar intensityTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar valueTrackBar;
        private System.Windows.Forms.Label label3;
    }
}