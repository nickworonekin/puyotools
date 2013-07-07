using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PuyoTools.Modules;
using PuyoTools.Modules.Archive;
using PuyoTools.Modules.Texture;

using Ookii.Dialogs;

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
            openedArchives = new Stack<ArchiveInfo>();
            openedArchiveNames = new List<string>();
        }

        private void OpenArchive(Stream data, int length, string fname, ArchiveFormat format)
        {
            // Let's open the archive and add it to the stack
            ArchiveReader archive = Archive.Open(data, length, format);

            ArchiveInfo info = new ArchiveInfo();
            info.Format = format;
            info.Archive = archive;

            openedArchives.Push(info);
            openedArchiveNames.Add((fname == String.Empty ? "Unnamed" : fname));

            Populate(info);
        }

        private void OpenArchive(Stream data, string fname, ArchiveFormat format)
        {
            // Let's open the archive and add it to the stack
            ArchiveReader archive = Archive.Open(data, format);

            ArchiveInfo info = new ArchiveInfo();
            info.Format = format;
            info.Archive = archive;

            openedArchives.Push(info);
            openedArchiveNames.Add((fname == String.Empty ? "Unnamed" : fname));

            Populate(info);
        }

        private void OpenTexture(Stream data, int length, string fname, TextureFormat format)
        {
            TextureViewer viewer = new TextureViewer();

            long oldPosition = data.Position;

            try
            {
                viewer.OpenTexture(data, length, fname, format);
                viewer.Show();
            }
            catch (TextureNeedsPaletteException)
            {
                ArchiveInfo info = openedArchives.Peek();

                // Seems like we need a palette for this texture. Let's try to find one.
                string textureName = Path.GetFileNameWithoutExtension(fname) + Texture.Formats[format].PaletteFileExtension;
                int paletteFileIndex = -1;

                for (int i = 0; i < info.Archive.Entries.Count; i++)
                {
                    if (info.Archive.Entries[i].Name.ToLower() == textureName.ToLower())
                    {
                        paletteFileIndex = i;
                        break;
                    }
                }

                // Let's see if we found the palette file. And if so, open it up.
                // Due to the nature of how this works, we need to copy the palette data to another stream first
                if (paletteFileIndex != -1)
                {
                    //ArchiveEntry paletteEntry = info.Archive.GetFile(paletteFileIndex);
                    Stream entryData = info.Archive.OpenEntry(paletteFileIndex);

                    //TextureReaderSettings textureSettings = new TextureReaderSettings();
                    //Stream paletteData;
                    //int paletteLength;
                    int paletteLength = (int)entryData.Length;

                    // Get the palette data (we may need to copy over the data to another stream)
                    Stream paletteData = new MemoryStream();
                    PTStream.CopyTo(entryData, paletteData);
                    paletteData.Position = 0;
                    
                    /*if (data == paletteEntry.Stream)
                    {
                        paletteEntry.Stream.Position = paletteEntry.Offset;

                        //textureSettings.PaletteStream = new MemoryStream();
                        //PTStream.CopyPartTo(paletteEntry.Stream, textureSettings.PaletteStream, paletteEntry.Length);

                        //textureSettings.PaletteStream.Position = 0;

                        paletteData = new MemoryStream();
                        PTStream.CopyPartTo(paletteEntry.Stream, paletteData, paletteEntry.Length);

                        paletteData.Position = 0;
                    }
                    else
                    {
                        paletteData = paletteEntry.Stream;
                        paletteData.Position = paletteEntry.Offset;
                    }*/

                    //textureSettings.PaletteLength = paletteEntry.Length;
                    //paletteLength = paletteEntry.Length;

                    // Now open the texture
                    data.Position = oldPosition;
                    //viewer.OpenTexture(data, paletteData, length, paletteEntry.Length, fname, format);
                    //viewer.OpenTexture(data, length, fname, textureSettings, format);
                    viewer.OpenTexture(data, length, fname, paletteData, paletteLength, format);
                    viewer.Show();
                }
            }
        }

        private void Populate(ArchiveInfo info)
        {
            listView.Items.Clear();

            // Add a blank row if this is not the top archive
            if (openedArchives.Count > 1) // Remember, we just added an entry
            {
                listView.Items.Add(new ListViewItem(new string[] {
                    "..",
                    "Parent Archive",
                }));
                listView.Items[0].Font = new Font(listView.Items[0].Font, FontStyle.Bold);
            }

            // Populate the list of entries
            /*for (int i = 0; i < info.Archive.Files.Length; i++)
            {
                listView.Items.Add(new ListViewItem(new string[] {
                    (i + 1).ToString(),
                    info.Archive.Files[i].Filename,
                    FormatFileLength(info.Archive.Files[i].Length),
                    info.Archive.Files[i].Length.ToString("#,##0"),
                }));
            }*/

            for (int i = 0; i < info.Archive.Entries.Count; i++)
            {
                ArchiveEntry entry = info.Archive.Entries[i];

                listView.Items.Add(new ListViewItem(new string[] {
                    (i + 1).ToString(),
                    entry.Name,
                    FormatFileLength(entry.Length),
                    entry.Length.ToString("#,##0"),
                }));
            }

            // Display information about the archive
            //numFilesLabel.Text = info.Archive.Files.Length.ToString();
            numFilesLabel.Text = info.Archive.Entries.Count.ToString();
            archiveFormatLabel.Text = Archive.Formats[info.Format].Name;

            archiveNameLabel.Text = openedArchiveNames[0];
            for (int i = 1; i < openedArchiveNames.Count; i++)
                archiveNameLabel.Text += " / " + openedArchiveNames[i];
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
                if (archiveStream != null)
                {
                    archiveStream.Close();
                }

                archiveStream = File.OpenRead(ofd.FileName);

                // Let's determine first if it is an archive
                ArchiveFormat archiveFormat;

                archiveFormat = Archive.GetFormat(archiveStream, (int)archiveStream.Length, ofd.SafeFileName);
                if (archiveFormat != ArchiveFormat.Unknown)
                {
                    // This is an archive. Let's open it.
                    openedArchives.Clear();
                    openedArchiveNames.Clear();

                    OpenArchive(archiveStream, (int)archiveStream.Length, ofd.SafeFileName, archiveFormat);

                    archiveInfoPanel.Visible = true;
                    return;
                }

                // It's not an archive. Maybe it's compressed?
                CompressionFormat compressionFormat = Compression.GetFormat(archiveStream, (int)archiveStream.Length, ofd.SafeFileName);
                if (compressionFormat != CompressionFormat.Unknown)
                {
                    // The file is compressed! Let's decompress it and then try to determine if it is an archive
                    MemoryStream decompressedData = new MemoryStream();
                    Compression.Decompress(archiveStream, decompressedData, (int)archiveStream.Length, compressionFormat);
                    decompressedData.Position = 0;

                    // Now with this decompressed data, let's determine if it is an archive
                    archiveFormat = Archive.GetFormat(decompressedData, (int)decompressedData.Length, ofd.SafeFileName);
                    if (archiveFormat != ArchiveFormat.Unknown)
                    {
                        // This is an archive. Let's open it.
                        openedArchives.Clear();
                        openedArchiveNames.Clear();

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

            /*ArchiveEntry entry = openedArchives.Peek().Archive.GetFile(index);
            entry.Stream.Position = entry.Offset;

            // Let's determine first if it is an archive or a texture
            ArchiveFormat archiveFormat;
            TextureFormat textureFormat;

            archiveFormat = Archive.GetFormat(entry.Stream, entry.Length, entry.Filename);
            if (archiveFormat != ArchiveFormat.Unknown)
            {
                // This is an archive. Let's open it.
                //OpenArchive(entry.Stream, entry.Length, entry.Filename, archiveFormat);
                StreamView entryStream = new StreamView(entry.Stream, entry.Length);
                OpenArchive(entryStream, entry.Filename, archiveFormat);

                return;
            }

            textureFormat = Texture.GetFormat(entry.Stream, entry.Length, entry.Filename);
            if (textureFormat != TextureFormat.Unknown)
            {
                // This is a texture. Let's attempt to open it up in the texture viewer
                OpenTexture(entry.Stream, entry.Length, entry.Filename, textureFormat);

                return;
            }

            // It's not an archive or a texture. Maybe it's compressed?
            CompressionFormat compressionFormat = Compression.GetFormat(entry.Stream, entry.Length, entry.Filename);
            if (compressionFormat != CompressionFormat.Unknown)
            {
                // The file is compressed! Let's decompress it and then try to determine if it is an archive or a texture
                MemoryStream decompressedData = new MemoryStream();
                Compression.Decompress(entry.Stream, decompressedData, entry.Length, compressionFormat);
                decompressedData.Position = 0;

                // Now with this decompressed data, let's determine if it is an archive or a texture
                archiveFormat = Archive.GetFormat(decompressedData, (int)decompressedData.Length, entry.Filename);
                if (archiveFormat != ArchiveFormat.Unknown)
                {
                    // This is an archive. Let's open it.
                    OpenArchive(decompressedData, (int)decompressedData.Length, entry.Filename, archiveFormat);

                    return;
                }

                textureFormat = Texture.GetFormat(decompressedData, (int)decompressedData.Length, entry.Filename);
                if (textureFormat != TextureFormat.Unknown)
                {
                    // This is a texture. Let's attempt to open it up in the texture viewer
                    OpenTexture(decompressedData, (int)decompressedData.Length, entry.Filename, textureFormat);

                    return;
                }
            }*/

            ArchiveEntry entry = openedArchives.Peek().Archive.Entries[index];
            Stream entryData = entry.Open();

            // Let's determine first if it is an archive or a texture
            ArchiveFormat archiveFormat;
            TextureFormat textureFormat;

            archiveFormat = Archive.GetFormat(entryData, (int)entryData.Length, entry.Name);
            if (archiveFormat != ArchiveFormat.Unknown)
            {
                // This is an archive. Let's open it.
                OpenArchive(entryData, entry.Name, archiveFormat);

                return;
            }

            textureFormat = Texture.GetFormat(entryData, (int)entryData.Length, entry.Name);
            if (textureFormat != TextureFormat.Unknown)
            {
                // This is a texture. Let's attempt to open it up in the texture viewer
                OpenTexture(entryData, (int)entryData.Length, entry.Name, textureFormat);

                return;
            }

            // It's not an archive or a texture. Maybe it's compressed?
            CompressionFormat compressionFormat = Compression.GetFormat(entryData, (int)entryData.Length, entry.Name);
            if (compressionFormat != CompressionFormat.Unknown)
            {
                // The file is compressed! Let's decompress it and then try to determine if it is an archive or a texture
                MemoryStream decompressedData = new MemoryStream();
                Compression.Decompress(entryData, decompressedData, (int)entryData.Length, compressionFormat);
                decompressedData.Position = 0;

                // Now with this decompressed data, let's determine if it is an archive or a texture
                archiveFormat = Archive.GetFormat(decompressedData, (int)decompressedData.Length, entry.Name);
                if (archiveFormat != ArchiveFormat.Unknown)
                {
                    // This is an archive. Let's open it.
                    OpenArchive(decompressedData, (int)decompressedData.Length, entry.Name, archiveFormat);

                    return;
                }

                textureFormat = Texture.GetFormat(decompressedData, (int)decompressedData.Length, entry.Name);
                if (textureFormat != TextureFormat.Unknown)
                {
                    // This is a texture. Let's attempt to open it up in the texture viewer
                    OpenTexture(decompressedData, (int)decompressedData.Length, entry.Name, textureFormat);

                    return;
                }
            }
        }

        private struct ArchiveInfo
        {
            public ArchiveFormat Format;
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

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = archive.Entries[index].Name;
                sfd.Filter = "All Files (*.*)|*.*";
                sfd.Title = "Extract File";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    /*ArchiveEntry entry = archive.GetFile(index);
                    entry.Stream.Position = entry.Offset;

                    using (FileStream outStream = File.Create(sfd.FileName))
                    {
                        PTStream.CopyPartTo(entry.Stream, outStream, entry.Length);
                    }*/

                    archive.ExtractToFile(index, sfd.FileName);
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
                    ArchiveReader archive = openedArchives.Peek().Archive;
                    for (int i = 0; i < listView.SelectedIndices.Count; i++)
                    {
                        int index = listView.SelectedIndices[i];
                        if (openedArchives.Count > 1)
                        {
                            index--;
                        }

                        string entryFilename = archive.Entries[index].Name;
                        if (entryFilename == String.Empty)
                        {
                            entryFilename = index.ToString("D" + archive.Entries.Count.ToString().Length);
                        }

                        archive.ExtractToFile(index, Path.Combine(fbd.SelectedPath, entryFilename));

                        /*ArchiveEntry entry = archive.GetFile(index);
                        entry.Stream.Position = entry.Offset;

                        string fname = entry.Filename;
                        if (fname == String.Empty)
                            fname = i.ToString("D" + archive.Files.Length.ToString().Length);

                        using (FileStream outStream = File.Create(Path.Combine(fbd.SelectedPath, fname)))
                        {
                            PTStream.CopyPartTo(entry.Stream, outStream, entry.Length);
                        }*/
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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
                ArchiveEntry entry = archive.Entries[0];

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = entry.Name;
                sfd.Filter = "All Files (*.*)|*.*";
                sfd.Title = "Extract File";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    archive.ExtractToFile(entry, sfd.FileName);
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
                        ArchiveEntry entry = archive.Entries[i];

                        string fname = entry.Name;
                        if (fname == String.Empty)
                            fname = i.ToString("D" + archive.Entries.Count.ToString().Length);

                        archive.ExtractToFile(entry, fname);
                    }
                }
            }
        }
    }
}
