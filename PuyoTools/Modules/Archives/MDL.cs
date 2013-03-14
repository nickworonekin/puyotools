using System;
using System.IO;

namespace PuyoTools.Old
{
    public class MDL : ArchiveModule
    {
        /*
         * MDL files are archives that contain PVM, NJ, and NM files.
         * No filenames are stored in the MDL file.
        */

        // Main Method
        public MDL()
        {
            Name       = "MDL";
            Extension  = ".mdl";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.mdl" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.MDL();
        }

        // Get the offsets, lengths, and filenames of all the files
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the number of files
                uint files = data.ReadUShort(0x2);

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // See if the archive contains filenames
                bool containsFilenames = (files > 0 && data.ReadUInt(0x10) != 0xC + (files * 0xC) && data.ReadString(0xC + (files * 0xC), 4) == "FLST");

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x10 + (i * 0xC)), // Offset
                        data.ReadUInt(0x0C + (i * 0xC)), // Length
                        (containsFilenames ? data.ReadString(0xC + (files * 0xC) + (i * 0x40), 64) : string.Empty) // Filename
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
                // Seperate settings
                //blockSize = 4096;
                bool addFilenames = settings[0];

                // Create the header and offset list
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(0x4 + (files.Length * 0x8));
                header.Write("\x02\x00", 2);
                header.Write((ushort)files.Length);

                // Set the intial offset
                if (addFilenames)
                    header.Capacity += 0x4 + (0x40 * files.Length);
                header.Capacity = header.Capacity.RoundUp(blockSize);
                uint offset     = (uint)header.Capacity;

                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write out the information
                    offsetList[i] = offset;
                    header.Write(length.SwapEndian()); // Length
                    header.Write(offset.SwapEndian()); // Offset

                    // Increment the offset
                    offset += length.RoundUp(blockSize);
                }

                // Do we want to add filenames?
                if (addFilenames)
                {
                    header.Write("FLST");
                    for (int i = 0; i < files.Length; i++)
                        header.Write(archiveFilenames[i], 63, 64);
                }

                return header;
            }
            catch
            {
                // An error occured.
                offsetList = null;
                return null;
            }
        }

        // Checks to see if the input stream is a MDL archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadUShort(0x0) == 0x2 &&
                    Path.GetExtension(filename).ToLower() == ".mdl");
            }
            catch
            {
                return false;
            }
        }
    }
}