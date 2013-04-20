namespace PuyoTools.Modules.Archive
{
    partial class SntWriterSettings
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.TypePs2Radio = new System.Windows.Forms.RadioButton();
            this.TypePspRadio = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.TypePs2Radio);
            this.flowLayoutPanel1.Controls.Add(this.TypePspRadio);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(146, 59);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SNT Type";
            // 
            // TypePs2Radio
            // 
            this.TypePs2Radio.AutoSize = true;
            this.TypePs2Radio.Checked = true;
            this.TypePs2Radio.Location = new System.Drawing.Point(3, 16);
            this.TypePs2Radio.Name = "TypePs2Radio";
            this.TypePs2Radio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.TypePs2Radio.Size = new System.Drawing.Size(107, 17);
            this.TypePs2Radio.TabIndex = 1;
            this.TypePs2Radio.TabStop = true;
            this.TypePs2Radio.Text = "PlayStation 2";
            this.TypePs2Radio.UseVisualStyleBackColor = true;
            // 
            // TypePspRadio
            // 
            this.TypePspRadio.AutoSize = true;
            this.TypePspRadio.Location = new System.Drawing.Point(3, 39);
            this.TypePspRadio.Name = "TypePspRadio";
            this.TypePspRadio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.TypePspRadio.Size = new System.Drawing.Size(140, 17);
            this.TypePspRadio.TabIndex = 2;
            this.TypePspRadio.TabStop = true;
            this.TypePspRadio.Text = "PlayStation Portable";
            this.TypePspRadio.UseVisualStyleBackColor = true;
            // 
            // SntWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "SntWriterSettings";
            this.Size = new System.Drawing.Size(152, 65);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.RadioButton TypePs2Radio;
        public System.Windows.Forms.RadioButton TypePspRadio;
    }
}
