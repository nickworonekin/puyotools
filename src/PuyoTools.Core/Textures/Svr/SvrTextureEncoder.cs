using PuyoTools.Core.Textures.Svr.DataCodecs;
using PuyoTools.Core.Textures.Svr.PixelCodecs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PuyoTools.Core.Textures.Svr
{
    public class SvrTextureEncoder
    {
        #region Fields
        private PixelCodec pixelCodec; // Pixel codec
        private DataCodec dataCodec;   // Data codec

        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] gbixMagicCode = { (byte)'G', (byte)'B', (byte)'I', (byte)'X' };
        private static readonly byte[] pvrtMagicCode = { (byte)'P', (byte)'V', (byte)'R', (byte)'T' };

        private byte[] encodedPaletteData;
        private byte[] encodedTextureData;

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
        /// Gets the pixel format.
        /// </summary>
        public SvrPixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// Gets the data format.
        /// </summary>
        public SvrDataFormat DataFormat { get; private set; }

        /// <summary>
        /// Gets or sets the global index. If <see langword="null"/>, the GBIX header will not be written.
        /// </summary>
        public uint? GlobalIndex { get; set; }


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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(string file, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
        {
            using (var stream = File.OpenRead(file))
            {
                Initialize(stream, pixelFormat, dataFormat);
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(Stream source, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
        {
            Initialize(source, pixelFormat, dataFormat);
        }

        private void Initialize(Stream source, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
        {
            // Set the pixel and data formats, and verify that we can encode to them.
            // Unlike with the decoder, an exception will be thrown here if a codec cannot be used to encode them.
            PixelFormat = pixelFormat;
            DataFormat = dataFormat;

            pixelCodec = PixelCodecFactory.Create(pixelFormat);
            if (pixelCodec is null)
            {
                throw new NotSupportedException($"Pixel format {PixelFormat:X} is not supported for encoding.");
            }
            if (!pixelCodec.CanEncode)
            {
                throw new NotSupportedException($"Pixel format {PixelFormat} is not supported for encoding.");
            }

            dataCodec = DataCodecFactory.Create(dataFormat, pixelCodec);
            if (dataCodec is null)
            {
                throw new NotSupportedException($"Data format {DataFormat:X} is not supported for encoding.");
            }
            if (!dataCodec.CanEncode)
            {
                throw new NotSupportedException($"Data format {DataFormat} is not supported for encoding.");
            }

            // Get the number of palette entries.
            paletteEntries = dataCodec.PaletteEntries;

            // Read the image.
            sourceImage = Image.Load<Bgra32>(source);

            Width = sourceImage.Width;
            Height = sourceImage.Height;

            // Set the correct data format (it's ok to do it after getting the codecs).
            if (dataFormat == SvrDataFormat.Index4Rgb5a3Rectangle
                || dataFormat == SvrDataFormat.Index4Rgb5a3Square
                || dataFormat == SvrDataFormat.Index4Argb8Rectangle
                || dataFormat == SvrDataFormat.Index4Argb8Square)
            {
                if (Width == Height) // Square texture
                {
                    if (pixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        dataFormat = SvrDataFormat.Index4Rgb5a3Square;
                    }
                    else
                    {
                        dataFormat = SvrDataFormat.Index4Argb8Square;
                    }
                }
                else // Rectangular texture
                {
                    if (pixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        dataFormat = SvrDataFormat.Index4Rgb5a3Rectangle;
                    }
                    else
                    {
                        dataFormat = SvrDataFormat.Index4Argb8Rectangle;
                    }
                }

                DataFormat = dataFormat;
            }

            else if (dataFormat == SvrDataFormat.Index8Rgb5a3Rectangle
                || dataFormat == SvrDataFormat.Index8Rgb5a3Square
                || dataFormat == SvrDataFormat.Index8Argb8Rectangle
                || dataFormat == SvrDataFormat.Index8Argb8Square)
            {
                if (Width == Height) // Square texture
                {
                    if (pixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        dataFormat = SvrDataFormat.Index8Rgb5a3Square;
                    }
                    else
                    {
                        dataFormat = SvrDataFormat.Index8Argb8Square;
                    }
                }
                else // Rectangular texture
                {
                    if (pixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        dataFormat = SvrDataFormat.Index8Rgb5a3Rectangle;
                    }
                    else
                    {
                        dataFormat = SvrDataFormat.Index8Argb8Rectangle;
                    }
                }

                DataFormat = dataFormat;
            }
        }
        #endregion

        #region Encode Texture
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
                if (dataCodec.NeedsExternalPalette)
                {
                    Palette = new SvrPaletteEncoder(this, EncodePalette(imageFrame.Palette), imageFrame.Palette.Length);
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
                pixelData = ImageHelper.GetPixelDataAsBytes(sourceImage.Frames.RootFrame);
            }

            return dataCodec.Encode(pixelData, Width, Height);
        }

        /// <summary>
        /// Encodes the palette.
        /// </summary>
        /// <returns></returns>
        private byte[] EncodePalette(ReadOnlyMemory<Bgra32> palette)
        {
            var bytesPerPixel = pixelCodec.BitsPerPixel / 8;
            var paletteData = MemoryMarshal.AsBytes(palette.Span).ToArray();
            var encodedPaletteData = new byte[dataCodec.PaletteEntries * bytesPerPixel];

            for (var i = 0; i < palette.Length; i++)
            {
                pixelCodec.EncodePixel(paletteData, 4 * i, encodedPaletteData, i * bytesPerPixel);
            }

            return encodedPaletteData;
        }

        /// <summary>
        /// Gets if an external palette file will be created after encoding.
        /// </summary>
        public bool NeedsExternalPalette => dataCodec.NeedsExternalPalette;

        /// <summary>
        /// Gets the palette that was created after encoding.
        /// </summary>
        /// <remarks>
        /// This property will be <see langword="null"/> until <see cref="Save(string)"/> or <see cref="Save(Stream)"/> is invoked
        /// and <see cref="NeedsExternalPalette"/> is <see langword="true"/>.
        /// </remarks>
        public SvrPaletteEncoder Palette { get; private set; }

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

            // Write out the GBIX header if a global index is present.
            if (GlobalIndex != null)
            {
                writer.Write(gbixMagicCode);
                writer.WriteInt32(8); // Length of the GBIX chunk minus 8. Always 8.
                writer.WriteUInt32(GlobalIndex.Value);
                writer.WriteInt32(0); // Always 0.
            }

            // Write out the PVRT header
            writer.Write(pvrtMagicCode);
            writer.WriteInt32(expectedLength + 8); // Length of the PVRT chunk minus 8.
            writer.WriteByte((byte)PixelFormat);
            writer.WriteByte((byte)DataFormat);
            writer.WriteInt16(0); // Always 0.
            writer.WriteUInt16((ushort)Width);
            writer.WriteUInt16((ushort)Height);

            // Write out the palette if an internal palette is present.
            if (encodedPaletteData != null)
            {
                writer.Write(encodedPaletteData);
            }

            // Write out the texture data.
            writer.Write(DataSwizzler.Swizzle(encodedTextureData, Width, Height, dataCodec.BitsPerPixel));
        }
        #endregion
    }
}