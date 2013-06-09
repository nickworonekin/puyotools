using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.GvrTexture
{
    public class GvrTextureEncoder : VrTextureEncoder
    {
        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(string file, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat) : base(file)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(byte[] source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(byte[] source, int offset, int length, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source, offset, length)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Stream source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Stream source, int length, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source, length)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a bitmap.
        /// </summary>
        /// <param name="source">Bitmap to encode.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Bitmap source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedBitmap != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        private bool Initalize(GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
        {
            // Set the default values
            hasGlobalIndex = true;
            gbixType = GvrGbixType.Gbix;
            globalIndex = 0;

            // Set the data format and pixel format and load the appropiate codecs
            this.dataFormat = dataFormat;
            dataCodec = GvrDataCodec.GetDataCodec(dataFormat);

            // Make sure the data codec exists and we can encode to it
            if (dataCodec == null || !dataCodec.CanEncode) return false;

            // Only palettized formats require a pixel codec.
            if (dataCodec.PaletteEntries != 0)
            {
                this.pixelFormat = pixelFormat;
                pixelCodec = GvrPixelCodec.GetPixelCodec(pixelFormat);

                // Make sure the pixel codec exists and we can encode to it
                if (pixelCodec == null || !pixelCodec.CanEncode) return false;

                dataCodec.PixelCodec = pixelCodec;

                dataFlags = GvrDataFlags.InternalPalette;

                // Convert the bitmap to an array containing indicies.
                decodedData = BitmapToRawIndexed(decodedBitmap, dataCodec.PaletteEntries, out texturePalette);
            }
            else
            {
                this.pixelFormat = GvrPixelFormat.Unknown;
                pixelCodec = null;

                dataFlags = GvrDataFlags.None;

                // Convert the bitmap to an array
                decodedData = BitmapToRaw(decodedBitmap);
            }

            return true;
        }
        #endregion

        #region Texture Properties
        /// <summary>
        /// Indicates the magic code used for the GBIX header. This only matters if IncludeGbixHeader is true. The default value is GvrGbixType.Gbix.
        /// </summary>
        public GvrGbixType GbixType
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return gbixType;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                gbixType = value;
            }
        }
        protected GvrGbixType gbixType;

        /// <summary>
        /// The texture's data flags. The default value is GvrDataDlags.InternalPalette if the data format is GvrDataFormat.Index4 or GvrDataFormat.Index8; GvrDataFlags.None otherwise.
        /// If both GvrDataFlags.InternalPalette and GvrDataFlags.ExternalPalette are set, GvrDataFlags.ExternalPalette will take precedence.
        /// </summary>
        public GvrDataFlags DataFlags
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataFlags;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                dataFlags = value;

                // If GvrDataFlags.ExternalPalette is set, set up the palette encoder.
                // Remove the palette encoder if the opposite is done.
                if (paletteEncoder == null && (dataFlags & GvrDataFlags.ExternalPalette) != 0)
                {
                    paletteEncoder = new GvpPaletteEncoder(texturePalette, (ushort)dataCodec.PaletteEntries, pixelFormat, pixelCodec);
                }
                else if (paletteEncoder != null && (dataFlags & GvrDataFlags.ExternalPalette) == 0)
                {
                    paletteEncoder = null;
                }
            }
        }
        protected GvrDataFlags dataFlags;

        /// <summary>
        /// The texture's pixel format. This only applies to palettized textures.
        /// </summary>
        public GvrPixelFormat PixelFormat
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return pixelFormat;
            }
        }
        private GvrPixelFormat pixelFormat;

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public GvrDataFormat DataFormat
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataFormat;
            }
        }
        private GvrDataFormat dataFormat;

        /// <summary>
        /// Gets or sets if this texture has mipmaps. This only applies to non-palettized textures.
        /// </summary>
        public bool HasMipmaps
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return hasMipmaps;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                hasMipmaps = value;
            }
        }
        private bool hasMipmaps;
        #endregion

        #region Palette
        /// <summary>
        /// Returns if the texture needs an external palette file.
        /// </summary>
        /// <returns></returns>
        public override bool NeedsExternalPalette
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return ((dataFlags & GvrDataFlags.ExternalPalette) != 0);
            }
        }
        #endregion

        #region Encode Texture
        protected override MemoryStream EncodeTexture()
        {
            // Before we write anything, let's make sure the data flags are set properly
            if ((dataFlags & GvrDataFlags.InternalPalette) != 0 && (dataFlags & GvrDataFlags.ExternalPalette) != 0)
            {
                // If both InternalPalette and ExternalPalette is set, default to ExternalPalette.
                dataFlags &= ~GvrDataFlags.InternalPalette;
            }

            if ((dataFlags & GvrDataFlags.Palette) != 0 && dataCodec.PaletteEntries == 0)
            {
                // If this texture has no palette, then don't set any palette flags.
                dataFlags &= ~GvrDataFlags.Palette;
            }

            // Temporary! Unset the mipmap flag if it is set (as there is currently no mipmap encode support)
            if ((dataFlags & GvrDataFlags.Mipmaps) != 0)
            {
                dataFlags &= ~GvrDataFlags.Mipmaps;
            }

            // Calculate what the length of the texture will be
            int textureLength = 16 + (textureWidth * textureHeight * dataCodec.Bpp / 8);
            if (hasGlobalIndex)
            {
                textureLength += 16;
            }
            if ((dataFlags & GvrDataFlags.InternalPalette) != 0)
            {
                textureLength += (dataCodec.PaletteEntries * pixelCodec.Bpp / 8);
            }

            MemoryStream destination = new MemoryStream(textureLength);

            // Write out the GBIX header (if we are including one)
            if (hasGlobalIndex)
            {
                if (gbixType == GvrGbixType.Gbix)
                {
                    destination.WriteByte((byte)'G');
                    destination.WriteByte((byte)'B');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'X');
                }
                else
                {
                    destination.WriteByte((byte)'G');
                    destination.WriteByte((byte)'C');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'X');
                }

                PTStream.WriteUInt32(destination, 8);
                PTStream.WriteUInt32BE(destination, globalIndex);
                PTStream.WriteUInt32(destination, 0);
            }

            // Write out the GVRT header
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'R');
            destination.WriteByte((byte)'T');

            PTStream.WriteInt32(destination, textureLength - 24);

            PTStream.WriteUInt16(destination, 0);
            if (PixelFormat != GvrPixelFormat.Unknown)
            {
                destination.WriteByte((byte)(((byte)pixelFormat << 4) | ((byte)dataFlags & 0x0F)));
            }
            else
            {
                destination.WriteByte((byte)((byte)dataFlags & 0xF));
            }
            destination.WriteByte((byte)dataFormat);

            PTStream.WriteUInt16BE(destination, textureWidth);
            PTStream.WriteUInt16BE(destination, textureHeight);

            // If we have an internal palette, write it
            if ((dataFlags & GvrDataFlags.InternalPalette) != 0)
            {
                byte[] palette = pixelCodec.EncodePalette(texturePalette, dataCodec.PaletteEntries);
                destination.Write(palette, 0, palette.Length);
            }

            // Write the texture data
            byte[] textureData = dataCodec.Encode(decodedData, textureWidth, textureHeight, null);
            destination.Write(textureData, 0, textureData.Length);

            return destination;
        }
        #endregion
    }
}