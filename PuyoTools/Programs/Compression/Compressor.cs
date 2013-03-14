using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace PuyoTools.Old
{
    public class Compressor : Form
    {
        // Set up form variables
        private Panel PanelContent;

        private GroupBox
            compressionSettings   = new GroupBox(); // Decompression Settings

        private CheckBox
            useStoredFilename   = new CheckBox(), // Use stored filename
            deleteSourceFile    = new CheckBox(), // Delete Source file
            compressSameDir     = new CheckBox(), // Output to same directory
            unpackImage         = new CheckBox(), // Unpack image
            deleteSourceImage   = new CheckBox(), // Delete Source Image
            convertSameDir      = new CheckBox(), // Output to same directory
            filesizeRestriction = new CheckBox(); // Filesize restriction

        private ComboBox
            compressionFormat = new ComboBox(); // Compression Format

        private TextBox
            filesizeRestrictionSize = new TextBox();

        private List<string> CompressionNames = new List<string>();
        private List<CompressionFormat> CompressionFormats = new List<CompressionFormat>();


        private Button
            startWorkButton = new Button(); // Start Work Button

        private string[]
            files; // Files to Decompress

        private StatusMessage
            status; // Status Message

        public Compressor()
        {
            // Select the files
            files = FileSelectionDialog.OpenFiles("Select Files to Compress", "All Files|*.*");

            // If no files were selected, don't continue
            if (files == null || files.Length == 0)
                return;

            // Initalize Compression Formats
            InitalizeCompressionFormats();

            showOptions();
        }

        public Compressor(bool selectDirectory)
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

            // Initalize compression formats
            InitalizeCompressionFormats();

            // Show Options
            showOptions();
        }

        private void showOptions()
        {
            // Set up the form
            FormContent.Create(this, "Compression - Compress", new Size(400, 196));
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

            // Compression Settings
            FormContent.Add(PanelContent, compressionSettings,
                "Compression Settings",
                new Point(8, 32),
                new Size(PanelContent.Size.Width - 24, 124));

            // Compression Format
            FormContent.Add(compressionSettings, new Label(),
                "Compression Format:",
                new Point(8, 20),
                new Size(compressionSettings.Size.Width - 16, 16));

            // Compression Format
            FormContent.Add(compressionSettings, compressionFormat,
                CompressionNames.ToArray(),
                new Point(8, 36),
                new Size(120, 16));

            // Output to same directory
            FormContent.Add(compressionSettings, compressSameDir,
                "Output file to same directory as source (and overwrite if necessary).",
                new Point(8, 60),
                new Size(compressionSettings.Size.Width - 16, 16));

            // Filesize restriction
            FormContent.Add(compressionSettings, filesizeRestriction,
                "Don't compress files with a file size smaller than (in bytes):",
                new Point(8, 80),
                new Size(compressionSettings.Size.Width - 16, 16));
            FormContent.Add(compressionSettings, filesizeRestrictionSize,
                "0",
                new Point(24, 100),
                new Size(64, 16));
            filesizeRestrictionSize.KeyPress += new KeyPressEventHandler(TextBoxIntegersOnly);

            // Convert
            FormContent.Add(PanelContent, startWorkButton,
                "Compress",
                new Point((PanelContent.Width / 2) - 60, 164),
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
            status = new StatusMessage("Compression - Compress", files);
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

            // Get file size restriction size
            uint restrictSize = 0;
            if (filesizeRestriction.Checked && !uint.TryParse(filesizeRestrictionSize.Text, out restrictSize)) // For some reason if this happens
                restrictSize = 0;

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
                        // Check to see if we want to compress this file (filesize restriction)
                        if (filesizeRestriction.Checked && inputStream.Length < restrictSize)
                            continue;

                        // Setup the decompressor
                        Compression compression = new Compression(inputStream, Path.GetFileName(fileList[i]), CompressionFormats[compressionFormat.SelectedIndex]);

                        // Set up the output directories and file names
                        outputDirectory = Path.GetDirectoryName(fileList[i]) + (compressSameDir.Checked ? String.Empty : Path.DirectorySeparatorChar + "Compressed");
                        outputFilename  = Path.GetFileName(fileList[i]);

                        // Decompress data and get decompressed filename
                        MemoryStream compressedData = compression.Compress();
                        outputFilename = compression.CompressFilename;

                        // Check to make sure the decompression was successful
                        if (compressedData == null)
                            continue;
                        else
                            data = compressedData;
                    }

                    // Create the output directory if it does not exist
                    if (!Directory.Exists(outputDirectory))
                        Directory.CreateDirectory(outputDirectory);

                    // Write file data
                    using (FileStream outputStream = new FileStream(outputDirectory + Path.DirectorySeparatorChar + outputFilename, FileMode.Create, FileAccess.Write))
                        outputStream.Write(data);
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

        // Integer Only
        private void TextBoxIntegersOnly(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }
    }
}