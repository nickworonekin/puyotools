using System;
using System.IO;

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

        public PvrTexture()
        {
            // Set default values
            HasGlobalIndex = true;
            GlobalIndex = 0;

            PixelFormat = PvrPixelFormat.Argb1555;
            DataFormat = PvrDataFormat.SquareTwiddled;

            compressionFormat = PvrCompressionFormat.None;
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Read(Stream source, Stream destination)
        {
            // Reading PVR textures is done through VrSharp, so just pass it to that
            VrSharp.PvrTexture.PvrTexture texture = new VrSharp.PvrTexture.PvrTexture(source);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (PaletteStream != null)
                {
                    if (PaletteLength == -1)
                    {
                        texture.SetPalette(new PvpPalette(PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new PvpPalette(PaletteStream, PaletteLength));
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
        /// Sets the texture's compression format for encoding. The default value is PvrCompressionFormat.None.
        /// </summary>
        public PvrCompressionFormat CompressionFormat
        {
            get { return compressionFormat; }
            set
            {
                if (value != PvrCompressionFormat.None && value != PvrCompressionFormat.Rle)
                {
                    throw new ArgumentOutOfRangeException("CompressionFormat");
                }

                compressionFormat = value;
            }
        }
        private PvrCompressionFormat compressionFormat;

        /// <summary>
        /// Sets the texture's pixel format for encoding. The default value is PvrPixelFormat.Argb1555.
        /// </summary>
        public PvrPixelFormat PixelFormat { get; set; }

        /// <summary>
        /// Sets the texture's data format for encoding. The default value is PvrDataFormat.SquareTwiddled.
        /// </summary>
        public PvrDataFormat DataFormat { get; set; }
        #endregion

        public override void Write(Stream source, Stream destination)
        {
            // Writing PVR textures is done through VrSharp, so just pass it to that
            PvrTextureEncoder texture = new PvrTextureEncoder(source, PixelFormat, DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.CompressionFormat = compressionFormat;

            texture.HasGlobalIndex = HasGlobalIndex;
            if (texture.HasGlobalIndex)
            {
                texture.GlobalIndex = GlobalIndex;
            }

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
            return new PvrWriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && VrSharp.PvrTexture.PvrTexture.Is(source, length));
        }
    }
}