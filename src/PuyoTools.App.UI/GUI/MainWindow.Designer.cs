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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            compressionMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            decompressToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            compressToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            texturesButton = new System.Windows.Forms.Button();
            archivesButton = new System.Windows.Forms.Button();
            compressionButton = new System.Windows.Forms.Button();
            archivesMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            extractToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            createToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            texturesMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            decodeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            encodeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            menuStrip1.SuspendLayout();
            compressionMenuStrip.SuspendLayout();
            archivesMenuStrip.SuspendLayout();
            texturesMenuStrip.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(492, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // compressionMenuStrip
            // 
            compressionMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { decompressToolStripMenuItem1, compressToolStripMenuItem1 });
            compressionMenuStrip.Name = "compressionMenuStrip";
            compressionMenuStrip.Size = new System.Drawing.Size(140, 48);
            // 
            // decompressToolStripMenuItem1
            // 
            decompressToolStripMenuItem1.Name = "decompressToolStripMenuItem1";
            decompressToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            decompressToolStripMenuItem1.Text = "Decompress";
            decompressToolStripMenuItem1.Click += decompressToolStripMenuItem1_Click;
            // 
            // compressToolStripMenuItem1
            // 
            compressToolStripMenuItem1.Name = "compressToolStripMenuItem1";
            compressToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            compressToolStripMenuItem1.Text = "Compress";
            compressToolStripMenuItem1.Click += compressToolStripMenuItem1_Click;
            // 
            // texturesButton
            // 
            texturesButton.Image = (System.Drawing.Image)resources.GetObject("texturesButton.Image");
            texturesButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            texturesButton.Location = new System.Drawing.Point(340, 12);
            texturesButton.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            texturesButton.Name = "texturesButton";
            texturesButton.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            texturesButton.Size = new System.Drawing.Size(140, 138);
            texturesButton.TabIndex = 4;
            texturesButton.TabStop = false;
            texturesButton.Text = "Textures";
            texturesButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            texturesButton.UseVisualStyleBackColor = true;
            texturesButton.Click += texturesButton_Click;
            // 
            // archivesButton
            // 
            archivesButton.Image = (System.Drawing.Image)resources.GetObject("archivesButton.Image");
            archivesButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            archivesButton.Location = new System.Drawing.Point(176, 12);
            archivesButton.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            archivesButton.Name = "archivesButton";
            archivesButton.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            archivesButton.Size = new System.Drawing.Size(140, 138);
            archivesButton.TabIndex = 3;
            archivesButton.TabStop = false;
            archivesButton.Text = "Archives";
            archivesButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            archivesButton.UseVisualStyleBackColor = true;
            archivesButton.Click += archivesButton_Click;
            // 
            // compressionButton
            // 
            compressionButton.Image = (System.Drawing.Image)resources.GetObject("compressionButton.Image");
            compressionButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            compressionButton.Location = new System.Drawing.Point(12, 12);
            compressionButton.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            compressionButton.Name = "compressionButton";
            compressionButton.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            compressionButton.Size = new System.Drawing.Size(140, 138);
            compressionButton.TabIndex = 1;
            compressionButton.TabStop = false;
            compressionButton.Text = "Compression";
            compressionButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            compressionButton.UseVisualStyleBackColor = true;
            compressionButton.Click += compressionButton_Click;
            // 
            // archivesMenuStrip
            // 
            archivesMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { extractToolStripMenuItem1, createToolStripMenuItem1, toolStripSeparator3, toolStripMenuItem1 });
            archivesMenuStrip.Name = "archivesMenuStrip";
            archivesMenuStrip.Size = new System.Drawing.Size(118, 76);
            // 
            // extractToolStripMenuItem1
            // 
            extractToolStripMenuItem1.Name = "extractToolStripMenuItem1";
            extractToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            extractToolStripMenuItem1.Text = "Extract";
            extractToolStripMenuItem1.Click += extractToolStripMenuItem1_Click;
            // 
            // createToolStripMenuItem1
            // 
            createToolStripMenuItem1.Name = "createToolStripMenuItem1";
            createToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            createToolStripMenuItem1.Text = "Create";
            createToolStripMenuItem1.Click += createToolStripMenuItem1_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(114, 6);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            toolStripMenuItem1.Text = "Explorer";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // texturesMenuStrip
            // 
            texturesMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { decodeToolStripMenuItem1, encodeToolStripMenuItem1, toolStripSeparator4, toolStripMenuItem2 });
            texturesMenuStrip.Name = "texturesMenuStrip";
            texturesMenuStrip.Size = new System.Drawing.Size(115, 76);
            // 
            // decodeToolStripMenuItem1
            // 
            decodeToolStripMenuItem1.Name = "decodeToolStripMenuItem1";
            decodeToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            decodeToolStripMenuItem1.Text = "Decode";
            decodeToolStripMenuItem1.Click += decodeToolStripMenuItem1_Click;
            // 
            // encodeToolStripMenuItem1
            // 
            encodeToolStripMenuItem1.Name = "encodeToolStripMenuItem1";
            encodeToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            encodeToolStripMenuItem1.Text = "Encode";
            encodeToolStripMenuItem1.Click += encodeToolStripMenuItem1_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(111, 6);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(114, 22);
            toolStripMenuItem2.Text = "Viewer";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(compressionButton);
            flowLayoutPanel1.Controls.Add(archivesButton);
            flowLayoutPanel1.Controls.Add(texturesButton);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(492, 164);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(492, 188);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainWindow";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Puyo Tools";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            compressionMenuStrip.ResumeLayout(false);
            archivesMenuStrip.ResumeLayout(false);
            texturesMenuStrip.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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