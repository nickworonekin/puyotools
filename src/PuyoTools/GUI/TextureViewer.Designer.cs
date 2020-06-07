namespace PuyoTools.GUI
{
    partial class TextureViewer
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
            if (disposing && (textureStream != null))
            {
                textureStream.Dispose();
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textureInfoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textureNameLabel = new System.Windows.Forms.Label();
            this.textureDimensionsLabel = new System.Windows.Forms.Label();
            this.textureFormatLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.texturePanel = new System.Windows.Forms.Panel();
            this.textureDisplay = new System.Windows.Forms.PictureBox();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.textureInfoPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.texturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.textureInfoPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 280);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 80);
            this.panel1.TabIndex = 2;
            // 
            // textureInfoPanel
            // 
            this.textureInfoPanel.ColumnCount = 2;
            this.textureInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.textureInfoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.textureInfoPanel.Controls.Add(this.label1, 0, 0);
            this.textureInfoPanel.Controls.Add(this.label2, 0, 1);
            this.textureInfoPanel.Controls.Add(this.label3, 0, 2);
            this.textureInfoPanel.Controls.Add(this.textureNameLabel, 1, 0);
            this.textureInfoPanel.Controls.Add(this.textureDimensionsLabel, 1, 1);
            this.textureInfoPanel.Controls.Add(this.textureFormatLabel, 1, 2);
            this.textureInfoPanel.Location = new System.Drawing.Point(10, 10);
            this.textureInfoPanel.Margin = new System.Windows.Forms.Padding(10);
            this.textureInfoPanel.Name = "textureInfoPanel";
            this.textureInfoPanel.RowCount = 3;
            this.textureInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.textureInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.textureInfoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.textureInfoPanel.Size = new System.Drawing.Size(564, 60);
            this.textureInfoPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dimensions";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Format";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textureNameLabel
            // 
            this.textureNameLabel.AutoSize = true;
            this.textureNameLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.textureNameLabel.Location = new System.Drawing.Point(77, 0);
            this.textureNameLabel.Name = "textureNameLabel";
            this.textureNameLabel.Size = new System.Drawing.Size(74, 20);
            this.textureNameLabel.TabIndex = 3;
            this.textureNameLabel.Text = "Texture Name";
            this.textureNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textureDimensionsLabel
            // 
            this.textureDimensionsLabel.AutoSize = true;
            this.textureDimensionsLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.textureDimensionsLabel.Location = new System.Drawing.Point(77, 20);
            this.textureDimensionsLabel.Name = "textureDimensionsLabel";
            this.textureDimensionsLabel.Size = new System.Drawing.Size(100, 20);
            this.textureDimensionsLabel.TabIndex = 4;
            this.textureDimensionsLabel.Text = "Texture Dimensions";
            this.textureDimensionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textureFormatLabel
            // 
            this.textureFormatLabel.AutoSize = true;
            this.textureFormatLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.textureFormatLabel.Location = new System.Drawing.Point(77, 40);
            this.textureFormatLabel.Name = "textureFormatLabel";
            this.textureFormatLabel.Size = new System.Drawing.Size(78, 20);
            this.textureFormatLabel.TabIndex = 5;
            this.textureFormatLabel.Text = "Texture Format";
            this.textureFormatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // texturePanel
            // 
            this.texturePanel.AutoScroll = true;
            this.texturePanel.BackColor = System.Drawing.SystemColors.Control;
            this.texturePanel.Controls.Add(this.textureDisplay);
            this.texturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.texturePanel.Location = new System.Drawing.Point(0, 24);
            this.texturePanel.Name = "texturePanel";
            this.texturePanel.Size = new System.Drawing.Size(584, 256);
            this.texturePanel.TabIndex = 4;
            this.texturePanel.ClientSizeChanged += new System.EventHandler(this.texturePanel_ClientSizeChanged);
            // 
            // textureDisplay
            // 
            this.textureDisplay.BackgroundImage = global::PuyoTools.BitmapResources.CheckeredBackgroundLight;
            this.textureDisplay.InitialImage = null;
            this.textureDisplay.Location = new System.Drawing.Point(0, 0);
            this.textureDisplay.Margin = new System.Windows.Forms.Padding(0);
            this.textureDisplay.Name = "textureDisplay";
            this.textureDisplay.Size = new System.Drawing.Size(128, 128);
            this.textureDisplay.TabIndex = 0;
            this.textureDisplay.TabStop = false;
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lightBackgroundToolStripMenuItem,
            this.darkBackgroundToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // lightBackgroundToolStripMenuItem
            // 
            this.lightBackgroundToolStripMenuItem.Checked = true;
            this.lightBackgroundToolStripMenuItem.CheckOnClick = true;
            this.lightBackgroundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lightBackgroundToolStripMenuItem.Name = "lightBackgroundToolStripMenuItem";
            this.lightBackgroundToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.lightBackgroundToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.lightBackgroundToolStripMenuItem.Text = "Light Background";
            this.lightBackgroundToolStripMenuItem.Click += new System.EventHandler(this.lightBackgroundToolStripMenuItem_Click);
            // 
            // darkBackgroundToolStripMenuItem
            // 
            this.darkBackgroundToolStripMenuItem.CheckOnClick = true;
            this.darkBackgroundToolStripMenuItem.Name = "darkBackgroundToolStripMenuItem";
            this.darkBackgroundToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.darkBackgroundToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.darkBackgroundToolStripMenuItem.Text = "Dark Background";
            this.darkBackgroundToolStripMenuItem.Click += new System.EventHandler(this.darkBackgroundToolStripMenuItem_Click);
            // 
            // TextureViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 360);
            this.Controls.Add(this.texturePanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TextureViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Texture Viewer";
            this.panel1.ResumeLayout(false);
            this.textureInfoPanel.ResumeLayout(false);
            this.textureInfoPanel.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.texturePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textureDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel textureInfoPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label textureNameLabel;
        private System.Windows.Forms.Label textureDimensionsLabel;
        private System.Windows.Forms.Label textureFormatLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel texturePanel;
        private System.Windows.Forms.PictureBox textureDisplay;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkBackgroundToolStripMenuItem;
    }
}