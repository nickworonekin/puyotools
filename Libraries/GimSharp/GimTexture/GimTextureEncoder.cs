using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using nQuant;

namespace GimSharp
{
    public class GimTextureEncoder
    {
        #region Fields
        private bool initalized = false; // Is the texture initalized?

        private byte[] decodedData; // Decoded texture data (either 32-bit RGBA or 8-bit indexed)

        private GimPixelCodec pixelCodec; // Pixel codec
        private GimDataCodec dataCodec;   // Data codec

        private byte[][] texturePalette; // The texture's palette
        #endregion

        #region Texture Properties
        /// <summary>
        /// Width of the texture (in pixels).
        /// </summary>
        public ushort TextureWidth
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureWidth;
            }
        }
        private ushort textureWidth;

        /// <summary>
        /// Height of the texture (in pixels).
        /// </summary>
        public ushort TextureHeight
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureHeight;
            }
        }
        private ushort textureHeight;

        /// <summary>
        /// The texture's palette format.
        /// </summary>
        public GimPaletteFormat PaletteFormat
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return paletteFormat;
            }
        }
        private GimPaletteFormat paletteFormat;

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public GimDataFormat DataFormat
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
        private GimDataFormat dataFormat;
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(string file, GimPaletteFormat paletteFormat, GimDataFormat dataFormat)
        {
            initalized = Initalize(new Bitmap(file), paletteFormat, dataFormat);
        }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(byte[] source, GimPaletteFormat paletteFormat, GimDataFormat dataFormat) : this(source, 0, source.Length, paletteFormat, dataFormat) { }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(byte[] source, int offset, int length, GimPaletteFormat paletteFormat, GimDataFormat dataFormat)
        {
            MemoryStream buffer = new MemoryStream();
            buffer.Write(source, offset, length);

            initalized = Initalize(new Bitmap(buffer), paletteFormat, dataFormat);
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(Stream source, GimPaletteFormat paletteFormat, GimDataFormat dataFormat) : this(source, (int)(source.Length - source.Position), paletteFormat, dataFormat) { }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(Stream source, int length, GimPaletteFormat paletteFormat, GimDataFormat dataFormat)
        {
            MemoryStream buffer = new MemoryStream();
            PTStream.CopyPartTo(source, buffer, length);

            initalized = Initalize(new Bitmap(buffer), paletteFormat, dataFormat);
        }

        /// <summary>
        /// Opens a texture to encode from a bitmap.
        /// </summary>
        /// <param name="source">Bitmap to encode.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(Bitmap source, GimPaletteFormat paletteFormat, GimDataFormat dataFormat)
        {
            initalized = Initalize(source, paletteFormat, dataFormat);
        }

        private bool Initalize(Bitmap source, GimPaletteFormat paletteFormat, GimDataFormat dataFormat)
        {
            // Make sure this bitmap's dimensions are valid
            if (!HasValidDimensions(source.Width, source.Height))
                return false;

            textureWidth = (ushort)source.Width;
            textureHeight = (ushort)source.Height;
            
            // Set default values
            hasMetadata = true;
            metadata = new GimMetadata();

            // Set the data format and palette format and load the appropiate codecs
            this.dataFormat = dataFormat;
            dataCodec = GimDataCodec.GetDataCodec(dataFormat);

            // Make sure the data codec exists and we can encode to it
            if (dataCodec == null || !dataCodec.CanEncode) return false;

            // Only palettized formats require a pixel codec.
            if (dataCodec.PaletteEntries != 0)
            {
                this.paletteFormat = paletteFormat;
                pixelCodec = GimPixelCodec.GetPixelCodec(paletteFormat);

                // Make sure the pixel codec exists and we can encode to it
                if (pixelCodec == null || !pixelCodec.CanEncode) return false;

                dataCodec.PixelCodec = pixelCodec;

                // Convert the bitmap to an array containing indicies.
                decodedData = BitmapToRawIndexed(source, dataCodec.PaletteEntries, out texturePalette);
            }
            else
            {
                this.paletteFormat = GimPaletteFormat.Unknown;
                pixelCodec = null;

                // Convert the bitmap to an array
                decodedData = BitmapToRaw(source);
            }

            return true;
        }

        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool Initalized
        {
            get { return initalized; }
        }

        // Returns if the texture dimensuons are valid
        private bool HasValidDimensions(int width, int height)
        {
            if (width % 16 != 0 || height % 8 != 0)
                return false;

            return true;
        }
        #endregion

        #region Metadata
        /// <summary>
        /// Gets or sets if the texture should include metadata.
        /// </summary>
        public bool HasMetadata
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return hasMetadata;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                hasMetadata = value;
            }
        }
        private bool hasMetadata;

        /// <summary>
        /// Returns the metadata object for the texture.
        /// </summary>
        public GimMetadata Metadata
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return metadata;
            }
        }
        private GimMetadata metadata;

        public class GimMetadata
        {
            /// <summary>
            /// Gets or sets the original filename of the texture specified in the metadata. The default value is "texture.png".
            /// </summary>
            public string OriginalFilename = "texture.png";

            /// <summary>
            /// Gets or sets the user that created this texture as specified in the metadata. The default value is "user".
            /// </summary>
            public string User = "user";

            /// <summary>
            /// Gets or sets the timestamp in the metadata. The timestamp is in the format of "ddd MMM d HH:mm:ss yyyy".
            /// </summary>
            public string Timestamp = DateTime.Now.ToString("ddd MMM d HH:mm:ss yyyy");

            /// <summary>
            /// Gets or sets the program used to create this texture as specified in the metadata. The default value is "GimConv 1.40".
            /// </summary>
            public string Program = "GimConv 1.40";
        }
        #endregion

        #region Texture Retrieval
        /// <summary>
        /// Returns the encoded texture as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            return EncodeTexture().ToArray();
        }

        /// <summary>
        /// Returns the encoded texture as a stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            return EncodeTexture();
        }

        /// <summary>
        /// Saves the encoded texture to the specified path.
        /// </summary>
        /// <param name="path">Name of the file to save the data to.</param>
        public void Save(string path)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            using (FileStream destination = File.Create(path))
            {
                MemoryStream textureStream = EncodeTexture();
                textureStream.Position = 0;
                PTStream.CopyTo(textureStream, destination);
            }
        }

        /// <summary>
        /// Saves the encoded texture to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            MemoryStream textureStream = EncodeTexture();
            textureStream.Position = 0;
            PTStream.CopyTo(textureStream, destination);
        }

        // Encodes a texture
        private MemoryStream EncodeTexture()
        {
            // Get the lengths of the various chunks
            int eofOffsetChunkLength = 16,
                metadataOffsetChunkLength = 16,
                paletteDataChunkLength = 0,
                textureDataChunkLength = 0,
                metadataChunkLength = 0;

            if (dataCodec.PaletteEntries != 0)
            {
                paletteDataChunkLength = 80 + (dataCodec.PaletteEntries * (pixelCodec.Bpp >> 3));
            }

            textureDataChunkLength = 80 + ((textureWidth * textureHeight * dataCodec.Bpp) >> 3);

            if (hasMetadata)
            {
                metadataChunkLength = 16;

                if (metadata.OriginalFilename != null)
                {
                    metadataChunkLength += metadata.OriginalFilename.Length;
                }
                metadataChunkLength++;

                if (metadata.User != null)
                {
                    metadataChunkLength += metadata.User.Length;
                }
                metadataChunkLength++;

                if (metadata.Timestamp != null)
                {
                    metadataChunkLength += metadata.Timestamp.Length;
                }
                metadataChunkLength++;

                if (metadata.Program != null)
                {
                    metadataChunkLength += metadata.Program.Length;
                }
                metadataChunkLength++;
            }

            // Calculate what the length of the texture will be
            int textureLength = 16 +
                eofOffsetChunkLength +
                metadataOffsetChunkLength +
                paletteDataChunkLength +
                textureDataChunkLength +
                metadataChunkLength;

            MemoryStream destination = new MemoryStream(textureLength);

            // Write the GIM header
            PTStream.WriteCString(destination, "MIG.00.1PSP", 12);
            PTStream.WriteUInt32(destination, 0);

            // Write the EOF offset chunk
            PTStream.WriteUInt16(destination, 0x02);
            PTStream.WriteUInt16(destination, 0);
            PTStream.WriteInt32(destination, textureLength - 16);
            PTStream.WriteInt32(destination, eofOffsetChunkLength);
            PTStream.WriteUInt32(destination, 16);

            // Write the metadata offset chunk
            PTStream.WriteUInt16(destination, 0x03);
            PTStream.WriteUInt16(destination, 0);

            if (hasMetadata)
            {
                PTStream.WriteInt32(destination, textureLength - metadataChunkLength - 32);
            }
            else
            {
                PTStream.WriteInt32(destination, textureLength - 32);
            }

            PTStream.WriteInt32(destination, metadataOffsetChunkLength);
            PTStream.WriteUInt32(destination, 16);

            // Write the palette data, if we have a palette
            if (dataCodec.PaletteEntries != 0)
            {
                PTStream.WriteUInt16(destination, 0x05);
                PTStream.WriteUInt16(destination, 0);
                PTStream.WriteInt32(destination, paletteDataChunkLength);
                PTStream.WriteInt32(destination, paletteDataChunkLength);
                PTStream.WriteUInt32(destination, 16);

                PTStream.WriteUInt16(destination, 48);
                PTStream.WriteUInt16(destination, 0);
                PTStream.WriteUInt16(destination, (byte)paletteFormat);
                PTStream.WriteUInt16(destination, 0);
                PTStream.WriteUInt16(destination, (ushort)dataCodec.PaletteEntries);
                destination.Write(new byte[] { 0x01, 0x00, 0x20, 0x00, 0x10, 0x00 }, 0, 6);

                destination.Write(new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 8);
                PTStream.WriteUInt32(destination, 0x30);
                PTStream.WriteUInt32(destination, 0x40);

                PTStream.WriteInt32(destination, paletteDataChunkLength - 16);
                PTStream.WriteUInt32(destination, 0);
                destination.Write(new byte[] { 0x02, 0x00, 0x01, 0x00, 0x03, 0x00, 0x01, 0x00 }, 0, 8);

                destination.Write(new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 16);

                byte[] palette = pixelCodec.EncodePalette(texturePalette, dataCodec.PaletteEntries);
                destination.Write(palette, 0, palette.Length);
            }

            // Write the texture data
            PTStream.WriteUInt16(destination, 0x04);
            PTStream.WriteUInt16(destination, 0);
            PTStream.WriteInt32(destination, textureDataChunkLength);
            PTStream.WriteInt32(destination, textureDataChunkLength);
            PTStream.WriteUInt32(destination, 16);

            PTStream.WriteUInt16(destination, 48);
            PTStream.WriteUInt16(destination, 0);
            PTStream.WriteUInt16(destination, (byte)dataFormat);
            PTStream.WriteUInt16(destination, 1); // Always swizzled
            PTStream.WriteUInt16(destination, textureWidth);
            PTStream.WriteUInt16(destination, textureHeight);

            if (dataCodec.PaletteEntries != 0)
            {
                // For palettized textures, this is the bpp for this data format
                PTStream.WriteUInt16(destination, (ushort)dataCodec.Bpp);
            }
            else
            {
                // For non-palettized textures, this is always specified as 32bpp
                PTStream.WriteUInt16(destination, 32);
            }

            PTStream.WriteUInt16(destination, 16);

            destination.Write(new byte[] { 0x08, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00 }, 0, 16);

            PTStream.WriteInt32(destination, textureDataChunkLength - 16);
            PTStream.WriteUInt32(destination, 0);
            destination.Write(new byte[] { 0x01, 0x00, 0x01, 0x00, 0x03, 0x00, 0x01, 0x00 }, 0, 8);

            destination.Write(new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 16);

            byte[] textureData = GimDataCodec.Swizzle(dataCodec.Encode(decodedData, 0, textureWidth, textureHeight), 0, textureWidth, textureHeight, dataCodec.Bpp);
            destination.Write(textureData, 0, textureData.Length);

            // Write the metadata, only if we are including it
            if (hasMetadata)
            {
                PTStream.WriteUInt16(destination, 0xFF);
                PTStream.WriteUInt16(destination, 0);
                PTStream.WriteInt32(destination, metadataChunkLength);
                PTStream.WriteInt32(destination, metadataChunkLength);
                PTStream.WriteUInt32(destination, 16);

                PTStream.WriteCString(destination, metadata.OriginalFilename);
                PTStream.WriteCString(destination, metadata.User);
                PTStream.WriteCString(destination, metadata.Timestamp);
                PTStream.WriteCString(destination, metadata.Program);
            }
            return destination;
        }
        #endregion

        #region Texture Conversion
        private byte[] BitmapToRaw(Bitmap source)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height * 4];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                img = newImage;
            }

            // Copy over the data to the destination. It's ok to do it without utilizing Stride
            // since each pixel takes up 4 bytes (aka Stride will always be equal to Width)
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            Marshal.Copy(bitmapData.Scan0, destination, 0, destination.Length);
            img.UnlockBits(bitmapData);

            return destination;
        }

        private unsafe byte[] BitmapToRawIndexed(Bitmap source, int maxColors, out byte[][] palette)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                img = newImage;
            }

            // Quantize the image
            WuQuantizer quantizer = new WuQuantizer();
            img = (Bitmap)quantizer.QuantizeImage(img, maxColors);

            // Copy over the data to the destination. We need to use Stride in this case, as it may not
            // always be equal to Width.
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

            byte* pointer = (byte*)bitmapData.Scan0;
            for (int y = 0; y < bitmapData.Height; y++)
            {
                for (int x = 0; x < bitmapData.Width; x++)
                {
                    destination[(y * img.Width) + x] = pointer[(y * bitmapData.Stride) + x];
                }
            }

            img.UnlockBits(bitmapData);

            // Copy over the palette
            palette = new byte[maxColors][];
            for (int i = 0; i < maxColors; i++)
            {
                palette[i] = new byte[4];

                palette[i][3] = img.Palette.Entries[i].A;
                palette[i][2] = img.Palette.Entries[i].R;
                palette[i][1] = img.Palette.Entries[i].G;
                palette[i][0] = img.Palette.Entries[i].B;
            }

            return destination;
        }
        #endregion
    }
}