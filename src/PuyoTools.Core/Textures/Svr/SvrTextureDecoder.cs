using PuyoTools.Core.Textures.Svr.DataCodecs;
using PuyoTools.Core.Textures.Svr.PixelCodecs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Textures.Svr
{
    public class SvrTextureDecoder
    {
        #region Fields
        private PixelCodec pixelCodec; // Pixel codec
        private DataCodec dataCodec;   // Data codec

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

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a SVR texture
            if (!Is(source))
            {
                throw new InvalidFormatException("Not a valid SVR texture.");
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
            pixelCodec = PixelCodecFactory.Create(PixelFormat);
            dataCodec = DataCodecFactory.Create(DataFormat, pixelCodec);

            // If we don't have a known pixel or data codec for these formats, that's ok.
            // This will allow the properties to be read if the user doesn't want to decode this texture.
            // The exception will be thrown when the texture is being decoded.
            if (pixelCodec is null || dataCodec is null)
            {
                return;
            }

            // Get the number of palette entries.
            paletteEntries = dataCodec.PaletteEntries;

            // Read the palette data (if present).
            if (dataCodec.PaletteEntries != 0 && !dataCodec.NeedsExternalPalette)
            {
                paletteData = reader.ReadBytes(paletteEntries * pixelCodec.BitsPerPixel / 8);
            }

            // Read the texture data
            textureData = reader.ReadBytes(Width * Height * dataCodec.BitsPerPixel / 8);
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

            if (paletteData != null) // The texture contains an embedded palette
            {
                if (decodedPaletteData is null)
                {
                    decodedPaletteData = DecodePalette();
                }

                dataCodec.Palette = decodedPaletteData;
            }

            return dataCodec.Decode(DataSwizzler.UnSwizzle(textureData, Width, Height, dataCodec.BitsPerPixel), width, height);
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
