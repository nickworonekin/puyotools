using PuyoTools.Core.Textures.Svr.PixelCodecs;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Textures.Svr
{
    public class SvrPalette
    {
        private PixelCodec pixelCodec; // Pixel Codec
        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] magicCode = { (byte)'P', (byte)'V', (byte)'P', (byte)'L' };

        private byte[] paletteData;

        private byte[] decodedData;

        #region Palette Properties
        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public virtual SvrPixelFormat PixelFormat { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a SVR palette from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the palette data.</param>
        public SvrPalette(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Open a SVR palette from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the palette data.</param>
        public SvrPalette(Stream source)
        {
            Initialize(source);
        }

        protected SvrPalette()
        {
        }

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a SVR palette
            if (!Is(source))
            {
                throw new InvalidFormatException("Not a valid SVR palette.");
            }

            var startPosition = source.Position;
            var reader = new BinaryReader(source);

            reader.BaseStream.Position += 8; // 0x08

            // Get the pixel format and the codec and make sure we can decode using them
            PixelFormat = (SvrPixelFormat)reader.ReadByte();
            pixelCodec = PixelCodecFactory.Create(PixelFormat);

            reader.BaseStream.Position += 5; // 0x0E

            // Get the number of colors contained in the palette
            paletteEntries = reader.ReadUInt16();

            if (pixelCodec is null)
            {
                return;
            }

            // Read the palette data
            paletteData = reader.ReadBytes(paletteEntries * pixelCodec.BitsPerPixel / 8);
        }

        // Decodes a palette
        private byte[] DecodePalette()
        {
            // Verify that a pixel codec has been set.
            if (pixelCodec is null)
            {
                throw new NotSupportedException($"Pixel format {PixelFormat:X} is invalid or not supported for decoding.");
            }

            // Decode the palette
            var decodedData = new byte[paletteEntries * 4];

            var bytesPerPixel = pixelCodec.BitsPerPixel / 8;
            var sourceIndex = 0;
            var destinationIndex = 0;

            for (var i = 0; i < paletteEntries; i++)
            {
                pixelCodec.DecodePixel(paletteData, sourceIndex, decodedData, destinationIndex);

                sourceIndex += bytesPerPixel;
                destinationIndex += 4;
            }

            return decodedData;
        }

        /// <summary>
        /// Decodes the palette and returns the palette data.
        /// </summary>
        /// <returns>The palette data as a byte array.</returns>
        public virtual byte[] GetPaletteData()
        {
            if (decodedData == null)
            {
                decodedData = DecodePalette();
            }

            return decodedData;
        }
        #endregion

        #region Palette Check
        /// <summary>
        /// Determines if this is a SVR palette.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a SVR palette, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return remainingLength >= 16
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)
                    && reader.At(startPosition + 0x4, x => x.ReadUInt32()) == remainingLength - 8;
            }
        }

        /// <summary>
        /// Determines if this is a SVR palette.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a SVR palette, false otherwise.</returns>
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