using PuyoTools.Core.Textures.Gvr.PaletteCodecs;
using PuyoTools.Core.Textures.Gvr.PixelCodecs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr
{
    public class GvrTextureDecoder
    {
        private PaletteCodec paletteCodec; // Palette codec
        private PixelCodec pixelCodec;    // Pixel codec

        protected int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] gbixMagicCode = { (byte)'G', (byte)'B', (byte)'I', (byte)'X' };
        private static readonly byte[] gcixMagicCode = { (byte)'G', (byte)'C', (byte)'I', (byte)'X' };
        private static readonly byte[] gvrtMagicCode = { (byte)'G', (byte)'V', (byte)'R', (byte)'T' };

        private byte[] paletteData;
        private byte[] textureData;

        private byte[] decodedPaletteData;
        private byte[] decodedTextureData;

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
        /// Gets the palette format, or <see langword="null"/> if a palette is not used.
        /// </summary>
        public GvrPixelFormat? PaletteFormat { get; private set; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        public GvrDataFlags Flags { get; private set; }

        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public GvrDataFormat PixelFormat { get; private set; }

        /// <summary>
        /// Gets the global index, or <see langword="null"/> if no global index exists.
        /// </summary>
        public uint? GlobalIndex { get; private set; }

        /// <summary>
        /// Gets the position of the GVRT chunk.
        /// </summary>
        internal long GvrtPosition { get; private set; }

        /// <summary>
        /// Gets the mipmaps, or an empty collection if there are none.
        /// </summary>
        /// <remarks>The collection is ordered by mipmap size in descending order.</remarks>
        public ReadOnlyCollection<GvrMipmapDecoder> Mipmaps => Array.AsReadOnly(mipmaps);
        private GvrMipmapDecoder[] mipmaps = Array.Empty<GvrMipmapDecoder>();
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a GVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public GvrTextureDecoder(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Open a GVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public GvrTextureDecoder(Stream source)
        {
            Initialize(source);
        }

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a GVR texture
            if (!Is(source))
            {
                throw new InvalidFormatException("Not a valid GVR texture.");
            }

            var startPosition = source.Position;
            var reader = new BinaryReader(source);

            // Read the GBIX/GCIX header (if present).
            if (IsGbixOrGcix(reader, startPosition))
            {
                source.Position += 4; // 0x04

                // Get the length of the GBIX/GCIX header
                var gbixLength = reader.ReadInt32() + 8;

                // Read the global index
                GlobalIndex = reader.ReadUInt32BigEndian();

                source.Position = startPosition + gbixLength;
            }

            GvrtPosition = source.Position;

            source.Position += 10; // 0x0A

            // Get the flags and the pixel format.
            var paletteFormatAndFlags = reader.ReadByte();
            Flags = (GvrDataFlags)(paletteFormatAndFlags & 0xF); // Flags uses the lower 4 bits
            PixelFormat = (GvrDataFormat)reader.ReadByte();

            // Get the texture dimensions
            Width = reader.ReadUInt16BigEndian();
            Height = reader.ReadUInt16BigEndian();

            // Get the codecs and make sure we can decode using them
            pixelCodec = PixelCodecFactory.Create(PixelFormat);

            // If we don't have a known pixel codec for this format, that's ok.
            // This will allow the properties to be read if the user doesn't want to decode this texture.
            // The exception will be thrown when the texture is being decoded.
            if (pixelCodec is null)
            {
                return;
            }

            // Get the palette format and codec if a palette is used.
            if (pixelCodec.PaletteEntries != 0)
            {
                PaletteFormat = (GvrPixelFormat)(paletteFormatAndFlags >> 4); // Palette format uses the upper 4 bits
                paletteCodec = PaletteCodecFactory.Create(PaletteFormat.Value);

                // If we don't have a known palette codec for this format, that's ok.
                // This will allow the properties to be read if the user doesn't want to decode this texture.
                // The exception will be thrown when the texture is being decoded.
                if (paletteCodec is null)
                {
                    return;
                }
            }

            // Get the number of palette entries.
            paletteEntries = pixelCodec.PaletteEntries;

            // Read the palette data (if present).
            if (pixelCodec.PaletteEntries != 0 && Flags.HasFlag(GvrDataFlags.InternalPalette))
            {
                paletteData = reader.ReadBytes(paletteEntries * paletteCodec.BitsPerPixel / 8);
            }

            // Read the texture data
            textureData = reader.ReadBytes(Width * Height * pixelCodec.BitsPerPixel / 8);

            // If the texture contains mipmaps, read them into the texture mipmap data array.
            // Mipmaps are stored in order from largest to smallest.
            if (Flags.HasFlag(GvrDataFlags.Mipmaps))
            {
                // The mipmap array only stores the smaller mipmaps, not the full-size texture.
                mipmaps = new GvrMipmapDecoder[(int)Math.Log(Width, 2)];

                for (int i = 0, size = Width >> 1; i < mipmaps.Length; i++, size >>= 1)
                {
                    mipmaps[i] = new GvrMipmapDecoder(
                        this,
                        reader.ReadBytes(Math.Max(size * size * pixelCodec.BitsPerPixel / 8, 32)),
                        size,
                        size);
                }
            }
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
            // Verify that a palette codec (if required) and pixel codec have been set.
            if (pixelCodec is null)
            {
                throw new NotSupportedException($"Pixel format {PixelFormat:X} is not supported for decoding.");
            }
            if (paletteCodec is null && pixelCodec.PaletteEntries != 0)
            {
                throw new NotSupportedException($"Palette format {PaletteFormat:X} is not supported for decoding.");
            }

            // Verify that a palette has been set for data formats requiring external palettes.
            if (NeedsExternalPalette && pixelCodec.Palette is null)
            {
                throw new InvalidOperationException("An external palette file is required for decoding.");
            }

            if (paletteData != null) // The texture contains an embedded palette
            {
                if (decodedPaletteData is null)
                {
                    decodedPaletteData = DecodePalette();
                }

                pixelCodec.Palette = decodedPaletteData;
            }

            return pixelCodec.Decode(textureData, width, height);
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
        public bool NeedsExternalPalette => paletteEntries != 0 && Flags.HasFlag(GvrDataFlags.ExternalPalette);

        /// <summary>
        /// Gets or sets the palette used when decoding.
        /// </summary>
        /// <remarks>This property must be set when <see cref="NeedsExternalPalette"/> is <see langword="true"/> and is ignored when <see langword="false"/>.</remarks>
        public GvrPalette Palette
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

                if (palette is GvrGrayscalePalette grayscalePalette)
                {
                    pixelCodec.Palette = grayscalePalette.GetPaletteData(pixelCodec);
                }
                else
                {
                    pixelCodec.Palette = palette.GetPaletteData();
                }
            }
        }
        private GvrPalette palette;

        private byte[] DecodePalette()
        {
            /*var decodedPaletteData = new byte[paletteEntries * 4];

            var bytesPerPixel = paletteCodec.BitsPerPixel / 8;
            var sourceIndex = 0;
            var destinationIndex = 0;

            for (var i = 0; i < paletteEntries; i++)
            {
                paletteCodec.DecodePixel(paletteData, sourceIndex, decodedPaletteData, destinationIndex);

                sourceIndex += bytesPerPixel;
                destinationIndex += 4;
            }

            return decodedPaletteData;*/

            return paletteCodec.Decode(paletteData);
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Returns if the data at the specified position is equal to GBIX or GCIX.
        /// </summary>
        /// <param name="reader">The data to check.</param>
        /// <param name="startPosition">The position in <paramref name="reader"/> to start checking at. If <see langword="null"/>, the current position is used.</param>
        /// <returns></returns>
        private static bool IsGbixOrGcix(BinaryReader reader, long? startPosition = null)
        {
            startPosition = startPosition ?? reader.BaseStream.Position;
            var remainingLength = reader.BaseStream.Length - startPosition;

            if (!(remainingLength > 4))
            {
                return false;
            }

            var magicCode = reader.At(reader.BaseStream.Position, x => x.ReadBytes(gbixMagicCode.Length)); // OK to use the GBIX length

            return magicCode.SequenceEqual(gbixMagicCode)
                || magicCode.SequenceEqual(gcixMagicCode);
        }

        /// <summary>
        /// Checks for the GVRT header and validates it.
        /// </summary>
        /// <param name="reader">The data to check.</param>
        /// <param name="startPosition">The position in <paramref name="reader"/> to start checking at.</param>
        /// <returns>True if validation passes, false otherwise.</returns>
        private static bool IsValidGvrt(BinaryReader reader, long startPosition)
        {
            var remainingLength = reader.BaseStream.Length - startPosition;

            return remainingLength > 16
                && reader.At(startPosition, x => x.ReadBytes(gvrtMagicCode.Length).SequenceEqual(gvrtMagicCode))
                && reader.At(startPosition + 0x4, x => x.ReadUInt32()) == remainingLength - 8;
        }

        /// <summary>
        /// Checks for the GBIX/GCIX and GVRT headers and validates them.
        /// </summary>
        /// <param name="reader">The data to check.</param>
        /// <param name="startPosition">The position in <paramref name="reader"/> to start checking at.</param>
        /// <returns>True if validation passes, false otherwise.</returns>
        private static bool IsValidGbixAndGvrt(BinaryReader reader, long startPosition)
        {
            var remainingLength = reader.BaseStream.Length - startPosition;

            if (!(remainingLength > 12
                && IsGbixOrGcix(reader, startPosition)))
            {
                return false;
            }

            var gbixLength = reader.At(startPosition + 0x4, x => x.ReadInt32()) + 8;

            return IsValidGvrt(reader, startPosition + gbixLength);
        }

        /// <summary>
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return IsValidGbixAndGvrt(reader, startPosition) // PVRT with GBIX/GCIX
                    || IsValidGvrt(reader, startPosition); // PVRT only
            }
        }

        /// <summary>
        /// Determines if this is a GVR texture.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a GVR texture, false otherwise.</returns>
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