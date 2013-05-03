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
        private const string Version = "1.1.0";

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("VrConvert");
                Console.WriteLine("Version {0}", Version);
                Console.WriteLine("------------------------");
                Console.WriteLine("Usage:");
                Console.WriteLine();
                Console.WriteLine("Decode: vrconvert -d <input> [-o <output>] [-c <clut>] [-ac]");
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
            int ClutArgIndex     = Array.IndexOf(args, "-c");
            int AutoClutArgIndex = Array.IndexOf(args, "-ac");

            // Get the strings in the command line arguments
            string InputFile  = args[1];
            string OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".png");
            string ClutFile   = (ClutArgIndex != -1 && AutoClutArgIndex == -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : null);

            string InputPath  = (Path.GetDirectoryName(InputFile) != String.Empty ? Path.GetDirectoryName(InputFile) + Path.DirectorySeparatorChar : String.Empty);
            string OutputPath = InputPath;

            // Load the data (as a byte array)
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("ERROR: {0} does not exist.", Path.GetFileNameWithoutExtension(args[1]));
                return;
            }
            byte[] VrData = new byte[0];
            using (BufferedStream stream = new BufferedStream(new FileStream(args[1], FileMode.Open, FileAccess.Read)))
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
                if (AutoClutArgIndex != -1)
                    ClutFile = InputPath + Path.GetFileNameWithoutExtension(InputFile) + ".gvp";

                DecodeSuccess = new VrDecoder.Gvr().DecodeTexture(VrData, ClutFile, out BitmapData);
            }
            else if (PvrTexture.Is(VrData))
            {
                if (AutoClutArgIndex != -1)
                    ClutFile = InputPath + Path.GetFileNameWithoutExtension(InputFile) + ".pvp";

                DecodeSuccess = new VrDecoder.Pvr().DecodeTexture(VrData, ClutFile, out BitmapData);
            }
            else if (SvrTexture.Is(VrData))
            {
                if (AutoClutArgIndex != -1)
                    ClutFile = InputPath + Path.GetFileNameWithoutExtension(InputFile) + ".svp";

                DecodeSuccess = new VrDecoder.Svr().DecodeTexture(VrData, ClutFile, out BitmapData);
            }
            else
                Console.WriteLine("ERROR: Not a Gvr, Pvr, or Svr texture.");

            // Was the data decoded successfully?
            if (DecodeSuccess && BitmapData != null)
            {
                try
                {
                    using (BufferedStream stream = new BufferedStream(new FileStream(OutputPath + OutputFile, FileMode.Create, FileAccess.Write)))
                        BitmapData.WriteTo(stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to output texture.");
                    Console.WriteLine(e.ToString());
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
            int ClutArgIndex        = Array.IndexOf(args, "-c");
            int GlobalIndexArgIndex = Array.IndexOf(args, "-gi");

            // Get the strings in the command line arguments
            string InputFile  = args[1];
            string OutputFile = String.Empty;
            string ClutFile   = String.Empty;

            string InputPath  = (Path.GetDirectoryName(InputFile) != String.Empty ? Path.GetDirectoryName(InputFile) + Path.DirectorySeparatorChar : String.Empty);
            string OutputPath = InputPath;

            // Get the global index and convert it to a string
            uint GlobalIndex = 0;
            if (GlobalIndexArgIndex != -1 && args.Length > GlobalIndexArgIndex + 1)
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
            using (BufferedStream stream = new BufferedStream(new FileStream(args[1], FileMode.Open, FileAccess.Read)))
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
                OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".gvr");
                ClutFile   = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.GetFileNameWithoutExtension(OutputFile) + ".gvp");

                EncodeSuccess = new VrEncoder.Gvr().EncodeTexture(BitmapData, PixelFormat, DataFormat, true, GlobalIndex, out TextureData, out ClutData);
            }
            if (VrFormat == "pvr")
            {
                // Pvr Unique Args
                int CompressionArgIndex  = Array.IndexOf(args, "-cmp");
                string CompressionFormat = (CompressionArgIndex != -1 && args.Length > CompressionArgIndex + 1 ? args[CompressionArgIndex + 1] : null);

                // Convert to a pvr
                OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".pvr");
                ClutFile   = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.GetFileNameWithoutExtension(OutputFile) + ".pvp");

                EncodeSuccess = new VrEncoder.Pvr().EncodeTexture(BitmapData, PixelFormat, DataFormat, CompressionFormat, true, GlobalIndex, out TextureData, out ClutData);
            }
            else if (VrFormat == "svr")
            {
                // Convert to a svr
                OutputFile = (OutFileArgIndex != -1 && OutFileArgIndex < args.Length ? args[OutFileArgIndex + 1] : Path.GetFileNameWithoutExtension(InputFile) + ".svr");
                ClutFile   = (ClutArgIndex != -1 && ClutArgIndex < args.Length ? args[ClutArgIndex + 1] : Path.GetFileNameWithoutExtension(OutputFile) + ".svp");

                EncodeSuccess = new VrEncoder.Svr().EncodeTexture(BitmapData, PixelFormat, DataFormat, true, GlobalIndex, out TextureData, out ClutData);
            }

            // Was the data encoded successfully?
            if (EncodeSuccess && TextureData != null)
            {
                try
                {
                    using (BufferedStream stream = new BufferedStream(new FileStream(OutputPath + OutputFile, FileMode.Create, FileAccess.Write)))
                        TextureData.WriteTo(stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to output texture.");
                    Console.WriteLine(e.ToString());
                }
            }
            else if (EncodeSuccess && TextureData == null)
                Console.WriteLine("ERROR: Unable to encode texture.");

            // Was the clut encoded successfully?
            if (EncodeSuccess && ClutData != null)
            {
                try
                {
                    using (BufferedStream stream = new BufferedStream(new FileStream(OutputPath + ClutFile, FileMode.Create, FileAccess.Write)))
                        ClutData.WriteTo(stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Unable to output texture.");
                    Console.WriteLine(e.ToString());
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
                "\t-o <output> : Set output filename (default is <input>.png)\n" +
                "\t-c <clut>   : Sets the clut filename\n" +
                "\t-ac         : Auto find clut file using <input> filename.");
        }

        private static void EncodingHelp(string format)
        {
            if (format.ToLower() == "gvr")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "<pixelfmt> Pixel Formats:\n" +
                    "\tia8    : Intensity 8-bit with Alpha\n" +
                    "\trgb565 : Rgb565\n" +
                    "\trgb5a3 : Rgb5a3\n" +
                    "\tnone   : Use if data format is not 4/8-bit Clut");
                Console.WriteLine();
                Console.WriteLine(
                    "<datafmt> Data Formats:\n" +
                    "\ti4       : Intensity 4-bit\n" +
                    "\ti8       : Intensity 8-bit\n" +
                    "\tia4      : Intensity 4-bit with Alpha\n" +
                    "\tia8      : Intensity 8-bit with Alpha\n" +
                    "\trgb565   : Rgb565\n" +
                    "\trgb5a3   : Rgb5a3\n" +
                    "\targb8888 : Argb8888\n" +
                    "\tindex4   : 4-bit Palettized (set pixel format)\n" +
                    "\tindex8   : 8-bit Palettized (set pixel format)\n");
                    //"\tcmp      : S3tc/Dxtn1 Compression");
                Console.WriteLine();
                Console.WriteLine(
                    "[options] Options:\n" +
                    "\t-o <output>  : Set output filename (default is <input>.gvr)\n" +
                    //"\t-gbix        : Include Gbix Header (Gamecube)\n" +
                    //"\t-gcix        : Include Gcix Header (Wii)\n" +
                    //"\t-nogbix      : Don't include Gbix/Gcix Header\n" +
                    "\t-gi <gindex> : Sets the Global Index (default is 0)\n");
            }

            else if (format.ToLower() == "pvr")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "<pixelfmt> Pixel Formats:\n" +
                    "\targb1555 : Argb1555\n" +
                    "\trgb565   : Rgb565\n" +
                    "\targb4444 : Argb4444");
                Console.WriteLine();
                Console.WriteLine(
                    "<datafmt> Data Formats:\n" +
                    "\tsqr         : Square Twiddled\n" +
                    //"\tsqrmips     : Square Twiddled with Mipmaps\n" +
                    //"\tvq          : Vq\n" +
                    //"\tvqmips      : Vq w/ Mipmaps\n" +
                    "\tindex4      : 4-bit Palettized with External Clut\n" +
                    "\tindex8      : 8-bit Palettized with External Clut\n" +
                    "\trect        : Rectangle\n" +
                    "\trecttwidled : Rectangle Twiddled\n");
                    //"\tsmallvq     : Small Vq\n" +
                //"\tsmallvqmips : Small Vq with Mipmaps\n");
                Console.WriteLine();
                Console.WriteLine(
                    "[options] Options:\n" +
                    "\t-o <output>  : Set output filename (default is <input>.pvr)\n" +
                    //"\t-gbix        : Include Gbix Header\n" +
                    //"\t-nogbix      : Don't include Gbix Header\n" +
                    "\t-gi <gindex> : Sets the Global Index (default is 0)\n" +
                    "\t-cmp <fmt>   : Sets the compression format\n" +
                    "\t               none (default), rle");
            }

            else if (format.ToLower() == "svr")
            {
                Console.WriteLine();
                Console.WriteLine(
                    "<pixelfmt> Pixel Formats:\n" +
                    "\trgb5a3   : Rgb5a3\n" +
                    "\targb8888 : Argb8888");
                Console.WriteLine();
                Console.WriteLine(
                    "<datafmt> Data Formats:\n" +
                    "\trect     : Rectangle\n" +
                    "\tindex4ec : 4-bit Palettized with External Clut\n" +
                    "\tindex8ec : 8-bit Palettized with External Clut\n" +
                    "\tindex4   : 4-bit Palettized (will set proper format based on\n" +
                    "\t           pixel format and texture dimensions.)\n" +
                    "\tindex8   : 8-bit Palettized (will set proper format based on\n" +
                    "\t           pixel format and texture dimensions.)");
                Console.WriteLine();
                Console.WriteLine(
                    "[options] Options:\n" +
                    "\t-o <output>  : Set output filename (default is <input>.svr)\n" +
                    //"\t-gbix        : Include Gbix Header\n" +
                    //"\t-nogbix      : Don't include Gbix Header\n" +
                    "\t-gi <gindex> : Sets the Global Index (default is 0)\n");
            }
        }
    }
}