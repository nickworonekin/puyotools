using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.Core.Textures;

using Ookii.Dialogs.WinForms;
using PuyoTools.App.Formats.Textures;
using PuyoTools.App;
using PuyoTools.App.Formats.Archives;
using PuyoTools.App.Formats.Compression;
using PuyoTools.Archives;

namespace PuyoTools.GUI
{
    public partial class ArchiveExplorer : Form
    {
        private Stream archiveStream;

        private Stack<ArchiveInfo> openedArchives;
        private List<string> openedArchiveNames;

        public ArchiveExplorer()
        {
            InitializeComponent();

            Icon = IconResources.ProgramIcon;
            MinimumSize = Size;

            // Make the list view rows bigger
            listView.SmallImageList = new ImageList
            {
                ImageSize = new Size(1, 20),
            };

            // Hide the archive information until an archive is opened
            archiveInfoPanel.Visible = false;

            // Resize the column widths
            ArchiveExplorer_ClientSizeChanged(null, null);

            // Create the OpenedArchives stack and the name list
            openedArchives = new Stack<ArchiveInfo>();
            openedArchiveNames = new List<string>();
        }

        private void OpenArchive(Stream data, string filename, IArchiveFormat format)
        {
            // Let's open the archive and add it to the stack
            ArchiveReader archive = format.CreateReader(data);

            ArchiveInfo info = new ArchiveInfo();
            info.Format = format;
            info.Archive = archive;

            openedArchives.Push(info);
            openedArchiveNames.Add(filename == string.Empty ? "Unnamed" : filename);

            Populate(info);
        }

        private void OpenTexture(Stream data, string filename, ITextureFormat format)
        {
            TextureViewer viewer = new TextureViewer();

            long oldPosition = data.Position;

            void OnExternalPaletteRequired(object sender, ExternalPaletteRequiredEventArgs e)
            {
                ArchiveInfo info = openedArchives.Peek();

                // Seems like we need a palette for this texture. Let's try to find one.
                string textureName = Path.ChangeExtension(filename, format.PaletteFileExtension);
                ArchiveReaderEntry paletteEntry = info.Archive.Entries.FirstOrDefault(x => x.FullName.Equals(textureName, StringComparison.OrdinalIgnoreCase));

                // Let's see if we found the palette file. And if so, open it up.
                // Due to the nature of how this works, we need to copy the palette data to another stream first
                if (paletteEntry != null)
                {
                    Stream entryData = paletteEntry.Open();

                    // Get the palette data (we may need to copy over the data to another stream)
                    Stream paletteData = new MemoryStream();
                    entryData.CopyTo(paletteData);
                    paletteData.Position = 0;
                    e.Palette = paletteData;
                }
            }

            try
            {
                viewer.OpenTexture(data, filename, format,
                    OnExternalPaletteRequired);
                viewer.Show();
            }
            catch
            {
                // Do nothing.
            }
            /*catch (TextureNeedsPaletteException)
            {
                ArchiveInfo info = openedArchives.Peek();

                // Seems like we need a palette for this texture. Let's try to find one.
                string textureName = Path.ChangeExtension(filename, format.PaletteFileExtension);
                ArchiveEntry paletteEntry = info.Archive.Entries.FirstOrDefault(x => x.FullName.Equals(textureName, StringComparison.OrdinalIgnoreCase));

                // Let's see if we found the palette file. And if so, open it up.
                // Due to the nature of how this works, we need to copy the palette data to another stream first
                if (paletteEntry != null)
                {
                    Stream entryData = paletteEntry.Open();

                    // Get the palette data (we may need to copy over the data to another stream)
                    Stream paletteData = new MemoryStream();
                    entryData.CopyTo(paletteData);
                    paletteData.Position = 0;

                    // Now open the texture
                    data.Position = oldPosition;
                    viewer.OpenTexture(data, filename, paletteData, format);
                    viewer.Show();
                }
            }*/
        }

        private void Populate(ArchiveInfo info)
        {
            listView.Items.Clear();

            // Add a blank row if this is not the top archive
            if (openedArchives.Count > 1) // Remember, we just added an entry
            {
                var listViewItem = new ListViewItem(new string[] {
                    "..",
                    "Parent Archive",
                });
                listViewItem.Font = new Font(listViewItem.Font, FontStyle.Bold);

                listView.Items.Add(listViewItem);
            }

            for (int i = 0; i < info.Archive.Entries.Count; i++)
            {
                ArchiveReaderEntry entry = info.Archive.Entries[i];

                var listViewItem = new ListViewItem(new string[] {
                    (i + 1).ToString(),
                    entry.FullName,
                    FormatFileLength(entry.Length),
                    entry.Length.ToString("N0"),
                });

                if (entry.FullName == string.Empty)
                {
                    listViewItem.UseItemStyleForSubItems = false;

                    var listViewSubItem = listViewItem.SubItems[1];
                    listViewSubItem.Text = "Unnamed File";
                    listViewSubItem.Font = new Font(listViewSubItem.Font, FontStyle.Italic);
                }

                listView.Items.Add(listViewItem);
            }

            // Display information about the archive
            numFilesLabel.Text = info.Archive.Entries.Count.ToString();
            archiveFormatLabel.Text = info.Format.Name;
            archiveNameLabel.Text = string.Join(" / ", openedArchiveNames);
        }

        private string FormatFileLength(long bytes)
        {
            // Set byte values
            long
                kilobyte = 1024,
                megabyte = 1024 * kilobyte,
                gigabyte = 1024 * megabyte,
                terabyte = 1024 * gigabyte;

            // Ok, let's format our filesize now
            if (bytes > terabyte) return decimal.Divide(bytes, terabyte).ToString("N2") + " TB";
            else if (bytes > gigabyte) return decimal.Divide(bytes, gigabyte).ToString("N2") + " GB";
            else if (bytes > megabyte) return decimal.Divide(bytes, megabyte).ToString("N2") + " MB";
            else if (bytes > kilobyte) return decimal.Divide(bytes, kilobyte).ToString("N2") + " KB";

            return bytes.ToString("N0") + " B";
        }

        private void ArchiveExplorer_ClientSizeChanged(object sender, EventArgs e)
        {
            listView.Columns[1].Width = Math.Max(150, this.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 10 - listView.Columns[0].Width - listView.Columns[2].Width - listView.Columns[3].Width);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.Title = "Open Archive";

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (archiveStream != null)
                {
                    archiveStream.Close();
                }

                archiveStream = File.OpenRead(ofd.FileName);

                // Let's determine first if it is an archive
                IArchiveFormat archiveFormat;

                archiveFormat = ArchiveFactory.GetFormat(archiveStream, ofd.SafeFileName);
                if (archiveFormat != null)
                {
                    // This is an archive. Let's open it.
                    openedArchives.Clear();
                    openedArchiveNames.Clear();

                    OpenArchive(archiveStream, ofd.SafeFileName, archiveFormat);

                    archiveInfoPanel.Visible = true;
                    return;
                }

                // It's not an archive. Maybe it's compressed?
                ICompressionFormat compressionFormat = CompressionFactory.GetFormat(archiveStream, ofd.SafeFileName);
                if (compressionFormat != null)
                {
                    // The file is compressed! Let's decompress it and then try to determine if it is an archive
                    MemoryStream decompressedData = new MemoryStream();
                    compressionFormat.GetCodec().Decompress(archiveStream, decompressedData);
                    decompressedData.Position = 0;

                    // Now with this decompressed data, let's determine if it is an archive
                    archiveFormat = ArchiveFactory.GetFormat(decompressedData, ofd.SafeFileName);
                    if (archiveFormat != null)
                    {
                        // This is an archive. Let's open it.
                        openedArchives.Clear();
                        openedArchiveNames.Clear();

                        OpenArchive(decompressedData,  ofd.SafeFileName, archiveFormat);

                        archiveInfoPanel.Visible = true;
                        return;
                    }
                }

                // Display a message if we are not able to open the file the user selected
                MessageBox.Show("Unknown or unsupported archive format.");
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            int index = listView.SelectedIndices[0];

            if (openedArchives.Count > 1)
            {
                // Do we want to go to the parent archive?
                if (index == 0)
                {
                    openedArchives.Pop();
                    openedArchiveNames.RemoveAt(openedArchiveNames.Count - 1);

                    Populate(openedArchives.Peek());

                    return;
                }
                else
                {
                    // Subtract the index by 1 so we're referring to the correct files
                    index--;
                }
            }

            ArchiveReaderEntry entry = openedArchives.Peek().Archive.Entries[index];
            Stream entryData = entry.Open();

            // Let's determine first if it is an archive or a texture
            IArchiveFormat archiveFormat;
            ITextureFormat textureFormat;

            archiveFormat = ArchiveFactory.GetFormat(entryData, entry.Name);
            if (archiveFormat != null)
            {
                // This is an archive. Let's open it.
                OpenArchive(entryData, entry.Name, archiveFormat);

                return;
            }

            textureFormat = TextureFactory.GetFormat(entryData, entry.Name);
            if (textureFormat != null)
            {
                // This is a texture. Let's attempt to open it up in the texture viewer
                OpenTexture(entryData, entry.Name, textureFormat);

                return;
            }

            // It's not an archive or a texture. Maybe it's compressed?
            ICompressionFormat compressionFormat = CompressionFactory.GetFormat(entryData, entry.Name);
            if (compressionFormat != null)
            {
                // The file is compressed! Let's decompress it and then try to determine if it is an archive or a texture
                MemoryStream decompressedData = new MemoryStream();
                compressionFormat.GetCodec().Decompress(entryData, decompressedData);
                decompressedData.Position = 0;

                // Now with this decompressed data, let's determine if it is an archive or a texture
                archiveFormat = ArchiveFactory.GetFormat(decompressedData, entry.Name);
                if (archiveFormat != null)
                {
                    // This is an archive. Let's open it.
                    OpenArchive(decompressedData, entry.Name, archiveFormat);

                    return;
                }

                textureFormat = TextureFactory.GetFormat(decompressedData, entry.Name);
                if (textureFormat != null)
                {
                    // This is a texture. Let's attempt to open it up in the texture viewer
                    OpenTexture(decompressedData, entry.Name, textureFormat);

                    return;
                }
            }
        }

        private struct ArchiveInfo
        {
            public IArchiveFormat Format;
            public ArchiveReader Archive;
        }

        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Nothing selected
            if (listView.SelectedIndices.Count == 0)
                return;

            // One entry select
            else if (listView.SelectedIndices.Count == 1)
            {
                ArchiveReader archive = openedArchives.Peek().Archive;
                int index = listView.SelectedIndices[0];
                if (openedArchives.Count > 1)
                {
                    index--;
                }

                ArchiveReaderEntry entry = archive.Entries[index];

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = entry.Name;
                sfd.Filter = "All Files (*.*)|*.*";
                sfd.Title = "Extract File";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    entry.ExtractToFile(sfd.FileName);
                }
            }

            // Multiple files selected
            else
            {
                VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                fbd.Description = "Select a folder to extract the files to.";
                fbd.UseDescriptionForTitle = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    //ArchiveReader archive = openedArchives.Peek().Archive;
                    ArchiveReader archive = openedArchives.Peek().Archive;
                    for (int i = 0; i < listView.SelectedIndices.Count; i++)
                    {
                        int index = listView.SelectedIndices[i];
                        if (openedArchives.Count > 1)
                        {
                            index--;
                        }

                        ArchiveReaderEntry entry = archive.Entries[index];

                        string entryFilename = entry.Name;
                        if (entryFilename == string.Empty)
                        {
                            entryFilename = index.ToString("D" + archive.Entries.Count.ToString().Length);
                        }

                        entry.ExtractToFile(Path.Combine(fbd.SelectedPath, entryFilename));
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArchiveReader archive = openedArchives.Peek().Archive;

            // No files in the archive
            if (archive.Entries.Count == 0)
                return;

            // One file in the archive
            else if (archive.Entries.Count == 1)
            {
                ArchiveReaderEntry entry = archive.Entries[0];

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = entry.Name;
                sfd.Filter = "All Files (*.*)|*.*";
                sfd.Title = "Extract File";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    entry.ExtractToFile(sfd.FileName);
                }
            }

            // Multiple files in the archive
            else
            {
                VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                fbd.Description = "Select a folder to extract the files to.";
                fbd.UseDescriptionForTitle = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < archive.Entries.Count; i++)
                    {
                        ArchiveReaderEntry entry = archive.Entries[i];

                        string entryFilename = entry.Name;
                        if (entryFilename == string.Empty)
                        {
                            entryFilename = i.ToString("D" + archive.Entries.Count.ToString().Length);
                        }

                        entry.ExtractToFile(Path.Combine(fbd.SelectedPath, entryFilename));
                    }
                }
            }
        }
    }
}
