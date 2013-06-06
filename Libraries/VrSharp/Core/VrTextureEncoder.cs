using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using nQuant;

namespace VrSharp
{
    public abstract class VrTextureEncoder
    {
        #region Fields
        protected bool initalized = false; // Is the texture initalized?

        protected byte[] decodedData; // Decoded texture data (either 32-bit RGBA or 8-bit indexed)
        protected Bitmap decodedBitmap; // Decoded bitmap

        protected VrPixelCodec pixelCodec; // Pixel codec
        protected VrDataCodec dataCodec;   // Data codec

        protected byte[][] texturePalette; // The texture's palette
        #endregion

        #region Get Texture
        /// <summary>
        /// Returns the encoded texture as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            return EncodeTexture().ToArray();
        }

        /// <summary>
        /// Returns the encoded texture as a stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            return EncodeTexture();
        }

        /// <summary>
        /// Saves the encoded texture to the specified path.
        /// </summary>
        /// <param name="path">Name of the file to save the data to.</param>
        public void Save(string path)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            using (FileStream destination = File.Create(path))
            {
                MemoryStream textureStream = EncodeTexture();
                PTStream.CopyTo(textureStream, destination);
            }
        }

        /// <summary>
        /// Saves the encoded texture to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot encode this texture as it is not initalized.");
            }

            MemoryStream textureStream = EncodeTexture();
            PTStream.CopyTo(textureStream, destination);
        }
        #endregion

        #region Palette
        /// <summary>
        /// Returns if the texture needs an external palette file.
        /// </summary>
        public virtual bool NeedsExternalPalette
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataCodec.NeedsExternalPalette;
            }
        }

        /// <summary>
        /// Returns the palette encoder if this texture uses an external palette file.
        /// </summary>
        public VpPaletteEncoder PaletteEncoder
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return paletteEncoder;
            }
        }
        protected VpPaletteEncoder paletteEncoder;
        #endregion

        #region Private Methods
        protected abstract MemoryStream EncodeTexture();
        /*
        // Palettize the bitmap used for the raw texture data
        // Make sure you test to see if DataCodec.GetNumClutEntries != 0
        protected void PalettizeBitmap()
        {
            // We only need to convert it to a palletized 8-bit texture if it is not yet already one.
            if (decodedBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // If it is not a 32-bit ARGB image, convert it to one.
                if (decodedBitmap.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    Bitmap newBitmap = new Bitmap(decodedBitmap.Width, decodedBitmap.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(newBitmap))
                    {
                        g.DrawImage(decodedBitmap, new Rectangle(0, 0, decodedBitmap.Width, decodedBitmap.Height));
                    }
                    decodedBitmap = newBitmap;
                }

                // This quantizer only works with 32-bit ARGB images
                WuQuantizer quantizer = new WuQuantizer();
                decodedBitmap = (Bitmap)quantizer.QuantizeImage(decodedBitmap, dataCodec.PaletteEntries);
                decodedData = ConvertBitmapToIndex(decodedBitmap);
            }

            // Build the palette
            texturePalette = new byte[dataCodec.PaletteEntries][];
            for (int i = 0; i < dataCodec.PaletteEntries; i++)
            {
                texturePalette[i] = new byte[4];
                texturePalette[i][3] = decodedBitmap.Palette.Entries[i].A;
                texturePalette[i][2] = decodedBitmap.Palette.Entries[i].R;
                texturePalette[i][1] = decodedBitmap.Palette.Entries[i].G;
                texturePalette[i][0] = decodedBitmap.Palette.Entries[i].B;
            }

            // If the texture requires an external palette file, now is the time to create it
            if (dataCodec.NeedsExternalPalette)
            {
                CreateVpClut(pixelCodec.EncodePalette(texturePalette, dataCodec.PaletteEntries), (ushort)dataCodec.PaletteEntries);
            }
        }*/

        // Returns if the texture contains mipmaps
        protected virtual bool ContainsMipmaps()
        {
            return dataCodec.ContainsMipmaps;
        }
        /*
        // Converts a bitmap to a raw Argb8888 array
        private byte[] ConvertBitmapToRaw(Bitmap bitmap)
        {
            byte[] output = new byte[bitmap.Width * bitmap.Height * 4];

            // What we are going to do here is draw the old bitmap on a 32bit bitmap
            // and then do a Marshal.Copy to get the data. It's much faster then using
            // the GetPixel function even though this method is more complicated.
            // Note: we have to use System.Drawing.Imaging.PixelFormat as PixelFormat is already defined.
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newBitmap))
                g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);

            BitmapData BitmapData = newBitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, newBitmap.PixelFormat);
            Marshal.Copy(BitmapData.Scan0, output, 0, output.Length);

            return output;
        }

        // Converts an 8-bit indexed bitmap to an indexed array
        private unsafe byte[] ConvertBitmapToIndex(Bitmap bitmap)
        {
            // Note that the bitmap needs to be an 8-bit indexed image for this to work
            byte[] output = new byte[bitmap.Width * bitmap.Height];
            BitmapData BitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            byte* ImagePointer = (byte*)BitmapData.Scan0;

            for (int y = 0; y < BitmapData.Height; y++)
            {
                for (int x = 0; x < BitmapData.Width; x++)
                    output[(y * bitmap.Width) + x] = ImagePointer[(y * BitmapData.Stride) + x];
            }

            bitmap.UnlockBits(BitmapData);
            return output;
        }*/

        // Returns if the texture dimensuons are valid
        private bool HasValidDimensions(int width, int height)
        {
            if (width < 4 || height < 4 || width > 1024 || height > 1024)
                return false;

            if ((width & (width - 1)) != 0 || (height & (height - 1)) != 0)
                return false;

            return true;
        }
        #endregion

        #region Constructors & Initalizers
        public VrTextureEncoder(string file)
        {
            Initalize(new Bitmap(file));
        }

        public VrTextureEncoder(byte[] source) : this(source, 0, source.Length) { }

        public VrTextureEncoder(byte[] source, int offset, int length)
        {
            MemoryStream buffer = new MemoryStream();
            buffer.Write(source, offset, length);

            Initalize(new Bitmap(buffer));
        }

        public VrTextureEncoder(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        public VrTextureEncoder(Stream source, int length)
        {
            MemoryStream buffer = new MemoryStream();
            PTStream.CopyPartTo(source, buffer, length);

            Initalize(new Bitmap(buffer));
        }

        public VrTextureEncoder(Bitmap source)
        {
            Initalize(source);
        }

        private void Initalize(Bitmap source)
        {
            // Make sure this bitmap's dimensions are valid
            if (!HasValidDimensions(source.Width, source.Height))
                return;

            try
            {
                decodedBitmap = source;

                textureWidth  = (ushort)source.Width;
                textureHeight = (ushort)source.Height;
            }
            catch
            {
                decodedBitmap = null;

                textureWidth  = 0;
                textureHeight = 0;
            }
        }

        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool Initalized
        {
            get { return initalized; }
        }
        #endregion

        #region Texture Properties
        /// <summary>
        /// Indicates whether or not to include the GBIX header in the texture. The default value is true.
        /// </summary>
        public bool IncludeGbixHeader
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return includeGbixHeader;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                includeGbixHeader = value;
            }
        }
        protected bool includeGbixHeader;

        /// <summary>
        /// Sets the texture's global index. This only matters if IncludeGbixHeader is true. The default value is 0.
        /// </summary>
        public uint GlobalIndex
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return globalIndex;
            }
            set
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                globalIndex = value;
            }
        }
        protected uint globalIndex;

        /// <summary>
        /// Width of the texture (in pixels).
        /// </summary>
        public ushort TextureWidth
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureWidth;
            }
        }
        protected ushort textureWidth;

        /// <summary>
        /// Height of the texture (in pixels).
        /// </summary>
        public ushort TextureHeight
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureHeight;
            }
        }
        protected ushort textureHeight;
        #endregion

        #region Texture Conversion
        protected byte[] BitmapToRaw(Bitmap source)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height * 4];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                img = newImage;
            }

            // Copy over the data to the destination. It's ok to do it without utilizing Stride
            // since each pixel takes up 4 bytes (aka Stride will always be equal to Width)
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            Marshal.Copy(bitmapData.Scan0, destination, 0, destination.Length);
            img.UnlockBits(bitmapData);

            return destination;
        }

        protected unsafe byte[] BitmapToRawIndexed(Bitmap source, int maxColors, out byte[][] palette)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }
                img = newImage;
            }

            // Quantize the image
            WuQuantizer quantizer = new WuQuantizer();
            img = (Bitmap)quantizer.QuantizeImage(img, maxColors);

            // Copy over the data to the destination. We need to use Stride in this case, as it may not
            // always be equal to Width.
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

            byte* pointer = (byte*)bitmapData.Scan0;
            for (int y = 0; y < bitmapData.Height; y++)
            {
                for (int x = 0; x < bitmapData.Width; x++)
                {
                    destination[(y * img.Width) + x] = pointer[(y * bitmapData.Stride) + x];
                }
            }

            img.UnlockBits(bitmapData);

            // Copy over the palette
            palette = new byte[maxColors][];
            for (int i = 0; i < maxColors; i++)
            {
                palette[i] = new byte[4];

                palette[i][3] = img.Palette.Entries[i].A;
                palette[i][2] = img.Palette.Entries[i].R;
                palette[i][1] = img.Palette.Entries[i].G;
                palette[i][0] = img.Palette.Entries[i].B;
            }

            return destination;
        }
        #endregion
    }
}