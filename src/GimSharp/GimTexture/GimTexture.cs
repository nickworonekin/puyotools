using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GimSharp
{
    public class GimTexture
    {
        #region Fields
        private bool initalized = false; // Is the texture initalized?
        private bool canDecode = false; // Can we decode this texture?

        private byte[] encodedData; // Encoded texture data (GIM data)

        private GimPixelCodec pixelCodec; // Pixel codec
        private GimDataCodec dataCodec;   // Data codec

        private int paletteOffset; // Offset of the palette data in the texture (-1 if there is none)
        private int dataOffset;    // Offset of the actual data in the texture

        private ushort paletteEntries; // Number of entries in the palette

        private bool swizzled; // Is the texture data swizzled?
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
        /// Open a GIM texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public GimTexture(string file)
        {
            encodedData = File.ReadAllBytes(file);

            if (encodedData != null)
            {
                Initalize();
            }
        }

        /// <summary>
        /// Open a GIM texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        public GimTexture(byte[] source) : this(source, 0, source.Length) { }

        /// <summary>
        /// Open a GIM texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GimTexture(byte[] source, int offset, int length)
        {
            if (source == null || (offset == 0 && source.Length == length))
            {
                encodedData = source;
            }
            else if (source != null)
            {
                encodedData = new byte[length];
                Array.Copy(source, offset, encodedData, 0, length);
            }

            if (encodedData != null)
            {
                Initalize();
            }
        }

        /// <summary>
        /// Open a GIM texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public GimTexture(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        /// <summary>
        /// Open a GIM texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GimTexture(Stream source, int length)
        {
            encodedData = new byte[length];
            source.Read(encodedData, 0, length);

            if (encodedData != null)
            {
                Initalize();
            }
        }

        private void Initalize()
        {
            // Check to see if what we are dealing with is a GIM texture
            if (!Is(encodedData))
            {
                throw new NotAValidTextureException("This is not a valid GIM texture.");
            }

            // Initalize some things
            paletteFormat = GimPaletteFormat.Unknown;
            dataFormat = GimDataFormat.Unknown;

            paletteEntries = 0;
            paletteOffset = -1;
            dataOffset = -1;

            hasMetadata = false;

            // A GIM is constructed of different chunks. They do not necessarily have to be in order.
            int eofOffset = -1;

            int offset = 0x10;
            int previousOffset = offset;
            while (offset < encodedData.Length)
            {
                // EOF offset chunk
                if (encodedData[offset] == 0x02)
                {
                    eofOffset = BitConverter.ToInt32(encodedData, offset + 0x04) + 16;

                    // Go to the next chunk
                    offset += BitConverter.ToInt32(encodedData, offset + 0x08);
                }

                // Metadata offset chunk
                else if (encodedData[offset] == 0x03)
                {
                    // Skip this chunk. It's not necessary for decoding this texture.
                    offset += BitConverter.ToInt32(encodedData, offset + 0x08);
                }

                // Texture data
                else if (encodedData[offset] == 0x04)
                {
                    // Get the data format
                    dataFormat = (GimDataFormat)encodedData[offset + 0x14];

                    // Get the data codec and make sure we can decode using it
                    dataCodec = GimDataCodec.GetDataCodec(dataFormat);

                    // Check to see if the texture data is swizzled
                    swizzled = (BitConverter.ToUInt16(encodedData, offset + 0x16) == 1);

                    // Get the texture's width and height
                    textureWidth  = BitConverter.ToUInt16(encodedData, offset + 0x18);
                    textureHeight = BitConverter.ToUInt16(encodedData, offset + 0x1A);

                    // Some textures do not have a width that is a multiple or 16 or a height that is a multiple of 8.
                    // We'll just do it the lazy way and set their width/height to a multiple of 16/8.
                    if (textureWidth % 16 != 0)
                    {
                        textureWidth = (ushort)PTMethods.RoundUp(textureWidth, 16);
                    }
                    if (textureHeight % 8 != 0)
                    {
                        textureHeight = (ushort)PTMethods.RoundUp(textureHeight, 8);
                    }

                    // Set the offset of the texture data
                    dataOffset = offset + 0x50;

                    // Go to the next chunk
                    offset += BitConverter.ToInt32(encodedData, offset + 0x08);
                }

                // Palette data
                else if (encodedData[offset] == 0x05)
                {
                    // Get the palette format
                    paletteFormat = (GimPaletteFormat)encodedData[offset + 0x14];

                    // Get the pixel codec and make sure we can decode using it
                    pixelCodec = GimPixelCodec.GetPixelCodec(paletteFormat);

                    // Get the number of entries in the palette
                    paletteEntries = BitConverter.ToUInt16(encodedData, offset + 0x18);

                    // Set the offset of the palette data
                    paletteOffset = offset + 0x50;

                    // Go to the next chunk
                    offset += BitConverter.ToInt32(encodedData, offset + 0x08);
                }

                // Metadata chunk
                else if (encodedData[offset] == 0xFF)
                {
                    // Read in some metadata
                    hasMetadata = true;
                    metadata = new GimMetadata();

                    int metadataOffset = 0x10;

                    metadata.OriginalFilename = PTMethods.StringFromBytes(encodedData, offset + metadataOffset);
                    metadataOffset += metadata.OriginalFilename.Length + 1;

                    metadata.User = PTMethods.StringFromBytes(encodedData, offset + metadataOffset);
                    metadataOffset += metadata.User.Length + 1;

                    metadata.Timestamp = PTMethods.StringFromBytes(encodedData, offset + metadataOffset);
                    metadataOffset += metadata.Timestamp.Length + 1;

                    metadata.Program = PTMethods.StringFromBytes(encodedData, offset + metadataOffset);
                    metadataOffset += metadata.Program.Length + 1;

                    // Go to the next chunk
                    offset += BitConverter.ToInt32(encodedData, offset + 0x08);
                }

                // Invalid chunk
                else
                {
                    return;
                }

                // Make sure we are actually advancing in the file, so we don't end up reaching a negative offset
                // or ending up in an infinite loop 
                if (offset <= previousOffset)
                {
                    return;
                }
                previousOffset = offset;
            }

            // If all went well, offset should be equal to eofOffset
            if (offset != eofOffset)
            {
                return;
            }

            // If this is a non-palettized format, make sure we have a data codec
            if (dataCodec != null && dataCodec.PaletteEntries == 0)
            {
                canDecode = true;
            }

            // If this is a palettized format, make sure we have a palette codec, a data codec,
            // and that the number of palette entires is less than or equal to what this data format supports
            if (dataCodec != null && dataCodec.PaletteEntries != 0 && pixelCodec != null && paletteEntries <= dataCodec.PaletteEntries)
            {
                // Set the data format's pixel codec
                dataCodec.PixelCodec = pixelCodec;

                canDecode = true;
            }

            initalized = true;
        }

        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool Initalized
        {
            get { return initalized; }
        }

        /// <summary>
        /// Returns if the texture can be decoded.
        /// </summary>
        public bool CanDecode
        {
            get { return canDecode; }
        }
        #endregion

        #region Metadata
        /// <summary>
        /// Returns if the texture has metadata.
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
        }
        private bool hasMetadata;

        /// <summary>
        /// Returns the metadata of the texture. If the texture has no metadata, this will return null.
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
            /// Returns the original filename of the texture specified in the metadata.
            /// </summary>
            public string OriginalFilename { get; internal set; }

            /// <summary>
            /// Returns the user that created this texture as specified in the metadata.
            /// </summary>
            public string User { get; internal set; }

            /// <summary>
            /// Returns the timestamp in the metadata. The timestamp is in the format of "ddd MMM d HH:mm:ss yyyy".
            /// </summary>
            public string Timestamp { get; internal set; }

            /// <summary>
            /// Returns the program used to create this texture as specified in the metadata.
            /// </summary>
            public string Program { get; internal set; }
        }
        #endregion

        #region Texture Retrieval
        /// <summary>
        /// Returns the decoded texture as an array containg raw 32-bit ARGB data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            return DecodeTexture();
        }

        /// <summary>
        /// Returns the decoded texture as a bitmap.
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            byte[] data = DecodeTexture();

            Bitmap img = new Bitmap(textureWidth, textureHeight, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, img.PixelFormat);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            img.UnlockBits(bitmapData);

            return img;
        }

        /// <summary>
        /// Returns the decoded texture as a stream containg a PNG.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            MemoryStream destination = new MemoryStream();
            ToBitmap().Save(destination, ImageFormat.Png);
            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Saves the decoded texture to the specified file.
        /// </summary>
        /// <param name="file">Name of the file to save the data to.</param>
        public void Save(string file)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            ToBitmap().Save(file, ImageFormat.Png);
        }

        /// <summary>
        /// Saves the decoded texture to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            ToBitmap().Save(destination, ImageFormat.Png);
        }

        // Decodes a texture
        private byte[] DecodeTexture()
        {
            // Make sure we can decode this texture
            if (!canDecode)
            {
                throw new CannotDecodeTextureException("Cannot decode texture. The palette format and/or data format may not be supported.");
            }

            if (paletteOffset != -1) // The texture contains an embedded palette
            {
                dataCodec.SetPalette(encodedData, paletteOffset, paletteEntries);
            }

            if (swizzled)
            {
                return dataCodec.Decode(GimDataCodec.UnSwizzle(encodedData, dataOffset, textureWidth, textureHeight, dataCodec.Bpp), 0, textureWidth, textureHeight);
            }

            return dataCodec.Decode(encodedData, dataOffset, textureWidth, textureHeight);
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Determines if this is a GIM texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">The offset in the byte array to start at.</param>
        /// <param name="length">Length of the data (in bytes).</param>
        /// <returns>True if this is a GIM texture, false otherwise.</returns>
        public static bool Is(byte[] source, int offset, int length)
        {
            if (length > 24 && 
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("MIG.00.1PSP")) &&
                BitConverter.ToUInt32(source, offset + 0x14) == length - 16)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if this is a GIM texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>True if this is a GIM texture, false otherwise.</returns>
        public static bool Is(byte[] source)
        {
            return Is(source, 0, source.Length);
        }

        /// <summary>
        /// Determines if this is a GIM texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>True if this is a GIM texture, false otherwise.</returns>
        public static bool Is(Stream source, int length)
        {
            // If the length is < 24, then there is no way this is a valid texture.
            if (length < 24)
            {
                return false;
            }

            byte[] buffer = new byte[24];
            source.Read(buffer, 0, 24);
            source.Position -= 24;

            return Is(buffer, 0, length);
        }

        /// <summary>
        /// Determines if this is a GIM texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a GIM texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            return Is(source, (int)(source.Length - source.Position));
        }

        /// <summary>
        /// Determines if this is a GIM texture.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a GIM texture, false otherwise.</returns>
        public static bool Is(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                return Is(stream);
            }
        }
        #endregion
    }
}