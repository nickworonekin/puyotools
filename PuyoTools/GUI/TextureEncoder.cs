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

namespace PuyoTools.GUI
{
    public partial class TextureEncoder : ToolForm
    {
        List<TextureWriterSettings> formatWriterSettings;
        List<Panel> writerSettingsPanel;
        List<TextureFormat> textureFormats;
        List<CompressionFormat> compressionFormats;

        public TextureEncoder()
        {
            InitializeComponent();

            // Set up the writer settings panel and format writer settings
            formatWriterSettings = new List<TextureWriterSettings>();
            writerSettingsPanel = new List<Panel>();

            // Fill the texture format box
            textureFormatBox.SelectedIndex = 0;
            textureFormats = new List<TextureFormat>();
            foreach (KeyValuePair<TextureFormat, TextureBase> format in Texture.Formats)
            {
                if (format.Value.CanWrite)
                {
                    textureFormatBox.Items.Add(format.Value.Name);
                    textureFormats.Add(format.Key);

                    TextureWriterSettings writerSettings = (TextureWriterSettings)format.Value.WriterSettingsObject();
                    if (writerSettings != null)
                    {
                        Panel panel = new Panel();
                        panel.AutoSize = true;
                        writerSettings.SetPanelContent(panel);
                        writerSettingsPanel.Add(panel);
                    }
                    else
                    {
                        writerSettingsPanel.Add(null);
                    }

                    formatWriterSettings.Add(writerSettings);
                }
            }

            // Fill the compression format box
            compressionFormatBox.SelectedIndex = 0;
            compressionFormats = new List<CompressionFormat>();
            foreach (KeyValuePair<CompressionFormat, CompressionBase> format in Compression.Formats)
            {
                if (format.Value.CanWrite)
                {
                    compressionFormatBox.Items.Add(format.Value.Name);
                    compressionFormats.Add(format.Key);
                }
            }
        }


        private void EnableRunButton()
        {
            runButton.Enabled = (fileList.Count > 0 && textureFormatBox.SelectedIndex > 0);
        }

        private void Run(Settings settings)
        {
            foreach (string file in fileList)
            {
                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    // Set the output path
                    string outPath = Path.Combine(Path.GetDirectoryName(file), "Encoded Textures");
                    string outFname = Path.ChangeExtension(Path.GetFileName(file), Texture.Formats[settings.TextureFormat].FileExtension);

                    // Set some things before we create the texture.
                    settings.TextureSettings.DestinationDirectory = outPath;
                    settings.TextureSettings.DestinationFileName = outFname;

                    MemoryStream buffer = new MemoryStream();

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Run it through the texture encoder.
                        Texture.Write(source, buffer, (int)source.Length, settings.TextureSettings, settings.TextureFormat);
                    }

                    // Do we want to compress this texture?
                    if (settings.CompressionFormat != CompressionFormat.Unknown)
                    {
                        MemoryStream tempBuffer = new MemoryStream();
                        buffer.Position = 0;

                        Compression.Compress(buffer, tempBuffer, (int)buffer.Length, settings.TextureSettings.DestinationFileName, settings.CompressionFormat);

                        buffer = tempBuffer;
                    }

                    // Create the output path if it does not exist.
                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    // Time to write out the file
                    using (FileStream destination = File.Create(Path.Combine(outPath, outFname)))
                    {
                        buffer.WriteTo(destination);
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

        private void textureFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            textureSettingsPanel.Controls.Clear();

            if (textureFormatBox.SelectedIndex != 0)
            {
                if (writerSettingsPanel[textureFormatBox.SelectedIndex - 1] != null)
                {
                    textureSettingsPanel.Controls.Add(writerSettingsPanel[textureFormatBox.SelectedIndex - 1]);
                }
            }

            EnableRunButton();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            this.Enabled = false;

            // Get the format of the texture the user wants to create
            TextureFormat textureFormat = textureFormats[textureFormatBox.SelectedIndex - 1];

            // Set the settings for the tool
            Settings settings = new Settings();
            settings.TextureFormat = textureFormat;

            if (compressionFormatBox.SelectedIndex != 0)
            {
                settings.CompressionFormat = compressionFormats[compressionFormatBox.SelectedIndex - 1];
            }
            else
            {
                settings.CompressionFormat = CompressionFormat.Unknown;
            }

            settings.TextureSettings = formatWriterSettings[textureFormatBox.SelectedIndex - 1];
            if (settings.TextureSettings != null)
            {
                settings.TextureSettings.SetSettings();
            }

            Run(settings);
        }

        private struct Settings
        {
            public TextureFormat TextureFormat;
            public CompressionFormat CompressionFormat;
            public TextureWriterSettings TextureSettings;
        }
    }
}
