using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

// This is only needed until I make a nice rename form
using Microsoft.VisualBasic;
// Remove the above once I do that

namespace PuyoTools.GUI
{
    public partial class ArchiveCreator : ToolForm
    {
        List<Panel> formatSettingsPanel;

        public ArchiveCreator()
        {
            InitializeComponent();

            // Remove event handlers from the base class
            addFilesButton.Click -= base.addFilesButton_Click;
            addDirectoryButton.Click -= base.addDirectoryButton_Click;

            // Make the list view rows bigger
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(1, 20);
            listView.SmallImageList = imageList;

            // Resize the column widths
            listView_ClientSizeChanged(null, null);

            // Set up the format settings panels
            formatSettingsPanel = new List<Panel>();

            // Fill the archive format box
            archiveFormatBox.SelectedIndex = 0;
            foreach (KeyValuePair<ArchiveFormat, PTArchive.FormatEntry> format in PTArchive.Formats)
            {
                if (format.Value.Instance.CanCreate())
                {
                    archiveFormatBox.Items.Add(format.Value.Name);

                    InitalizeSettingsPanel(format.Key);
                }
            }

            // Fill the compression format box
            compressionFormatBox.SelectedIndex = 0;
            foreach (KeyValuePair<CompressionFormat, PTCompression.FormatEntry> format in PTCompression.Formats)
            {
                if (format.Value.Instance.CanCompress())
                {
                    compressionFormatBox.Items.Add(format.Value.Name);
                }
            }

            SettingsPanel p = new SettingsPanel(archiveSettingsPanel);
            p.AddComboBox("Block Size", new string[] { "16", "2048" }, ComboBoxStyle.DropDown);
            p.AddRadioButtons("AFS Version", new string[] { "v1 (Dreamcast)", "v2" });
            p.AddCheckBox("Choice #1");
            p.AddCheckBox("Choice #2");
            p.AddCheckBox("Choice #3");
        }

        private void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.Filename = Path.GetFileName(file);
                fileEntry.FilenameInArchive = fileEntry.Filename;

                ListViewItem item = new ListViewItem(new string[] {
                    (listView.Items.Count + 1).ToString(),
                    fileEntry.Filename,
                    fileEntry.FilenameInArchive,
                });

                item.Tag = fileEntry;

                listView.Items.Add(item);
            }
        }

        private void EnableRunButton()
        {
            runButton.Enabled = (listView.Items.Count > 0 && archiveFormatBox.SelectedIndex > 0);
        }

        private void InitalizeSettingsPanel(ArchiveFormat format)
        {
        }

        private struct FileEntry
        {
            public string Filename;
            public string FilenameInArchive;
        }

        private void listView_ClientSizeChanged(object sender, EventArgs e)
        {
            int columnWidth = Math.Max(150, (listView.ClientSize.Width - 20 - listView.Columns[0].Width) / 2);
            listView.Columns[1].Width = columnWidth;
            listView.Columns[2].Width = columnWidth;
        }

        private new void addFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.Multiselect = true;
            ofd.Title = "Add Files";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddFiles(ofd.FileNames);

                EnableRunButton();
            }
        }

        private new void addDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a directory.";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Include files within sub-directories?", "Subdirectories", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    AddFiles(Directory.GetFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories));
                }
                else
                {
                    AddFiles(Directory.GetFiles(fbd.SelectedPath));
                }

                EnableRunButton();
            }
        }

        private void archiveFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableRunButton();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The first index in the selected indices.
            // We'll need it for later. I'll explain it then
            int firstIndex = 0;

            // Loop through each selected item (not index)
            foreach (ListViewItem item in listView.SelectedItems)
            {
                // Check to see if this is the first index selected.
                if (item.Index > firstIndex)
                {
                    firstIndex = item.Index;
                }

                listView.Items.Remove(item);
            }

            // Now reorder each item
            for (int i = firstIndex; i < listView.Items.Count; i++)
            {
                listView.Items[i].SubItems[0].Text = (i + 1).ToString();
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TEMP! Use Visual Basic's InputBox
            string fname = Interaction.InputBox("Rename selected files to", "Rename Files");

            if (fname != String.Empty)
            {
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    item.SubItems[2].Text = fname;
                    FileEntry fileEntry = (FileEntry)item.Tag;
                    fileEntry.FilenameInArchive = fname;
                }
            }
        }

        // A settings panel inside the settings panel. Isn't that great!
        // Actually, it's just so we don't need to use a million different panels
        // in the form designer.
        private class SettingsPanel : Panel
        {
            int yPos = 0;

            public SettingsPanel(Panel parent)
            {
                this.Dock = DockStyle.Fill;
                parent.Controls.Add(this);
            }

            // Add a combo box (with a label) to the panel
            public ComboBox AddComboBox(string labelText, string[] choices, ComboBoxStyle style)
            {
                Label label = new Label();
                label.Location = new Point(this.Margin.Left + label.Margin.Left, yPos + 3);
                label.AutoSize = true;
                label.Text = labelText;
                this.Controls.Add(label);

                ComboBox comboBox = new ComboBox();
                comboBox.Location = new Point(label.Left + label.Width + label.Margin.Right + comboBox.Margin.Left, yPos);
                comboBox.Width = 150;
                comboBox.DropDownStyle = style;
                comboBox.Items.AddRange(choices);
                this.Controls.Add(comboBox);

                yPos += 24;

                return comboBox;
            }

            // Add a checkbox to the panel
            public CheckBox AddCheckBox(string text)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Location = new Point(this.Margin.Left + checkBox.Margin.Left, yPos);
                checkBox.AutoSize = true;
                checkBox.Text = text;
                this.Controls.Add(checkBox);

                yPos += 24;

                return checkBox;
            }

            // Add radio buttons (with a label) to the panel
            public RadioButton[] AddRadioButtons(string labelText, string[] choices)
            {
                Label label = new Label();
                label.Location = new Point(this.Margin.Left + label.Margin.Left, yPos + 3);
                label.AutoSize = true;
                label.Text = labelText;
                this.Controls.Add(label);

                yPos += 24;

                Panel panel = new Panel();
                panel.Location = new Point(0, yPos);
                panel.Size = new Size(this.Width, 0);
                panel.AutoSize = true;
                this.Controls.Add(panel);

                // Add the radio buttons to the panel
                int panelYPos = 0;
                RadioButton[] radioButtons = new RadioButton[choices.Length];
                for (int i = 0; i < radioButtons.Length; i++)
                {
                    radioButtons[i] = new RadioButton();
                    radioButtons[i].Location = new Point(this.Margin.Left + radioButtons[i].Margin.Left, panelYPos);
                    radioButtons[i].Padding = new Padding(20, 0, 0, 0);
                    radioButtons[i].Text = choices[i];
                    radioButtons[i].AutoSize = true;
                    panel.Controls.Add(radioButtons[i]);

                    panelYPos += 20;
                    yPos += 20;
                }

                yPos += 4;

                return radioButtons;
            }
        }
    }
}
