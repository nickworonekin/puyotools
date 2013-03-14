using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using PuyoTools.Archive;
using PuyoTools.Compression;
using PuyoTools.Texture;

namespace PuyoTools.GUI
{
    public partial class ArchiveExtractor : ToolForm
    {
        public ArchiveExtractor()
        {
            InitializeComponent();
        }

        private void Run(Settings settings)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                string file = fileList[i];

                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    ArchiveFormat format;
                    string outPath, outName;

                    using (FileStream inStream = File.OpenRead(file))
                    {
                        // Set source to inStream
                        // The reason we do it like this is because source does not always equal inStream.
                        // You'll see why very soon.
                        Stream source = inStream;

                        // Get the format of the archive
                        format = PTArchive.GetFormat(source, (int)source.Length, Path.GetFileName(file));
                        if (format == ArchiveFormat.Unknown)
                        {
                            // Maybe it's compressed? Let's check.
                            // But first, we need to make sure we want to check
                            if (settings.DecompressSourceArchive)
                            {
                                // Get the compression format, if it is compressed that is.
                                CompressionFormat compressionFormat = PTCompression.GetFormat(source, (int)source.Length, Path.GetFileName(file));
                                if (compressionFormat != CompressionFormat.Unknown)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then check the format again
                                    source = new MemoryStream();
                                    PTCompression.Decompress(inStream, source, (int)inStream.Length, compressionFormat);

                                    source.Position = 0;
                                    format = PTArchive.GetFormat(source, (int)source.Length, Path.GetFileName(file));
                                }
                            }

                            // If we still don't know what the archive format is, just skip the file.
                            if (format == ArchiveFormat.Unknown)
                            {
                                continue;
                            }
                        }

                        // Now that we know its format, let's open it and start working with it.
                        ArchiveReader archive = PTArchive.Open(source, (int)source.Length, format);

                        // Get the appropiate output directory
                        if (settings.ExtractToSourceDirectory)
                        {
                            // Extract to the same directory as the source archive
                            outPath = Path.GetDirectoryName(file);
                        }
                        else if (settings.ExtractToSameNameDirectory)
                        {
                            // Extract to a directory of the same name
                            outPath = file + "." + Path.GetRandomFileName();
                        }
                        else
                        {
                            // Just the standard output path
                            outPath = Path.Combine(Path.Combine(Path.GetDirectoryName(file), "Extracted Files"), Path.GetFileNameWithoutExtension(file));
                        }

                        // Create the output directory if it does not exist
                        if (!Directory.Exists(outPath))
                        {
                            Directory.CreateDirectory(outPath);
                        }

                        // Now we can start extracting the files
                        for (int j = 0; j < archive.Files.Length; j++)
                        {
                            ArchiveEntry entry = archive.GetFile(j);

                            // Get the output name for this file
                            if (settings.FileNumberAsFilename || entry.Filename == String.Empty)
                            {
                                // Use the file number as its filename
                                outName = j.ToString("D" + archive.Files.Length.ToString().Length) + Path.GetExtension(entry.Filename);
                            }
                            else if (settings.AppendFileNumber)
                            {
                                // Append the file number to its filename
                                outName = Path.GetFileNameWithoutExtension(entry.Filename) + j.ToString("D" + archive.Files.Length.ToString().Length) + Path.GetExtension(entry.Filename);
                            }
                            else
                            {
                                // Just use the filename as defined in the archive
                                outName = entry.Filename;
                            }

                            // What we're going to do here may seem a tiny bit hackish, but it'll make my job much simplier.
                            // First, let's check to see if the file is compressed, and if we want to decompress it.
                            if (settings.DecompressExtractedFiles)
                            {
                                entry.Stream.Position = entry.Offset;

                                // Get the compression format, if it is compressed that is.
                                CompressionFormat compressionFormat = PTCompression.GetFormat(entry.Stream, entry.Length, entry.Filename);
                                if (compressionFormat != CompressionFormat.Unknown)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then edit the entry
                                    MemoryStream decompressedData = new MemoryStream();
                                    PTCompression.Decompress(entry.Stream, decompressedData, entry.Length, compressionFormat);

                                    entry.Stream = decompressedData;
                                    entry.Offset = 0;
                                    entry.Length = (int)decompressedData.Length;
                                }
                            }

                            // Check to see if this file is a texture. If so, let's convert it to a PNG and then edit the entry
                            if (settings.ConvertExtractedTextures)
                            {
                                entry.Stream.Position = entry.Offset;

                                // Get the texture format, if it is a texture that is.
                                TextureFormat textureFormat = PTTexture.GetFormat(entry.Stream, entry.Length, entry.Filename);
                                if (textureFormat != TextureFormat.Unknown)
                                {
                                    // Ok, it appears to be a texture. Let's convert it, and then edit the entry
                                    Bitmap textureBitmap;
                                    MemoryStream textureData = new MemoryStream();
                                    PTTexture.Read(entry.Stream, out textureBitmap, entry.Length, entry.Filename);
                                    textureBitmap.Save(textureData, ImageFormat.Png);

                                    entry.Stream = textureData;
                                    entry.Offset = 0;
                                    entry.Length = (int)textureData.Length;

                                    outName = Path.GetFileNameWithoutExtension(outName) + ".png";
                                }
                            }

                            // Time to write out the file
                            entry.Stream.Position = entry.Offset;
                            using (FileStream destination = File.Create(Path.Combine(outPath, outName)))
                            {
                                PTStream.CopyPartTo(entry.Stream, destination, entry.Length);
                            }

                            // Let's check to see if this is an archive. If it is an archive, add it to the file list.
                            entry.Stream.Position = entry.Offset;
                            if (settings.ExtractExtractedArchives)
                            {
                                ArchiveFormat archiveFormat = PTArchive.GetFormat(entry.Stream, entry.Length, entry.Filename);
                                if (archiveFormat != ArchiveFormat.Unknown)
                                {
                                    // It appears to be an archive. Let's add it to the file list
                                    if (settings.ExtractToSameNameDirectory)
                                    {
                                        // If we're adding to a directory of the same name, the outPath will be different.
                                        // We should remember that.
                                        fileList.Add(Path.Combine(file, outName));
                                    }
                                    else
                                    {
                                        fileList.Add(Path.Combine(outPath, outName));
                                    }
                                }
                            }
                        }
                    }

                    // Delete the source archive if the user chose to
                    if (settings.DeleteSourceArchive)
                    {
                        File.Delete(file);
                    }

                    // If we're extracting to a directory of the same name, we can now rename the directory
                    if (settings.ExtractToSameNameDirectory)
                    {
                        Directory.Move(outPath, file);
                    }
                }
                catch
                {
                    // Meh, just ignore the error.
                }
            }

            // The tool is finished doing what it needs to do. We can close it now.
            this.Close();
        }

        private struct Settings
        {
            public bool DecompressSourceArchive;
            public bool ExtractToSourceDirectory;
            public bool ExtractToSameNameDirectory;
            public bool DeleteSourceArchive;

            public bool DecompressExtractedFiles;
            public bool FileNumberAsFilename;
            public bool AppendFileNumber;
            public bool ExtractExtractedArchives;
            public bool ConvertExtractedTextures;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            this.Enabled = false;

            // Set up the settings we will be using for this
            Settings settings = new Settings();
            settings.DecompressSourceArchive = decompressSourceArchiveCheckbox.Checked;
            settings.ExtractToSourceDirectory = extractToSourceDirCheckbox.Checked;
            settings.ExtractToSameNameDirectory = extractToSameNameDirCheckbox.Checked && !settings.ExtractToSourceDirectory;
            settings.DeleteSourceArchive = deleteSourceArchiveCheckbox.Checked || settings.ExtractToSameNameDirectory;

            settings.DecompressExtractedFiles = decompressExtractedFilesCheckbox.Checked;
            settings.FileNumberAsFilename = fileNumberAsFilenameCheckbox.Checked;
            settings.AppendFileNumber = appendFileNumberCheckbox.Checked && !settings.FileNumberAsFilename;
            settings.ExtractExtractedArchives = extractExtractedArchivesCheckbox.Checked;
            settings.ConvertExtractedTextures = convertExtractedTexturesCheckbox.Checked;

            // Run the tool
            Run(settings);
        }
    }
}
