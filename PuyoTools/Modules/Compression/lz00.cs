using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Extensions;

namespace PuyoTools
{
    public class LZ00 : CompressionModule
    {
        /* LZ00 Cracked by QPjDDYwQLI
             thanks to author of ps2dis
        */

        public LZ00()
        {
            Name          = "LZ00";
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
                uint DecompressedSize = data.ReadUInt(0x30);
                uint MagicValue       = data.ReadUInt(0x34);

                byte[] CompressedData   = data.ToByteArray();
                byte[] DecompressedData = new byte[DecompressedSize];
                byte[] DestBuffer       = new byte[0x1000];

                uint SourcePointer = 0x40;
                uint DestPointer   = 0x0;
                uint BufferPointer = 0xFEE;

                // Start Decompression
                while (SourcePointer < CompressedSize && DestPointer < DecompressedSize)
                {
                    MagicValue = GetNewMagicValue(MagicValue);
                    byte Flag = DecryptByte(CompressedData[SourcePointer], MagicValue); // Compression Flag
                    SourcePointer++;

                    for (int i = 0; i < 8; i++)
                    {
                        if ((Flag & (1 << i)) > 0) // Data is not compressed
                        {
                            MagicValue = GetNewMagicValue(MagicValue);
                            DecompressedData[DestPointer] = DecryptByte(CompressedData[SourcePointer], MagicValue);
                            DestBuffer[BufferPointer]     = DecompressedData[DestPointer];
                            SourcePointer++;
                            DestPointer++;
                            BufferPointer = (BufferPointer + 1) & 0xFFF;
                        }
                        else // Data is compressed
                        {
                            MagicValue      = GetNewMagicValue(MagicValue);
                            byte PairFirst  = DecryptByte(CompressedData[SourcePointer], MagicValue);
                            MagicValue      = GetNewMagicValue(MagicValue);
                            byte PairSecond = DecryptByte(CompressedData[SourcePointer + 1], MagicValue);

                            int Offset = ((((PairSecond >> 4) & 0xF) << 8) | PairFirst);
                            int Amount = (PairSecond & 0xF) + 3;
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
                uint DestPointer   = 0x40;

                uint MagicValue = (uint)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

                // Test if the file is too large to be compressed
                if (data.Length > 0xFFFFFFFF)
                    throw new Exception("Input file is too large to compress.");

                // Set up the Lz Compression Dictionary
                LzBufferDictionary LzDictionary = new LzBufferDictionary();
                LzDictionary.SetBufferSize(0x1000);
                LzDictionary.SetBufferStart(0xFEE);
                LzDictionary.SetMaxMatchAmount(0xF + 3);

                // Start compression
                CompressedData.Write("LZ00");
                CompressedData.Write(0u); // Will be filled in later
                CompressedData.Seek(8, SeekOrigin.Current); // Advance 8 bytes

                // If the file extension is MRZ or TEZ, we probably want to change it
                if (Path.GetExtension(filename).ToLower() == ".mrz")
                    filename = Path.GetFileNameWithoutExtension(filename) + ".mrg";
                else if (Path.GetExtension(filename).ToLower() == ".tez")
                    filename = Path.GetFileNameWithoutExtension(filename) + ".tex";

                CompressedData.Write(filename, 31, 32, Encoding.GetEncoding("Shift_JIS"));
                CompressedData.Write(DecompressedSize);
                CompressedData.Write(MagicValue);
                CompressedData.Seek(8, SeekOrigin.Current); // Advance 8 bytes

                while (SourcePointer < DecompressedSize)
                {
                    MagicValue = GetNewMagicValue(MagicValue);

                    byte Flag = 0x0;
                    uint FlagPosition   = DestPointer;
                    uint FlagMagicValue = MagicValue; // Since it won't be filled in now
                    CompressedData.WriteByte(Flag); // It will be filled in later
                    DestPointer++;

                    for (int i = 0; i < 8; i++)
                    {
                        int[] LzSearchMatch = LzDictionary.Search(DecompressedData, SourcePointer, DecompressedSize);
                        if (LzSearchMatch[1] > 0) // There is a compression match
                        {
                            Flag |= (byte)(0 << i);

                            MagicValue = GetNewMagicValue(MagicValue);
                            CompressedData.WriteByte(EncryptByte((byte)(LzSearchMatch[0] & 0xFF), MagicValue));
                            MagicValue = GetNewMagicValue(MagicValue);
                            CompressedData.WriteByte(EncryptByte((byte)(((LzSearchMatch[0] & 0xF00) >> 4) | ((LzSearchMatch[1] - 3) & 0xF)), MagicValue));

                            LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);

                            SourcePointer += (uint)LzSearchMatch[1];
                            DestPointer   += 2;
                        }
                        else // There wasn't a match
                        {
                            Flag |= (byte)(1 << i);

                            MagicValue = GetNewMagicValue(MagicValue);
                            CompressedData.WriteByte(EncryptByte(DecompressedData[SourcePointer], MagicValue));

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
                    CompressedData.WriteByte(EncryptByte(Flag, FlagMagicValue));
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

        // Get the new magic value
        private uint GetNewMagicValue(uint xValue)
        {
            uint x;

            x = (((((((xValue << 1) + xValue) << 5) - xValue) << 5) + xValue) << 7) - xValue;
            x = (x << 6) - x;
            x = (x << 4) - x;

            return ((x << 2) - x) + 12345;
        }

        // Decrypt & Encrypt bytes (they are the same function really)
        private byte DecryptByte(byte value, uint xValue)
        {
            uint t0 = ((uint)xValue >> 16) & 0x7fff;
            return (byte)(value ^ ((uint)(((t0 << 8) - t0) >> 15)));
        }
        private byte EncryptByte(byte value, uint xValue)
        {
            uint t0 = ((uint)xValue >> 16) & 0x7fff;
            return (byte)(value ^ ((uint)(((t0 << 8) - t0) >> 15)));
        }


        // Get the filename
        public override string DecompressFilename(Stream data, string filename)
        {
            string EmbeddedFilename = data.ReadString(0x10, 32, Encoding.GetEncoding("Shift_JIS"));
            return (EmbeddedFilename == String.Empty ? filename : EmbeddedFilename);
        }
        public override string CompressFilename(Stream data, string filename)
        {
            switch (Path.GetExtension(filename))
            {
                case ".mrg": return Path.GetFileNameWithoutExtension(filename) + ".mrz";
                case ".tex": return Path.GetFileNameWithoutExtension(filename) + ".tez";
            }

            return filename;
        }

        // Check
        public override bool Check(Stream data, string filename)
        {
            try
            {
                return (data.ReadString(0x0, 4) == "LZ00");
            }
            catch
            {
                return false;
            }
        }
    }
}