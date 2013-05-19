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
        protected bool InitSuccess = false; // Initalization

        protected byte[] RawImageData;    // Raw Image Data
        protected Bitmap BitmapImageData; // Bitmap Image (to use for mipmaps)

        //public ushort TextureWidth { get; protected set; }  // Vr Texture Width
        //public ushort TextureHeight { get; protected set; } // Vr Texture Height

        //public uint GlobalIndex { get; protected set; } // Global Index

        //protected byte PixelFormat;        // Pixel Format
        //protected byte DataFormat;         // Data Format
        protected VrPixelCodec PixelCodec; // Pixel Codec
        protected VrDataCodec DataCodec;   // Data Codec

        //protected int GbixOffset; // Gbix Offset
        //protected int PvrtOffset; // Pvrt (Gvrt) Offset
        //protected int ClutOffset; // Clut Offset
        //protected int DataOffset; // Data Offset

        //protected byte[,] TextureClut;       // Texture Clut
        protected byte[][] TextureClut;
        protected VpClutEncoder ClutEncoder; // Clut Encoder


        //public bool Initalized { get; protected set; }
        #endregion

        #region Constructors
        /*
        /// <summary>
        /// Open a bitmap from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the bitmap data.</param>
        public VrTextureEncoder(string file)
        {
            Bitmap bitmap;
            try { bitmap = new Bitmap(file); }
            catch
            {
                RawImageData = new byte[0];
                InitSuccess  = false;
                return;
            }

            TextureWidth    = (ushort)bitmap.Width;
            TextureHeight   = (ushort)bitmap.Height;
            BitmapImageData = bitmap;
            RawImageData    = ConvertBitmapToRaw(bitmap);
        }

        /// <summary>
        /// Open a bitmap from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the bitmap data.</param>
        public VrTextureEncoder(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin); // Seek to the beginning
            Bitmap bitmap;
            try { bitmap = new Bitmap(stream); }
            catch
            {
                RawImageData = new byte[0];
                InitSuccess  = false;
                return;
            }

            TextureWidth    = (ushort)bitmap.Width;
            TextureHeight   = (ushort)bitmap.Height;
            BitmapImageData = bitmap;
            RawImageData    = ConvertBitmapToRaw(bitmap);
        }

        /// <summary>
        /// Open a bitmap from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the bitmap data.</param>
        public VrTextureEncoder(byte[] array)
        {
            Bitmap bitmap;
            try { bitmap = new Bitmap(new MemoryStream(array)); }
            catch
            {
                RawImageData = new byte[0];
                InitSuccess  = false;
                return;
            }

            TextureWidth    = (ushort)bitmap.Width;
            TextureHeight   = (ushort)bitmap.Height;
            BitmapImageData = bitmap;
            RawImageData    = ConvertBitmapToRaw(bitmap);
        }

        /// <summary>
        /// Open a bitmap from a System.Drawing.Bitmap.
        /// </summary>
        /// <param name="bitmap">A System.Drawing.Bitmap instance.</param>
        public VrTextureEncoder(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                RawImageData = new byte[0];
                InitSuccess  = false;
                return;
            }

            TextureWidth    = (ushort)bitmap.Width;
            TextureHeight   = (ushort)bitmap.Height;
            BitmapImageData = bitmap;
            RawImageData    = ConvertBitmapToRaw(bitmap);
        }
         */
        #endregion

        #region Get Texture
        /// <summary>
        /// Return the texture as an array (clone of GetTextureAsArray).
        /// </summary>
        /// <returns></returns>
        public byte[] GetTexture()
        {
            return GetTextureAsArray();
        }

        /// <summary>
        /// Return the texture as an array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetTextureAsArray()
        {
            if (!InitSuccess) return null;

            return EncodeTexture().ToArray();
        }

        /// <summary>
        /// Return the texture as a memory stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetTextureAsStream()
        {
            if (!InitSuccess) return null;

            return EncodeTexture();
        }
        #endregion

        #region Clut
        /// <summary>
        /// Returns the clut as an array (clone of GetClutAsArray).
        /// </summary>
        /// <returns></returns>
        public byte[] GetClut()
        {
            if (!InitSuccess || ClutEncoder == null) return null;

            return ClutEncoder.GetClut();
        }

        /// <summary>
        /// Returns the clut as an array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetClutAsArray()
        {
            if (!InitSuccess || ClutEncoder == null) return null;

            return ClutEncoder.GetClutAsArray();
        }

        /// <summary>
        /// Returns the clut an a memory stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetClutAsStream()
        {
            if (!InitSuccess || ClutEncoder == null) return null;

            return ClutEncoder.GetClutAsStream();
        }

        /// <summary>
        /// Returns if the texture needs an external clut file.
        /// </summary>
        /// <returns></returns>
        public bool NeedsExternalClut()
        {
            if (!InitSuccess) return false;

            return TexNeedsExternalClut();
        }

        // Returns if the texture needa an external clut file (internal method)
        protected virtual bool TexNeedsExternalClut()
        {
            return DataCodec.NeedsExternalClut;
        }

        // Create a vp clut encoder for an external clut.
        protected abstract void CreateVpClut(byte[] ClutData, ushort NumClutEntries);
        #endregion

        #region Header
        /*
        /// <summary>
        /// Add or leave out the Gbix header in the texture
        /// </summary>
        /// <param name="enable">If the texture contains the Gbix header.</param>
        public void EnableGbix(bool enable)
        {
            if (!InitSuccess) return;

            if (enable)
            {
                GbixOffset = 0x00;
                PvrtOffset = 0x10;
            }
            else
            {
                GbixOffset = -1;
                PvrtOffset = 0x00;
            }
        }

        /// <summary>
        /// Write the Global Index for the texture.
        /// </summary>
        /// <param name="gbix">Global Index</param>
        public void WriteGbix(uint gbix)
        {
            if (!InitSuccess) return;

            GlobalIndex = gbix;
        }
         */
        #endregion

        #region Misc
        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool LoadSuccess()
        {
            return InitSuccess;
        }

        /// <summary>
        /// Returns information about the texture.
        /// </summary>
        /// <returns></returns>
        //public abstract VrTextureInfo GetTextureInfo();
        /*
        // Swap endian of a 16-bit unsigned integer (a ushort)
        protected ushort SwapUShort(ushort x)
        {
            return (ushort)((x << 8) | (x >> 8));
        }

        // Swap endian of a 32-bit unsigned integer (a uint)
        protected uint SwapUInt(uint x)
        {
            return (x >> 24) |
                ((x << 8) & 0x00FF0000) |
                ((x >> 8) & 0x0000FF00) |
                (x << 24);
        }

        // Does post build events on the encoded texture
        // Mainly used for compression
        protected virtual byte[] DoPostEncodeEvents(byte[] TextureData) { return TextureData; }*/
        #endregion

        #region Private Methods
        protected abstract MemoryStream EncodeTexture();

        // Encode the texture
        /*
        private byte[] EncodeTexture()
        {
            return EncodeTextureNew().ToArray();
            /*
            // Get the offsets (GbixOffset and PvrtOffset are already set)
            if (DataCodec.ClutEntries != 0 && !TexNeedsExternalClut())
            {
                ClutOffset = PvrtOffset + 0x10;
                DataOffset = ClutOffset + (DataCodec.ClutEntries * (PixelCodec.Bpp >> 3));
            }
            else
            {
                ClutOffset = -1;
                DataOffset = PvrtOffset + 0x10;
            }

            // Get the data
            byte[] VrTextureData = DataCodec.Encode(RawImageData, TextureWidth, TextureHeight, PixelCodec);
            byte[] VrPvrtHeader = WritePvrtHeader(VrTextureData.Length);
            byte[] VrGbixHeader = new byte[0];
            if (GbixOffset != -1)
                VrGbixHeader = WriteGbixHeader();
            byte[] VrClutData = new byte[0];
            if (ClutOffset != -1)
                VrClutData = PixelCodec.EncodeClut(TextureClut, DataCodec.ClutEntries);
                //VrClutData = PixelCodec.CreateClut(TextureClut);

            // Write the data
            byte[] TextureData = new byte[DataOffset + VrTextureData.Length];
            if (GbixOffset != -1)
                VrGbixHeader.CopyTo(TextureData, GbixOffset);
            VrPvrtHeader.CopyTo(TextureData, PvrtOffset);
            if (ClutOffset != -1)
                VrClutData.CopyTo(TextureData, ClutOffset);
            VrTextureData.CopyTo(TextureData, DataOffset);

            TextureData = DoPostEncodeEvents(TextureData);

            return TextureData;
        }*/

        // Palettize the bitmap used for the raw texture data
        // Make sure you test to see if DataCodec.GetNumClutEntries != 0
        protected void PalettizeBitmap()
        {
            //OctreeQuantizer Quantizer = new OctreeQuantizer(DataCodec.GetNumClutEntries() - 1, DataCodec.GetBpp(PixelCodec));
            //BitmapImageData = Quantizer.Quantize(BitmapImageData);
            //RawImageData    = ConvertBitmapToIndex(BitmapImageData); // We need to convert it to indexes instead of colors

            // We only need to convert it to a palletized 8-bit texture if it is not yet already one.
            if (BitmapImageData.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // If it is not a 32-bit ARGB image, convert it to one.
                if (BitmapImageData.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    Bitmap newBitmap = new Bitmap(BitmapImageData.Width, BitmapImageData.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(newBitmap))
                    {
                        g.DrawImage(BitmapImageData, new Rectangle(0, 0, BitmapImageData.Width, BitmapImageData.Height));
                    }
                    BitmapImageData = newBitmap;
                }

                // This quantizer only works with 32-bit ARGB images
                WuQuantizer quantizer = new WuQuantizer();
                BitmapImageData = (Bitmap)quantizer.QuantizeImage(BitmapImageData, DataCodec.ClutEntries);
                RawImageData = ConvertBitmapToIndex(BitmapImageData);
            }

            // We have a clut that we need to create
            //if (!TexNeedsExternalClut())
            //{
            //    ClutOffset = PvrtOffset + 0x10;
            //    DataOffset = ClutOffset + (DataCodec.ClutEntries * (PixelCodec.Bpp >> 3));
            //}

            // Build the clut list
            //TextureClut = new byte[DataCodec.GetNumClutEntries(), 4];
            TextureClut = new byte[DataCodec.ClutEntries][];
            for (int i = 0; i < DataCodec.ClutEntries; i++)
            {
                TextureClut[i] = new byte[4];
                TextureClut[i][3] = BitmapImageData.Palette.Entries[i].A;
                TextureClut[i][2] = BitmapImageData.Palette.Entries[i].R;
                TextureClut[i][1] = BitmapImageData.Palette.Entries[i].G;
                TextureClut[i][0] = BitmapImageData.Palette.Entries[i].B;

                /*
                TextureClut[i, 3] = BitmapImageData.Palette.Entries[i].A;
                TextureClut[i, 2] = BitmapImageData.Palette.Entries[i].R;
                TextureClut[i, 1] = BitmapImageData.Palette.Entries[i].G;
                TextureClut[i, 0] = BitmapImageData.Palette.Entries[i].B;
                 * */
            }

            //if (!TexNeedsExternalClut())
            //{
            //    ClutOffset = PvrtOffset + 0x10;
            //    DataOffset = ClutOffset + (DataCodec.ClutEntries * (PixelCodec.Bpp >> 3));
            //}

            // If the texture contains an external clut, create a vp clut encoder to write it
            if (TexNeedsExternalClut())
                CreateVpClut(PixelCodec.EncodeClut(TextureClut, DataCodec.ClutEntries), (ushort)DataCodec.ClutEntries);
                //CreateVpClut(PixelCodec.CreateClut(TextureClut), (ushort)DataCodec.GetNumClutEntries());
        }

        // Returns if the texture contains mipmaps
        protected virtual bool ContainsMipmaps()
        {
            return DataCodec.ContainsMipmaps;
        }

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
        }

        // Writes the Gbix header for a Vr Texture.
        //protected abstract byte[] WriteGbixHeader();
        // Writes the Pvrt header for a Vr Texture.
        // TextureSize includes the clut, data, and any mipmaps
        //protected abstract byte[] WritePvrtHeader(int TextureSize);
        #endregion

        #region Constructors & Initalizers
        public VrTextureEncoder(string file)
        {
            try
            {
                BitmapImageData = new Bitmap(file);

                TextureWidth  = (ushort)BitmapImageData.Width;
                TextureHeight = (ushort)BitmapImageData.Height;

                RawImageData = BitmapToRaw(BitmapImageData);
            }
            catch
            {
                BitmapImageData = null;
                RawImageData = null;
            }
        }

        public VrTextureEncoder(byte[] source)
        {
            try
            {
                BitmapImageData = new Bitmap(new MemoryStream(source));

                TextureWidth  = (ushort)BitmapImageData.Width;
                TextureHeight = (ushort)BitmapImageData.Height;

                RawImageData = BitmapToRaw(BitmapImageData);
            }
            catch
            {
                BitmapImageData = null;
                RawImageData = null;
            }
        }

        public VrTextureEncoder(byte[] source, int offset, int length)
        {
            try
            {
                MemoryStream buffer = new MemoryStream(length);
                buffer.Write(source, offset, length);
                BitmapImageData = new Bitmap(buffer);

                TextureWidth  = (ushort)BitmapImageData.Width;
                TextureHeight = (ushort)BitmapImageData.Height;

                RawImageData = BitmapToRaw(BitmapImageData);
            }
            catch
            {
                BitmapImageData = null;
                RawImageData = null;
            }
        }

        public VrTextureEncoder(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        public VrTextureEncoder(Stream source, int length)
        {
            try
            {
                MemoryStream buffer = new MemoryStream(length);
                PTStream.CopyPartTo(source, buffer, length);
                BitmapImageData = new Bitmap(buffer);

                TextureWidth  = (ushort)BitmapImageData.Width;
                TextureHeight = (ushort)BitmapImageData.Height;

                RawImageData = BitmapToRaw(BitmapImageData);
            }
            catch
            {
                BitmapImageData = null;
                RawImageData = null;
            }
        }

        public VrTextureEncoder(Bitmap source)
        {
            try
            {
                BitmapImageData = source;

                TextureWidth  = (ushort)BitmapImageData.Width;
                TextureHeight = (ushort)BitmapImageData.Height;

                RawImageData = BitmapToRaw(BitmapImageData);
            }
            catch
            {
                BitmapImageData = null;
                RawImageData = null;
            }
        }
        #endregion

        #region Texture Properties
        /// <summary>
        /// Sets the texture's global index. This only matters if IncludeGbixHeader is true. The default value is 0.
        /// </summary>
        public uint GlobalIndex;

        /// <summary>
        /// Width of the texture (in pixels).
        /// </summary>
        public ushort TextureWidth { get; protected set; }

        /// <summary>
        /// Height of the texture (in pixels).
        /// </summary>
        public ushort TextureHeight { get; protected set; }

        /// <summary>
        /// Indicates whether or not to include the GBIX header in the texture. The default value is true.
        /// </summary>
        public bool IncludeGbixHeader;
        #endregion

        #region Texture Conversion
        private byte[] BitmapToRaw(Bitmap source)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height * 4];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(img))
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

        private unsafe byte[] BitmapToRawIndexed(Bitmap source, int maxColors, out byte[][] palette)
        {
            Bitmap img = source;
            byte[] destination = new byte[img.Width * img.Height];

            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(img))
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