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
            public bool DecodeTexture(byte[] VrData, string ClutFile, out MemoryStream BitmapData)
            {
                BitmapData = null; // Set the bitmap data to null for now

                // Load the Gvr texture
                GvrTexture GvrTexture = new GvrTexture(VrData);
                if (!GvrTexture.LoadSuccess())
                {
                    Console.WriteLine("ERROR: Unsupported textue format or unable to load texture.");
                    return false;
                }

                // Set the external clut file
                if (GvrTexture.NeedsExternalClut())
                {
                    if (ClutFile == null || !File.Exists(ClutFile))
                    {
                        Console.WriteLine("ERROR: Texture needs an external clut file.");
                        return false;
                    }
                    GvpClut GvpClut = new GvpClut(ClutFile);
                    if (!GvpClut.LoadSuccess())
                    {
                        Console.WriteLine("ERROR: Unable to load clut file.");
                        return false;
                    }
                    GvrTexture.SetClut(GvpClut);
                }

                // Output information to the console
                GvrTextureInfo TextureInfo = (GvrTextureInfo)GvrTexture.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Gvr");
                Console.WriteLine("Dimensions   : {0}x{1}", TextureInfo.TextureWidth, TextureInfo.TextureHeight);
                if (TextureInfo.PixelFormat != (byte)GvrPixelFormat.Unknown)
                    Console.WriteLine("Pixel Format : {0} ({1})", TextureInfo.PixelFormat.ToString("X2"), GetPixelFormatAsText(TextureInfo.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", TextureInfo.DataFormat.ToString("X2"), GetDataFormatAsText(TextureInfo.DataFormat));
                if (TextureInfo.DataFlags != 0x00)
                    Console.WriteLine("Data Flags   : {0} ({1})", TextureInfo.DataFlags.ToString("X2"), GetDataFlagsAsText(TextureInfo.DataFlags));
                Console.WriteLine();

                // Decode the texture
                try { BitmapData = GvrTexture.GetTextureAsStream(); }
                catch
                {
                    Console.WriteLine("ERROR: Unable to decode texture.");
                    return false;
                }

                return true;
            }

            private string GetPixelFormatAsText(byte PixelFormat)
            {
                switch ((GvrPixelFormat)PixelFormat)
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
            private string GetDataFormatAsText(byte DataFormat)
            {
                switch ((GvrDataFormat)DataFormat)
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
            private string GetDataFlagsAsText(byte DataFlags)
            {
                List<string> Flags = new List<string>();

                if ((DataFlags & 0x01) != 0) // Contains Mipmaps
                    Flags.Add("Mipmaps");
                if ((DataFlags & 0x02) != 0) // External Clut
                    Flags.Add("External Clut");
                if ((DataFlags & 0x08) != 0) // Internal Clut
                    Flags.Add("Internal Clut");

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
            public bool DecodeTexture(byte[] VrData, string ClutFile, out MemoryStream BitmapData)
            {
                BitmapData = null; // Set the bitmap data to null for now

                // Load the Pvr texture
                PvrTexture PvrTexture = new PvrTexture(VrData);
                if (!PvrTexture.LoadSuccess())
                {
                    Console.WriteLine("ERROR: Unsupported textue format or unable to load texture.");
                    return false;
                }

                // Set the external clut file
                if (PvrTexture.NeedsExternalClut())
                {
                    if (ClutFile == null || !File.Exists(ClutFile))
                    {
                        Console.WriteLine("ERROR: Texture needs an external clut file.");
                        return false;
                    }
                    PvpClut PvpClut = new PvpClut(ClutFile);
                    if (!PvpClut.LoadSuccess())
                    {
                        Console.WriteLine("ERROR: Unable to load clut file.");
                        return false;
                    }
                    PvrTexture.SetClut(PvpClut);
                }

                // Output information to the console
                PvrTextureInfo TextureInfo = (PvrTextureInfo)PvrTexture.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Pvr");
                if (TextureInfo.CompressionFormat != PvrCompressionFormat.None)
                    Console.WriteLine("Compression  : {0}",   TextureInfo.CompressionFormat);
                Console.WriteLine("Dimensions   : {0}x{1}",   TextureInfo.TextureWidth, TextureInfo.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", TextureInfo.PixelFormat.ToString("X2"), GetPixelFormatAsText(TextureInfo.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", TextureInfo.DataFormat.ToString("X2"),  GetDataFormatAsText(TextureInfo.DataFormat));
                Console.WriteLine();

                // Decode the texture
                try { BitmapData = PvrTexture.GetTextureAsStream(); }
                catch
                {
                    Console.WriteLine("ERROR: Unable to decode texture.");
                    return false;
                }

                return true;
            }

            private string GetPixelFormatAsText(byte PixelFormat)
            {
                switch ((PvrPixelFormat)PixelFormat)
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
            private string GetDataFormatAsText(byte DataFormat)
            {
                switch ((PvrDataFormat)DataFormat)
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
        }
        #endregion

        #region Svr Texture Decoder
        // Svr Texture Decoder
        public class Svr : VrDecoder
        {
            public bool DecodeTexture(byte[] VrData, string ClutFile, out MemoryStream BitmapData)
            {
                BitmapData = null; // Set the bitmap data to null for now

                // Load the Svr texture
                SvrTexture SvrTexture = new SvrTexture(VrData);
                if (!SvrTexture.LoadSuccess())
                {
                    Console.WriteLine("ERROR: Unsupported textue format or unable to load texture.");
                    return false;
                }

                // Set the external clut file
                if (SvrTexture.NeedsExternalClut())
                {
                    if (ClutFile == null || !File.Exists(ClutFile))
                    {
                        Console.WriteLine("ERROR: Texture needs an external clut file.");
                        return false;
                    }
                    SvpClut SvpClut = new SvpClut(ClutFile);
                    if (!SvpClut.LoadSuccess())
                    {
                        Console.WriteLine("ERROR: Unable to load clut file.");
                        return false;
                    }
                    SvrTexture.SetClut(SvpClut);
                }

                // Output information to the console
                SvrTextureInfo TextureInfo = (SvrTextureInfo)SvrTexture.GetTextureInfo();
                Console.WriteLine();
                Console.WriteLine("Texture Type : Svr");
                Console.WriteLine("Dimensions   : {0}x{1}",   TextureInfo.TextureWidth, TextureInfo.TextureHeight);
                Console.WriteLine("Pixel Format : {0} ({1})", TextureInfo.PixelFormat.ToString("X2"), GetPixelFormatAsText(TextureInfo.PixelFormat));
                Console.WriteLine("Data Format  : {0} ({1})", TextureInfo.DataFormat.ToString("X2"),  GetDataFormatAsText(TextureInfo.DataFormat));
                Console.WriteLine();

                // Decode the texture
                try { BitmapData = SvrTexture.GetTextureAsStream(); }
                catch
                {
                    Console.WriteLine("ERROR: Unable to decode texture.");
                    return false;
                }

                return true;
            }

            private string GetPixelFormatAsText(byte PixelFormat)
            {
                switch ((SvrPixelFormat)PixelFormat)
                {
                    case SvrPixelFormat.Rgb5a3:
                        return "Rgb5a3";
                    case SvrPixelFormat.Argb8888:
                        return "Argb8888";
                }

                return String.Empty;
            }
            private string GetDataFormatAsText(byte DataFormat)
            {
                switch ((SvrDataFormat)DataFormat)
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
        }
        #endregion
    }
}