using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PuyoTools.Core.Textures;

namespace PuyoTools.Core.Archives
{
    public class GvmArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'G', (byte)'V', (byte)'M', (byte)'H' };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new GvmArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
        {
            return new GvmArchiveWriter(destination);
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
                return source.Length - startPosition > 12
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode);
            }
        }
    }

    #region Archive Reader
    public class GvmArchiveReader : LegacyArchiveReader
    {
        bool hasFilenames, hasFormats, hasDimensions, hasGlobalIndexes;
        int tableEntryLength, globalIndexOffset;

        //bool needToFix;

        public GvmArchiveReader(Stream source) : base(source)
        {
            // The offset of the first entry
            source.Position += 4;
            int entryOffset = PTStream.ReadInt32(source) + 8;
            int headerOffset = 0xC;

            // Read what properties this archive stores for each texture
            source.Position++;
            byte properties  = PTStream.ReadByte(source);
            hasFilenames     = (properties & (1 << 3)) > 0;
            hasFormats       = (properties & (1 << 2)) > 0;
            hasDimensions    = (properties & (1 << 1)) > 0;
            hasGlobalIndexes = (properties & (1 << 0)) > 0;

            // Determine the size of each entry in the file table
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
            ushort numEntries = PTStream.ReadUInt16BE(source);
            entries = new List<ArchiveEntry>(numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // We need to need to determine the offset based on the length, which is stored in the texture data.
                // We already have the entry offset.
                source.Position = startOffset + entryOffset + 4;
                int entryLength = PTStream.ReadInt32(source) + 8;

                string entryFname = String.Empty;
                if (hasFilenames)
                {
                    source.Position = startOffset + headerOffset + 2;
                    entryFname = PTStream.ReadCString(source, 28) + ".gvr";
                    //headerOffset += tableEntryLength;
                }

                uint? globalIndex = null;
                if (hasGlobalIndexes)
                {
                    source.Position = startOffset + headerOffset + globalIndexOffset;
                    globalIndex = PTStream.ReadUInt32(source);
                }

                headerOffset += tableEntryLength;

                // Some Billy Hatcher textures have an oddity where the texture length is 16 more than what it
                // actually should be. This seems to only effect the last texture of a GVM, and only some of them
                // are affected. In that case, we will "fix" the GVRs in question.
                var needToFix = i == entries.Count - 1 && entryOffset + entryOffset + entryLength > source.Length;

                // Add this entry to the collection
                //entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname) { Index = i });
                entries.Add(new GvmArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname, globalIndex, needToFix));

                entryOffset += entryLength;
            }

            // Some Billy Hatcher textures have an oddity where the texture length is 16 more than what it
            // actually should be. This seems to only effect the last texture of a GVM, and only some of them
            // are affected. In that case, we will "fix" the GVRs in question.
            //needToFix = (entryOffset > source.Length);

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
            
        /*public override Stream OpenEntry(ArchiveEntry entry)
        {
            // Some Billy Hatcher textures have an oddity where the texture length is 16 more than what it
            // actually should be. This seems to only effect the last texture of a GVM, and only some of them
            // are affected. In that case, we will "fix" the GVRs in question.
            bool needToFix = (entry.Index == entries.Count - 1 && this.needToFix);

            // If this archive does not contain any global indicies, then just return the data as is.
            if (!hasGlobalIndexes && !needToFix)
            {
                return base.OpenEntry(entry);
            }

            long oldPosition = archiveData.Position;

            MemoryStream data = new MemoryStream();

            // Write out the GBIX header, if this archive contains global indexes
            if (hasGlobalIndexes)
            {
                data.WriteByte((byte)'G');
                data.WriteByte((byte)'B');
                data.WriteByte((byte)'I');
                data.WriteByte((byte)'X');
                PTStream.WriteInt32(data, 8);

                archiveData.Position = 0xC + (entry.Index * tableEntryLength) + globalIndexOffset;
                PTStream.WriteInt32BE(data, PTStream.ReadInt32BE(archiveData));

                data.Position += 4;
            }

            // Now copy over the file data
            using (var stream = new StreamView(archiveData, entry.Offset, entry.Length))
            {
                stream.CopyTo(data);
            }

            // Fix the texture lengths for the textures that need to be "fixed"
            if (needToFix)
            {
                if (hasGlobalIndexes)
                {
                    data.Position = 0x14;
                }
                else
                {
                    data.Position = 0x4;
                }

                uint actualLength = PTStream.ReadUInt32(data);
                data.Position -= 4;
                PTStream.WriteUInt32(data, actualLength - 16);
            }

            archiveData.Position = oldPosition;
            data.Position = 0;

            return data;
        }*/
    }
    #endregion

    #region Archive Writer
    public class GvmArchiveWriter : LegacyArchiveWriter
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

        public GvmArchiveWriter(Stream destination) : base(destination)
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
            // Only GVR textures can be added to a GVM archive. If this is not a GVR texture, throw an exception.
            if (!GvrTexture.Identify(source))
            {
                throw new FileRejectedException("GVM archives can only contain GVR textures.");
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
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'M');
            destination.WriteByte((byte)'H');

            // Offset of the first texture in the archive
            long entryOffset = PTMethods.RoundUp(28 + (entries.Count * entryLength), 16);
            PTStream.WriteInt32(destination, (int)entryOffset - 8);

            // Write out the flags
            PTStream.WriteUInt16BE(destination, flags);

            // Write out the number of entries
            PTStream.WriteUInt16BE(destination, (ushort)entries.Count);

            // We're going to be using this a few times. Might as well do this here
            long oldPosition;

            // Now, let's add the entries
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                Stream entryData = entries[i].Open();

                // We need to get some information about the texture.
                // We already checked to make sure this texture is a GVR.
                // No need to check it again.
                oldPosition = entryData.Position;
                PuyoTools.Core.Textures.Gvr.GvrTextureDecoder texture = new PuyoTools.Core.Textures.Gvr.GvrTextureDecoder(entryData);
                entryData.Position = oldPosition;

                // Write out the entry number
                PTStream.WriteUInt16BE(destination, (ushort)i);

                // Write the information for this entry in the header
                if (HasFilenames)
                {
                    PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(entries[i].Name), 28);
                }
                if (HasFormats)
                {
                    destination.WriteByte((byte)(((byte)texture.PaletteFormat << 4) | ((byte)texture.Flags & 0xF)));
                    destination.WriteByte((byte)texture.PixelFormat);
                }
                if (HasDimensions)
                {
                    ushort dimensions = 0;
                    dimensions |= (ushort)(((byte)Math.Log(texture.Width, 2) - 2) & 0xF);
                    dimensions |= (ushort)((((byte)Math.Log(texture.Height, 2) - 2) & 0xF) << 4);
                    PTStream.WriteUInt16BE(destination, dimensions);
                }
                if (HasGlobalIndexes)
                {
                    PTStream.WriteUInt32BE(destination, texture.GlobalIndex ?? 0);
                }

                // Now write out the entry information
                oldPosition = destination.Position;
                destination.Position = entryOffset;
                entryData.Position += texture.GvrtPosition;

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