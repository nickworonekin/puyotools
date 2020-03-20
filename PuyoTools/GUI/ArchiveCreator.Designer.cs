namespace PuyoTools.GUI
{
    partial class ArchiveCreator
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView = new System.Windows.Forms.ListViewWithReordering();
            this.numCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.archiveNameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.itemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.archiveSettingsPanel = new System.Windows.Forms.Panel();
            this.compressionFormatBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.archiveFormatBox = new System.Windows.Forms.ComboBox();
            this.settingsPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.itemContextMenu.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsPanel
            // 
            this.settingsPanel.Controls.Add(this.archiveFormatBox);
            this.settingsPanel.Controls.Add(this.label1);
            this.settingsPanel.Controls.Add(this.tabControl1);
            this.settingsPanel.Size = new System.Drawing.Size(584, 362);
            // 
            // runButton
            // 
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // fileListButton
            // 
            this.fileListButton.Enabled = false;
            this.fileListButton.Visible = false;
            // 
            // addFilesButton
            // 
            this.addFilesButton.Click += new System.EventHandler(this.addFilesButton_Click);
            // 
            // addDirectoryButton
            // 
            this.addDirectoryButton.Click += new System.EventHandler(this.addDirectoryButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(10, 38);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(564, 314);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(556, 288);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "File List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView
            // 
            this.listView.AllowDrop = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.numCol,
            this.fileCol,
            this.archiveNameCol});
            this.listView.ContextMenuStrip = this.itemContextMenu;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(3, 3);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(550, 282);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ClientSizeChanged += new System.EventHandler(this.listView_ClientSizeChanged);
            // 
            // numCol
            // 
            this.numCol.Text = "";
            this.numCol.Width = 50;
            // 
            // fileCol
            // 
            this.fileCol.Text = "File";
            this.fileCol.Width = 150;
            // 
            // archiveNameCol
            // 
            this.archiveNameCol.Text = "Filename in archive";
            this.archiveNameCol.Width = 150;
            // 
            // itemContextMenu
            // 
            this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.itemContextMenu.Name = "itemContextMenu";
            this.itemContextMenu.Size = new System.Drawing.Size(118, 48);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.archiveSettingsPanel);
            this.tabPage2.Controls.Add(this.compressionFormatBox);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(556, 188);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // archiveSettingsPanel
            // 
            this.archiveSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.archiveSettingsPanel.Location = new System.Drawing.Point(0, 30);
            this.archiveSettingsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.archiveSettingsPanel.Name = "archiveSettingsPanel";
            this.archiveSettingsPanel.Size = new System.Drawing.Size(556, 358);
            this.archiveSettingsPanel.TabIndex = 4;
            // 
            // compressionFormatBox
            // 
            this.compressionFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compressionFormatBox.FormattingEnabled = true;
            this.compressionFormatBox.Items.AddRange(new object[] {
            "No"});
            this.compressionFormatBox.Location = new System.Drawing.Point(105, 6);
            this.compressionFormatBox.Name = "compressionFormatBox";
            this.compressionFormatBox.Size = new System.Drawing.Size(150, 21);
            this.compressionFormatBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Compress Archive";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Archive Format";
            // 
            // archiveFormatBox
            // 
            this.archiveFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.archiveFormatBox.FormattingEnabled = true;
            this.archiveFormatBox.Items.AddRange(new object[] {
            "Select a format"});
            this.archiveFormatBox.Location = new System.Drawing.Point(94, 11);
            this.archiveFormatBox.Name = "archiveFormatBox";
            this.archiveFormatBox.Size = new System.Drawing.Size(250, 21);
            this.archiveFormatBox.TabIndex = 2;
            this.archiveFormatBox.SelectedIndexChanged += new System.EventHandler(this.archiveFormatBox_SelectedIndexChanged);
            // 
            // ArchiveCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 462);
            this.Name = "ArchiveCreator";
            this.Text = "Archive Creator";
            this.Controls.SetChildIndex(this.settingsPanel, 0);
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.itemContextMenu.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListViewWithReordering listView;
        private System.Windows.Forms.ColumnHeader numCol;
        private System.Windows.Forms.ColumnHeader fileCol;
        private System.Windows.Forms.ColumnHeader archiveNameCol;
        private System.Windows.Forms.ComboBox archiveFormatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip itemContextMenu;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ComboBox compressionFormatBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel archiveSettingsPanel;
    }
}
