namespace PuyoTools.Formats.Archives.WriterSettings
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
            this.hasFilenamesCheckbox = new System.Windows.Forms.CheckBox();
            this.hasGlobalIndexesCheckbox = new System.Windows.Forms.CheckBox();
            this.hasFormatsCheckbox = new System.Windows.Forms.CheckBox();
            this.hasDimensionsCheckbox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.hasFilenamesCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.hasGlobalIndexesCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.hasFormatsCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.hasDimensionsCheckbox);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(147, 92);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // hasFilenamesCheckbox
            // 
            this.hasFilenamesCheckbox.AutoSize = true;
            this.hasFilenamesCheckbox.Checked = true;
            this.hasFilenamesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasFilenamesCheckbox.Location = new System.Drawing.Point(3, 3);
            this.hasFilenamesCheckbox.Name = "hasFilenamesCheckbox";
            this.hasFilenamesCheckbox.Size = new System.Drawing.Size(95, 17);
            this.hasFilenamesCheckbox.TabIndex = 0;
            this.hasFilenamesCheckbox.Text = "Has Filenames";
            this.hasFilenamesCheckbox.UseVisualStyleBackColor = true;
            // 
            // hasGlobalIndexesCheckbox
            // 
            this.hasGlobalIndexesCheckbox.AutoSize = true;
            this.hasGlobalIndexesCheckbox.Checked = true;
            this.hasGlobalIndexesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasGlobalIndexesCheckbox.Location = new System.Drawing.Point(3, 26);
            this.hasGlobalIndexesCheckbox.Name = "hasGlobalIndexesCheckbox";
            this.hasGlobalIndexesCheckbox.Size = new System.Drawing.Size(118, 17);
            this.hasGlobalIndexesCheckbox.TabIndex = 1;
            this.hasGlobalIndexesCheckbox.Text = "Has Global Indexes";
            this.hasGlobalIndexesCheckbox.UseVisualStyleBackColor = true;
            // 
            // hasFormatsCheckbox
            // 
            this.hasFormatsCheckbox.AutoSize = true;
            this.hasFormatsCheckbox.Checked = true;
            this.hasFormatsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasFormatsCheckbox.Location = new System.Drawing.Point(3, 49);
            this.hasFormatsCheckbox.Name = "hasFormatsCheckbox";
            this.hasFormatsCheckbox.Size = new System.Drawing.Size(124, 17);
            this.hasFormatsCheckbox.TabIndex = 2;
            this.hasFormatsCheckbox.Text = "Has Texture Formats";
            this.hasFormatsCheckbox.UseVisualStyleBackColor = true;
            // 
            // hasDimensionsCheckbox
            // 
            this.hasDimensionsCheckbox.AutoSize = true;
            this.hasDimensionsCheckbox.Checked = true;
            this.hasDimensionsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasDimensionsCheckbox.Location = new System.Drawing.Point(3, 72);
            this.hasDimensionsCheckbox.Name = "hasDimensionsCheckbox";
            this.hasDimensionsCheckbox.Size = new System.Drawing.Size(141, 17);
            this.hasDimensionsCheckbox.TabIndex = 3;
            this.hasDimensionsCheckbox.Text = "Has Texture Dimensions";
            this.hasDimensionsCheckbox.UseVisualStyleBackColor = true;
            // 
            // PvmWriterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "PvmWriterSettings";
            this.Size = new System.Drawing.Size(153, 98);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox hasFilenamesCheckbox;
        private System.Windows.Forms.CheckBox hasGlobalIndexesCheckbox;
        private System.Windows.Forms.CheckBox hasFormatsCheckbox;
        private System.Windows.Forms.CheckBox hasDimensionsCheckbox;
    }
}
