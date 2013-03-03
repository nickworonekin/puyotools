using System;
using System.IO;

namespace PuyoTools
{
    public class NARC : ArchiveModule
    {
        /*
         * NARC files are archives used on the Nintendo DS.
         * NARC files may or may not contain filenames.
        */

        // Main Method
        public NARC()
        {
            Name       = "NARC";
            Extension  = ".narc";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.narc;*.carc" };
            PaddingByte  = 0xFF;
            PackSettings = new ArchivePackSettings.NARC();
        }

        // Get the offsets, lengths, and filenames of all the files
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the offset of each section of the NARC file
                uint offset_fatb = data.ReadUShort(0xC);
                uint offset_fntb = offset_fatb + data.ReadUInt(offset_fatb + 0x4);
                uint offset_fimg = offset_fntb + data.ReadUInt(offset_fntb + 0x4);

                // Stuff for filenames
                bool containsFilenames = (data.ReadUInt(offset_fntb + 0x8) == 8);
                uint offset_filename   = offset_fntb + 0x10;

                // Get the number of files
                uint files = data.ReadUInt(offset_fatb + 0x8);

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    // Get the offset & length
                    uint offset = data.ReadUInt(offset_fatb + 0x0C + (i * 0x8));
                    uint length = data.ReadUInt(offset_fatb + 0x10 + (i * 0x8)) - offset;

                    // Get the filename, if the NARC contains filenames
                    string filename = String.Empty;
                    if (containsFilenames)
                    {
                        // Ok, since the NARC contains filenames, let's go grab it now
                        byte filename_length = data.ReadByte(offset_filename);
                        filename             = data.ReadString(offset_filename + 1, filename_length);
                        offset_filename     += (uint)(filename_length + 1);
                    }

                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        offset + offset_fimg + 0x8, // Offset
                        length,  // Length
                        filename // Filename
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

        // Add a header to a blank archive
        public override MemoryStream CreateHeader(string[] files, string[] storedFilenames, int blockSize, bool[] settings, out uint[] offsetList)
        {
            try
            {
                // Create variables from settings
                blockSize         = 4;
                bool addFilenames = settings[0];

                // Get the sizes for each section of the narc
                uint size_fatb = 12 + (uint)(files.Length * 8);
                uint size_fntb = 16;
                uint size_fimg = 8;

                // Add the size of the filenames, if we are adding filenames
                if (addFilenames)
                {
                    foreach (string file in storedFilenames)
                        size_fntb += (1 + Math.Min((uint)file.Length, 255));

                    size_fntb++;
                    size_fntb = size_fntb.RoundUp(blockSize);
                }

                // Add the size for the files
                foreach (string file in files)
                    size_fimg += Number.RoundUp((uint)new FileInfo(file).Length, blockSize);

                // Ok, get the offsets for each section
                uint offset_fatb = 0x10;
                uint offset_fntb = offset_fatb + size_fatb;
                uint offset_fimg = offset_fntb + size_fntb;

                // Create the header
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream((int)offset_fimg + 8);

                // Write out the NARC header
                header.Write(ArchiveHeader.NARC, 4); // NARC
                header.Write(new byte[] { 0xFE, 0xFF, 0x00, 0x01 });    // Fixed Values
                header.Write(0x10 + size_fatb + size_fntb + size_fimg); // File Size
                header.Write(new byte[] { 0x10, 0x00, 0x03, 0x00 });    // Fixed Values

                // Write our the FATB header
                header.Write("BTAF");       // FATB
                header.Write(size_fatb);    // FATB Size
                header.Write(files.Length); // Number of Files

                // Get the offset & length for the file
                uint offset = offset_fimg + 8;
                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    offsetList[i] = offset;
                    header.Write(offset - offset_fimg - 8);          // Offset
                    header.Write(offset + length - offset_fimg - 8); // Length

                    offset += length.RoundUp(blockSize);
                }

                // Write out the FNTB header
                header.Write("BTNF"); // FNTB
                header.Write(size_fntb); // FNTB Size
                header.Write((addFilenames ? 8 : 4)); // NARC contains filenames (8) or not (4)
                header.Write(new byte[] { 0x00, 0x00, 0x01, 0x00 }); // Fixed Values

                // Write out the filenames
                if (addFilenames)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        header.Write((byte)Math.Min(storedFilenames[i].Length, 255)); // Length of filename
                        header.Write(storedFilenames[i], Math.Min(storedFilenames[i].Length, 255)); // Filename
                    }

                    // Pad the file if we are not at the FIMG section yet
                    while (header.Position < offset_fimg)
                        header.WriteByte(PaddingByte);
                }

                // Write out the FIMG header
                header.Write("GMIF"); // FIMG
                header.Write(size_fimg); // FIMG Size

                return header;
            }
            catch
            {
                // Something went wrong, so return nothing
                offsetList = null;
                return null;
            }
        }

        // Checks to see if the input stream is a NARC archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4) == ArchiveHeader.NARC);
            }
            catch
            {
                return false;
            }
        }
    }
}