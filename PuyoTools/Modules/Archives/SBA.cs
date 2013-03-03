using System;
using System.IO;

namespace PuyoTools
{
    public class SBA : ArchiveModule
    {
        /*
         * Storybook Archives contain any type of file that is PRS compressed.
         * Only used in the Sonic Storybook Series.
        */

        // Main Method
        public SBA()
        {
            Name       = "Storybook ONE";
            Extension  = ".one";
            CanPack    = false;
            CanExtract = true;
            Translate  = true;

            Filter       = new string[] { Name + " Archive", "*.one" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.SBA();
        }

        // Get the offsets, lengths, and filenames of all the files
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the number of files
                uint files = data.ReadUInt(0x0);

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x4 + (i * 0x2C)), // Offset
                        data.ReadUInt(0x8 + (i * 0x2C)), // Length
                        data.ReadString(0xC + (i * 0x2C), 36) // Filename
                    );
                }

                return fileList;
            }
            catch
            {
                return null;
            }
        }

        /* To simplify the process greatly, we are going to convert
         * the Storybook Archive to a new format */
        public override MemoryStream TranslateData(Stream stream)
        {
            try
            {
                // Get the number of files
                uint files = stream.ReadUInt(0x0).SwapEndian();

                // Now create the header
                MemoryStream data = new MemoryStream();
                data.Write(files);

                // Write each file in the header
                uint offset = 0xC + (files * 0x2C);
                for (int i = 0; i < files; i++)
                {
                    uint length = stream.ReadUInt(0x3C + (i * 0x30)).SwapEndian();

                    data.Write(offset); // Offset
                    data.Write(length); // Length
                    data.Write(stream.ReadString(0x10 + (i * 0x30), 36), 36); // Filename

                    // Let's write the decompressed data
                    uint sourceOffset     = stream.ReadUInt(0x34 + (i * 0x30)).SwapEndian();
                    uint sourceLength     = stream.ReadUInt(0x38 + (i * 0x30)).SwapEndian();
                    Stream compressedData = stream.Copy(sourceOffset, sourceLength);

                    // Decompress the data
                    PRS decompressor = new PRS();
                    //MemoryStream decompressedData = decompressor.Decompress(compressedData, length);
                    MemoryStream decompressedData = decompressor.Decompress(compressedData);
                    if (decompressedData == null)
                        throw new Exception();

                    // Write the data
                    data.Position = offset;
                    data.Write(decompressedData);
                    data.Position = 0x30 + (i * 0x2C);
                    decompressedData.Close();

                    offset += length;
                }

                return data;
            }
            catch
            {
                return new MemoryStream();
            }
        }

        public override MemoryStream CreateHeader(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, out uint[] offsetList)
        {
            offsetList = null;
            return null;
        }

        // Checks to see if the input stream is a Storybook archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadUInt(0x4).SwapEndian() == 0x10 &&
                   (input.ReadUInt(0xC).SwapEndian() == 0xFFFFFFFF ||
                    input.ReadUInt(0xC).SwapEndian() == 0x00000000));
            }
            catch
            {
                return false;
            }
        }
    }
}