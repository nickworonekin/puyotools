using System;
using System.IO;

namespace PuyoTools
{
    public class VDD : ArchiveModule
    {
        /*
         * VDD files are archives that contains files.
         * File names can be up to 16 characters in length.
        */

        // Main Method
        public VDD()
        {
            Name       = "VDD";
            Extension  = ".vdd";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.vdd" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.VDD();
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
                for (int i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x14   + (i * 0x18)) * 0x800, // Offset
                        data.ReadUInt(0x18   + (i * 0x18)),         // Length
                        data.ReadString(0x04 + (i * 0x18), 16)      // Filename
                    );
                }

                return fileList;
            }
            catch
            {
                // Something went wrong, so return nothing
                return null;
            }
        }

        // Create a header for an archive
        public override MemoryStream CreateHeader(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, out uint[] offsetList)
        {
            try
            {
                // Create variables from settings
                blockSize = 2048;

                // Create the header data.
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(Number.RoundUp(0x4 + (files.Length * 0x18), blockSize));
                header.Write(files.Length);

                // Set the intial offset
                uint offset = (uint)header.Capacity;

                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write out the information
                    offsetList[i] = offset;
                    header.Write(archiveFilenames[i], 15, 16); // Filename
                    header.Write(offset / 0x800); // Offset
                    header.Write(length); // Length

                    // Increment the offset
                    offset += length.RoundUp(blockSize);
                }

                return header;
            }
            catch
            {
                offsetList = null;
                return null;
            }
        }

        // Checks to see if the input stream is a VDD archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.Length % 2048 == 0 && Path.GetExtension(filename) == ".vdd");
            }
            catch
            {
                return false;
            }
        }
    }
}