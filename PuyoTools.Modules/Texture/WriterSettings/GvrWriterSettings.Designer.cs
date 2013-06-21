namespace PuyoTools.Modules.Texture
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
            this.GlobalIndexTextBox = new System.Windows.Forms.TextBox();
            this.HasGlobalIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DataFormatBox = new System.Windows.Forms.ComboBox();
            this.PaletteFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.HasMipmapsCheckBox = new System.Windows.Forms.CheckBox();
            this.HasExternalPaletteCheckBox = new System.Windows.Forms.CheckBox();
            this.GbixTypeBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // GlobalIndexTextBox
            // 
            this.GlobalIndexTextBox.Location = new System.Drawing.Point(84, 58);
            this.GlobalIndexTextBox.Name = "GlobalIndexTextBox";
            this.GlobalIndexTextBox.Size = new System.Drawing.Size(123, 20);
            this.GlobalIndexTextBox.TabIndex = 20;
            this.GlobalIndexTextBox.Text = "0";
            // 
            // HasGlobalIndexCheckBox
            // 
            this.HasGlobalIndexCheckBox.AutoSize = true;
            this.HasGlobalIndexCheckBox.Checked = true;
            this.HasGlobalIndexCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HasGlobalIndexCheckBox.Location = new System.Drawing.Point(319, 61);
            this.HasGlobalIndexCheckBox.Name = "HasGlobalIndexCheckBox";
            this.HasGlobalIndexCheckBox.Size = new System.Drawing.Size(15, 14);
            this.HasGlobalIndexCheckBox.TabIndex = 19;
            this.HasGlobalIndexCheckBox.UseVisualStyleBackColor = true;
            this.HasGlobalIndexCheckBox.CheckedChanged += new System.EventHandler(this.hasGlobalIndexCheckBox_CheckedChanged);
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
            // DataFormatBox
            // 
            this.DataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DataFormatBox.FormattingEnabled = true;
            this.DataFormatBox.Items.AddRange(new object[] {
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
            this.DataFormatBox.Location = new System.Drawing.Point(84, 30);
            this.DataFormatBox.Name = "DataFormatBox";
            this.DataFormatBox.Size = new System.Drawing.Size(250, 21);
            this.DataFormatBox.TabIndex = 17;
            this.DataFormatBox.SelectedIndexChanged += new System.EventHandler(this.DataFormatBox_SelectedIndexChanged);
            // 
            // PaletteFormatBox
            // 
            this.PaletteFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PaletteFormatBox.FormattingEnabled = true;
            this.PaletteFormatBox.Items.AddRange(new object[] {
            "8-bit Intensity with Alpha",
            "RGB565",
            "RGB5A3"});
            this.PaletteFormatBox.Location = new System.Drawing.Point(84, 3);
            this.PaletteFormatBox.Name = "PaletteFormatBox";
            this.PaletteFormatBox.Size = new System.Drawing.Size(250, 21);
            this.PaletteFormatBox.TabIndex = 16;
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
            // HasMipmapsCheckBox
            // 
            this.HasMipmapsCheckBox.AutoSize = true;
            this.HasMipmapsCheckBox.Location = new System.Drawing.Point(6, 84);
            this.HasMipmapsCheckBox.Name = "HasMipmapsCheckBox";
            this.HasMipmapsCheckBox.Size = new System.Drawing.Size(90, 17);
            this.HasMipmapsCheckBox.TabIndex = 21;
            this.HasMipmapsCheckBox.Text = "Has Mipmaps";
            this.HasMipmapsCheckBox.UseVisualStyleBackColor = true;
            // 
            // HasExternalPaletteCheckBox
            // 
            this.HasExternalPaletteCheckBox.AutoSize = true;
            this.HasExternalPaletteCheckBox.Location = new System.Drawing.Point(6, 107);
            this.HasExternalPaletteCheckBox.Name = "HasExternalPaletteCheckBox";
            this.HasExternalPaletteCheckBox.Size = new System.Drawing.Size(122, 17);
            this.HasExternalPaletteCheckBox.TabIndex = 22;
            this.HasExternalPaletteCheckBox.Text = "Has External Palette";
            this.HasExternalPaletteCheckBox.UseVisualStyleBackColor = true;
            // 
            // GbixTypeBox
            // 
            this.GbixTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GbixTypeBox.FormattingEnabled = true;
            this.GbixTypeBox.Items.AddRange(new object[] {
            "GBIX",
            "GCIX"});
            this.GbixTypeBox.Location = new System.Drawing.Point(213, 58);
            this.GbixTypeBox.Name = "GbixTypeBox";
            this.GbixTypeBox.Size = new System.Drawing.Size(100, 21);
            this.GbixTypeBox.TabIndex = 23;
            // 
            // GvrWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.GbixTypeBox);
            this.Controls.Add(this.HasExternalPaletteCheckBox);
            this.Controls.Add(this.HasMipmapsCheckBox);
            this.Controls.Add(this.GlobalIndexTextBox);
            this.Controls.Add(this.HasGlobalIndexCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DataFormatBox);
            this.Controls.Add(this.PaletteFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GvrWriterSettings";
            this.Size = new System.Drawing.Size(337, 127);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox GlobalIndexTextBox;
        public System.Windows.Forms.CheckBox HasGlobalIndexCheckBox;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox DataFormatBox;
        public System.Windows.Forms.ComboBox PaletteFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox HasMipmapsCheckBox;
        public System.Windows.Forms.CheckBox HasExternalPaletteCheckBox;
        public System.Windows.Forms.ComboBox GbixTypeBox;
    }
}
