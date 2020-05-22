using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PuyoTools.Modules.Compression;
using PuyoTools.Formats.Compression;

namespace PuyoTools.GUI
{
    public partial class FileCompressor : ToolForm
    {
        List<ICompressionFormat> compressionFormats;

        public FileCompressor()
        {
            InitializeComponent();

            // Add additional event handlers from the base class
            addFilesButton.Click += EnableRunButton;
            addDirectoryButton.Click += EnableRunButton;

            // Fill the compression format box
            compressionFormatBox.SelectedIndex = 0;
            compressionFormats = new List<ICompressionFormat>();
            foreach (var format in Compression.EncoderFormats)
            {
                compressionFormatBox.Items.Add(format.Name);
                compressionFormats.Add(format);
            }
        }

        private void EnableRunButton(object sender, EventArgs e)
        {
            runButton.Enabled = (fileList.Count > 0 && compressionFormatBox.SelectedIndex > 0);
        }

        private void Run(Settings settings, ProgressDialog dialog)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                string file = fileList[i];

                // Report progress. If we only have one file to process, no need to display (x of n).
                if (fileList.Count == 1)
                {
                    dialog.ReportProgress(i * 100 / fileList.Count, String.Format("Processing {0}", Path.GetFileName(file)));
                }
                else
                {
                    dialog.ReportProgress(i * 100 / fileList.Count, String.Format("Processing {0} ({1:N0} of {2:N0})", Path.GetFileName(file), i + 1, fileList.Count));
                }

                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    MemoryStream buffer = new MemoryStream();

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Run it through the compressor.
                        settings.CompressionFormat.GetCodec().Compress(source, buffer);
                    }

                    // Now that we have a decompressed file (we hope!), let's see what we need to do with it.
                    if (settings.OverwriteSourceFile)
                    {
                        // Overwrite the source file. Ok, we can do that!
                        using (FileStream destination = File.Create(file))
                        {
                            buffer.WriteTo(destination);

                            // We are done here. Continue on with the next file
                            continue;
                        }
                    }

                    // Get the output path and create it if it does not exist.
                    string outPath = Path.Combine(Path.GetDirectoryName(file), "Compressed Files");
                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    // Time to write out the file
                    using (FileStream destination = File.Create(Path.Combine(outPath, Path.GetFileName(file))))
                    {
                        buffer.WriteTo(destination);
                    }

                    // Delete the source file if the user chose to
                    if (settings.DeleteSourceFile)
                    {
                        File.Delete(file);
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
            public ICompressionFormat CompressionFormat;
            public bool OverwriteSourceFile;
            public bool DeleteSourceFile;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            this.Enabled = false;

            // Set up the settings we will be using for this
            Settings settings = new Settings();
            settings.CompressionFormat = compressionFormats[compressionFormatBox.SelectedIndex - 1];
            settings.OverwriteSourceFile = overwriteSourceFileCheckbox.Checked;
            settings.DeleteSourceFile = deleteSourceFileCheckbox.Checked;

            // Set up the process dialog and then run the tool
            ProgressDialog dialog = new ProgressDialog();
            dialog.WindowTitle = "Processing";
            dialog.Title = "Compressing Files";
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
