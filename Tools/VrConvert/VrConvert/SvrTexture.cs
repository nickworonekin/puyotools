using System;
using System.IO;
using System.Collections.Generic;

using VrSharp.SvrTexture;

namespace VrConvert
{
    public static class SvrTextureConverter
    {
        public static void Decode(string inFile, string outFile, string[] args)
        {
            string paletteFile;

            // Go through the args
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-p" && i + 1 < args.Length)
                {
                    paletteFile = args[i + 1];
                }
            }

            // Load the texture
            SvrTexture texture = new SvrTexture(inFile);
            if (!texture.Initalized)
            {
                Console.WriteLine("Error: Unable to load {0}.", Path.GetFileName(inFile));
                return;
            }

            // Check to see if this texture requires an external palette file.
            if (texture.NeedsExternalPalette)
            {
                paletteFile = Path.ChangeExtension(inFile, ".svp");

                // Ok, do we have one?
                if (!File.Exists(paletteFile))
                {
                    Console.WriteLine("Error: This texture requires an external palette file.");
                    return;
                }

                // Ok, good. Apply it.
                SvpPalette palette = new SvpPalette(paletteFile);
                if (!palette.Initalized)
                {
                    Console.WriteLine("Error: Unable to load {0}.", Path.GetFileName(paletteFile));
                    return;
                }
            }

            // Time to save the texture.
            texture.Save(outFile);
        }

        public static void Encode(string inFile, string outFile, string pixelFormatText, string dataFormatText, string[] args)
        {
        }

        public static void DisplayHelp()
        {
        }

        private static SvrPixelFormat GetPixelFormat(string pixelFormat)
        {
            switch (pixelFormat)
            {
                case "none": return SvrPixelFormat.Unknown;
                case "rgb5a3": return SvrPixelFormat.Rgb5a3;
                case "argb8888": return SvrPixelFormat.Argb8888;
            }

            return SvrPixelFormat.Unknown;
        }

        private static SvrDataFormat GetDataFormat(string dataFormat)
        {
            switch (dataFormat)
            {
                case "rect": return SvrDataFormat.Rectangle;
                case "index4p": return SvrDataFormat.Index4ExtClut;
                case "index8p": return SvrDataFormat.Index8ExtClut;
                case "index4": return SvrDataFormat.Index4RectRgb5a3; // This will be auto corrected.
                case "index8": return SvrDataFormat.Index8RectRgb5a3; // This will be auto corrected.
            }

            return SvrDataFormat.Unknown;
        }
    }
}