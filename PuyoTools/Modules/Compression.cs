using System;
using System.IO;
using System.Collections.Generic;

// Compression Module
namespace PuyoTools
{
    public class Compression
    {
        // The compressor and decompressor objects
        public CompressionModule Compressor   = null;
        public CompressionModule Decompressor = null;

        private Stream Data     = null;
        private string Filename = null;

        // Restricted variables
        public CompressionFormat Format { get; private set; }
        public string Name              { get; private set; }

        // Compression Dictionary
        public static Dictionary<CompressionFormat, CompressionModule> Dictionary { get; private set; }

        // Set up compression object for decompression
        public Compression(Stream data, string filename)
        {
            // Set up information and
            Format   = CompressionFormat.NULL;
            Name     = null;
            Data     = data;
            Filename = filename;

            // Initalize Decompressor
            InitalizeDecompressor();
        }

        // Set up compression object for compression
        public Compression(Stream data, string filename, CompressionFormat format)
        {
            // Set up information
            Name       = null;
            Data       = data;
            Filename   = filename;
            Format     = format;

            // Initalize Compressor
            InitalizeCompressor();
        }

        // Decompress
        public MemoryStream Decompress()
        {
            if (Decompressor == null)
                throw new Exception("Could not decompress because no decompressor was initalized.");

            // TESTING.
            // If this is LZ10 compression then we should test the new decompressor API
            if (Format == CompressionFormat.LZ10)
            {
                MemoryStream outStream = new MemoryStream();
                Data.Position = 0;
                PuyoTools2.Compression.Compression.Decompress(Data, (int)Data.Length, outStream, PuyoTools2.Compression.CompressionFormat.LZ10);
                return outStream;
            }

            return Decompressor.Decompress(Data);
        }

        // Compress
        public MemoryStream Compress()
        {
            if (Compressor == null)
                throw new Exception("Could not compress because no compressor was initalized.");

            return Compressor.Compress(Data, Path.GetFileName(Filename));
        }

        // Get Filename
        public string DecompressFilename
        {
            get
            {
                return Decompressor.DecompressFilename(Data, Filename);
            }
        }
        public string CompressFilename
        {
            get
            {
                return Compressor.CompressFilename(Data, Filename);
            }
        }

        // Output Directory
        public string OutputDirectory
        {
            get
            {
                if (Compressor != null)
                    return (Name ?? "File Data") + " Compressed";
                else
                    return (Name ?? "File Data") + " Decompressed";
            }
        }

        // Initalize Decompressor
        private void InitalizeDecompressor()
        {
            // Reset all values
            Format       = CompressionFormat.NULL;
            Decompressor = null;
            Name         = null;

            foreach (KeyValuePair<CompressionFormat, CompressionModule> value in Dictionary)
            {
                if (value.Value.Check(Data, Filename))
                {
                    // This is the compression format
                    if (value.Value.CanDecompress)
                    {
                        Format       = value.Key;
                        Decompressor = value.Value;
                        Name         = Decompressor.Name;
                    }

                    break;
                }
            }
        }

        // Initalize Compressor
        private void InitalizeCompressor()
        {
            // Get compressor based on compression format
            if (Dictionary.ContainsKey(Format) && Dictionary[Format].CanCompress)
            {
                Compressor = Dictionary[Format];
                Name       = Compressor.Name;
            }
        }

        // Initalize Compression Dictionary
        public static void InitalizeDictionary()
        {
            Dictionary = new Dictionary<CompressionFormat, CompressionModule>();

            // Add all the entries to the dictionary
            Dictionary.Add(CompressionFormat.CNX,  new CNX());
            Dictionary.Add(CompressionFormat.CXLZ, new CXLZ());
            Dictionary.Add(CompressionFormat.LZ00, new LZ00());
            Dictionary.Add(CompressionFormat.LZ01, new LZ01());
            Dictionary.Add(CompressionFormat.LZ10, new LZ10());
            Dictionary.Add(CompressionFormat.LZ11, new LZ11());
            Dictionary.Add(CompressionFormat.PRS,  new PRS());
        }
    }
    
    // Compression Format
    public enum CompressionFormat
    {
        NULL,
        CNX,
        CXLZ,
        LZ00,
        LZ01,
        LZ10,
        LZ11,
        PRS,
    }

    public abstract class CompressionModule
    {
        // Variables
        public string Name        { get; protected set; }
        public bool CanCompress   { get; protected set; }
        public bool CanDecompress { get; protected set; }

        // Compression Functions
        public abstract MemoryStream Decompress(Stream data); // Decompress Data
        public abstract MemoryStream Compress(Stream data, string filename); // Compress Data
        public abstract bool Check(Stream data, string filename); // Check
        public virtual string DecompressFilename(Stream data, string filename) // Get Filename for Decompressed File
        {
            return filename;
        }
        public virtual string CompressFilename(Stream data, string filename) // Get Filename for Compressed File
        {
            return filename;
        }
    }
}