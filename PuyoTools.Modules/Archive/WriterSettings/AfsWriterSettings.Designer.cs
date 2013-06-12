namespace PuyoTools.Modules.Archive
{
    partial class AfsWriterSettings
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
            this.BlockSizeBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.AfsVersion1Radio = new System.Windows.Forms.RadioButton();
            this.AfsVersion2Radio = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Block Size";
            // 
            // BlockSizeBox
            // 
            this.BlockSizeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BlockSizeBox.FormattingEnabled = true;
            this.BlockSizeBox.Items.AddRange(new object[] {
            "2048",
            "16"});
            this.BlockSizeBox.Location = new System.Drawing.Point(68, 4);
            this.BlockSizeBox.Name = "BlockSizeBox";
            this.BlockSizeBox.Size = new System.Drawing.Size(121, 21);
            this.BlockSizeBox.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.AfsVersion1Radio);
            this.flowLayoutPanel1.Controls.Add(this.AfsVersion2Radio);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 51);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(95, 59);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "AFS Version";
            // 
            // AfsVersion1Radio
            // 
            this.AfsVersion1Radio.AutoSize = true;
            this.AfsVersion1Radio.Checked = true;
            this.AfsVersion1Radio.Location = new System.Drawing.Point(3, 3);
            this.AfsVersion1Radio.Name = "AfsVersion1Radio";
            this.AfsVersion1Radio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.AfsVersion1Radio.Size = new System.Drawing.Size(89, 17);
            this.AfsVersion1Radio.TabIndex = 1;
            this.AfsVersion1Radio.TabStop = true;
            this.AfsVersion1Radio.Text = "Version 1";
            this.AfsVersion1Radio.UseVisualStyleBackColor = true;
            // 
            // AfsVersion2Radio
            // 
            this.AfsVersion2Radio.AutoSize = true;
            this.AfsVersion2Radio.Location = new System.Drawing.Point(3, 26);
            this.AfsVersion2Radio.Name = "AfsVersion2Radio";
            this.AfsVersion2Radio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.AfsVersion2Radio.Size = new System.Drawing.Size(89, 17);
            this.AfsVersion2Radio.TabIndex = 2;
            this.AfsVersion2Radio.Text = "Version 2";
            this.AfsVersion2Radio.UseVisualStyleBackColor = true;
            // 
            // AfsWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.BlockSizeBox);
            this.Controls.Add(this.label1);
            this.Name = "AfsWriterSettings";
            this.Size = new System.Drawing.Size(192, 113);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox BlockSizeBox;
        public System.Windows.Forms.RadioButton AfsVersion1Radio;
        public System.Windows.Forms.RadioButton AfsVersion2Radio;

    }
}
