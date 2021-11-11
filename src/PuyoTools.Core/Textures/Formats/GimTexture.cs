using System;
using System.IO;

using PuyoTools.Core.Textures.Gim;

namespace PuyoTools.Core.Textures
{
    public class GimTexture : TextureBase
    {
        public GimTexture()
        {
            // Set default values
            //PaletteFormat = GimPaletteFormat.Unknown;
            PaletteFormat = null;
            DataFormat = GimPixelFormat.Rgb565;

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
            // Reading GIM textures is done through the GIM texture decoder, so just pass it to that
            GimTextureDecoder texture = new GimTextureDecoder(source);

            texture.Save(destination);
        }

        #region Writer Settings
        /// <summary>
        /// The texture's palette format. The default value is GimPaletteFormat.Unknown.
        /// </summary>
        public GimPaletteFormat? PaletteFormat { get; set; }

        /// <summary>
        /// The texture's data format. The default value is GimDataFormat.Rgb565.
        /// </summary>
        public GimPixelFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets if the texture should include metadata. The default value is true.
        /// </summary>
        public bool HasMetadata { get; set; }

        /// <summary>
        /// Gets or sets if the texture should be swizzled.
        /// </summary>
        public bool Swizzle { get; set; }

        /// <summary>
        /// Gets or sets if dithering should be used when creating palette-based textures.
        /// </summary>
        public bool Dither { get; set; }
        #endregion

        public override void Write(Stream source, Stream destination)
        {
            // Writing GIM textures is done through GIM texture encoder, so just pass it to that
            GimTextureEncoder texture = new GimTextureEncoder(source, PaletteFormat, DataFormat);

            texture.HasMetadata = HasMetadata;
            texture.IsSwizzled = Swizzle;
            texture.Dither = Dither;
            /*if (texture.HasMetadata)
            {
                texture.Metadata.OriginalFilename = source is FileStream fs
                    ? Path.GetFileName(fs.Name)
                    : string.Empty;
                texture.Metadata.User = Environment.UserName;
                texture.Metadata.Program = "Puyo Tools";
            }*/

            texture.Save(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source) => GimTextureDecoder.Is(source);
    }
}