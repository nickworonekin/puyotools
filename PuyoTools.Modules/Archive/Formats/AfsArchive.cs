using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PuyoTools.Modules.Archive
{
    public class AfsArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "AFS"; }
        }

        public override string FileExtension
        {
            get { return ".afs"; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source, int length)
        {
            return new Reader(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ModuleWriterSettings settings)
        {
            return new Writer(destination, (settings as WriterSettings) ?? new WriterSettings());
        }

        public override ModuleWriterSettings WriterSettingsObject()
        {
            return new WriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'A', (byte)'F', (byte)'S', 0 }));
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // Get the number of files in the archive
                source.Position += 4;
                int numFiles = PTStream.ReadInt32(source);
                Files = new ArchiveEntry[numFiles];

                // Get the offset of the metadata
                source.Position += (numFiles * 8);
                int metadataOffset = PTStream.ReadInt32(source);

                // If the offset isn't stored there, then it is stored right before the offset of the first file
                if (metadataOffset == 0)
                {
                    source.Position = archiveOffset + 8;
                    source.Position = PTStream.ReadInt32(source) - 8;
                    metadataOffset = PTStream.ReadInt32(source);
                }

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Readin the entry offset and length
                    source.Position = archiveOffset + 8 + (i * 8);
                    int entryOffset = PTStream.ReadInt32(source);
                    int entryLength = PTStream.ReadInt32(source);

                    // Read in the entry fname
                    source.Position = metadataOffset + (i * 48);
                    string entryFname = PTStream.ReadCString(source, 32);

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, archiveOffset + entryOffset, entryLength, entryFname);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }
        }

        public class Writer : ArchiveWriter
        {
            WriterSettings settings;

            public Writer(Stream destination) : this(destination, new WriterSettings()) { }

            public Writer(Stream destination, WriterSettings settings)
            {
                Initalize(destination);
                this.settings = settings;
            }

            public override void Flush()
            {
                // The start of the archive
                long offset = destination.Position;
                int blockSize = settings.BlockSize;
                WriterSettings.AfsVersion version = settings.Version;

                // Magic code "AFS\0"
                destination.WriteByte((byte)'A');
                destination.WriteByte((byte)'F');
                destination.WriteByte((byte)'S');
                destination.WriteByte(0);

                // Number of files in the archive
                PTStream.WriteInt32(destination, files.Count);

                // Write out the header for the archive
                int entryOffset = PTMethods.RoundUp(12 + (files.Count * 8), blockSize);
                int firstEntryOffset = entryOffset;
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32(destination, entryOffset);
                    PTStream.WriteInt32(destination, files[i].Length);

                    entryOffset += PTMethods.RoundUp(files[i].Length, blockSize);
                }

                // If this is AFS v1, then the metadata offset is stored at 8 bytes before
                // the first entry offset.
                if (version == WriterSettings.AfsVersion.Version1)
                {
                    destination.Position = offset + firstEntryOffset - 8;
                }

                // Write out the metadata offset and length
                PTStream.WriteInt32(destination, entryOffset);
                PTStream.WriteInt32(destination, files.Count * 48);

                destination.Position = offset + firstEntryOffset;

                // Write out the file data for each file
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, blockSize, 0);
                }

                // Write out the footer for the archive
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteCString(destination, files[i].Filename, 32);

                    // File creation time
                    if (!String.IsNullOrEmpty(files[i].SourceFile) && File.Exists(files[i].SourceFile))
                    {
                        // File exists, let's read in the file creation time
                        FileInfo fileInfo = new FileInfo(files[i].SourceFile);

                        PTStream.WriteInt16(destination, (short)fileInfo.CreationTime.Year);
                        PTStream.WriteInt16(destination, (short)fileInfo.CreationTime.Month);
                        PTStream.WriteInt16(destination, (short)fileInfo.CreationTime.Day);
                        PTStream.WriteInt16(destination, (short)fileInfo.CreationTime.Hour);
                        PTStream.WriteInt16(destination, (short)fileInfo.CreationTime.Minute);
                        PTStream.WriteInt16(destination, (short)fileInfo.CreationTime.Second);
                    }
                    else
                    {
                        // File does not exist, just store all 0s
                        PTStream.WriteInt16(destination, 0);
                        PTStream.WriteInt16(destination, 0);
                        PTStream.WriteInt16(destination, 0);
                        PTStream.WriteInt16(destination, 0);
                        PTStream.WriteInt16(destination, 0);
                        PTStream.WriteInt16(destination, 0);
                    }

                    // Write out this data that I have no idea what its purpose is
                    long oldPosition = destination.Position;
                    byte[] buffer = new byte[4];

                    if (version == WriterSettings.AfsVersion.Version1)
                        destination.Position = offset + 8 + (i * 8);
                    else
                        destination.Position = offset + 4 + (i * 4);

                    destination.Read(buffer, 0, 4);
                    destination.Position = oldPosition;
                    destination.Write(buffer, 0, 4);
                }

                // Finish padding out the archive
                while ((destination.Position - offset) % blockSize != 0)
                    destination.WriteByte(0);
            }
        }

        public class WriterSettings : ModuleWriterSettings
        {
            private AfsWriterSettings writerSettingsControls;

            public int BlockSize = 2048;
            public AfsVersion Version = AfsVersion.Version1;

            public enum AfsVersion
            {
                Version1, // Dreamcast
                Version2, // Post Dreamcast (PS2, GC, Xbox and after)
            }

            public override Control Content()
            {
                writerSettingsControls = new AfsWriterSettings();
                return writerSettingsControls;
            }

            public override void SetSettings()
            {
                BlockSize = int.Parse(writerSettingsControls.BlockSizeBox.GetItemText(writerSettingsControls.BlockSizeBox.SelectedItem));
                Version = (writerSettingsControls.AfsVersion1Radio.Checked ? AfsVersion.Version1 : AfsVersion.Version2);
            }
        }
    }
}