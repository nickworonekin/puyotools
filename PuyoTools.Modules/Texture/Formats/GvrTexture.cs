using System;
using System.IO;

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

        public GvrTexture()
        {
            // Set default values
            HasGlobalIndex = true;
            GlobalIndex = 0;
            gbixType = GvrGbixType.Gbix;

            PaletteFormat = GvrPixelFormat.Unknown;
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
            // Reading GVR textures is done through VrSharp, so just pass it to that
            VrSharp.GvrTexture.GvrTexture texture = new VrSharp.GvrTexture.GvrTexture(source);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (PaletteStream != null)
                {
                    if (PaletteLength == -1)
                    {
                        texture.SetPalette(new GvpPalette(PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new GvpPalette(PaletteStream, PaletteLength));
                    }

                    PaletteStream = null;
                    PaletteLength = -1;
                }
                else
                {
                    throw new TextureNeedsPaletteException();
                }
            }

            texture.Save(destination);
        }

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
        public GvrPixelFormat PaletteFormat { get; set; }

        /// <summary>
        /// The texture's data format. The default value is GvrDataFormat.Rgb5a3.
        /// </summary>
        public GvrDataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets if this texture has mipmaps. This only applies to 4-bit or 16-bit non-palettized textures. The default value is false.
        /// </summary>
        public bool HasMipmaps { get; set; }

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
            // Writing GVR textures is done through VrSharp, so just pass it to that
            GvrTextureEncoder texture = new GvrTextureEncoder(source, PaletteFormat, DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.HasGlobalIndex = HasGlobalIndex;
            if (texture.HasGlobalIndex)
            {
                texture.GlobalIndex = GlobalIndex;
                texture.GbixType = GbixType;
            }

            texture.HasMipmaps = HasMipmaps;
            texture.NeedsExternalPalette = needsExternalPalette;

            // If we have an external palette file, save it
            if (texture.NeedsExternalPalette)
            {
                needsExternalPalette = true;

                PaletteStream = new MemoryStream();
                texture.PaletteEncoder.Save(PaletteStream);
            }
            else
            {
                needsExternalPalette = false;
            }

            texture.Save(destination);
        }

        public override ModuleSettingsControl GetModuleSettingsControl()
        {
            return new GvrWriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && VrSharp.GvrTexture.GvrTexture.Is(source, length));
        }
    }
}