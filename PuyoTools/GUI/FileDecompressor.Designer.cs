namespace PuyoTools.GUI
{
    partial class FileDecompressor
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
            this.overwriteSourceFileCheckbox = new System.Windows.Forms.CheckBox();
            this.deleteSourceFileCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.deleteSourceFileCheckbox);
            this.settingsPanel.Controls.Add(this.overwriteSourceFileCheckbox);
            // 
            // runButton
            // 
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // overwriteSourceFileCheckbox
            // 
            this.overwriteSourceFileCheckbox.AutoSize = true;
            this.overwriteSourceFileCheckbox.Location = new System.Drawing.Point(12, 14);
            this.overwriteSourceFileCheckbox.Name = "overwriteSourceFileCheckbox";
            this.overwriteSourceFileCheckbox.Size = new System.Drawing.Size(122, 17);
            this.overwriteSourceFileCheckbox.TabIndex = 0;
            this.overwriteSourceFileCheckbox.Text = "Overwrite source file";
            this.overwriteSourceFileCheckbox.UseVisualStyleBackColor = true;
            // 
            // deleteSourceFileCheckbox
            // 
            this.deleteSourceFileCheckbox.AutoSize = true;
            this.deleteSourceFileCheckbox.Location = new System.Drawing.Point(12, 37);
            this.deleteSourceFileCheckbox.Name = "deleteSourceFileCheckbox";
            this.deleteSourceFileCheckbox.Size = new System.Drawing.Size(183, 17);
            this.deleteSourceFileCheckbox.TabIndex = 1;
            this.deleteSourceFileCheckbox.Text = "Delete source file (upon success)";
            this.deleteSourceFileCheckbox.UseVisualStyleBackColor = true;
            // 
            // FileDecompressor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Name = "FileDecompressor";
            this.Text = "File Decompressor";
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox overwriteSourceFileCheckbox;
        private System.Windows.Forms.CheckBox deleteSourceFileCheckbox;
    }
}
