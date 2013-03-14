using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace PuyoTools.Old
{
    public class ArchiveCreator : Form
    {
        /*
        // Set up form variables
        private Panel PanelContent;

        private ComboBox[]
            blockSizes; // Block Sizes

        private Button
            startWorkButton = new Button(); // Start Work Button

        private ListViewWithReordering
            archiveFileList = new ListViewWithReordering(); // Contents of Archive

        private string[] fileList, archiveFnames; // Used for later

        private StatusMessage
            status; // Status Message

        private Form renameDialog;
        private TextBox renameTextBox;
        private bool renameAll;

        private ToolStripButton renameAllFiles;

        private TabControl settingsBox;
        private TabPage[] settingsPages;

        // Archive & Compression Information
        private List<string> ArchiveNames           = new List<string>();
        private List<ArchiveFormat> ArchiveFormats  = new List<ArchiveFormat>();
        private List<int[]> ArchiveBlockSizes       = new List<int[]>();
        private List<List<Control>> ArchiveSettings = new List<List<Control>>();
        private List<string[]> ArchiveFilters       = new List<string[]>();

        private List<string> CompressionNames              = new List<string>();
        private List<CompressionFormat> CompressionFormats = new List<CompressionFormat>();

        // Archive & Compression Formats
        ToolStripComboBox archiveFormatList;
        ToolStripComboBox compressionFormatList;

        public ArchiveCreator()
        {
            // Set up the form
            FormContent.Create(this, "Archive Creator", new Size(680, 400));
            PanelContent = new Panel()
            {
                Location = new Point(0, 0),
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Height,
            };
            this.Controls.Add(PanelContent);
            this.MaximizeBox = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            PanelContent.Dock = DockStyle.Fill;

            // Fill the archive & compression formats
            InitalizeArchiveFormats();
            InitalizeCompressionFormats();

            // Create the combobox that contains the archive formats
            archiveFormatList               = new ToolStripComboBox();
            archiveFormatList.DropDownStyle = ComboBoxStyle.DropDownList;
            archiveFormatList.Items.AddRange(ArchiveNames.ToArray());

            archiveFormatList.SelectedIndex         = 0;
            archiveFormatList.MaxDropDownItems      = archiveFormatList.Items.Count;
            archiveFormatList.SelectedIndexChanged += new EventHandler(ChangeArchiveFormat);

            // Create the combobox that contains the compression formats
            compressionFormatList               = new ToolStripComboBox();
            compressionFormatList.DropDownStyle = ComboBoxStyle.DropDownList;
            compressionFormatList.Items.Add("Do Not Compress");
            compressionFormatList.Items.AddRange(CompressionNames.ToArray());

            compressionFormatList.SelectedIndex    = 0;
            compressionFormatList.MaxDropDownItems = compressionFormatList.Items.Count;

            renameAllFiles = new ToolStripButton("Rename All", null, ShowRenameDialog);

            // Create the toolstrip
            ToolStrip toolStrip = new ToolStrip(new ToolStripItem[] {
                new ToolStripButton("Add", IconResources.New, AddFiles),
                new ToolStripSeparator(),
                new ToolStripLabel("Archive:"),
                archiveFormatList,
                new ToolStripSeparator(),
                new ToolStripLabel("Compression:"),
                compressionFormatList,
                new ToolStripSeparator(),
                renameAllFiles,
            });
            toolStrip.Dock = DockStyle.Top;
            PanelContent.Controls.Add(toolStrip);

            // Create the right click menu
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Rename", null, ShowRenameDialog),
                new ToolStripMenuItem("Delete", null, Remove),
            });
            archiveFileList.ContextMenuStrip = contextMenu;

            // Archive Contents
            FormContent.Add(PanelContent, archiveFileList,
                new string[] { "#", "Source Filename", "Filename in the Archive" },
                new int[] { 48, 170, 170 },
                new Point(8, 32),
                new Size(420, 328));

            // Quick hack for setting height of list view items
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(1, 20);
            archiveFileList.SmallImageList = imageList;

            archiveFileList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            archiveFileList.AllowDrop = true;
            archiveFileList.DragDrop += archiveFileList_DragDrop;

            // Settings Box
            settingsBox = new TabControl() {
                Location = new Point(436, 32),
                Size     = new Size(236, 328),
            };

            // Set up the settings
            settingsPages = new TabPage[ArchiveNames.Count];
            blockSizes    = new ComboBox[ArchiveBlockSizes.Count];
            for (int i = 0; i < ArchiveNames.Count; i++)
            {
                settingsPages[i] = new TabPage(ArchiveNames[i] + " Settings") {
                    UseVisualStyleBackColor = true,
                };

                // Add block size
                settingsPages[i].Controls.Add(new Label() {
                    Text     = "Block Size",
                    Location = new Point(8, 8),
                    Size     = new Size(64, 16),
                });

                // Now add the entries
                blockSizes[i] = new ComboBox() {
                    Location      = new Point(72, 4),
                    Size          = new Size(64, 16),
                    DropDownStyle = ComboBoxStyle.DropDown,
                    MaxLength     = 4,
                };
                blockSizes[i].KeyPress += CheckBlockSizeText;

                for (int j = 0; j < ArchiveBlockSizes[i].Length; j++)
                {
                    // If the last element is -1, we aren't allowed to edit it
                    if (ArchiveBlockSizes[i][j] == -1)
                    {
                        blockSizes[i].Enabled = false;
                        break;
                    }
                    blockSizes[i].Items.Add(ArchiveBlockSizes[i][j].ToString());
                }

                blockSizes[i].SelectedIndex    = 0;
                blockSizes[i].MaxDropDownItems = blockSizes[i].Items.Count;
                settingsPages[i].Controls.Add(blockSizes[i]);

                int yPos = 0;
                for (int j = 0; j < ArchiveSettings[i].Count; j++)
                {
                    // See if this is a panel, which can contain multiple items
                    // so we can set the space between everything properly
                    int ItemsInOption = 1;
                    if (ArchiveSettings[i][j] is Panel)
                    {
                        ItemsInOption = ArchiveSettings[i][j].Controls.Count;
                        for (int k = 0; k < ItemsInOption; k++)
                        {
                            ArchiveSettings[i][j].Controls[k].Location = new Point(0, k * 20);
                            ArchiveSettings[i][j].Controls[k].Size     = new Size(settingsBox.Size.Width - 16, 16);
                        }
                    }

                    ArchiveSettings[i][j].Location = new Point(8, 32 + yPos);
                    ArchiveSettings[i][j].Size     = new Size(settingsBox.Size.Width - 16, 16 * ItemsInOption + ((ItemsInOption - 1) * 8));
                    settingsPages[i].Controls.Add(ArchiveSettings[i][j]);

                    yPos += (ItemsInOption * 20) + 4;
                }
            }

            PanelContent.Controls.Add(settingsBox);
            settingsBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            settingsBox.TabPages.Add(settingsPages[0]);

            // Create Archive
            FormContent.Add(PanelContent, startWorkButton,
                "Create Archive",
                new Point((PanelContent.Width / 2) - 60, 368),
                new Size(120, 24),
                StartWork);

            startWorkButton.Anchor = AnchorStyles.Bottom;

            this.ShowDialog();
        }

        void archiveFileList_DragDrop(object sender, DragEventArgs e)
        {
            for (int i = 0; i < archiveFileList.Items.Count; i++)
            {
                archiveFileList.Items[i].SubItems[0].Text = (i + 1).ToString("#,0");
            }
        }

        // Start Work
        private void StartWork(object sender, EventArgs e)
        {
            // Make sure we have files
            if (archiveFileList.Items.Count == 0)
                return;

            // Get output filename
            string outFname = FileSelectionDialog.SaveFile("Create Archive", String.Empty, String.Format("{0} ({1})|{1}", ArchiveFilters[archiveFormatList.SelectedIndex][0], ArchiveFilters[archiveFormatList.SelectedIndex][1]));
            if (outFname == null || outFname == String.Empty)
                return;

            // Set up our background worker
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object sender2, DoWorkEventArgs e2) {
                Run(outFname);
            };
            //bw.DoWork += run;

            // Create a file listing and list of archive file names for the header
            fileList = new string[archiveFileList.Items.Count];
            archiveFnames = new string[archiveFileList.Items.Count];

            for (int i = 0; i < archiveFileList.Items.Count; i++)
            {
                fileList[i] = ((ArchiveEntry)archiveFileList.Items[i].Tag).fname;
                archiveFnames[i] = ((ArchiveEntry)archiveFileList.Items[i].Tag).archiveFname;
            }

            // Now, show our status
            status = new StatusMessage("Archive - Create", fileList);
            status.Show();
            //status.Visible = false;

            bw.RunWorkerAsync();
        }

        // Create the archive
        //private void run(object sender, DoWorkEventArgs e)
        private void Run(string outFname)
        {
            try
            {
                // Get archive and compression format
                int ArchiveFormatIndex = archiveFormatList.SelectedIndex;
                int CompressionFormatIndex = compressionFormatList.SelectedIndex;

                // Disable the window and show the status box
                PanelContent.Enabled = false;

                // FORCE AFS FOR NOW
                using (FileStream outputStream = new FileStream(outFname, FileMode.Create, FileAccess.ReadWrite))
                {
                    PuyoTools.Archive.ArchiveWriter archive = PuyoTools.Archive.Archive.Create(outputStream, PuyoTools.Archive.ArchiveFormat.AFS, new PuyoTools.Archive.ArchiveWriterSettings());

                    for (int i = 0; i < archiveFileList.Items.Count; i++)
                    {
                        status.CurrentFile = i;
                        archive.AddFile(File.OpenRead(fileList[i]), archiveFnames[i], File.GetCreationTime(fileList[i]));
                    }

                    archive.Flush();
                }


                // Start creating the archive
                /*
                Archive archive = new Archive(ArchiveFormats[archiveFormatList.SelectedIndex], Path.GetFileName(outFname));

                using (FileStream outputStream = new FileStream(outFname, FileMode.Create, FileAccess.ReadWrite))
                {

                    // Get the block size
                    int blockSize;
                    if (blockSizes[ArchiveFormatIndex].Enabled && blockSizes[ArchiveFormatIndex].Text.Length > 0)
                        blockSize = int.Parse(blockSizes[ArchiveFormatIndex].Text);
                    else
                        blockSize = ArchiveBlockSizes[ArchiveFormatIndex][0];

                    // Create and write the header
                    bool[] settings = GetSettings(ArchiveFormatIndex);
                    uint[] offsetList;
                    MemoryStream header = archive.CreateHeader(fileList, archiveFnames, blockSize, settings, out offsetList);
                    outputStream.Write(header);

                    // Add the files
                    for (int i = 0; i < archiveFileList.Items.Count; i++)
                    {
                        // Set the current file
                        status.CurrentFile = i;

                        // Pad file so we can have the correct block offset
                        while (outputStream.Position < offsetList[i])
                            outputStream.WriteByte(archive.PaddingByte);

                        using (Stream inputStream = new FileStream(fileList[i], FileMode.Open, FileAccess.Read))
                        {
                            // Format the file so we can add it
                            Stream inputFile = inputStream;
                            inputFile = archive.FormatFileToAdd(inputFile);
                            if (inputFile == null)
                                throw new Exception();

                            // Write the data to the file
                            outputStream.Write(inputFile);
                        }
                    }

                    // Pad file so we can have the correct block offset
                    while (outputStream.Position % blockSize != 0)
                        outputStream.WriteByte(archive.PaddingByte);

                    // Write the footer
                    MemoryStream footer = archive.CreateFooter(fileList, archiveFnames, blockSize, settings, header);
                    if (footer != null)
                    {
                        outputStream.Write(footer);

                        // Pad file so we can have the correct block offset
                        while (outputStream.Position % blockSize != 0)
                            outputStream.WriteByte(archive.PaddingByte);
                    }

                    // Compress the data if we want to
                    if (compressionFormatList.SelectedIndex != 0)
                    {
                        Compression compression = new Compression(outputStream, outFname, CompressionFormats[CompressionFormatIndex - 1]);
                        MemoryStream compressedData = compression.Compress();
                        if (compressedData != null)
                        {
                            // Clear the output stream and write the compressed data
                            outputStream.Position = 0;
                            outputStream.SetLength(compressedData.Length);
                            outputStream.Write(compressedData);
                        }
                    }
                }*/ /*

                this.Close();
            }
            catch
            {
                this.Close();
            }
            finally
            {
                status.Close();
            }
        }

        // Change Archive Format
        private void ChangeArchiveFormat(object sender, EventArgs e)
        {
            settingsBox.TabPages.Clear();
            settingsBox.TabPages.Add(settingsPages[archiveFormatList.SelectedIndex]);
        }

        // Add files
        private void AddFiles(object sender, EventArgs e)
        {
            string[] files = FileSelectionDialog.OpenFiles("Select Files", "All Files (*.*)|*.*");

            if (files == null || files.Length == 0)
                return;

            // Add the files now
            foreach (string file in files)
            {
                ArchiveEntry entry = new ArchiveEntry();
                entry.fname = file;
                entry.sourceFname = Path.GetFileName(file);
                entry.archiveFname = entry.sourceFname;

                ListViewItem item = new ListViewItem(new string[] {
                    archiveFileList.Items.Count.ToString("#,0"),
                    entry.sourceFname,
                    entry.archiveFname,
                });
                item.Tag = entry;
                archiveFileList.Items.Add(item);
            }
        }

        // Remove files
        private void Remove(object sender, EventArgs e)
        {
            // Make sure we selected something
            if (archiveFileList.SelectedIndices.Count == 0)
                return;

            // Remove items
            foreach (int item in archiveFileList.SelectedIndices)
            {
                //archiveEntries.RemoveAt(item);
                archiveFileList.Items.RemoveAt(item);
            }

            // New item numbers
            for (int i = 0; i < archiveFileList.Items.Count; i++)
            {
                archiveFileList.Items[i].SubItems[0].Text = (i + 1).ToString();
            }
        }

        // Show rename dialog
        private void ShowRenameDialog(object sender, EventArgs e)
        {
            // Make sure we have items first
            if (archiveFileList.Items.Count == 0)
                return;

            // If we want to rename all, select all the items
            if (sender == renameAllFiles)
                renameAll = true;
            else if (archiveFileList.SelectedIndices.Count == 0)
                return;

            renameDialog  = new Form();
            renameTextBox = new TextBox();
            string selectedFilename = String.Empty;

            if (!renameAll)
                selectedFilename = ((ArchiveEntry)archiveFileList.Items[archiveFileList.SelectedIndices[0]].Tag).archiveFname;

            FormContent.Create(renameDialog,
                "Rename Files",
                new Size(320, 100), false);

            // Rename this to
            FormContent.Add(renameDialog, new Label(),
                "Rename selected files to:",
                new Point(8, 8),
                new Size(304, 32));

            // Display Text Box
            FormContent.Add(renameDialog, renameTextBox,
                selectedFilename,
                new Point(8, 40),
                new Size(304, 24));

            // Display Rename Button
            FormContent.Add(renameDialog, new Button(),
                "Rename",
                new Point(94, 68),
                new Size(64, 24),
                Rename);

            // Display Cancel Button
            FormContent.Add(renameDialog, new Button(),
                "Cancel",
                new Point(162, 68),
                new Size(64, 24),
                CloseRenameDialog);

            renameDialog.ShowDialog();
        }

        // Rename files
        private void Rename(object sender, EventArgs e)
        {
            // Rename selected files, or all files
            if (renameAll)
            {
                for (int i = 0; i < archiveFileList.Items.Count; i++)
                {
                    ((ArchiveEntry)archiveFileList.Items[i].Tag).archiveFname = renameTextBox.Text;
                    archiveFileList.Items[i].SubItems[2].Text = renameTextBox.Text;
                }

                renameAll = false;
            }
            else
            {
                foreach (int item in archiveFileList.SelectedIndices)
                {
                    ((ArchiveEntry)archiveFileList.Items[item].Tag).archiveFname = renameTextBox.Text;
                    archiveFileList.Items[item].SubItems[2].Text = renameTextBox.Text;
                }
            }

            renameDialog.Close();
        }

        // Close rename dialog
        private void CloseRenameDialog(object sender, EventArgs e)
        {
            renameDialog.Close();
        }

        // Check block size value
        private void CheckBlockSizeText(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // Initalize Archive Formats
        private void InitalizeArchiveFormats()
        {
            foreach (KeyValuePair<ArchiveFormat, ArchiveModule> value in Archive.Dictionary)
            {
                if (value.Value.CanPack)
                {
                    // Since we can pack this format, add it to the list
                    ArchiveNames.Add(value.Value.Name);
                    ArchiveFormats.Add(value.Key);
                    ArchiveBlockSizes.Add(value.Value.PackSettings.BlockSizes);
                    ArchiveSettings.Add(value.Value.PackSettings.Settings);
                    ArchiveFilters.Add(value.Value.Filter);
                }
            }
        }

        // Initalize Compression Formats
        private void InitalizeCompressionFormats()
        {
            foreach (KeyValuePair<CompressionFormat, CompressionModule> value in Compression.Dictionary)
            {
                if (value.Value.CanCompress)
                {
                    // Since we can compress this format, add it to the list
                    CompressionNames.Add(value.Value.Name);
                    CompressionFormats.Add(value.Key);
                }
            }
        }

        // Get Settings from the archive format
        private bool[] GetSettings(int format)
        {
            bool[] settings = new bool[ArchiveSettings[format].Count];

            for (int i = 0; i < settings.Length; i++)
            {
                // If this is a panel, we check if the first option was checked
                // For now, assume if it's in a panel it contains radio buttons
                if (ArchiveSettings[format][i] is Panel)
                    settings[i] = (ArchiveSettings[format][i].Controls[0] as RadioButton).Checked;
                else
                    settings[i] = (ArchiveSettings[format][i] as CheckBox).Checked;
            }

            return settings;
        }

        private class ArchiveEntry
        {
            public string fname;
            public string sourceFname;
            public string archiveFname;
        }
                     * */
    }
}