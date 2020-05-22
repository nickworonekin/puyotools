namespace PuyoTools.Formats.Archives.WriterSettings
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
            this.hasTimestampsCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.afsVersion1Radio = new System.Windows.Forms.RadioButton();
            this.afsVersion2Radio = new System.Windows.Forms.RadioButton();
            this.blockSizeBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hasTimestampsCheckbox
            // 
            this.hasTimestampsCheckbox.AutoSize = true;
            this.hasTimestampsCheckbox.Checked = true;
            this.hasTimestampsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasTimestampsCheckbox.Location = new System.Drawing.Point(8, 116);
            this.hasTimestampsCheckbox.Name = "hasTimestampsCheckbox";
            this.hasTimestampsCheckbox.Size = new System.Drawing.Size(104, 17);
            this.hasTimestampsCheckbox.TabIndex = 3;
            this.hasTimestampsCheckbox.Text = "Has Timestamps";
            this.hasTimestampsCheckbox.UseVisualStyleBackColor = true;
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
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.afsVersion1Radio);
            this.flowLayoutPanel1.Controls.Add(this.afsVersion2Radio);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 51);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(95, 59);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // afsVersion1Radio
            // 
            this.afsVersion1Radio.AutoSize = true;
            this.afsVersion1Radio.Checked = true;
            this.afsVersion1Radio.Location = new System.Drawing.Point(3, 3);
            this.afsVersion1Radio.Name = "afsVersion1Radio";
            this.afsVersion1Radio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.afsVersion1Radio.Size = new System.Drawing.Size(89, 17);
            this.afsVersion1Radio.TabIndex = 1;
            this.afsVersion1Radio.TabStop = true;
            this.afsVersion1Radio.Text = "Version 1";
            this.afsVersion1Radio.UseVisualStyleBackColor = true;
            // 
            // afsVersion2Radio
            // 
            this.afsVersion2Radio.AutoSize = true;
            this.afsVersion2Radio.Location = new System.Drawing.Point(3, 26);
            this.afsVersion2Radio.Name = "afsVersion2Radio";
            this.afsVersion2Radio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.afsVersion2Radio.Size = new System.Drawing.Size(89, 17);
            this.afsVersion2Radio.TabIndex = 2;
            this.afsVersion2Radio.Text = "Version 2";
            this.afsVersion2Radio.UseVisualStyleBackColor = true;
            // 
            // blockSizeBox
            // 
            this.blockSizeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.blockSizeBox.FormattingEnabled = true;
            this.blockSizeBox.Items.AddRange(new object[] {
            "2048",
            "16"});
            this.blockSizeBox.Location = new System.Drawing.Point(68, 4);
            this.blockSizeBox.Name = "blockSizeBox";
            this.blockSizeBox.Size = new System.Drawing.Size(121, 21);
            this.blockSizeBox.TabIndex = 1;
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
            // AfsWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hasTimestampsCheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.blockSizeBox);
            this.Controls.Add(this.label1);
            this.Name = "AfsWriterSettings";
            this.Size = new System.Drawing.Size(192, 136);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox blockSizeBox;
        private System.Windows.Forms.RadioButton afsVersion1Radio;
        private System.Windows.Forms.RadioButton afsVersion2Radio;
        private System.Windows.Forms.CheckBox hasTimestampsCheckbox;

    }
}
