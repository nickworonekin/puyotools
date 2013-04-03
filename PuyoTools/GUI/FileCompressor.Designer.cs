namespace PuyoTools.GUI
{
    partial class FileCompressor
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
            this.compressionFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deleteSourceFileCheckbox = new System.Windows.Forms.CheckBox();
            this.overwriteSourceFileCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.deleteSourceFileCheckbox);
            this.settingsPanel.Controls.Add(this.overwriteSourceFileCheckbox);
            this.settingsPanel.Controls.Add(this.compressionFormatBox);
            this.settingsPanel.Controls.Add(this.label2);
            // 
            // runButton
            // 
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // compressionFormatBox
            // 
            this.compressionFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compressionFormatBox.FormattingEnabled = true;
            this.compressionFormatBox.Items.AddRange(new object[] {
            "Select a format"});
            this.compressionFormatBox.Location = new System.Drawing.Point(121, 7);
            this.compressionFormatBox.Name = "compressionFormatBox";
            this.compressionFormatBox.Size = new System.Drawing.Size(150, 21);
            this.compressionFormatBox.TabIndex = 5;
            this.compressionFormatBox.SelectedIndexChanged += new System.EventHandler(this.EnableRunButton);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Compression Format";
            // 
            // deleteSourceFileCheckbox
            // 
            this.deleteSourceFileCheckbox.AutoSize = true;
            this.deleteSourceFileCheckbox.Location = new System.Drawing.Point(12, 57);
            this.deleteSourceFileCheckbox.Name = "deleteSourceFileCheckbox";
            this.deleteSourceFileCheckbox.Size = new System.Drawing.Size(183, 17);
            this.deleteSourceFileCheckbox.TabIndex = 7;
            this.deleteSourceFileCheckbox.Text = "Delete source file (upon success)";
            this.deleteSourceFileCheckbox.UseVisualStyleBackColor = true;
            // 
            // overwriteSourceFileCheckbox
            // 
            this.overwriteSourceFileCheckbox.AutoSize = true;
            this.overwriteSourceFileCheckbox.Location = new System.Drawing.Point(12, 34);
            this.overwriteSourceFileCheckbox.Name = "overwriteSourceFileCheckbox";
            this.overwriteSourceFileCheckbox.Size = new System.Drawing.Size(122, 17);
            this.overwriteSourceFileCheckbox.TabIndex = 6;
            this.overwriteSourceFileCheckbox.Text = "Overwrite source file";
            this.overwriteSourceFileCheckbox.UseVisualStyleBackColor = true;
            // 
            // FileCompressor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Name = "FileCompressor";
            this.Text = "File Compressor";
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox compressionFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox deleteSourceFileCheckbox;
        private System.Windows.Forms.CheckBox overwriteSourceFileCheckbox;
    }
}
