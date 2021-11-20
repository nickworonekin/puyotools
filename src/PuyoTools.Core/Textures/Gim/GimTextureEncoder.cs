using PuyoTools.Core.Textures.Gim.PaletteCodecs;
using PuyoTools.Core.Textures.Gim.PixelCodecs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace PuyoTools.Core.Textures.Gim
{
    public class GimTextureEncoder
    {
        private PaletteCodec paletteCodec; // Palette codec
        private PixelCodec pixelCodec;   // Pixel codec

        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] magicCodeLittleEndian =
        {
            (byte)'M', (byte)'I', (byte)'G', (byte)'.',
            (byte)'0', (byte)'0', (byte)'.', (byte)'1',
            (byte)'P', (byte)'S', (byte)'P', 0,
        };

        private static readonly byte[] magicCodeBigEndian =
        {
            (byte)'.', (byte)'G', (byte)'I', (byte)'M',
            (byte)'1', (byte)'.', (byte)'0', (byte)'0',
            0, (byte)'P', (byte)'S', (byte)'P',
        };

        private byte[] encodedPaletteData;
        private byte[] encodedTextureData;

        private readonly int strideAlignment = 16; // Stride alignment will always be 16, except for DXTn-based pixel formats.
        private int heightAlignment; // Height alignment will be 8 (if swizzled) or 1 (if not swizzled), or 4 if using a DXTn pixel format.

        private int stride;
        private int pixelsPerRow;
        private int pixelsPerColumn;

        private Image<Bgra32> sourceImage;

        private GimMetadata metadata;

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
        public GimPixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// Gets or sets if the texture should include metadata.
        /// </summary>
        public bool HasMetadata { get; set; } = true;

        /// <summary>
        /// Gets or sets the endianness. Defaults to <see cref="Endianness.Little"/>.
        /// </summary>
        public Endianness Endianness
        {
            get => endianness;
            set
            {
                ArgumentHelper.ThrowIfInvalidEnumValue(value);

                endianness = value;
            }
        }
        private Endianness endianness = Endianness.Little;

        /// <summary>
        /// Gets or sets if this texture should be swizzled.
        /// </summary>
        /// <remarks>Swizzling isn't supported when using DXTn-based pixel formats.</remarks>
        public bool IsSwizzled { get; set; }

        /// <summary>
        /// Gets or sets whether dithering should be used when quantizing.
        /// </summary>
        public bool Dither { get; set; }

        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="path">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(string path, GimPixelFormat pixelFormat)
            : this(path, null, pixelFormat)
        {
        }

        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="path">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(string path, GimPaletteFormat? paletteFormat, GimPixelFormat pixelFormat)
        {
            using (var stream = File.OpenRead(path))
            {
                Initialize(stream, paletteFormat, pixelFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(Stream source, GimPixelFormat pixelFormat)
            : this(source, null, pixelFormat)
        {
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(Stream source, GimPaletteFormat? paletteFormat, GimPixelFormat pixelFormat)
        {
            Initialize(source, paletteFormat, pixelFormat);
        }

        private void Initialize(Stream source, GimPaletteFormat? paletteFormat, GimPixelFormat pixelFormat)
        {
            // Set the palette and pixel formats, and verify that we can encode to them.
            // We'll also need to verify that the palette format is set if it's a palettized pixel format.
            // Unlike with the decoder, an exception will be thrown here if a codec cannot be used to encode them.
            PixelFormat = pixelFormat;
            pixelCodec = PixelCodecFactory.Create(pixelFormat);
            if (pixelCodec is null)
            {
                throw new NotSupportedException($"Pixel format {PixelFormat:X} is not supported for encoding.");
            }

            // Get the number of palette entries.
            paletteEntries = pixelCodec.PaletteEntries;

            if (paletteEntries != 0)
            {
                if (paletteFormat is null)
                {
                    throw new ArgumentNullException(nameof(paletteFormat), $"Palette format must be set for pixel format {PixelFormat}");
                }

                PaletteFormat = paletteFormat.Value;
                paletteCodec = PaletteCodecFactory.Create(paletteFormat.Value);
                if (paletteCodec is null)
                {
                    throw new NotSupportedException($"Palette format {PaletteFormat:X} is not supported for encoding.");
                }
            }

            // Read the image.
            sourceImage = Image.Load<Bgra32>(source);

            Width = sourceImage.Width;
            Height = sourceImage.Height;

            // Create the metadata and set the default values.
            metadata = new GimMetadata
            {
                OriginalFilename = source is FileStream fs
                    ? Path.GetFileName(fs.Name)
                    : "unnamed",
                User = Environment.UserName,
                Timestamp = DateTime.Now.ToString("ddd MMM d HH:mm:ss yyyy"),
                Program =
                    Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product
                    + " " +
                    Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,

            };
        }

        /// <summary>
        /// Saves the encoded texture to the specified path.
        /// </summary>
        /// <param name="path">Name of the file to save the data to.</param>
        public void Save(string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Saves the encoded texture to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            var writer = new BinaryWriter(destination);

            if (encodedTextureData is null)
            {
                encodedTextureData = EncodeTexture();
            }

            // Get the lengths of the various chunks
            int eofOffsetChunkLength = 16,
                metadataOffsetChunkLength = 16,
                paletteDataChunkLength = 0,
                textureDataChunkLength = 0,
                metadataChunkLength = 0;

            if (encodedPaletteData is not null)
            {
                paletteDataChunkLength = 80 + encodedPaletteData.Length;
            }

            textureDataChunkLength = 80 + encodedTextureData.Length;

            if (HasMetadata)
            {
                metadataChunkLength = 16;

                if (metadata.OriginalFilename is not null)
                {
                    metadataChunkLength += metadata.OriginalFilename.Length;
                }
                metadataChunkLength++;

                if (metadata.User is not null)
                {
                    metadataChunkLength += metadata.User.Length;
                }
                metadataChunkLength++;

                if (metadata.Timestamp is not null)
                {
                    metadataChunkLength += metadata.Timestamp.Length;
                }
                metadataChunkLength++;

                if (metadata.Program is not null)
                {
                    metadataChunkLength += metadata.Program.Length;
                }
                metadataChunkLength++;
            }

            // Calculate what the length of the texture will be
            int textureLength = 16 +
                eofOffsetChunkLength +
                metadataOffsetChunkLength +
                textureDataChunkLength +
                paletteDataChunkLength +
                metadataChunkLength;

            // Write the GIM header
            if (endianness == Endianness.Big)
            {
                writer.Write(magicCodeBigEndian);
            }
            else
            {
                writer.Write(magicCodeLittleEndian);
            }
            writer.WriteInt32(0);

            // Write the EOF offset chunk
            writer.WriteUInt16(0x02, endianness);
            writer.WriteUInt16(0);
            writer.WriteInt32(textureLength - 16, endianness);
            writer.WriteInt32(eofOffsetChunkLength, endianness);
            writer.WriteUInt32(16, endianness);

            // Write the metadata offset chunk
            writer.WriteUInt16(0x03, endianness);
            writer.WriteUInt16(0);

            if (HasMetadata)
            {
                writer.WriteInt32(textureLength - metadataChunkLength - 32, endianness);
            }
            else
            {
                writer.WriteInt32(textureLength - 32, endianness);
            }

            writer.WriteInt32(metadataOffsetChunkLength, endianness);
            writer.WriteUInt32(16, endianness);

            // Write the texture data
            writer.WriteUInt16(0x04, endianness);
            writer.WriteUInt16(0);
            writer.WriteInt32(textureDataChunkLength, endianness);
            writer.WriteInt32(textureDataChunkLength, endianness);
            writer.WriteUInt32(16, endianness);

            writer.WriteUInt16(48, endianness);
            writer.WriteUInt16(0);
            writer.WriteUInt16((ushort)PixelFormat, endianness);
            writer.WriteUInt16((ushort)(IsSwizzled ? 1 : 0), endianness);
            writer.WriteUInt16((ushort)Width, endianness);
            writer.WriteUInt16((ushort)Height, endianness);

            if (paletteEntries != 0)
            {
                // For palettized textures, this is the bpp for this data format
                writer.WriteUInt16((ushort)pixelCodec.BitsPerPixel, endianness);
            }
            else
            {
                // For non-palettized textures, this is always specified as 32bpp
                writer.WriteUInt16(32, endianness);
            }

            writer.WriteUInt16((ushort)strideAlignment, endianness); // Stride alignment (always 16)
            writer.WriteUInt16((ushort)heightAlignment, endianness); // Height alignment (always 8 for swizzled)

            writer.WriteUInt16(0x02, endianness);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0x30, endianness);
            writer.WriteUInt32(0x40, endianness);

            writer.WriteInt32(textureDataChunkLength - 16, endianness);
            writer.WriteUInt32(0);
            writer.WriteUInt16(0x01, endianness);
            writer.WriteUInt16(0x01, endianness);
            writer.WriteUInt16(0x03, endianness);
            writer.WriteUInt16(0x01, endianness);
            writer.WriteUInt32(0x40, endianness);
            writer.Write(new byte[12]);

            if (IsSwizzled)
            {
                writer.Write(Swizzle(encodedTextureData, stride, pixelsPerColumn));
            }
            else
            {
                writer.Write(encodedTextureData);
            }

            // Write the palette data, if we have a palette
            if (encodedPaletteData is not null)
            {
                writer.WriteUInt16(0x05, endianness);
                writer.WriteUInt16(0);
                writer.WriteInt32(paletteDataChunkLength, endianness);
                writer.WriteInt32(paletteDataChunkLength, endianness);
                writer.WriteUInt32(16, endianness);

                writer.WriteUInt16(48, endianness);
                writer.WriteUInt16(0);
                writer.WriteUInt16((byte)PaletteFormat.Value, endianness);
                writer.WriteUInt16(0);
                writer.WriteUInt16((ushort)paletteEntries, endianness);

                writer.WriteUInt16(0x01, endianness);
                writer.WriteUInt16(0x20, endianness);
                writer.WriteUInt16(0x10, endianness);
                writer.WriteUInt16(0x01, endianness);
                writer.WriteUInt16(0x02, endianness);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0x30, endianness);
                writer.WriteUInt32(0x40, endianness);

                writer.WriteInt32(paletteDataChunkLength - 16, endianness);
                writer.WriteUInt32(0);
                writer.WriteUInt16(0x02, endianness);
                writer.WriteUInt16(0x01, endianness);
                writer.WriteUInt16(0x03, endianness);
                writer.WriteUInt16(0x01, endianness);
                writer.WriteUInt32(0x40, endianness);
                writer.Write(new byte[12]);

                writer.Write(encodedPaletteData);
            }

            // Write the metadata, only if we are including it
            if (HasMetadata)
            {
                writer.WriteUInt16(0xFF, endianness);
                writer.WriteUInt16(0);
                writer.WriteInt32(metadataChunkLength, endianness);
                writer.WriteInt32(metadataChunkLength, endianness);
                writer.WriteUInt32(16, endianness);

                writer.WriteNullTerminatedString(metadata.OriginalFilename);
                writer.WriteNullTerminatedString(metadata.User);
                writer.WriteNullTerminatedString(metadata.Timestamp);
                writer.WriteNullTerminatedString(metadata.Program);
            }
        }

        /// <summary>
        /// Encodes the texture. Also encodes the palette if needed.
        /// </summary>
        /// <returns>The byte array containing the encoded texture data.</returns>
        private byte[] EncodeTexture()
        {
            byte[] pixelData;

            // Calculate the alignment, stride, and pixels per row/column.
            heightAlignment = IsSwizzled ? 8 : 1;

            stride = MathHelper.RoundUp((int)Math.Ceiling((double)Width * pixelCodec.BitsPerPixel / 8), strideAlignment);
            pixelsPerRow = stride * 8 / pixelCodec.BitsPerPixel;
            pixelsPerColumn = MathHelper.RoundUp(Height, heightAlignment);

            // Encode as a palettized image.
            if (paletteEntries != 0)
            {
                // Create the quantizer and quantize the texture.
                IQuantizer<Bgra32> quantizer;
                IndexedImageFrame<Bgra32> imageFrame;
                var quantizerOptions = new QuantizerOptions
                {
                    MaxColors = paletteEntries,
                    Dither = Dither ? QuantizerConstants.DefaultDither : null,
                };

                if (ImageHelper.TryBuildExactPalette(sourceImage, paletteEntries, out var palette))
                {
                    quantizer = new PaletteQuantizer(palette.Select(x => (Color)x).ToArray(), quantizerOptions)
                        .CreatePixelSpecificQuantizer<Bgra32>(Configuration.Default);

                    imageFrame = quantizer.QuantizeFrame(sourceImage.Frames.RootFrame, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
                }
                else
                {
                    quantizer = new WuQuantizer(quantizerOptions)
                        .CreatePixelSpecificQuantizer<Bgra32>(Configuration.Default);

                    imageFrame = quantizer.BuildPaletteAndQuantizeFrame(sourceImage.Frames.RootFrame, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
                }

                // Save the palette
                encodedPaletteData = EncodePalette(imageFrame.Palette);

                pixelData = ImageHelper.GetPixelDataAsBytes(imageFrame);
            }

            // Encode as an RGBA image.
            else
            {
                pixelData = ImageHelper.GetPixelDataAsBytes(sourceImage.Frames.RootFrame);
            }

            return pixelCodec.Encode(pixelData, Width, Height, pixelsPerRow, pixelsPerColumn);
        }

        /// <summary>
        /// Encodes the palette.
        /// </summary>
        /// <returns></returns>
        private byte[] EncodePalette(ReadOnlyMemory<Bgra32> palette)
        {
            var paletteData = MemoryMarshal.AsBytes(palette.Span).ToArray();

            return paletteCodec.Encode(paletteData);
        }

        private static byte[] Swizzle(byte[] source, int stride, int pixelsPerColumn)
        {
            int sourceIndex = 0;

            byte[] destination = new byte[stride * pixelsPerColumn];

            int rowblocks = stride / 16;

            for (int y = 0; y < pixelsPerColumn; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    int blockX = x / 16;
                    int blockY = y / 8;

                    int blockIndex = blockX + (blockY * rowblocks);
                    int blockAddress = blockIndex * 16 * 8;

                    destination[blockAddress + (x - blockX * 16) + ((y - blockY * 8) * 16)] = source[sourceIndex];
                    sourceIndex++;
                }
            }

            return destination;
        }
    }
}