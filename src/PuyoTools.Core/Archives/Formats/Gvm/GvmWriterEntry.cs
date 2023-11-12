using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gvm
{
    public class GvmWriterEntry : ArchiveWriterEntry
    {
        private long _gvrtPosition = 0;
        private uint? _globalIndex = null;
        private byte _flagsAndPaletteFormat;
        private byte _pixelFormat;
        private ushort _width;
        private ushort _height;

        public GvmWriterEntry(Stream source, string? name = null, bool leaveOpen = false)
            : base(source, name, leaveOpen)
        {
            long streamStart = source.Position;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            // If this GVR has a GBIX or GCIX header, read the global index
            // and the offset of the GVRT chunk.
            ReadOnlySpan<byte> magicCode = reader.At(streamStart, x => x.ReadBytes(4));
            if (magicCode.SequenceEqual(GvmConstants.GbixMagicCode)
                || magicCode.SequenceEqual(GvmConstants.GcixMagicCode))
            {
                reader.At(streamStart + 4, x =>
                {
                    _gvrtPosition = reader.ReadInt32() + 8;
                    _globalIndex = reader.ReadUInt32();
                });
            }

            reader.At(streamStart + _gvrtPosition + 0xA, x =>
            {
                _flagsAndPaletteFormat = x.ReadByte();
                _pixelFormat = x.ReadByte();
                _width = x.ReadUInt16BigEndian();
                _height = x.ReadUInt16BigEndian();
            });
        }

        public uint? GlobalIndex => _globalIndex;

        public byte FlagsAndPaletteFormat => _flagsAndPaletteFormat;

        public byte PixelFormat => _pixelFormat;

        public ushort Width => _width;

        public ushort Height => _height;

        public override void WriteTo(Stream destination)
        {
            _source.Position += _gvrtPosition;

            base.WriteTo(destination);
        }
    }
}
