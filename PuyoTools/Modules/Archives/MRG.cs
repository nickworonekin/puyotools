using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PuyoTools
{
    public class MRG : ArchiveModule
    {
        /*
         * MRG files are archives that contain files (duh!).
        */

        // Main Method
        public MRG()
        {
            Name       = "MRG";
            Extension  = ".mrg;*.mrz";
            CanPack    = true;
            CanExtract = true;
            Translate  = false;

            Filter       = new string[] { Name + " Archive", "*.mrg;*.mrz" };
            PaddingByte  = 0x00;
            PackSettings = new ArchivePackSettings.MRG();
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
                    // Get filename and extension
                    string filename = data.ReadString(0x20 + (i * 0x30), 32, Encoding.GetEncoding("Shift_JIS")); // Name
                    string fileext  = data.ReadString(0x10 + (i * 0x30), 4);  // Extension

                    fileList.Entries[i] = new ArchiveFileList.Entry(
                        data.ReadUInt(0x14 + (i * 0x30)), // Offset
                        data.ReadUInt(0x18 + (i * 0x30)), // Length
                        (filename == String.Empty ? String.Empty : filename) + (fileext == string.Empty ? string.Empty : '.' + fileext) // Filename
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
                //blockSize = 16;

                // Create the header data.
                offsetList          = new uint[files.Length];
                MemoryStream header = new MemoryStream(Number.RoundUp(0x8, blockSize) + (Number.RoundUp(0xB, blockSize) * files.Length) + (Number.RoundUp(0x20, blockSize) * files.Length));

                // Write out the identifier and number of files
                header.Write(ArchiveHeader.MRG, 4);
                header.Write(files.Length);

                // Write null bytes
                while (header.Position % blockSize != 0)
                    header.Write(PaddingByte);

                // Set the offset
                uint offset = (uint)header.Capacity;

                // Now add the filenames, offsets and lengths
                for (int i = 0; i < files.Length; i++)
                {
                    uint length = (uint)new FileInfo(files[i]).Length;

                    // Write the file extension
                    string fileext = Path.GetExtension(archiveFilenames[i]);
                    header.Write((fileext == String.Empty ? String.Empty : fileext.Substring(1)), 3, 4);

                    // Write the offsets and lengths
                    offsetList[i] = offset;
                    header.Write(offset);
                    header.Write(length);

                    // Write null bytes
                    while (header.Position % blockSize != 0)
                        header.Write(PaddingByte);

                    // Write the filename
                    header.Write(Path.GetFileNameWithoutExtension(archiveFilenames[i]), 31, 32, Encoding.GetEncoding("Shift_JIS"));

                    // Now increment the offset
                    offset += length.RoundUp(blockSize);
                }

                return header;
            }
            catch
            {
                // Something went wrong, so return nothing
                offsetList = null;
                return null;
            }
        }

        // Checks to see if the input stream is a MRG archive
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 4) == ArchiveHeader.MRG);
            }
            catch
            {
                return false;
            }
        }
    }
}