using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools
{
    public class CXLZ : CompressionModule
    {
        public CXLZ()
        {
            Name = "CXLZ";
            CanCompress   = true;
            CanDecompress = true;
        }

        // Decompress
        public override MemoryStream Decompress(Stream data)
        {
            try
            {
                // Compressed & Decompressed Data Information
                uint CompressedSize   = (uint)data.Length;
                uint DecompressedSize = data.ReadUInt(0x4) >> 8;

                uint SourcePointer = 0x8;
                uint DestPointer   = 0x0;

                byte[] CompressedData   = data.ToByteArray();
                byte[] DecompressedData = new byte[DecompressedSize];

                // Start Decompression
                while (SourcePointer < CompressedSize && DestPointer < DecompressedSize)
                {
                    byte Flag = CompressedData[SourcePointer]; // Compression Flag
                    SourcePointer++;

                    for (int i = 7; i >= 0; i--)
                    {
                        if ((Flag & (1 << i)) == 0) // Data is not compressed
                        {
                            DecompressedData[DestPointer] = CompressedData[SourcePointer];
                            SourcePointer++;
                            DestPointer++;
                        }
                        else // Data is compressed
                        {
                            int Distance = (((CompressedData[SourcePointer] & 0xF) << 8) | CompressedData[SourcePointer + 1]) + 1;
                            int Amount   = (CompressedData[SourcePointer] >> 4) + 3;
                            SourcePointer += 2;

                            // Copy the data
                            for (int j = 0; j < Amount; j++)
                                DecompressedData[DestPointer + j] = DecompressedData[DestPointer - Distance + j];
                            DestPointer += (uint)Amount;
                        }

                        // Check for out of range
                        if (SourcePointer >= CompressedSize || DestPointer >= DecompressedSize)
                            break;
                    }
                }

                return new MemoryStream(DecompressedData);
            }
            catch
            {
                return null; // An error occured while decompressing
            }
        }

        // Compress
        public override MemoryStream Compress(Stream data, string filename)
        {
            try
            {
                uint DecompressedSize = (uint)data.Length;

                MemoryStream CompressedData = new MemoryStream();
                byte[] DecompressedData     = data.ToByteArray();

                uint SourcePointer = 0x0;
                uint DestPointer   = 0x8;

                // Test if the file is too large to be compressed
                if (data.Length > 0xFFFFFF)
                    throw new Exception("Input file is too large to compress.");

                // Set up the Lz Compression Dictionary
                LzWindowDictionary LzDictionary = new LzWindowDictionary();
                LzDictionary.SetWindowSize(0x1000);
                LzDictionary.SetMaxMatchAmount(0xF + 3);

                // Start compression
                CompressedData.Write("CXLZ");
                CompressedData.Write((uint)('\x10' | (DecompressedSize << 8)));
                while (SourcePointer < DecompressedSize)
                {
                    byte Flag = 0x0;
                    uint FlagPosition = DestPointer;
                    CompressedData.WriteByte(Flag); // It will be filled in later
                    DestPointer++;

                    for (int i = 7; i >= 0; i--)
                    {
                        int[] LzSearchMatch = LzDictionary.Search(DecompressedData, SourcePointer, DecompressedSize);
                        if (LzSearchMatch[1] > 0) // There is a compression match
                        {
                            Flag |= (byte)(1 << i);

                            CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 3) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
                            CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));

                            LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);
                            LzDictionary.SlideWindow(LzSearchMatch[1]);

                            SourcePointer += (uint)LzSearchMatch[1];
                            DestPointer   += 2;
                        }
                        else // There wasn't a match
                        {
                            Flag |= (byte)(0 << i);

                            CompressedData.WriteByte(DecompressedData[SourcePointer]);

                            LzDictionary.AddEntry(DecompressedData, (int)SourcePointer);
                            LzDictionary.SlideWindow(1);

                            SourcePointer++;
                            DestPointer++;
                        }

                        // Check for out of bounds
                        if (SourcePointer >= DecompressedSize)
                            break;
                    }

                    // Write the flag.
                    // Note that the original position gets reset after writing.
                    CompressedData.Seek(FlagPosition, SeekOrigin.Begin);
                    CompressedData.WriteByte(Flag);
                    CompressedData.Seek(DestPointer, SeekOrigin.Begin);
                }

                return CompressedData;
            }
            catch
            {
                return null; // An error occured while compressing
            }
        }

        // Check
        public override bool Check(Stream data, string filename)
        {
            try
            {
                return (data.ReadString(0x0, 5) == "CXLZ\x10");
            }
            catch
            {
                return false;
            }
        }
    }
}
