using System;
using System.IO;
using Extensions;

namespace PuyoTools
{
    public class GNT : ArchiveModule
    {
        /*
         * GNT files are archives that contains GVR files.
         * No filenames are stored in the SNT file.
        */

        // Main Method
        public GNT()
        {
            Name       = "GNT";
            Extension  = ".gnt";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.gnt" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.GNT();
        }

        // Get the offsets, lengths, and filenames of all the files
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the number of files
                uint files = data.ReadUInt(0x30).SwapEndian();

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // See if the archive contains filenames
                bool containsFilenames = (files > 0 && data.ReadUInt(0x3C + (files * 0x14)).SwapEndian() + 0x20 != 0x3C + (files * 0x1C) && data.ReadString(0x3C + (files * 0x1C), 4) == "FLST");

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x40 + (files * 0x14) + (i * 0x8)).SwapEndian() + 0x20, // Offset
                        data.ReadUInt(0x3C + (files * 0x14) + (i * 0x8)).SwapEndian(),        // Length
                        (containsFilenames ? data.ReadString(0x40 + (files * 0x1C) + (i * 0x40), 64) : string.Empty) // Filename
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
                //blockSize         = 8;
                bool addFilenames = settings[0];

                // Create the header data.
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(0x3C + (files.Length * 0x1C));

                // Start with the NSIF/NUIF header
                header.Write(ArchiveHeader.NGIF, 4);
                header.Write(0x18);
                header.Write(Endian.Swap(0x01));
                header.Write(Endian.Swap(0x20));

                // Get the size for the NTL data
                uint ntl_size = 0;
                foreach (string file in files)
                    ntl_size += Number.RoundUp((uint)new FileInfo(file).Length, blockSize);

                header.Write(Endian.Swap(0x1C + ((uint)files.Length * 0x1C) + ntl_size));
                header.Write(Endian.Swap(0x3C + ((uint)files.Length * 0x1C) + ntl_size));
                header.Write(Endian.Swap(0x18 + (files.Length * 4)));
                header.Write(Endian.Swap(0x01));

                // NTL Header
                header.Write(ArchiveHeader.NGTL, 4);
                header.Write(0x14 + ((uint)files.Length * 0x1C) + ntl_size);
                header.Write(Endian.Swap(0x10));
                header.Write(0x00);
                header.Write(files.Length.SwapEndian());
                header.Write(Endian.Swap(0x1C));
                header.Write(Endian.Swap(((uint)files.Length * 0x14) + 0x1C));

                // Start adding the crap bytes at the top
                for (int i = 0; i < files.Length; i++)
                {
                    header.Write(0x00);
                    header.Write(0x00);
                    header.Write(new byte[] { 0x0, 0x1, 0x0, 0x1 });
                    header.Write(i.SwapEndian());
                    header.Write(0x00);
                }

                // Set the intial offset
                if (addFilenames)
                    header.Capacity += 0x4 + (0x40 * files.Length);
                uint offset = (uint)header.Capacity;

                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write out the information
                    offsetList[i] = offset;
                    header.Write(length.SwapEndian());        // Length
                    header.Write(Endian.Swap(offset - 0x20)); // Offset

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
                blockSize = 16;

                // Create the footer
                MemoryStream footer = new MemoryStream(Number.RoundUp(0x14 + (files.Length * 0x4), blockSize) + Number.RoundUp(0x4, blockSize));
                footer.Write("NOF0");

                // Write the crap data on the footer
                footer.Write(0x14 + (files.Length * 0x4));
                footer.Write(Endian.Swap(files.Length + 2));
                footer.Write(0x00);
                footer.Write(Endian.Swap(0x14));

                // Write this number for whatever reason
                for (int i = 0; i < files.Length; i++)
                    footer.Write(Endian.Swap(0x20 + ((uint)files.Length * 0x14) + ((uint)i * 0x8)));

                footer.Write(Endian.Swap(0x18));

                // Pad data before NEND
                while (footer.Position % blockSize != 0)
                    footer.WriteByte(PaddingByte);

                // Add the NEND stuff and then pad file
                footer.Write("NEND");
                while (footer.Position < footer.Capacity)
                    footer.WriteByte(PaddingByte);

                return footer;
            }
            catch
            {
                // An error occured
                return null;
            }
        }

        // Checks to see if the input stream is a GNT archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4, false) == ArchiveHeader.NGIF &&
                    input.ReadString(0x20, 4) == ArchiveHeader.NGTL);
            }
            catch
            {
                return false;
            }
        }
    }
}