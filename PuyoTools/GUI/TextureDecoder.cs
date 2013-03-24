using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using PuyoTools.Texture;

namespace PuyoTools.GUI
{
    public partial class TextureDecoder : ToolForm
    {
        public TextureDecoder()
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
                    TextureFormat format;
                    MemoryStream textureData = new MemoryStream();

                    using (FileStream inStream = File.OpenRead(file))
                    {
                        // Set source to inStream
                        // The reason we do it like this is because source does not always equal inStream.
                        // You'll see why very soon.
                        Stream source = inStream;

                        // Get the format of the texture
                        format = PTTexture.GetFormat(source, (int)source.Length, Path.GetFileName(file));
                        if (format == TextureFormat.Unknown)
                        {
                            // Maybe it's compressed? Let's check.
                            // But first, we need to make sure we want to check
                            if (settings.DecodeCompressedTextures)
                            {
                                // Get the compression format, if it is compressed that is.
                                CompressionFormat compressionFormat = PTCompression.GetFormat(source, (int)source.Length, Path.GetFileName(file));
                                if (compressionFormat != CompressionFormat.Unknown)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then check the format again
                                    source = new MemoryStream();
                                    PTCompression.Decompress(inStream, source, (int)inStream.Length, compressionFormat);

                                    source.Position = 0;
                                    format = PTTexture.GetFormat(source, (int)source.Length, Path.GetFileName(file));
                                }
                            }

                            // If we still don't know what the texture format is, just skip the file.
                            if (format == TextureFormat.Unknown)
                            {
                                continue;
                            }
                        }

                        // Alright, let's decode the texture now
                        try
                        {
                            PTTexture.Read(source, textureData, (int)source.Length, format);
                        }
                        catch (TextureNeedsPaletteException)
                        {
                            // It appears that we need to load an external palette.
                            // Let's get the filename for this palette file, see if it exists, and load it in
                            string paletteName = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + PTTexture.Formats[format].PaletteExtension;

                            if (!File.Exists(paletteName))
                            {
                                // If the palette file doesn't exist, just skip over this texture.
                                continue;
                            }

                            source.Position = 0; // We need to reset the position
                            textureData = new MemoryStream(); // Just incase some data was written
                            using (FileStream paletteData = File.OpenRead(paletteName))
                            {
                                PTTexture.ReadWithPalette(source, paletteData, textureData, (int)source.Length, (int)paletteData.Length, format);
                            }

                            // Delete the palette file if the user chose to delete the source texture
                            if (settings.DeleteSource)
                            {
                                File.Delete(paletteName);
                            }
                        }
                    }

                    // Get the output path and create it if it does not exist.
                    string outPath;
                    if (settings.OutputToSourceDirectory)
                    {
                        outPath = Path.GetDirectoryName(file);
                    }
                    else
                    {
                        outPath = Path.Combine(Path.GetDirectoryName(file), "Decoded Textures");
                    }

                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    // Time to write out the file
                    using (FileStream destination = File.Create(Path.Combine(outPath, Path.GetFileNameWithoutExtension(file) + ".png")))
                    {
                        textureData.WriteTo(destination);
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

            // The tool is finished doing what it needs to do. We can close it now.
            this.Close();
        }

        private struct Settings
        {
            public bool DecodeCompressedTextures;
            public bool OutputToSourceDirectory;
            public bool DeleteSource;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            this.Enabled = false;

            // Set up the settings we will be using for this
            Settings settings = new Settings();
            settings.DecodeCompressedTextures = decodeCompressedTexturesButton.Checked;
            settings.OutputToSourceDirectory = outputToSourceDirButton.Checked;
            settings.DeleteSource = deleteSourceButton.Checked;

            // Run the tool
            Run(settings);
        }
    }
}
