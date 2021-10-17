using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PuyoTools.Core.Textures.Svr
{
    public class SvrTextureDecoder
    {
        #region Fields
        private SvrPixelCodec pixelCodec; // Pixel codec
        private SvrDataCodec dataCodec;   // Data codec

        protected int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] gbixMagicCode = { (byte)'G', (byte)'B', (byte)'I', (byte)'X' };
        private static readonly byte[] pvrtMagicCode = { (byte)'P', (byte)'V', (byte)'R', (byte)'T' };

        private byte[] paletteData;
        private byte[] textureData;

        private byte[] decodedPaletteData;
        private byte[] decodedTextureData;
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
        /// Gets the pixel format.
        /// </summary>
        public SvrPixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// Gets the data format.
        /// </summary>
        public SvrDataFormat DataFormat { get; private set; }

        /// <summary>
        /// Gets the global index, or <see langword="null"/> if no global index exists.
        /// </summary>
        public uint? GlobalIndex { get; private set; }

        /// <summary>
        /// Gets the position of the PVRT chunk.
        /// </summary>
        internal long PvrtPosition { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a SVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public SvrTextureDecoder(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Open a SVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public SvrTextureDecoder(Stream source)
        {
            Initialize(source);
        }

        /*protected override void Initalize()
        {
            // Check to see if what we are dealing with is a SVR texture
            if (!Is(encodedData))
            {
                throw new NotAValidTextureException("This is not a valid GVR texture.");
            }

            // Determine the offsets of the GBIX (if present) and PVRT header chunks.
            if (encodedData.Take(4).SequenceEqual(Encoding.UTF8.GetBytes("GBIX")))
            {
                gbixOffset = 0x00;
                pvrtOffset = 0x10;
            }
            else
            {
                gbixOffset = -1;
                pvrtOffset = 0x00;
            }

            // Read the global index (if it is present). If it is not present, just set it to 0.
            if (gbixOffset != -1)
            {
                globalIndex = BitConverter.ToUInt32(encodedData, gbixOffset + 0x08);
            }
            else
            {
                globalIndex = 0;
            }

            // Read information about the texture
            textureWidth  = BitConverter.ToUInt16(encodedData, pvrtOffset + 0x0C);
            textureHeight = BitConverter.ToUInt16(encodedData, pvrtOffset + 0x0E);

            pixelFormat = (SvrPixelFormat)encodedData[pvrtOffset + 0x08];
            dataFormat  = (SvrDataFormat)encodedData[pvrtOffset + 0x09];

            // Get the codecs and make sure we can decode using them
            pixelCodec = SvrPixelCodec.GetPixelCodec(pixelFormat);
            dataCodec = SvrDataCodec.GetDataCodec(dataFormat);

            if (dataCodec != null && pixelCodec != null)
            {
                dataCodec.PixelCodec = pixelCodec;
                canDecode = true;
            }

            // Set the palette and data offsets
            paletteEntries = dataCodec.PaletteEntries;
            if (!canDecode || paletteEntries == 0 || dataCodec.NeedsExternalPalette)
            {
                paletteOffset = -1;
                dataOffset = pvrtOffset + 0x10;
            }
            else
            {
                paletteOffset = pvrtOffset + 0x10;
                dataOffset = paletteOffset + (paletteEntries * (pixelCodec.Bpp >> 3));
            }

            initalized = true;
        }*/

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a SVR texture
            if (!Is(source))
            {
                throw new NotAValidTextureException("This is not a valid SVR texture.");
            }

            var startPosition = source.Position;
            var reader = new BinaryReader(source);

            // Read the GBIX header (if present).
            if (reader.At(startPosition, x => x.ReadBytes(gbixMagicCode.Length).SequenceEqual(gbixMagicCode)))
            {
                source.Position += 4; // 0x04

                // Get the length of the GBIX header
                var gbixLength = reader.ReadInt32() + 8;

                // Read the global index
                GlobalIndex = reader.ReadUInt32();

                source.Position = startPosition + gbixLength;
            }

            PvrtPosition = source.Position;

            source.Position += 8; // 0x08

            // Get the pixel & data formats
            PixelFormat = (SvrPixelFormat)reader.ReadByte();
            DataFormat = (SvrDataFormat)reader.ReadByte();

            // Get the texture dimensions
            source.Position += 2; // 0x0C
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();

            // Get the codecs and make sure we can decode using them
            pixelCodec = SvrPixelCodec.GetPixelCodec(PixelFormat);
            dataCodec = SvrDataCodec.GetDataCodec(DataFormat);

            // If we don't have a known pixel or data codec for these formats, that's ok.
            // This will allow the properties to be read if the user doesn't want to decode this texture.
            // The exception will be thrown when the texture is being decoded.
            if (pixelCodec is null || dataCodec is null)
            {
                return;
            }

            dataCodec.PixelCodec = pixelCodec;

            // Get the number of palette entries.
            paletteEntries = dataCodec.PaletteEntries;

            // Read the palette data (if present).
            if (dataCodec.PaletteEntries != 0 && !dataCodec.NeedsExternalPalette)
            {
                paletteData = reader.ReadBytes(paletteEntries * pixelCodec.Bpp / 8);
            }

            // Read the texture data
            textureData = reader.ReadBytes(Width * Height * dataCodec.Bpp / 8);
        }
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
            return DecodeTexture(textureData, Width, Height);
        }

        internal byte[] DecodeTexture(byte[] textureData, int width, int height)
        {
            // Verify that a pixel and data codec have been set.
            if (pixelCodec is null)
            {
                throw new CannotDecodeTextureException($"Pixel format {PixelFormat:X2} is invalid or not supported for decoding.");
            }
            if (dataCodec is null)
            {
                throw new CannotDecodeTextureException($"Data format {DataFormat:X2} is invalid or not supported for decoding.");
            }

            if (paletteData != null) // The texture contains an embedded palette
            {
                if (decodedPaletteData is null)
                {
                    decodedPaletteData = DecodePalette();
                }

                dataCodec.Palette = decodedPaletteData;
            }

            return dataCodec.Decode(textureData, 0, width, height);
        }

        /// <summary>
        /// Decodes the texture and returns the pixel data.
        /// </summary>
        /// <returns>The pixel data as a byte array.</returns>
        public byte[] GetPixelData()
        {
            if (decodedTextureData == null)
            {
                decodedTextureData = DecodeTexture();
            }

            return decodedTextureData;
        }
        #endregion

        #region Palette
        /// <summary>
        /// Gets if an external palette file is needed to decode.
        /// </summary>
        /// <returns></returns>
        public bool NeedsExternalPalette => dataCodec?.NeedsExternalPalette == true;

        /// <summary>
        /// Gets or sets the palette used when decoding.
        /// </summary>
        /// <remarks>This property must be set when <see cref="NeedsExternalPalette"/> is <see langword="true"/> and is ignored when <see langword="false"/>.</remarks>
        public SvrPalette Palette
        {
            get => palette;
            set
            {
                palette = value;

                // No need to actually update the data codec if a external palette isn't needed.
                if (!NeedsExternalPalette)
                {
                    return;
                }

                if (palette is SvrGrayscalePalette grayscalePalette)
                {
                    dataCodec.Palette = grayscalePalette.GetPaletteData(dataCodec);
                }
                else
                {
                    dataCodec.Palette = palette.GetPaletteData();
                }
            }
        }
        private SvrPalette palette;

        private byte[] DecodePalette()
        {
            var decodedPaletteData = new byte[paletteEntries * 4];

            var bytesPerPixel = pixelCodec.Bpp / 8;
            var sourceIndex = 0;
            var destinationIndex = 0;

            for (var i = 0; i < paletteEntries; i++)
            {
                pixelCodec.DecodePixel(paletteData, sourceIndex, decodedPaletteData, destinationIndex);

                sourceIndex += bytesPerPixel;
                destinationIndex += 4;
            }

            return decodedPaletteData;
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Checks for the PVRT header and validates it.
        /// </summary>
        /// <param name="reader">The data to check.</param>
        /// <param name="startPosition">The position in <paramref name="reader"/> to start checking at.</param>
        /// <returns>True if validation passes, false otherwise.</returns>
        private static bool IsValidPvrt(BinaryReader reader, long startPosition)
        {
            var remainingLength = reader.BaseStream.Length - startPosition;

            // Check the remaining length and the magic code
            if (!(remainingLength > 16
                && reader.At(startPosition, x => x.ReadBytes(pvrtMagicCode.Length).SequenceEqual(pvrtMagicCode))))
            {
                return false;
            }

            // Check the expected length. For SVRs, they sometimes have an extra byte at the end of the file.
            var pvrtLength = reader.At(startPosition + 0x4, x => x.ReadUInt32());
            if (!(pvrtLength == remainingLength - 8 || pvrtLength == remainingLength - 9))
            {
                return false;
            }

            // Check the data format.
            var dataFormat = reader.At(startPosition + 0x9, x => x.ReadByte());
            if (!(dataFormat >= 0x60 && dataFormat < 0x70))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for the GBIX and PVRT headers and validates them.
        /// </summary>
        /// <param name="reader">The data to check.</param>
        /// <param name="startPosition">The position in <paramref name="reader"/> to start checking at.</param>
        /// <returns>True if validation passes, false otherwise.</returns>
        private static bool IsValidGbixAndPvrt(BinaryReader reader, long startPosition)
        {
            var remainingLength = reader.BaseStream.Length - startPosition;

            if (!(remainingLength > 12
                && reader.At(startPosition, x => x.ReadBytes(gbixMagicCode.Length).SequenceEqual(gbixMagicCode))))
            {
                return false;
            }

            var gbixLength = reader.At(startPosition + 0x4, x => x.ReadInt32()) + 8;

            return IsValidPvrt(reader, startPosition + gbixLength);
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return IsValidGbixAndPvrt(reader, startPosition) // PVRT with GBIX
                    || IsValidPvrt(reader, startPosition); // PVRT only
            }
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
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
