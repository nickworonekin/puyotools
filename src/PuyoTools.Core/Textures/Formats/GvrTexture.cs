using System;
using System.IO;

using PuyoTools.Core.Textures;
using PuyoTools.Core.Textures.Gvr;

namespace PuyoTools.Core.Textures
{
    public class GvrTexture : TextureBase, ITextureHasExternalPalette
    {
        public GvrTexture()
        {
            // Set default values
            HasGlobalIndex = true;
            GlobalIndex = 0;
            gbixType = GvrGbixType.Gbix;

            PaletteFormat = null;
            DataFormat = GvrDataFormat.Rgb5a3;

            HasMipmaps = false;
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Read(Stream source, Stream destination)
        {
            // Reading GVR textures is done through the GVR texture decoder, so just pass it to that
            GvrTextureDecoder texture = new GvrTextureDecoder(source);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                var eventArgs = new ExternalPaletteRequiredEventArgs();
                OnExternalPaletteRequired(eventArgs);

                if (eventArgs.Palette != null)
                {
                    texture.Palette = new GvrPalette(eventArgs.Palette);

                    if (eventArgs.CloseAfterRead)
                    {
                        eventArgs.Palette.Close();
                    }
                }
                else
                {
                    texture.Palette = new GvrGrayscalePalette();
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
        /// Indicates the magic code used for the GBIX header. This only matters if IncludeGbixHeader is true. The default value is GvrGbixType.Gbix.
        /// </summary>
        public GvrGbixType GbixType
        {
            get { return gbixType; }
            set
            {
                if (value != GvrGbixType.Gbix && value != GvrGbixType.Gcix)
                {
                    throw new ArgumentOutOfRangeException("GbixType");
                }

                gbixType = value;
            }
        }
        private GvrGbixType gbixType;

        /// <summary>
        /// The texture's palette format. This only applies to palettized textures. The default value is GvrPixelFormat.Unknown.
        /// </summary>
        public GvrPixelFormat? PaletteFormat { get; set; }

        /// <summary>
        /// The texture's data format. The default value is GvrDataFormat.Rgb5a3.
        /// </summary>
        public GvrDataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets if this texture has mipmaps. This only applies to 4-bit or 16-bit non-palettized textures. The default value is false.
        /// </summary>
        public bool HasMipmaps { get; set; }

        /// <summary>
        /// Gets or sets if dithering should be used when creating palette-based textures.
        /// </summary>
        public bool Dither { get; set; }

        /// <summary>
        /// Gets or sets if the texture needs an external palette file. This only applies to palettized textures.
        /// After encoding, this will state if the texture actually has an external palette file.
        /// </summary>
        /// <returns></returns>
        public new bool NeedsExternalPalette
        {
            get { return needsExternalPalette; }
            set { needsExternalPalette = value; }
        }
        #endregion

        public override void Write(Stream source, Stream destination)
        {
            // Writing GVR textures is done through the GVR texture encoder, so just pass it to that
            GvrTextureEncoder texture = new GvrTextureEncoder(source, PaletteFormat, DataFormat);

            texture.GlobalIndexType = GbixType;
            texture.GlobalIndex = HasGlobalIndex
                ? GlobalIndex
                : (uint?)null;

            texture.HasMipmaps = HasMipmaps;
            texture.Dither = Dither;
            texture.NeedsExternalPalette = needsExternalPalette;

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
        public static bool Identify(Stream source) => GvrTextureDecoder.Is(source);
    }
}