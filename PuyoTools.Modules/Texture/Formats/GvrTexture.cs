using System;
using System.IO;
using System.Windows.Forms;

using VrSharp;
using VrSharp.GvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class GvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "GVR"; }
        }

        public override string FileExtension
        {
            get { return ".gvr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".gvp"; }
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
            // Reading GVR textures is done through VrSharp, so just pass it to that
            VrSharp.GvrTexture.GvrTexture texture = new VrSharp.GvrTexture.GvrTexture(source, length);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (settings != null && settings.PaletteStream != null)
                {
                    if (settings.PaletteLength == -1)
                    {
                        texture.SetPalette(new GvpPalette(settings.PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new GvpPalette(settings.PaletteStream, settings.PaletteLength));
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

            // Writing GVR textures is done through VrSharp, so just pass it to that
            GvrTextureEncoder texture = new GvrTextureEncoder(source, length, writerSettings.PaletteFormat, writerSettings.DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.HasGlobalIndex = writerSettings.HasGlobalIndex;
            if (texture.HasGlobalIndex)
            {
                texture.GlobalIndex = writerSettings.GlobalIndex;
                texture.GbixType = writerSettings.GbixType;
            }

            texture.HasMipmaps = writerSettings.HasMipmaps;
            texture.NeedsExternalPalette = writerSettings.HasExternalPalette;

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
            return (length > 16 && VrSharp.GvrTexture.GvrTexture.Is(source, length));
        }

        public class WriterSettings : TextureWriterSettings
        {
            private GvrWriterSettings writerSettingsPanel;

            public GvrPixelFormat PaletteFormat = GvrPixelFormat.Unknown;
            public GvrDataFormat DataFormat = GvrDataFormat.Rgb5a3;

            public bool HasGlobalIndex = true;
            public uint GlobalIndex = 0;
            public GvrGbixType GbixType = GvrGbixType.Gbix;

            public bool HasMipmaps = false;
            public bool HasExternalPalette = false;

            public override void SetPanelContent(Panel panel)
            {
                writerSettingsPanel = new GvrWriterSettings();
                panel.Controls.Add(writerSettingsPanel);
            }

            public override void SetSettings()
            {
                bool hasPaletteFormat = (writerSettingsPanel.DataFormatBox.SelectedIndex == 7 ||
                    writerSettingsPanel.DataFormatBox.SelectedIndex == 8);

                bool canHaveMipmaps = (writerSettingsPanel.DataFormatBox.SelectedIndex == 4 ||
                    writerSettingsPanel.DataFormatBox.SelectedIndex == 5 ||
                    writerSettingsPanel.DataFormatBox.SelectedIndex == 9);

                // Set the palette format
                if (writerSettingsPanel.PaletteFormatBox.Enabled)
                {
                    switch (writerSettingsPanel.PaletteFormatBox.SelectedIndex)
                    {
                        case 0: PaletteFormat = GvrPixelFormat.IntensityA8; break;
                        case 1: PaletteFormat = GvrPixelFormat.Rgb565; break;
                        case 2: PaletteFormat = GvrPixelFormat.Rgb5a3; break;
                    }
                }
                else
                {
                    PaletteFormat = GvrPixelFormat.Unknown;
                }

                // Set the data format
                switch (writerSettingsPanel.DataFormatBox.SelectedIndex)
                {
                    case 0: DataFormat = GvrDataFormat.Intensity4; break;
                    case 1: DataFormat = GvrDataFormat.Intensity8; break;
                    case 2: DataFormat = GvrDataFormat.IntensityA4; break;
                    case 3: DataFormat = GvrDataFormat.IntensityA8; break;
                    case 4: DataFormat = GvrDataFormat.Rgb565; break;
                    case 5: DataFormat = GvrDataFormat.Rgb5a3; break;
                    case 6: DataFormat = GvrDataFormat.Argb8888; break;
                    case 7: DataFormat = GvrDataFormat.Index4; break;
                    case 8: DataFormat = GvrDataFormat.Index8; break;
                    case 9: DataFormat = GvrDataFormat.Dxt1; break;
                }

                // Set the global index stuff
                HasGlobalIndex = writerSettingsPanel.HasGlobalIndexCheckBox.Checked;
                if (HasGlobalIndex)
                {
                    if (!uint.TryParse(writerSettingsPanel.GlobalIndexTextBox.Text, out GlobalIndex))
                    {
                        GlobalIndex = 0;
                    }

                    switch (writerSettingsPanel.GbixTypeBox.SelectedIndex)
                    {
                        case 0: GbixType = GvrGbixType.Gbix; break;
                        case 1: GbixType = GvrGbixType.Gcix; break;
                    }
                }

                // Has mipmaps?
                HasMipmaps = (canHaveMipmaps && writerSettingsPanel.HasMipmapsCheckBox.Checked);

                // Has external palette?
                HasExternalPalette = (hasPaletteFormat && writerSettingsPanel.HasExternalPaletteCheckBox.Checked);
            }
        }
    }
}