using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Extensions;

namespace PuyoTools
{
    public class Decompressor : Form
    {
        // Set up form variables
        private Panel PanelContent;

        private GroupBox
            decompressionSettings   = new GroupBox(), // Decompression Settings
            imageConversionSettings = new GroupBox(); // Image Conversion Settings

        private CheckBox
            useStoredFilename = new CheckBox(), // Use stored filename
            deleteSourceFile  = new CheckBox(), // Delete Source file
            decompressSameDir = new CheckBox(), // Output to same directory
            unpackImage       = new CheckBox(), // Unpack image
            deleteSourceImage = new CheckBox(), // Delete Source Image
            convertSameDir    = new CheckBox(); // Output to same directory


        private Button
            startWorkButton = new Button(); // Start Work Button

        private string[]
            files; // Files to Decompress

        private StatusMessage
            status; // Status Message

        public Decompressor()
        {
            // Select the files
            files = FileSelectionDialog.OpenFiles("Select Compressed Files",
                "All Files (*.*)|*.*|" +
                "CNX Compressed Files (*.cnx)|*.cnx|" +
                "NARC Compressed Archives (*.carc)|*.carc|" +
                "MRG Compressed Archives (*.mrz)|*.mrz|" +
                "ONE Compressed Archives (*.onz)|*.onz|" +
                "TEX Compressed Arcives (*.tez)|*.tez|" +
                "Puyo Fever 2 Compressed Files (*.mrz;*.tez)|*.mrg;*.tez");

            // If no files were selected, don't continue
            if (files == null || files.Length == 0)
                return;

            showOptions();
        }

        public Decompressor(bool selectDirectory)
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
            FormContent.Create(this, "Decompressor", new Size(400, 248));
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

            // Decompression Settings
            FormContent.Add(PanelContent, decompressionSettings,
                "Decompression Settings",
                new Point(8, 32),
                new Size(PanelContent.Size.Width - 24, 84));

            // Use stored filename
            FormContent.Add(decompressionSettings, useStoredFilename, true,
                "Use filename stored in the compressed file.",
                new Point(8, 20),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Output to same directory
            FormContent.Add(decompressionSettings, decompressSameDir,
                "Output file to same directory as source (and overwrite if necessary).",
                new Point(8, 40),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Delete source file
            FormContent.Add(decompressionSettings, deleteSourceFile,
                "Delete source file (on successful decompression).",
                new Point(8, 60),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Image Conversion Settings
            FormContent.Add(PanelContent, imageConversionSettings,
                "Texture Conversion Settings",
                new Point(8, 124),
                new Size(PanelContent.Size.Width - 24, 84));

            // Unpack image
            FormContent.Add(imageConversionSettings, unpackImage,
                "Convert decompressed textures to PNG (if it is a supported texture).",
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

            // Convert
            FormContent.Add(PanelContent, startWorkButton,
                "Decompress",
                new Point((PanelContent.Width / 2) - 60, 216),
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
            status = new StatusMessage("Compression - Decompress", files);
            status.Show();

            bw.RunWorkerAsync();
        }

        // Decompress the files
        private void run(object sender, DoWorkEventArgs e)
        {
            // Create the file list
            List<string> fileList = new List<string>();
            foreach (string i in files)
                fileList.Add(i);

            for (int i = 0; i < files.Length; i++)
            {
                // Set the current file
                status.CurrentFile = i;

                try
                {
                    // Open up the file
                    MemoryStream data;
                    string outputDirectory, outputFilename;
                    using (FileStream inputStream = new FileStream(fileList[i], FileMode.Open, FileAccess.Read))
                    {
                        // Set up the decompressor
                        Compression compression = new Compression(inputStream, Path.GetFileName(fileList[i]));
                        if (compression.Format == CompressionFormat.NULL)
                            continue;

                        // Set up the output directories and file names
                        outputDirectory = Path.GetDirectoryName(fileList[i]) + (decompressSameDir.Checked ? String.Empty : Path.DirectorySeparatorChar + compression.OutputDirectory);
                        outputFilename  = (useStoredFilename.Checked ? compression.DecompressFilename : Path.GetFileName(fileList[i]));

                        // Decompress data
                        MemoryStream decompressedData = compression.Decompress();

                        // Check to make sure the decompression was successful
                        if (decompressedData == null)
                            continue;
                        else
                            data = decompressedData;
                    }

                    // Create the output directory if it does not exist
                    if (!Directory.Exists(outputDirectory))
                        Directory.CreateDirectory(outputDirectory);

                    // Write file data
                    using (FileStream outputStream = new FileStream(outputDirectory + Path.DirectorySeparatorChar + outputFilename, FileMode.Create, FileAccess.Write))
                        outputStream.Write(data);

                    // Delete source file?
                    if (deleteSourceFile.Checked && File.Exists(fileList[i]))
                        File.Delete(fileList[i]);

                    // Unpack image?
                    if (unpackImage.Checked)
                    {
                        // Create Image object and make sure the format is supported
                        Textures images = new Textures(data, outputFilename);
                        if (images.Format != TextureFormat.NULL)
                        {
                            // Set up our input and output image
                            string inputImage  = outputDirectory + Path.DirectorySeparatorChar + outputFilename;
                            string outputImage = outputDirectory + Path.DirectorySeparatorChar + (convertSameDir.Checked ? String.Empty : images.OutputDirectory + Path.DirectorySeparatorChar) + Path.GetFileNameWithoutExtension(outputFilename) + ".png";

                            // Convert image
                            Bitmap imageData = null;
                            try
                            {
                                imageData = images.Decode();
                            }
                            catch (TextureFormatNeedsPalette)
                            {
                                // See if the palette file exists
                                if (File.Exists(outputDirectory + Path.DirectorySeparatorChar + images.PaletteFilename))
                                {
                                    using (FileStream paletteData = new FileStream(outputDirectory + Path.DirectorySeparatorChar + images.PaletteFilename, FileMode.Open, FileAccess.Read))
                                    {
                                        images.Decoder.PaletteData = paletteData;
                                        imageData = images.Decode();
                                    }
                                }
                            }

                            // Make sure an image was written
                            if (imageData != null)
                            {
                                data = new MemoryStream();
                                imageData.Save(data, ImageFormat.Png);

                                // Create the output directory if it does not exist
                                if (!Directory.Exists(Path.GetDirectoryName(outputImage)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(outputImage));

                                // Output the image
                                using (FileStream outputStream = new FileStream(outputImage, FileMode.Create, FileAccess.Write))
                                    outputStream.Write(data);

                                // Delete the source image if we want to
                                if (deleteSourceImage.Checked && File.Exists(inputImage) && File.Exists(outputImage))
                                    File.Delete(inputImage);
                            }
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
    }
}