using System;
using System.IO;

namespace PuyoTools2.Compression
{
    public class PRS : CompressionBase
    {
        public override void Compress(byte[] source, long offset, Stream destination, int length, string fname)
        {

        }

        public override void Decompress(byte[] source, long offset, Stream destination, int length)
        {
            // Hackish solution ATM
            MemoryStream source2 = new MemoryStream();
            source2.Write(source, (int)offset, length);
            source2.Position = 0;

            int bitPos = 9;
            byte currentByte;
            int lookBehindOffset, lookBehindLength;

            currentByte = ReadByte(source2);
            for (; ; )
            {
                if (GetControlBit(ref bitPos, ref currentByte, source2) != 0)
                {
                    // Direct byte
                    destination.WriteByte(ReadByte(source2));
                    continue;
                }

                if (GetControlBit(ref bitPos, ref currentByte, source2) != 0)
                {
                    lookBehindOffset = ReadByte(source2);
                    lookBehindOffset |= ReadByte(source2) << 8;
                    if (lookBehindOffset == 0)
                    {
                        // End of the compressed data
                        break;
                    }

                    lookBehindLength = lookBehindOffset & 7;
                    lookBehindOffset = (lookBehindOffset >> 3) | -0x2000;
                    if (lookBehindLength == 0)
                    {
                        lookBehindLength = ReadByte(source2) + 1;
                    }
                    else
                    {
                        lookBehindLength += 2;
                    }
                }
                else
                {
                    lookBehindLength = 0;
                    lookBehindLength = (lookBehindLength << 1) | GetControlBit(ref bitPos, ref currentByte, source2);
                    lookBehindLength = (lookBehindLength << 1) | GetControlBit(ref bitPos, ref currentByte, source2);
                    lookBehindOffset = ReadByte(source2) | -0x100;
                    lookBehindLength += 2;
                }

                for (int i = 0; i < lookBehindLength; i++)
                {
                    long writePosition = destination.Position;
                    destination.Seek(writePosition + lookBehindOffset, SeekOrigin.Begin);
                    byte b = ReadByte(destination);
                    destination.Seek(writePosition, SeekOrigin.Begin);
                    destination.WriteByte(b);
                }
            }
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (Path.GetExtension(fname) == ".prs");
        }

        public override bool CanCompress()
        {
            return false;
        }

        private static void Copy(int offset, int size, ref byte controlByte, ref byte bitPos, MemoryStream data, Stream destination)
        {
            if ((offset >= -0x100) && (size <= 5))
            {
                size -= 2;
                PutControlBit(0, ref controlByte, ref bitPos, data, destination);
                PutControlBit(0, ref controlByte, ref bitPos, data, destination);
                PutControlBit((size >> 1) & 1, ref controlByte, ref bitPos, data, destination);
                data.WriteByte((byte)(offset & 0xFF));
                PutControlBit(size & 1, ref controlByte, ref bitPos, data, destination);
            }
            else
            {
                if (size <= 9)
                {
                    PutControlBit(0, ref controlByte, ref bitPos, data, destination);
                    data.WriteByte((byte)(((offset << 3) & 0xF8) | ((size - 2) & 0x07)));
                    data.WriteByte((byte)((offset >> 5) & 0xFF));
                    PutControlBit(1, ref controlByte, ref bitPos, data, destination);
                }
                else
                {
                    PutControlBit(0, ref controlByte, ref bitPos, data, destination);
                    data.WriteByte((byte)((offset << 3) & 0xF8));
                    data.WriteByte((byte)((offset >> 5) & 0xFF));
                    data.WriteByte((byte)(size - 1));
                    PutControlBit(1, ref controlByte, ref bitPos, data, destination);
                }
            }
        }

        private static void PutControlBit(int bit, ref byte controlByte, ref byte bitPos, MemoryStream data, Stream destination)
        {
            controlByte >>= 1;
            controlByte |= (byte)(bit << 7);
            bitPos++;
            if (bitPos >= 8)
            {
                Flush(ref controlByte, ref bitPos, data, destination);
            }
        }

        private static void Flush(ref byte controlByte, ref byte bitPos, MemoryStream data, Stream destination)
        {
            destination.WriteByte(controlByte);
            controlByte = 0;
            bitPos = 0;

            byte[] bytes = data.ToArray();
            destination.Write(bytes, 0, bytes.Length);
            data.SetLength(0);
        }


        private static int GetControlBit(ref int bitPos, ref byte currentByte, Stream source)
        {
            bitPos--;
            if (bitPos == 0)
            {
                currentByte = ReadByte(source);
                bitPos = 8;
            }

            int flag = currentByte & 1;
            currentByte >>= 1;
            return flag;
        }

        private static byte ReadByte(Stream stream)
        {
            int value = stream.ReadByte();
            if (value == -1)
            {
                throw new EndOfStreamException();
            }

            return (byte)value;
        }
    }
}