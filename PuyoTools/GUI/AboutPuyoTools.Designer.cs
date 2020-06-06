namespace PuyoTools.GUI
{
    partial class AboutPuyoTools
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
            this.gitHubLinkLabel = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.licenseLinkLabel = new System.Windows.Forms.LinkLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gitHubLinkLabel
            // 
            this.gitHubLinkLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gitHubLinkLabel.AutoSize = true;
            this.gitHubLinkLabel.Location = new System.Drawing.Point(136, 13);
            this.gitHubLinkLabel.Name = "gitHubLinkLabel";
            this.gitHubLinkLabel.Size = new System.Drawing.Size(40, 13);
            this.gitHubLinkLabel.TabIndex = 4;
            this.gitHubLinkLabel.TabStop = true;
            this.gitHubLinkLabel.Text = "GitHub";
            this.gitHubLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.gitHubLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.gitHubLinkLabel_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::PuyoTools.BitmapResources.Logo192;
            this.pictureBox1.Location = new System.Drawing.Point(19, 19);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(10, 10, 15, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(192, 249);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(222, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(216, 42);
            this.label13.TabIndex = 8;
            this.label13.Text = "Puyo Tools";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.Location = new System.Drawing.Point(226, 61);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.versionLabel.Size = new System.Drawing.Size(49, 23);
            this.versionLabel.TabIndex = 9;
            this.versionLabel.Text = "Version";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.gitHubLinkLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.licenseLinkLabel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 281);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 40);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // licenseLinkLabel
            // 
            this.licenseLinkLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.licenseLinkLabel.AutoSize = true;
            this.licenseLinkLabel.Location = new System.Drawing.Point(446, 13);
            this.licenseLinkLabel.Name = "licenseLinkLabel";
            this.licenseLinkLabel.Size = new System.Drawing.Size(44, 13);
            this.licenseLinkLabel.TabIndex = 5;
            this.licenseLinkLabel.TabStop = true;
            this.licenseLinkLabel.Text = "License";
            this.licenseLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.licenseLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.licenseLinkLabel_LinkClicked);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(229, 110);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(383, 158);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "Francis Gagné (FraGag)\r\nJohn Chadwick (nmn)\r\nLuke Zapart (drx)\r\nQPjDDYwQLI\r\nMicha" +
    "el Fadely\r\ntj32767\r\nGerbilSoft\r\nMainMemory\r\nnnn1590\r\nThomas May (WamWooWam)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 84);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.label2.Size = new System.Drawing.Size(255, 23);
            this.label2.TabIndex = 12;
            this.label2.Text = "Developed by Nick Woronekin with contributions by:";
            // 
            // AboutPuyoTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 321);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutPuyoTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Puyo Tools";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.LinkLabel gitHubLinkLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel licenseLinkLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
    }
}