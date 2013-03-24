namespace PuyoTools.GUI
{
    partial class TextureDecoder
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
            this.decodeCompressedTexturesButton = new System.Windows.Forms.CheckBox();
            this.outputToSourceDirButton = new System.Windows.Forms.CheckBox();
            this.deleteSourceButton = new System.Windows.Forms.CheckBox();
            this.settingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.deleteSourceButton);
            this.settingsPanel.Controls.Add(this.outputToSourceDirButton);
            this.settingsPanel.Controls.Add(this.decodeCompressedTexturesButton);
            // 
            // runButton
            // 
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // decodeCompressedTexturesButton
            // 
            this.decodeCompressedTexturesButton.AutoSize = true;
            this.decodeCompressedTexturesButton.Checked = true;
            this.decodeCompressedTexturesButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.decodeCompressedTexturesButton.Location = new System.Drawing.Point(14, 14);
            this.decodeCompressedTexturesButton.Name = "decodeCompressedTexturesButton";
            this.decodeCompressedTexturesButton.Size = new System.Drawing.Size(164, 17);
            this.decodeCompressedTexturesButton.TabIndex = 0;
            this.decodeCompressedTexturesButton.Text = "Decode compressed textures";
            this.decodeCompressedTexturesButton.UseVisualStyleBackColor = true;
            // 
            // outputToSourceDirButton
            // 
            this.outputToSourceDirButton.AutoSize = true;
            this.outputToSourceDirButton.Location = new System.Drawing.Point(14, 37);
            this.outputToSourceDirButton.Name = "outputToSourceDirButton";
            this.outputToSourceDirButton.Size = new System.Drawing.Size(261, 17);
            this.outputToSourceDirButton.TabIndex = 1;
            this.outputToSourceDirButton.Text = "Output to the same directory as the source texture";
            this.outputToSourceDirButton.UseVisualStyleBackColor = true;
            // 
            // deleteSourceButton
            // 
            this.deleteSourceButton.AutoSize = true;
            this.deleteSourceButton.Location = new System.Drawing.Point(14, 61);
            this.deleteSourceButton.Name = "deleteSourceButton";
            this.deleteSourceButton.Size = new System.Drawing.Size(196, 17);
            this.deleteSourceButton.TabIndex = 2;
            this.deleteSourceButton.Text = "Delete source texture (upon sucess)";
            this.deleteSourceButton.UseVisualStyleBackColor = true;
            // 
            // TextureDecoder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Name = "TextureDecoder";
            this.Text = "Texture Decoder";
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox decodeCompressedTexturesButton;
        private System.Windows.Forms.CheckBox outputToSourceDirButton;
        private System.Windows.Forms.CheckBox deleteSourceButton;
    }
}
