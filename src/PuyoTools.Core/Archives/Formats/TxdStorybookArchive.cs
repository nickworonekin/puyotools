using PuyoTools.Core.Compression;
using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class TxdStorybookArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'T', (byte)'X', (byte)'A', (byte)'G' };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new TxdStorybookArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
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
    public class TxdStorybookArchiveReader : LegacyArchiveReader
    {
        public TxdStorybookArchiveReader(Stream source) : base(source)
        {
            source.Position += 6;

            var fileCount = PTStream.ReadInt16BE(source);
            entries = new List<ArchiveEntry>(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                var offset = PTStream.ReadUInt32BE(source);
                var length = PTStream.ReadInt32BE(source);
                var fileName = PTStream.ReadCString(source, 32);

                fileName = string.IsNullOrWhiteSpace(fileName) ? fileName : Path.ChangeExtension(fileName, "GVR");

                entries.Add(new ArchiveEntry(this, startOffset + offset, length, fileName));
            }
        }
    }
    #endregion

    public class TxdStorybookArchiveWriter : LegacyArchiveWriter
    {
        public TxdStorybookArchiveWriter(Stream destination) : base(destination) { }

        /// <inheritdoc/>
        public override ArchiveEntry CreateEntry(Stream source, string entryName)
        {
            // Only GVR textures can be added to a Sonic Storybook TXD archive. If this is not a GVR texture, throw an exception.
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