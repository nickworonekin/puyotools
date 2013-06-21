using System;
using System.IO;
using System.Windows.Forms;

using GimSharp;

namespace PuyoTools.Modules.Texture
{
    public class GimTexture : TextureBase
    {
        public override string Name
        {
            get { return "GIM"; }
        }

        public override string FileExtension
        {
            get { return ".gim"; }
        }

        public override string PaletteFileExtension
        {
            get { return String.Empty; }
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
            // Reading GIM textures is done through GimSharp, so just pass it to that
            GimSharp.GimTexture texture = new GimSharp.GimTexture(source, length);

            texture.Save(destination);
        }

        public override void Write(Stream source, Stream destination, int length, TextureWriterSettings settings)
        {
            WriterSettings writerSettings = (settings as WriterSettings) ?? new WriterSettings();

            // Writing GIM textures is done through GimSharp, so just pass it to that
            GimTextureEncoder texture = new GimTextureEncoder(source, length, writerSettings.PaletteFormat, writerSettings.DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.HasMetadata = writerSettings.HasMetadata;
            if (texture.HasMetadata)
            {
                texture.Metadata.OriginalFilename = Path.GetFileName(writerSettings.SourcePath);
                texture.Metadata.User = Environment.UserName;
                texture.Metadata.Program = "Puyo Tools";
            }

            texture.Save(destination);
        }

        public override ModuleWriterSettings WriterSettingsObject()
        {
            return new WriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 24 && GimSharp.GimTexture.Is(source, length));
        }

        public class WriterSettings : TextureWriterSettings
        {
            private GimWriterSettings writerSettingsPanel;

            public GimPaletteFormat PaletteFormat = GimPaletteFormat.Unknown;
            public GimDataFormat DataFormat = GimDataFormat.Rgb565;

            public bool HasMetadata = true;

            public override void SetPanelContent(Panel panel)
            {
                writerSettingsPanel = new GimWriterSettings();
                panel.Controls.Add(writerSettingsPanel);
            }

            public override void SetSettings()
            {
                bool hasPaletteFormat = (writerSettingsPanel.DataFormatBox.SelectedIndex == 4
                    || writerSettingsPanel.DataFormatBox.SelectedIndex == 5);

                // Set the palette format
                if (hasPaletteFormat)
                {
                    switch (writerSettingsPanel.PaletteFormatBox.SelectedIndex)
                    {
                        case 0: PaletteFormat = GimPaletteFormat.Rgb565; break;
                        case 1: PaletteFormat = GimPaletteFormat.Argb1555; break;
                        case 2: PaletteFormat = GimPaletteFormat.Argb4444; break;
                        case 3: PaletteFormat = GimPaletteFormat.Argb8888; break;
                    }
                }
                else
                {
                    PaletteFormat = GimPaletteFormat.Unknown;
                }

                // Set the data format
                switch (writerSettingsPanel.DataFormatBox.SelectedIndex)
                {
                    case 0: DataFormat = GimDataFormat.Rgb565; break;
                    case 1: DataFormat = GimDataFormat.Argb1555; break;
                    case 2: DataFormat = GimDataFormat.Argb4444; break;
                    case 3: DataFormat = GimDataFormat.Argb8888; break;
                    case 4: DataFormat = GimDataFormat.Index4; break;
                    case 5: DataFormat = GimDataFormat.Index8; break;
                }

                // Has metadata?
                HasMetadata = writerSettingsPanel.HasMetadataCheckBox.Checked;
            }
        }
    }
}