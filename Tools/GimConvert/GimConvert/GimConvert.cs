using System;
using System.IO;

using GimSharp;

namespace GimConvert
{
    public static class GimConvert
    {
        private const string Version = "2.0.0";

        public static void Main(string[] args)
        {
            Console.WriteLine("GimConvert");
            Console.WriteLine("Version {0}", Version);
            Console.WriteLine("--------------------\n");

            // Display texture information
            if (args.Length > 1 && args[0] == "-i")
            {
                // Make sure the file exists
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Error: {0} does not exist.", args[1]);
                    return;
                }

                GimTexture texture = new GimTexture(args[1]);

                // Was this texture initalized successfully
                if (!texture.Initalized)
                {
                    Console.WriteLine("Error: This is not a valid GIM texture, or it is an unsupported one.");
                    return;
                }

                Console.WriteLine("Texture Information");
                Console.WriteLine("--------------------");
                Console.WriteLine("Dimensions     : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
                if (texture.PaletteFormat != GimPaletteFormat.Unknown)
                {
                    Console.WriteLine("Palette Format : {0}", PaletteFormatToString(texture.PaletteFormat));
                }
                Console.WriteLine("Data Format    : {0}", DataFormatToString(texture.DataFormat));

                if (texture.HasMetadata)
                {
                    Console.WriteLine("\nTexture Metadata");
                    Console.WriteLine("--------------------");
                    Console.WriteLine("Original Filename : {0}", texture.Metadata.OriginalFilename);
                    Console.WriteLine("User              : {0}", texture.Metadata.User);
                    Console.WriteLine("Timestamp         : {0}", texture.Metadata.Timestamp);
                    Console.WriteLine("Program           : {0}", texture.Metadata.Program);
                }
            }

            // Decode a texture
            else if (args.Length > 1 && args[0] == "-d")
            {
                // Make sure the file exists
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Error: {0} does not exist.", args[1]);
                    return;
                }

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
                }

                GimTexture texture = new GimTexture(args[1]);

                // Was this texture initalized successfully
                if (!texture.Initalized)
                {
                    Console.WriteLine("Error: This is not a valid GIM texture, or it is an unsupported one.");
                    return;
                }

                Console.WriteLine("Texture Information");
                Console.WriteLine("--------------------");
                Console.WriteLine("Dimensions     : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
                if (texture.PaletteFormat != GimPaletteFormat.Unknown)
                {
                    Console.WriteLine("Palette Format : {0}", PaletteFormatToString(texture.PaletteFormat));
                }
                Console.WriteLine("Data Format    : {0}", DataFormatToString(texture.DataFormat));

                if (texture.HasMetadata)
                {
                    Console.WriteLine("\nTexture Metadata");
                    Console.WriteLine("--------------------");
                    Console.WriteLine("Original Filename : {0}", texture.Metadata.OriginalFilename);
                    Console.WriteLine("User              : {0}", texture.Metadata.User);
                    Console.WriteLine("Timestamp         : {0}", texture.Metadata.Timestamp);
                    Console.WriteLine("Program           : {0}", texture.Metadata.Program);
                }

                texture.Save(outPath);

                Console.WriteLine("\nTexture decoded successfully.");
            }

            // Encode a texture
            else if (args.Length > 3 && args[0] == "-e")
            {
                // Make sure the file exists
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Error: {0} does not exist.", args[1]);
                    return;
                }

                string outPath = Path.ChangeExtension(args[1], ".gim");

                GimTextureEncoder texture = new GimTextureEncoder(args[1], StringToPaletteFormat(args[2]), StringToDataFormat(args[3]));

                // Was this texture initalized successfully
                if (!texture.Initalized)
                {
                    Console.WriteLine("Error: Unable to encode using the specified texture.");
                    return;
                }

                texture.Metadata.OriginalFilename = Path.GetFileName(args[1]);
                texture.Metadata.User = Environment.UserName;
                texture.Metadata.Program = "GimConvert " + Version;

                // Get arguments
                for (int i = 4; i < args.Length; i++)
                {
                    string arg = args[i].ToLower();

                    // Change the output path
                    if (args[i] == "-o" && args.Length > i + 1)
                    {
                        outPath = args[i + 1];
                        i++;
                    }

                    // No metadata
                    if (arg == "-nometa")
                    {
                        texture.HasMetadata = false;
                    }

                    // Metadata original filename
                    if (arg == "-metafile" && args.Length > i + 1)
                    {
                        texture.Metadata.OriginalFilename = args[i + 1];
                        i++;
                    }

                    // Metadata user
                    if (arg == "-metauser" && args.Length > i + 1)
                    {
                        texture.Metadata.User = args[i + 1];
                        i++;
                    }

                    // Metadata timestamp
                    if (arg == "-metatimestamp" && args.Length > i + 1)
                    {
                        texture.Metadata.Timestamp = args[i + 1];
                        i++;
                    }

                    // Metadata program
                    if (arg == "-metaprogram" && args.Length > i + 1)
                    {
                        texture.Metadata.Program = args[i + 1];
                        i++;
                    }
                }

                Console.WriteLine("Texture Information");
                Console.WriteLine("--------------------");
                Console.WriteLine("Dimensions     : {0}x{1}", texture.TextureWidth, texture.TextureHeight);
                if (texture.PaletteFormat != GimPaletteFormat.Unknown)
                {
                    Console.WriteLine("Palette Format : {0}", PaletteFormatToString(texture.PaletteFormat));
                }
                Console.WriteLine("Data Format    : {0}", DataFormatToString(texture.DataFormat));

                if (texture.HasMetadata)
                {
                    Console.WriteLine("\nTexture Metadata");
                    Console.WriteLine("--------------------");
                    Console.WriteLine("Original Filename : {0}", texture.Metadata.OriginalFilename);
                    Console.WriteLine("User              : {0}", texture.Metadata.User);
                    Console.WriteLine("Timestamp         : {0}", texture.Metadata.Timestamp);
                    Console.WriteLine("Program           : {0}", texture.Metadata.Program);
                }

                texture.Save(outPath);

                Console.WriteLine("\nTexture encoded successfully.");
            }

            // More help
            else if (args.Length > 0 && args[0] == "/?")
            {
                Console.WriteLine("Decode texture:\n");
                Console.WriteLine("    GimConvert -d <input> [options]\n");
                Console.WriteLine("    <input>     : Input file");
                Console.WriteLine("    -o <output> : Specify output file\n");
                Console.WriteLine("Encode texture:\n");
                Console.WriteLine("    GimConvert -e <input> <palette format> <data format> [options]\n");
                Console.WriteLine("    <input>          : Input file");
                Console.WriteLine("    <palette format> : Palette format");
                Console.WriteLine("    <data format>    : Data format");
                Console.WriteLine("    -o <output>      : Specify output file");
                Console.WriteLine("    -nometa          : Do not include metadata");
                Console.WriteLine("    -metafile        : Specify the original filename in the metadata");
                Console.WriteLine("    -metauser        : Specify the user in the metadata");
                Console.WriteLine("    -metatimestamp   : Specify the timestamp in the metadata");
                Console.WriteLine("    -metaprogram     : Specify the program used in the metadata\n");
                Console.WriteLine("    Palette formats:\n");
                Console.WriteLine("        none (only for non-palettized formats)");
                Console.WriteLine("        rgb565");
                Console.WriteLine("        argb1555");
                Console.WriteLine("        argb4444");
                Console.WriteLine("        argb8888\n");
                Console.WriteLine("    Data formats:\n");
                Console.WriteLine("        rgb565");
                Console.WriteLine("        argb1555");
                Console.WriteLine("        argb4444");
                Console.WriteLine("        argb8888");
                Console.WriteLine("        index4");
                Console.WriteLine("        index8\n");
                Console.WriteLine("Texture information:\n");
                Console.WriteLine("    GimConvert -i <input>\n");
                Console.WriteLine("    <input> : Input file");
            }

            // Display simple help
            else
            {
                Console.WriteLine("Decode texture:\n");
                Console.WriteLine("    GimConvert -d <input> [options]\n");
                Console.WriteLine("Encode texture:\n");
                Console.WriteLine("    GimConvert -e <input> <palette format> <data format> [options]\n");
                Console.WriteLine("Texture information:\n");
                Console.WriteLine("    GimConvert -i <input>\n");
                Console.WriteLine("More help:\n");
                Console.WriteLine("    GimConvert /?\n");
            }
        }

        private static string PaletteFormatToString(GimPaletteFormat format)
        {
            switch (format)
            {
                case GimPaletteFormat.Rgb565:   return "RGB565";
                case GimPaletteFormat.Argb1555: return "ARGB1555";
                case GimPaletteFormat.Argb4444: return "ARGB4444";
                case GimPaletteFormat.Argb8888: return "ARGB8888";
            }

            return "None";
        }
        private static GimPaletteFormat StringToPaletteFormat(string format)
        {
            switch (format.ToLower())
            {
                case "rgb565":   return GimPaletteFormat.Rgb565;
                case "argb1555": return GimPaletteFormat.Argb1555;
                case "argb4444": return GimPaletteFormat.Argb4444;
                case "argb8888": return GimPaletteFormat.Argb8888;
            }

            return GimPaletteFormat.Unknown;
        }

        private static string DataFormatToString(GimDataFormat format)
        {
            switch (format)
            {
                case GimDataFormat.Rgb565:   return "RGB565";
                case GimDataFormat.Argb1555: return "ARGB1555";
                case GimDataFormat.Argb4444: return "ARGB4444";
                case GimDataFormat.Argb8888: return "ARGB8888";
                case GimDataFormat.Index4:   return "4-bit Indexed";
                case GimDataFormat.Index8:   return "8-bit Indexed";
            }

            return "Unknown";
        }
        private static GimDataFormat StringToDataFormat(string format)
        {
            switch (format.ToLower())
            {
                case "rgb565":   return GimDataFormat.Rgb565;
                case "argb1555": return GimDataFormat.Argb1555;
                case "argb4444": return GimDataFormat.Argb4444;
                case "argb8888": return GimDataFormat.Argb8888;
                case "index4":   return GimDataFormat.Index4;
                case "index8":   return GimDataFormat.Index8;
            }

            return GimDataFormat.Unknown;
        }
    }
}