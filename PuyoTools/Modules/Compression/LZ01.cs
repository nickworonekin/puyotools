using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Old
{
    public class LZ01 : CompressionModule
    {
        public LZ01()
        {
            Name = "LZ01";
            CanCompress   = true;
            CanDecompress = true;
        }

        // Decompress
        public override MemoryStream Decompress(Stream data)
        {
            try
            {
                // Compressed & Decompressed Data Information
                uint CompressedSize   = data.ReadUInt(0x4);
                uint DecompressedSize = data.ReadUInt(0x8);

                byte[] CompressedData   = data.ToByteArray();
                byte[] DecompressedData = new byte[DecompressedSize];
                byte[] DestBuffer       = new byte[0x1000];

                uint SourcePointer = 0x10;
                uint DestPointer   = 0x0;
                uint BufferPointer = 0xFEE;

                // Start Decompression
                while (SourcePointer < CompressedSize && DestPointer < DecompressedSize)
                {
                    byte Flag = CompressedData[SourcePointer]; // Compression Flag
                    SourcePointer++;

                    for (int i = 0; i < 8; i++)
                    {
                        if ((Flag & (1 << i)) > 0) // Data is not compressed
                        {
                            DecompressedData[DestPointer] = CompressedData[SourcePointer];
                            DestBuffer[BufferPointer]     = DecompressedData[DestPointer];
                            SourcePointer++;
                            DestPointer++;
                            BufferPointer = (BufferPointer + 1) & 0xFFF;
                        }
                        else // Data is compressed
                        {
                            int Offset = ((((CompressedData[SourcePointer + 1] >> 4) & 0xF) << 8) | CompressedData[SourcePointer]);
                            int Amount = (CompressedData[SourcePointer + 1] & 0xF) + 3;
                            SourcePointer += 2;

                            for (int j = 0; j < Amount; j++)
                            {
                                DecompressedData[DestPointer + j] = DestBuffer[(Offset + j) & 0xFFF];
                                DestBuffer[BufferPointer]         = DecompressedData[DestPointer + j];
                                BufferPointer = (BufferPointer + 1) & 0xFFF;
                            }
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
                uint DestPointer   = 0x10;

                // Test if the file is too large to be compressed
                if (data.Length > 0xFFFFFFFF)
                    throw new Exception("Input file is too large to compress.");

                // Set up the Lz Compression Dictionary
                LzBufferDictionary LzDictionary = new LzBufferDictionary();
                LzDictionary.SetBufferSize(0x1000);
                LzDictionary.SetBufferStart(0xFEE);
                LzDictionary.SetMaxMatchAmount(0xF + 3);

                // Start compression
                CompressedData.Write("LZ01");
                CompressedData.Write((uint)0); // Will be filled in later
                CompressedData.Write(DecompressedSize);
                CompressedData.Seek(4, SeekOrigin.Current); // Advance 4 bytes

                while (SourcePointer < DecompressedSize)
                {
                    byte Flag = 0x0;
                    uint FlagPosition = DestPointer;
                    CompressedData.WriteByte(Flag); // It will be filled in later
                    DestPointer++;

                    for (int i = 0; i < 8; i++)
                    {
                        int[] LzSearchMatch = LzDictionary.Search(DecompressedData, SourcePointer, DecompressedSize);
                        if (LzSearchMatch[1] > 0) // There is a compression match
                        {
                            Flag |= (byte)(0 << i);

                            CompressedData.WriteByte((byte)(LzSearchMatch[0] & 0xFF));
                            CompressedData.WriteByte((byte)(((LzSearchMatch[0] & 0xF00) >> 4) | ((LzSearchMatch[1] - 3) & 0xF)));

                            LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);

                            SourcePointer += (uint)LzSearchMatch[1];
                            DestPointer   += 2;
                        }
                        else // There wasn't a match
                        {
                            Flag |= (byte)(1 << i);

                            CompressedData.WriteByte(DecompressedData[SourcePointer]);

                            LzDictionary.AddEntry(DecompressedData, (int)SourcePointer);

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

                CompressedData.Seek(0x4, SeekOrigin.Begin);
                CompressedData.Write((uint)CompressedData.Length);
                CompressedData.Seek(0, SeekOrigin.End);

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
                return (data.ReadString(0x0, 4) == "LZ01");
            }
            catch
            {
                return false;
            }
        }
    }
}
