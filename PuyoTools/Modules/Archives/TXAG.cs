using System;
using System.IO;

namespace PuyoTools
{
    public class TXAG : ArchiveModule
    {
        /*
         * TXAG archives are TXD archives in the Sonic Storybook Series.
         * They are named TXAG to reduce confusion with Renderware TXD files.
         * They contain GVR textures.
        */

        // Main Method
        public TXAG()
        {
            Name       = "Storybook TXD";
            Extension  = ".txd";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.txd" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.TXAG();
        }

        // Get the offsets, lengths, and filenames of all the files
        public override ArchiveFileList GetFileList(Stream data)
        {
            try
            {
                // Get the number of files
                uint files = data.ReadUInt(0x4).SwapEndian();

                // Create the array of files now
                ArchiveFileList fileList = new ArchiveFileList(files);

                // Now we can get the file offsets, lengths, and filenames
                for (uint i = 0; i < files; i++)
                {
                    string filename = data.ReadString(0x10 + (i * 0x28), 32);

                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x08 + (i * 0x28)).SwapEndian(), // Offset
                        data.ReadUInt(0x0C + (i * 0x28)).SwapEndian(), // Length
                        (filename == String.Empty ? String.Empty : filename + (filename.IsAllUpperCase() ? ".GVR" : ".gvr")) // Filename
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
                //blockSize = 64;

                // Create the header data.
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(Number.RoundUp(0x8 + (files.Length * 0x28), blockSize));
                header.Write(ArchiveHeader.TXAG, 4);
                header.Write(files.Length.SwapEndian());

                // Set the intial offset
                uint offset = (uint)header.Capacity;

                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write out the information
                    offsetList[i] = offset;
                    header.Write(offset.SwapEndian()); // Offset
                    header.Write(length.SwapEndian()); // Length
                    header.Write(Path.GetFileNameWithoutExtension(archiveFilenames[i]), 31, 32); // Filename

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

        // Checks to see if the input stream is a TXAG archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4) == ArchiveHeader.TXAG);
            }
            catch
            {
                return false;
            }
        }
    }
}