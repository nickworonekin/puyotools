using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PuyoTools.Core.Textures;
using PuyoTools.Core.Textures.Svr;

namespace PuyoTools.Core.Archives
{
    public class SvmArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'P', (byte)'V', (byte)'M', (byte)'H' };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new SvmArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
        {
            return new SvmArchiveWriter(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = source.AsBinaryReader())
            {
                if (!(remainingLength > 12
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)))
                {
                    return false;
                }

                // Since PVMs and SVMs have identical headers, we need to check the data format of the first texture in the archive.
                // For SVRs, the data format will be >= 0x60 and < 0x70.
                var firstEntryOffset = reader.At(startPosition + 4, x => x.ReadInt32()) + 8;

                if (remainingLength < firstEntryOffset + 16)
                {
                    return false;
                }

                var dataFormat = reader.At(startPosition + firstEntryOffset + 0x9, x => x.ReadByte());

                if (dataFormat >= 0x60 && dataFormat < 0x70)
                {
                    return true;
                }

                return false;
            }
        }
    }

    #region Archive Reader
    public class SvmArchiveReader : LegacyArchiveReader
    {
        bool hasFilenames, hasFormats, hasDimensions, hasGlobalIndexes;
        int tableEntryLength, globalIndexOffset;

        public SvmArchiveReader(Stream source) : base(source)
        {
            // The offset of the first entry
            source.Position += 4;
            int entryOffset = PTStream.ReadInt32(source) + 8;
            int headerOffset = 0xC;

            // Read what properties this archive stores for each texture
            byte properties  = PTStream.ReadByte(source);
            hasFilenames     = (properties & (1 << 3)) > 0;
            hasFormats       = (properties & (1 << 2)) > 0;
            hasDimensions    = (properties & (1 << 1)) > 0;
            hasGlobalIndexes = (properties & (1 << 0)) > 0;
            source.Position++;

            // Determine the size of each entry in the entry table
            tableEntryLength = 2;
            if (hasFilenames)  tableEntryLength += 28;
            if (hasFormats)    tableEntryLength += 2;
            if (hasDimensions) tableEntryLength += 2;

            if (hasGlobalIndexes)
            {
                globalIndexOffset = tableEntryLength;
                tableEntryLength += 4;
            }

            // Get the number of entries in the archive
            ushort numEntries = PTStream.ReadUInt16(source);
            entries = new List<ArchiveEntry>(numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // We need to need to determine the offset based on the length,
                // which is stored in the texture data.
                // We already have the entry offset
                source.Position = startOffset + entryOffset + 4;
                int entryLength = PTStream.ReadInt32(source) + 8;

                string entryFname = String.Empty;
                if (hasFilenames)
                {
                    source.Position = startOffset + headerOffset + 2;
                    entryFname = PTStream.ReadCString(source, 28) + ".svr";
                    //headerOffset += tableEntryLength;
                }

                uint? globalIndex = null;
                if (hasGlobalIndexes)
                {
                    source.Position = startOffset + headerOffset + globalIndexOffset;
                    globalIndex = PTStream.ReadUInt32(source);
                }

                headerOffset += tableEntryLength;

                // Add this entry to the collection
                //entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname) { Index = i });
                entries.Add(new PvmArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname, globalIndex));

                entryOffset += entryLength;
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }

        /*public override Stream OpenEntry(ArchiveEntry entry)
        {
            // If this archive does not contain any global indexes, then just return the data as is.
            if (!hasGlobalIndexes)
            {
                return base.OpenEntry(entry);
            }

            long oldPosition = archiveData.Position;

            MemoryStream data = new MemoryStream();

            // Write out the GBIX header
            data.WriteByte((byte)'G');
            data.WriteByte((byte)'B');
            data.WriteByte((byte)'I');
            data.WriteByte((byte)'X');
            PTStream.WriteInt32(data, 8);

            archiveData.Position = 0xC + (entry.Index * tableEntryLength) + globalIndexOffset;
            PTStream.WriteUInt32(data, PTStream.ReadUInt32(archiveData));

            data.Position += 4;

            // Now copy over the file data
            using (var stream = new StreamView(archiveData, entry.Offset, entry.Length))
            {
                stream.CopyTo(data);
            }

            archiveData.Position = oldPosition;
            data.Position = 0;

            return data;
        }*/
    }
    #endregion

    #region Archive Writer
    public class SvmArchiveWriter : LegacyArchiveWriter
    {
        #region Settings
        /// <summary>
        /// Sets if filenames should be stored in the archive. The default value is true.
        /// </summary>
        public bool HasFilenames { get; set; }

        /// <summary>
        /// Sets if global indexes should be stored in the archive. The default value is true.
        /// </summary>
        public bool HasGlobalIndexes { get; set; }

        /// <summary>
        /// Sets if texture formats should be stored in the archive. The default value is true.
        /// </summary>
        public bool HasFormats { get; set; }

        /// <summary>
        /// Sets if texture dimensions should be stored in the archive. The default value is true.
        /// </summary>
        public bool HasDimensions { get; set; }
        #endregion

        public SvmArchiveWriter(Stream destination) : base(destination)
        {
            // Set default settings
            HasFilenames = true;
            HasGlobalIndexes = true;
            HasFormats = true;
            HasDimensions = true;
        }

        /// <inheritdoc/>
        public override ArchiveEntry CreateEntry(Stream source, string entryName)
        {
            // Only SVR textures can be added to a SVM archive. If this is not a SVR texture, throw an exception.
            if (!SvrTexture.Identify(source))
            {
                throw new FileRejectedException("SVM archives can only contain SVR textures.");
            }

            return base.CreateEntry(source, entryName);
        }

        protected override void WriteFile()
        {
            // Determine the length of each entry in the header
            // and the flags that indicate what is stored in the header
            int entryLength = 2;
            ushort flags = 0;

            if (HasFilenames)
            {
                entryLength += 28;
                flags |= 0x8;
            }
            if (HasFormats)
            {
                entryLength += 2;
                flags |= 0x4;
            }
            if (HasDimensions)
            {
                entryLength += 2;
                flags |= 0x2;
            }
            if (HasGlobalIndexes)
            {
                entryLength += 4;
                flags |= 0x1;
            }

            // Write the start of the header
            destination.WriteByte((byte)'P');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'M');
            destination.WriteByte((byte)'H');

            // Offset of the first texture in the archive
            long entryOffset = PTMethods.RoundUp(12 + (entries.Count * entryLength), 16);
            PTStream.WriteInt32(destination, (int)entryOffset - 8);

            // Write out the flags
            PTStream.WriteUInt16(destination, flags);

            // Write out the number of entries
            PTStream.WriteUInt16(destination, (ushort)entries.Count);

            // We're going to be using this a few times. Might as well do this here
            long oldPosition;

            // Now, let's add the entries
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                Stream entryData = entries[i].Open();

                // We need to get some information about the texture.
                // We already checked to make sure this texture is a SVR.
                // No need to check it again.
                oldPosition = entryData.Position;
                SvrTextureDecoder texture = new SvrTextureDecoder(entryData);
                entryData.Position = oldPosition;

                // Write out the entry number
                PTStream.WriteUInt16(destination, (ushort)i);

                // Write the information for this entry in the header
                if (HasFilenames)
                {
                    PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(entries[i].Name), 28);
                }
                if (HasFormats)
                {
                    destination.WriteByte(0);
                    destination.WriteByte((byte)texture.DataFormat);
                }
                if (HasDimensions)
                {
                    ushort dimensions = 0;
                    dimensions |= (ushort)(((byte)Math.Log(texture.Width, 2) - 2) & 0xF);
                    dimensions |= (ushort)((((byte)Math.Log(texture.Height, 2) - 2) & 0xF) << 4);
                    PTStream.WriteUInt16(destination, dimensions);
                }
                if (HasGlobalIndexes)
                {
                    PTStream.WriteUInt32(destination, texture.GlobalIndex ?? 0);
                }

                // Now write out the entry information
                oldPosition = destination.Position;
                destination.Position = entryOffset;
                entryData.Position += texture.PvrtPosition;

                PTStream.CopyToPadded(entryData, destination, 16, 0);

                entryOffset = destination.Position;
                destination.Position = oldPosition;

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }
        }
    }
    #endregion
}