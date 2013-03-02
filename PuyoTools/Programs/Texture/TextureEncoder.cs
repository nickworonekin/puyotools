using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Extensions;
using VrSharp.PvrTexture;

namespace PuyoTools
{
    public class TextureEncoder : Form
    {
        // Set up form variables
        private Panel PanelContent;

        private GroupBox
            imageConversionSettings = new GroupBox(), // Image Conversion Settings
            compressionSettings     = new GroupBox(); // Decompression Settings

        private CheckBox
            convertSameDir    = new CheckBox(), // Output to same directory
            deleteSourceImage = new CheckBox(), // Delete Source Image
            compressFile      = new CheckBox(), // Compress File
            addGbix           = new CheckBox(); // Add GBIX

        private ComboBox
            compressionFormat = new ComboBox(), // Compression Format
            dataFormat        = new ComboBox(), // Data Format
            paletteFormat     = new ComboBox(); // Palette Format

        private TextBox
            globalIndex = new TextBox();

        private Button
            importSettings  = new Button(), // Import Settings
            startWorkButton = new Button(); // Start Work Button

        private string[]
            files; // Files to Decompress

        private StatusMessage
            status; // Status Message

        // Compression Formats
        private List<string> CompressionNames              = new List<string>();
        private List<CompressionFormat> CompressionFormats = new List<CompressionFormat>();

        public TextureEncoder()
        {
            // Select the files
            files = FileSelectionDialog.OpenFiles("Select Image Files",
                "Supported Image Formats (*.png)|*.png|" +
                "All Files (*.*)|*.*");

            // If no files were selected, don't continue
            if (files == null || files.Length == 0)
                return;

            // Show Options
            showOptions();
        }

        public TextureEncoder(bool selectDirectory)
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
            FormContent.Create(this, "Image - Encoder", new Size(400, 316));
            PanelContent = new Panel() {
                Location = new Point(0, 0),
                Width    = this.ClientSize.Width,
                Height   = this.ClientSize.Height,
            };
            this.Controls.Add(PanelContent);

            // Fill the compression formats
            InitalizeCompressionFormats();

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
                "Image Conversion Settings",
                new Point(8, 32),
                new Size(PanelContent.Size.Width - 24, 172));

            // PVR temporarily
            // Palette Format
            imageConversionSettings.Controls.Add(new Label() {
                Text     = "PVR Pixel Format: ",
                Location = new Point(8, 24),
                Size     = new Size(112, 16),
            });
            FormContent.Add(imageConversionSettings, paletteFormat,
                new string[] { "ARGB1555", "RGB565", "ARGB4444" },
                new Point(8 + 112, 20),
                new Size(96, 20));

            // Data Format
            imageConversionSettings.Controls.Add(new Label() {
                Text     = "PVR Data Format: ",
                Location = new Point(8, 48),
                Size     = new Size(112, 16),
            });
            FormContent.Add(imageConversionSettings, dataFormat,
                new string[] { "Square Twiddled", "Rectangle", "Rectangular Twiddled" },
                new Point(8 + 112, 44),
                new Size(140, 20));

            // GBIX header
            imageConversionSettings.Controls.Add(new Label() {
                Text     = "Global Index:",
                Location = new Point(8, 72),
                Size     = new Size(112, 16),
            });
            globalIndex   = new TextBox() {
                Multiline = false,
                Location  = new Point(8 + 112, 68),
                Size      = new Size(112, 16),
                Text      = "0",
                MaxLength = 8,
            };
            globalIndex.KeyPress += delegate(object sender2, KeyPressEventArgs f) {
                if (!Char.IsDigit(f.KeyChar) && !Char.IsControl(f.KeyChar))
                    f.Handled = true;
            };
            imageConversionSettings.Controls.Add(globalIndex);
            addGbix = new CheckBox() {
                Checked  = true,
                Location = new Point(8 + 112 + 112 + 4, 72),
                Size     = new Size(16, 16),
            };
            imageConversionSettings.Controls.Add(addGbix);
            addGbix.CheckedChanged += delegate(object sender2, EventArgs f) {
                globalIndex.Enabled = addGbix.Checked;
            };

            // Import settings from another PVR
            importSettings = new Button() {
                Text     = "Import Settings from another PVR",
                Location = new Point(8, 92),
                Size     = new Size(224, 24),
            };
            importSettings.Click += new EventHandler(ImportPvrSettings);
            imageConversionSettings.Controls.Add(importSettings);

            // Output to same directory
            FormContent.Add(imageConversionSettings, convertSameDir,
                "Output file to same directory as source (and overwrite if necessary).",
                new Point(8, 128),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Delete source file
            FormContent.Add(imageConversionSettings, deleteSourceImage,
                "Delete source image (on successful conversion).",
                new Point(8, 148),
                new Size(imageConversionSettings.Size.Width - 16, 16));

            // Compression Settings
            FormContent.Add(PanelContent, compressionSettings,
                "Compression Settings",
                new Point(8, 212),
                new Size(PanelContent.Size.Width - 24, 64));

            // Compress file
            FormContent.Add(compressionSettings, compressFile, false,
                "Compress output image with the following compression:",
                new Point(8, 20),
                new Size(compressionSettings.Size.Width - 16, 16));
            compressFile.CheckedChanged += delegate(object sender2, EventArgs f) {
                compressionFormat.Enabled = compressFile.Checked;
            };


            // Compression Format
            FormContent.Add(compressionSettings, compressionFormat,
                CompressionNames.ToArray(),
                new Point(8, 36),
                new Size(120, 16));
            compressionFormat.Enabled = false;

            // Convert
            FormContent.Add(PanelContent, startWorkButton,
                "Convert",
                new Point((PanelContent.Width / 2) - 60, 284),
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
            status = new StatusMessage("Image - Convert", files);
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
                    Stream data = null;
                    string outputFilename  = Path.GetFileName(fileList[i]);
                    string outputDirectory = Path.GetDirectoryName(fileList[i]);
                    using (Stream inputStream = new FileStream(fileList[i], FileMode.Open, FileAccess.Read))
                    {
                        // Set up the PVR creator
                        data = inputStream;
                        PVR encoder = new PVR();

                        switch (paletteFormat.SelectedIndex)
                        {
                            case 0: encoder.PixelFormat = PvrPixelFormat.Argb1555; break;
                            case 1: encoder.PixelFormat = PvrPixelFormat.Rgb565; break;
                            case 2: encoder.PixelFormat = PvrPixelFormat.Argb4444; break;
                        }
                        switch (dataFormat.SelectedIndex)
                        {
                            case 0: encoder.DataFormat = PvrDataFormat.SquareTwiddled; break;
                            case 1: encoder.DataFormat = PvrDataFormat.Rectangle; break;
                            case 2: encoder.DataFormat = PvrDataFormat.RectangleTwiddled; break;
                        }

                        // Set the global index and gbix header
                        // There shouldn't be an error when parsing anymore, so we won't do checks
                        encoder.GbixHeader = addGbix.Checked;
                        if (addGbix.Checked && globalIndex.Text.Length > 0)
                            encoder.GlobalIndex = uint.Parse(globalIndex.Text);
                        else
                            encoder.GlobalIndex = 0;

                        // Set up our input and output image
                        string inputImage  = outputDirectory + Path.DirectorySeparatorChar + outputFilename;
                        string outputImage = outputDirectory + Path.DirectorySeparatorChar + (convertSameDir.Checked ? String.Empty : "PVR Created" + Path.DirectorySeparatorChar) + Path.GetFileNameWithoutExtension(outputFilename) + (compressFile.Checked && compressionFormat.SelectedIndex == 0 ? ".pvz" : ".pvr");
                        outputFilename     = outputImage;

                        // Create image
                        MemoryStream imageData = (MemoryStream)encoder.Encode(data);

                        // Don't continue if an image wasn't created
                        if (imageData == null)
                            continue;

                        // Compress image?
                        if (compressFile.Checked)
                        {
                            Compression compression = new Compression(imageData, outputFilename, CompressionFormats[compressionFormat.SelectedIndex]);

                            MemoryStream compressedData = compression.Compress();
                            if (compressedData != null)
                            {
                                imageData = compressedData;
                                outputImage = Path.GetDirectoryName(outputImage) + Path.DirectorySeparatorChar + compression.CompressFilename;
                            }
                        }

                        // Create the output directory if it does not exist
                        if (!Directory.Exists(Path.GetDirectoryName(outputImage)))
                            Directory.CreateDirectory(Path.GetDirectoryName(outputImage));

                        // Output the image
                        using (FileStream outputStream = new FileStream(outputImage, FileMode.Create, FileAccess.Write))
                            outputStream.Write(imageData);
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

        // Import PVR settings
        private void ImportPvrSettings(object sender, EventArgs e)
        {
            // Select Pvr
            string pvrFile = FileSelectionDialog.OpenFile("Select PVR file",
                "PVR Files (*.pvr;*.pvz)|*.pvr;*.pvz|" +
                "All Files (*.*)|*.*");

            if (pvrFile == null || pvrFile == String.Empty)
                return;

            // Load pvr file
            using (Stream file = new FileStream(pvrFile, FileMode.Open, FileAccess.Read))
            {
                Stream fileData = file;

                // Is the file compressed?
                Compression compression = new Compression(fileData, pvrFile);
                if (compression.Format != CompressionFormat.NULL)
                {
                    MemoryStream decompressedData = compression.Decompress();
                    if (decompressedData != null)
                        fileData = decompressedData;
                }

                // Is this a PVR?
                Textures images = new Textures(fileData, pvrFile);
                if (images.Format == TextureFormat.PVR)
                {
                    // Get file offset
                    int fileOffset = (fileData.ReadString(0x0, 4) == TextureHeader.GBIX ? 0x10 : 0x0);

                    // Get Palette Format, Data Format, and Global Index
                    byte pvrPaletteFormat = fileData.ReadByte(fileOffset + 0x8);
                    byte pvrDataFormat    = fileData.ReadByte(fileOffset + 0x9);

                    uint pvrGlobalIndex = 0;
                    if (fileData.ReadString(0x0, 4) == TextureHeader.GBIX)
                        pvrGlobalIndex = fileData.ReadUInt(0x8);

                    // Set palette format
                    switch (pvrPaletteFormat)
                    {
                        case 0x0: paletteFormat.SelectedIndex = 0; break;
                        case 0x1: paletteFormat.SelectedIndex = 1; break;
                        case 0x2: paletteFormat.SelectedIndex = 2; break;
                    }

                    // Set data format
                    switch (pvrDataFormat)
                    {
                        case 0x1: dataFormat.SelectedIndex = 0; break;
                        case 0x9: dataFormat.SelectedIndex = 1; break;
                        case 0xD: dataFormat.SelectedIndex = 2; break;
                    }

                    // Set global index
                    addGbix.Checked  = (fileData.ReadString(0x0, 4) == "GBIX");
                    globalIndex.Text = pvrGlobalIndex.ToString();
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
    }
}