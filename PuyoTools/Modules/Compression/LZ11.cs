using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools
{
    public class LZ11 : CompressionModule
    {
        public LZ11()
        {
            Name = "LZ11"; // Also known as ONZ and LZ77 Format 11
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
                uint DecompressedSize = data.ReadUInt(0x0) >> 8;

                uint SourcePointer = 0x4;
                uint DestPointer   = 0x0;

                if (DecompressedSize == 0) // Next 4 bytes are the decompressed size
                {
                    DecompressedSize = data.ReadUInt(0x4);
                    SourcePointer += 0x4;
                }

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
                            int Distance;
                            int Amount;

                            // Let's determine how many bytes the distance & length pair take up
                            switch (CompressedData[SourcePointer] >> 4)
                            {
                                case 0: // 3 bytes
                                    Distance = (((CompressedData[SourcePointer + 1] & 0xF) << 8) | CompressedData[SourcePointer + 2]) + 1;
                                    Amount   = (((CompressedData[SourcePointer] & 0xF) << 4) | (CompressedData[SourcePointer + 1] >> 4)) + 17;
                                    SourcePointer += 3;
                                    break;

                                case 1: // 4 bytes
                                    Distance = (((CompressedData[SourcePointer + 2] & 0xF) << 8) | CompressedData[SourcePointer + 3]) + 1;
                                    Amount   = (((CompressedData[SourcePointer] & 0xF) << 12) | (CompressedData[SourcePointer + 1] << 4) | (CompressedData[SourcePointer + 2] >> 4)) + 273;
                                    SourcePointer += 4;
                                    break;

                                default: // 2 bytes
                                    Distance = (((CompressedData[SourcePointer] & 0xF) << 8) | CompressedData[SourcePointer + 1]) + 1;
                                    Amount   = (CompressedData[SourcePointer] >> 4) + 1;
                                    SourcePointer += 2;
                                    break;
                            }

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
                uint DestPointer   = 0x4;

                // Test if the file is too large to be compressed
                if (data.Length > 0xFFFFFFFF)
                    throw new Exception("Input file is too large to compress.");

                // Set up the Lz Compression Dictionary
                LzWindowDictionary LzDictionary = new LzWindowDictionary();
                LzDictionary.SetWindowSize(0x1000);
                LzDictionary.SetMaxMatchAmount(0xFFFF + 273);

                // Figure out where we are going to write the decompressed file size
                if (data.Length <= 0xFFFFFF)
                    CompressedData.Write((uint)('\x11' | (DecompressedSize << 8)));
                else
                {
                    CompressedData.Write((uint)('\x11'));
                    CompressedData.Write(DecompressedSize);
                    DestPointer += 0x4;
                }

                // Start compression
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

                            // Write the distance/length pair
                            if (LzSearchMatch[1] <= 0xF + 1) // 2 bytes
                            {
                                CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 1) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
                                CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));
                                DestPointer += 2;
                            }
                            else if (LzSearchMatch[1] <= 0xFF + 17) // 3 bytes
                            {
                                CompressedData.WriteByte((byte)(((LzSearchMatch[1] - 17) & 0xFF) >> 4));
                                CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 17) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
                                CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));
                                DestPointer += 3;
                            }
                            else // 4 bytes
                            {
                                CompressedData.WriteByte((byte)((1 << 4) | (((LzSearchMatch[1] - 273) & 0xFFFF) >> 12)));
                                CompressedData.WriteByte((byte)(((LzSearchMatch[1] - 273) & 0xFFF) >> 4));
                                CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 273) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
                                CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));
                                DestPointer += 4;
                            }

                            LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);
                            LzDictionary.SlideWindow(LzSearchMatch[1]);

                            SourcePointer += (uint)LzSearchMatch[1];
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

        // Get Filename
        public override string DecompressFilename(Stream data, string filename)
        {
            // Only return a different extension if the current one is onz
            if (Path.GetExtension(filename).ToLower() == ".onz")
                return Path.GetFileNameWithoutExtension(filename) + (Path.GetExtension(filename).IsAllUpperCase() ? ".ONE" : ".one");

            return filename;
        }
        public override string CompressFilename(Stream data, string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".one")
                return Path.GetFileNameWithoutExtension(filename) + (Path.GetExtension(filename).IsAllUpperCase() ? ".ONZ" : ".onz");

            return filename;
        }

        // Check
        public override bool Check(Stream data, string filename)
        {
            try
            {
                // Because this can conflict with other compression formats we are going to add a check them too
                return (data.ReadString(0x0, 1) == "\x11" &&
                    !Compression.Dictionary[CompressionFormat.PRS].Check(data, filename) &&
                    !Textures.Dictionary[TextureFormat.PVR].Check(data, filename));
            }
            catch
            {
                return false;
            }
        }
    }
}