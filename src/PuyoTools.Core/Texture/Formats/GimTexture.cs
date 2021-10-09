using System;
using System.IO;

using GimSharp;
using GimSharpGimTexture = GimSharp.GimTexture;

namespace PuyoTools.Core.Texture
{
    public class GimTexture : TextureBase
    {
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
            GimSharpGimTexture texture = new GimSharpGimTexture(source);

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
                texture.Metadata.OriginalFilename = source is FileStream fs
                    ? Path.GetFileName(fs.Name)
                    : string.Empty;
                texture.Metadata.User = Environment.UserName;
                texture.Metadata.Program = "Puyo Tools";
            }

            texture.Save(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source) => GimSharpGimTexture.Is(source);
    }
}