using System;
using System.IO;

using VrSharp.PvrTexture;

namespace VrConvert
{
    public static class PvrConvert
    {
        public static void Decode(string[] args)
        {
            string inPalettePath = Path.ChangeExtension(args[1], ".pvp");
            string outPath = Path.ChangeExtension(args[1], ".png");

            // Get arguments
            for (int i = 2; i < args.Length; i++)
            {
                string arg = args[i].ToLower();

                // Change the output path
                if (arg == "-o" && args.Length > i + 1)
                {
                    outPath = args[i + 1];
                    i++;
                }

                // Palette path
                if (arg == "-p" && args.Length > i + 1)
                {
                    inPalettePath = args[i + 1];
                    i++;
                }
            }

            PvrTexture texture = new PvrTexture(args[1]);

            // Was this texture initalized successfully
            if (!texture.Initalized)
            {
                Console.WriteLine("Error: This is not a valid PVR texture, or it is an unsupported one.");
                return;
            }

            // Does this texture need an external palette file?
            if (texture.NeedsExternalPalette)
            {
                if (!File.Exists(inPalettePath))
                {
                    Console.WriteLine("Error: This texture requires an external palette file.");
                    return;
                }

                PvpPalette palette = new PvpPalette(inPalettePath);
                if (!palette.Initalized)
                {
                    Console.WriteLine("Error: {0} is not a valid PVR palette file.", inPalettePath);
                }

                texture.SetPalette(palette);
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format       : PVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions   : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            Console.WriteLine("Pixel Format : {0}", PixelFormatToString(texture.PixelFormat));
            Console.WriteLine("Data Format  : {0}", DataFormatToString(texture.DataFormat));
            if (texture.CompressionFormat != PvrCompressionFormat.None)
            {
                Console.WriteLine("Compression  : {0}", CompressionFormatToString(texture.CompressionFormat));
            }

            texture.Save(outPath);

            Console.WriteLine("\nTexture decoded successfully.");
        }

        public static void Encode(string[] args)
        {
            string outPath = Path.ChangeExtension(args[1], ".pvr");
            string outPalettePath = Path.ChangeExtension(args[1], ".pvp");

            PvrTextureEncoder texture = new PvrTextureEncoder(args[1], StringToPixelFormat(args[3]), StringToDataFormat(args[4]));

            // Was this texture initalized successfully
            if (!texture.Initalized)
            {
                Console.WriteLine("Error: Unable to encode using the specified texture.");
                return;
            }

            // Get arguments
            for (int i = 5; i < args.Length; i++)
            {
                string arg = args[i].ToLower();

                // Change the output path
                if (arg == "-o" && args.Length > i + 1)
                {
                    outPath = args[i + 1];
                    i++;
                }

                // Change the output path for the palette
                if (arg == "-op" && args.Length > i + 1)
                {
                    outPalettePath = args[i + 1];
                    i++;
                }

                // No global index
                if (arg == "-nogbix")
                {
                    texture.HasGlobalIndex = false;
                }

                // Set global index
                if (arg == "-gi" && args.Length > i + 1)
                {
                    uint globalIndex;
                    if (!uint.TryParse(args[i + 1], out globalIndex))
                    {
                        globalIndex = 0;
                    }
                    texture.GlobalIndex = globalIndex;

                    i++;
                }

                // Set compression format
                if (arg == "-cmp" && args.Length > i + 1)
                {
                    texture.CompressionFormat = StringToCompressionFormat(args[i + 1]);
                    i++;
                }
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format       : PVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions   : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            Console.WriteLine("Pixel Format : {0}", PixelFormatToString(texture.PixelFormat));
            Console.WriteLine("Data Format  : {0}", DataFormatToString(texture.DataFormat));
            if (texture.CompressionFormat != PvrCompressionFormat.None)
            {
                Console.WriteLine("Compression  : {0}", CompressionFormatToString(texture.CompressionFormat));
            }

            texture.Save(outPath);

            if (texture.NeedsExternalPalette)
            {
                texture.PaletteEncoder.Save(outPalettePath);
            }

            Console.WriteLine("\nTexture encoded successfully.");
        }

        public static void Information(string[] args)
        {
            PvrTexture texture = new PvrTexture(args[1]);

            // Was this texture initalized successfully
            if (!texture.Initalized)
            {
                Console.WriteLine("Error: This is not a valid PVR texture, or it is an unsupported one.");
                return;
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format       : PVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions   : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            Console.WriteLine("Pixel Format : {0}", PixelFormatToString(texture.PixelFormat));
            Console.WriteLine("Data Format  : {0}", DataFormatToString(texture.DataFormat));
            if (texture.CompressionFormat != PvrCompressionFormat.None)
            {
                Console.WriteLine("Compression  : {0}", CompressionFormatToString(texture.CompressionFormat));
            }
        }

        private static string PixelFormatToString(PvrPixelFormat format)
        {
            switch (format)
            {
                case PvrPixelFormat.Argb1555: return "ARGB1555";
                case PvrPixelFormat.Rgb565:   return "RGB565";
                case PvrPixelFormat.Argb4444: return "ARGB4444";
            }

            return "Unknown";
        }
        private static PvrPixelFormat StringToPixelFormat(string format)
        {
            switch (format.ToLower())
            {
                case "argb1555": return PvrPixelFormat.Argb1555;
                case "rgb565":   return PvrPixelFormat.Rgb565;
                case "argb4444": return PvrPixelFormat.Argb4444;
            }

            return PvrPixelFormat.Unknown;
        }

        private static string DataFormatToString(PvrDataFormat format)
        {
            switch (format)
            {
                case PvrDataFormat.SquareTwiddled:           return "Square Twiddled";
                case PvrDataFormat.SquareTwiddledMipmaps:    return "Square Twiddled with Mipmaps";
                case PvrDataFormat.Vq:                       return "VQ";
                case PvrDataFormat.VqMipmaps:                return "VQ with Mipmaps";
                case PvrDataFormat.Index4:                   return "4-bit Indexed with External Palette";
                case PvrDataFormat.Index8:                   return "8-bit Indexed with External Palette";
                case PvrDataFormat.Rectangle:                return "Rectangle";
                case PvrDataFormat.RectangleTwiddled:        return "Rectangle Twiddled";
                case PvrDataFormat.SmallVq:                  return "Small VQ";
                case PvrDataFormat.SmallVqMipmaps:           return "Small VQ with Mipmaps";
                case PvrDataFormat.SquareTwiddledMipmapsAlt: return "Square Twiddled with Mipmaps (Alternate)";
            }

            return "Unknown";
        }
        private static PvrDataFormat StringToDataFormat(string format)
        {
            switch (format.ToLower())
            {
                case "square":            return PvrDataFormat.SquareTwiddled;
                case "squaremipmaps":     return PvrDataFormat.SquareTwiddledMipmaps;
                case "vq":                return PvrDataFormat.Vq;
                case "vqmipmaps":         return PvrDataFormat.VqMipmaps;
                case "index4":            return PvrDataFormat.Index4;
                case "index8":            return PvrDataFormat.Index8;
                case "rectangle":         return PvrDataFormat.Rectangle;
                case "rectangletwiddled": return PvrDataFormat.RectangleTwiddled;
                case "smallvq":           return PvrDataFormat.SmallVq;
                case "smallvqmipmaps":    return PvrDataFormat.SmallVqMipmaps;
                case "squaremipmapsalt":  return PvrDataFormat.SquareTwiddledMipmapsAlt;
            }

            return PvrDataFormat.Unknown;
        }

        private static string CompressionFormatToString(PvrCompressionFormat format)
        {
            switch (format)
            {
                case PvrCompressionFormat.Rle: return "RLE";
            }

            return "None";
        }
        private static PvrCompressionFormat StringToCompressionFormat(string format)
        {
            switch (format.ToLower())
            {
                case "rle": return PvrCompressionFormat.Rle;
            }

            return PvrCompressionFormat.None;
        }
    }
}
