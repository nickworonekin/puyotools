namespace PuyoTools.Modules.Texture
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
            this.DataFormatBox = new System.Windows.Forms.ComboBox();
            this.PaletteFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.HasMetadataCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DataFormatBox
            // 
            this.DataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DataFormatBox.FormattingEnabled = true;
            this.DataFormatBox.Items.AddRange(new object[] {
            "RGB565",
            "ARGB1555",
            "ARGB4444",
            "ARGB8888",
            "4-bit Indexed",
            "8-bit Indexed"});
            this.DataFormatBox.Location = new System.Drawing.Point(84, 30);
            this.DataFormatBox.Name = "DataFormatBox";
            this.DataFormatBox.Size = new System.Drawing.Size(200, 21);
            this.DataFormatBox.TabIndex = 21;
            this.DataFormatBox.SelectedIndexChanged += new System.EventHandler(this.DataFormatBox_SelectedIndexChanged);
            // 
            // PaletteFormatBox
            // 
            this.PaletteFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PaletteFormatBox.FormattingEnabled = true;
            this.PaletteFormatBox.Items.AddRange(new object[] {
            "RGB565",
            "ARGB1555",
            "ARGB4444",
            "ARGB8888"});
            this.PaletteFormatBox.Location = new System.Drawing.Point(84, 3);
            this.PaletteFormatBox.Name = "PaletteFormatBox";
            this.PaletteFormatBox.Size = new System.Drawing.Size(200, 21);
            this.PaletteFormatBox.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Data Format";
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
            // HasMetadataCheckBox
            // 
            this.HasMetadataCheckBox.AutoSize = true;
            this.HasMetadataCheckBox.Checked = true;
            this.HasMetadataCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HasMetadataCheckBox.Location = new System.Drawing.Point(6, 57);
            this.HasMetadataCheckBox.Name = "HasMetadataCheckBox";
            this.HasMetadataCheckBox.Size = new System.Drawing.Size(93, 17);
            this.HasMetadataCheckBox.TabIndex = 22;
            this.HasMetadataCheckBox.Text = "Has Metadata";
            this.HasMetadataCheckBox.UseVisualStyleBackColor = true;
            // 
            // GimWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.HasMetadataCheckBox);
            this.Controls.Add(this.DataFormatBox);
            this.Controls.Add(this.PaletteFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GimWriterSettings";
            this.Size = new System.Drawing.Size(287, 77);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox DataFormatBox;
        public System.Windows.Forms.ComboBox PaletteFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox HasMetadataCheckBox;
    }
}
