namespace PuyoTools.Modules.Texture
{
    partial class SvrWriterSettings
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
            this.PixelFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // GlobalIndexTextBox
            // 
            this.GlobalIndexTextBox.Location = new System.Drawing.Point(74, 57);
            this.GlobalIndexTextBox.Name = "GlobalIndexTextBox";
            this.GlobalIndexTextBox.Size = new System.Drawing.Size(229, 20);
            this.GlobalIndexTextBox.TabIndex = 13;
            this.GlobalIndexTextBox.Text = "0";
            // 
            // HasGlobalIndexCheckBox
            // 
            this.HasGlobalIndexCheckBox.AutoSize = true;
            this.HasGlobalIndexCheckBox.Checked = true;
            this.HasGlobalIndexCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HasGlobalIndexCheckBox.Location = new System.Drawing.Point(309, 60);
            this.HasGlobalIndexCheckBox.Name = "HasGlobalIndexCheckBox";
            this.HasGlobalIndexCheckBox.Size = new System.Drawing.Size(15, 14);
            this.HasGlobalIndexCheckBox.TabIndex = 12;
            this.HasGlobalIndexCheckBox.UseVisualStyleBackColor = true;
            this.HasGlobalIndexCheckBox.CheckStateChanged += new System.EventHandler(this.hasGlobalIndexCheckBox_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Global Index";
            // 
            // DataFormatBox
            // 
            this.DataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DataFormatBox.FormattingEnabled = true;
            this.DataFormatBox.Items.AddRange(new object[] {
            "Rectangle",
            "4-bit Indexed with External Palette",
            "8-bit Indexed with External Palette",
            "4-bit Indexed",
            "8-bit Indexed"});
            this.DataFormatBox.Location = new System.Drawing.Point(74, 30);
            this.DataFormatBox.Name = "DataFormatBox";
            this.DataFormatBox.Size = new System.Drawing.Size(250, 21);
            this.DataFormatBox.TabIndex = 10;
            // 
            // PixelFormatBox
            // 
            this.PixelFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PixelFormatBox.FormattingEnabled = true;
            this.PixelFormatBox.Items.AddRange(new object[] {
            "RGB5A3",
            "ARGB8888"});
            this.PixelFormatBox.Location = new System.Drawing.Point(74, 3);
            this.PixelFormatBox.Name = "PixelFormatBox";
            this.PixelFormatBox.Size = new System.Drawing.Size(250, 21);
            this.PixelFormatBox.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Data Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Pixel Format";
            // 
            // SvrWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.GlobalIndexTextBox);
            this.Controls.Add(this.HasGlobalIndexCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DataFormatBox);
            this.Controls.Add(this.PixelFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SvrWriterSettings";
            this.Size = new System.Drawing.Size(327, 80);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox GlobalIndexTextBox;
        public System.Windows.Forms.CheckBox HasGlobalIndexCheckBox;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox DataFormatBox;
        public System.Windows.Forms.ComboBox PixelFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
