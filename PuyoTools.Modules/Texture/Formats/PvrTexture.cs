using System;
using System.IO;
using System.Windows.Forms;

using VrSharp;
using VrSharp.PvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class PvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "PVR"; }
        }

        public override string FileExtension
        {
            get { return ".pvr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".pvp"; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="settings">Settings to use when decoding.</param>
        public override void Read(Stream source, Stream destination, int length, TextureReaderSettings settings)
        {
            // Reading PVR textures is done through VrSharp, so just pass it to that
            VrSharp.PvrTexture.PvrTexture texture = new VrSharp.PvrTexture.PvrTexture(source, length);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (settings != null && settings.PaletteStream != null)
                {
                    if (settings.PaletteLength == -1)
                    {
                        texture.SetPalette(new PvpPalette(settings.PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new PvpPalette(settings.PaletteStream, settings.PaletteLength));
                    }
                }
                else
                {
                    throw new TextureNeedsPaletteException();
                }
            }

            texture.Save(destination);
        }

        public override void Write(Stream source, Stream destination, int length, TextureWriterSettings settings)
        {
            WriterSettings writerSettings = (settings as WriterSettings) ?? new WriterSettings();

            // Writing PVR textures is done through VrSharp, so just pass it to that
            PvrTextureEncoder texture = new PvrTextureEncoder(source, length, writerSettings.PixelFormat, writerSettings.DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.HasGlobalIndex = writerSettings.HasGlobalIndex;
            if (texture.HasGlobalIndex)
            {
                texture.GlobalIndex = writerSettings.GlobalIndex;
            }

            // If we have an external palette file, save it
            if (texture.NeedsExternalPalette)
            {
                settings.PaletteStream = new MemoryStream();
                texture.PaletteEncoder.Save(settings.PaletteStream);
            }

            texture.Save(destination);
        }

        public override ModuleWriterSettings WriterSettingsObject()
        {
            return new WriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && VrSharp.PvrTexture.PvrTexture.Is(source, length));
        }

        public class WriterSettings : TextureWriterSettings
        {
            private PvrWriterSettings writerSettingsControls;

            public PvrPixelFormat PixelFormat = PvrPixelFormat.Argb1555;
            public PvrDataFormat DataFormat = PvrDataFormat.SquareTwiddled;
            public PvrCompressionFormat CompressionFormat = PvrCompressionFormat.None;

            public bool HasGlobalIndex = true;
            public uint GlobalIndex = 0;

            public override Control Content()
            {
                writerSettingsControls = new PvrWriterSettings();
                return writerSettingsControls;
            }

            public override void SetSettings()
            {
                // Set the pixel format
                switch (writerSettingsControls.PixelFormatBox.SelectedIndex)
                {
                    case 0: PixelFormat = PvrPixelFormat.Argb1555; break;
                    case 1: PixelFormat = PvrPixelFormat.Rgb565; break;
                    case 2: PixelFormat = PvrPixelFormat.Argb4444; break;
                }

                // Set the data format
                switch (writerSettingsControls.DataFormatBox.SelectedIndex)
                {
                    case 0: DataFormat = PvrDataFormat.SquareTwiddled; break;
                    case 1: DataFormat = PvrDataFormat.SquareTwiddledMipmaps; break;
                    case 2: DataFormat = PvrDataFormat.Index4; break;
                    case 3: DataFormat = PvrDataFormat.Index8; break;
                    case 4: DataFormat = PvrDataFormat.Rectangle; break;
                    case 5: DataFormat = PvrDataFormat.RectangleTwiddled; break;
                    case 6: DataFormat = PvrDataFormat.SquareTwiddledMipmapsAlt; break;
                }

                // Set the global index stuff
                HasGlobalIndex = writerSettingsControls.HasGlobalIndexCheckBox.Checked;
                if (HasGlobalIndex)
                {
                    if (!uint.TryParse(writerSettingsControls.GlobalIndexTextBox.Text, out GlobalIndex))
                    {
                        GlobalIndex = 0;
                    }
                }

                // RLE compressed?
                if (writerSettingsControls.RleCompressionCheckBox.Checked)
                {
                    CompressionFormat = PvrCompressionFormat.Rle;
                }
            }
        }
    }
}