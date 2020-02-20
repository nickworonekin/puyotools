using PuyoTools.Modules.Compression;
using PuyoTools.Modules.Texture;
using System;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class TxdStorybookArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "TXD (Sonic and the Secret Rings)"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".txd"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source)
        {
            return new TxdStorybookArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new TxdStorybookArchiveWriter(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            source.Position = 0;
            return (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'T', (byte)'X', (byte)'A', (byte)'G' }));
        }
    }

    #region Archive Reader
    public class TxdStorybookArchiveReader : ArchiveReader
    {
        public TxdStorybookArchiveReader(Stream source) : base(source)
        {
            var fileCount = PTStream.ReadInt32BEAt(source, 0x4);
            entries = new ArchiveEntryCollection(this, fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                var fileName = PTStream.ReadCStringAt(source, 0x10 + (i * 0x28), 32);
                var offset = PTStream.ReadUInt32BEAt(source, 0x08 + (i * 0x28));
                var length = PTStream.ReadInt32BEAt(source, 0x0C + (i * 0x28));

                fileName = string.IsNullOrWhiteSpace(fileName) ? fileName : Path.ChangeExtension(fileName, "GVR");

                entries.Add(startOffset + offset, length, fileName);
            }
        }

    }
    #endregion

    public class TxdStorybookArchiveWriter : ArchiveWriter
    {
        public TxdStorybookArchiveWriter(Stream destination) : base(destination) { }

        /// <summary>
        /// Creates an entry that has the specified data entry name in the archive.
        /// </summary>
        /// <param name="source">The data to be added to the archive.</param>
        /// <param name="entryName">The name of the entry to be created.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public override void CreateEntry(Stream source, string entryName)
        {
            // Only PVR textures can be added to a PVM archive. If this is not a PVR texture, throw an exception.
            if (!(new GvrTexture()).Is(source, entryName))
            {
                throw new CannotAddFileToArchiveException();
            }

            base.CreateEntry(source, entryName.ToUpper());
        }

        public override void Flush()
        {
            const int blockSize = 64;
            var offsetList = new int[entries.Count];

            destination.WriteByte((byte)'T');
            destination.WriteByte((byte)'X');
            destination.WriteByte((byte)'A');
            destination.WriteByte((byte)'G');

            PTStream.WriteInt32BE(destination, entries.Count);

            var offset = PTMethods.RoundUp(0x8 + (entries.Count * 0x28), blockSize);
            for (int i = 0; i < entries.Count; i++)
            {
                var length = entries[i].Length;
                offsetList[i] = offset;

                PTStream.WriteInt32BE(destination, offset);
                PTStream.WriteInt32BE(destination, length);
                PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(entries[i].Name), 32);

                offset += PTMethods.RoundUp(length, blockSize);
            }

            for (int i = 0; i < entries.Count; i++)
            {
                destination.Position = offsetList[i];
                PTStream.CopyToPadded(entries[i].Open(), destination, blockSize, 0);
                OnFileAdded(EventArgs.Empty);
            }
        }
    }
}