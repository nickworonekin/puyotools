using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PuyoTools.App.Tools;
using System.Threading.Tasks;
using PuyoTools.App.Formats.Compression;

namespace PuyoTools.GUI
{
    public partial class FileDecompressor : ToolForm
    {
        public FileDecompressor()
        {
            InitializeComponent();
        }

        private void Run(Settings settings, ProgressDialog dialog)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                string file = fileList[i];

                // Report progress. If we only have one file to process, no need to display (x of n).
                if (fileList.Count == 1)
                {
                    dialog.ReportProgress(i * 100 / fileList.Count, string.Format("Processing {0}", Path.GetFileName(file)));
                }
                else
                {
                    dialog.ReportProgress(i * 100 / fileList.Count, string.Format("Processing {0} ({1:N0} of {2:N0})", Path.GetFileName(file), i + 1, fileList.Count));
                }

                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    MemoryStream buffer = new MemoryStream();

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Get the compression format, then run it through the decompressor.
                        var format = CompressionFactory.GetFormat(source, Path.GetFileName(file));
                        if (format == null)
                        {
                            // File isn't compressed or the compression format is unknown.
                            // Just continue on with the next file.
                            continue;
                        }
                        format.GetCodec().Decompress(source, buffer);
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
                    string outPath = Path.Combine(Path.GetDirectoryName(file), "Decompressed Files");
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
            public bool OverwriteSourceFile;
            public bool DeleteSourceFile;
        }

        private async void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            Enabled = false;

            /*// Set up the settings we will be using for this
            Settings settings = new Settings
            {
                OverwriteSourceFile = overwriteSourceFileCheckbox.Checked,
                DeleteSourceFile = deleteSourceFileCheckbox.Checked,
            };

            // Set up the process dialog and then run the tool
            ProgressDialog dialog = new ProgressDialog
            {
                WindowTitle = "Processing",
                Title = "Decompressing Files",
            };
            dialog.DoWork += (sender2, e2) => Run(settings, dialog);
            dialog.RunWorkerCompleted += (sender2, e2) => Close();
            dialog.RunWorkerAsync();*/

            // Create options in the format the tool uses
            var toolOptions = new CompressionDecompressorOptions
            {
                OverwriteSourceFile = overwriteSourceFileCheckbox.Checked,
                DeleteSourceFile = deleteSourceFileCheckbox.Checked,
            };

            // Create the progress dialog and handler
            var progressDialog = new ProgressDialog
            {
                WindowTitle = "Processing",
                Title = "Decompressing Files",
            };

            var progress = new Progress<ToolProgress>(x =>
            {
                progressDialog.ReportProgress((int)(x.Progress * 100), $"Processing {Path.GetFileName(x.File)} ({x.Progress:P0})");
            });

            progressDialog.Show();

            // Execute the tool
            var tool = new CompressionDecompressor(toolOptions);
            await Task.Run(() => tool.Execute(fileList, progress));

            // Close the dialogs
            progressDialog.Close();
            Close();
        }
    }
}
