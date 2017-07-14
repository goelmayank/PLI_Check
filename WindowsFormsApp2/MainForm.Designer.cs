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
            this.parentDirectoryInput = new System.Windows.Forms.TextBox();
            this.targetDirectoryInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.UpdatePath = new System.Windows.Forms.Button();
            this.UpdateTargetPath = new System.Windows.Forms.Button();
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
            // parentDirectoryInput
            // 
            this.parentDirectoryInput.Location = new System.Drawing.Point(95, 27);
            this.parentDirectoryInput.Name = "parentDirectoryInput";
            this.parentDirectoryInput.Size = new System.Drawing.Size(183, 20);
            this.parentDirectoryInput.TabIndex = 22;
            this.parentDirectoryInput.Text = "C:\\Users\\INMAGOE1\\Documents\\Visual Studio 2017\\Projects\\WindowsFormsApp2\\Database" +
    "";
            // 
            // targetDirectoryInput
            // 
            this.targetDirectoryInput.Location = new System.Drawing.Point(95, 53);
            this.targetDirectoryInput.Name = "targetDirectoryInput";
            this.targetDirectoryInput.Size = new System.Drawing.Size(183, 20);
            this.targetDirectoryInput.TabIndex = 23;
            this.targetDirectoryInput.Text = "C:\\Users\\INMAGOE1\\Documents\\Visual Studio 2017\\Projects\\WindowsFormsApp2\\Database" +
    "\\test";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Parent Directory";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Target Directory";
            // 
            // UpdatePath
            // 
            this.UpdatePath.Location = new System.Drawing.Point(284, 25);
            this.UpdatePath.Name = "UpdatePath";
            this.UpdatePath.Size = new System.Drawing.Size(25, 20);
            this.UpdatePath.TabIndex = 26;
            this.UpdatePath.Text = "...";
            this.UpdatePath.UseVisualStyleBackColor = true;
            this.UpdatePath.Click += new System.EventHandler(this.UpdatePath_Click);
            // 
            // UpdateTargetPath
            // 
            this.UpdateTargetPath.Location = new System.Drawing.Point(284, 51);
            this.UpdateTargetPath.Name = "UpdateTargetPath";
            this.UpdateTargetPath.Size = new System.Drawing.Size(25, 20);
            this.UpdateTargetPath.TabIndex = 27;
            this.UpdateTargetPath.Text = "...";
            this.UpdateTargetPath.UseVisualStyleBackColor = true;
            this.UpdateTargetPath.Click += new System.EventHandler(this.UpdateTargetPath_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 84);
            this.Controls.Add(this.UpdateTargetPath);
            this.Controls.Add(this.UpdatePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.targetDirectoryInput);
            this.Controls.Add(this.parentDirectoryInput);
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
        private System.Windows.Forms.TextBox parentDirectoryInput;
        private System.Windows.Forms.TextBox targetDirectoryInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button UpdatePath;
        private System.Windows.Forms.Button UpdateTargetPath;
    }
}

