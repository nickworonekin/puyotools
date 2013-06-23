using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using PuyoTools.Modules.Texture;

namespace PuyoTools.Modules.Archive
{
    public class SntArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "SNT"; }
        }

        public override string FileExtension
        {
            get { return ".snt"; }
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
            return (length > 36
                && (PTStream.Contains(source, 0, new byte[] { (byte)'N', (byte)'U', (byte)'I', (byte)'F' })
                && PTStream.Contains(source, 32, new byte[] { (byte)'N', (byte)'U', (byte)'T', (byte)'L' }))
                || (PTStream.Contains(source, 0, new byte[] { (byte)'N', (byte)'S', (byte)'I', (byte)'F' })
                && PTStream.Contains(source, 32, new byte[] { (byte)'N', (byte)'S', (byte)'T', (byte)'L' })));
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // Get the number of files in the archive
                source.Position += 48;
                int numFiles = PTStream.ReadInt32(source);
                Files = new ArchiveEntry[numFiles];

                source.Position += 8 + (numFiles * 20);

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Readin the entry offset and length
                    int entryLength = PTStream.ReadInt32(source);
                    int entryOffset = PTStream.ReadInt32(source) + 32;

                    // If this archive contains GIM textures, then it's possible that they may contain filenames.
                    // Let's check and see
                    string entryFname = String.Empty;
                    if (entryLength > 40)
                    {
                        long oldPosition = source.Position;
                        source.Position = archiveOffset + entryOffset;

                        if ((new GimTexture()).Is(source, entryLength, String.Empty))
                        {
                            // It's a GIM texture. Let's try to find a filename
                            source.Position += 36;
                            long fnameOffset = PTStream.ReadInt32(source) + 48;

                            if (fnameOffset < length)
                            {
                                source.Position = archiveOffset + entryOffset + fnameOffset;
                                entryFname = Path.GetFileNameWithoutExtension(PTStream.ReadCString(source, (int)(length - fnameOffset)));

                                if (entryFname != String.Empty)
                                {
                                    entryFname += ".gim";
                                }
                            }
                        }

                        source.Position = oldPosition;
                    }

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

                // Magic code "NSIF/NUTL"
                if (settings.Type == WriterSettings.SntType.Ps2)
                {
                    destination.WriteByte((byte)'N');
                    destination.WriteByte((byte)'S');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'F');
                }
                else
                {
                    destination.WriteByte((byte)'N');
                    destination.WriteByte((byte)'U');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'F');
                }

                PTStream.WriteInt32(destination, 24); // Unknown
                PTStream.WriteInt32(destination, 1); // Unknown
                PTStream.WriteInt32(destination, 32); // Offset of the NGTL chunk?

                // Calculate the size of the NGTL chunk
                int NGTLLength = 0;
                for (int i = 0; i < files.Count; i++)
                {
                    NGTLLength += PTMethods.RoundUp(files[i].Length, 8);
                }

                PTStream.WriteInt32(destination, PTMethods.RoundUp(28 + (files.Count * 28), 8) + NGTLLength);
                PTStream.WriteInt32(destination, PTMethods.RoundUp(60 + (files.Count * 28), 8) + NGTLLength);
                PTStream.WriteInt32(destination, 24 + (files.Count * 4));
                PTStream.WriteInt32(destination, 1);

                // NSTL/NUIL chunk
                if (settings.Type == WriterSettings.SntType.Ps2)
                {
                    destination.WriteByte((byte)'N');
                    destination.WriteByte((byte)'S');
                    destination.WriteByte((byte)'T');
                    destination.WriteByte((byte)'L');
                }
                else
                {
                    destination.WriteByte((byte)'N');
                    destination.WriteByte((byte)'U');
                    destination.WriteByte((byte)'T');
                    destination.WriteByte((byte)'L');
                }

                PTStream.WriteInt32(destination, PTMethods.RoundUp(20 + (files.Count * 28), 8) + NGTLLength);
                PTStream.WriteInt32(destination, 16);
                PTStream.WriteInt32(destination, 0);
                PTStream.WriteInt32(destination, files.Count);
                PTStream.WriteInt32(destination, 28);
                PTStream.WriteInt32(destination, 28 + (files.Count * 20));

                // Write out crap bytes
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32(destination, 1);
                    PTStream.WriteInt32(destination, 0);
                    destination.Write(new byte[] { 1, 0, 1, 0 }, 0, 4);
                    PTStream.WriteInt32(destination, i);
                    PTStream.WriteInt32(destination, 0);
                }

                // Write out the header for the archive
                int entryOffset = 60 + (files.Count * 28);
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32(destination, files[i].Length);
                    PTStream.WriteInt32(destination, entryOffset - 32);

                    entryOffset += PTMethods.RoundUp(files[i].Length, 4);
                }

                // Write out the file data for each file
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, 4, 0);

                    // Call the file added event
                    OnFileAdded(EventArgs.Empty);
                }

                // Pad before writing out the NOF0 chunk
                while ((destination.Position - offset) % 8 != 0)
                    destination.WriteByte(0);

                // NOF0 chunk
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'O');
                destination.WriteByte((byte)'F');
                destination.WriteByte((byte)'0');

                // Write out crap bytes
                PTStream.WriteInt32(destination, PTMethods.RoundUp(28 + (files.Count * 4), 8));
                PTStream.WriteInt32(destination, files.Count + 2);
                PTStream.WriteInt32(destination, 0);
                PTStream.WriteInt32(destination, 20);

                // Write out more unknown stuff
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32(destination, 32 + (files.Count * 20) + (i * 8));
                }

                PTStream.WriteInt32(destination, 24);

                // Pad before we write NEND
                // Finish padding out the archive
                while ((destination.Position - offset) % 16 != 0)
                    destination.WriteByte(0);

                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'E');
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'D');

                while ((destination.Position - offset) % 16 != 0)
                    destination.WriteByte(0);
            }
        }

        public class WriterSettings : ModuleWriterSettings
        {
            private SntWriterSettings writerSettingsControls;

            public SntType Type = SntType.Ps2;

            public enum SntType
            {
                Ps2, // PlayStation 2
                Psp, // PlayStation Portable
            }

            public override Control Content()
            {
                writerSettingsControls = new SntWriterSettings();
                return writerSettingsControls;
            }

            public override void SetSettings()
            {
                Type = (writerSettingsControls.TypePs2Radio.Checked ? SntType.Ps2 : SntType.Psp);
            }
        }
    }
}