using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PuyoTools.Core.Textures;
using PuyoTools.Core.Textures.Pvr;

namespace PuyoTools.Core.Archives
{
    public class PvmArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'P', (byte)'V', (byte)'M', (byte)'H' };
        private static readonly byte[] pvrtMagicCode = { (byte)'P', (byte)'V', (byte)'R', (byte)'T' };

        public override ArchiveReader Open(Stream source)
        {
            return new PvmArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new PvmArchiveWriter(destination);
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

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                if (!(remainingLength > 12
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)))
                {
                    return false;
                }

                // Since PVMs and SVMs have identical headers, we need to check the data format of the first texture in the archive.
                // For PVRs, the data format will be < 0x60.
                var firstEntryOffset = reader.At(startPosition + 4, x => x.ReadInt32()) + 8;

                // Get the actual offset of the first texture.
                while (true)
                {
                    if (remainingLength < firstEntryOffset + 16)
                    {
                        return false;
                    }

                    var chunkIdentifier = reader.At(startPosition + firstEntryOffset, x => x.ReadBytes(4));
                    var chunkLength = reader.At(startPosition + firstEntryOffset + 4, x => x.ReadInt32()) + 8;

                    if (chunkIdentifier.SequenceEqual(pvrtMagicCode))
                    {
                        break;
                    }

                    firstEntryOffset += chunkLength;
                }

                var dataFormat = reader.At(startPosition + firstEntryOffset + 0x9, x => x.ReadByte());

                if (dataFormat < 0x60)
                {
                    return true;
                }

                return false;
            }
        }
    }

    [Flags]
    internal enum PvmFlags : ushort
    {
        /// <summary>
        /// Specifies global indexes are provided.
        /// </summary>
        GlobalIndexes = (1 << 0),

        /// <summary>
        /// Specifies texture dimensions are provided within the entry table.
        /// </summary>
        Dimensions = (1 << 1),

        /// <summary>
        /// Specifies pixel and data formats are provided within the entry table.
        /// </summary>
        Formats = (1 << 2),

        /// <summary>
        /// Specifies filenames are present within the entry table.
        /// </summary>
        Filenames = (1 << 3),

        /// <summary>
        /// Specifies an MDLN chunk is present, which contains the filenames of the models associated with this PVM.
        /// </summary>
        MdlnChunk = (1 << 4),

        /// <summary>
        /// Specifies a PVMI chunk is present, which contains the original filenames of the textures converted to PVR.
        /// </summary>
        PvmiChunk = (1 << 5),

        /// <summary>
        /// Specifies a CONV chunk is present, which contains the name of the converter used to convert textures to PVR.
        /// </summary>
        ConvChunk = (1 << 6),

        /// <summary>
        /// Specifies IMGC chunks are present, which contains the original data of the textures converted to PVR.
        /// </summary>
        ImgcChunks = (1 << 7),

        /// <summary>
        /// Specifies PVRT chunks are present.
        /// </summary>
        PvrtChunks = (1 << 8),
    }

    #region Archive Reader
    public class PvmArchiveReader : ArchiveReader
    {
        private static readonly byte[] pvrtMagicCode = { (byte)'P', (byte)'V', (byte)'R', (byte)'T' };

        bool hasFilenames, hasFormats, hasDimensions, hasGlobalIndexes;
        int tableEntryLength, globalIndexOffset;

        public PvmArchiveReader(Stream source) : base(source)
        {
            using (var reader = source.AsBinaryReader())
            {
                // The offset of the first entry
                source.Position += 4;
                int entryOffset = reader.ReadInt32() + 8;
                int headerOffset = 0xC;

                // Read what properties this archive stores for each texture
                PvmFlags properties = (PvmFlags)reader.ReadUInt16();
                hasFilenames = (properties & PvmFlags.Filenames) != 0;
                hasFormats = (properties & PvmFlags.Formats) != 0;
                hasDimensions = (properties & PvmFlags.Dimensions) != 0;
                hasGlobalIndexes = (properties & PvmFlags.GlobalIndexes) != 0;

                // Determine the size of each entry in the entry table
                tableEntryLength = 2;
                if (hasFilenames) tableEntryLength += 28;
                if (hasFormats) tableEntryLength += 2;
                if (hasDimensions) tableEntryLength += 2;

                if (hasGlobalIndexes)
                {
                    globalIndexOffset = tableEntryLength;
                    tableEntryLength += 4;
                }

                // Get the number of entries in the archive
                ushort numEntries = reader.ReadUInt16();
                entries = new List<ArchiveEntry>(numEntries);

                // Read in all the entries
                for (int i = 0; i < numEntries;)
                {
                    // Verify if the current chunk contains texture data. If not, skip over it.
                    var chunkIdentifier = reader.At(startOffset + entryOffset, x => x.ReadBytes(4));
                    var chunkLength = reader.At(startOffset + entryOffset + 4, x => x.ReadInt32()) + 8;

                    if (!chunkIdentifier.SequenceEqual(pvrtMagicCode))
                    {
                        entryOffset += chunkLength;
                        continue;
                    }

                    // We need to need to determine the offset based on the length,
                    // which is stored in the texture data.
                    // We already have the entry offset
                    source.Position = startOffset + entryOffset + 4;
                    int entryLength = reader.ReadInt32() + 8;

                    string entryName = string.Empty;
                    if (hasFilenames)
                    {
                        entryName = reader.At(startOffset + headerOffset + 2, x => x.ReadString(28)) + ".pvr";
                        //headerOffset += tableEntryLength;
                    }

                    uint? globalIndex = null;
                    if (hasGlobalIndexes)
                    {
                        globalIndex = reader.At(startOffset + headerOffset + globalIndexOffset, x => x.ReadUInt32());
                    }

                    headerOffset += tableEntryLength;

                    // Add this entry to the collection
                    //entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname) { Index = i });
                    entries.Add(new PvmArchiveEntry(this, startOffset + entryOffset, entryLength, entryName, globalIndex));

                    entryOffset += entryLength;
                    i++;
                }
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
    public class PvmArchiveWriter : ArchiveWriter
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

        public PvmArchiveWriter(Stream destination) : base(destination)
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
            // Only PVR textures can be added to a PVM archive. If this is not a PVR texture, throw an exception.
            if (!PvrTexture.Identify(source))
            {
                throw new FileRejectedException("PVM archives can only contain PVR textures.");
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
                // We already checked to make sure this texture is a PVR.
                // No need to check it again.
                oldPosition = entryData.Position;
                PvrTextureDecoder texture = new PvrTextureDecoder(entryData);
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
                if (texture.GlobalIndex.HasValue)
                {
                    PTStream.WriteUInt32(destination, texture.GlobalIndex.Value);
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