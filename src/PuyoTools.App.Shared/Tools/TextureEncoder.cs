using PuyoTools.App.Formats.Textures;
using PuyoTools.Modules.Texture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace PuyoTools.App.Tools
{
    class TextureEncoder
    {
        public static void Execute(
            ITextureFormat format,
            IList<string> files,
            TextureEncoderOptions options,
            ITextureFormatOptions formatOptions,
            IProgress<ToolProgress> progress = null)
        {
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // Report progress.
                progress?.Report(new ToolProgress(file, i));

                // Let's open the file.
                // But, we're going to do this in a try catch in case any errors happen.
                try
                {
                    // Set the output path and filename
                    string outPath;
                    if (options.OutputToSourceDirectory)
                    {
                        outPath = Path.GetDirectoryName(file);
                    }
                    else
                    {
                        outPath = Path.Combine(Path.GetDirectoryName(file), "Encoded Textures");
                    }
                    string outFname = Path.ChangeExtension(Path.GetFileName(file), format.FileExtension);

                    MemoryStream buffer = new MemoryStream();

                    // Run it through the texture encoder.
                    TextureBase texture = format.GetCodec();

                    // Events
                    if (texture is ITextureHasExternalPalette textureWithExternalPalette)
                    {
                        textureWithExternalPalette.ExternalPaletteCreated += (sender, e) =>
                        {
                            using (FileStream destination = File.Create(Path.Combine(outPath, Path.ChangeExtension(outFname, format.PaletteFileExtension))))
                            {
                                e.Palette.CopyTo(destination);
                            }
                        };
                    }

                    using (FileStream source = File.OpenRead(file))
                    {
                        // Set texture settings
                        /*ModuleSettingsControl settingsControl = settings.WriterSettingsControl;
                        if (settingsControl != null)
                        {
                            Action moduleSettingsAction = () => settingsControl.SetModuleSettings(texture);
                            settingsControl.Invoke(moduleSettingsAction);
                        }*/
                        if (formatOptions != null)
                        {
                            if (formatOptions is ISynchronizeInvoke invoker && invoker.InvokeRequired)
                            {
                                invoker.Invoke((Action)(() => formatOptions.MapTo(texture)), null);
                            }
                            else
                            {
                                formatOptions.MapTo(texture);
                            }
                        }

                        texture.Write(source, buffer, (int)source.Length);
                    }

                    // Do we want to compress this texture?
                    if (options.CompressionFormat != null)
                    {
                        MemoryStream tempBuffer = new MemoryStream();
                        buffer.Position = 0;

                        options.CompressionFormat.GetCodec().Compress(buffer, tempBuffer);

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

                    /*// Write out the palette file (if one was created along with the texture).
                    if (texture.PaletteStream != null)
                    {
                        using (FileStream destination = File.Create(Path.Combine(outPath, Path.ChangeExtension(outFname, settings.TextureFormat.PaletteFileExtension))))
                        {
                            texture.PaletteStream.Position = 0;
                            texture.PaletteStream.CopyTo(destination);
                        }
                    }*/

                    // Delete the source if the user chose to
                    if (options.DeleteSource)
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
    }
}
