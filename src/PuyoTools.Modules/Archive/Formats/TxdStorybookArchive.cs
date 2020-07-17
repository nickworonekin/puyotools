using PuyoTools.Modules.Compression;
using PuyoTools.Modules.Texture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Modules.Archive
{
    public class TxdStorybookArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'T', (byte)'X', (byte)'A', (byte)'G' };

        public override ArchiveReader Open(Stream source)
        {
            return new TxdStorybookArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new TxdStorybookArchiveWriter(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return source.Length - startPosition > 16
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode);
            }
        }
    }

    #region Archive Reader
    public class TxdStorybookArchiveReader : ArchiveReader
    {
        public TxdStorybookArchiveReader(Stream source) : base(source)
        {
            var fileCount = PTStream.ReadInt32BEAt(source, 0x4);
            entries = new List<ArchiveEntry>(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                var fileName = PTStream.ReadCStringAt(source, 0x10 + (i * 0x28), 32);
                var offset = PTStream.ReadUInt32BEAt(source, 0x08 + (i * 0x28));
                var length = PTStream.ReadInt32BEAt(source, 0x0C + (i * 0x28));

                fileName = string.IsNullOrWhiteSpace(fileName) ? fileName : Path.ChangeExtension(fileName, "GVR");

                entries.Add(new ArchiveEntry(this, startOffset + offset, length, fileName));
            }
        }

    }
    #endregion

    public class TxdStorybookArchiveWriter : ArchiveWriter
    {
        public TxdStorybookArchiveWriter(Stream destination) : base(destination) { }

        /// <inheritdoc/>
        public override ArchiveEntry CreateEntry(Stream source, string entryName)
        {
            // Only GVR textures can be added to a Sonic Storybook TXD archive. If this is not a PVR texture, throw an exception.
            if (!GvrTexture.Identify(source))
            {
                throw new FileRejectedException("Sonic Storybook TXD archives can only contain GVR textures.");
            }

            return base.CreateEntry(source, entryName.ToUpper());
        }

        protected override void WriteFile()
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
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                destination.Position = offsetList[i];
                PTStream.CopyToPadded(entries[i].Open(), destination, blockSize, 0);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }
        }
    }
}