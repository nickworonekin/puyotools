using System;
using System.IO;

using PuyoTools.Modules.Texture;

namespace PuyoTools.Modules.Archive
{
    public class GvmArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "GVM"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".gvm"; }
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
            return new GvmArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new GvmArchiveWriter(destination);
        }

        public override ModuleSettingsControl GetModuleSettingsControl()
        {
            return new PvmWriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 12 && PTStream.Contains(source, 0, new byte[] { (byte)'G', (byte)'V', (byte)'M', (byte)'H' }));
        }
    }

    #region Archive Reader
    public class GvmArchiveReader : ArchiveReader
    {
        bool hasFilenames, hasFormats, hasDimensions, hasGlobalIndexes;
        int tableEntryLength, globalIndexOffset;

        bool needToFix;

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
            entries = new ArchiveEntryCollection(this, numEntries);

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
                    headerOffset += tableEntryLength;
                }

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, entryFname);

                entryOffset += entryLength;
            }

            // Some Billy Hatcher textures have an oddity where the texture length is 16 more than what it
            // actually should be. This seems to only effect the last texture of a GVM, and only some of them
            // are affected. In that case, we will "fix" the GVRs in question.
            needToFix = (entryOffset > source.Length);

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
            
        public override Stream OpenEntry(ArchiveEntry entry)
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
            archiveData.Position = entry.Offset;
            PTStream.CopyPartTo(archiveData, data, entry.Length);

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
        }
    }
    #endregion

    #region Archive Writer
    public class GvmArchiveWriter : ArchiveWriter
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
            // Only GVR textures can be added to a GVM archive. If this is not a GVR texture, throw an exception.
            if (!(new GvrTexture()).Is(source, entryName))
            {
                throw new CannotAddFileToArchiveException();
            }

            base.CreateEntry(source, entryName);
        }

        public override void Flush()
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
                Stream entryData = entries[i].Open();

                // We need to get some information about the texture.
                // We already checked to make sure this texture is a GVR.
                // No need to check it again.
                oldPosition = entryData.Position;
                VrSharp.GvrTexture.GvrTexture texture = new VrSharp.GvrTexture.GvrTexture(entryData);
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
                    destination.WriteByte((byte)(((byte)texture.PixelFormat << 4) | ((byte)texture.DataFlags & 0xF)));
                    destination.WriteByte((byte)texture.DataFormat);
                }
                if (HasDimensions)
                {
                    ushort dimensions = 0;
                    dimensions |= (ushort)(((byte)Math.Log(texture.TextureWidth, 2) - 2) & 0xF);
                    dimensions |= (ushort)((((byte)Math.Log(texture.TextureHeight, 2) - 2) & 0xF) << 4);
                    PTStream.WriteUInt16BE(destination, dimensions);
                }
                if (HasGlobalIndexes)
                {
                    PTStream.WriteUInt32BE(destination, texture.GlobalIndex);
                }

                // Now write out the entry information
                oldPosition = destination.Position;
                destination.Position = entryOffset;
                entryData.Position += texture.PvrtOffset;

                PTStream.CopyToPadded(entryData, destination, 16, 0);

                entryOffset = destination.Position;
                destination.Position = oldPosition;

                // Call the file added event
                OnFileAdded(EventArgs.Empty);
            }
        }
    }
    #endregion
}