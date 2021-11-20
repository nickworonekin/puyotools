using PuyoTools.Core.Textures.Pvr.DataCodecs;
using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr
{
    public class PvrTextureDecoder
    {
        #region Fields
        private PixelCodec pixelCodec; // Pixel codec
        private DataCodec dataCodec;   // Data codec
        private PvrCompressionCodec compressionCodec; // Compression Codec

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
        public PvrPixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// Gets the data format.
        /// </summary>
        public PvrDataFormat DataFormat { get; private set; }

        /// <summary>
        /// Gets the compression format.
        /// </summary>
        public PvrCompressionFormat CompressionFormat { get; private set; }

        /// <summary>
        /// Gets the global index, or <see langword="null"/> if no global index exists.
        /// </summary>
        public uint? GlobalIndex { get; private set; }

        /// <summary>
        /// Gets the position of the PVRT chunk.
        /// </summary>
        internal long PvrtPosition { get; private set; }

        /// <summary>
        /// Gets the mipmaps, or an empty collection if there are none.
        /// </summary>
        /// <remarks>The collection is ordered by mipmap size in descending order.</remarks>
        public ReadOnlyCollection<PvrMipmapDecoder> Mipmaps => Array.AsReadOnly(mipmaps);
        private PvrMipmapDecoder[] mipmaps = Array.Empty<PvrMipmapDecoder>();
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a PVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public PvrTextureDecoder(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Open a PVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public PvrTextureDecoder(Stream source)
        {
            Initialize(source);
        }

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a PVR texture
            if (!Is(source))
            {
                throw new InvalidFormatException("Not a valid PVR texture.");
            }

            var startPosition = source.Position;
            var reader = new BinaryReader(source);

            // Determine the offsets of the GBIX (if present) and PVRT header chunks.
            if (reader.At(startPosition, x => x.ReadBytes(gbixMagicCode.Length).SequenceEqual(gbixMagicCode)))
            {
                ReadGbix(reader);
                PvrtPosition = source.Position - startPosition;
                ReadPvrt(reader);
            }
            else if (reader.At(startPosition, x => x.ReadBytes(pvrtMagicCode.Length).SequenceEqual(pvrtMagicCode)))
            {
                PvrtPosition = source.Position - startPosition;
                ReadPvrt(reader);
            }
            else if (reader.At(startPosition + 0x4, x => x.ReadBytes(gbixMagicCode.Length).SequenceEqual(gbixMagicCode)))
            {
                // Assume this is RLE compressed
                CompressionFormat = PvrCompressionFormat.Rle;

                source.Position += 4;

                ReadGbix(reader);
                PvrtPosition = source.Position - startPosition;
                ReadPvrt(reader);
            }
            else if (reader.At(startPosition + 0x4, x => x.ReadBytes(pvrtMagicCode.Length).SequenceEqual(pvrtMagicCode)))
            {
                // Assume this is RLE compressed
                CompressionFormat = PvrCompressionFormat.Rle;

                source.Position += 4;

                PvrtPosition = source.Position - startPosition;
                ReadPvrt(reader);
            }
            else
            {
                // This should never be thrown, but is included anyway.
                throw new InvalidFormatException("GBIX and PVRT headers not found.");
            }

            // Get the codecs and make sure we can decode using them
            pixelCodec = PixelCodecFactory.Create(PixelFormat);
            dataCodec = DataCodecFactory.Create(DataFormat, pixelCodec);

            // If we don't have a known pixel or data codec for these formats, that's ok.
            // This will allow the properties to be read if the user doesn't want to decode this texture.
            // The exception will be thrown when the texture is being decoded.
            if (pixelCodec is null || dataCodec is null)
            {
                return;
            }

            if (CompressionFormat != PvrCompressionFormat.None)
            {
                compressionCodec = PvrCompressionCodec.GetCompressionCodec(CompressionFormat);
            }

            // Get the number of palette entries.
            // In a Small VQ encoded texture, it's determined by the texture dimensions.
            paletteEntries = dataCodec.PaletteEntries;
            if (DataFormat == PvrDataFormat.SmallVq)
            {
                if (Width <= 16)
                {
                    paletteEntries = 64; // Actually 16
                }
                else if (Width <= 32)
                {
                    paletteEntries = 128; // Actually 32
                }
                else if (Width <= 64)
                {
                    paletteEntries = 512; // Actually 128
                }
                else
                {
                    paletteEntries = 1024; // Actually 256
                }
            }
            else if (DataFormat == PvrDataFormat.SmallVqMipmaps)
            {
                if (Width <= 16)
                {
                    paletteEntries = 64; // Actually 16
                }
                else if (Width <= 32)
                {
                    paletteEntries = 256; // Actually 64
                }
                else
                {
                    paletteEntries = 1024; // Actually 256
                }
            }

            // Read the palette data (if present)
            if (dataCodec.PaletteEntries != 0 && !dataCodec.NeedsExternalPalette)
            {
                paletteData = reader.ReadBytes(paletteEntries * pixelCodec.BitsPerPixel / 8);
            }

            // If the texture contains mipmaps, read them into the texture mipmap data array.
            // Mipmaps are stored in order from smallest to largest.
            if (dataCodec.HasMipmaps)
            {
                // The mipmap array only stores the smaller mipmaps, not the full-size texture.
                mipmaps = new PvrMipmapDecoder[(int)Math.Log(Width, 2)];

                // Calculate the initial padding for the 1x1 mipmap.
                // Due to PVR twiddling works, the actual pixel will be in the last bytes of its mipmap.
                switch (DataFormat)
                {
                    // A 1x1 mipmap takes up as much space as a 2x1 mipmap.
                    case PvrDataFormat.SquareTwiddledMipmaps:
                        source.Position += dataCodec.BitsPerPixel / 8;
                        break;

                    // A 1x1 mipmap takes up as much space as a 2x2 mipmap.
                    // The pixel is stored in the upper 4 bits of the final byte.
                    case PvrDataFormat.Index4Mipmaps:
                        source.Position += 2 * dataCodec.BitsPerPixel / 8;
                        break;

                    // A 1x1 mipmap takes up as much space as a 2x2 mipmap.
                    case PvrDataFormat.Index8Mipmaps:
                        source.Position += 3 * dataCodec.BitsPerPixel / 8;
                        break;

                    // A 1x1 mipmap takes up as much space as a 2x2 mipmap.
                    case PvrDataFormat.SquareTwiddledMipmapsAlt:
                        source.Position += 3 * dataCodec.BitsPerPixel / 8;
                        break;
                }

                for (int i = mipmaps.Length - 1, size = 1; i >= 0; i--, size <<= 1)
                {
                    mipmaps[i] = new PvrMipmapDecoder(
                        this,
                        reader.ReadBytes(Math.Max(size * size * dataCodec.BitsPerPixel / 8, 1)),
                        size,
                        size);
                }
            }

            // Read the texture data
            if (compressionCodec is not null)
            {
                // The texture data is compressed.
                textureData = new byte[Width * Height * dataCodec.BitsPerPixel / 8];
                compressionCodec.Decompress(source, new MemoryStream(textureData), pixelCodec, dataCodec);
            }
            else
            {
                textureData = reader.ReadBytes(Width * Height * dataCodec.BitsPerPixel / 8);
            }
        }

        private void ReadGbix(BinaryReader reader)
        {
            var startPosition = reader.BaseStream.Position;
            reader.BaseStream.Position += 4; // 0x04

            // Get the length of the GBIX header
            var length = reader.ReadInt32() + 8;

            // Read the global index
            GlobalIndex = reader.ReadUInt32();

            reader.BaseStream.Position = startPosition + length;
        }

        private void ReadPvrt(BinaryReader reader)
        {
            PvrtPosition = reader.BaseStream.Position;

            reader.BaseStream.Position += 8; // 0x08

            // Get the pixel & data formats
            PixelFormat = (PvrPixelFormat)reader.ReadByte();
            DataFormat = (PvrDataFormat)reader.ReadByte();

            // Get the texture dimensions
            reader.BaseStream.Position += 2; // 0x0C
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
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
            var image = Image.LoadPixelData<Bgra32>(GetPixelData(), Width, Height);
            image.Save(destination, new PngEncoder());
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
                throw new NotSupportedException($"Pixel format {PixelFormat:X} is not supported for decoding.");
            }
            if (dataCodec is null)
            {
                throw new NotSupportedException($"Data format {DataFormat:X} is not supported for decoding.");
            }

            // Verify that a palette has been set for data formats requiring external palettes.
            if (NeedsExternalPalette && dataCodec.Palette is null)
            {
                throw new InvalidOperationException("An external palette file is required for decoding.");
            }

            if (paletteData is not null) // The texture contains an embedded palette
            {
                if (decodedPaletteData is null)
                {
                    decodedPaletteData = DecodePalette();
                }

                dataCodec.Palette = decodedPaletteData;
            }

            return dataCodec.Decode(textureData, width, height);
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
        public bool NeedsExternalPalette => dataCodec?.NeedsExternalPalette == true;

        /// <summary>
        /// Gets or sets the palette used when decoding.
        /// </summary>
        /// <remarks>This property must be set when <see cref="NeedsExternalPalette"/> is <see langword="true"/> and is ignored when <see langword="false"/>.</remarks>
        public PvrPalette Palette
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

                if (palette is PvrGrayscalePalette grayscalePalette)
                {
                    dataCodec.Palette = grayscalePalette.GetPaletteData(dataCodec);
                }
                else
                {
                    dataCodec.Palette = palette.GetPaletteData();
                }
            }
        }
        private PvrPalette palette;

        private byte[] DecodePalette()
        {
            var decodedPaletteData = new byte[paletteEntries * 4];

            var bytesPerPixel = pixelCodec.BitsPerPixel / 8;
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
        /// <param name="expectedLength">The expected length of the PVR data minus the preceding header sizes. If <see langword="null"/>, the remaining length of <paramref name="reader"/> is used.</param>
        /// <returns>True if validation passes, false otherwise.</returns>
        private static bool IsValidPvrt(BinaryReader reader, long startPosition, long? expectedLength = null)
        {
            var remainingLength = reader.BaseStream.Length - startPosition;
            var length = expectedLength ?? remainingLength;

            return remainingLength > 16
                && reader.At(startPosition, x => x.ReadBytes(pvrtMagicCode.Length).SequenceEqual(pvrtMagicCode))
                && reader.At(startPosition + 0x4, x => x.ReadUInt32()) == length - 8
                && reader.At(startPosition + 0x9, x => x.ReadByte()) < 0x60;
        }

        /// <summary>
        /// Checks for the GBIX and PVRT headers and validates them.
        /// </summary>
        /// <param name="reader">The data to check.</param>
        /// <param name="startPosition">The position in <paramref name="reader"/> to start checking at.</param>
        /// <param name="expectedLength">The expected length of the PVR data. If <see langword="null"/>, the remaining length of <paramref name="reader"/> is used.</param>
        /// <returns>True if validation passes, false otherwise.</returns>
        private static bool IsValidGbixAndPvrt(BinaryReader reader, long startPosition, long? expectedLength = null)
        {
            var remainingLength = reader.BaseStream.Length - startPosition;

            if (!(remainingLength > 12
                && reader.At(startPosition, x => x.ReadBytes(gbixMagicCode.Length).SequenceEqual(gbixMagicCode))))
            {
                return false;
            }

            var gbixLength = reader.At(startPosition + 0x4, x => x.ReadInt32()) + 8;
            if (expectedLength != null)
            {
                expectedLength -= gbixLength;
            }

            return IsValidPvrt(reader, startPosition + gbixLength, expectedLength);
        }

        /// <summary>
        /// Determines if this is a PVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a PVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                if (IsValidGbixAndPvrt(reader, startPosition) // PVRT with GBIX
                    || IsValidPvrt(reader, startPosition)) // PVRT only
                {
                    return true;
                }

                // Check if this is a valid texture that's using RLE compression.
                if (remainingLength > 4)
                {
                    var expectedLength = reader.At(startPosition, x => x.ReadInt32());
                    if (IsValidGbixAndPvrt(reader, startPosition + 0x4, expectedLength) // PVRT with GBIX
                        || IsValidPvrt(reader, startPosition + 0x4, expectedLength)) // PVRT only
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if this is a PVR texture.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a PVR texture, false otherwise.</returns>
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