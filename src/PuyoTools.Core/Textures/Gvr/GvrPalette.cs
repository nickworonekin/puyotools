using PuyoTools.Core.Textures.Gvr.PaletteCodecs;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr
{
    public class GvrPalette
    {
        private PaletteCodec paletteCodec; // Palette Codec
        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] magicCode = { (byte)'G', (byte)'V', (byte)'P', (byte)'L' };

        private byte[] paletteData;

        private byte[] decodedData;

        #region Palette Properties
        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public virtual GvrPixelFormat PaletteFormat { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a GVP palette from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the palette data.</param>
        public GvrPalette(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream);
            }
        }

        /// <summary>
        /// Open a GVP palette from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the palette data.</param>
        public GvrPalette(Stream source)
        {
            Initialize(source);
        }

        protected GvrPalette()
        {
        }

        private void Initialize(Stream source)
        {
            // Check to see if what we are dealing with is a GVR palette
            if (!Is(source))
            {
                throw new InvalidFormatException("Not a valid GVR palette.");
            }

            var startPosition = source.Position;
            var reader = new BinaryReader(source);

            reader.BaseStream.Position += 9; // 0x09

            // Get the pixel format and the codec and make sure we can decode using them
            PaletteFormat = (GvrPixelFormat)reader.ReadByte();
            paletteCodec = PaletteCodecFactory.Create(PaletteFormat);

            reader.BaseStream.Position += 4; // 0x0E

            // Get the number of colors contained in the palette
            paletteEntries = reader.ReadUInt16BigEndian();

            if (paletteCodec is null)
            {
                return;
            }

            // Read the palette data
            paletteData = reader.ReadBytes(paletteEntries * paletteCodec.BitsPerPixel / 8);
        }

        // Decodes a palette
        private byte[] DecodePalette()
        {
            // Verify that a palette codec has been set.
            if (paletteCodec is null)
            {
                throw new NotSupportedException($"Pixel format {PaletteFormat:X} is invalid or not supported for decoding.");
            }

            // Decode the palette
            return paletteCodec.Decode(paletteData);
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
        /// Determines if this is a GVR palette.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a GVR palette, false otherwise.</returns>
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
        /// Determines if this is a GVR palette.
        /// </summary>
        /// <param name="file">Filename of the file that contains the data.</param>
        /// <returns>True if this is a GVR palette, false otherwise.</returns>
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