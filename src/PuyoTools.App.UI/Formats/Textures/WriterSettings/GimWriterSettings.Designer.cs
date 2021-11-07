namespace PuyoTools.App.Formats.Textures.WriterSettings
{
    partial class GimWriterSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataFormatBox = new System.Windows.Forms.ComboBox();
            this.paletteFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hasMetadataCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // dataFormatBox
            // 
            this.dataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataFormatBox.FormattingEnabled = true;
            this.dataFormatBox.Items.AddRange(new object[] {
            "RGB565",
            "ARGB1555",
            "ARGB4444",
            "ARGB8888",
            "4-bit Indexed",
            "8-bit Indexed"});
            this.dataFormatBox.Location = new System.Drawing.Point(84, 30);
            this.dataFormatBox.Name = "dataFormatBox";
            this.dataFormatBox.Size = new System.Drawing.Size(200, 21);
            this.dataFormatBox.TabIndex = 21;
            this.dataFormatBox.SelectedIndexChanged += new System.EventHandler(this.DataFormatBox_SelectedIndexChanged);
            // 
            // paletteFormatBox
            // 
            this.paletteFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paletteFormatBox.FormattingEnabled = true;
            this.paletteFormatBox.Items.AddRange(new object[] {
            "RGB565",
            "ARGB1555",
            "ARGB4444",
            "ARGB8888"});
            this.paletteFormatBox.Location = new System.Drawing.Point(84, 3);
            this.paletteFormatBox.Name = "paletteFormatBox";
            this.paletteFormatBox.Size = new System.Drawing.Size(200, 21);
            this.paletteFormatBox.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Pixel Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Palette Format";
            // 
            // hasMetadataCheckBox
            // 
            this.hasMetadataCheckBox.AutoSize = true;
            this.hasMetadataCheckBox.Checked = true;
            this.hasMetadataCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasMetadataCheckBox.Location = new System.Drawing.Point(6, 57);
            this.hasMetadataCheckBox.Name = "hasMetadataCheckBox";
            this.hasMetadataCheckBox.Size = new System.Drawing.Size(93, 17);
            this.hasMetadataCheckBox.TabIndex = 22;
            this.hasMetadataCheckBox.Text = "Has Metadata";
            this.hasMetadataCheckBox.UseVisualStyleBackColor = true;
            // 
            // GimWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hasMetadataCheckBox);
            this.Controls.Add(this.dataFormatBox);
            this.Controls.Add(this.paletteFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GimWriterSettings";
            this.Size = new System.Drawing.Size(287, 77);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox paletteFormatBox;
        private System.Windows.Forms.ComboBox dataFormatBox;
        private System.Windows.Forms.CheckBox hasMetadataCheckBox;
    }
}
