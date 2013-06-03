using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.PvrTexture
{
    public class PvrTextureEncoder : VrTextureEncoder
    {
        #region Fields
        private PvrCompressionCodec compressionCodec;   // Compression codec
        #endregion

        #region Texture Properties
        /// <summary>
        /// The texture's compression format. The default value is PvrCompressionFormat.None.
        /// </summary>
        public PvrCompressionFormat CompressionFormat
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return compressionFormat;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                compressionFormat = value;
            }
        }
        protected PvrCompressionFormat compressionFormat;

        /// <summary>
        /// The texture's pixel format.
        /// </summary>
        public PvrPixelFormat PixelFormat
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
        private PvrPixelFormat pixelFormat;

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public PvrDataFormat DataFormat
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
        private PvrDataFormat dataFormat;
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(string file, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat) : base(file)
        {
            if (decodedData != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(byte[] source, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedData != null)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(byte[] source, int offset, int length, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
            : base(source, offset, length)
        {
            if (decodedData != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(Stream source, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedData != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(Stream source, int length, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
            : base(source, length)
        {
            if (decodedData != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a bitmap.
        /// </summary>
        /// <param name="source">Bitmap to encode.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(Bitmap source, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
            : base(source)
        {
            if (decodedData != null)
            {
                initalized = Initalize(pixelFormat, dataFormat);
            }
        }

        private bool Initalize(PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
        {
            // Set the default values
            includeGbixHeader = true;
            globalIndex = 0;
            compressionFormat = PvrCompressionFormat.None;

            // Set the data format and pixel format and load the appropiate codecs
            this.pixelFormat = pixelFormat;
            pixelCodec = PvrPixelCodec.GetPixelCodec(pixelFormat);

            this.dataFormat = dataFormat;
            dataCodec = PvrDataCodec.GetDataCodec(dataFormat);

            // Make sure the pixel and data codecs exists and we can encode to it
            if (pixelCodec == null || !pixelCodec.CanEncode) return false;
            if (dataCodec == null || !dataCodec.CanEncode) return false;

            if (dataCodec.PaletteEntries != 0)
            {
                // Convert the bitmap to an array containing indicies.
                decodedData = BitmapToRawIndexed(decodedBitmap, dataCodec.PaletteEntries, out texturePalette);

                // If this texture has an external palette file, set up the palette encoder
                if (NeedsExternalPalette)
                {
                    paletteEncoder = new PvpPaletteEncoder(texturePalette, (ushort)dataCodec.PaletteEntries, pixelFormat, pixelCodec);
                }
            }
            else
            {
                // Convert the bitmap to an array
                decodedData = BitmapToRaw(decodedBitmap);
            }

            return true;
        }
        #endregion

        #region Encode Texture
        protected override MemoryStream EncodeTexture()
        {
            // Calculate what the length of the texture will be
            int textureLength = 16 + (textureWidth * textureHeight * dataCodec.Bpp / 8);
            if (includeGbixHeader)
            {
                textureLength += 16;
            }
            if (dataCodec.PaletteEntries != 0 && !dataCodec.NeedsExternalPalette)
            {
                textureLength += (dataCodec.PaletteEntries * pixelCodec.Bpp / 8);
            }

            MemoryStream destination = new MemoryStream(textureLength);

            // Write out the GBIX header (if we are including one)
            if (includeGbixHeader)
            {
                destination.WriteByte((byte)'G');
                destination.WriteByte((byte)'B');
                destination.WriteByte((byte)'I');
                destination.WriteByte((byte)'X');

                PTStream.WriteUInt32(destination, 8);
                PTStream.WriteUInt32(destination, globalIndex);
                PTStream.WriteUInt32(destination, 0);
            }

            // Write out the PVRT header
            destination.WriteByte((byte)'P');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'R');
            destination.WriteByte((byte)'T');

            PTStream.WriteInt32(destination, textureLength - 24);

            destination.WriteByte((byte)pixelFormat);
            destination.WriteByte((byte)dataFormat);
            PTStream.WriteUInt16(destination, 0);

            PTStream.WriteUInt16(destination, textureWidth);
            PTStream.WriteUInt16(destination, textureHeight);

            // If we have an internal palette, write it
            if (dataCodec.PaletteEntries != 0 && !dataCodec.NeedsExternalPalette)
            {
                byte[] palette = pixelCodec.EncodePalette(texturePalette, dataCodec.PaletteEntries);
                destination.Write(palette, 0, palette.Length);
            }

            // Write the texture data
            byte[] textureData = dataCodec.Encode(decodedData, textureWidth, textureHeight, null);
            destination.Write(textureData, 0, textureData.Length);

            // Compress the texture
            if (compressionFormat != PvrCompressionFormat.None)
            {
                compressionCodec = PvrCompressionCodec.GetCompressionCodec(compressionFormat);

                if (compressionCodec != null)
                {
                    // Ok, we need to convert the current stream to an array, compress it, then write it back to a new stream
                    byte[] buffer = destination.ToArray();
                    buffer = compressionCodec.Compress(buffer, (includeGbixHeader ? 0x10 : 0), pixelCodec, dataCodec);

                    destination = new MemoryStream();
                    destination.Write(buffer, 0, buffer.Length);
                }
            }

            return destination;
        }
        #endregion
    }
}