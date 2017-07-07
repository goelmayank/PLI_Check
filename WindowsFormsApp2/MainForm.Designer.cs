namespace WindowsFormsApp2
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixedPipeDimension = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.parentDirectoryInput = new System.Windows.Forms.TextBox();
            this.targetDirectoryInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.UpdatePath = new System.Windows.Forms.Button();
            this.UpdateTargetPath = new System.Windows.Forms.Button();
            this.fixedLineWidth = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.checkToolStripMenuItem,
            this.rectifyToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(315, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // checkToolStripMenuItem
            // 
            this.checkToolStripMenuItem.Name = "checkToolStripMenuItem";
            this.checkToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.checkToolStripMenuItem.Text = "Check ";
            this.checkToolStripMenuItem.Click += new System.EventHandler(this.checkToolStripMenuItem_Click);
            // 
            // rectifyToolStripMenuItem
            // 
            this.rectifyToolStripMenuItem.Name = "rectifyToolStripMenuItem";
            this.rectifyToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.rectifyToolStripMenuItem.Text = "Rectify";
            this.rectifyToolStripMenuItem.Click += new System.EventHandler(this.rectifyToolStripMenuItem_Click);
            // 
            // fixedPipeDimension
            // 
            this.fixedPipeDimension.Location = new System.Drawing.Point(123, 31);
            this.fixedPipeDimension.Name = "fixedPipeDimension";
            this.fixedPipeDimension.Size = new System.Drawing.Size(30, 20);
            this.fixedPipeDimension.TabIndex = 21;
            this.fixedPipeDimension.Text = "7";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Fixed Pipe Dimension";
            // 
            // parentDirectoryInput
            // 
            this.parentDirectoryInput.Location = new System.Drawing.Point(95, 57);
            this.parentDirectoryInput.Name = "parentDirectoryInput";
            this.parentDirectoryInput.Size = new System.Drawing.Size(183, 20);
            this.parentDirectoryInput.TabIndex = 22;
            this.parentDirectoryInput.Text = "C:\\Users\\INMAGOE1\\Documents\\Visual Studio 2017\\Projects\\WindowsFormsApp2\\Database" +
    "";
            // 
            // targetDirectoryInput
            // 
            this.targetDirectoryInput.Location = new System.Drawing.Point(95, 83);
            this.targetDirectoryInput.Name = "targetDirectoryInput";
            this.targetDirectoryInput.Size = new System.Drawing.Size(183, 20);
            this.targetDirectoryInput.TabIndex = 23;
            this.targetDirectoryInput.Text = "C:\\Users\\INMAGOE1\\Documents\\Visual Studio 2017\\Projects\\WindowsFormsApp2\\Database" +
    "\\test";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Parent Directory";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Target Directory";
            // 
            // UpdatePath
            // 
            this.UpdatePath.Location = new System.Drawing.Point(284, 55);
            this.UpdatePath.Name = "UpdatePath";
            this.UpdatePath.Size = new System.Drawing.Size(25, 23);
            this.UpdatePath.TabIndex = 26;
            this.UpdatePath.Text = "...";
            this.UpdatePath.UseVisualStyleBackColor = true;
            this.UpdatePath.Click += new System.EventHandler(this.UpdatePath_Click);
            // 
            // UpdateTargetPath
            // 
            this.UpdateTargetPath.Location = new System.Drawing.Point(284, 81);
            this.UpdateTargetPath.Name = "UpdateTargetPath";
            this.UpdateTargetPath.Size = new System.Drawing.Size(25, 23);
            this.UpdateTargetPath.TabIndex = 27;
            this.UpdateTargetPath.Text = "...";
            this.UpdateTargetPath.UseVisualStyleBackColor = true;
            this.UpdateTargetPath.Click += new System.EventHandler(this.UpdateTargetPath_Click);
            // 
            // fixedLineWidth
            // 
            this.fixedLineWidth.Location = new System.Drawing.Point(272, 31);
            this.fixedLineWidth.Name = "fixedLineWidth";
            this.fixedLineWidth.Size = new System.Drawing.Size(31, 20);
            this.fixedLineWidth.TabIndex = 29;
            this.fixedLineWidth.Text = "3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Fixed Line Width";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 107);
            this.Controls.Add(this.fixedLineWidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.UpdateTargetPath);
            this.Controls.Add(this.UpdatePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.targetDirectoryInput);
            this.Controls.Add(this.parentDirectoryInput);
            this.Controls.Add(this.fixedPipeDimension);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Pipes And Lines Intersection Check";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rectifyToolStripMenuItem;
        private System.Windows.Forms.TextBox fixedPipeDimension;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox parentDirectoryInput;
        private System.Windows.Forms.TextBox targetDirectoryInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button UpdatePath;
        private System.Windows.Forms.Button UpdateTargetPath;
        private System.Windows.Forms.TextBox fixedLineWidth;
        private System.Windows.Forms.Label label4;
    }
}

