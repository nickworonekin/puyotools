using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using PuyoTools.Modules;
using PuyoTools.Modules.Archive;
using PuyoTools.Modules.Texture;

namespace PuyoTools.GUI
{
    public partial class ArchiveExtractor : ToolForm
    {
        public ArchiveExtractor()
        {
            InitializeComponent();
        }

        private void Run(Settings settings, ProgressDialog dialog)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                string file = fileList[i];
                string description;

                if (fileList.Count == 1)
                {
                    description = String.Format("Processing {0}", Path.GetFileName(file));
                }
                else
                {
                    description = String.Format("Processing {0} ({1:N0} of {2:N0})", Path.GetFileName(file), i + 1, fileList.Count);
                }

                dialog.ReportProgress(i * 100 / fileList.Count, description);

                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    ArchiveFormat format;
                    string outPath, outName;
                    Queue<TextureEntry> textureFileQueue = null;
                    List<string> entryFilenames = settings.ExtractFileStructure ? new List<string>() : null;

                    using (FileStream inStream = File.OpenRead(file))
                    {
                        // Set source to inStream
                        // The reason we do it like this is because source does not always equal inStream.
                        // You'll see why very soon.
                        Stream source = inStream;

                        // Get the format of the archive
                        format = Archive.GetFormat(source, Path.GetFileName(file));
                        if (format == ArchiveFormat.Unknown)
                        {
                            // Maybe it's compressed? Let's check.
                            // But first, we need to make sure we want to check
                            if (settings.DecompressSourceArchive)
                            {
                                // Get the compression format, if it is compressed that is.
                                CompressionFormat compressionFormat = Compression.GetFormat(source, Path.GetFileName(file));
                                if (compressionFormat != CompressionFormat.Unknown)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then check the format again
                                    source = new MemoryStream();
                                    Compression.Decompress(inStream, source, compressionFormat);

                                    source.Position = 0;
                                    format = Archive.GetFormat(source, Path.GetFileName(file));
                                }
                            }

                            // If we still don't know what the archive format is, just skip the file.
                            if (format == ArchiveFormat.Unknown)
                            {
                                continue;
                            }
                        }

                        // Now that we know its format, let's open it and start working with it.
                        ArchiveReader archive = Archive.Open(source, format);

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
                        for (int j = 0; j < archive.Entries.Count; j++)
                        {
                            if (fileList.Count == 1)
                            {
                                // If there is just one file in the file list, then the progress will be
                                // based on how many files are being extracted from the archive, not
                                // how many archives we are extracting.
                                dialog.ReportProgress(j * 100 / archive.Entries.Count, description + "\n\n" + String.Format("{0:N0} of {1:N0} extracted", j + 1, archive.Entries.Count));
                            }
                            else
                            {
                                dialog.Description = description + "\n\n" + String.Format("{0:N0} of {1:N0} extracted", j + 1, archive.Entries.Count);
                            }

                            ArchiveEntry entry = archive.Entries[j];
                            Stream entryData = entry.Open();

                            // Get the output name for this file
                            if (settings.FileNumberAsFilename || entry.Name == String.Empty)
                            {
                                // Use the file number as its filename
                                outName = j.ToString("D" + archive.Entries.Count.ToString().Length) + Path.GetExtension(entry.Name);
                            }
                            else if (settings.PrependFileNumber)
                            {
                                // Prepend the file number to its filename
                                outName = j.ToString("D" + archive.Entries.Count.ToString().Length) + "_" + entry.Name;
                            }
                            else
                            {
                                // Just use the filename as defined in the archive
                                outName = entry.Name;
                            }

                            // Add the output filename to the entry filename list if we are extracting the archive's file structure.
                            if (entryFilenames != null)
                            {
                                entryFilenames.Add(outName);
                            }

                            // What we're going to do here may seem a tiny bit hackish, but it'll make my job much simplier.
                            // First, let's check to see if the file is compressed, and if we want to decompress it.
                            if (settings.DecompressExtractedFiles)
                            {
                                // Get the compression format, if it is compressed that is.
                                CompressionFormat compressionFormat = Compression.GetFormat(entryData, entry.Name);
                                if (compressionFormat != CompressionFormat.Unknown)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then edit the entry
                                    MemoryStream decompressedData = new MemoryStream();
                                    Compression.Decompress(entryData, decompressedData, compressionFormat);

                                    entryData = decompressedData;
                                    entryData.Position = 0;
                                }
                            }

                            // Check to see if this file is a texture. If so, let's convert it to a PNG and then edit the entry
                            if (settings.ConvertExtractedTextures)
                            {
                                // Get the texture format, if it is a texture that is.
                                TextureFormat textureFormat = Texture.GetFormat(entryData, entry.Name);
                                if (textureFormat != TextureFormat.Unknown)
                                {
                                    // Ok, it appears to be a texture. We're going to attempt to convert it here.
                                    // If we get a TextureNeedsPalette exception, we'll wait until after we extract
                                    // all the files in this archive before we process it.
                                    try
                                    {
                                        MemoryStream textureData = new MemoryStream();
                                        Texture.Read(entryData, textureData, textureFormat);

                                        // If no exception was thrown, then we are all good doing what we need to do
                                        entryData = textureData;
                                        entryData.Position = 0;

                                        outName = Path.GetFileNameWithoutExtension(outName) + ".png";
                                    }
                                    catch (TextureNeedsPaletteException)
                                    {
                                        // Uh oh, looks like we need a palette.
                                        // What we are going to do is add it to textureFileQueue, then convert it
                                        // after we extract all of the files.
                                        if (textureFileQueue == null)
                                        {
                                            textureFileQueue = new Queue<TextureEntry>();
                                        }

                                        TextureEntry textureEntry = new TextureEntry();
                                        textureEntry.Format = textureFormat;
                                        textureEntry.Filename = Path.Combine(outPath, outName);

                                        textureFileQueue.Enqueue(textureEntry);
                                    }
                                }
                            }

                            // Time to write out the file
                            using (FileStream destination = File.Create(Path.Combine(outPath, outName)))
                            {
                                PTStream.CopyTo(entryData, destination, (int)entryData.Length);
                            }

                            // Let's check to see if this is an archive. If it is an archive, add it to the file list.
                            entryData.Position = 0;
                            if (settings.ExtractExtractedArchives)
                            {
                                ArchiveFormat archiveFormat = Archive.GetFormat(entryData, entry.Name);
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

                                    if (fileList.Count == 2)
                                    {
                                        // If there was one archive in the file list, and now there is more,
                                        // adjust the progress bar and the description
                                        description = String.Format("Processing {0} ({1:N0} of {2:N0})", Path.GetFileName(file), i + 1, fileList.Count);
                                        dialog.ReportProgress(i * 100 / fileList.Count, description + "\n\n" + String.Format("{0:N0} of {1:N0} extracted", j + 1, archive.Entries.Count));
                                    }
                                }
                            }
                        }
                    }

                    // Let's see if we have any textures we still need to convert.
                    if (settings.ConvertExtractedTextures && textureFileQueue != null)
                    {
                        // Ok, it appears we do. So, let's loop through the queue until it is empty.
                        while (textureFileQueue.Count > 0)
                        {
                            TextureEntry textureEntry = textureFileQueue.Dequeue();

                            // Get the palette file name, and the out file name
                            string paletteName = Path.Combine(Path.GetDirectoryName(textureEntry.Filename), Path.GetFileNameWithoutExtension(textureEntry.Filename)) + Texture.Formats[textureEntry.Format].PaletteFileExtension;
                            string textureOutName = Path.Combine(Path.GetDirectoryName(textureEntry.Filename), Path.GetFileNameWithoutExtension(textureEntry.Filename)) + ".png";

                            // Make sure the two files exist before we attempt to open them.
                            // Wrap the whole thing in a try catch in case for some reason the texture file was modifed.
                            // That way, it'll fail peacefully and not screw over last minute things that need to be done to the archive.

                            // Open up the archive and test to make sure it's still a valid texture.
                            // You know, in case somehow it was edited or not extracted properly.
                            if (File.Exists(textureEntry.Filename) && File.Exists(paletteName))
                            {
                                try
                                {
                                    using (FileStream inTextureStream = File.OpenRead(textureEntry.Filename))
                                    {
                                        if (!Texture.Formats[textureEntry.Format].Is(inTextureStream, (int)inTextureStream.Length, textureEntry.Filename))
                                        {
                                            // Oh dear, somehow this isn't a texture anymore. Just skip over it
                                            continue;
                                        }

                                        // Ok, now we can load the palette data and try to convert it.
                                        using (FileStream inPaletteStream = File.OpenRead(paletteName),
                                        outTextureStream = File.Create(textureOutName))
                                        {
                                            TextureBase texture = Texture.Formats[textureEntry.Format];
                                            texture.PaletteStream = inPaletteStream;
                                            texture.PaletteLength = (int)inPaletteStream.Length;
                                            texture.Read(inTextureStream, outTextureStream, (int)inTextureStream.Length);
                                        }
                                    }

                                    // Now we can delete those two files
                                    File.Delete(textureEntry.Filename);
                                    File.Delete(paletteName);
                                }
                                catch
                                {
                                    // Something happened! But we'll just ignore it.
                                }
                            }
                        }
                    }

                    // Extract the archive's file structure if this option was selected
                    if (settings.ExtractFileStructure)
                    {
                        // TODO extract the file structure
                        File.WriteAllLines(Path.Combine(outPath, "entries.txt"), entryFilenames);
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
        }

        private struct Settings
        {
            public bool DecompressSourceArchive;
            public bool ExtractToSourceDirectory;
            public bool ExtractToSameNameDirectory;
            public bool ExtractFileStructure;
            public bool DeleteSourceArchive;

            public bool DecompressExtractedFiles;
            public bool FileNumberAsFilename;
            public bool PrependFileNumber;
            public bool ExtractExtractedArchives;
            public bool ConvertExtractedTextures;
        }

        private struct TextureEntry
        {
            public TextureFormat Format;
            public string Filename;
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
            settings.ExtractFileStructure = extractFileStructureCheckbox.Checked;
            settings.DeleteSourceArchive = deleteSourceArchiveCheckbox.Checked || settings.ExtractToSameNameDirectory;

            settings.DecompressExtractedFiles = decompressExtractedFilesCheckbox.Checked;
            settings.FileNumberAsFilename = fileNumberAsFilenameCheckbox.Checked;
            settings.PrependFileNumber = prependFileNumberCheckbox.Checked && !settings.FileNumberAsFilename;
            settings.ExtractExtractedArchives = extractExtractedArchivesCheckbox.Checked;
            settings.ConvertExtractedTextures = convertExtractedTexturesCheckbox.Checked;

            // Set up the process dialog and then run the tool
            ProgressDialog dialog = new ProgressDialog();
            dialog.WindowTitle = "Processing";
            dialog.Title = "Extracting Archives";
            dialog.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                Run(settings, dialog);
            };
            dialog.RunWorkerCompleted += delegate(object sender2, RunWorkerCompletedEventArgs e2)
            {
                // The tool is finished doing what it needs to do. We can close it now.
                this.Close();
            };
            dialog.RunWorkerAsync();
        }
    }
}
