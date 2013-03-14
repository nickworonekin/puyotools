using System;
using System.IO;

namespace PuyoTools.Old
{
    public class ONE : ArchiveModule
    {
        /*
         * ONE Archives are used in PS2 & Wii Sonic Unleashed
        */

        // Main Method
        public ONE()
        {
            Name       = "Unleashed ONE";
            Extension  = ".one";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.one;*.onz" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.ONE();
        }

        // Get the offsets, lengths, and filenames of all the files
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the number of files
                uint files = data.ReadUInt(0x4);

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x40 + (i * 0x40)),      // Offset
                        data.ReadUInt(0x44 + (i * 0x40)),      // Length
                        data.ReadString(0x08 + (i * 0x40), 56) // Filename
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
                //blockSize = 32;

                // Create the header data.
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(Number.RoundUp(0x8 + (files.Length * 0x40), blockSize));
                header.Write(ArchiveHeader.ONE, 4);
                header.Write(files.Length);

                // Set the intial offset
                uint offset = (uint)header.Capacity;

                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write out the information
                    offsetList[i] = offset;
                    header.Write(archiveFilenames[i], 55, 56); // Filename
                    header.Write(offset); // Offset
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

        // Checks to see if the input stream is an ONE archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4, false) == ArchiveHeader.ONE);
            }
            catch
            {
                return false;
            }
        }
    }
}