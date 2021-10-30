using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace PuyoTools.Core.Textures.Gim
{
    public class GimTextureEncoder
    {
        #region Fields
        private GimPixelCodec paletteCodec; // Palette codec
        private GimDataCodec pixelCodec;   // Pixel codec

        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] magicCode =
        {
            (byte)'M', (byte)'I', (byte)'G', (byte)'.',
            (byte)'0', (byte)'0', (byte)'.',
            (byte)'1', (byte)'P', (byte)'S', (byte)'P',
            0,
        };

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

        private Image<Bgra32> sourceImage;

        private GimMetadata metadata;
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

        /// <summary>
        /// Gets or sets the endianness. Defaults to <see cref="Endianness.Little"/>.
        /// </summary>
        public Endianness Endianness { get; set; } = Endianness.Little;
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(string file, GimDataFormat pixelFormat)
            : this(file, null, pixelFormat)
        {
        }

        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(string file, GimPaletteFormat? paletteFormat, GimDataFormat pixelFormat)
        {
            using (var stream = File.OpenRead(file))
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
        public GimTextureEncoder(Stream source, GimDataFormat pixelFormat)
            : this(source, null, pixelFormat)
        {
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GimTextureEncoder(Stream source, GimPaletteFormat? paletteFormat, GimDataFormat pixelFormat)
        {
            Initialize(source, paletteFormat, pixelFormat);
        }

        private void Initialize(Stream source, GimPaletteFormat? paletteFormat, GimDataFormat pixelFormat)
        {
            // Set the palette and pixel formats, and verify that we can encode to them.
            // We'll also need to verify that the palette format is set if it's a palettized pixel format.
            // Unlike with the decoder, an exception will be thrown here if a codec cannot be used to encode them.
            PixelFormat = pixelFormat;
            pixelCodec = GimDataCodec.GetDataCodec(pixelFormat);
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
                paletteCodec = GimPixelCodec.GetPixelCodec(paletteFormat.Value);
                if (paletteCodec is null)
                {
                    throw new NotSupportedException($"Palette format {PaletteFormat:X} is not supported for encoding.");
                }
                pixelCodec.PixelCodec = paletteCodec;
            }

            // Read the image.
            sourceImage = Image.Load<Bgra32>(source);

            Width = sourceImage.Width;
            Height = sourceImage.Height;

            // Verify if the dimensions are valid
            if (!HasValidDimensions(Width, Height))
            {
                throw new NotSupportedException("Source image width must be a multiple of 16 and height must be a multiple of 8.");
            }

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
        public bool HasMetadata { get; set; } = true;
        #endregion

        #region Texture Retrieval
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
                paletteDataChunkLength +
                textureDataChunkLength +
                metadataChunkLength;

            // Write the GIM header
            writer.Write(magicCode);
            writer.WriteInt32(0);

            // Write the EOF offset chunk
            writer.WriteUInt16(0x02);
            writer.WriteUInt16(0);
            writer.WriteInt32(textureLength - 16);
            writer.WriteInt32(eofOffsetChunkLength);
            writer.WriteUInt32(16);

            // Write the metadata offset chunk
            writer.WriteUInt16(0x03);
            writer.WriteUInt16(0);

            if (HasMetadata)
            {
                writer.WriteInt32(textureLength - metadataChunkLength - 32);
            }
            else
            {
                writer.WriteInt32(textureLength - 32);
            }

            writer.WriteInt32(metadataOffsetChunkLength);
            writer.WriteUInt32(16);

            // Write the palette data, if we have a palette
            if (encodedPaletteData is not null)
            {
                writer.WriteUInt16(0x05);
                writer.WriteUInt16(0);
                writer.WriteInt32(paletteDataChunkLength);
                writer.WriteInt32(paletteDataChunkLength);
                writer.WriteUInt32(16);

                writer.WriteUInt16(48);
                writer.WriteUInt16(0);
                writer.WriteUInt16((byte)PaletteFormat.Value);
                writer.WriteUInt16(0);
                writer.WriteUInt16((ushort)paletteEntries);
                writer.Write(new byte[] { 0x01, 0x00, 0x20, 0x00, 0x10, 0x00 });

                writer.Write(new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00 });
                writer.WriteUInt32(0x30);
                writer.WriteUInt32(0x40);

                writer.WriteInt32(paletteDataChunkLength - 16);
                writer.WriteUInt32(0);
                writer.Write(new byte[] { 0x02, 0x00, 0x01, 0x00, 0x03, 0x00, 0x01, 0x00 });

                writer.Write(new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

                writer.Write(encodedPaletteData);
            }

            // Write the texture data
            writer.WriteUInt16(0x04);
            writer.WriteUInt16(0);
            writer.WriteInt32(textureDataChunkLength);
            writer.WriteInt32(textureDataChunkLength);
            writer.WriteUInt32(16);

            writer.WriteUInt16(48);
            writer.WriteUInt16(0);
            writer.WriteUInt16((ushort)PixelFormat);
            writer.WriteUInt16(1); // Always swizzled
            writer.WriteUInt16((ushort)Width);
            writer.WriteUInt16((ushort)Height);

            if (paletteEntries != 0)
            {
                // For palettized textures, this is the bpp for this data format
                writer.WriteUInt16((ushort)pixelCodec.Bpp);
            }
            else
            {
                // For non-palettized textures, this is always specified as 32bpp
                writer.WriteUInt16(32);
            }

            writer.WriteUInt16(16);

            writer.Write(new byte[] { 0x08, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00 });

            writer.WriteInt32(textureDataChunkLength - 16);
            writer.WriteUInt32(0);
            writer.Write(new byte[] { 0x01, 0x00, 0x01, 0x00, 0x03, 0x00, 0x01, 0x00 });

            writer.Write(new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

            writer.Write(GimDataCodec.Swizzle(encodedTextureData, 0, Width, Height, pixelCodec.Bpp));

            // Write the metadata, only if we are including it
            if (HasMetadata)
            {
                writer.WriteUInt16(0xFF);
                writer.WriteUInt16(0);
                writer.WriteInt32(metadataChunkLength);
                writer.WriteInt32(metadataChunkLength);
                writer.WriteUInt32(16);

                writer.WriteNullTerminatedString(metadata.OriginalFilename);
                writer.WriteNullTerminatedString(metadata.User);
                writer.WriteNullTerminatedString(metadata.Timestamp);
                writer.WriteNullTerminatedString(metadata.Program);
            }
        }
        #endregion

        #region Texture Conversion
        /// <summary>
        /// Encodes the texture. Also encodes the palette if needed.
        /// </summary>
        /// <returns>The byte array containing the encoded texture data.</returns>
        private byte[] EncodeTexture()
        {
            byte[] pixelData;

            // Encode as a palettized image.
            if (paletteEntries != 0)
            {
                // Create the quantizer and quantize the texture.
                IQuantizer<Bgra32> quantizer;
                IndexedImageFrame<Bgra32> imageFrame;
                var quantizerOptions = new QuantizerOptions
                {
                    MaxColors = paletteEntries,
                };

                if (ImageHelper.TryBuildExactPalette(sourceImage, paletteEntries, out var palette))
                {
                    quantizer = new PaletteQuantizer(palette.Cast<Color>().ToArray(), quantizerOptions)
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

            return pixelCodec.Encode(pixelData, 0, Width, Height);
        }

        /// <summary>
        /// Encodes the palette.
        /// </summary>
        /// <returns></returns>
        private byte[] EncodePalette(ReadOnlyMemory<Bgra32> palette)
        {
            var bytesPerPixel = paletteCodec.Bpp / 8;
            var paletteData = MemoryMarshal.AsBytes(palette.Span).ToArray();
            var encodedPaletteData = new byte[palette.Length * bytesPerPixel];

            for (var i = 0; i < palette.Length; i++)
            {
                paletteCodec.EncodePixel(paletteData, 4 * i, encodedPaletteData, i * bytesPerPixel);
            }

            return encodedPaletteData;
        }
        #endregion
    }
}