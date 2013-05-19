using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;
using VrSharp.SvrTexture;

namespace VrConvert
{
    public class VrEncoder
    {
        #region Gvr Texture Encoder
        // Gvr Texture Encoder
        public class Gvr : VrEncoder
        {
            public bool EncodeTexture(byte[] BitmapData, string PixelFormatText, string DataFormatText, bool IncludeGI, uint GlobalIndex, out MemoryStream TextureData, out MemoryStream ClutData)
            {
                TextureData = null; // Set texture data to null
                ClutData    = null; // Set external clut data to null

                // Get the Pixel and Data Formats
                GvrPixelFormat PixelFormat = GetPixelFormat(PixelFormatText);
                GvrDataFormat DataFormat   = GetDataFormat(DataFormatText);
                if ((PixelFormat == GvrPixelFormat.Unknown && (DataFormat == GvrDataFormat.Index4 || DataFormat == GvrDataFormat.Index8)) || DataFormat == GvrDataFormat.Unknown)
                {
                    Console.WriteLine("ERROR: Unknown pixel or data format.");
                    return false;
                }
                if (PixelFormat == GvrPixelFormat.Unknown && DataFormat != GvrDataFormat.Index4 && DataFormat != GvrDataFormat.Index8)
                    PixelFormat = GvrPixelFormat.IntensityA8; // Just so it gets set to 00.

                // Load the bitmap
                GvrTextureEncoder GvrTextureEncoder = new GvrTextureEncoder(BitmapData, (GvrPixelFormat)PixelFormat, (GvrDataFormat)DataFormat);
                if (!GvrTextureEncoder.LoadSuccess())
                {
                    Console.WriteLine("ERROR: Unable to load image, file is not a supported image,");
                    Console.WriteLine("       or image cannot be converted to the specified pixel/data formats.");
                    return false;
                }
                //GvrTextureEncoder.WriteGbix(GlobalIndex);

                // Output information to the console
                /*
                GvrTextureInfo TextureInfo = (GvrTextureInfo)GvrTextureEncoder.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Gvr");
                Console.WriteLine("Dimensions   : {0}x{1}", TextureInfo.TextureWidth, TextureInfo.TextureHeight);
                if (TextureInfo.PixelFormat != (byte)GvrPixelFormat.Unknown)
                    Console.WriteLine("Pixel Format : {0} ({1})", TextureInfo.PixelFormat.ToString("X2"), GetPixelFormatAsText(TextureInfo.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", TextureInfo.DataFormat.ToString("X2"), GetDataFormatAsText(TextureInfo.DataFormat));
                if (TextureInfo.DataFlags != 0x00)
                    Console.WriteLine("Data Flags   : {0} ({1})", TextureInfo.DataFlags.ToString("X2"), GetDataFlagsAsText(TextureInfo.DataFlags));
                Console.WriteLine();
                 */
                //GvrTextureInfo TextureInfo = (GvrTextureInfo)GvrTextureEncoder.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Gvr");
                Console.WriteLine("Dimensions   : {0}x{1}", GvrTextureEncoder.TextureWidth, GvrTextureEncoder.TextureHeight);
                if (GvrTextureEncoder.PixelFormat != GvrPixelFormat.Unknown)
                    Console.WriteLine("Pixel Format : {0} ({1})", GvrTextureEncoder.PixelFormat, GetPixelFormatAsText(GvrTextureEncoder.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", GvrTextureEncoder.DataFormat, GetDataFormatAsText(GvrTextureEncoder.DataFormat));
                if (GvrTextureEncoder.DataFlags != 0x00)
                    Console.WriteLine("Data Flags   : {0} ({1})", GvrTextureEncoder.DataFlags, GetDataFlagsAsText(GvrTextureEncoder.DataFlags));
                Console.WriteLine();

                // Encode the texture
                try { TextureData = GvrTextureEncoder.GetTextureAsStream(); }
                catch
                {
                    Console.WriteLine("ERROR: Unable to encode texture.");
                    return false;
                }

                // Encode the clut (if it has one)
                if (GvrTextureEncoder.NeedsExternalClut())
                {
                    try { ClutData = GvrTextureEncoder.GetClutAsStream(); }
                    catch
                    {
                        Console.WriteLine("ERROR: Unable to encode clut.");
                        return false;
                    }
                }

                return true;
            }

            private string GetPixelFormatAsText(GvrPixelFormat PixelFormat)
            {
                switch (PixelFormat)
                {
                    case GvrPixelFormat.IntensityA8:
                        return "Intensity 8-bit w/ Alpha";
                    case GvrPixelFormat.Rgb565:
                        return "Rgb565";
                    case GvrPixelFormat.Rgb5a3:
                        return "Rgb5a3";
                }

                return String.Empty;
            }
            private string GetDataFormatAsText(GvrDataFormat DataFormat)
            {
                switch (DataFormat)
                {
                    case GvrDataFormat.Intensity4:
                        return "Intensity 4-bit";
                    case GvrDataFormat.Intensity8:
                        return "Intensity 8-bit";
                    case GvrDataFormat.IntensityA4:
                        return "Intensity 4-bit w/ Alpha";
                    case GvrDataFormat.IntensityA8:
                        return "Intensity 8-bit w/ Alpha";
                    case GvrDataFormat.Rgb565:
                        return "Rgb565";
                    case GvrDataFormat.Rgb5a3:
                        return "Rgb5a3";
                    case GvrDataFormat.Argb8888:
                        return "Argb8888";
                    case GvrDataFormat.Index4:
                        return "4-bit Indexed";
                    case GvrDataFormat.Index8:
                        return "8-bit Indexed";
                    case GvrDataFormat.Dxt1:
                        return "Dxt1 Compressed";
                }

                return String.Empty;
            }
            private string GetDataFlagsAsText(GvrDataFlags DataFlags)
            {
                List<string> Flags = new List<string>();

                if ((DataFlags & GvrDataFlags.Mipmaps) != 0) // Contains Mipmaps
                    Flags.Add("Mipmaps");
                if ((DataFlags & GvrDataFlags.ExternalClut) != 0) // External Clut
                    Flags.Add("External Clut");
                if ((DataFlags & GvrDataFlags.InternalClut) != 0) // Internal Clut
                    Flags.Add("Internal Clut");

                if (Flags.Count == 0)
                    return String.Empty;

                return String.Join(", ", Flags.ToArray());
            }

            private GvrPixelFormat GetPixelFormat(string format)
            {
                switch (format.ToLower())
                {
                    case "ia8": case "00":
                        return GvrPixelFormat.IntensityA8;
                    case "rgb565": case "01":
                        return GvrPixelFormat.Rgb565;
                    case "rgb5a3": case "02":
                        return GvrPixelFormat.Rgb5a3;
                }

                return GvrPixelFormat.Unknown; // Unknown format
            }

            private GvrDataFormat GetDataFormat(string format)
            {
                switch (format.ToLower())
                {
                    case "i4": case "00":
                        return GvrDataFormat.Intensity4;
                    case "i8": case "01":
                        return GvrDataFormat.Intensity8;
                    case "ia4": case "02":
                        return GvrDataFormat.IntensityA4;
                    case "ia8": case "03":
                        return GvrDataFormat.IntensityA8;
                    case "rgb565": case "04":
                        return GvrDataFormat.Rgb565;
                    case "rgb5a3": case "05":
                        return GvrDataFormat.Rgb5a3;
                    case "argb8888": case "06":
                        return GvrDataFormat.Argb8888;
                    case "index4": case "08":
                        return GvrDataFormat.Index4;
                    case "index8": case "09":
                        return GvrDataFormat.Index8;
                    case "cmp": case "0E":
                        return GvrDataFormat.Dxt1;
                }

                return GvrDataFormat.Unknown; // Unknown format
            }
        }
        #endregion

        #region Pvr Texture Encoder
        // Pvr Texture Encoder
        public class Pvr : VrEncoder
        {
            public bool EncodeTexture(byte[] BitmapData, string PixelFormatText, string DataFormatText, string CompressionFormatText, bool IncludeGI, uint GlobalIndex, out MemoryStream TextureData, out MemoryStream ClutData)
            {
                TextureData = null; // Set texture data to null
                ClutData    = null; // Set external clut data to null

                // Get the Pixel, Data, and Compression Formats
                PvrPixelFormat PixelFormat = GetPixelFormat(PixelFormatText);
                PvrDataFormat DataFormat   = GetDataFormat(DataFormatText);
                PvrCompressionFormat CompressionFormat = GetCompressionFormat(CompressionFormatText);
                if (PixelFormat == PvrPixelFormat.Unknown || DataFormat == PvrDataFormat.Unknown)
                {
                    Console.WriteLine("ERROR: Unknown pixel or data format.");
                    return false;
                }

                // Load the bitmap
                PvrTextureEncoder PvrTextureEncoder = new PvrTextureEncoder(BitmapData, (PvrPixelFormat)PixelFormat, (PvrDataFormat)DataFormat);
                if (!PvrTextureEncoder.LoadSuccess())
                {
                    Console.WriteLine("ERROR: Unable to load image, file is not a supported image,");
                    Console.WriteLine("       or image cannot be converted to the specified pixel/data formats.");
                    return false;
                }
                //PvrTextureEncoder.WriteGbix(GlobalIndex);
                if (CompressionFormat != PvrCompressionFormat.None)
                    PvrTextureEncoder.SetCompressionFormat(CompressionFormat);

                // Output information to the console
                /*
                PvrTextureInfo TextureInfo = (PvrTextureInfo)PvrTextureEncoder.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Pvr");
                if (TextureInfo.CompressionFormat != PvrCompressionFormat.None)
                    Console.WriteLine("Compression  : {0}",   TextureInfo.CompressionFormat);
                Console.WriteLine("Dimensions   : {0}x{1}",   TextureInfo.TextureWidth, TextureInfo.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", TextureInfo.PixelFormat.ToString("X2"), GetPixelFormatAsText(TextureInfo.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", TextureInfo.DataFormat.ToString("X2"),  GetDataFormatAsText(TextureInfo.DataFormat));
                Console.WriteLine();
                 */
                //PvrTextureInfo TextureInfo = (PvrTextureInfo)PvrTextureEncoder.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Pvr");
                if (PvrTextureEncoder.CompressionFormat != PvrCompressionFormat.None)
                    Console.WriteLine("Compression  : {0}", PvrTextureEncoder.CompressionFormat);
                Console.WriteLine("Dimensions   : {0}x{1}", PvrTextureEncoder.TextureWidth, PvrTextureEncoder.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", PvrTextureEncoder.PixelFormat, GetPixelFormatAsText(PvrTextureEncoder.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", PvrTextureEncoder.DataFormat, GetDataFormatAsText(PvrTextureEncoder.DataFormat));
                Console.WriteLine();

                // Encode the texture
                try { TextureData = PvrTextureEncoder.GetTextureAsStream(); }
                catch
                {
                    Console.WriteLine("ERROR: Unable to encode texture.");
                    return false;
                }

                // Encode the clut (if it has one)
                if (PvrTextureEncoder.NeedsExternalClut())
                {
                    try { ClutData = PvrTextureEncoder.GetClutAsStream(); }
                    catch
                    {
                        Console.WriteLine("ERROR: Unable to encode clut.");
                        return false;
                    }
                }

                return true;
            }

            private string GetPixelFormatAsText(PvrPixelFormat PixelFormat)
            {
                switch (PixelFormat)
                {
                    case PvrPixelFormat.Argb1555:
                        return "Argb1555";
                    case PvrPixelFormat.Rgb565:
                        return "Rgb565";
                    case PvrPixelFormat.Argb4444:
                        return "Argb4444";
                }

                return String.Empty;
            }
            private string GetDataFormatAsText(PvrDataFormat DataFormat)
            {
                switch (DataFormat)
                {
                    case PvrDataFormat.SquareTwiddled:
                        return "Square Twiddled";
                    case PvrDataFormat.SquareTwiddledMipmaps:
                    case PvrDataFormat.SquareTwiddledMipmapsDup:
                        return "Square Twiddled w/ Mipmaps";
                    case PvrDataFormat.Vq:
                        return "Vq";
                    case PvrDataFormat.VqMipmaps:
                        return "Vq w/ Mipmaps";
                    case PvrDataFormat.Index4:
                        return "4-bit Indexed w/ External Clut";
                    case PvrDataFormat.Index8:
                        return "8-bit Indexed w/ External Clut";
                    case PvrDataFormat.Rectangle:
                        return "Rectangle";
                    case PvrDataFormat.RectangleTwiddled:
                        return "Rectangle Twiddled";
                    case PvrDataFormat.SmallVq:
                        return "Small Vq";
                    case PvrDataFormat.SmallVqMipmaps:
                        return "Small Vq w/ Mipmaps";
                }

                return String.Empty;
            }

            private PvrPixelFormat GetPixelFormat(string format)
            {
                switch (format.ToLower())
                {
                    case "argb1555": case "00":
                        return PvrPixelFormat.Argb1555;
                    case "rgb565": case "01":
                        return PvrPixelFormat.Rgb565;
                    case "argb4444": case "02":
                        return PvrPixelFormat.Argb4444;
                }

                return PvrPixelFormat.Unknown; // Unknown format
            }

            private PvrDataFormat GetDataFormat(string format)
            {
                switch (format.ToLower())
                {
                    case "sqr": case "01":
                        return PvrDataFormat.SquareTwiddled;
                    case "index4": case "05":
                        return PvrDataFormat.Index4;
                    case "index8": case "07":
                        return PvrDataFormat.Index8;
                    case "rect": case "09":
                        return PvrDataFormat.Rectangle;
                    case "recttwiddled": case "0d":
                        return PvrDataFormat.RectangleTwiddled;
                }

                return PvrDataFormat.Unknown; // Unknown format
            }

            private PvrCompressionFormat GetCompressionFormat(string format)
            {
                if (format == null)
                    return PvrCompressionFormat.None;

                switch (format.ToLower())
                {
                    case "rle":
                        return PvrCompressionFormat.Rle;
                    case "none":
                        return PvrCompressionFormat.None;
                }

                return PvrCompressionFormat.None; // No Compression
            }
        }
        #endregion

        #region Svr Texture Encoder
        // Svr Texture Encoder
        public class Svr : VrEncoder
        {
            public bool EncodeTexture(byte[] BitmapData, string PixelFormatText, string DataFormatText, bool IncludeGI, uint GlobalIndex, out MemoryStream TextureData, out MemoryStream ClutData)
            {
                TextureData = null; // Set texture data to null
                ClutData    = null; // Set external clut data to null

                // We need the width and height, so we will convert it to a bitmap
                Bitmap BitmapBmp = null;
                try { BitmapBmp = new Bitmap(new MemoryStream(BitmapData)); }
                catch
                {
                    Console.WriteLine("ERROR: Input file is not a valid or supported image.");
                    return false;
                }

                // Get the Pixel and Data Formats
                SvrPixelFormat PixelFormat = GetPixelFormat(PixelFormatText);
                SvrDataFormat DataFormat   = GetDataFormat(DataFormatText, PixelFormat, BitmapBmp.Width, BitmapBmp.Height);
                if (PixelFormat == SvrPixelFormat.Unknown || DataFormat == SvrDataFormat.Unknown)
                {
                    Console.WriteLine("ERROR: Unknown pixel or data format.");
                    return false;
                }

                // Load the bitmap
                SvrTextureEncoder SvrTextureEncoder = new SvrTextureEncoder(BitmapBmp, (SvrPixelFormat)PixelFormat, (SvrDataFormat)DataFormat);
                if (!SvrTextureEncoder.LoadSuccess())
                {
                    Console.WriteLine("ERROR: Unable to load image, file is not a supported image,");
                    Console.WriteLine("       or image cannot be converted to the specified pixel/data formats.");
                    return false;
                }
                //SvrTextureEncoder.WriteGbix(GlobalIndex);

                // Output information to the console
                /*
                SvrTextureInfo TextureInfo = (SvrTextureInfo)SvrTextureEncoder.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Svr");
                Console.WriteLine("Dimensions   : {0}x{1}",   TextureInfo.TextureWidth, TextureInfo.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", TextureInfo.PixelFormat.ToString("X2"), GetPixelFormatAsText(TextureInfo.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", TextureInfo.DataFormat.ToString("X2"),  GetDataFormatAsText(TextureInfo.DataFormat));
                Console.WriteLine();
                 */
                //SvrTextureInfo TextureInfo = (SvrTextureInfo)SvrTextureEncoder.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Svr");
                Console.WriteLine("Dimensions   : {0}x{1}", SvrTextureEncoder.TextureWidth, SvrTextureEncoder.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", SvrTextureEncoder.PixelFormat, GetPixelFormatAsText(SvrTextureEncoder.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", SvrTextureEncoder.DataFormat, GetDataFormatAsText(SvrTextureEncoder.DataFormat));
                Console.WriteLine();

                // Encode the texture
                try { TextureData = SvrTextureEncoder.GetTextureAsStream(); }
                catch
                {
                    Console.WriteLine("ERROR: Unable to encode texture.");
                    return false;
                }

                // Encode the clut (if it has one)
                if (SvrTextureEncoder.NeedsExternalClut())
                {
                    try { ClutData = SvrTextureEncoder.GetClutAsStream(); }
                    catch
                    {
                        Console.WriteLine("ERROR: Unable to encode clut.");
                        return false;
                    }
                }

                return true;
            }

            private string GetPixelFormatAsText(SvrPixelFormat PixelFormat)
            {
                switch (PixelFormat)
                {
                    case SvrPixelFormat.Rgb5a3:
                        return "Rgb5a3";
                    case SvrPixelFormat.Argb8888:
                        return "Argb8888";
                }

                return String.Empty;
            }
            private string GetDataFormatAsText(SvrDataFormat DataFormat)
            {
                switch (DataFormat)
                {
                    case SvrDataFormat.Rectangle:
                        return "Rectangle";
                    case SvrDataFormat.Index4ExtClut:
                        return "4-bit Indexed w/ External Clut";
                    case SvrDataFormat.Index8ExtClut:
                        return "8-bit Indexed w/ External Clut";
                    case SvrDataFormat.Index4RectRgb5a3:
                        return "4-bit Indexed Rectangle w/ Rgb5a3";
                    case SvrDataFormat.Index4SqrRgb5a3:
                        return "4-bit Indexed Square w/ Rgb5a3";
                    case SvrDataFormat.Index4RectArgb8:
                        return "4-bit Indexed Rectangle w/ Argb8888";
                    case SvrDataFormat.Index4SqrArgb8:
                        return "4-bit Indexed Square w/ Argb8888";
                    case SvrDataFormat.Index8RectRgb5a3:
                        return "8-bit Indexed Rectangle w/ Rgb5a3";
                    case SvrDataFormat.Index8SqrRgb5a3:
                        return "8-bit Indexed Square w/ Rgb5a3";
                    case SvrDataFormat.Index8RectArgb8:
                        return "8-bit Indexed Rectangle w/ Argb8888";
                    case SvrDataFormat.Index8SqrArgb8:
                        return "8-bit Indexed Square w/ Argb8888";
                }

                return String.Empty;
            }

            private SvrPixelFormat GetPixelFormat(string format)
            {
                switch (format.ToLower())
                {
                    case "rgb5a3": case "08":
                        return SvrPixelFormat.Rgb5a3;
                    case "argb8888": case "09":
                        return SvrPixelFormat.Argb8888;
                }

                return SvrPixelFormat.Unknown; // Unknown format
            }

            private SvrDataFormat GetDataFormat(string format, SvrPixelFormat? PixelFormat, int width, int height)
            {
                switch (format.ToLower())
                {
                    case "rect": case "60":
                        return SvrDataFormat.Rectangle;
                    case "index4ec": case "62":
                        return SvrDataFormat.Index4ExtClut;
                    case "index8ec": case "64":
                        return SvrDataFormat.Index8ExtClut;
                    case "index4":
                        if (PixelFormat == SvrPixelFormat.Rgb5a3)
                        {
                            if (width == height) return SvrDataFormat.Index4SqrRgb5a3;
                            else return SvrDataFormat.Index4RectRgb5a3;
                        }
                        else if (PixelFormat == SvrPixelFormat.Argb8888)
                        {
                            if (width == height) return SvrDataFormat.Index4SqrArgb8;
                            else return SvrDataFormat.Index4RectArgb8;
                        }

                        break;
                    case "index8":
                        if (PixelFormat == SvrPixelFormat.Rgb5a3)
                        {
                            if (width == height) return SvrDataFormat.Index8SqrRgb5a3;
                            else return SvrDataFormat.Index8RectRgb5a3;
                        }
                        else if (PixelFormat == SvrPixelFormat.Argb8888)
                        {
                            if (width == height) return SvrDataFormat.Index8SqrArgb8;
                            else return SvrDataFormat.Index8RectArgb8;
                        }

                        break;
                    case "66": return SvrDataFormat.Index4RectRgb5a3;
                    case "67": return SvrDataFormat.Index4SqrRgb5a3;
                    case "68": return SvrDataFormat.Index4RectArgb8;
                    case "69": return SvrDataFormat.Index4SqrArgb8;
                    case "6a": return SvrDataFormat.Index8RectRgb5a3;
                    case "6b": return SvrDataFormat.Index8SqrRgb5a3;
                    case "6c": return SvrDataFormat.Index8RectArgb8;
                    case "6d": return SvrDataFormat.Index8SqrArgb8;
                }

                return SvrDataFormat.Unknown; // Unknown format
            }
        }
        #endregion
    }
}