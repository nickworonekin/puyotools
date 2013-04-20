namespace PuyoTools.Modules.Archive
{
    partial class PvmWriterSettings
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
            this.FilenameCheckbox = new System.Windows.Forms.CheckBox();
            this.GlobalIndexCheckbox = new System.Windows.Forms.CheckBox();
            this.FormatCheckbox = new System.Windows.Forms.CheckBox();
            this.DimensionsCheckbox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.FilenameCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.GlobalIndexCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.FormatCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.DimensionsCheckbox);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(153, 92);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // FilenameCheckbox
            // 
            this.FilenameCheckbox.AutoSize = true;
            this.FilenameCheckbox.Checked = true;
            this.FilenameCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FilenameCheckbox.Location = new System.Drawing.Point(3, 3);
            this.FilenameCheckbox.Name = "FilenameCheckbox";
            this.FilenameCheckbox.Size = new System.Drawing.Size(101, 17);
            this.FilenameCheckbox.TabIndex = 0;
            this.FilenameCheckbox.Text = "Store Filenames";
            this.FilenameCheckbox.UseVisualStyleBackColor = true;
            // 
            // GlobalIndexCheckbox
            // 
            this.GlobalIndexCheckbox.AutoSize = true;
            this.GlobalIndexCheckbox.Checked = true;
            this.GlobalIndexCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GlobalIndexCheckbox.Location = new System.Drawing.Point(3, 26);
            this.GlobalIndexCheckbox.Name = "GlobalIndexCheckbox";
            this.GlobalIndexCheckbox.Size = new System.Drawing.Size(123, 17);
            this.GlobalIndexCheckbox.TabIndex = 1;
            this.GlobalIndexCheckbox.Text = "Store Global Indicies";
            this.GlobalIndexCheckbox.UseVisualStyleBackColor = true;
            // 
            // FormatCheckbox
            // 
            this.FormatCheckbox.AutoSize = true;
            this.FormatCheckbox.Checked = true;
            this.FormatCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FormatCheckbox.Location = new System.Drawing.Point(3, 49);
            this.FormatCheckbox.Name = "FormatCheckbox";
            this.FormatCheckbox.Size = new System.Drawing.Size(125, 17);
            this.FormatCheckbox.TabIndex = 2;
            this.FormatCheckbox.Text = "Store Texture Format";
            this.FormatCheckbox.UseVisualStyleBackColor = true;
            // 
            // DimensionsCheckbox
            // 
            this.DimensionsCheckbox.AutoSize = true;
            this.DimensionsCheckbox.Checked = true;
            this.DimensionsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DimensionsCheckbox.Location = new System.Drawing.Point(3, 72);
            this.DimensionsCheckbox.Name = "DimensionsCheckbox";
            this.DimensionsCheckbox.Size = new System.Drawing.Size(147, 17);
            this.DimensionsCheckbox.TabIndex = 3;
            this.DimensionsCheckbox.Text = "Store Texture Dimensions";
            this.DimensionsCheckbox.UseVisualStyleBackColor = true;
            // 
            // PvmWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "PvmWriterSettings";
            this.Size = new System.Drawing.Size(159, 98);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.CheckBox FilenameCheckbox;
        public System.Windows.Forms.CheckBox GlobalIndexCheckbox;
        public System.Windows.Forms.CheckBox FormatCheckbox;
        public System.Windows.Forms.CheckBox DimensionsCheckbox;
    }
}
