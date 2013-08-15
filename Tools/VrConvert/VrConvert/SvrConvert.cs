using System;
using System.IO;

using VrSharp;
using VrSharp.SvrTexture;

namespace VrConvert
{
    public static class SvrConvert
    {
        public static void Decode(string[] args)
        {
            string inPalettePath = Path.ChangeExtension(args[1], ".svp");
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

            SvrTexture texture;

            // Initalize the texture
            try
            {
                texture = new SvrTexture(args[1]);
            }
            catch (NotAValidTextureException)
            {
                Console.WriteLine("Error: This is not a valid SVR texture.");
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

                SvpPalette palette = new SvpPalette(inPalettePath);
                if (!palette.Initalized)
                {
                    Console.WriteLine("Error: {0} is not a valid SVR palette file.", inPalettePath);
                }

                texture.SetPalette(palette);
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format       : SVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions   : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            Console.WriteLine("Pixel Format : {0}", PixelFormatToString(texture.PixelFormat));
            Console.WriteLine("Data Format  : {0}", DataFormatToString(texture.DataFormat));

            // Decode the texture
            try
            {
                texture.Save(outPath);
            }
            catch (CannotDecodeTextureException)
            {
                Console.WriteLine("Error: Unable to decode this texture. The texture's pixel format or data format may not be supported.");
                return;
            }

            Console.WriteLine("\nTexture decoded successfully.");
        }

        public static void Encode(string[] args)
        {
            string outPath = Path.ChangeExtension(args[1], ".svr");
            string outPalettePath = Path.ChangeExtension(args[1], ".svp");

            SvrTextureEncoder texture = new SvrTextureEncoder(args[1], StringToPixelFormat(args[3]), StringToDataFormat(args[4]));

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
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format       : SVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions   : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            Console.WriteLine("Pixel Format : {0}", PixelFormatToString(texture.PixelFormat));
            Console.WriteLine("Data Format  : {0}", DataFormatToString(texture.DataFormat));

            texture.Save(outPath);

            if (texture.NeedsExternalPalette)
            {
                texture.PaletteEncoder.Save(outPalettePath);
            }

            Console.WriteLine("\nTexture encoded successfully.");
        }

        public static void Information(string[] args)
        {
            SvrTexture texture;

            // Initalize the texture
            try
            {
                texture = new SvrTexture(args[1]);
            }
            catch (NotAValidTextureException)
            {
                Console.WriteLine("Error: This is not a valid SVR texture.");
                return;
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format       : SVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions   : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            Console.WriteLine("Pixel Format : {0}", PixelFormatToString(texture.PixelFormat));
            Console.WriteLine("Data Format  : {0}", DataFormatToString(texture.DataFormat));
        }

        private static string PixelFormatToString(SvrPixelFormat format)
        {
            switch (format)
            {
                case SvrPixelFormat.Rgb5a3:   return "RGB5A3";
                case SvrPixelFormat.Argb8888: return "ARGB8888";
            }

            return "Unknown";
        }
        private static SvrPixelFormat StringToPixelFormat(string format)
        {
            switch (format.ToLower())
            {
                case "rgb5a3":   return SvrPixelFormat.Rgb5a3;
                case "argb8888": return SvrPixelFormat.Argb8888;
            }

            return SvrPixelFormat.Unknown;
        }

        private static string DataFormatToString(SvrDataFormat format)
        {
            switch (format)
            {
                case SvrDataFormat.Rectangle:             return "Rectangle";
                case SvrDataFormat.Index4ExternalPalette: return "4-bit Indexed with External Palette";
                case SvrDataFormat.Index8ExternalPalette: return "8-bit Indexed with External Palette";
                case SvrDataFormat.Index4Rgb5a3Rectangle:
                case SvrDataFormat.Index4Rgb5a3Square:
                case SvrDataFormat.Index4Argb8Rectangle:
                case SvrDataFormat.Index4Argb8Square:     return "4-bit Indexed";
                case SvrDataFormat.Index8Rgb5a3Rectangle:
                case SvrDataFormat.Index8Rgb5a3Square:
                case SvrDataFormat.Index8Argb8Rectangle:
                case SvrDataFormat.Index8Argb8Square:     return "8-bit Indexed";
            }

            return "Unknown";
        }
        private static SvrDataFormat StringToDataFormat(string format)
        {
            switch (format.ToLower())
            {
                case "rectangle": return SvrDataFormat.Rectangle;
                case "index4ep":  return SvrDataFormat.Index4ExternalPalette;
                case "index8ep":  return SvrDataFormat.Index8ExternalPalette;
                case "index4":    return SvrDataFormat.Index4Rgb5a3Rectangle;
                case "index8":    return SvrDataFormat.Index8Rgb5a3Rectangle;
            }

            return SvrDataFormat.Unknown;
        }
    }
}
