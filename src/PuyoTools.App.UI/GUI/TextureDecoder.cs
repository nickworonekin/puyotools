using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using PuyoTools.Core.Textures;
using PuyoTools.App.Formats.Textures;
using PuyoTools.App;
using PuyoTools.App.Formats.Compression;
using PuyoTools.App.Tools;
using System.Threading.Tasks;

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
                    ITextureFormat format;
                    MemoryStream textureData = new MemoryStream();

                    using (FileStream inStream = File.OpenRead(file))
                    {
                        // Set source to inStream
                        // The reason we do it like this is because source does not always equal inStream.
                        // You'll see why very soon.
                        Stream source = inStream;

                        // Get the format of the texture
                        format = TextureFactory.GetFormat(source, Path.GetFileName(file));
                        if (format == null)
                        {
                            // Maybe it's compressed? Let's check.
                            // But first, we need to make sure we want to check
                            if (settings.DecodeCompressedTextures)
                            {
                                // Get the compression format, if it is compressed that is.
                                ICompressionFormat compressionFormat = CompressionFactory.GetFormat(source, Path.GetFileName(file));
                                if (compressionFormat != null)
                                {
                                    // Ok, it appears to be compressed. Let's decompress it, and then check the format again
                                    source = new MemoryStream();
                                    compressionFormat.GetCodec().Decompress(inStream, source);

                                    source.Position = 0;
                                    format = TextureFactory.GetFormat(source, Path.GetFileName(file));
                                }
                            }

                            // If we still don't know what the texture format is, just skip the file.
                            if (format == null)
                            {
                                continue;
                            }
                        }

                        // Alright, let's decode the texture now
                        TextureBase texture = format.GetCodec();

                        // Events
                        if (texture is ITextureHasExternalPalette textureWithExternalPalette)
                        {
                            textureWithExternalPalette.ExternalPaletteRequired += (sender, e) =>
                            {
                                // It appears that we need to load an external palette.
                                // Let's get the filename for this palette file, see if it exists, and load it in
                                string paletteName = Path.ChangeExtension(file, format.PaletteFileExtension);

                                if (!File.Exists(paletteName))
                                {
                                    // If the palette file doesn't exist, just skip over this texture.
                                    return;
                                }

                                // Copy the palette data to another stream so we can safely delete the data.
                                var paletteMemoryStream = new MemoryStream();
                                using (FileStream paletteFileStream = File.OpenRead(paletteName))
                                {
                                    paletteFileStream.CopyTo(paletteMemoryStream);
                                }
                                paletteMemoryStream.Position = 0;

                                e.Palette = paletteMemoryStream;

                                // Delete the palette file if the user chose to delete the source texture
                                if (settings.DeleteSource)
                                {
                                    File.Delete(paletteName);
                                }
                            };
                        }

                        try
                        {
                            texture.Read(source, textureData, (int)source.Length);
                        }
                        catch (TextureNeedsPaletteException)
                        {
                            // Just skip over this texture.
                            continue;

                            /*// It appears that we need to load an external palette.
                            // Let's get the filename for this palette file, see if it exists, and load it in
                            string paletteName = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + format.PaletteFileExtension;

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
                                texture.Read(source, textureData);
                            }

                            // Delete the palette file if the user chose to delete the source texture
                            if (settings.DeleteSource)
                            {
                                File.Delete(paletteName);
                            }*/
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

        private async void runButton_Click(object sender, EventArgs e)
        {
            // Disable the form
            Enabled = false;

            /*// Set up the settings we will be using for this
            Settings settings = new Settings
            {
                DecodeCompressedTextures = decodeCompressedTexturesButton.Checked,
                OutputToSourceDirectory = outputToSourceDirButton.Checked,
                DeleteSource = deleteSourceButton.Checked,
            };

            // Set up the process dialog and then run the tool
            ProgressDialog dialog = new ProgressDialog
            {
                WindowTitle = "Processing",
                Title = "Decoding Textures",
            };
            dialog.DoWork += (sender2, e2) => Run(settings, dialog);
            dialog.RunWorkerCompleted += (sender2, e2) => Close();
            dialog.RunWorkerAsync();*/

            // Create options in the format the tool uses
            var toolOptions = new TextureDecoderOptions
            {
                DecodeCompressedTextures = decodeCompressedTexturesButton.Checked,
                OutputToSourceDirectory = outputToSourceDirButton.Checked,
                DeleteSource = deleteSourceButton.Checked,
            };

            // Create the progress dialog and handler
            var progressDialog = new ProgressDialog
            {
                WindowTitle = "Processing",
                Title = "Decoding Textures",
            };

            var progress = new Progress<ToolProgress>(x =>
            {
                progressDialog.ReportProgress((int)(x.Progress * 100), $"Processing {Path.GetFileName(x.File)} ({x.Progress:P0})");
            });

            progressDialog.Show();

            // Execute the tool
            var tool = new PuyoTools.App.Tools.TextureDecoder(toolOptions);
            await Task.Run(() => tool.Execute(fileList, progress));

            // Close the dialogs
            progressDialog.Close();
            Close();
        }
    }
}
