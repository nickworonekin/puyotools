using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace PuyoTools
{
    public class ArchiveExtractor : Form
    {
        // Set up form variables
        private Panel PanelContent;

        private GroupBox
            extractionSettings      = new GroupBox(), // Extraction Settings
            decompressionSettings   = new GroupBox(), // Decompression Settings
            imageConversionSettings = new GroupBox(); // Image Conversion Settings

        private CheckBox
            extractFilenames        = new CheckBox(), // Extract Filenames
            extractSameDir          = new CheckBox(), // Extract to the same directory
            extractExtracted        = new CheckBox(), // Extract extracted archives
            extractDirSameFilename  = new CheckBox(), // Extract to directory with same filename
            deleteSourceArchive     = new CheckBox(), // Delete source archive
            decompressSourceFile    = new CheckBox(), // Decompress Source File
            decompressExtractedFile = new CheckBox(), // Decompress Extracted File
            decompressExtractedDir  = new CheckBox(), // Decompress Extracted File to different directory
            useStoredFilename       = new CheckBox(), // Use stored filename
            unpackImage             = new CheckBox(), // Unpack image
            convertSameDir          = new CheckBox(), // Output to same directory
            deleteSourceImage       = new CheckBox(); // Delete Source Image

        private Button
            startWorkButton = new Button(); // Start Work Button

        private string[]
            files; // Files to Decompress

        private StatusMessage
            status; // Status Message

        public ArchiveExtractor()
        {
            // File Selection
            files = FileSelectionDialog.OpenFiles("Select Archive",
                "Supported Archives (*.acx;*.afs;*.carc;*.gnt;*.gvm;*.mdl;*.mrg;*.mrz;*.narc;*.one;*.onz;*.pvm;*.snt;*.spk;*.tex;*.tez;*.txd;*.vdd)|*.acx;*.afs;*.carc;*.gnt;*.gvm;*.mdl;*.mrg;*.mrz;*.narc;*.one;*.onz;*.pvm;*.snt;*.spk;*.tex;*.tez;*.txd;*.vdd|" +
                "ACX Archive (*.acx)|*.acx|" +
                "AFS Archive (*.afs)|*.afs|" +
                "GNT Archive (*.gnt)|*.gnt|" +
                "GVM Archive (*.gvm)|*.gvm|" +
                "MDL Archive (*.mdl)|*.mdl|" +
                "MRG Archive (*.mrg;*.mrz)|*.mrg;*.mrz|" +
                "NARC Archive (*.narc;*.carc)|*.narc;*.carc|" +
                "ONE Archive (*.one;*.onz)|*.one;*.onz|" +
                "PVM Archive (*.pvm)|*.pvm|" +
                "SNT Archive (*.snt)|*.snt|" +
                "SPK Archive (*.spk)|*.spk|" +
                "TEX Archive (*.tex;*.tez)|*.tex;*.tez|" +
                "TXAG Archive (*.txd)|*.txd|" +
                "VDD Archive (*.vdd)|*.vdd|" +
                "All Files (*.*)|*.*");
            if (files == null || files.Length == 0)
                return;

            // Show the options
            showOptions();
        }

        public ArchiveExtractor(bool selectDirectory)
        {
            // Select the directory
            string directory = FileSelectionDialog.SaveDirectory("Select a directory");
            if (directory == null || directory == String.Empty)
                return;

            // Include files from subdirectories
            DialogResult result = MessageBox.Show(this, "Include files from subdirectories?", "Add Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);

            // Show Options
            showOptions();
        }

        private void showOptions()
        {
            // Set up the form
            FormContent.Create(this, "Archive - Extract", new Size(428, 420));
            PanelContent = new Panel() {
                Location = new Point(0, 0),
                Width    = this.ClientSize.Width,
                Height   = this.ClientSize.Height,
            };
            this.Controls.Add(PanelContent);

            // Files Selected
            FormContent.Add(PanelContent, new Label(),
                String.Format("{0} {1} Selected",
                    files.Length.ToString(),
                    (files.Length > 1 ? "Files" : "File")),
                new Point(0, 8),
                new Size(PanelContent.Width, 16),
                ContentAlignment.TopCenter,
                new Font(SystemFonts.DialogFont.FontFamily.Name, SystemFonts.DialogFont.Size, FontStyle.Bold));

            // Extraction Settings
            FormContent.Add(PanelContent, extractionSettings,
                "Extraction Settings",
                new Point(8, 32),
                new Size(PanelContent.Size.Width - 24, 144));

            // Extract filenames
            FormContent.Add(extractionSettings, extractFilenames, true,
                "Extract filenames from archive.",
                new Point(8, 20),
                new Size(extractionSettings.Size.Width - 16, 16));

            // Extract to same directory
            FormContent.Add(extractionSettings, extractSameDir,
                "Extract files to the same directory as source (and overwrite if necessary).",
                new Point(8, 40),
                new Size(extractionSettings.Size.Width - 16, 16));

            // Extract to directory with same filename
            FormContent.Add(extractionSettings, extractDirSameFilename,
                "Extract to directory with the same filename as source archive.\n(You must check the option to delete the source archive.)",
                new Point(8, 60),
                new Size(extractionSettings.Size.Width - 16, 36));

            // Extract extracted archives
            FormContent.Add(extractionSettings, extractExtracted,
                "Extract archives extracted from the source archive.",
                new Point(8, 100),
                new Size(extractionSettings.Size.Width - 16, 16));

            // Delete archive
            FormContent.Add(extractionSettings, deleteSourceArchive,
                "Delete source archive (on successful extraction).",
                new Point(8, 120),
                new Size(extractionSettings.Size.Width - 16, 16));

            // Decompression Settings
            FormContent.Add(PanelContent, decompressionSettings,
                "Decompression Settings",
                new Point(8, 184),
                new Size(PanelContent.Size.Width - 24, 104));

            // Decompress file
            FormContent.Add(decompressionSettings, decompressSourceFile, true,
                "Decompress archive before extracting.",
                new Point(8, 20),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Decompress extracted files
            FormContent.Add(decompressionSettings, decompressExtractedFile,
                "Decompress extracted files.",
                new Point(8, 40),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Decompress to different directory
            FormContent.Add(decompressionSettings, decompressExtractedDir,
                "Place decompressed files in different directory.",
                new Point(24, 60),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Use stored filename
            FormContent.Add(decompressionSettings, useStoredFilename, true,
                "Use filename stored in the compressed file.",
                new Point(8, 80),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Image Conversion Settings
            FormContent.Add(PanelContent, imageConversionSettings,
                "Texture Conversion Settings",
                new Point(8, 296),
                new Size(PanelContent.Size.Width - 24, 84));

            // Unpack image
            FormContent.Add(imageConversionSettings, unpackImage,
                "Convert extracted textures to PNG (if it is a supported texture).",
                new Point(8, 20),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Output to same directory
            FormContent.Add(imageConversionSettings, convertSameDir,
                "Output texture to same directory as source (and overwrite if necessary).",
                new Point(8, 40),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Delete source file
            FormContent.Add(imageConversionSettings, deleteSourceImage,
                "Delete source texture (on successful conversion).",
                new Point(8, 60),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Extract
            FormContent.Add(PanelContent, startWorkButton,
                "Extract",
                new Point((PanelContent.Width / 2) - 60, 388),
                new Size(120, 24),
                startWork);

            this.ShowDialog();
        }

        // Start Work
        private void startWork(object sender, EventArgs e)
        {
            // Disable the window
            PanelContent.Enabled = false;

            // Set up our background worker
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += run;

            // Now, show our status
            status = new StatusMessage("Archive - Extract", files);
            status.addProgressBarLocal();
            status.Show();

            bw.RunWorkerAsync();
        }

        // Extract the files
        private void run(object sender, DoWorkEventArgs e)
        {
            // Create the file list
            List<string> fileList = new List<string>();
            foreach (string i in files)
                fileList.Add(i);

            for (int i = 0; i < fileList.Count; i++)
            {
                // Set the current file in the status
                status.CurrentFile      = i;
                status.CurrentFileLocal = 0;

                // Set up the image file list for conversion
                List<string> imageFileList = new List<string>();

                try
                {
                    // Set up the file paths and open up the file
                    string InFile       = Path.GetFileName(fileList[i]);
                    string InDirectory  = Path.GetDirectoryName(fileList[i]);
                    string OutFile      = InFile;
                    string OutDirectory = InDirectory;

                    using (FileStream InputStream = new FileStream(fileList[i], FileMode.Open, FileAccess.Read))
                    {
                        Stream InputData = InputStream;

                        // Decompress the file
                        if (decompressSourceFile.Checked)
                        {
                            Compression compression = new Compression(InputData, InFile);
                            if (compression.Format != CompressionFormat.NULL)
                            {
                                MemoryStream DecompressedData = compression.Decompress();
                                if (DecompressedData != null)
                                {
                                    InputData = DecompressedData;
                                    if (useStoredFilename.Checked)
                                        InFile = compression.DecompressFilename;
                                }
                            }
                        }

                        // Open up the archive
                        Archive archive = new Archive(InputData, InFile);
                        if (archive.Format == ArchiveFormat.NULL)
                            continue;
                        ArchiveFileList ArchiveFileList = archive.GetFileList();
                        if (ArchiveFileList == null || ArchiveFileList.Entries.Length == 0)
                            continue;
                        status.TotalFilesLocal = ArchiveFileList.Entries.Length;

                        // Create the directory where we will extract the files to
                        if (extractDirSameFilename.Checked && deleteSourceArchive.Checked)
                            OutDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()); // Create a temporary place to store the files for now
                        else if (extractSameDir.Checked)
                            OutDirectory = InDirectory;
                        else
                            OutDirectory = InDirectory + Path.DirectorySeparatorChar + archive.OutputDirectory + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFile);
                        if (!Directory.Exists(OutDirectory))
                            Directory.CreateDirectory(OutDirectory);

                        // Extract the data from the archive
                        for (int j = 0; j < ArchiveFileList.Entries.Length; j++)
                        {
                            // Set the current file and get the data
                            status.CurrentFileLocal = j;
                            MemoryStream OutData = archive.GetData().Copy(ArchiveFileList.Entries[j].Offset, ArchiveFileList.Entries[j].Length);

                            // Get the filename that the file will be extracted to
                            if (extractFilenames.Checked && ArchiveFileList.Entries[j].Filename != String.Empty)
                                OutFile = ArchiveFileList.Entries[j].Filename;
                            else
                                OutFile = j.ToString().PadLeft(ArchiveFileList.Entries.Length.Digits(), '0') + FileTypeInfo.GetFileExtension(OutData);

                            // Decompress the data
                            if (decompressExtractedFile.Checked)
                            {
                                Compression compression = new Compression(OutData, OutFile);
                                if (compression.Format != CompressionFormat.NULL)
                                {
                                    MemoryStream DecompressedData = compression.Decompress();

                                    if (decompressExtractedDir.Checked)
                                    {
                                        // Write this to a different directory
                                        string TempOutFile = OutFile; // We will change it temporarily
                                        if (useStoredFilename.Checked)
                                            OutFile = compression.DecompressFilename;

                                        if (!Directory.Exists(OutDirectory + Path.DirectorySeparatorChar + compression.OutputDirectory))
                                            Directory.CreateDirectory(OutDirectory + Path.DirectorySeparatorChar + compression.OutputDirectory);
                                        using (FileStream OutputStream = new FileStream(OutDirectory + Path.DirectorySeparatorChar + compression.OutputDirectory + Path.DirectorySeparatorChar + OutFile, FileMode.Create, FileAccess.Write))
                                            OutputStream.Write(DecompressedData);

                                        OutFile = TempOutFile; // Reset the output file now
                                    }
                                    else
                                        OutData = DecompressedData;
                                }
                            }

                            // See if we want to extract subarchives
                            if (extractExtracted.Checked)
                            {
                                Archive archiveTemp = new Archive(OutData, OutFile);
                                if (archiveTemp.Format != ArchiveFormat.NULL)
                                {
                                    string AddFile = String.Empty;
                                    if (extractDirSameFilename.Checked && deleteSourceArchive.Checked)
                                        AddFile = fileList[i] + Path.DirectorySeparatorChar + OutFile;
                                    else
                                        AddFile = OutDirectory + Path.DirectorySeparatorChar + OutFile;

                                    fileList.Add(AddFile);
                                    status.AddFile(AddFile);
                                }
                            }

                            // Output an image
                            if (unpackImage.Checked)
                            {
                                Textures images = new Textures(OutData, OutFile);
                                if (images.Format != TextureFormat.NULL)
                                    imageFileList.Add(OutDirectory + Path.DirectorySeparatorChar + OutFile); // Add this to the image conversion list (in case we need a palette file)
                            }

                            // Write the file
                            using (FileStream OutputStream = new FileStream(OutDirectory + Path.DirectorySeparatorChar + OutFile, FileMode.Create, FileAccess.Write))
                                OutputStream.Write(OutData);
                        }
                    }

                    // Process image conversion now
                    if (unpackImage.Checked && imageFileList.Count > 0)
                    {
                        // Reset the local file count
                        status.CurrentFileLocal = 0;
                        status.TotalFilesLocal = imageFileList.Count;

                        for (int j = 0; j < imageFileList.Count; j++)
                        {
                            status.CurrentFileLocal = j;

                            string InFileBitmap       = Path.GetFileName(imageFileList[j]);
                            string InDirectoryBitmap  = Path.GetDirectoryName(imageFileList[j]);
                            string OutFileBitmap      = Path.GetFileNameWithoutExtension(InFileBitmap) + ".png";
                            string OutDirectoryBitmap = InDirectoryBitmap;

                            // Load the file
                            using (FileStream InputStream = new FileStream(imageFileList[j], FileMode.Open, FileAccess.Read))
                            {
                                Textures images = new Textures(InputStream, Path.GetFileName(imageFileList[j]));
                                if (images.Format != TextureFormat.NULL)
                                {
                                    if (!convertSameDir.Checked)
                                        OutDirectoryBitmap = InDirectoryBitmap + Path.DirectorySeparatorChar + images.OutputDirectory;

                                    // Start the conversion
                                    Bitmap ImageData = null;
                                    try
                                    {
                                        ImageData = images.Decode();
                                    }
                                    catch (TextureFormatNeedsPalette)
                                    {
                                        // We need a palette file
                                        if (File.Exists(InDirectoryBitmap + Path.DirectorySeparatorChar + images.PaletteFilename))
                                        {
                                            using (FileStream InStreamPalette = new FileStream(InDirectoryBitmap + Path.DirectorySeparatorChar + images.PaletteFilename, FileMode.Open, FileAccess.Read))
                                            {
                                                images.Decoder.PaletteData = InStreamPalette;
                                                ImageData = images.Decode();
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        continue;
                                    }

                                    // Output the image
                                    if (ImageData != null)
                                    {
                                        if (!Directory.Exists(OutDirectoryBitmap))
                                            Directory.CreateDirectory(OutDirectoryBitmap);

                                        ImageData.Save(OutDirectoryBitmap + Path.DirectorySeparatorChar + OutFileBitmap, ImageFormat.Png);
                                    }
                                }
                            }

                            if (deleteSourceImage.Checked &&
                                File.Exists(InDirectoryBitmap + Path.DirectorySeparatorChar + InFileBitmap) &&
                                File.Exists(OutDirectoryBitmap + Path.DirectorySeparatorChar + OutFileBitmap))
                                File.Delete(InDirectoryBitmap + Path.DirectorySeparatorChar + InFileBitmap);
                        }
                    }

                    // Delete the source archive now
                    if (deleteSourceArchive.Checked && File.Exists(fileList[i]))
                    {
                        File.Delete(fileList[i]);
                        if (extractDirSameFilename.Checked)
                        {
                            // If the source and destination directory are on the same volume we can just move the directory
                            if (Directory.GetDirectoryRoot(OutDirectory) == Directory.GetDirectoryRoot(fileList[i]))
                                Directory.Move(OutDirectory, fileList[i]);
                            // Otherwise we have to do a series of complicated file moves
                            else
                                MoveDirectory(new DirectoryInfo(OutDirectory), new DirectoryInfo(fileList[i]));
                        }
                    }
                }
                catch
                {
                    // Something went wrong. Continue please.
                    continue;
                }
            }

            // Close the status box now
            status.Close();
            this.Close();
        }

        // Move a directory (mainly for diferent volumes)
        private void MoveDirectory(DirectoryInfo sourceDir, DirectoryInfo destDir)
        {
            // Check if the directory exists
            if (!Directory.Exists(destDir.FullName))
                destDir.Create();

            // Copy each file in the directory
            FileInfo[] files = sourceDir.GetFiles();
            foreach (FileInfo file in files)
                file.MoveTo(Path.Combine(destDir.FullName, file.Name));

            // Copy the subdirectories
            DirectoryInfo[] dirs = sourceDir.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                MoveDirectory(dir, new DirectoryInfo(Path.Combine(destDir.FullName, dir.Name)));
                dir.Delete(); // Directory should be empty if we moved all the files
            }
        }
    }
}