using System;
using System.IO;

using PuyoTools.Core.Textures;
using PuyoTools.Core.Textures.Svr;

namespace PuyoTools.Core.Textures
{
    public class SvrTexture : TextureBase, ITextureHasExternalPalette
    {
        public SvrTexture()
        {
            // Set default values
            HasGlobalIndex = true;
            GlobalIndex = 0;

            PixelFormat = SvrPixelFormat.Rgb5a3;
            DataFormat = SvrDataFormat.Rectangle;
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Read(Stream source, Stream destination)
        {
            // Reading SVR textures is done through the SVR texture decoder, so just pass it to that
            SvrTextureDecoder texture = new SvrTextureDecoder(source);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                var eventArgs = new ExternalPaletteRequiredEventArgs();
                OnExternalPaletteRequired(eventArgs);

                if (eventArgs.Palette != null)
                {
                    texture.Palette = new SvrPalette(eventArgs.Palette);

                    if (eventArgs.CloseAfterRead)
                    {
                        eventArgs.Palette.Close();
                    }
                }
                else
                {
                    texture.Palette = new SvrGrayscalePalette();
                    //throw new TextureNeedsPaletteException();
                }
            }

            texture.Save(destination);
        }

        /// <inheritdoc/>
        public event EventHandler<ExternalPaletteRequiredEventArgs> ExternalPaletteRequired;

        protected virtual void OnExternalPaletteRequired(ExternalPaletteRequiredEventArgs e) => ExternalPaletteRequired?.Invoke(this, e);

        #region Writer Settings
        /// <summary>
        /// Sets whether or not this texture has a global index when encoding. If false, the texture will not include a GBIX header. The default value is true.
        /// </summary>
        public bool HasGlobalIndex { get; set; }

        /// <summary>
        /// Sets the texture's global index when encoding. This only matters if HasGlobalIndex is true. The default value is 0.
        /// </summary>
        public uint GlobalIndex { get; set; }

        /// <summary>
        /// Sets the texture's pixel format for encoding. The default value is SvrPixelFormat.Rgb5a3.
        /// </summary>
        public SvrPixelFormat PixelFormat { get; set; }

        /// <summary>
        /// Sets the texture's data format for encoding. The default value is SvrDataFormat.Rectangle.
        /// </summary>
        public SvrDataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets if dithering should be used when creating palette-based textures.
        /// </summary>
        public bool Dither { get; set; }
        #endregion

        public override void Write(Stream source, Stream destination)
        {
            // Writing SVR textures is done through the SVR texture encoder, so just pass it to that
            SvrTextureEncoder texture = new SvrTextureEncoder(source, PixelFormat, DataFormat);

            texture.GlobalIndex = HasGlobalIndex
                ? GlobalIndex
                : (uint?)null;

            texture.Dither = Dither;

            texture.Save(destination);

            // If we have an external palette file, save it
            if (texture.NeedsExternalPalette)
            {
                var paletteStream = new MemoryStream();
                texture.Palette.Save(paletteStream);
                paletteStream.Position = 0;

                OnExternalPaletteCreated(new ExternalPaletteCreatedEventArgs(paletteStream));
            }
        }

        /// <inheritdoc/>
        public event EventHandler<ExternalPaletteCreatedEventArgs> ExternalPaletteCreated;

        protected virtual void OnExternalPaletteCreated(ExternalPaletteCreatedEventArgs e) => ExternalPaletteCreated?.Invoke(this, e);

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source) => SvrTextureDecoder.Is(source);
    }
}