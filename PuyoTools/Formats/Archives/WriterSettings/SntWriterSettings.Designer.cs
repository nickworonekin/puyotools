namespace PuyoTools.Formats.Archives.WriterSettings
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
            this.platformPs2Radio = new System.Windows.Forms.RadioButton();
            this.platformPspRadio = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.platformPs2Radio);
            this.flowLayoutPanel1.Controls.Add(this.platformPspRadio);
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
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SNT Platform";
            // 
            // platformPs2Radio
            // 
            this.platformPs2Radio.AutoSize = true;
            this.platformPs2Radio.Checked = true;
            this.platformPs2Radio.Location = new System.Drawing.Point(3, 16);
            this.platformPs2Radio.Name = "platformPs2Radio";
            this.platformPs2Radio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.platformPs2Radio.Size = new System.Drawing.Size(107, 17);
            this.platformPs2Radio.TabIndex = 1;
            this.platformPs2Radio.TabStop = true;
            this.platformPs2Radio.Text = "PlayStation 2";
            this.platformPs2Radio.UseVisualStyleBackColor = true;
            // 
            // platformPspRadio
            // 
            this.platformPspRadio.AutoSize = true;
            this.platformPspRadio.Location = new System.Drawing.Point(3, 39);
            this.platformPspRadio.Name = "platformPspRadio";
            this.platformPspRadio.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.platformPspRadio.Size = new System.Drawing.Size(140, 17);
            this.platformPspRadio.TabIndex = 2;
            this.platformPspRadio.TabStop = true;
            this.platformPspRadio.Text = "PlayStation Portable";
            this.platformPspRadio.UseVisualStyleBackColor = true;
            // 
            // SntWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
        private System.Windows.Forms.RadioButton platformPs2Radio;
        private System.Windows.Forms.RadioButton platformPspRadio;
    }
}
