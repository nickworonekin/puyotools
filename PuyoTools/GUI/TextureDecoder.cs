using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using PuyoTools.Modules.Texture;

namespace PuyoTools.GUI
{
    public partial class TextureDecoder : ToolForm
    {
        public TextureDecoder()
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
                    TextureFormat format;
                    MemoryStream textureData = new MemoryStream();

                    using (FileStream inStream = File.OpenRead(file))
                    {
                        // Set source to inStream
                        // The reason we do it like this is because source does not always equal inStream.
                        // You'll see why very soon.
                        Stream source = inStream;

                        // Get the format of the texture
                        format = Texture.GetFormat(source, Path.GetFileName(file));
                        if (format == TextureFormat.Unknown)
                        {
                            // Maybe it's compressed? Let's check.
                            // But first, we need to make sure we want to check
                            if (settings.DecodeCompressedTextures)
                            {
                                // Get the compression format, if it is compressed that is.
                                CompressionFormat compressionFormat = Compression.GetFormat(source, Path.GetFileName(file));
                                if (compressionFormat != CompressionFormat.Unknown)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then check the format again
                                    source = new MemoryStream();
                                    Compression.Decompress(inStream, source, compressionFormat);

                                    source.Position = 0;
                                    format = Texture.GetFormat(source, Path.GetFileName(file));
                                }
                            }

                            // If we still don't know what the texture format is, just skip the file.
                            if (format == TextureFormat.Unknown)
                            {
                                continue;
                            }
                        }

                        // Alright, let's decode the texture now
                        TextureBase texture = Texture.GetModule(format);
                        try
                        {
                            texture.Read(source, textureData, (int)source.Length);
                            //Texture.Read(source, textureData, (int)source.Length, format);
                        }
                        catch (TextureNeedsPaletteException)
                        {
                            // It appears that we need to load an external palette.
                            // Let's get the filename for this palette file, see if it exists, and load it in
                            string paletteName = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + Texture.GetModule(format).PaletteFileExtension;

                            if (!File.Exists(paletteName))
                            {
                                // If the palette file doesn't exist, just skip over this texture.
                                continue;
                            }

                            source.Position = 0; // We need to reset the position
                            textureData = new MemoryStream(); // Just incase some data was written
                            using (FileStream paletteData = File.OpenRead(paletteName))
                            {
                                texture.PaletteStream = paletteData;
                                texture.PaletteLength = (int)paletteData.Length;
                                texture.Read(source, textureData, (int)source.Length);
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

            // Set up the process dialog and then run the tool
            ProgressDialog dialog = new ProgressDialog();
            dialog.WindowTitle = "Processing";
            dialog.Title = "Decoding Textures";
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
