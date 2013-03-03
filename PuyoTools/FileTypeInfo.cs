using System;
using System.IO;

namespace PuyoTools
{
    public static class FileTypeInfo
    {
        public static string GetFileExtension(Stream data)
        {
            // Check to see if the file is an archive
            Archive archive = new Archive(data, null);
            if (archive.Format != ArchiveFormat.NULL)
                return archive.FileExtension;

            // Check to see if the file is an texture
            Textures images = new Textures(data, null);
            if (images.Format != TextureFormat.NULL)
                return images.FileExtension;

            // Check to see if the file is an ADX (special case)
            if (data.Length > 4 && data.ReadUShort(0x00) == 0x0080 &&
                data.Length > data.ReadUShort(0x02).SwapEndian() + 4 &&
                data.ReadString(data.ReadUShort(0x02).SwapEndian() - 0x02, 6, true) == "(c)CRI")
                return ".adx";

            // Unknown
            return String.Empty;
        }
    }
}