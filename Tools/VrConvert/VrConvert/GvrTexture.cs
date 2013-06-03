using System;
using System.IO;
using System.Collections.Generic;

using VrSharp.GvrTexture;

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
                "\tia4      : 4-bit intensity (grayscale) with 4-bit alpha\n" +
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
                "\t-ep     : Store the texture's palette in a seperate GVP file\n" +
                "\t          (only for indexed textures).\n"
            );
        }

        private static GvrPixelFormat GetPixelFormat(string pixelFormat)
        {
            switch (pixelFormat)
            {
                case "none": return GvrPixelFormat.Unknown;
                case "ia8": return GvrPixelFormat.IntensityA8;
                case "rgb565": return GvrPixelFormat.Rgb565;
                case "rgb5a3": return GvrPixelFormat.Rgb5a3;
            }

            return GvrPixelFormat.Unknown;
        }

        private static GvrDataFormat GetDataFormat(string dataFormat)
        {
            switch (dataFormat)
            {
                case "i4": return GvrDataFormat.Intensity4;
                case "i8": return GvrDataFormat.Intensity8;
                case "ia4": return GvrDataFormat.IntensityA4;
                case "ia8": return GvrDataFormat.IntensityA8;
                case "rgb565": return GvrDataFormat.Rgb565;
                case "rgb5a3": return GvrDataFormat.Rgb5a3;
                case "argb8888": return GvrDataFormat.Argb8888;
                case "index4": return GvrDataFormat.Index4;
                case "index8": return GvrDataFormat.Index8;
                case "dxt1": return GvrDataFormat.Dxt1;
            }

            return GvrDataFormat.Unknown;
        }
    }
}