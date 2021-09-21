namespace PuyoTools.App.Formats.Textures.WriterSettings
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
            this.globalIndexTextBox = new System.Windows.Forms.TextBox();
            this.hasGlobalIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataFormatBox = new System.Windows.Forms.ComboBox();
            this.pixelFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // globalIndexTextBox
            // 
            this.globalIndexTextBox.Location = new System.Drawing.Point(74, 57);
            this.globalIndexTextBox.Name = "globalIndexTextBox";
            this.globalIndexTextBox.Size = new System.Drawing.Size(229, 20);
            this.globalIndexTextBox.TabIndex = 13;
            this.globalIndexTextBox.Text = "0";
            // 
            // hasGlobalIndexCheckBox
            // 
            this.hasGlobalIndexCheckBox.AutoSize = true;
            this.hasGlobalIndexCheckBox.Checked = true;
            this.hasGlobalIndexCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasGlobalIndexCheckBox.Location = new System.Drawing.Point(309, 60);
            this.hasGlobalIndexCheckBox.Name = "hasGlobalIndexCheckBox";
            this.hasGlobalIndexCheckBox.Size = new System.Drawing.Size(15, 14);
            this.hasGlobalIndexCheckBox.TabIndex = 12;
            this.hasGlobalIndexCheckBox.UseVisualStyleBackColor = true;
            this.hasGlobalIndexCheckBox.CheckStateChanged += new System.EventHandler(this.hasGlobalIndexCheckBox_CheckedChanged);
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
            // dataFormatBox
            // 
            this.dataFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataFormatBox.FormattingEnabled = true;
            this.dataFormatBox.Items.AddRange(new object[] {
            "Rectangle",
            "4-bit Indexed with External Palette",
            "8-bit Indexed with External Palette",
            "4-bit Indexed",
            "8-bit Indexed"});
            this.dataFormatBox.Location = new System.Drawing.Point(74, 30);
            this.dataFormatBox.Name = "dataFormatBox";
            this.dataFormatBox.Size = new System.Drawing.Size(250, 21);
            this.dataFormatBox.TabIndex = 10;
            // 
            // pixelFormatBox
            // 
            this.pixelFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pixelFormatBox.FormattingEnabled = true;
            this.pixelFormatBox.Items.AddRange(new object[] {
            "RGB5A3",
            "ARGB8888"});
            this.pixelFormatBox.Location = new System.Drawing.Point(74, 3);
            this.pixelFormatBox.Name = "pixelFormatBox";
            this.pixelFormatBox.Size = new System.Drawing.Size(250, 21);
            this.pixelFormatBox.TabIndex = 9;
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
            this.Controls.Add(this.globalIndexTextBox);
            this.Controls.Add(this.hasGlobalIndexCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataFormatBox);
            this.Controls.Add(this.pixelFormatBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SvrWriterSettings";
            this.Size = new System.Drawing.Size(327, 80);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox pixelFormatBox;
        private System.Windows.Forms.ComboBox dataFormatBox;
        private System.Windows.Forms.TextBox globalIndexTextBox;
        private System.Windows.Forms.CheckBox hasGlobalIndexCheckBox;
    }
}
