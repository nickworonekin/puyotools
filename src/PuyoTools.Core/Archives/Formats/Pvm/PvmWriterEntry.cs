using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Pvm
{
    public class PvmWriterEntry : ArchiveWriterEntry
    {
        private long _pvrtPosition = 0;
        private uint? _globalIndex = null;
        private byte _pixelFormat;
        private byte _dataFormat;
        private ushort _width;
        private ushort _height;

        public PvmWriterEntry(Stream source, string? name = null, bool leaveOpen = false)
            : base(source, name, leaveOpen)
        {
            long streamStart = source.Position;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            // If this PVR has a GBIX header, read the global index and the offset of the PVRT chunk.
            ReadOnlySpan<byte> magicCode = reader.At(streamStart, x => x.ReadBytes(4));
            if (magicCode.SequenceEqual(PvmConstants.GbixMagicCode))
            {
                reader.At(streamStart + 4, x =>
                {
                    _pvrtPosition = reader.ReadInt32() + 8;
                    _globalIndex = reader.ReadUInt32();
                });
            }

            reader.At(streamStart + _pvrtPosition + 0xA, x =>
            {
                _pixelFormat = x.ReadByte();
                _dataFormat = x.ReadByte();
                _width = x.ReadUInt16();
                _height = x.ReadUInt16();
            });
        }

        public uint? GlobalIndex => _globalIndex;

        public byte PixelFormat => _pixelFormat;

        public byte DataFormat => _dataFormat;

        public ushort Width => _width;

        public ushort Height => _height;

        public override void WriteTo(Stream destination)
        {
            _source.Position += _pvrtPosition;

            base.WriteTo(destination);
        }
    }
}
