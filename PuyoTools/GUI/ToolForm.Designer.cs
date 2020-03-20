namespace PuyoTools.GUI
{
    partial class ToolForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.addFromEntriesButton = new System.Windows.Forms.Button();
            this.fileListButton = new System.Windows.Forms.Button();
            this.addDirectoryButton = new System.Windows.Forms.Button();
            this.addFilesButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.runButton = new System.Windows.Forms.Button();
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.addFromEntriesButton);
            this.panel1.Controls.Add(this.fileListButton);
            this.panel1.Controls.Add(this.addDirectoryButton);
            this.panel1.Controls.Add(this.addFilesButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 50);
            this.panel1.TabIndex = 0;
            // 
            // addFromEntriesButton
            // 
            this.addFromEntriesButton.Location = new System.Drawing.Point(206, 13);
            this.addFromEntriesButton.Name = "addFromEntriesButton";
            this.addFromEntriesButton.Size = new System.Drawing.Size(126, 25);
            this.addFromEntriesButton.TabIndex = 3;
            this.addFromEntriesButton.Text = "Add from entries.txt";
            this.addFromEntriesButton.UseVisualStyleBackColor = true;
            this.addFromEntriesButton.Visible = false;
            // 
            // fileListButton
            // 
            this.fileListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileListButton.Location = new System.Drawing.Point(492, 12);
            this.fileListButton.Name = "fileListButton";
            this.fileListButton.Size = new System.Drawing.Size(80, 25);
            this.fileListButton.TabIndex = 2;
            this.fileListButton.Text = "File List";
            this.fileListButton.UseVisualStyleBackColor = true;
            this.fileListButton.Click += new System.EventHandler(this.fileListButton_Click);
            // 
            // addDirectoryButton
            // 
            this.addDirectoryButton.Location = new System.Drawing.Point(99, 13);
            this.addDirectoryButton.Name = "addDirectoryButton";
            this.addDirectoryButton.Size = new System.Drawing.Size(100, 25);
            this.addDirectoryButton.TabIndex = 1;
            this.addDirectoryButton.Text = "Add Directory";
            this.addDirectoryButton.UseVisualStyleBackColor = true;
            this.addDirectoryButton.Click += new System.EventHandler(this.addDirectoryButton_Click);
            // 
            // addFilesButton
            // 
            this.addFilesButton.Location = new System.Drawing.Point(13, 13);
            this.addFilesButton.Name = "addFilesButton";
            this.addFilesButton.Size = new System.Drawing.Size(80, 25);
            this.addFilesButton.TabIndex = 0;
            this.addFilesButton.Text = "Add Files";
            this.addFilesButton.UseVisualStyleBackColor = true;
            this.addFilesButton.Click += new System.EventHandler(this.addFilesButton_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Controls.Add(this.runButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 312);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(584, 50);
            this.panel2.TabIndex = 1;
            // 
            // runButton
            // 
            this.runButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.runButton.Enabled = false;
            this.runButton.Location = new System.Drawing.Point(252, 13);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(80, 25);
            this.runButton.TabIndex = 3;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            // 
            // settingsPanel
            // 
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPanel.Location = new System.Drawing.Point(0, 50);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Padding = new System.Windows.Forms.Padding(10);
            this.settingsPanel.Size = new System.Drawing.Size(584, 262);
            this.settingsPanel.TabIndex = 2;
            // 
            // ToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.settingsPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ToolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ToolForm";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        protected System.Windows.Forms.Panel settingsPanel;
        protected System.Windows.Forms.Button runButton;
        protected System.Windows.Forms.Button fileListButton;
        protected System.Windows.Forms.Button addFilesButton;
        protected System.Windows.Forms.Button addDirectoryButton;
        protected System.Windows.Forms.Button addFromEntriesButton;
    }
}