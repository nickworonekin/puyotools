using PuyoTools.Core.Textures.Gvr.PaletteCodecs;
using PuyoTools.Core.Textures.Gvr.PixelCodecs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PuyoTools.Core.Textures.Gvr
{
    public class GvrTextureEncoder
    {
        #region Fields
        private PaletteCodec paletteCodec; // Palette codec
        private PixelCodec pixelCodec;   // Pixel codec

        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] gbixMagicCode = { (byte)'G', (byte)'B', (byte)'I', (byte)'X' };
        private static readonly byte[] gcixMagicCode = { (byte)'G', (byte)'C', (byte)'I', (byte)'X' };
        private static readonly byte[] gvrtMagicCode = { (byte)'G', (byte)'V', (byte)'R', (byte)'T' };

        private byte[] encodedPaletteData;
        private byte[] encodedTextureData;
        private byte[][] encodedMipmapData;

        private Image<Bgra32> sourceImage;
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
        /// Gets the flags.
        /// </summary>
        public GvrDataFlags Flags { get; private set; }

        /// <summary>
        /// Gets or sets the global index type. If <see cref="GvrGbixType.None"/>, the GBIX or GCIX header will not be written regardless of the value of <see cref="GlobalIndex"/>.
        /// </summary>
        public GvrGbixType GlobalIndexType { get; set; }

        /// <summary>
        /// Gets or sets the global index. If <see langword="null"/>, the GBIX or GCIX header will not be written regardless of the value of <see cref="GlobalIndexType"/>.
        /// </summary>
        public uint? GlobalIndex { get; set; }

        /// <summary>
        /// Gets the palette format, or <see langword="null"/> if a palette is not used.
        /// </summary>
        public GvrPixelFormat? PaletteFormat { get; private set; }

        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public GvrDataFormat PixelFormat { get; private set; }

        /// <summary>
        /// Gets or sets if the texture has mipmaps.
        /// </summary>
        /// <remarks>
        /// Only square textures with pixel formats
        /// <see cref="GvrDataFormat.Rgb565"/>, <see cref="GvrDataFormat.Rgb5a3"/>, and <see cref="GvrDataFormat.Dxt1"/>
        /// support mipmaps.
        /// If mipmaps aren't supported, this property will always return <see langword="false"/>.
        /// </remarks>
        public bool HasMipmaps
        {
            get => hasMipmaps;
            set
            {
                // Mipmaps can only be used with square textures on certain pixel formats.
                if (Width != Height
                    && !(PixelFormat == GvrDataFormat.Rgb565
                        || PixelFormat == GvrDataFormat.Rgb5a3
                        || PixelFormat == GvrDataFormat.Dxt1))
                {
                    return;
                }

                if (value)
                {
                    hasMipmaps = true;
                    Flags |= GvrDataFlags.Mipmaps;
                }
                else
                {
                    hasMipmaps = false;
                    Flags &= ~GvrDataFlags.Mipmaps;
                }
            }
        }
        private bool hasMipmaps;


        /// <summary>
        /// Gets or sets whether dithering should be used when quantizing.
        /// </summary>
        public bool Dither { get; set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(string file, GvrDataFormat pixelFormat)
            : this(file, null, pixelFormat)
        {
        }

        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(string file, GvrPixelFormat? paletteFormat, GvrDataFormat pixelFormat)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Stream source, GvrDataFormat pixelFormat)
            : this(source, null, pixelFormat)
        {
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Stream source, GvrPixelFormat? paletteFormat, GvrDataFormat pixelFormat)
        {
            Initialize(source, paletteFormat, pixelFormat);
        }

        private void Initialize(Stream source, GvrPixelFormat? paletteFormat, GvrDataFormat pixelFormat)
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
        }
        #endregion

        #region Palette
        /// <summary>
        /// Gets or sets if an external palette file will be created after encoding.
        /// </summary>
        /// <remarks>If palettes aren't used, this property will always return <see langword="false"/>.</remarks>
        public bool NeedsExternalPalette
        {
            get => needsExternalPalette;
            set
            {
                // If this is a non-palettized texture, don't do anything.
                if (pixelCodec.PaletteEntries == 0)
                {
                    return;
                }

                // If true, set the external palette flag and unset the internal palette flag.
                if (value)
                {
                    needsExternalPalette = true;
                    Flags &= ~GvrDataFlags.InternalPalette;
                    Flags |= GvrDataFlags.ExternalPalette;
                }

                // If false, set the internal palette flag and unset the external palette flag.
                else
                {
                    needsExternalPalette = false;
                    Flags &= ~GvrDataFlags.ExternalPalette;
                    Flags |= GvrDataFlags.InternalPalette;
                }
            }
        }
        private bool needsExternalPalette;
        #endregion

        #region Encode Texture
        /// <summary>
        /// Encodes the texture. Also encodes the palette and mipmaps if needed.
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

                // Palettized textures don't support mipmaps, so that logic will not be included here.

                // Save the palette
                if (NeedsExternalPalette)
                {
                    Palette = new GvrPaletteEncoder(this, EncodePalette(imageFrame.Palette), imageFrame.Palette.Length);
                }
                else
                {
                    encodedPaletteData = EncodePalette(imageFrame.Palette);
                }

                pixelData = ImageHelper.GetPixelDataAsBytes(imageFrame);
            }

            // Encode as an RGBA image.
            else
            {
                // Encode mipmaps
                if (HasMipmaps)
                {
                    encodedMipmapData = new byte[(int)Math.Log(Width, 2)][];

                    // Mipmaps are ordered from largest to smallest.
                    for (int i = 0, size = Width >> 1; i < encodedMipmapData.Length && size >= 1; i++, size >>= 1)
                    {
                        encodedMipmapData[i] = EncodeRgbaTexture(ImageHelper.Resize(sourceImage, size, size));
                    }
                }

                // Encode & return the texture
                return EncodeRgbaTexture(sourceImage);
            }

            return pixelCodec.Encode(pixelData, Width, Height);
        }

        private byte[] EncodeRgbaTexture<TPixel>(Image<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var pixelData = ImageHelper.GetPixelDataAsBytes(image.Frames.RootFrame);
            return pixelCodec.Encode(pixelData, image.Width, image.Height);
        }

        private byte[] EncodeIndexedTexture<TPixel>(Image<TPixel> image, IQuantizer<TPixel> quantizer)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var imageFrame = quantizer.QuantizeFrame(image.Frames.RootFrame, new Rectangle(0, 0, image.Width, image.Height));
            var pixelData = ImageHelper.GetPixelDataAsBytes(imageFrame);
            return pixelCodec.Encode(pixelData, image.Width, image.Height);
        }

        /// <summary>
        /// Encodes the palette.
        /// </summary>
        /// <returns></returns>
        private byte[] EncodePalette(ReadOnlyMemory<Bgra32> palette)
        {
            var paletteData = MemoryMarshal.AsBytes(palette.Span).ToArray();

            if (NeedsExternalPalette)
            {
                return paletteCodec.Encode(paletteData);
            }

            var bytesPerPixel = paletteCodec.BitsPerPixel / 8;
            var encodedPaletteData = new byte[pixelCodec.PaletteEntries * bytesPerPixel];

            return paletteCodec.Encode(paletteData, encodedPaletteData);
        }

        /// <summary>
        /// Gets the palette that was created after encoding.
        /// </summary>
        /// <remarks>
        /// This property will be <see langword="null"/> until <see cref="Save(string)"/> or <see cref="Save(Stream)"/> is invoked
        /// and <see cref="NeedsExternalPalette"/> is <see langword="true"/>.
        /// </remarks>
        public GvrPaletteEncoder Palette { get; private set; }

        /// <summary>
        /// Saves the encoded texture to the specified file.
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

            // Get the expected length of the texture data including palette and mipmaps.
            var expectedLength = encodedTextureData.Length;
            if (encodedPaletteData != null)
            {
                expectedLength += encodedPaletteData.Length;
            }
            if (encodedMipmapData != null)
            {
                expectedLength += encodedMipmapData.Sum(x => Math.Max(x.Length, 32));
            }

            // Write out the GBIX or GCIX header if a global index is present.
            if (GlobalIndex != null && (GlobalIndexType == GvrGbixType.Gbix || GlobalIndexType == GvrGbixType.Gcix))
            {
                if (GlobalIndexType == GvrGbixType.Gbix)
                {
                    writer.Write(gbixMagicCode);
                }
                else if (GlobalIndexType == GvrGbixType.Gcix)
                {
                    writer.Write(gcixMagicCode);
                }
                writer.WriteInt32(8); // Length of the GBIX or GCIX chunk minus 8. Always 8.
                writer.WriteUInt32BigEndian(GlobalIndex.Value);
                writer.WriteInt32(0); // Always 0.
            }

            // Write out the PVRT header
            writer.Write(gvrtMagicCode);
            writer.WriteInt32(expectedLength + 8); // Length of the PVRT chunk minus 8.
            writer.WriteInt16(0); // Always 0.
            writer.WriteByte((byte)(((byte)PaletteFormat.GetValueOrDefault() << 4) | ((byte)Flags & 0xF)));
            writer.WriteByte((byte)PixelFormat);
            
            writer.WriteUInt16BigEndian((ushort)Width);
            writer.WriteUInt16BigEndian((ushort)Height);

            // Write out the palette if an internal palette is present.
            if (encodedPaletteData != null)
            {
                writer.Write(encodedPaletteData);
            }

            // Write out the texture data.
            writer.Write(encodedTextureData);

            // Write out the mipmaps if present.
            if (encodedMipmapData != null)
            {
                foreach (var encodedMipmapDataItem in encodedMipmapData)
                {
                    writer.Write(encodedMipmapDataItem);

                    // Each mipmap must be at least 32 bytes in length. Pad the end of each as needed.
                    if (encodedMipmapDataItem.Length < 32)
                    {
                        writer.Write(new byte[32 - encodedMipmapDataItem.Length]);
                    }
                }
            }
        }
        #endregion
    }
}