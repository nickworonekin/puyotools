using System;
using System.IO;

using VrSharp;
using VrSharp.GvrTexture;

namespace VrConvert
{
    public static class GvrConvert
    {
        public static void Decode(string[] args)
        {
            string inPalettePath = Path.ChangeExtension(args[1], ".gvp");
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

            GvrTexture texture;
            
            // Initalize the texture
            try
            {
                texture = new GvrTexture(args[1]);
            }
            catch (NotAValidTextureException)
            {
                Console.WriteLine("Error: This is not a valid GVR texture.");
                return;
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format         : GVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index   : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions     : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            if (texture.PixelFormat != GvrPixelFormat.Unknown)
            {
                Console.WriteLine("Palette Format : {0}", PixelFormatToString(texture.PixelFormat));
            }
            Console.WriteLine("Data Format    : {0}", DataFormatToString(texture.DataFormat));
            if (texture.DataFlags != GvrDataFlags.None)
            {
                Console.WriteLine("Data Flags     : {0}", DataFlagsToString(texture.DataFlags));
            }

            // Decode the texture
            try
            {
                texture.Save(outPath);
            }
            catch (CannotDecodeTextureException)
            {
                Console.WriteLine("Error: Unable to decode this texture. The texture's palette format or data format may not be supported.");
                return;
            }

            Console.WriteLine("\nTexture decoded successfully.");
        }

        public static void Encode(string[] args)
        {
            string outPath = Path.ChangeExtension(args[1], ".gvr");
            string outPalettePath = Path.ChangeExtension(args[1], ".gvp");

            GvrTextureEncoder texture = new GvrTextureEncoder(args[1], StringToPixelFormat(args[3]), StringToDataFormat(args[4]));

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

                // GBIX type is GCIX
                if (arg == "-gcix")
                {
                    texture.GbixType = GvrGbixType.Gcix;
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

                // Set that the texture has mipmaps
                if (arg == "-mipmaps")
                {
                    texture.HasMipmaps = true;
                }

                // Set that the texture has an external palette
                if (arg == "-ep")
                {
                    texture.NeedsExternalPalette = true;
                }
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format         : GVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index   : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions     : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            if (texture.PixelFormat != GvrPixelFormat.Unknown)
            {
                Console.WriteLine("Palette Format : {0}", PixelFormatToString(texture.PixelFormat));
            }
            Console.WriteLine("Data Format    : {0}", DataFormatToString(texture.DataFormat));
            if (texture.DataFlags != GvrDataFlags.None)
            {
                Console.WriteLine("Data Flags     : {0}", DataFlagsToString(texture.DataFlags));
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
            GvrTexture texture;

            // Initalize the texture
            try
            {
                texture = new GvrTexture(args[1]);
            }
            catch (NotAValidTextureException)
            {
                Console.WriteLine("Error: This is not a valid GVR texture.");
                return;
            }

            Console.WriteLine("Texture Information");
            Console.WriteLine("--------------------");
            Console.WriteLine("Format         : GVR");
            if (texture.HasGlobalIndex)
            {
                Console.WriteLine("Global Index   : {0}", texture.GlobalIndex);
            }
            Console.WriteLine("Dimensions     : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
            if (texture.PixelFormat != GvrPixelFormat.Unknown)
            {
                Console.WriteLine("Palette Format : {0}", PixelFormatToString(texture.PixelFormat));
            }
            Console.WriteLine("Data Format    : {0}", DataFormatToString(texture.DataFormat));
            if (texture.DataFlags != GvrDataFlags.None)
            {
                Console.WriteLine("Data Flags     : {0}", DataFlagsToString(texture.DataFlags));
            }
        }

        private static string PixelFormatToString(GvrPixelFormat format)
        {
            switch (format)
            {
                case GvrPixelFormat.IntensityA8: return "8-bit Intensity with Alpha";
                case GvrPixelFormat.Rgb565:      return "RGB565";
                case GvrPixelFormat.Rgb5a3:      return "RGB5A3";
            }

            return "None";
        }
        private static GvrPixelFormat StringToPixelFormat(string format)
        {
            switch (format.ToLower())
            {
                case "ia8":    return GvrPixelFormat.IntensityA8;
                case "rgb565": return GvrPixelFormat.Rgb565;
                case "rgb5a3": return GvrPixelFormat.Rgb5a3;
            }

            return GvrPixelFormat.Unknown;
        }

        private static string DataFormatToString(GvrDataFormat format)
        {
            switch (format)
            {
                case GvrDataFormat.Intensity4:  return "4-bit Intensity";
                case GvrDataFormat.Intensity8:  return "8-bit Intensity";
                case GvrDataFormat.IntensityA4: return "4-bit Intensity with Alpha";
                case GvrDataFormat.IntensityA8: return "8-bit Intensity with Alpha";
                case GvrDataFormat.Rgb565:      return "RGB565";
                case GvrDataFormat.Rgb5a3:      return "RGB5A3";
                case GvrDataFormat.Argb8888:    return "ARGB8888";
                case GvrDataFormat.Index4:      return "4-bit Indexed";
                case GvrDataFormat.Index8:      return "8-bit Indexed";
                case GvrDataFormat.Dxt1:        return "DXT1 Compressed";
            }

            return "Unknown";
        }
        private static GvrDataFormat StringToDataFormat(string format)
        {
            switch (format.ToLower())
            {
                case "i4":       return GvrDataFormat.Intensity4;
                case "i8":       return GvrDataFormat.Intensity8;
                case "ia4":      return GvrDataFormat.IntensityA4;
                case "ia8":      return GvrDataFormat.IntensityA8;
                case "rgb565":   return GvrDataFormat.Rgb565;
                case "rgb5a3":   return GvrDataFormat.Rgb5a3;
                case "argb8888": return GvrDataFormat.Argb8888;
                case "index4":   return GvrDataFormat.Index4;
                case "index8":   return GvrDataFormat.Index8;
                case "dxt1":     return GvrDataFormat.Dxt1;
            }

            return GvrDataFormat.Unknown;
        }

        private static string DataFlagsToString(GvrDataFlags flags)
        {
            // Even though these are flags, only one of them is set at a time
            // (although some textures may have more than one set, only one of them matters)
            if ((flags & GvrDataFlags.Mipmaps) != 0)
            {
                return "Mipmaps";
            }
            if ((flags & GvrDataFlags.ExternalPalette) != 0)
            {
                return "External Palette";
            }
            if ((flags & GvrDataFlags.InternalPalette) != 0)
            {
                return "Internal Palette";
            }

            return "None";
        }
    }
}
