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
            this.panel3 = new System.Windows.Forms.Panel();
            this.textureSettingsPanel = new System.Windows.Forms.Panel();
            this.compressionFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.settingsPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.panel3);
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
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.textureSettingsPanel);
            this.panel3.Controls.Add(this.compressionFormatBox);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(13, 38);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(3);
            this.panel3.Size = new System.Drawing.Size(559, 311);
            this.panel3.TabIndex = 5;
            // 
            // textureSettingsPanel
            // 
            this.textureSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureSettingsPanel.Location = new System.Drawing.Point(3, 33);
            this.textureSettingsPanel.Name = "textureSettingsPanel";
            this.textureSettingsPanel.Size = new System.Drawing.Size(551, 273);
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
            // TextureEncoder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 462);
            this.Name = "TextureEncoder";
            this.Text = "Texture Encoder";
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox textureFormatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox compressionFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel textureSettingsPanel;


    }
}
