using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PuyoTools.Modules;
using PuyoTools.Modules.Compression;
using PuyoTools.Modules.Texture;
using PuyoTools.Formats.Compression;
using PuyoTools.Formats.Textures;

namespace PuyoTools.GUI
{
    public partial class TextureEncoder : ToolForm
    {
        List<ModuleSettingsControl> writerSettingsControls;
        List<ITextureFormat> textureFormats;
        List<ICompressionFormat> compressionFormats;

        public TextureEncoder()
        {
            InitializeComponent();

            // Set up the writer settings panel and format writer settings
            writerSettingsControls = new List<ModuleSettingsControl>();

            // Fill the texture format box
            textureFormatBox.SelectedIndex = 0;
            textureFormats = new List<ITextureFormat>();
            foreach (var format in Texture.EncoderFormats)
            {
                textureFormatBox.Items.Add(format.Name);
                textureFormats.Add(format);

                writerSettingsControls.Add(format.GetModuleSettingsControl());
            }

            // Fill the compression format box
            compressionFormatBox.SelectedIndex = 0;
            compressionFormats = new List<ICompressionFormat>();
            foreach (var format in Compression.EncoderFormats)
            {
                compressionFormatBox.Items.Add(format.Name);
                compressionFormats.Add(format);
            }
        }

        private void EnableRunButton()
        {
            runButton.Enabled = (fileList.Count > 0 && textureFormatBox.SelectedIndex > 0);
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
                    // Set the output path and filename
                    string outPath;
                    if (settings.OutputToSourceDirectory)
                    {
                        outPath = Path.GetDirectoryName(file);
                    }
                    else
                    {
                        outPath = Path.Combine(Path.GetDirectoryName(file), "Encoded Textures");
                    }
                    string outFname = Path.ChangeExtension(Path.GetFileName(file), settings.TextureFormat.FileExtension);

                    MemoryStream buffer = new MemoryStream();

                    // Run it through the texture encoder.
                    TextureBase texture = settings.TextureFormat.GetCodec();

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Set the source path (really only used for GIM textures at the current moment).
                        texture.SourcePath = file;

                        // Set texture settings
                        ModuleSettingsControl settingsControl = settings.WriterSettingsControl;
                        if (settingsControl != null)
                        {
                            Action moduleSettingsAction = () => settingsControl.SetModuleSettings(texture);
                            settingsControl.Invoke(moduleSettingsAction);
                        }

                        texture.Write(source, buffer, (int)source.Length);
                    }

                    // Do we want to compress this texture?
                    if (settings.CompressionFormat != null)
                    {
                        MemoryStream tempBuffer = new MemoryStream();
                        buffer.Position = 0;

                        settings.CompressionFormat.GetCodec().Compress(buffer, tempBuffer);

                        buffer = tempBuffer;
                    }

                    // Create the output path it if it does not exist.
                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    // Time to write out the file
                    using (FileStream destination = File.Create(Path.Combine(outPath, outFname)))
                    {
                        buffer.WriteTo(destination);
                    }

                    // Write out the palette file (if one was created along with the texture).
                    if (texture.PaletteStream != null)
                    {
                        using (FileStream destination = File.Create(Path.Combine(outPath, Path.ChangeExtension(outFname, settings.TextureFormat.PaletteFileExtension))))
                        {
                            texture.PaletteStream.Position = 0;
                            PTStream.CopyTo(texture.PaletteStream, destination);
                        }
                    }

                    // Delete the source if the user chose to
                    if (settings.DeleteSource)
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

        private void textureFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            textureSettingsPanel.Controls.Clear();

            if (textureFormatBox.SelectedIndex != 0)
            {
                if (writerSettingsControls[textureFormatBox.SelectedIndex - 1] != null)
                {
                    textureSettingsPanel.Controls.Add(writerSettingsControls[textureFormatBox.SelectedIndex - 1]);
                }
            }

            EnableRunButton();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            this.Enabled = false;

            // Get the format of the texture the user wants to create
            ITextureFormat textureFormat = textureFormats[textureFormatBox.SelectedIndex - 1];

            // Set the settings for the tool
            Settings settings = new Settings();
            settings.TextureFormat = textureFormat;
            settings.OutputToSourceDirectory = outputToSourceDirButton.Checked;
            settings.DeleteSource = deleteSourceButton.Checked;

            if (compressionFormatBox.SelectedIndex != 0)
            {
                settings.CompressionFormat = compressionFormats[compressionFormatBox.SelectedIndex - 1];
            }
            else
            {
                settings.CompressionFormat = null;
            }

            settings.WriterSettingsControl = writerSettingsControls[textureFormatBox.SelectedIndex - 1];

            // Set up the process dialog and then run the tool
            ProgressDialog dialog = new ProgressDialog();
            dialog.WindowTitle = "Processing";
            dialog.Title = "Encoding Textures";
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

        private struct Settings
        {
            public ITextureFormat TextureFormat;
            public ICompressionFormat CompressionFormat;
            public ModuleSettingsControl WriterSettingsControl;

            public bool OutputToSourceDirectory;
            public bool DeleteSource;
        }
    }
}
