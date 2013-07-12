using System;
using System.IO;

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

        public GimTexture()
        {
            // Set default values
            PaletteFormat = GimPaletteFormat.Unknown;
            DataFormat = GimDataFormat.Rgb565;

            HasMetadata = true;
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Read(Stream source, Stream destination)
        {
            // Reading GIM textures is done through GimSharp, so just pass it to that
            GimSharp.GimTexture texture = new GimSharp.GimTexture(source);

            texture.Save(destination);
        }

        #region Writer Settings
        /// <summary>
        /// The texture's palette format. The default value is GimPaletteFormat.Unknown.
        /// </summary>
        public GimPaletteFormat PaletteFormat { get; set; }

        /// <summary>
        /// The texture's data format. The default value is GimDataFormat.Rgb565.
        /// </summary>
        public GimDataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets if the texture should include metadata. The default value is true.
        /// </summary>
        public bool HasMetadata { get; set; }
        #endregion

        public override void Write(Stream source, Stream destination)
        {
            // Writing GIM textures is done through GimSharp, so just pass it to that
            GimTextureEncoder texture = new GimTextureEncoder(source, PaletteFormat, DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.HasMetadata = HasMetadata;
            if (texture.HasMetadata)
            {
                texture.Metadata.OriginalFilename = Path.GetFileName(SourcePath);
                texture.Metadata.User = Environment.UserName;
                texture.Metadata.Program = "Puyo Tools";
            }

            texture.Save(destination);
        }

        public override ModuleSettingsControl GetModuleSettingsControl()
        {
            return new GimWriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 24 && GimSharp.GimTexture.Is(source, length));
        }
    }
}