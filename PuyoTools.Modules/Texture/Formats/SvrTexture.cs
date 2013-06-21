using System;
using System.IO;
using System.Windows.Forms;

using VrSharp;
using VrSharp.SvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class SvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "SVR"; }
        }

        public override string FileExtension
        {
            get { return ".svr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".svp"; }
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
            // Reading SVR textures is done through VrSharp, so just pass it to that
            VrSharp.SvrTexture.SvrTexture texture = new VrSharp.SvrTexture.SvrTexture(source, length);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (settings != null && settings.PaletteStream != null)
                {
                    if (settings.PaletteLength == -1)
                    {
                        texture.SetPalette(new SvpPalette(settings.PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new SvpPalette(settings.PaletteStream, settings.PaletteLength));
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

            // Writing SVR textures is done through VrSharp, so just pass it to that
            SvrTextureEncoder texture = new SvrTextureEncoder(source, length, writerSettings.PixelFormat, writerSettings.DataFormat);

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
            return (length > 16 && VrSharp.SvrTexture.SvrTexture.Is(source, length));
        }

        public class WriterSettings : TextureWriterSettings
        {
            private SvrWriterSettings writerSettingsPanel;

            public SvrPixelFormat PixelFormat = SvrPixelFormat.Rgb5a3;
            public SvrDataFormat DataFormat = SvrDataFormat.Rectangle;

            public bool HasGlobalIndex = true;
            public uint GlobalIndex = 0;

            public override void SetPanelContent(Panel panel)
            {
                writerSettingsPanel = new SvrWriterSettings();
                panel.Controls.Add(writerSettingsPanel);
            }

            public override void SetSettings()
            {
                // Set the pixel format
                switch (writerSettingsPanel.PixelFormatBox.SelectedIndex)
                {
                    case 0: PixelFormat = SvrPixelFormat.Rgb5a3; break;
                    case 1: PixelFormat = SvrPixelFormat.Argb8888; break;
                }

                // Set the data format
                switch (writerSettingsPanel.DataFormatBox.SelectedIndex)
                {
                    case 0: DataFormat = SvrDataFormat.Rectangle; break;
                    case 1: DataFormat = SvrDataFormat.Index4ExternalPalette; break;
                    case 2: DataFormat = SvrDataFormat.Index8ExternalPalette; break;
                    case 3: DataFormat = SvrDataFormat.Index4; break;
                    case 4: DataFormat = SvrDataFormat.Index8; break;
                }

                // Set the global index stuff
                HasGlobalIndex = writerSettingsPanel.HasGlobalIndexCheckBox.Checked;
                if (HasGlobalIndex)
                {
                    if (!uint.TryParse(writerSettingsPanel.GlobalIndexTextBox.Text, out GlobalIndex))
                    {
                        GlobalIndex = 0;
                    }
                }
            }
        }
    }
}