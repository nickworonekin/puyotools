using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PuyoTools.Core.Textures.Gim
{
    public class GimTextureDecoder
    {
        #region Fields
        private GimPixelCodec paletteCodec; // Palette codec
        private GimDataCodec pixelCodec;    // Pixel codec

        private ushort paletteEntries; // Number of entries in the palette

        private bool isSwizzled; // Is the texture data swizzled?

        private static readonly byte[] magicCode =
        {
            (byte)'M', (byte)'I', (byte)'G', (byte)'.',
            (byte)'0', (byte)'0', (byte)'.',
            (byte)'1', (byte)'P', (byte)'S', (byte)'P'
        };

        private byte[] paletteData;
        private byte[] textureData;

        private byte[] decodedData;

        private int actualWidth;
        private int actualHeight;
        #endregion

        #region Texture Properties
        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the palette format, or null if a palette is not used.
        /// </summary>
        public GimPaletteFormat? PaletteFormat { get; private set; }

        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public GimDataFormat PixelFormat { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a GIM texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public GimTextureDecoder(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Open a GIM texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public GimTextureDecoder(Stream source)
        {
            Initialize(source);
        }

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a GIM texture
            if (!Is(source))
            {
                throw new NotAValidTextureException("This is not a valid GIM texture.");
            }

            var startPosition = source.Position;
            var reader = new BinaryReader(source);

            paletteEntries = 0;

            // A GIM is constructed of different chunks. They do not necessarily have to be in order.
            int eofOffset = -1;

            source.Position += 0x10;
            while (source.Position < source.Length)
            {
                var chunkPosition = source.Position;
                int chunkLength;

                var chunkType = reader.ReadUInt16();
                source.Position += 2; // 0x04

                switch (chunkType)
                {
                    case 0x02: // EOF offset chunk

                        eofOffset = reader.ReadInt32() + 16;

                        // Get the length of this chunk
                        chunkLength = reader.ReadInt32();

                        break;

                    case 0x03: // Metadata offset chunk

                        // Skip this chunk. It's not necessary for decoding this texture.
                        source.Position += 4; // 0x08
                        chunkLength = reader.ReadInt32();

                        break;

                    case 0x04: // Texture data chunk

                        // Get the length of this chunk
                        source.Position += 4; // 0x08
                        chunkLength = reader.ReadInt32();

                        // Get the pixel format & codec
                        source.Position += 8; // 0x20
                        PixelFormat = (GimDataFormat)reader.ReadUInt16();
                        pixelCodec = GimDataCodec.GetDataCodec(PixelFormat);

                        if (pixelCodec is null)
                        {
                            throw new CannotDecodeTextureException($"Pixel format {PixelFormat:X2} is invalid or not supported for decoding.");
                        }

                        // Get whether this texture is swizzled
                        isSwizzled = reader.ReadUInt16() == 1;

                        // Get the texture dimensions
                        Width = actualWidth = reader.ReadUInt16();
                        Height = actualHeight = reader.ReadUInt16();

                        // Some textures do not have a width that is a multiple or 16 or a height that is a multiple of 8.
                        // We'll just do it the lazy way and set their width/height to a multiple of 16/8.
                        if (actualWidth % 16 != 0)
                        {
                            actualWidth = PTMethods.RoundUp(actualWidth, 16);
                        }
                        if (actualHeight % 8 != 0)
                        {
                            actualHeight = PTMethods.RoundUp(actualHeight, 8);
                        }

                        // Read the texture data
                        textureData = reader.At(chunkPosition + 0x50, x => x.ReadBytes(actualWidth * actualHeight * pixelCodec.Bpp / 8));

                        break;

                    case 0x05: // Palette data chunk

                        // Get the length of this chunk
                        source.Position += 4; // 0x08
                        chunkLength = reader.ReadInt32();

                        // Get the palette format & codec
                        source.Position += 8; // 0x20
                        PaletteFormat = (GimPaletteFormat)reader.ReadUInt16();
                        paletteCodec = GimPixelCodec.GetPixelCodec(PaletteFormat.Value);

                        if (paletteCodec is null)
                        {
                            throw new CannotDecodeTextureException($"Palette format {PaletteFormat:X2} is invalid or not supported for decoding.");
                        }

                        // Get the number of entries in the palette
                        source.Position += 2; // 0x24
                        paletteEntries = reader.ReadUInt16();

                        // Read the palette data
                        paletteData = reader.At(chunkPosition + 0x50, x => x.ReadBytes(paletteEntries * paletteCodec.Bpp / 8));

                        break;

                    case 0xFF: // Metadata chunk

                        // Get the length of this chunk
                        source.Position += 4; // 0x08
                        chunkLength = reader.ReadInt32();

                        // Read the metadata
                        source.Position += 4; // 0x10

                        Metadata = new GimMetadata
                        {
                            OriginalFilename = reader.ReadNullTerminatedString(),
                            User = reader.ReadNullTerminatedString(),
                            Timestamp = reader.ReadNullTerminatedString(),
                            Program = reader.ReadNullTerminatedString()
                        };

                        break;

                    default: // Unknown chunk

                        throw new Exception($"Unknown chunk type {chunkType:X2}");
                }

                // Verify that the chunk length will allow the stream to progress
                if (chunkLength <= 0)
                {
                    throw new Exception("Chunk length cannot be zero or negative.");
                }

                // Go to the next chunk
                source.Position = chunkPosition + chunkLength;
            }

            // Verify that the stream's position is as the expected position
            if (source.Position - startPosition != eofOffset)
            {
                throw new Exception("Stream position does not match expected end-of-file position.");
            }

            // Verify that a data codec exists
            if (pixelCodec is null)
            {
                throw new CannotDecodeTextureException("No texture data found.");
            }

            if (pixelCodec.PaletteEntries != 0)
            {
                // Verify that a palette exists for palettized formats
                if (paletteCodec is null)
                {
                    throw new CannotDecodeTextureException("No palette found for palette-based pixel format.");
                }

                // Verify that there aren't too many entries in the palette
                if (paletteEntries > pixelCodec.PaletteEntries)
                {
                    throw new CannotDecodeTextureException("Too many entries in palette for the specified pixel format.");
                }

                // Set the data format's palette codec
                pixelCodec.PixelCodec = paletteCodec;
            }
        }
        #endregion

        #region Metadata
        /// <summary>
        /// Gets the metadata, or null if there is no metadata.
        /// </summary>
        public GimMetadata Metadata { get; private set; }
        #endregion

        #region Texture Retrieval
        /// <summary>
        /// Saves the decoded texture to the specified file as a PNG.
        /// </summary>
        /// <param name="file">Name of the file to save the data to.</param>
        public void Save(string file)
        {
            using (var stream = File.OpenWrite(file))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Saves the decoded texture to the specified stream as a PNG.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            var pixelData = GetPixelData();

            Bitmap img = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, img.PixelFormat);
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            img.UnlockBits(bitmapData);

            img.Save(destination, ImageFormat.Png);
        }

        // Decodes a texture
        private byte[] DecodeTexture()
        {
            if (paletteData != null) // The texture contains an embedded palette
            {
                pixelCodec.SetPalette(paletteData, 0, paletteEntries);
            }

            if (isSwizzled)
            {
                return pixelCodec.Decode(GimDataCodec.UnSwizzle(textureData, 0, actualWidth, actualHeight, pixelCodec.Bpp), 0, actualWidth, actualHeight);
            }

            return pixelCodec.Decode(textureData, 0, actualWidth, actualHeight);
        }

        /// <summary>
        /// Decodes the texture and returns the pixel data.
        /// </summary>
        /// <returns>The pixel data as a byte array.</returns>
        public byte[] GetPixelData()
        {
            if (decodedData == null)
            {
                decodedData = DecodeTexture();
            }

            return decodedData;
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Determines if this is a GIM texture.
        /// </summary>
        /// <param name="source">The stream to read.</param>
        /// <returns>True if this is a GIM texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return remainingLength > 24
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)
                    && reader.At(startPosition + 0x14, x => x.ReadUInt32()) == remainingLength - 16;
            }
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