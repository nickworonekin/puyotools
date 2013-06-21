using System;
using System.IO;

using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;
using VrSharp.SvrTexture;

namespace VrConvert
{
    public static class VrConvert
    {
        private const string Version = "2.0.0";

        public static void Main(string[] args)
        {
            Console.WriteLine("VrConvert");
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

                // Determine which texture converter we must use
                if (GvrTexture.Is(args[1]))
                {
                    GvrConvert.Information(args);
                }
                else if (PvrTexture.Is(args[1]))
                {
                    PvrConvert.Information(args);
                }
                else if (SvrTexture.Is(args[1]))
                {
                    SvrConvert.Information(args);
                }
                else
                {
                    Console.WriteLine("Error: This is not a GVR, PVR, or SVR texture.");
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

                // Determine which texture converter we must use
                if (GvrTexture.Is(args[1]))
                {
                    GvrConvert.Decode(args);
                }
                else if (PvrTexture.Is(args[1]))
                {
                    PvrConvert.Decode(args);
                }
                else if (SvrTexture.Is(args[1]))
                {
                    SvrConvert.Decode(args);
                }
                else
                {
                    Console.WriteLine("Error: This is not a GVR, PVR, or SVR texture.");
                }
            }

            // Encode a texture
            else if (args.Length > 4 && args[0] == "-e")
            {
                // Make sure the file exists
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Error: {0} does not exist.", args[1]);
                    return;
                }

                // Determine which texture converter we must use
                string format = args[2].ToLower();
                if (format == "gvr")
                {
                    GvrConvert.Encode(args);
                }
                else if (format == "pvr")
                {
                    PvrConvert.Encode(args);
                }
                else if (format == "svr")
                {
                    SvrConvert.Encode(args);
                }
                else
                {
                    Console.WriteLine("Error: Unknown format {0}", format);
                }
            }

            // More help
            else if (args.Length > 0 && args[0] == "/?")
            {
                if (args.Length > 1 && args[1].ToLower() == "gvr")
                {
                    Console.WriteLine("Encode GVR texture:\n");
                    Console.WriteLine("    VrConvert -e <input> gvr <palette format> <data format> [options]\n");
                    Console.WriteLine("    <input>          : Input file");
                    Console.WriteLine("    <palette format> : Palette format");
                    Console.WriteLine("    <data format>    : Data format");
                    Console.WriteLine("    -o <output>      : Specify output file");
                    Console.WriteLine("    -op <palette>    : Specify output palette file");
                    Console.WriteLine("    -nogbix          : Do not include a GBIX header");
                    Console.WriteLine("    -gcix            : Use GCIX instead of GBIX");
                    Console.WriteLine("    -gi <value>      : Set global index");
                    Console.WriteLine("    -mipmaps         : Texture will have mipmaps");
                    Console.WriteLine("    -ep              : Texture will have an external palette\n");
                    Console.WriteLine("    Palette formats:\n");
                    Console.WriteLine("        none (only for non-palettized formats)");
                    Console.WriteLine("        ia8");
                    Console.WriteLine("        rgb565");
                    Console.WriteLine("        rgb5a3\n");
                    Console.WriteLine("    Data formats:\n");
                    Console.WriteLine("        i4");
                    Console.WriteLine("        i8");
                    Console.WriteLine("        ia4");
                    Console.WriteLine("        ia8");
                    Console.WriteLine("        rgb565");
                    Console.WriteLine("        rgb5a3");
                    Console.WriteLine("        argb8888");
                    Console.WriteLine("        index4");
                    Console.WriteLine("        index8");
                    Console.WriteLine("        dxt1");
                }
                else if (args.Length > 1 && args[1].ToLower() == "pvr")
                {
                    Console.WriteLine("Encode PVR texture:\n");
                    Console.WriteLine("    VrConvert -e <input> pvr <palette format> <data format> [options]\n");
                    Console.WriteLine("    <input>        : Input file");
                    Console.WriteLine("    <pixel format> : Pixel format");
                    Console.WriteLine("    <data format>  : Data format");
                    Console.WriteLine("    -o <output>    : Specify output file");
                    Console.WriteLine("    -op <palette>  : Specify output palette file");
                    Console.WriteLine("    -nogbix        : Do not include a GBIX header");
                    Console.WriteLine("    -gi <value>    : Set global index");
                    Console.WriteLine("    -cmp <format>  : Set compression format\n");
                    Console.WriteLine("    Pixel formats:\n");
                    Console.WriteLine("        argb1555");
                    Console.WriteLine("        rgb565");
                    Console.WriteLine("        argb4444\n");
                    Console.WriteLine("    Data formats:\n");
                    Console.WriteLine("        square");
                    Console.WriteLine("        squaremipmaps");
                    Console.WriteLine("        index4");
                    Console.WriteLine("        index8");
                    Console.WriteLine("        rectangle");
                    Console.WriteLine("        rectangletwiddled");
                    Console.WriteLine("        squaretwiddledalt\n");
                    Console.WriteLine("    Compression formats:\n");
                    Console.WriteLine("        none");
                    Console.WriteLine("        rle");
                }
                else if (args.Length > 1 && args[1].ToLower() == "svr")
                {
                    Console.WriteLine("Encode SVR texture:\n");
                    Console.WriteLine("    VrConvert -e <input> svr <palette format> <data format> [options]\n");
                    Console.WriteLine("    <input>        : Input file");
                    Console.WriteLine("    <pixel format> : Pixel format");
                    Console.WriteLine("    <data format>  : Data format");
                    Console.WriteLine("    -o <output>    : Specify output file");
                    Console.WriteLine("    -op <palette>  : Specify output palette file");
                    Console.WriteLine("    -nogbix        : Do not include a GBIX header");
                    Console.WriteLine("    -gi <value>    : Set global index\n");
                    Console.WriteLine("    Pixel formats:\n");
                    Console.WriteLine("        rgb5a3");
                    Console.WriteLine("        argb8888\n");
                    Console.WriteLine("    Data formats:\n");
                    Console.WriteLine("        rectangle");
                    Console.WriteLine("        index4ep");
                    Console.WriteLine("        index8ep");
                    Console.WriteLine("        index4");
                    Console.WriteLine("        index8");
                }
                else
                {
                    Console.WriteLine("Decode texture:\n");
                    Console.WriteLine("    VrConvert -d <input> [options]\n");
                    Console.WriteLine("    <input>      : Input file");
                    Console.WriteLine("    -p <palette> : Specify palette file");
                    Console.WriteLine("    -o <output>  : Specify output file\n");
                    Console.WriteLine("Encode texture:\n");
                    Console.WriteLine("    VrConvert -e <input> <vr format> <pixel format> <data format> [options]\n");
                    Console.WriteLine("    <input>        : Input file");
                    Console.WriteLine("    <vr format>    : VR format");
                    Console.WriteLine("    <pixel format> : Pixel format");
                    Console.WriteLine("    <data format>  : Data format");
                    Console.WriteLine("    -o <output>    : Specify output file");
                    Console.WriteLine("    -op <palette>  : Specify output palette file");
                    Console.WriteLine("    -nogbix        : Do not include a GBIX header");
                    Console.WriteLine("    -gi <value>    : Set global index\n");
                    Console.WriteLine("Texture information:\n");
                    Console.WriteLine("    VrConvert -i <input>\n");
                    Console.WriteLine("    <input> : Input file\n");
                    Console.WriteLine("More help:\n");
                    Console.WriteLine("    VrConvert /? gvr");
                    Console.WriteLine("    VrConvert /? pvr");
                    Console.WriteLine("    VrConvert /? svr");
                }
            }

            // Display simple help
            else
            {
                Console.WriteLine("Decode texture:\n");
                Console.WriteLine("    VrConvert -d <input> [options]\n");
                Console.WriteLine("Encode texture:\n");
                Console.WriteLine("    VrConvert -e <input> <vr format> <pixel format> <data format> [options]\n");
                Console.WriteLine("Texture information:\n");
                Console.WriteLine("    VrConvert -i <input>\n");
                Console.WriteLine("More help:\n");
                Console.WriteLine("    VrConvert /?\n");
                Console.WriteLine("    VrConvert /? gvr");
                Console.WriteLine("    VrConvert /? pvr");
                Console.WriteLine("    VrConvert /? svr");
            }
        }
    }
}