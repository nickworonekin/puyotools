using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.Core.Compression;

using Ookii.Dialogs.WinForms;
using System.Linq;
using PuyoTools.App.Tools;
using PuyoTools.App.Formats.Archives;
using System.Threading.Tasks;
using PuyoTools.App.Formats.Compression;

namespace PuyoTools.GUI
{
    public partial class ArchiveCreator : ToolForm
    {
        Dictionary<IArchiveFormat, ModuleSettingsControl> writerSettingsControlsCache;

        public ArchiveCreator()
        {
            InitializeComponent();

            // Remove event handlers from the base class
            addFilesButton.Click -= base.addFilesButton_Click;
            addDirectoryButton.Click -= base.addDirectoryButton_Click;

            // Enable the "Add from entries.txt" button
            addFromEntriesButton.Visible = true;
            addFromEntriesButton.Click += addFromEntriesButton_Click;

            // Make the list view rows bigger
            listView.SmallImageList = new ImageList
            {
                ImageSize = new Size(1, 20),
            };

            // Resize the column widths
            listView_ClientSizeChanged(null, null);

            // Set up the writer settings controls cache
            writerSettingsControlsCache = new Dictionary<IArchiveFormat, ModuleSettingsControl>();

            // Fill the archive format box
            archiveFormatBox.SelectedIndex = 0;
            archiveFormatBox.Items.AddRange(ArchiveFactory.WriterFormats.ToArray());
            archiveFormatBox.DisplayMember = nameof(IArchiveFormat.Name);

            // Fill the compression format box
            compressionFormatBox.SelectedIndex = 0;
            compressionFormatBox.Items.AddRange(CompressionFactory.EncoderFormats.ToArray());
            compressionFormatBox.DisplayMember = nameof(ICompressionFormat.Name);
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

        private void AddFiles(IEnumerable<string> files, string rootPath)
        {
            var rootPathUri = new Uri(rootPath + Path.DirectorySeparatorChar);

            foreach (string file in files)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.SourceFile = file;
                fileEntry.Filename = Path.GetFileName(file);
                fileEntry.FilenameInArchive = rootPathUri.MakeRelativeUri(new Uri(file)).ToString();

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

        private void Run(Settings settings, ProgressDialog dialog)
        {
            // Setup some stuff for the progress dialog
            int entryIndex = 0;
            string description = string.Format("Processing {0}", Path.GetFileName(settings.OutFilename));

            dialog.ReportProgress(0, description);

            // For some archives, the file needs to be a specific format. As such,
            // they may be rejected when trying to add them. We'll store such files in
            // this list to let the user know they could not be added.
            List<string> rejectedFiles = new List<string>();

            // Create the stream we are going to write the archive to
            Stream destination;
            if (settings.CompressionFormat == null)
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
            using (LegacyArchiveWriter archive = settings.ArchiveFormat.GetCodec().Create(destination))
            {
                // Set archive settings
                ModuleSettingsControl settingsControl = settings.WriterSettingsControl;
                if (settingsControl != null)
                {
                    Action moduleSettingsAction = () => settingsControl.SetModuleSettings(archive);
                    settingsControl.Invoke(moduleSettingsAction);
                }

                // Set up event handlers
                archive.EntryWriting += (sender, e) =>
                {
                    if (archive.Entries.Count == 1)
                    {
                        dialog.Description = description + "\n\n" + string.Format("Adding {0}", Path.GetFileName(e.Entry.Path));
                    }
                    else
                    {
                        dialog.Description = description + "\n\n" + string.Format("Adding {0} ({1:N0} of {2:N0})", Path.GetFileName(e.Entry.Path), entryIndex + 1, archive.Entries.Count);
                    }
                };

                archive.EntryWritten += (sender, e) =>
                {
                    entryIndex++;

                    dialog.ReportProgress(entryIndex * 100 / archive.Entries.Count, description);

                    if (entryIndex == archive.Entries.Count)
                    {
                        dialog.ReportProgress(100, "Finishing up");
                    }
                };

                // Add the files to the archive. We're going to do this in a try catch since
                // sometimes an exception may be thrown (namely if the archive cannot contain
                // the file the user is trying to add)
                foreach (FileEntry entry in settings.FileEntries)
                {
                    try
                    {
                        archive.CreateEntryFromFile(entry.SourceFile, entry.FilenameInArchive);
                    }
                    catch (FileRejectedException)
                    {
                        rejectedFiles.Add(entry.SourceFile);
                    }
                }

                // If rejectedFiles is not empty, then show a message to the user
                // and ask them if they want to continue
                if (rejectedFiles.Count > 0)
                {
                    if (new RejectedFilesDialog(rejectedFiles).ShowDialog() != DialogResult.Yes)
                    {
                        destination.Close();
                        return;
                    }
                }
            }

            // Do we want to compress this archive?
            if (settings.CompressionFormat != null)
            {
                destination.Position = 0;

                using (FileStream outStream = File.Create(settings.OutFilename))
                {
                    settings.CompressionFormat.GetCodec().Compress(destination, outStream);
                }
            }

            destination.Close();
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
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.Description = "Select a directory.";
            fbd.UseDescriptionForTitle = true;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Include files within subdirectories?", "Subdirectories", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //AddFiles(Directory.GetFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories), fbd.SelectedPath);
                    AddFiles(Directory.EnumerateFiles(fbd.SelectedPath, "*", SearchOption.AllDirectories));
                }
                else
                {
                    //AddFiles(Directory.GetFiles(fbd.SelectedPath));
                    AddFiles(Directory.EnumerateFiles(fbd.SelectedPath));
                }

                EnableRunButton();
            }
        }

        private void addFromEntriesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "entries.txt|entries.txt|All Files (*.*)|*.*";
            ofd.Multiselect = true;
            ofd.Title = "Add Files";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string directory = Path.GetDirectoryName(ofd.FileName);

                AddFiles(File.ReadLines(ofd.FileName).Select(x => Path.Combine(directory, x)));

                EnableRunButton();
            }
        }

        private void archiveFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            archiveSettingsPanel.Controls.Clear();

            if (archiveFormatBox.SelectedIndex != 0)
            {
                var archiveFormat = (IArchiveFormat)archiveFormatBox.SelectedItem;
                if (!writerSettingsControlsCache.TryGetValue(archiveFormat, out var writerSettingsControl))
                {
                    writerSettingsControl = archiveFormat.GetModuleSettingsControl();
                    writerSettingsControlsCache.Add(archiveFormat, writerSettingsControl);
                }

                archiveSettingsPanel.Controls.Add(writerSettingsControl);
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
            InputDialog dialog = new InputDialog();
            dialog.WindowTitle = "Rename Files";
            dialog.MainInstruction = dialog.WindowTitle;
            dialog.Content = "Selected files will use this filename when added to the archive.";

            if (listView.SelectedItems.Count == 1)
            {
                dialog.Input = listView.SelectedItems[0].SubItems[2].Text;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    item.SubItems[2].Text = dialog.Input;
                    FileEntry fileEntry = (FileEntry)item.Tag;
                    fileEntry.FilenameInArchive = dialog.Input;
                }
            }
        }

        private async void runButton_Click(object sender, EventArgs e)
        {
            // Get the format of the archive the user wants to create
            IArchiveFormat archiveFormat = (IArchiveFormat)archiveFormatBox.SelectedItem;
            string fileExtension = archiveFormat.FileExtension != string.Empty ? archiveFormat.FileExtension : ".*";

            // Prompt the user to save the archive
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Archive";
            sfd.Filter = archiveFormat.Name + " Archive (*" + fileExtension + ")|*" + fileExtension + "|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Disable the form
                Enabled = false;

                /*var settings = new Settings
                {
                    ArchiveFormat = archiveFormat,
                    OutFilename = sfd.FileName,
                    CompressionFormat = compressionFormatBox.SelectedIndex != 0
                        ? (ICompressionFormat)compressionFormatBox.SelectedItem
                        : null,
                    WriterSettingsControl = writerSettingsControlsCache.TryGetValue(archiveFormat, out var writerSettingsControl)
                        ? writerSettingsControl
                        : null,
                    FileEntries = listView.Items
                        .Cast<ListViewItem>()
                        .Select(x => (FileEntry)x.Tag)
                        .Select(x => new FileEntry
                        {
                            Filename = x.Filename,
                            FilenameInArchive = x.FilenameInArchive,
                            SourceFile = x.SourceFile,
                        })
                        .ToList(),
                };

                // Set up the process dialog and then run the tool
                var dialog = new ProgressDialog
                {
                    WindowTitle = "Processing",
                    Title = "Creating Archive",
                };
                dialog.DoWork += (sender2, e2) => Run(settings, dialog);
                dialog.RunWorkerCompleted += (sender2, e2) => Close();
                dialog.RunWorkerAsync();*/

                var files = listView.Items
                    .Cast<ListViewItem>()
                    .Select(x => (FileEntry)x.Tag)
                    .Select(x => new ArchiveCreatorFileEntry
                    {
                        Filename = x.Filename,
                        FilenameInArchive = x.FilenameInArchive,
                        SourceFile = x.SourceFile,
                    })
                    .ToList();
                var outputPath = sfd.FileName;

                // Create options in the format the tool uses
                var toolOptions = new ArchiveCreatorOptions
                {
                    CompressionFormat = compressionFormatBox.SelectedIndex != 0
                        ? (ICompressionFormat)compressionFormatBox.SelectedItem
                        : null,
                };

                // Get the format specific options
                var formatOptions = writerSettingsControlsCache.TryGetValue(archiveFormat, out var writerSettingsControl)
                        ? (IArchiveFormatOptions)writerSettingsControl
                        : null;

                // Create the progress dialog and handler
                var progressDialog = new ProgressDialog
                {
                    WindowTitle = "Processing",
                    Title = "ncoding Textures",
                };

                var progress = new Progress<ToolProgress>(x =>
                {
                    progressDialog.ReportProgress((int)(x.Progress * 100), $"Processing {Path.GetFileName(x.File)} ({x.Progress:P0})");
                });

                progressDialog.Show();

                // Execute the tool
                var tool = new PuyoTools.App.Tools.ArchiveCreator(archiveFormat, toolOptions, formatOptions);
                await Task.Run(() => tool.Execute(files, outputPath, progress));

                // Close the dialogs
                progressDialog.Close();
                Close();
            }
        }

        private struct Settings
        {
            public IArchiveFormat ArchiveFormat;
            public ICompressionFormat CompressionFormat;
            public string OutFilename;
            public List<FileEntry> FileEntries;
            public ModuleSettingsControl WriterSettingsControl;
        }
    }
}
