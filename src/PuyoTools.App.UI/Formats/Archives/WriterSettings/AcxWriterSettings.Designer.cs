namespace PuyoTools.Formats.Archives.WriterSettings
{
    partial class AcxWriterSettings
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
            this.blockSizeBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // blockSizeBox
            // 
            this.blockSizeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.blockSizeBox.FormattingEnabled = true;
            this.blockSizeBox.Items.AddRange(new object[] {
            "4",
            "2048"});
            this.blockSizeBox.Location = new System.Drawing.Point(67, 3);
            this.blockSizeBox.Name = "blockSizeBox";
            this.blockSizeBox.Size = new System.Drawing.Size(121, 21);
            this.blockSizeBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Block Size";
            // 
            // AcxWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.blockSizeBox);
            this.Controls.Add(this.label1);
            this.Name = "AcxWriterSettings";
            this.Size = new System.Drawing.Size(191, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox blockSizeBox;
        private System.Windows.Forms.Label label1;
    }
}
