using System;
using System.IO;
using Extensions;
using System.Collections.Generic;

namespace PuyoTools
{
    public class CNX : CompressionModule
    {
        /* CNX cracked by drx (Luke Zapart)
         * <thedrx@gmail.com> */

        public CNX()
        {
            Name = "CNX";
            CanCompress   = false;
            CanDecompress = true;
        }

        // Decompress
        public override MemoryStream Decompress(Stream data)
        {
            try
            {
                // Set variables
                uint compressedSize   = data.ReadUInt(0x8).SwapEndian() + 16; // Compressed Size
                uint decompressedSize = data.ReadUInt(0xC).SwapEndian();      // Decompressed Size

                uint Cpointer = 0x10; // Compressed Pointer
                uint Dpointer = 0x0;  // Decompressed Pointer

                byte[] compressedData   = data.ReadBytes(0x0, compressedSize); // Compressed Data
                byte[] decompressedData = new byte[decompressedSize]; // Decompressed Data

                // Ok, let's decompress the data
                while (Cpointer < compressedSize && Dpointer < decompressedSize)
                {
                    byte Cflag = compressedData[Cpointer];
                    Cpointer++;

                    for (int i = 0; i < 4; i++)
                    {
                        // Check for the mode
                        switch ((Cflag >> (i * 2)) & 3)
                        {
                            /* Padding Mode
					         * All CNX archives seem to be packed in 0x800 chunks. when nearing
					         * a 0x800 cutoff, there usually is a padding command at the end to skip
					         * a few bytes (to the next 0x800 chunk, i.e. 0x4800, 0x7000, etc.) */
                            case 0:
                                byte temp_byte = compressedData[Cpointer];
                                Cpointer      += (uint)(temp_byte & 0xFF) + 1;

                                i = 3;
                                break;

                            // Single Byte Copy Mode
                            case 1:
                                decompressedData[Dpointer] = compressedData[Cpointer];
                                Cpointer++;
                                Dpointer++;
                                break;

                            // Copy from destination buffer to current position
                            case 2:
                                uint temp_word = BitConverter.ToUInt16(compressedData, (int)Cpointer).SwapEndian();

                                uint off = (temp_word >> 5)   + 1;
                                uint len = (temp_word & 0x1F) + 4;

                                Cpointer += 2;

                                for (int j = 0; j < len; j++)
                                {
                                    decompressedData[Dpointer] = decompressedData[Dpointer - off];
                                    Dpointer++;
                                }

                                break;

                            // Direct Block Copy (first byte signifies length of copy)
                            case 3:
                                byte blockLength = compressedData[Cpointer];
                                Cpointer++;

                                for (int j = 0; j < blockLength; j++)
                                {
                                    decompressedData[Dpointer] = compressedData[Cpointer];
                                    Cpointer++;
                                    Dpointer++;
                                }

                                break;
                        }
                    }
                }

                // Finished decompression, now return the data
                return new MemoryStream(decompressedData);
            }
            catch (Exception f)
            {
                // An error occured
                System.Windows.Forms.MessageBox.Show(f.ToString());
                return null;
            }
        }

        // Compress
        public override MemoryStream Compress(Stream data, string filename)
        {
            try
            {
                uint DecompressedSize = (uint)data.Length;
                uint BlockSize = 0x800;

                MemoryStream CompressedData = new MemoryStream();
                byte[] DecompressedData     = data.ToByteArray();

                uint SourcePointer = 0x0;
                uint DestPointer   = 0x10;
                uint BlockPointer  = 0x10;

                // Test if the file is too large to be compressed
                if (data.Length > 0xFFFFFFFFu)
                    throw new Exception("Input file is too large to compress.");

                // Set up the Lz Compression Dictionary
                LzWindowDictionary LzDictionary = new LzWindowDictionary();
                LzDictionary.SetBlockSize(0x800);
                LzDictionary.SetMinMatchAmount(4);
                LzDictionary.SetMaxMatchAmount(0x1F + 4);

                // Write out the header
                CompressedData.Write("CNX\x2");
                CompressedData.Write((Path.HasExtension(filename) ? Path.GetExtension(filename).Substring(1) : String.Empty), 3);
                CompressedData.WriteByte(0x10);
                CompressedData.Write((uint)0); // Compressed size (we will set it later)
                CompressedData.Write(DecompressedSize.SwapEndian());

                // Start compression
                while (SourcePointer < DecompressedSize)
                {
                    while (BlockPointer < BlockSize)
                    {
                        byte Flag = 0x0;
                        uint FlagPosition = DestPointer;
                        CompressedData.WriteByte(Flag); // It will be filled in later
                        DestPointer++;

                        for (int i = 0; i < 4; i++)
                        {
                            List<byte> UnmatchedBytes = new List<byte>();
                            int[] LzSearchMatch = new int[] { 0, 0 };
                            do // Do a search for unmatched bytes
                            {
                                LzSearchMatch = LzDictionary.Search(DecompressedData, SourcePointer, DecompressedSize);
                                if (LzSearchMatch[1] == 0)
                                {
                                    UnmatchedBytes.Add(DecompressedData[SourcePointer]);
                                    SourcePointer++;
                                }
                            }
                            while (LzSearchMatch[1] == 0 && UnmatchedBytes.Count < 0xFF);

                            if (LzSearchMatch[1] > 0) // We reached a compression match
                            {
                                Flag |= (byte)(2 << (i * 2));

                                CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 3) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
                                CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));

                                LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);
                                LzDictionary.SlideWindow(LzSearchMatch[1]);

                                SourcePointer += (uint)LzSearchMatch[1];
                                DestPointer += 2;
                            }
                            else // We did not find any matches
                            {
                                if (UnmatchedBytes.Count == 1) // We must have hit the end of the block
                                {
                                    Flag |= (byte)(1 << (i * 2));
                                    CompressedData.WriteByte(UnmatchedBytes[0]);
                                }
                                else // We hit the 0xFF limit or the end of the 0x800 block
                                {
                                    Flag |= (byte)(3 << (i * 2));

                                    CompressedData.WriteByte((byte)UnmatchedBytes.Count);
                                    for (int j = 0; j < UnmatchedBytes.Count; j++)
                                        CompressedData.WriteByte(UnmatchedBytes[j]);
                                }
                                Flag |= (byte)(0 << (i * 2));

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

                        // End of block?
                        if (BlockPointer >= 0x800 - 2)
                        {
                        }

                        // Write the flag.
                        // Note that the original position gets reset after writing.
                        CompressedData.Seek(FlagPosition, SeekOrigin.Begin);
                        CompressedData.WriteByte(Flag);
                        CompressedData.Seek(DestPointer, SeekOrigin.Begin);
                    }
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
            string fileext = data.ReadString(0x4, 3);
            return (fileext == string.Empty ? filename : Path.GetFileNameWithoutExtension(filename) + '.' + fileext);
        }
        public override string CompressFilename(Stream data, string filename)
        {
            return Path.GetFileNameWithoutExtension(filename) + (Path.GetExtension(filename).IsAllUpperCase() ? ".CNX" : ".cnx");
        }

        // Check
        public override bool Check(Stream data, string filename)
        {
            try
            {
                return (data.ReadString(0x0, 4) == "CNX\x02");
            }
            catch
            {
                return false;
            }
        }
    }
}