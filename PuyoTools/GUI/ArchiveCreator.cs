using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PuyoTools.Modules.Archive;

// This is only needed until I make a nice rename form
using Microsoft.VisualBasic;
// Remove the above once I do that

namespace PuyoTools.GUI
{
    public partial class ArchiveCreator : ToolForm
    {
        List<SettingsPanel> formatSettingsPanel;
        List<ArchiveFormat> archiveFormats;
        List<CompressionFormat> compressionFormats;

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
            formatSettingsPanel = new List<SettingsPanel>();

            // Fill the archive format box
            archiveFormatBox.SelectedIndex = 0;
            archiveFormats = new List<ArchiveFormat>();
            foreach (KeyValuePair<ArchiveFormat, Archive.FormatEntry> format in Archive.Formats)
            {
                if (format.Value.Instance.CanCreate)
                {
                    archiveFormatBox.Items.Add(format.Value.Instance.Name);
                    archiveFormats.Add(format.Key);

                    SettingsPanel panel = new SettingsPanel(archiveSettingsPanel);

                    if (format.Value.SettingsInstanceGUI != null)
                    {
                        format.Value.SettingsInstanceGUI.SetPanelContent(panel);
                    }

                    formatSettingsPanel.Add(panel);
                }
            }

            // Fill the compression format box
            compressionFormatBox.SelectedIndex = 0;
            compressionFormats = new List<CompressionFormat>();
            foreach (KeyValuePair<CompressionFormat, Compression.FormatEntry> format in Compression.Formats)
            {
                if (format.Value.Instance.CanCompress)
                {
                    compressionFormatBox.Items.Add(format.Value.Instance.Name);
                    compressionFormats.Add(format.Key);
                }
            }
        }

        private void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.SourceFile = file;
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

        private void Run(Settings settings)
        {
            // For some archives, the file needs to be a specific format. As such,
            // they may be rejected when trying to add them. We'll store such files in
            // this list to let the user know they could not be added.
            List<FileEntry> FilesNotAdded = new List<FileEntry>();

            // Create the stream we are going to write the archive to
            Stream destination;
            if (settings.CompressionFormat == CompressionFormat.Unknown)
            {
                // We are not compression the archive. Write directly to the destination
                destination = File.Create(settings.OutFilename);
            }
            else
            {
                // We are compressing the archive. Write to a memory stream first.
                destination = new MemoryStream();
            }

            // Create the archive
            ArchiveWriter archive = Archive.Create(destination, settings.ArchiveFormat, settings.ArchiveSettings);

            // Add the files to the archive. We're going to do this in a try catch since
            // sometimes an exception may be thrown (namely if the archive cannot contain
            // the file the user is trying to add)
            foreach (ListViewItem item in listView.Items)
            {
                FileEntry entry = (FileEntry)item.Tag;

                try
                {
                    archive.AddFile(File.OpenRead(entry.SourceFile), entry.FilenameInArchive, entry.SourceFile);
                }
                catch (CannotAddFileToArchiveException)
                {
                    FilesNotAdded.Add(entry);
                }
            }

            // If filesNotAdded is not empty, then show a message to the user
            // and ask them if they want to continue
            if (FilesNotAdded.Count > 0)
            {
                // ...
            }

            // Finish writing the archive
            archive.Flush();

            // Do we want to compress this archive?
            if (settings.CompressionFormat != CompressionFormat.Unknown)
            {
                destination.Position = 0;

                using (FileStream outStream = File.Create(settings.OutFilename))
                {
                    Compression.Compress(destination, outStream, (int)destination.Length, Path.GetFileName(settings.OutFilename), settings.CompressionFormat);
                }
            }

            destination.Close();

            // The tool is finished doing what it needs to do. We can close it now.
            this.Close();
        }

        private struct FileEntry
        {
            public string SourceFile;
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
                if (MessageBox.Show("Include files within subdirectories?", "Subdirectories", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            archiveSettingsPanel.Controls.Clear();

            if (archiveFormatBox.SelectedIndex != 0)
            {
                archiveSettingsPanel.Controls.Add(formatSettingsPanel[archiveFormatBox.SelectedIndex - 1]);
            }

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

            EnableRunButton();
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
        public class SettingsPanel : Panel
        {
            int yPos = 0;

            public SettingsPanel(Panel parent)
            {
                this.Dock = DockStyle.Fill;
            }

            // Add a combo box (with a label) to the panel
            public void AddComboBox(out ComboBox comboBox, string labelText, string[] choices, ComboBoxStyle style)
            {
                Label label = new Label();
                label.Location = new Point(this.Margin.Left + label.Margin.Left, yPos + 3);
                label.AutoSize = true;
                label.Text = labelText;
                this.Controls.Add(label);

                comboBox = new ComboBox();
                comboBox.Location = new Point(label.Left + label.Width + label.Margin.Right + comboBox.Margin.Left, yPos);
                comboBox.Width = 150;
                comboBox.DropDownStyle = style;
                comboBox.Items.AddRange(choices);
                comboBox.SelectedIndex = 0;
                this.Controls.Add(comboBox);

                yPos += 24;
            }

            // Add a checkbox to the panel
            public void AddCheckBox(out CheckBox checkBox, string text)
            {
                checkBox = new CheckBox();
                checkBox.Location = new Point(this.Margin.Left + checkBox.Margin.Left, yPos);
                checkBox.AutoSize = true;
                checkBox.Text = text;
                this.Controls.Add(checkBox);

                yPos += 24;
            }

            // Add radio buttons (with a label) to the panel
            public void AddRadioButtons(out RadioButton[] radioButtons, string labelText, string[] choices)
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

                radioButtons = new RadioButton[choices.Length];
                for (int i = 0; i < choices.Length; i++)
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

                radioButtons[0].Checked = true;

                yPos += 4;
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Get the format of the archive the user wants to create
            ArchiveFormat archiveFormat = archiveFormats[archiveFormatBox.SelectedIndex - 1];
            string fileExtension = (Archive.Formats[archiveFormat].Instance.FileExtension != String.Empty ? Archive.Formats[archiveFormat].Instance.FileExtension : ".*");

            // Prompt the user to save the archive
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Archive";
            sfd.Filter = Archive.Formats[archiveFormat].Instance.Name + " Archive (*" + fileExtension + ")|*" + fileExtension + "|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Disable the form
                this.Enabled = false;

                Settings settings = new Settings();
                settings.ArchiveFormat = archiveFormat;
                settings.OutFilename = sfd.FileName;

                if (compressionFormatBox.SelectedIndex != 0)
                {
                    settings.CompressionFormat = compressionFormats[compressionFormatBox.SelectedIndex - 1];
                }
                else
                {
                    settings.CompressionFormat = CompressionFormat.Unknown;
                }

                settings.ArchiveSettings = Archive.Formats[archiveFormat].Instance.GetWriterSettings();
                if (Archive.Formats[archiveFormat].SettingsInstanceGUI != null)
                {
                    Archive.Formats[archiveFormat].SettingsInstanceGUI.SetSettings(settings.ArchiveSettings);
                }

                Run(settings);
            }
        }

        private struct Settings
        {
            public ArchiveFormat ArchiveFormat;
            public CompressionFormat CompressionFormat;
            public string OutFilename;
            public ArchiveWriterSettings ArchiveSettings;
        }
    }
}
