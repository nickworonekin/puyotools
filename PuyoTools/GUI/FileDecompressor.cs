using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PuyoTools.GUI
{
    public partial class FileDecompressor : ToolForm
    {
        public FileDecompressor()
        {
            InitializeComponent();
        }

        private void Run(Settings settings)
        {
            foreach (string file in fileList)
            {
                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    CompressionFormat format;
                    MemoryStream buffer = new MemoryStream();

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Just run it through the decompressor.
                        // No need to check the format beforehand.
                        format = PTCompression.Decompress(source, buffer, (int)source.Length, Path.GetFileName(file));
                    }

                    // If the compression format is unknown, then nothing happened.
                    // Just continue on with the next file
                    if (format == CompressionFormat.Unknown)
                        continue;

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

            // The tool is finished doing what it needs to do. We can close it now.
            this.Close();
        }

        private struct Settings
        {
            public bool OverwriteSourceFile;
            public bool DeleteSourceFile;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            this.Enabled = false;

            // Set up the settings we will be using for this
            Settings settings = new Settings();
            settings.OverwriteSourceFile = overwriteSourceFileCheckbox.Checked;
            settings.DeleteSourceFile = deleteSourceFileCheckbox.Checked;

            // Run the tool
            Run(settings);
        }
    }
}
