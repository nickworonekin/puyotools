namespace PuyoTools.GUI
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.decompressToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.compressToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.texturesButton = new System.Windows.Forms.Button();
            this.archivesButton = new System.Windows.Forms.Button();
            this.compressionButton = new System.Windows.Forms.Button();
            this.archivesMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.texturesMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.decodeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.encodeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.compressionMenuStrip.SuspendLayout();
            this.archivesMenuStrip.SuspendLayout();
            this.texturesMenuStrip.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(420, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // compressionMenuStrip
            // 
            this.compressionMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decompressToolStripMenuItem1,
            this.compressToolStripMenuItem1});
            this.compressionMenuStrip.Name = "compressionMenuStrip";
            this.compressionMenuStrip.Size = new System.Drawing.Size(140, 48);
            // 
            // decompressToolStripMenuItem1
            // 
            this.decompressToolStripMenuItem1.Name = "decompressToolStripMenuItem1";
            this.decompressToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.decompressToolStripMenuItem1.Text = "Decompress";
            this.decompressToolStripMenuItem1.Click += new System.EventHandler(this.decompressToolStripMenuItem1_Click);
            // 
            // compressToolStripMenuItem1
            // 
            this.compressToolStripMenuItem1.Name = "compressToolStripMenuItem1";
            this.compressToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.compressToolStripMenuItem1.Text = "Compress";
            this.compressToolStripMenuItem1.Click += new System.EventHandler(this.compressToolStripMenuItem1_Click);
            // 
            // texturesButton
            // 
            this.texturesButton.Image = global::PuyoTools.BitmapResources.TextureIconDark;
            this.texturesButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.texturesButton.Location = new System.Drawing.Point(290, 10);
            this.texturesButton.Margin = new System.Windows.Forms.Padding(10);
            this.texturesButton.Name = "texturesButton";
            this.texturesButton.Padding = new System.Windows.Forms.Padding(10);
            this.texturesButton.Size = new System.Drawing.Size(120, 120);
            this.texturesButton.TabIndex = 4;
            this.texturesButton.TabStop = false;
            this.texturesButton.Text = "Textures";
            this.texturesButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.texturesButton.UseVisualStyleBackColor = true;
            this.texturesButton.Click += new System.EventHandler(this.texturesButton_Click);
            // 
            // archivesButton
            // 
            this.archivesButton.Image = global::PuyoTools.BitmapResources.ArchiveIconDark;
            this.archivesButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.archivesButton.Location = new System.Drawing.Point(150, 10);
            this.archivesButton.Margin = new System.Windows.Forms.Padding(10);
            this.archivesButton.Name = "archivesButton";
            this.archivesButton.Padding = new System.Windows.Forms.Padding(10);
            this.archivesButton.Size = new System.Drawing.Size(120, 120);
            this.archivesButton.TabIndex = 3;
            this.archivesButton.TabStop = false;
            this.archivesButton.Text = "Archives";
            this.archivesButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.archivesButton.UseVisualStyleBackColor = true;
            this.archivesButton.Click += new System.EventHandler(this.archivesButton_Click);
            // 
            // compressionButton
            // 
            this.compressionButton.Image = global::PuyoTools.BitmapResources.CompressIconDark;
            this.compressionButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.compressionButton.Location = new System.Drawing.Point(10, 10);
            this.compressionButton.Margin = new System.Windows.Forms.Padding(10);
            this.compressionButton.Name = "compressionButton";
            this.compressionButton.Padding = new System.Windows.Forms.Padding(10);
            this.compressionButton.Size = new System.Drawing.Size(120, 120);
            this.compressionButton.TabIndex = 1;
            this.compressionButton.TabStop = false;
            this.compressionButton.Text = "Compression";
            this.compressionButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.compressionButton.UseVisualStyleBackColor = true;
            this.compressionButton.Click += new System.EventHandler(this.compressionButton_Click);
            // 
            // archivesMenuStrip
            // 
            this.archivesMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToolStripMenuItem1,
            this.createToolStripMenuItem1,
            this.toolStripSeparator3,
            this.toolStripMenuItem1});
            this.archivesMenuStrip.Name = "archivesMenuStrip";
            this.archivesMenuStrip.Size = new System.Drawing.Size(118, 76);
            // 
            // extractToolStripMenuItem1
            // 
            this.extractToolStripMenuItem1.Name = "extractToolStripMenuItem1";
            this.extractToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.extractToolStripMenuItem1.Text = "Extract";
            this.extractToolStripMenuItem1.Click += new System.EventHandler(this.extractToolStripMenuItem1_Click);
            // 
            // createToolStripMenuItem1
            // 
            this.createToolStripMenuItem1.Name = "createToolStripMenuItem1";
            this.createToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.createToolStripMenuItem1.Text = "Create";
            this.createToolStripMenuItem1.Click += new System.EventHandler(this.createToolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(114, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem1.Text = "Explorer";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // texturesMenuStrip
            // 
            this.texturesMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decodeToolStripMenuItem1,
            this.encodeToolStripMenuItem1,
            this.toolStripSeparator4,
            this.toolStripMenuItem2});
            this.texturesMenuStrip.Name = "texturesMenuStrip";
            this.texturesMenuStrip.Size = new System.Drawing.Size(115, 76);
            // 
            // decodeToolStripMenuItem1
            // 
            this.decodeToolStripMenuItem1.Name = "decodeToolStripMenuItem1";
            this.decodeToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.decodeToolStripMenuItem1.Text = "Decode";
            this.decodeToolStripMenuItem1.Click += new System.EventHandler(this.decodeToolStripMenuItem1_Click);
            // 
            // encodeToolStripMenuItem1
            // 
            this.encodeToolStripMenuItem1.Name = "encodeToolStripMenuItem1";
            this.encodeToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.encodeToolStripMenuItem1.Text = "Encode";
            this.encodeToolStripMenuItem1.Click += new System.EventHandler(this.encodeToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(111, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(114, 22);
            this.toolStripMenuItem2.Text = "Viewer";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.compressionButton);
            this.flowLayoutPanel1.Controls.Add(this.archivesButton);
            this.flowLayoutPanel1.Controls.Add(this.texturesButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(420, 139);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 163);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Puyo Tools";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.compressionMenuStrip.ResumeLayout(false);
            this.archivesMenuStrip.ResumeLayout(false);
            this.texturesMenuStrip.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button compressionButton;
        private System.Windows.Forms.ContextMenuStrip compressionMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem compressToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem decompressToolStripMenuItem1;
        private System.Windows.Forms.Button archivesButton;
        private System.Windows.Forms.Button texturesButton;
        private System.Windows.Forms.ContextMenuStrip archivesMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip texturesMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem decodeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem encodeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}