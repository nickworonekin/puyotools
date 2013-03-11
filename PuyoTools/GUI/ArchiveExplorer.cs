using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PuyoTools.GUI
{
    public partial class ArchiveExplorer : Form
    {
        private Stack<ArchiveInfo> OpenedArchives;
        private List<string> OpenedArchiveNames;

        public ArchiveExplorer()
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;
            this.MinimumSize = this.Size;

            // Make the list view rows bigger
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(1, 20);
            listView.SmallImageList = imageList;

            // Hide the archive information until an archive is opened
            archiveInfoPanel.Visible = false;

            // Resize the column widths
            ArchiveExplorer_ClientSizeChanged(null, null);

            // Create the OpenedArchives stack and the name list
            OpenedArchives = new Stack<ArchiveInfo>();
            OpenedArchiveNames = new List<string>();
        }

        private void OpenArchive(Stream data, int length, string fname, PuyoTools2.Archive.ArchiveFormat format)
        {
            // Let's open the archive and add it to the stack
            PuyoTools2.Archive.ArchiveReader archive = PuyoTools2.Archive.Archive.Open(data, length, format);

            ArchiveInfo info = new ArchiveInfo();
            info.Format = format;
            info.Archive = archive;

            OpenedArchives.Push(info);
            OpenedArchiveNames.Add((fname == String.Empty ? "Unnamed" : fname));

            Populate(info);
        }

        private void OpenTexture(Stream data, int length, string fname, PuyoTools2.Texture.TextureFormat format)
        {
            TextureViewer viewer = new TextureViewer();
            viewer.OpenTexture(data, length, fname, format);
            viewer.Show();
        }

        private void Populate(ArchiveInfo info)
        {
            listView.Items.Clear();

            // Add a blank row if this is not the top archive
            if (OpenedArchives.Count > 1) // Remember, we just added an entry
            {
                listView.Items.Add(new ListViewItem(new string[] {
                    "..",
                    "Parent Archive",
                }));
                listView.Items[0].Font = new Font(listView.Items[0].Font, FontStyle.Bold);
            }

            // Populate the list of entries
            for (int i = 0; i < info.Archive.Files.Length; i++)
            {
                listView.Items.Add(new ListViewItem(new string[] {
                    (i + 1).ToString(),
                    info.Archive.Files[i].Filename,
                    FormatFileLength(info.Archive.Files[i].Length),
                    info.Archive.Files[i].Length.ToString("#,##0"),
                }));
            }

            // Display information about the archive
            numFilesLabel.Text = info.Archive.Files.Length.ToString();
            archiveFormatLabel.Text = PuyoTools2.Archive.Archive.Formats[info.Format].Name;

            archiveNameLabel.Text = OpenedArchiveNames[0];
            for (int i = 1; i < OpenedArchiveNames.Count; i++)
                archiveNameLabel.Text += " / " + OpenedArchiveNames[i];
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
            if (bytes > terabyte) return Decimal.Divide(bytes, terabyte).ToString("#,##0.00") + " TB";
            else if (bytes > gigabyte) return Decimal.Divide(bytes, gigabyte).ToString("#,##0.00") + " GB";
            else if (bytes > megabyte) return Decimal.Divide(bytes, megabyte).ToString("#,##0.00") + " MB";
            else if (bytes > kilobyte) return Decimal.Divide(bytes, kilobyte).ToString("#,##0.00") + " KB";

            return bytes.ToString("#,0") + " B";
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
                FileStream data = File.OpenRead(ofd.FileName);

                // Let's determine first if it is an archive
                PuyoTools2.Archive.ArchiveFormat archiveFormat;

                archiveFormat = PuyoTools2.Archive.Archive.GetFormat(data, (int)data.Length, ofd.SafeFileName);
                if (archiveFormat != PuyoTools2.Archive.ArchiveFormat.Unknown)
                {
                    // This is an archive. Let's open it.
                    OpenedArchives.Clear();
                    OpenedArchiveNames.Clear();

                    OpenArchive(data, (int)data.Length, ofd.SafeFileName, archiveFormat);

                    archiveInfoPanel.Visible = true;
                    return;
                }

                // It's not an archive. Maybe it's compressed?
                PuyoTools2.Compression.CompressionFormat compressionFormat = PuyoTools2.Compression.Compression.GetFormat(data, (int)data.Length, ofd.SafeFileName);
                if (compressionFormat != PuyoTools2.Compression.CompressionFormat.Unknown)
                {
                    // The file is compressed! Let's decompress it and then try to determine if it is an archive
                    MemoryStream decompressedData = new MemoryStream();
                    PuyoTools2.Compression.Compression.Decompress(data, decompressedData, (int)data.Length, compressionFormat);
                    decompressedData.Position = 0;

                    // Now with this decompressed data, let's determine if it is an archive
                    archiveFormat = PuyoTools2.Archive.Archive.GetFormat(decompressedData, (int)decompressedData.Length, ofd.SafeFileName);
                    if (archiveFormat != PuyoTools2.Archive.ArchiveFormat.Unknown)
                    {
                        // This is an archive. Let's open it.
                        OpenedArchives.Clear();
                        OpenedArchiveNames.Clear();

                        OpenArchive(decompressedData, (int)decompressedData.Length, ofd.SafeFileName, archiveFormat);

                        archiveInfoPanel.Visible = true;
                        return;
                    }
                }
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            int index = listView.SelectedIndices[0];

            if (OpenedArchives.Count > 1)
            {
                // Do we want to go to the parent archive?
                if (index == 0)
                {
                    OpenedArchives.Pop();
                    OpenedArchiveNames.RemoveAt(OpenedArchiveNames.Count - 1);

                    Populate(OpenedArchives.Peek());

                    return;
                }
                else
                {
                    // Subtract the index by 1 so we're referring to the correct files
                    index--;
                }
            }

            PuyoTools2.Archive.ArchiveEntry entry = OpenedArchives.Peek().Archive.GetFile(index);
            entry.Stream.Position = entry.Offset;

            // Let's determine first if it is an archive or a texture
            PuyoTools2.Archive.ArchiveFormat archiveFormat;
            PuyoTools2.Texture.TextureFormat textureFormat;

            archiveFormat = PuyoTools2.Archive.Archive.GetFormat(entry.Stream, entry.Length, entry.Filename);
            if (archiveFormat != PuyoTools2.Archive.ArchiveFormat.Unknown)
            {
                // This is an archive. Let's open it.
                OpenArchive(entry.Stream, entry.Length, entry.Filename, archiveFormat);

                return;
            }

            textureFormat = PuyoTools2.Texture.Texture.GetFormat(entry.Stream, entry.Length, entry.Filename);
            if (textureFormat != PuyoTools2.Texture.TextureFormat.Unknown)
            {
                // This is a texture. Let's attempt to open it up in the texture viewer
                OpenTexture(entry.Stream, entry.Length, entry.Filename, textureFormat);

                return;
            }

            // It's not an archive or a texture. Maybe it's compressed?
            PuyoTools2.Compression.CompressionFormat compressionFormat = PuyoTools2.Compression.Compression.GetFormat(entry.Stream, entry.Length, entry.Filename);
            if (compressionFormat != PuyoTools2.Compression.CompressionFormat.Unknown)
            {
                // The file is compressed! Let's decompress it and then try to determine if it is an archive or a texture
                MemoryStream decompressedData = new MemoryStream();
                PuyoTools2.Compression.Compression.Decompress(entry.Stream, decompressedData, entry.Length, compressionFormat);

                // Now with this decompressed data, let's determine if it is an archive or a texture
                archiveFormat = PuyoTools2.Archive.Archive.GetFormat(decompressedData, (int)decompressedData.Length, entry.Filename);
                if (archiveFormat != PuyoTools2.Archive.ArchiveFormat.Unknown)
                {
                    // This is an archive. Let's open it.
                    OpenArchive(decompressedData, (int)decompressedData.Length, entry.Filename, archiveFormat);

                    return;
                }

                textureFormat = PuyoTools2.Texture.Texture.GetFormat(decompressedData, (int)decompressedData.Length, entry.Filename);
                if (textureFormat != PuyoTools2.Texture.TextureFormat.Unknown)
                {
                    // This is a texture. Let's attempt to open it up in the texture viewer
                    OpenTexture(decompressedData, (int)decompressedData.Length, entry.Filename, textureFormat);

                    return;
                }
            }
        }

        private struct ArchiveInfo
        {
            public PuyoTools2.Archive.ArchiveFormat Format;
            public PuyoTools2.Archive.ArchiveReader Archive;
        }

        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Nothing selected
            if (listView.SelectedIndices.Count == 0)
                return;

            // One entry select
            else if (listView.SelectedIndices.Count == 1)
            {
                PuyoTools2.Archive.ArchiveReader archive = OpenedArchives.Peek().Archive;
                int index = listView.SelectedIndices[0];

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = archive.Files[index].Filename;
                sfd.Filter = "All Files (*.*)|*.*";
                sfd.Title = "Extract File";

                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    PuyoTools2.Archive.ArchiveEntry entry = archive.GetFile(index);
                    entry.Stream.Position = entry.Offset;

                    using (FileStream outStream = File.Create(sfd.FileName))
                    {
                        PuyoTools2.PTStream.CopyPartTo(entry.Stream, outStream, entry.Length);
                    }
                }
            }

            // Multiple files selected
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Select a folder to extract the files to.";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
