using System;
using System.IO;
using System.Collections;

namespace GimSharp
{
    public class GimFile
    {
        // - Variables -
        static private byte[] MigMagic = // GIM Magic (Litte Endian)
            { 0x4D, 0x49, 0x47, 0x2E, 0x30, 0x30, 0x2E, 0x31, 0x50, 0x53, 0x50, 0x00 };
        //static private byte[] GimMagic = // GIM Magic (Big Endian)
        //    { 0x47, 0x43, 0x49, 0x58 };
        private byte[] CompressedData;   // GIM Data
        private byte[] DecompressedData; // Regular, Decompressed Data

        private int GimFileHeight;
        private int GimFileWidth;

        //private GimPaletteCodec GimPaletteCodec;
        //private GimDataCodec GimDataCodec;

        private GimPaletteDecoder GimPaletteDecoder;
        private GimDataDecoder GimDataDecoder;

        private byte GimPaletteFormatCode;
        private byte GimDataFormatCode;

        private ushort GimPaletteColors;
        private int GimPaletteSize;

        private bool GimDataSwizzled;

        private int GimPaletteOffset;
        private int GimDataOffset;


        // - Constructors -
        // GimFile(byte[] GimFile)
        // Parameters:
        //  VrFile: A byte array of the GIM file
        // Description: Loads a GIM from a byte array
        public GimFile(byte[] GimFile)
        {
            if (GimFile == null)
            {
                throw new ArgumentException("GimFile(byte[]): Argument 1, 'GimFile', Can not be null.");
            }
            SetCompressedData(GimFile);
        }

        // GimFile(byte[] GimFile)
        // Parameters:
        //  GimFileName: A string to a filename of a GIM
        // Description: Loads a GIM from a file
        public GimFile(string GimFileName)
        {
            FileStream File = new FileStream(GimFileName, FileMode.Open);
            byte[] Data = new byte[File.Length];
            File.Read(Data, 0, Data.Length);
            File.Close();

            SetCompressedData(Data);
        }


        // public byte[] GetCompressedData()
        // Return Value: The byte array of the compressed data
        // Description: Gets the compressed GIM data
        public byte[] GetCompressedData()
        {
            return CompressedData;
        }
        // public byte[] GetCompressedData()
        // Return Value: The byte array of the decompressed data
        // Description: Gets the decompressed data
        public byte[] GetDecompressedData()
        {
            return DecompressedData;
        }


        // - Data Input -
        // Description: Inputs compressed GIM data (And unpacks it to RGBA8888)
        // Parameters:
        //  byte[] Compressed: Compressed Data to load
        // Return Value: True if the data was properly loaded.
        public bool SetCompressedData(byte[] Compressed)
        {
            if (Compressed == null)
            {
                throw new ArgumentException("SetCompressedData: Argument 1, 'Compressed', Can not be null.");
            }
            else
            {
                CompressedData = Compressed;
            }
            if (!IsMig()) throw new NotGimException("The file sent to SetCompressedData() is not a little endian GIM file.");

            // Sometimes, the palette is stored after the image data
            if (CompressedData[0x30] == 0x05)
            {
                // Palette is stored before the image data
                GimPaletteOffset = 0x30;
                GimDataOffset    = 0x30 + BitConverter.ToInt32(CompressedData, 0x30 + 0x8);
            }
            else if (0x30 + BitConverter.ToInt32(CompressedData, 0x30 + 0x8) < CompressedData.Length && CompressedData[0x30 + BitConverter.ToInt32(CompressedData, 0x30 + 0x8)] == 0x05)
            {
                // Palette is stored after the image data
                GimPaletteOffset = 0x30 + BitConverter.ToInt32(CompressedData, 0x30 + 0x8);
                GimDataOffset    = 0x30;
            }
            else
            {
                // No Palette
                GimPaletteOffset = 0x00;
                GimDataOffset    = 0x30;
            }

            // Some files do not have a palette. Trying to get a palette will throw an exception.
            if (GimPaletteOffset > 0x00)
            {
                // Get Palette Information
                GimPaletteFormatCode = Compressed[GimPaletteOffset + 0x14];
                GimPaletteColors     = BitConverter.ToUInt16(Compressed, GimPaletteOffset + 0x18);

                // Set up the palette decoder
                try
                {
                    GimPaletteDecoder = GimCodecs.GetPaletteCodec(GimPaletteFormatCode).Decode;
                    GimPaletteSize    = GimPaletteColors * (GimPaletteDecoder.GetBpp() / 8);
                }
                catch (Exception e)
                {
                    throw new GimCodecLoadingException("The codec for palette format 0x" + GimPaletteFormatCode.ToString("X") + " could not be loaded or does not exist.", e);
                }
            }

            GimFileWidth  = RoundUp(BitConverter.ToUInt16(Compressed, GimDataOffset + 0x18), 16);
            GimFileHeight = RoundUp(BitConverter.ToUInt16(Compressed, GimDataOffset + 0x1A), 8);

            GimDataFormatCode = Compressed[GimDataOffset + 0x14];
            GimDataSwizzled   = (BitConverter.ToUInt16(Compressed, GimDataOffset + 0x16) == 0x01);

            // Set up and initalize the data decoder
            try
            {
                GimDataDecoder = GimCodecs.GetDataCodec(GimDataFormatCode).Decode;
                GimDataDecoder.Initalize(GimFileWidth, GimFileHeight, GimPaletteDecoder, GimPaletteColors);
            }
            catch (Exception e)
            {
                throw new GimCodecLoadingException("The codec for data format 0x" + GimDataFormatCode.ToString("X") + " could not be loaded or does not exist.", e);
            }

            // Decode palette and then start decoding the image
            DecompressedData = new byte[GimFileWidth * GimFileHeight * 4];
            GimDataDecoder.DecodePalette(ref CompressedData, GimPaletteOffset + 0x50);

            if (GimDataSwizzled)
                GimDataDecoder.UnSwizzle(ref CompressedData, GimDataOffset + 0x50);

            GimDataDecoder.DecodeData(ref CompressedData, GimDataOffset + 0x50, ref DecompressedData);

            return true;
        }

        // - File Property Retrieval -
        // public bool IsMig()
        // Return Value: True if the Data Magic is equivalant to that of a GIM Little Endian file
        //               False if not.
        // Description: This function will allow you to validate that the file you have is a Little Endian GIM file.
        static public bool IsMig(byte[] FileContents)
        {
            if (FileContents.Length < 0xC) return false;

            if (IsMagic(FileContents, MigMagic, 0x0))
                return true;

            return false;
        }
        public bool IsMig()
        {
            return IsMig(CompressedData);
        }

        // public bool IsMagic()
        // Return Value: True if the magic is equal.
        //               False if not.
        // Description: This function will allow you to validate the magic matches.
        public static bool IsMagic(byte[] FileContents, byte[] Magic, int Position)
        {
            for (int i = 0; i < Magic.Length; i++)
                if (FileContents[i + Position] != Magic[i]) return false;

            return true;
        }

        public int GetHeight()
        {
            return GimFileHeight;
        }

        public int GetWidth()
        {
            return GimFileWidth;
        }

        // Fix for some GIM files who don't list their file widths and lengths properly
        public int RoundUp(int number, int multiple)
        {
            if (number % multiple == 0)
                return number;

            return number + (multiple - (number % multiple));
        }
    }
}