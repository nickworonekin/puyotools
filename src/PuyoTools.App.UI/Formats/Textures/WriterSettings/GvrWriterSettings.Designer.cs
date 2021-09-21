namespace PuyoTools.App.Formats.Textures.WriterSettings
{
    partial class GvrWriterSettings
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
            this.globalIndexTextBox = new System.Windows.Forms.TextBox();
            this.hasGlobalIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataFormatBox = new System.Windows.Forms.ComboBox();
            this.paletteFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hasMipmapsCheckBox = new System.Windows.Forms.CheckBox();
            this.hasExternalPaletteCheckBox = new System.Windows.Forms.CheckBox();
            this.gbixTypeBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // globalIndexTextBox
            // 
            this.globalIndexTextBox.Location = new System.Drawing.Point(84, 58);
            this.globalIndexTextBox.Name = "globalIndexTextBox";
            this.globalIndexTextBox.Size = new System.Drawing.Size(123, 20);
            this.globalIndexTextBox.TabIndex = 20;
            this.globalIndexTextBox.Text = "0";
            // 
            // hasGlobalIndexCheckBox
            // 
            this.hasGlobalIndexCheckBox.AutoSize = true;
            this.hasGlobalIndexCheckBox.Checked = true;
            this.hasGlobalIndexCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasGlobalIndexCheckBox.Location = new System.Drawing.Point(319, 61);
            this.hasGlobalIndexCheckBox.Name = "hasGlobalIndexCheckBox";
            this.hasGlobalIndexCheckBox.Size = new System.Drawing.Size(15, 14);
            this.hasGlobalIndexCheckBox.TabIndex = 19;
            this.hasGlobalIndexCheckBox.UseVisualStyleBackColor = true;
            this.hasGlobalIndexCheckBox.CheckedChanged += new System.EventHandler(this.hasGlobalIndexCheckBox_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Global Index";
            // 
            // dataFormatBox
            // 
            this.dataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataFormatBox.FormattingEnabled = true;
            this.dataFormatBox.Items.AddRange(new object[] {
            "4-bit Intensity",
            "8-bit Intensity",
            "4-bit Intensity with Alpha",
            "8-bit Intensity with Alpha",
            "RGB565",
            "RGB5A3",
            "ARGB8888",
            "4-bit Indexed",
            "8-bit Indexed",
            "DXT1 Compressed"});
            this.dataFormatBox.Location = new System.Drawing.Point(84, 30);
            this.dataFormatBox.Name = "dataFormatBox";
            this.dataFormatBox.Size = new System.Drawing.Size(250, 21);
            this.dataFormatBox.TabIndex = 17;
            this.dataFormatBox.SelectedIndexChanged += new System.EventHandler(this.DataFormatBox_SelectedIndexChanged);
            // 
            // paletteFormatBox
            // 
            this.paletteFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paletteFormatBox.FormattingEnabled = true;
            this.paletteFormatBox.Items.AddRange(new object[] {
            "8-bit Intensity with Alpha",
            "RGB565",
            "RGB5A3"});
            this.paletteFormatBox.Location = new System.Drawing.Point(84, 3);
            this.paletteFormatBox.Name = "paletteFormatBox";
            this.paletteFormatBox.Size = new System.Drawing.Size(250, 21);
            this.paletteFormatBox.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Data Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Palette Format";
            // 
            // hasMipmapsCheckBox
            // 
            this.hasMipmapsCheckBox.AutoSize = true;
            this.hasMipmapsCheckBox.Location = new System.Drawing.Point(6, 84);
            this.hasMipmapsCheckBox.Name = "hasMipmapsCheckBox";
            this.hasMipmapsCheckBox.Size = new System.Drawing.Size(90, 17);
            this.hasMipmapsCheckBox.TabIndex = 21;
            this.hasMipmapsCheckBox.Text = "Has Mipmaps";
            this.hasMipmapsCheckBox.UseVisualStyleBackColor = true;
            // 
            // hasExternalPaletteCheckBox
            // 
            this.hasExternalPaletteCheckBox.AutoSize = true;
            this.hasExternalPaletteCheckBox.Location = new System.Drawing.Point(6, 107);
            this.hasExternalPaletteCheckBox.Name = "hasExternalPaletteCheckBox";
            this.hasExternalPaletteCheckBox.Size = new System.Drawing.Size(122, 17);
            this.hasExternalPaletteCheckBox.TabIndex = 22;
            this.hasExternalPaletteCheckBox.Text = "Has External Palette";
            this.hasExternalPaletteCheckBox.UseVisualStyleBackColor = true;
            // 
            // gbixTypeBox
            // 
            this.gbixTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gbixTypeBox.FormattingEnabled = true;
            this.gbixTypeBox.Items.AddRange(new object[] {
            "GBIX",
            "GCIX"});
            this.gbixTypeBox.Location = new System.Drawing.Point(213, 58);
            this.gbixTypeBox.Name = "gbixTypeBox";
            this.gbixTypeBox.Size = new System.Drawing.Size(100, 21);
            this.gbixTypeBox.TabIndex = 23;
            // 
            // GvrWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbixTypeBox);
            this.Controls.Add(this.hasExternalPaletteCheckBox);
            this.Controls.Add(this.hasMipmapsCheckBox);
            this.Controls.Add(this.globalIndexTextBox);
            this.Controls.Add(this.hasGlobalIndexCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataFormatBox);
            this.Controls.Add(this.paletteFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GvrWriterSettings";
            this.Size = new System.Drawing.Size(337, 127);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox dataFormatBox;
        private System.Windows.Forms.ComboBox paletteFormatBox;
        private System.Windows.Forms.TextBox globalIndexTextBox;
        private System.Windows.Forms.ComboBox gbixTypeBox;
        private System.Windows.Forms.CheckBox hasGlobalIndexCheckBox;
        private System.Windows.Forms.CheckBox hasMipmapsCheckBox;
        private System.Windows.Forms.CheckBox hasExternalPaletteCheckBox;
    }
}
