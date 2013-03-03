using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

// Texture Module
namespace PuyoTools
{
    public class Textures
    {
        // Texture format
        public TextureModule Encoder { get; private set; }
        public TextureModule Decoder { get; private set; }

        public TextureFormat Format = TextureFormat.NULL;
        private Stream Data         = null;
        private string Filename     = null;
        public string ImageName     = null;
        private string FileExt      = null;

        // Texture Dictionary
        public static Dictionary<TextureFormat, TextureModule> Dictionary { get; private set; }

        // Set up texture object for decoding
        public Textures(Stream data, string filename)
        {
            // Set up information and initalize decompressor
            Decoder  = null;
            Data     = data;
            Filename = filename;

            InitalizeDecoder();
        }

        // Decode texture
        public Bitmap Decode()
        {
            return Decoder.Decode(Data);
        }

        // Encode texture
        public Stream Encode()
        {
            //return Encoder.Pack(imageData);
            return null;
        }

        // External Palette Filename
        public string PaletteFilename
        {
            get
            {
                return Decoder.PaletteFilename(Filename);
            }
        }

        // Output Directory
        public string OutputDirectory
        {
            get
            {
                return (ImageName == null ? null : ImageName + " Converted");
            }
        }

        // File Extension
        public string FileExtension
        {
            get
            {
                return (FileExt == null ? String.Empty : FileExt);
            }
        }

        // Initalize Decoder
        private void InitalizeDecoder()
        {
            foreach (KeyValuePair<TextureFormat, TextureModule> value in Dictionary)
            {
                if (Dictionary[value.Key].Check(Data, Filename))
                {
                    // This is the compression format
                    if (value.Value.CanDecode)
                    {
                        Format    = value.Key;
                        Decoder   = value.Value;
                        ImageName = Decoder.Name;
                        FileExt   = Decoder.Extension;
                    }

                    break;
                }
            }
        }

        // Initalize Compressor
        private void InitalizeEncoder()
        {
            // Get compressor based on compression format
            if (Dictionary.ContainsKey(Format) && Dictionary[Format].CanEncode)
            {
                Encoder   = Dictionary[Format];
                ImageName = Encoder.Name;
            }
        }

        // Initalize Image Dictionary
        public static void InitalizeDictionary()
        {
            Dictionary = new Dictionary<TextureFormat, TextureModule>();

            // Add all the entries to the dictionary
            Dictionary.Add(TextureFormat.GIM, new GIM());
            Dictionary.Add(TextureFormat.GMP, new GMP());
            Dictionary.Add(TextureFormat.GVR, new GVR());
            Dictionary.Add(TextureFormat.PVR, new PVR());
            Dictionary.Add(TextureFormat.SVR, new SVR());
        }
    }

    // Image Format
    public enum TextureFormat
    {
        NULL,
        GIM,
        GMP,
        GVR,
        PVR,
        SVR,
    }

    // Image Header
    public static class TextureHeader
    {
        public const string
            GBIX = "GBIX",
            GCIX = "GCIX",
            GIM  = ".GIM1.00\x00PSP",
            GMP  = "GMP-200\x00",
            GVRT = "GVRT",
            MIG  = "MIG.00.1PSP\x00",
            PVRT = "PVRT";
    }

    public abstract class TextureModule
    {
        // Variables
        public string Name      { get; protected set; }
        public string Extension { get; protected set; }
        public bool CanEncode   { get; protected set; }
        public bool CanDecode   { get; protected set; }

        // Other variables
        public Stream PaletteData   = null;
        public int PixelFormatIndex = -1;
        public int DataFormatIndex  = -1;

        // Texture Functions
        public abstract Bitmap Decode(Stream data); // Decode texture
        public abstract Stream Encode(Stream data); // Encode Image
        public abstract bool Check(Stream data, string filename); // Check texture
        public virtual string PaletteFilename(string filename) // External Palette Filename
        {
            return filename;
        }
    }
}