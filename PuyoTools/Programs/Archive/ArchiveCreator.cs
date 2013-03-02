using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Extensions;

namespace PuyoTools
{
    public class ArchiveCreator : Form
    {
        // Set up form variables
        private Panel PanelContent;

        private ComboBox[]
            blockSizes; // Block Sizes

        private Button
            startWorkButton = new Button(); // Start Work Button

        private ListView
            archiveFileList = new ListView(); // Contents of Archive

        private List<string>
            sourceFilenames  = new List<string>(), // List of source filenames
            archiveFilenames = new List<string>(); // List of filenames in the archive

        private List<string>
            fileList = new List<string>(); // Files

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

            // Fill the archive & compression formats
            InitalizeArchiveFormats();
            InitalizeCompressionFormats();

            // Create the combobox that contains the archive formats
            archiveFormatList               = new ToolStripComboBox();
            archiveFormatList.DropDownStyle = ComboBoxStyle.DropDownList;
            archiveFormatList.Items.AddRange(ArchiveNames.ToArray());

            archiveFormatList.SelectedIndex         = 0;
            archiveFormatList.MaxDropDownItems      = archiveFormatList.Items.Count;
            archiveFormatList.SelectedIndexChanged += new EventHandler(changeArchiveFormat);

            // Create the combobox that contains the compression formats
            compressionFormatList               = new ToolStripComboBox();
            compressionFormatList.DropDownStyle = ComboBoxStyle.DropDownList;
            compressionFormatList.Items.Add("Do Not Compress");
            compressionFormatList.Items.AddRange(CompressionNames.ToArray());

            compressionFormatList.SelectedIndex    = 0;
            compressionFormatList.MaxDropDownItems = compressionFormatList.Items.Count;

            renameAllFiles = new ToolStripButton("Rename All", null, rename);

            // Create the toolstrip
            ToolStrip toolStrip = new ToolStrip(new ToolStripItem[] {
                new ToolStripButton("Add", IconResources.New, addFiles),
                new ToolStripSeparator(),
                new ToolStripLabel("Archive:"),
                archiveFormatList,
                new ToolStripSeparator(),
                new ToolStripLabel("Compression:"),
                compressionFormatList,
                new ToolStripSeparator(),
                renameAllFiles,
            });
            PanelContent.Controls.Add(toolStrip);

            // Create the right click menu
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Rename", null, rename),
                new ToolStripMenuItem("Delete", null, delete),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Move Up", null, moveUp),
                new ToolStripMenuItem("Move Down", null, moveDown),
            });
            archiveFileList.ContextMenuStrip = contextMenu;

            // Archive Contents
            FormContent.Add(PanelContent, archiveFileList,
                new string[] { "#", "Source Filename", "Filename in the Archive" },
                new int[] { 48, 170, 170 },
                new Point(8, 32),
                new Size(420, 328));

            // Quick hack for setting height of list view items
            archiveFileList.SmallImageList = new ImageList() {
                ImageSize = new Size(1, 16),
            };

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
            settingsBox.TabPages.Add(settingsPages[0]);

            // Create Archive
            FormContent.Add(PanelContent, startWorkButton,
                "Create Archive",
                new Point((PanelContent.Width / 2) - 60, 368),
                new Size(120, 24),
                startWork);

            this.ShowDialog();
        }

        // Start Work
        private void startWork(object sender, EventArgs e)
        {
            // Make sure we have files
            if (fileList.Count == 0)
                return;

            // Get output filename
            string output_filename = FileSelectionDialog.SaveFile("Create Archive", String.Empty, String.Format("{0} ({1})|{1}", ArchiveFilters[archiveFormatList.SelectedIndex][0], ArchiveFilters[archiveFormatList.SelectedIndex][1]));
            if (output_filename == null || output_filename == String.Empty)
                return;

            // Set up our background worker
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += delegate(object sender2, DoWorkEventArgs e2) {
                run(output_filename);
            };
            //bw.DoWork += run;

            // Now, show our status
            status = new StatusMessage("Archive - Create", fileList.ToArray());
            status.Show();
            //status.Visible = false;

            bw.RunWorkerAsync();
        }

        // Create the archive
        //private void run(object sender, DoWorkEventArgs e)
        private void run(string output_filename)
        {
            try
            {
                // Get archive and compression format
                int ArchiveFormatIndex = archiveFormatList.SelectedIndex;
                int CompressionFormatIndex = compressionFormatList.SelectedIndex;

                // Get output filename
                //string output_filename = FileSelectionDialog.SaveFile("Create Archive", String.Empty, String.Format("{0} ({1})|{1}", ArchiveFilters[archiveFormatList.SelectedIndex][0], ArchiveFilters[archiveFormatList.SelectedIndex][1]));
                //if (output_filename == null || output_filename == String.Empty)
                //    return;

                // Disable the window and show the status box
                PanelContent.Enabled = false;
                //status.Visible = true;

                // Start creating the archive
                Archive archive = new Archive(ArchiveFormats[archiveFormatList.SelectedIndex], Path.GetFileName(output_filename));

                using (FileStream outputStream = new FileStream(output_filename, FileMode.Create, FileAccess.ReadWrite))
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
                    MemoryStream header = archive.CreateHeader(fileList.ToArray(), archiveFilenames.ToArray(), blockSize, settings, out offsetList);
                    outputStream.Write(header);

                    // Add the files
                    for (int i = 0; i < fileList.Count; i++)
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
                    MemoryStream footer = archive.CreateFooter(fileList.ToArray(), archiveFilenames.ToArray(), blockSize, settings, header);
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
                        Compression compression = new Compression(outputStream, output_filename, CompressionFormats[CompressionFormatIndex - 1]);
                        MemoryStream compressedData = compression.Compress();
                        if (compressedData != null)
                        {
                            // Clear the output stream and write the compressed data
                            outputStream.Position = 0;
                            outputStream.SetLength(compressedData.Length);
                            outputStream.Write(compressedData);
                        }
                    }
                }

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
        private void changeArchiveFormat(object sender, EventArgs e)
        {
            settingsBox.TabPages.Clear();
            settingsBox.TabPages.Add(settingsPages[archiveFormatList.SelectedIndex]);
        }

        // Add files
        private void addFiles(object sender, EventArgs e)
        {
            string[] files = FileSelectionDialog.OpenFiles("Select Files", "All Files (*.*)|*.*");

            if (files == null || files.Length == 0)
                return;

            // Add the files now
            foreach (string file in files)
            {
                fileList.Add(file);
                sourceFilenames.Add(Path.GetFileName(file));
                archiveFilenames.Add(Path.GetFileName(file));

                archiveFileList.Items.Add(new ListViewItem(new string[] {
                    fileList.Count.ToString("#,0"),
                    sourceFilenames[sourceFilenames.Count - 1],
                    archiveFilenames[archiveFilenames.Count - 1],
                }));
            }
        }

        // Remove files
        private void delete(object sender, EventArgs e)
        {
            // Make sure we selected something
            if (archiveFileList.SelectedIndices.Count == 0)
                return;

            // Remove items
            foreach (int item in archiveFileList.SelectedIndices)
            {
                fileList.RemoveAt(item);
                sourceFilenames.RemoveAt(item);
                archiveFilenames.RemoveAt(item);
            }

            // Populate the list again
            populateList();
        }

        // Rename files
        private void rename(object sender, EventArgs e)
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
                selectedFilename = archiveFilenames[archiveFileList.SelectedIndices[0]];

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
                doRename);

            // Display Cancel Button
            FormContent.Add(renameDialog, new Button(),
                "Cancel",
                new Point(162, 68),
                new Size(64, 24),
                cancelRename);

            renameDialog.ShowDialog();
        }

        // Do rename
        private void doRename(object sender, EventArgs e)
        {
            // Rename selected files, or all files
            if (renameAll)
            {
                for (int i = 0; i < archiveFileList.Items.Count; i++)
                    archiveFilenames[i] = renameTextBox.Text;

                renameAll = false;
            }
            else
            {
                foreach (int item in archiveFileList.SelectedIndices)
                    archiveFilenames[item] = renameTextBox.Text;
            }

            renameDialog.Close();

            // Populate the list
            populateList();
        }

        // Cancel rename
        private void cancelRename(object sender, EventArgs e)
        {
            renameDialog.Close();
        }

        // Move item up in the list
        private void moveUp(object sender, EventArgs e)
        {
            // Make sure we selected stuff
            if (archiveFileList.SelectedIndices.Count == 0)
                return;

            // Make sure we didn't select the first one
            if (archiveFileList.SelectedIndices.Contains(0))
                return;

            // Move files up in the list now
            for (int i = 0; i < archiveFileList.SelectedIndices.Count; i++)
            {
                int item = archiveFileList.SelectedIndices[i];

                string file        = fileList[item];
                string sourceFile  = sourceFilenames[item];
                string archiveFile = archiveFilenames[item];
                fileList.RemoveAt(item);
                sourceFilenames.RemoveAt(item);
                archiveFilenames.RemoveAt(item);
                fileList.Insert(item - 1, file);
                sourceFilenames.Insert(item - 1, sourceFile);
                archiveFilenames.Insert(item - 1, archiveFile);
            }

            // Populate the list
            populateList();
        }

        // Move items down in the list
        private void moveDown(object sender, EventArgs e)
        {
            // Make sure we selected stuff
            if (archiveFileList.SelectedIndices.Count == 0)
                return;

            // Make sure we didn't select the last one
            if (archiveFileList.SelectedIndices.Contains(archiveFileList.Items.Count - 1))
                return;

            // Move files down in the list now
            for (int i = 0; i < archiveFileList.SelectedIndices.Count; i++)
            {
                int item = archiveFileList.SelectedIndices[i];

                string file        = fileList[item];
                string sourceFile  = sourceFilenames[item];
                string archiveFile = archiveFilenames[item];
                fileList.RemoveAt(item);
                sourceFilenames.RemoveAt(item);
                archiveFilenames.RemoveAt(item);
                fileList.Insert(item + 1, file);
                sourceFilenames.Insert(item + 1, sourceFile);
                archiveFilenames.Insert(item + 1, archiveFile);
            }

            // Populate the list
            populateList();
        }

        // Check block size value
        private void CheckBlockSizeText(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // Populate the list
        private void populateList()
        {
            // Clear the file list first
            archiveFileList.Items.Clear();

            // Populate it now!
            for (int i = 0; i < fileList.Count; i++)
            {
                archiveFileList.Items.Add(new ListViewItem(new string[] {
                    (i + 1).ToString("#,0"),
                    sourceFilenames[i],
                    archiveFilenames[i],
                }));
            }
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
    }
}