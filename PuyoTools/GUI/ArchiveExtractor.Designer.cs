namespace PuyoTools.GUI
{
    partial class ArchiveExtractor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.extractFileStructureCheckbox = new System.Windows.Forms.CheckBox();
            this.deleteSourceArchiveCheckbox = new System.Windows.Forms.CheckBox();
            this.extractToSameNameDirCheckbox = new System.Windows.Forms.CheckBox();
            this.extractToSourceDirCheckbox = new System.Windows.Forms.CheckBox();
            this.decompressSourceArchiveCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fileNumberAsFilenameCheckbox = new System.Windows.Forms.CheckBox();
            this.prependFileNumberCheckbox = new System.Windows.Forms.CheckBox();
            this.convertExtractedTexturesCheckbox = new System.Windows.Forms.CheckBox();
            this.extractExtractedArchivesCheckbox = new System.Windows.Forms.CheckBox();
            this.decompressExtractedFilesCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.groupBox2);
            this.settingsPanel.Controls.Add(this.groupBox1);
            this.settingsPanel.Size = new System.Drawing.Size(584, 331);
            // 
            // runButton
            // 
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.extractFileStructureCheckbox);
            this.groupBox1.Controls.Add(this.deleteSourceArchiveCheckbox);
            this.groupBox1.Controls.Add(this.extractToSameNameDirCheckbox);
            this.groupBox1.Controls.Add(this.extractToSourceDirCheckbox);
            this.groupBox1.Controls.Add(this.decompressSourceArchiveCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.groupBox1.Size = new System.Drawing.Size(557, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Archive";
            // 
            // extractFileStructureCheckbox
            // 
            this.extractFileStructureCheckbox.AutoSize = true;
            this.extractFileStructureCheckbox.Location = new System.Drawing.Point(13, 90);
            this.extractFileStructureCheckbox.Name = "extractFileStructureCheckbox";
            this.extractFileStructureCheckbox.Size = new System.Drawing.Size(339, 17);
            this.extractFileStructureCheckbox.TabIndex = 4;
            this.extractFileStructureCheckbox.Text = "Create a file (entries.txt) with the archive\'s entries and file structure.";
            this.extractFileStructureCheckbox.UseVisualStyleBackColor = true;
            // 
            // deleteSourceArchiveCheckbox
            // 
            this.deleteSourceArchiveCheckbox.AutoSize = true;
            this.deleteSourceArchiveCheckbox.Location = new System.Drawing.Point(13, 113);
            this.deleteSourceArchiveCheckbox.Name = "deleteSourceArchiveCheckbox";
            this.deleteSourceArchiveCheckbox.Size = new System.Drawing.Size(205, 17);
            this.deleteSourceArchiveCheckbox.TabIndex = 3;
            this.deleteSourceArchiveCheckbox.Text = "Delete source archive (upon success)";
            this.deleteSourceArchiveCheckbox.UseVisualStyleBackColor = true;
            // 
            // extractToSameNameDirCheckbox
            // 
            this.extractToSameNameDirCheckbox.AutoSize = true;
            this.extractToSameNameDirCheckbox.Location = new System.Drawing.Point(13, 67);
            this.extractToSameNameDirCheckbox.Name = "extractToSameNameDirCheckbox";
            this.extractToSameNameDirCheckbox.Size = new System.Drawing.Size(499, 17);
            this.extractToSameNameDirCheckbox.TabIndex = 2;
            this.extractToSameNameDirCheckbox.Text = "Extract files to a directory with the same name as the source archive (and delete" +
    " the source archive).";
            this.extractToSameNameDirCheckbox.UseVisualStyleBackColor = true;
            // 
            // extractToSourceDirCheckbox
            // 
            this.extractToSourceDirCheckbox.AutoSize = true;
            this.extractToSourceDirCheckbox.Location = new System.Drawing.Point(13, 43);
            this.extractToSourceDirCheckbox.Name = "extractToSourceDirCheckbox";
            this.extractToSourceDirCheckbox.Size = new System.Drawing.Size(289, 17);
            this.extractToSourceDirCheckbox.TabIndex = 1;
            this.extractToSourceDirCheckbox.Text = "Extract files to the same directory as the source archive.";
            this.extractToSourceDirCheckbox.UseVisualStyleBackColor = true;
            // 
            // decompressSourceArchiveCheckbox
            // 
            this.decompressSourceArchiveCheckbox.AutoSize = true;
            this.decompressSourceArchiveCheckbox.Checked = true;
            this.decompressSourceArchiveCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.decompressSourceArchiveCheckbox.Location = new System.Drawing.Point(13, 19);
            this.decompressSourceArchiveCheckbox.Name = "decompressSourceArchiveCheckbox";
            this.decompressSourceArchiveCheckbox.Size = new System.Drawing.Size(162, 17);
            this.decompressSourceArchiveCheckbox.TabIndex = 0;
            this.decompressSourceArchiveCheckbox.Text = "Extract compressed archives";
            this.decompressSourceArchiveCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.fileNumberAsFilenameCheckbox);
            this.groupBox2.Controls.Add(this.prependFileNumberCheckbox);
            this.groupBox2.Controls.Add(this.convertExtractedTexturesCheckbox);
            this.groupBox2.Controls.Add(this.extractExtractedArchivesCheckbox);
            this.groupBox2.Controls.Add(this.decompressExtractedFilesCheckbox);
            this.groupBox2.Location = new System.Drawing.Point(14, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.groupBox2.Size = new System.Drawing.Size(558, 149);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Extracted Files";
            // 
            // fileNumberAsFilenameCheckbox
            // 
            this.fileNumberAsFilenameCheckbox.AutoSize = true;
            this.fileNumberAsFilenameCheckbox.Location = new System.Drawing.Point(13, 44);
            this.fileNumberAsFilenameCheckbox.Name = "fileNumberAsFilenameCheckbox";
            this.fileNumberAsFilenameCheckbox.Size = new System.Drawing.Size(155, 17);
            this.fileNumberAsFilenameCheckbox.TabIndex = 4;
            this.fileNumberAsFilenameCheckbox.Text = "Use file number as filename";
            this.fileNumberAsFilenameCheckbox.UseVisualStyleBackColor = true;
            // 
            // prependFileNumberCheckbox
            // 
            this.prependFileNumberCheckbox.AutoSize = true;
            this.prependFileNumberCheckbox.Location = new System.Drawing.Point(13, 67);
            this.prependFileNumberCheckbox.Name = "prependFileNumberCheckbox";
            this.prependFileNumberCheckbox.Size = new System.Drawing.Size(174, 17);
            this.prependFileNumberCheckbox.TabIndex = 3;
            this.prependFileNumberCheckbox.Text = "Prepend file number to filename";
            this.prependFileNumberCheckbox.UseVisualStyleBackColor = true;
            // 
            // convertExtractedTexturesCheckbox
            // 
            this.convertExtractedTexturesCheckbox.AutoSize = true;
            this.convertExtractedTexturesCheckbox.Location = new System.Drawing.Point(13, 113);
            this.convertExtractedTexturesCheckbox.Name = "convertExtractedTexturesCheckbox";
            this.convertExtractedTexturesCheckbox.Size = new System.Drawing.Size(267, 17);
            this.convertExtractedTexturesCheckbox.TabIndex = 2;
            this.convertExtractedTexturesCheckbox.Text = "If the extracted file is a texture, convert it to a PNG.";
            this.convertExtractedTexturesCheckbox.UseVisualStyleBackColor = true;
            // 
            // extractExtractedArchivesCheckbox
            // 
            this.extractExtractedArchivesCheckbox.AutoSize = true;
            this.extractExtractedArchivesCheckbox.Location = new System.Drawing.Point(13, 90);
            this.extractExtractedArchivesCheckbox.Name = "extractExtractedArchivesCheckbox";
            this.extractExtractedArchivesCheckbox.Size = new System.Drawing.Size(225, 17);
            this.extractExtractedArchivesCheckbox.TabIndex = 1;
            this.extractExtractedArchivesCheckbox.Text = "If the extracted file is an archive, extract it.";
            this.extractExtractedArchivesCheckbox.UseVisualStyleBackColor = true;
            // 
            // decompressExtractedFilesCheckbox
            // 
            this.decompressExtractedFilesCheckbox.AutoSize = true;
            this.decompressExtractedFilesCheckbox.Location = new System.Drawing.Point(13, 20);
            this.decompressExtractedFilesCheckbox.Name = "decompressExtractedFilesCheckbox";
            this.decompressExtractedFilesCheckbox.Size = new System.Drawing.Size(153, 17);
            this.decompressExtractedFilesCheckbox.TabIndex = 0;
            this.decompressExtractedFilesCheckbox.Text = "Decompress extracted files";
            this.decompressExtractedFilesCheckbox.UseVisualStyleBackColor = true;
            // 
            // ArchiveExtractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 431);
            this.Name = "ArchiveExtractor";
            this.Text = "Archive Extractor";
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox decompressSourceArchiveCheckbox;
        private System.Windows.Forms.CheckBox extractToSourceDirCheckbox;
        private System.Windows.Forms.CheckBox extractToSameNameDirCheckbox;
        private System.Windows.Forms.CheckBox deleteSourceArchiveCheckbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox convertExtractedTexturesCheckbox;
        private System.Windows.Forms.CheckBox extractExtractedArchivesCheckbox;
        private System.Windows.Forms.CheckBox decompressExtractedFilesCheckbox;
        private System.Windows.Forms.CheckBox prependFileNumberCheckbox;
        private System.Windows.Forms.CheckBox fileNumberAsFilenameCheckbox;
        private System.Windows.Forms.CheckBox extractFileStructureCheckbox;
    }
}
