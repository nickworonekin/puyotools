using System;
using System.IO;
using System.Collections.Generic;
using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;
using VrSharp.SvrTexture;

namespace VrConvert
{
    public class VrDecoder
    {
        #region Gvr Texture Decoder
        // Gvr Texture Decoder
        public class Gvr : VrDecoder
        {
            public bool DecodeTexture(byte[] VrData, string PaletteFile, out MemoryStream BitmapData)
            {
                BitmapData = null; // Set the bitmap data to null for now

                // Load the Gvr texture
                GvrTexture GvrTexture = new GvrTexture(VrData);
                if (!GvrTexture.Initalized)
                {
                    Console.WriteLine("ERROR: Unsupported textue format or unable to load texture.");
                    return false;
                }

                // Set the external palette file
                if (GvrTexture.NeedsExternalPalette)
                {
                    if (PaletteFile == null || !File.Exists(PaletteFile))
                    {
                        Console.WriteLine("ERROR: Texture needs an external palette file.");
                        return false;
                    }
                    GvpPalette GvpPalette = new GvpPalette(PaletteFile);
                    if (!GvpPalette.Initalized)
                    {
                        Console.WriteLine("ERROR: Unable to load palette file.");
                        return false;
                    }
                    GvrTexture.SetPalette(GvpPalette);
                }

                // Output information to the console
                Console.WriteLine();
                Console.WriteLine("Texture Type : GVR");
                Console.WriteLine("Dimensions   : {0}x{1}", GvrTexture.TextureWidth, GvrTexture.TextureHeight);
                if (GvrTexture.PixelFormat != GvrPixelFormat.Unknown)
                    Console.WriteLine("Pixel Format : {0} ({1})", ((byte)GvrTexture.PixelFormat).ToString("X2"), GetPixelFormatAsText(GvrTexture.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", ((byte)GvrTexture.DataFormat).ToString("X2"), GetDataFormatAsText(GvrTexture.DataFormat));
                if (GvrTexture.DataFlags != GvrDataFlags.None)
                    Console.WriteLine("Data Flags   : {0} ({1})", ((byte)GvrTexture.DataFlags).ToString("X2"), GetDataFlagsAsText(GvrTexture.DataFlags));
                Console.WriteLine();

                // Decode the texture
                try { BitmapData = GvrTexture.ToStream(); }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to decode texture. The error returned was:\n{0}", e.Message);
                    return false;
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
        }
        #endregion

        #region Pvr Texture Decoder
        // Pvr Texture Decoder
        public class Pvr : VrDecoder
        {
            public bool DecodeTexture(byte[] VrData, string PaletteFile, out MemoryStream BitmapData)
            {
                BitmapData = null; // Set the bitmap data to null for now

                // Load the Pvr texture
                PvrTexture PvrTexture = new PvrTexture(VrData);
                if (!PvrTexture.Initalized)
                {
                    Console.WriteLine("ERROR: Unsupported textue format or unable to load texture.");
                    return false;
                }

                // Set the external palette file
                if (PvrTexture.NeedsExternalPalette)
                {
                    if (PaletteFile == null || !File.Exists(PaletteFile))
                    {
                        Console.WriteLine("ERROR: Texture needs an external palette file.");
                        return false;
                    }
                    PvpPalette PvpPalette = new PvpPalette(PaletteFile);
                    if (!PvpPalette.Initalized)
                    {
                        Console.WriteLine("ERROR: Unable to load palette file.");
                        return false;
                    }
                    PvrTexture.SetPalette(PvpPalette);
                }

                // Output information to the console
                Console.WriteLine();
                Console.WriteLine("Texture Type : PVR");
                if (PvrTexture.CompressionFormat != PvrCompressionFormat.None)
                    Console.WriteLine("Compression  : {0}", GetCompressionFormatAsText(PvrTexture.CompressionFormat));
                Console.WriteLine("Dimensions   : {0}x{1}", PvrTexture.TextureWidth, PvrTexture.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", ((byte)PvrTexture.PixelFormat).ToString("X2"), GetPixelFormatAsText(PvrTexture.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", ((byte)PvrTexture.DataFormat).ToString("X2"), GetDataFormatAsText(PvrTexture.DataFormat));
                Console.WriteLine();

                // Decode the texture
                try { BitmapData = PvrTexture.ToStream(); }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to decode texture. The error returned was:\n{0}", e.Message);
                    return false;
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
                    case PvrDataFormat.SquareTwiddledMipmapsAlt:
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
        }
        #endregion

        #region Svr Texture Decoder
        // Svr Texture Decoder
        public class Svr : VrDecoder
        {
            public bool DecodeTexture(byte[] VrData, string PaletteFile, out MemoryStream BitmapData)
            {
                BitmapData = null; // Set the bitmap data to null for now

                // Load the Svr texture
                SvrTexture SvrTexture = new SvrTexture(VrData);
                if (!SvrTexture.Initalized)
                {
                    Console.WriteLine("ERROR: Unsupported textue format or unable to load texture.");
                    return false;
                }

                // Set the external palette file
                if (SvrTexture.NeedsExternalPalette)
                {
                    if (PaletteFile == null || !File.Exists(PaletteFile))
                    {
                        Console.WriteLine("ERROR: Texture needs an external palette file.");
                        return false;
                    }
                    SvpPalette SvpPalette = new SvpPalette(PaletteFile);
                    if (!SvpPalette.Initalized)
                    {
                        Console.WriteLine("ERROR: Unable to load palette file.");
                        return false;
                    }
                    SvrTexture.SetPalette(SvpPalette);
                }

                // Output information to the console
                Console.WriteLine();
                Console.WriteLine("Texture Type : SVR");
                Console.WriteLine("Dimensions   : {0}x{1}", SvrTexture.TextureWidth, SvrTexture.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", ((byte)SvrTexture.PixelFormat).ToString("X2"), GetPixelFormatAsText(SvrTexture.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", ((byte)SvrTexture.DataFormat).ToString("X2"), GetDataFormatAsText(SvrTexture.DataFormat));
                Console.WriteLine();

                // Decode the texture
                try { BitmapData = SvrTexture.ToStream(); }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to decode texture. The error returned was:\n{0}", e.Message);
                    return false;
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
        }
        #endregion
    }
}