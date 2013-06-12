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
            public bool EncodeTexture(byte[] BitmapData, string PixelFormatText, string DataFormatText, bool IncludeGI, uint GlobalIndex, bool gcix, bool hasMipmaps, bool hasExternalPalette, out MemoryStream TextureData, out MemoryStream PaletteData)
            {
                TextureData = null; // Set texture data to null
                PaletteData    = null; // Set external palette data to null

                // Get the Pixel and Data Formats
                GvrPixelFormat PixelFormat = GetPixelFormat(PixelFormatText);
                GvrDataFormat DataFormat   = GetDataFormat(DataFormatText);
                if ((PixelFormat == GvrPixelFormat.Unknown && (DataFormat == GvrDataFormat.Index4 || DataFormat == GvrDataFormat.Index8)) || DataFormat == GvrDataFormat.Unknown)
                {
                    Console.WriteLine("ERROR: Unknown pixel or data format.");
                    return false;
                }
                //if (PixelFormat == GvrPixelFormat.Unknown && DataFormat != GvrDataFormat.Index4 && DataFormat != GvrDataFormat.Index8)
                //    PixelFormat = GvrPixelFormat.IntensityA8; // Just so it gets set to 00.

                // Load the bitmap
                GvrTextureEncoder GvrTextureEncoder = new GvrTextureEncoder(BitmapData, (GvrPixelFormat)PixelFormat, (GvrDataFormat)DataFormat);
                if (!GvrTextureEncoder.Initalized)
                {
                    Console.WriteLine("ERROR: Unable to load image, file is not a supported image,");
                    Console.WriteLine("       or image cannot be converted to the specified pixel/data formats.");
                    return false;
                }
                //GvrTextureEncoder.WriteGbix(GlobalIndex);

                GvrTextureEncoder.HasGlobalIndex = IncludeGI;
                if (IncludeGI)
                {
                    GvrTextureEncoder.GlobalIndex = GlobalIndex;

                    if (gcix)
                    {
                        GvrTextureEncoder.GbixType = GvrGbixType.Gcix;
                    }
                }
                if (hasMipmaps)
                {
                    GvrTextureEncoder.DataFlags |= GvrDataFlags.Mipmaps;
                }
                if (hasExternalPalette)
                {
                    GvrTextureEncoder.DataFlags |= GvrDataFlags.ExternalPalette;
                }

                // Output information to the console
                Console.WriteLine();
                Console.WriteLine("Texture Type : GVR");
                Console.WriteLine("Dimensions   : {0}x{1}", GvrTextureEncoder.TextureWidth, GvrTextureEncoder.TextureHeight);
                if (GvrTextureEncoder.PixelFormat != GvrPixelFormat.Unknown)
                    Console.WriteLine("Pixel Format : {0} ({1})", ((byte)GvrTextureEncoder.PixelFormat).ToString("X2"), GetPixelFormatAsText(GvrTextureEncoder.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", ((byte)GvrTextureEncoder.DataFormat).ToString("X2"), GetDataFormatAsText(GvrTextureEncoder.DataFormat));
                if (GvrTextureEncoder.DataFlags != 0x00)
                    Console.WriteLine("Data Flags   : {0} ({1})", ((byte)GvrTextureEncoder.DataFlags).ToString("X2"), GetDataFlagsAsText(GvrTextureEncoder.DataFlags));
                Console.WriteLine();

                // Encode the texture
                try { TextureData = GvrTextureEncoder.ToStream(); }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to encode texture. The error returned was:\n{0}", e.Message);
                    return false;
                }

                // Encode the palette (if it has one)
                if (GvrTextureEncoder.NeedsExternalPalette)
                {
                    try { PaletteData = GvrTextureEncoder.PaletteEncoder.ToStream(); }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: Unable to encode palette. The error returned was:\n{0}", e.Message);
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
                        return "S3TC/DXT1 Compressed";
                }

                return String.Empty;
            }
            private string GetDataFlagsAsText(GvrDataFlags DataFlags)
            {
                List<string> Flags = new List<string>();

                if ((DataFlags & GvrDataFlags.Mipmaps) != 0) // Contains Mipmaps
                    Flags.Add("Mipmaps");
                if ((DataFlags & GvrDataFlags.ExternalPalette) != 0) // External Palette
                    Flags.Add("External Palette");
                if ((DataFlags & GvrDataFlags.InternalPalette) != 0) // Internal Palette
                    Flags.Add("Internal Palette");

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
                    case "dxt1": case "0E":
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
            public bool EncodeTexture(byte[] BitmapData, string PixelFormatText, string DataFormatText, string CompressionFormatText, bool IncludeGI, uint GlobalIndex, out MemoryStream TextureData, out MemoryStream PaletteData)
            {
                TextureData = null; // Set texture data to null
                PaletteData    = null; // Set external palette data to null

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
                if (!PvrTextureEncoder.Initalized)
                {
                    Console.WriteLine("ERROR: Unable to load image, file is not a supported image,");
                    Console.WriteLine("       or image cannot be converted to the specified pixel/data formats.");
                    return false;
                }
                //PvrTextureEncoder.WriteGbix(GlobalIndex);

                PvrTextureEncoder.HasGlobalIndex = IncludeGI;
                if (IncludeGI)
                {
                    PvrTextureEncoder.GlobalIndex = GlobalIndex;
                }

                if (CompressionFormat != PvrCompressionFormat.None)
                    PvrTextureEncoder.CompressionFormat = CompressionFormat;
                    //PvrTextureEncoder.SetCompressionFormat(CompressionFormat);

                // Output information to the console
                Console.WriteLine();
                Console.WriteLine("Texture Type : PVR");
                if (PvrTextureEncoder.CompressionFormat != PvrCompressionFormat.None)
                    Console.WriteLine("Compression  : {0}", GetCompressionFormatAsText(PvrTextureEncoder.CompressionFormat));
                Console.WriteLine("Dimensions   : {0}x{1}", PvrTextureEncoder.TextureWidth, PvrTextureEncoder.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", ((byte)PvrTextureEncoder.PixelFormat).ToString("X2"), GetPixelFormatAsText(PvrTextureEncoder.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", ((byte)PvrTextureEncoder.DataFormat).ToString("X2"), GetDataFormatAsText(PvrTextureEncoder.DataFormat));
                Console.WriteLine();

                // Encode the texture
                try { TextureData = PvrTextureEncoder.ToStream(); }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to encode texture. The error returned was:\n{0}", e.Message);
                    return false;
                }

                // Encode the palette (if it has one)
                if (PvrTextureEncoder.NeedsExternalPalette)
                {
                    try { PaletteData = PvrTextureEncoder.PaletteEncoder.ToStream(); }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: Unable to encode palette. The error returned was:\n{0}", e.Message);
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
                        return "Square Twiddled w/ Mipmaps";
                    case PvrDataFormat.Vq:
                        return "VQ";
                    case PvrDataFormat.VqMipmaps:
                        return "VQ w/ Mipmaps";
                    case PvrDataFormat.Index4:
                        return "4-bit Indexed w/ External Palette";
                    case PvrDataFormat.Index8:
                        return "8-bit Indexed w/ External Palette";
                    case PvrDataFormat.Rectangle:
                        return "Rectangle";
                    case PvrDataFormat.RectangleTwiddled:
                        return "Rectangle Twiddled";
                    case PvrDataFormat.SmallVq:
                        return "Small VQ";
                    case PvrDataFormat.SmallVqMipmaps:
                        return "Small VQ w/ Mipmaps";
                    case PvrDataFormat.SquareTwiddledMipmapsAlt:
                        return "Square Twiddled w/ Mipmaps (Alternate)";
                }

                return String.Empty;
            }
            private string GetCompressionFormatAsText(PvrCompressionFormat compressionFormat)
            {
                switch (compressionFormat)
                {
                    case PvrCompressionFormat.Rle: return "RLE";
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
                    case "square": case "01":
                        return PvrDataFormat.SquareTwiddled;
                    case "squaremipmaps": case "02":
                        return PvrDataFormat.SquareTwiddledMipmaps;
                    case "index4": case "05":
                        return PvrDataFormat.Index4;
                    case "index8": case "07":
                        return PvrDataFormat.Index8;
                    case "rectangle": case "09":
                        return PvrDataFormat.Rectangle;
                    case "rectangletwiddled": case "0d":
                        return PvrDataFormat.RectangleTwiddled;
                    case "squaremipmapsalt": case "12":
                        return PvrDataFormat.SquareTwiddledMipmapsAlt;
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
            public bool EncodeTexture(byte[] BitmapData, string PixelFormatText, string DataFormatText, bool IncludeGI, uint GlobalIndex, out MemoryStream TextureData, out MemoryStream PaletteData)
            {
                TextureData = null; // Set texture data to null
                PaletteData    = null; // Set external palette data to null

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
                if (!SvrTextureEncoder.Initalized)
                {
                    Console.WriteLine("ERROR: Unable to load image, file is not a supported image,");
                    Console.WriteLine("       or image cannot be converted to the specified pixel/data formats.");
                    return false;
                }
                //SvrTextureEncoder.WriteGbix(GlobalIndex);

                SvrTextureEncoder.HasGlobalIndex = IncludeGI;
                if (IncludeGI)
                {
                    SvrTextureEncoder.GlobalIndex = GlobalIndex;
                }

                // Output information to the console
                Console.WriteLine();
                Console.WriteLine("Texture Type : SVR");
                Console.WriteLine("Dimensions   : {0}x{1}", SvrTextureEncoder.TextureWidth, SvrTextureEncoder.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", ((byte)SvrTextureEncoder.PixelFormat).ToString("X2"), GetPixelFormatAsText(SvrTextureEncoder.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", ((byte)SvrTextureEncoder.DataFormat).ToString("X2"), GetDataFormatAsText(SvrTextureEncoder.DataFormat));
                Console.WriteLine();

                // Encode the texture
                try { TextureData = SvrTextureEncoder.ToStream(); }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to encode texture. The error returned was:\n{0}", e.Message);
                    return false;
                }

                // Encode the palette (if it has one)
                if (SvrTextureEncoder.NeedsExternalPalette)
                {
                    try { PaletteData = SvrTextureEncoder.PaletteEncoder.ToStream(); }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: Unable to encode palette. The error returned was:\n{0}", e.Message);
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
                    case SvrDataFormat.Index4ExternalPalette:
                        return "4-bit Indexed w/ External Palette";
                    case SvrDataFormat.Index8ExternalPalette:
                        return "8-bit Indexed w/ External Palette";
                    case SvrDataFormat.Index4Rgb5a3Rectangle:
                        return "4-bit Indexed Rectangle w/ Rgb5a3";
                    case SvrDataFormat.Index4Rgb5a3Square:
                        return "4-bit Indexed Square w/ Rgb5a3";
                    case SvrDataFormat.Index4Argb8Rectangle:
                        return "4-bit Indexed Rectangle w/ Argb8888";
                    case SvrDataFormat.Index4Argb8Square:
                        return "4-bit Indexed Square w/ Argb8888";
                    case SvrDataFormat.Index8Rgb5a3Rectangle:
                        return "8-bit Indexed Rectangle w/ Rgb5a3";
                    case SvrDataFormat.Index8Rgb5a3Square:
                        return "8-bit Indexed Square w/ Rgb5a3";
                    case SvrDataFormat.Index8Argb8Rectangle:
                        return "8-bit Indexed Rectangle w/ Argb8888";
                    case SvrDataFormat.Index8Argb8Square:
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
                    case "rectangle": case "60":
                        return SvrDataFormat.Rectangle;
                    case "index4ep": case "62":
                        return SvrDataFormat.Index4ExternalPalette;
                    case "index8ep": case "64":
                        return SvrDataFormat.Index8ExternalPalette;
                    case "index4":
                    case "66":
                    case "67":
                    case "68":
                    case "69":
                        // Just pass in any of the Index4 family.
                        // VrSharp will pick the correct one
                        return SvrDataFormat.Index4Rgb5a3Rectangle;
                        /*
                        if (PixelFormat == SvrPixelFormat.Rgb5a3)
                        {
                            if (width == height) return SvrDataFormat.Index4Rgb5a3Square;
                            else return SvrDataFormat.Index4Rgb5a3Rectangle;
                        }
                        else if (PixelFormat == SvrPixelFormat.Argb8888)
                        {
                            if (width == height) return SvrDataFormat.Index4Argb8Square;
                            else return SvrDataFormat.Index4Argb8Rectangle;
                        }
                        break;*/
                    case "index8":
                    case "6a":
                    case "6b":
                    case "6c":
                    case "6d":
                        // Just pass in any of the Index4 family.
                        // VrSharp will pick the correct one
                        return SvrDataFormat.Index8Rgb5a3Rectangle;
                        /*
                        if (PixelFormat == SvrPixelFormat.Rgb5a3)
                        {
                            if (width == height) return SvrDataFormat.Index8Rgb5a3Square;
                            else return SvrDataFormat.Index8Rgb5a3Rectangle;
                        }
                        else if (PixelFormat == SvrPixelFormat.Argb8888)
                        {
                            if (width == height) return SvrDataFormat.Index8Argb8Square;
                            else return SvrDataFormat.Index8Argb8Rectangle;
                        }
                        break;*/
                        /*
                    case "66": return SvrDataFormat.Index4Rgb5a3Rectangle;
                    case "67": return SvrDataFormat.Index4Rgb5a3Square;
                    case "68": return SvrDataFormat.Index4Argb8Rectangle;
                    case "69": return SvrDataFormat.Index4Argb8Square;
                    case "6a": return SvrDataFormat.Index8Rgb5a3Rectangle;
                    case "6b": return SvrDataFormat.Index8Rgb5a3Square;
                    case "6c": return SvrDataFormat.Index8Argb8Rectangle;
                    case "6d": return SvrDataFormat.Index8Argb8Square;*/
                }

                return SvrDataFormat.Unknown; // Unknown format
            }
        }
        #endregion
    }
}