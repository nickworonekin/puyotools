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
    public class TextureDecoder : Form
    {
        // Set up form variables
        private Panel PanelContent;

        private GroupBox
            imageConversionSettings = new GroupBox(), // Image Conversion Settings
            decompressionSettings   = new GroupBox(); // Decompression Settings

        private CheckBox
            convertSameDir    = new CheckBox(), // Output to same directory
            deleteSourceImage = new CheckBox(), // Delete Source Image
            decompressFile    = new CheckBox(), // Decompress File
            useStoredFilename = new CheckBox(); // Use stored filename

        private Button
            startWorkButton = new Button(); // Start Work Button

        private string[]
            files; // Files to Decompress

        private StatusMessage
            status; // Status Message

        public TextureDecoder()
        {
            // Select the files
            files = FileSelectionDialog.OpenFiles("Select Image Files",
                "Supported Texture Formats (*.cnx;*.gim;*.gvr;*.pvr;*.pvz;*.svr)|*.cnx;*.gim;*.gvr;*.pvr;*.pvz;*.svr|" +
                "CNX Compressed Textures (*.cnx)|*.cnx|" +
                "GIM Textures (*.gim)|*.gim|" +
                "GVR Textures (*.gvr)|*.gvr|" +
                "PVR Textures (*.pvr;*.pvz)|*.pvr;*.pvz|" +
                "SVR Textures (*.svr)|*.svr|" +
                "All Files (*.*)|*.*");

            // If no files were selected, don't continue
            if (files == null || files.Length == 0)
                return;

            // Show Options
            showOptions();
        }

        public TextureDecoder(bool selectDirectory)
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
            FormContent.Create(this, "Texture Decoder", new Size(400, 208));
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

            // Image Conversion Settings
            FormContent.Add(PanelContent, imageConversionSettings,
                "Texture Conversion Settings",
                new Point(8, 32),
                new Size(PanelContent.Size.Width - 24, 64));

            // Output to same directory
            FormContent.Add(imageConversionSettings, convertSameDir,
                "Output file to same directory as source (and overwrite if necessary).",
                new Point(8, 20),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Delete source file
            FormContent.Add(imageConversionSettings, deleteSourceImage,
                "Delete source image (on successful conversion).",
                new Point(8, 40),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Decompression Settings
            FormContent.Add(PanelContent, decompressionSettings,
                "Decompression Settings",
                new Point(8, 104),
                new Size(PanelContent.Size.Width - 24, 64));

            // Decompress file
            FormContent.Add(decompressionSettings, decompressFile, true,
                "Decompress source file.",
                new Point(8, 20),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Use stored filename
            FormContent.Add(decompressionSettings, useStoredFilename, true,
                "Use filename stored in the compressed file.",
                new Point(8, 40),
                new Size(decompressionSettings.Size.Width - 16, 16));

            // Convert
            FormContent.Add(PanelContent, startWorkButton,
                "Convert",
                new Point((PanelContent.Width / 2) - 60, 176),
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
            status = new StatusMessage("Texture Decoder", files);
            status.Show();

            bw.RunWorkerAsync();
        }

        // Convert the images
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
                    MemoryStream data = null;
                    string outputFilename  = Path.GetFileName(fileList[i]);
                    string outputDirectory = Path.GetDirectoryName(fileList[i]);
                    using (Stream inputStream = new FileStream(fileList[i], FileMode.Open, FileAccess.Read))
                    {
                        // Decompress this file?
                        if (decompressFile.Checked)
                        {
                            // Set up the decompressor
                            Compression compression = new Compression(inputStream, Path.GetFileName(fileList[i]));
                            if (compression.Format != CompressionFormat.NULL)
                            {
                                // Decompress data
                                MemoryStream decompressedData = compression.Decompress();

                                // Check to make sure the decompression was successful
                                if (decompressedData != null)
                                {
                                    data = decompressedData;
                                    if (useStoredFilename.Checked)
                                        outputFilename = compression.DecompressFilename;
                                }
                            }
                        }

                        // Create Image object and make sure the format is supported
                        Textures images = new Textures((data == null ? inputStream : data), outputFilename);
                        if (images.Format == TextureFormat.NULL)
                            continue;

                        // Set up our input and output image
                        string inputImage  = outputDirectory + Path.DirectorySeparatorChar + outputFilename;
                        string outputImage = outputDirectory + Path.DirectorySeparatorChar + (convertSameDir.Checked ? String.Empty : images.OutputDirectory + Path.DirectorySeparatorChar) + Path.GetFileNameWithoutExtension(outputFilename) + ".png";
                        outputFilename     = outputImage;

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

                        // Don't continue if an image wasn't created
                        if (imageData == null)
                            continue;

                        data = new MemoryStream();
                        imageData.Save(data, ImageFormat.Png);

                        // Create the output directory if it does not exist
                        if (!Directory.Exists(Path.GetDirectoryName(outputImage)))
                            Directory.CreateDirectory(Path.GetDirectoryName(outputImage));

                        // Output the image
                        using (FileStream outputStream = new FileStream(outputImage, FileMode.Create, FileAccess.Write))
                            outputStream.Write(data);
                    }

                    // Delete the source image if we want to
                    if (deleteSourceImage.Checked && File.Exists(fileList[i]) && File.Exists(outputFilename))
                        File.Delete(fileList[i]);
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