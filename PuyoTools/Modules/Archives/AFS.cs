using System;
using System.IO;

namespace PuyoTools
{
    public class AFS : ArchiveModule
    {
        /*
         * AFS files are archives that contains files.
         * File names can be up to 32 characters in length.
        */

        // Main Method
        public AFS()
        {
            Name       = "AFS";
            Extension  = ".afs";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.afs" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.AFS();
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

                // Find the metadata location
                uint metadataLocation = data.ReadUInt((files * 0x8) + 0x8);
                if (metadataLocation == 0x0)
                    metadataLocation = data.ReadUInt(data.ReadUInt(0x8) - 0x8);

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x8 + (i * 0x8)), // Offset
                        data.ReadUInt(0xC + (i * 0x8)), // Length
                        (metadataLocation == 0x0 ? String.Empty : data.ReadString(metadataLocation + (i * 0x30), 32)) // Filename
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
                //blockSize = 2048;
                bool v1 = settings[0];

                // Create the header data.
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(Number.RoundUp(0x10 + (files.Length * 0x8), blockSize));
                header.Write(ArchiveHeader.AFS, 4);
                header.Write(files.Length);

                // Set the intial offset
                uint offset = (uint)header.Capacity;

                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write out the information
                    offsetList[i] = offset;
                    header.Write(offset); // Offset
                    header.Write(length); // Length

                    // Increment the offset
                    offset += length.RoundUp(blockSize);
                }

                // Add the location to the metadata
                if (v1) // AFS v1
                    header.Position = header.Capacity - 0x8;

                header.Write(offset);
                header.Write(files.Length * 0x30);

                return header;
            }
            catch
            {
                offsetList = null;
                return null;
            }
        }

        // Create a footer for the archive
        public override MemoryStream CreateFooter(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, MemoryStream header)
        {
            try
            {
                // Create variables from settings
                //blockSize = 2048;
                bool v1                = settings[0];
                bool storeFilenames    = settings[1];
                bool storeCreationTime = settings[2];

                // Create the footer
                MemoryStream footer = new MemoryStream(Number.RoundUp(files.Length * 0x30, blockSize));

                for (int i = 0; i < files.Length; i++)
                {
                    DateTime fileDate = new FileInfo(files[i]).CreationTime;

                    // Write the filename and info
                    if (storeFilenames)
                        footer.Write(archiveFilenames[i], 31, 32);
                    else
                    {
                        for (int j = 0; j < 32; j++)
                            footer.WriteByte(0x0);
                    }

                    if (storeCreationTime)
                    {
                        footer.Write((short)fileDate.Year);
                        footer.Write((short)fileDate.Month);
                        footer.Write((short)fileDate.Day);
                        footer.Write((short)fileDate.Hour);
                        footer.Write((short)fileDate.Minute);
                        footer.Write((short)fileDate.Second);
                    }
                    else
                    {
                        for (int j = 0; j < 12; j++)
                            footer.WriteByte(0x0);
                    }

                    // Store these useless bytes for some reason
                    if (v1) // AFS v1
                        footer.Write(header, 0x8 + (i * 0x8), 4);
                    else // AFS v2
                        footer.Write(header, 0x4 + (i * 0x4), 4);
                }

                return footer;
            }
            catch
            {
                // An error occured
                return null;
            }
        }

        // Checks to see if the input stream is an AFS archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4, false) == ArchiveHeader.AFS);
            }
            catch
            {
                return false;
            }
        }
    }
}