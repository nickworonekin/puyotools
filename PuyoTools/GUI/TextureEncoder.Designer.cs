namespace PuyoTools.GUI
{
    partial class TextureEncoder
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
            this.textureFormatBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textureSettingsPanel = new System.Windows.Forms.Panel();
            this.compressionFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.deleteSourceButton = new System.Windows.Forms.CheckBox();
            this.outputToSourceDirButton = new System.Windows.Forms.CheckBox();
            this.settingsPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.tabControl1);
            this.settingsPanel.Controls.Add(this.textureFormatBox);
            this.settingsPanel.Controls.Add(this.label1);
            this.settingsPanel.Size = new System.Drawing.Size(584, 362);
            // 
            // runButton
            // 
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // textureFormatBox
            // 
            this.textureFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textureFormatBox.FormattingEnabled = true;
            this.textureFormatBox.Items.AddRange(new object[] {
            "Select a format"});
            this.textureFormatBox.Location = new System.Drawing.Point(94, 11);
            this.textureFormatBox.Name = "textureFormatBox";
            this.textureFormatBox.Size = new System.Drawing.Size(150, 21);
            this.textureFormatBox.TabIndex = 4;
            this.textureFormatBox.SelectedIndexChanged += new System.EventHandler(this.textureFormatBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Texture Format";
            // 
            // textureSettingsPanel
            // 
            this.textureSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureSettingsPanel.Location = new System.Drawing.Point(3, 3);
            this.textureSettingsPanel.Name = "textureSettingsPanel";
            this.textureSettingsPanel.Size = new System.Drawing.Size(545, 279);
            this.textureSettingsPanel.TabIndex = 8;
            // 
            // compressionFormatBox
            // 
            this.compressionFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compressionFormatBox.FormattingEnabled = true;
            this.compressionFormatBox.Items.AddRange(new object[] {
            "No"});
            this.compressionFormatBox.Location = new System.Drawing.Point(104, 6);
            this.compressionFormatBox.Name = "compressionFormatBox";
            this.compressionFormatBox.Size = new System.Drawing.Size(150, 21);
            this.compressionFormatBox.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Compress Texture";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(13, 38);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(559, 311);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textureSettingsPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(551, 285);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Format Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.deleteSourceButton);
            this.tabPage2.Controls.Add(this.outputToSourceDirButton);
            this.tabPage2.Controls.Add(this.compressionFormatBox);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(551, 285);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "General Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // deleteSourceButton
            // 
            this.deleteSourceButton.AutoSize = true;
            this.deleteSourceButton.Location = new System.Drawing.Point(9, 57);
            this.deleteSourceButton.Name = "deleteSourceButton";
            this.deleteSourceButton.Size = new System.Drawing.Size(196, 17);
            this.deleteSourceButton.TabIndex = 9;
            this.deleteSourceButton.Text = "Delete source texture (upon sucess)";
            this.deleteSourceButton.UseVisualStyleBackColor = true;
            // 
            // outputToSourceDirButton
            // 
            this.outputToSourceDirButton.AutoSize = true;
            this.outputToSourceDirButton.Location = new System.Drawing.Point(9, 33);
            this.outputToSourceDirButton.Name = "outputToSourceDirButton";
            this.outputToSourceDirButton.Size = new System.Drawing.Size(261, 17);
            this.outputToSourceDirButton.TabIndex = 8;
            this.outputToSourceDirButton.Text = "Output to the same directory as the source texture";
            this.outputToSourceDirButton.UseVisualStyleBackColor = true;
            // 
            // TextureEncoder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 462);
            this.Name = "TextureEncoder";
            this.Text = "Texture Encoder";
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox textureFormatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox compressionFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel textureSettingsPanel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox deleteSourceButton;
        private System.Windows.Forms.CheckBox outputToSourceDirButton;


    }
}
