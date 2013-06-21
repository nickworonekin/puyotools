namespace PuyoTools.Modules.Texture
{
    partial class PvrWriterSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PixelFormatBox = new System.Windows.Forms.ComboBox();
            this.DataFormatBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.HasGlobalIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.GlobalIndexTextBox = new System.Windows.Forms.TextBox();
            this.RleCompressionCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pixel Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Data Format";
            // 
            // PixelFormatBox
            // 
            this.PixelFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PixelFormatBox.FormattingEnabled = true;
            this.PixelFormatBox.Items.AddRange(new object[] {
            "ARGB1555",
            "RGB565",
            "ARGB4444"});
            this.PixelFormatBox.Location = new System.Drawing.Point(74, 3);
            this.PixelFormatBox.Name = "PixelFormatBox";
            this.PixelFormatBox.Size = new System.Drawing.Size(250, 21);
            this.PixelFormatBox.TabIndex = 2;
            // 
            // DataFormatBox
            // 
            this.DataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DataFormatBox.FormattingEnabled = true;
            this.DataFormatBox.Items.AddRange(new object[] {
            "Square Twiddled",
            "Square Twiddled with Mipmaps",
            "4-bit Indexed with External Palette",
            "8-bit Indexed with External Palette",
            "Rectangle",
            "Rectangle Twiddled",
            "Square Twiddled with Mipmaps (Alternate)"});
            this.DataFormatBox.Location = new System.Drawing.Point(74, 30);
            this.DataFormatBox.Name = "DataFormatBox";
            this.DataFormatBox.Size = new System.Drawing.Size(250, 21);
            this.DataFormatBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Global Index";
            // 
            // HasGlobalIndexCheckBox
            // 
            this.HasGlobalIndexCheckBox.AutoSize = true;
            this.HasGlobalIndexCheckBox.Checked = true;
            this.HasGlobalIndexCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HasGlobalIndexCheckBox.Location = new System.Drawing.Point(309, 59);
            this.HasGlobalIndexCheckBox.Name = "HasGlobalIndexCheckBox";
            this.HasGlobalIndexCheckBox.Size = new System.Drawing.Size(15, 14);
            this.HasGlobalIndexCheckBox.TabIndex = 5;
            this.HasGlobalIndexCheckBox.UseVisualStyleBackColor = true;
            this.HasGlobalIndexCheckBox.CheckedChanged += new System.EventHandler(this.hasGlobalIndexCheckBox_CheckedChanged);
            // 
            // GlobalIndexTextBox
            // 
            this.GlobalIndexTextBox.Location = new System.Drawing.Point(74, 57);
            this.GlobalIndexTextBox.Name = "GlobalIndexTextBox";
            this.GlobalIndexTextBox.Size = new System.Drawing.Size(229, 20);
            this.GlobalIndexTextBox.TabIndex = 6;
            this.GlobalIndexTextBox.Text = "0";
            this.GlobalIndexTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.globalIndexTextBox_KeyPress);
            // 
            // RleCompressionCheckBox
            // 
            this.RleCompressionCheckBox.AutoSize = true;
            this.RleCompressionCheckBox.Location = new System.Drawing.Point(6, 83);
            this.RleCompressionCheckBox.Name = "RleCompressionCheckBox";
            this.RleCompressionCheckBox.Size = new System.Drawing.Size(160, 17);
            this.RleCompressionCheckBox.TabIndex = 7;
            this.RleCompressionCheckBox.Text = "RLE compress texture (PVZ)";
            this.RleCompressionCheckBox.UseVisualStyleBackColor = true;
            // 
            // PvrWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.RleCompressionCheckBox);
            this.Controls.Add(this.GlobalIndexTextBox);
            this.Controls.Add(this.HasGlobalIndexCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DataFormatBox);
            this.Controls.Add(this.PixelFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PvrWriterSettings";
            this.Size = new System.Drawing.Size(327, 103);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox PixelFormatBox;
        public System.Windows.Forms.ComboBox DataFormatBox;
        public System.Windows.Forms.CheckBox HasGlobalIndexCheckBox;
        public System.Windows.Forms.TextBox GlobalIndexTextBox;
        public System.Windows.Forms.CheckBox RleCompressionCheckBox;
    }
}
