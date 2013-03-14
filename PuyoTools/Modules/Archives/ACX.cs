using System;
using System.IO;

namespace PuyoTools.Old
{
    public class ACX : ArchiveModule
    {
        /*
         * ACX files are archives that contains ADX files.
         * No filenames are stored in the ACX file.
        */

        // Main Method
        public ACX()
        {
            Name       = "ACX";
            Extension  = ".acx";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.acx" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.ACX();
        }

        // Get file list containing the entries in the archive
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the number of files
                uint files = data.ReadUInt(0x4).SwapEndian();

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // See if the archive contains filenames
                bool containsFilenames = (files > 0 && data.ReadUInt(0x8) != 0x8 + (files * 0x8) && data.ReadString(0x8 + (files * 0x8), 4) == "FLST");

                // Now we can get the file offsets, lengths, and filenames
                for (int i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x8 + (i * 0x8)).SwapEndian(), // Offset
                        data.ReadUInt(0xC + (i * 0x8)).SwapEndian(), // Length
                        (containsFilenames ? data.ReadString(0xC + (files * 0x8) + (i * 0x40), 64) : string.Empty) // Filename
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

        // Create a header for the archive
        public override MemoryStream CreateHeader(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, out uint[] offsetList)
        {
            try
            {
                // Seperate settings
                //blockSize = 4;
                bool addFilenames = settings[0];

                // Create the header and offset list
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(0x8 + (files.Length * 0x8));
                header.Write(ArchiveHeader.ACX, 4);
                header.Write(files.Length.SwapEndian());

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
                    header.Write(offset.SwapEndian()); // Offset
                    header.Write(length.SwapEndian()); // Length

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

        // Check to see if the following data is an ACX
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4, false) == ArchiveHeader.ACX &&
                    Path.GetExtension(filename).ToLower() == ".acx");
            }
            catch
            {
                return false;
            }
        }
    }
}