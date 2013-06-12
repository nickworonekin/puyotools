using System;
using System.IO;
using System.Diagnostics;
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
            if (args.Length == 0)
            {
                Console.WriteLine("VrConvert");
                Console.WriteLine("Version {0}", Version);
                Console.WriteLine("------------------------");
                Console.WriteLine("Usage:");
                Console.WriteLine();
                //Console.WriteLine("Decode: vrconvert -d <input> [-o <output>] [-c <clut>] [-ac]");
                Console.WriteLine("Decode: vrconvert -d <input> [-o <output>] [-p <palette>]");
                Console.WriteLine("Encode: vrconvert -e <input> <vrformat> <pixelfmt> <datafmt> [options]");
                Console.WriteLine("Help:   Decoding: vrconvert -d /?");
                Console.WriteLine("        Encoding: vrconvert -e /? <vrformat (gvr/pvr/svr)>");
            }
            else
            {
                if (args[0] == "-d")
                {
                    if (args.Length >= 2 && args[1] == "/?")
                        DecodingHelp();
                    else
                        DecodeVrTexture(args);
                }
                else if (args[0] == "-e")
                {
                    if (args.Length >= 3 && args[1] == "/?")
                        EncodingHelp(args[2]);
                    else
                        EncodeVrTexture(args);
                }
                else
                    Console.WriteLine("I don't know what you want to do!");
            }
        }

        private static void DecodeVrTexture(string[] args)
        {
            // Get the command line arguments
            int OutFileArgIndex  = Array.IndexOf(args, "-o");
            int ClutArgIndex     = Array.IndexOf(args, "-p");
            //int AutoClutArgIndex = Array.IndexOf(args, "-ac");

            // Get the strings in the command line arguments
            string InputFile  = args[1];
            string OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".png");
            //string ClutFile   = (ClutArgIndex != -1 && AutoClutArgIndex == -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : null);
            string ClutFile = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : null);

            string InputPath  = (Path.GetDirectoryName(InputFile) != String.Empty ? Path.GetDirectoryName(InputFile) + Path.DirectorySeparatorChar : String.Empty);
            string OutputPath = InputPath;

            // Load the data (as a byte array)
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("ERROR: {0} does not exist.", Path.GetFileNameWithoutExtension(args[1]));
                return;
            }
            byte[] VrData = new byte[0];
            using (FileStream stream = new FileStream(args[1], FileMode.Open, FileAccess.Read))
            {
                VrData = new byte[stream.Length];
                stream.Read(VrData, 0x00, VrData.Length);
            }

            Console.WriteLine("VrConvert");
            Console.WriteLine("------------------------");
            Console.WriteLine("Decoding: {0}", Path.GetFileName(InputFile));

            // Start the watch to see how long it takes to decode
            bool DecodeSuccess = false;
            MemoryStream BitmapData = null;
            Stopwatch timer = Stopwatch.StartNew();

            // Decode the data now
            if (GvrTexture.Is(VrData))
            {
                //if (AutoClutArgIndex != -1)
                //    ClutFile = InputPath + Path.GetFileNameWithoutExtension(InputFile) + ".gvp";

                if (ClutFile == null && File.Exists(Path.ChangeExtension(InputFile, ".gvp")))
                    ClutFile = Path.ChangeExtension(InputFile, ".gvp");

                DecodeSuccess = new VrDecoder.Gvr().DecodeTexture(VrData, ClutFile, out BitmapData);
            }
            else if (PvrTexture.Is(VrData))
            {
                //if (AutoClutArgIndex != -1)
                //    ClutFile = InputPath + Path.GetFileNameWithoutExtension(InputFile) + ".pvp";

                if (ClutFile == null && File.Exists(Path.ChangeExtension(InputFile, ".pvp")))
                    ClutFile = Path.ChangeExtension(InputFile, ".pvp");

                DecodeSuccess = new VrDecoder.Pvr().DecodeTexture(VrData, ClutFile, out BitmapData);
            }
            else if (SvrTexture.Is(VrData))
            {
                //if (AutoClutArgIndex != -1)
                //    ClutFile = InputPath + Path.GetFileNameWithoutExtension(InputFile) + ".svp";

                if (ClutFile == null && File.Exists(Path.ChangeExtension(InputFile, ".svp")))
                    ClutFile = Path.ChangeExtension(InputFile, ".svp");

                DecodeSuccess = new VrDecoder.Svr().DecodeTexture(VrData, ClutFile, out BitmapData);
            }
            else
                Console.WriteLine("ERROR: Not a GVR, PVR, or SVR texture.");

            // Was the data decoded successfully?
            if (DecodeSuccess && BitmapData != null)
            {
                try
                {
                    using (FileStream stream = new FileStream(OutputPath + OutputFile, FileMode.Create, FileAccess.Write))
                        BitmapData.WriteTo(stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to write the texture. The error returned was:\n{0}", e.Message);
                }

                timer.Stop();
                Console.WriteLine("Texture decoded in {0} ms.", timer.ElapsedMilliseconds);
            }
            else if (DecodeSuccess && BitmapData == null)
                Console.WriteLine("ERROR: Unable to decode texture.");

            Console.WriteLine();
        }

        private static void EncodeVrTexture(string[] args)
        {
            // Fixed!
            // Get the command line arguments
            int OutFileArgIndex     = Array.IndexOf(args, "-o");
            int ClutArgIndex        = Array.IndexOf(args, "-p");
            int GlobalIndexArgIndex = Array.IndexOf(args, "-gi");

            // Get the strings in the command line arguments
            string InputFile  = args[1];
            string OutputFile = String.Empty;
            string ClutFile   = String.Empty;

            //string InputPath  = (Path.GetDirectoryName(InputFile) != String.Empty ? Path.GetDirectoryName(InputFile) + Path.DirectorySeparatorChar : String.Empty);
            //string OutputPath = InputPath;
            string InputPath = String.Empty;
            string OutputPath = String.Empty;

            // Get the global index and convert it to a string
            bool hasGlobalIndex = Array.IndexOf(args, "-nogbix") == -1;

            uint GlobalIndex = 0;
            if (hasGlobalIndex && GlobalIndexArgIndex != -1 && args.Length > GlobalIndexArgIndex + 1)
            {
                if (!uint.TryParse(args[GlobalIndexArgIndex + 1], out GlobalIndex))
                    GlobalIndex = 0;
            }

            // Get the format
            string VrFormat    = (args.Length > 2 ? args[2].ToLower() : null);
            string PixelFormat = (args.Length > 3 ? args[3].ToLower() : null);
            string DataFormat  = (args.Length > 4 ? args[4].ToLower() : null);

            // Make sure the vr format is correct
            if (VrFormat != "gvr" && VrFormat != "pvr" && VrFormat != "svr")
            {
                Console.WriteLine("ERROR: Unknown vr format: {0}", (VrFormat == null ? "null" : VrFormat));
                return;
            }

            // Load the data (as a byte array)
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("ERROR: {0} does not exist.", Path.GetFileNameWithoutExtension(args[1]));
                return;
            }
            byte[] BitmapData = new byte[0];
            using (FileStream stream = new FileStream(args[1], FileMode.Open, FileAccess.Read))
            {
                BitmapData = new byte[stream.Length];
                stream.Read(BitmapData, 0x00, BitmapData.Length);
            }

            Console.WriteLine("VrConvert");
            Console.WriteLine("------------------------");
            Console.WriteLine("Encoding: {0}", Path.GetFileName(InputFile));

            // Start the watch to see how long it takes to encode and set our variables
            bool EncodeSuccess       = false;
            MemoryStream TextureData = null;
            MemoryStream ClutData    = null;
            Stopwatch timer = Stopwatch.StartNew();

            if (VrFormat == "gvr")
            {
                // Convert to a pvr
                //OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".gvr");
                //ClutFile   = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.GetFileNameWithoutExtension(OutputFile) + ".gvp");
                OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.ChangeExtension(InputFile, ".gvr"));
                ClutFile = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.ChangeExtension(OutputFile, ".gvp"));

                bool gcix = Array.IndexOf(args, "-gcix") != -1;
                bool hasMipmaps = Array.IndexOf(args, "-mipmaps") != -1;
                bool hasExternalPalette = Array.IndexOf(args, "-ep") != -1;

                EncodeSuccess = new VrEncoder.Gvr().EncodeTexture(BitmapData, PixelFormat, DataFormat, hasGlobalIndex, GlobalIndex, gcix, hasMipmaps, hasExternalPalette, out TextureData, out ClutData);
            }
            if (VrFormat == "pvr")
            {
                // Pvr Unique Args
                int CompressionArgIndex  = Array.IndexOf(args, "-compression");
                string CompressionFormat = (CompressionArgIndex != -1 && args.Length > CompressionArgIndex + 1 ? args[CompressionArgIndex + 1] : null);

                // Convert to a pvr
                //OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".pvr");
                //ClutFile   = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.GetFileNameWithoutExtension(OutputFile) + ".pvp");
                OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.ChangeExtension(InputFile, ".pvr"));
                ClutFile = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.ChangeExtension(OutputFile, ".pvp"));

                EncodeSuccess = new VrEncoder.Pvr().EncodeTexture(BitmapData, PixelFormat, DataFormat, CompressionFormat, hasGlobalIndex, GlobalIndex, out TextureData, out ClutData);
            }
            else if (VrFormat == "svr")
            {
                // Convert to a svr
                //OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".svr");
                //ClutFile   = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.GetFileNameWithoutExtension(OutputFile) + ".svp");
                OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.ChangeExtension(InputFile, ".svr"));
                ClutFile = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.ChangeExtension(OutputFile, ".svp"));

                EncodeSuccess = new VrEncoder.Svr().EncodeTexture(BitmapData, PixelFormat, DataFormat, hasGlobalIndex, GlobalIndex, out TextureData, out ClutData);
            }

            // Was the data encoded successfully?
            if (EncodeSuccess && TextureData != null)
            {
                try
                {
                    using (FileStream stream = new FileStream(OutputPath + OutputFile, FileMode.Create, FileAccess.Write))
                        TextureData.WriteTo(stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to write the texture. The error returned was:\n{0}", e.Message);
                }
            }
            else if (EncodeSuccess && TextureData == null)
                Console.WriteLine("ERROR: Unable to encode texture.");

            // Was the clut encoded successfully?
            if (EncodeSuccess && ClutData != null)
            {
                try
                {
                    using (FileStream stream = new FileStream(OutputPath + ClutFile, FileMode.Create, FileAccess.Write))
                        ClutData.WriteTo(stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to write the palette. The error returned was:\n{0}", e.Message);
                }
            }

            // Stop the timer if everything was encoded.
            if (EncodeSuccess && TextureData != null)
            {
                timer.Stop();
                Console.WriteLine("Texture encoded in {0} ms.", timer.ElapsedMilliseconds);
            }
        }

        private static void DecodingHelp()
        {
            Console.WriteLine();
            Console.WriteLine(
                "\t-o <output>  : Set output filename (default is <input>.png)\n" +
                "\t-p <palette> : Sets the palette filename\n");
        }

        private static void EncodingHelp(string format)
        {
            if (format.ToLower() == "gvr")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "<pixelfmt> Pixel Formats:\n\n" +
                    "\tia8    : Intensity 8-bit with Alpha\n" +
                    "\trgb565 : Rgb565\n" +
                    "\trgb5a3 : Rgb5a3\n" +
                    "\tnone   : Use if data format is not index4 or index8");
                Console.WriteLine();
                Console.WriteLine(
                    "<datafmt> Data Formats:\n\n" +
                    "\ti4       : Intensity 4-bit\n" +
                    "\ti8       : Intensity 8-bit\n" +
                    "\tia4      : Intensity 4-bit with Alpha\n" +
                    "\tia8      : Intensity 8-bit with Alpha\n" +
                    "\trgb565   : Rgb565\n" +
                    "\trgb5a3   : Rgb5a3\n" +
                    "\targb8888 : Argb8888\n" +
                    "\tindex4   : 4-bit Palettized (set pixel format)\n" +
                    "\tindex8   : 8-bit Palettized (set pixel format)\n" +
                    "\tdxt1     : S3TC/DXT1 Compressed");
                Console.WriteLine();
                Console.WriteLine(
                    "[options] Options:\n\n" +
                    "\t-o <output>  : Set output filename\n" +
                    "\t-p <palette> : Sets the palette filename\n" +
                    "\t-gi <value>  : Set the global index value\n" +
                    "\t-gcix        : Use a GCIX header instead of a GBIX header\n" +
                    "\t-nogbix      : Do not include a GBIX/GCIX header\n" +
                    "\t-mipmaps     : Texture has mipmaps\n" +
                    "\t-ep          : Texture has an external palette\n");
            }

            else if (format.ToLower() == "pvr")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "<pixelfmt> Pixel Formats:\n\n" +
                    "\targb1555 : Argb1555\n" +
                    "\trgb565   : Rgb565\n" +
                    "\targb4444 : Argb4444");
                Console.WriteLine();
                Console.WriteLine(
                    "<datafmt> Data Formats:\n\n" +
                    "\tsquare            : Square Twiddled\n" +
                    "\tsquaremipmaps     : Square Twiddled with Mipmaps\n" +
                    "\tindex4            : 4-bit Palettized with External Palette\n" +
                    "\tindex8            : 8-bit Palettized with External Palette\n" +
                    "\trectangle         : Rectangle\n" +
                    "\trectangletwiddled : Rectangle Twiddled\n" +
                    "\tsquaremipmapsalt  : Square Twiddled with Mipmaps (Alternate)");
                Console.WriteLine();
                Console.WriteLine(
                    "[options] Options:\n\n" +
                    "\t-o <output>        : Set output filename\n" +
                    "\t-p <palette>       : Sets the palette filename\n" +
                    "\t-gi <value>        : Set the global index value\n" +
                    "\t-nogbix            : Do not include a GBIX header\n" +
                    "\t-compression <fmt> : Sets the compression format:\n" +
                    "\t                     none (default), rle");
            }

            else if (format.ToLower() == "svr")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "<pixelfmt> Pixel Formats:\n\n" +
                    "\trgb5a3   : Rgb5a3\n" +
                    "\targb8888 : Argb8888");
                Console.WriteLine();
                Console.WriteLine(
                    "<datafmt> Data Formats:\n\n" +
                    "\trectangle  : Rectangle\n" +
                    "\tindex4ep   : 4-bit Palettized with External Palette\n" +
                    "\tindex8ep   : 8-bit Palettized with External Palette\n" +
                    "\tindex4     : 4-bit Palettized (will set proper format based on\n" +
                    "\t             pixel format and texture dimensions.)\n" +
                    "\tindex8     : 8-bit Palettized (will set proper format based on\n" +
                    "\t             pixel format and texture dimensions.)");
                Console.WriteLine();
                Console.WriteLine(
                    "[options] Options:\n\n" +
                    "\t-o <output>  : Set output filename\n" +
                    "\t-p <palette> : Sets the palette filename\n" +
                    "\t-gi <value>  : Set the global index value\n" +
                    "\t-nogbix      : Do not include a GBIX header\n");
            }
        }
    }
}