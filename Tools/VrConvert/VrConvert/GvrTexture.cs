using System;
using System.IO;
using System.Collections.Generic;

namespace VrConvert
{
    public static class GvrTextureConverter
    {
        public static void Encode(string inFile, string outFile, string pixelFormatText, string dataFormatText, string[] args)
        {
        }

        public static void Decode(string inFile, string outFile, string[] args)
        {
        }

        public static void DisplayHelp()
        {
            Console.WriteLine(
                "Pixel formats:\n" +
                "\tnone   : Use this unless the data format is index4 or index8\n" +
                "\tia8    : 8-bit intensity (grayscale) with 8-bit alpha\n" +
                "\trgb565 : 16-bit color with no alpha\n" +
                "\trgb5a3 : 16-bit color with alpha\n" +
                "\n" +
                "Data formats:\n" +
                "\ti4       : 4-bit intensity (grayscale) with no alpha\n" +
                "\ti8       : 8-bit intensity (grayscale) with no alpha\n" +
                "\ti4a      : 4-bit intensity (grayscale) with 4-bit alpha\n" +
                "\tia8      : 8-bit intensity (grayscale) with 8-bit alpha\n" +
                "\trgb565   : 16-bit color with no alpha\n" +
                "\trgb5a3   : 16-bit color with alpha\n" +
                "\targb8888 : 32-bit color with alpha\n" +
                "\tindex4   : 4-bit indexed texture (specify a pixel format)\n" +
                "\tindex8   : 8-bit indexed texture (specify a pixel format)\n" +
                "\tdxt1     : S3TC/DXT1 compressed texture\n" +
                "\n" +
                "Options:\n" +
                "\t-o <output>       : Specify the output filename.\n" +
                "\t-gi <globalindex> : Specify the global index (default is 0).\n" +
                "\t-nogbix : Don't include a GBIX/GCIX header.\n" +
                "\t-gcix   : Use GCIX instead of GBIX in the header.\n" +
                "\t-ec     : Store the texture's palette in a seperate GVP file\n" +
                "\t          (only for indexed textures).\n"
            );
        }
    }
}