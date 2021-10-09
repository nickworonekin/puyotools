using System;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class PrsCompression : CompressionBase
    {
        /*
         * PRS compression implementation from FraGag.Compression.Prs
         * https://github.com/FraGag/prs.net
         */

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            int bitPos = 9;
            byte currentByte;
            int lookBehindOffset, lookBehindLength;

            currentByte = ReadByte(source);
            for (; ; )
            {
                if (GetControlBit(ref bitPos, ref currentByte, source) != 0)
                {
                    // Direct byte
                    destination.WriteByte(ReadByte(source));
                    continue;
                }

                if (GetControlBit(ref bitPos, ref currentByte, source) != 0)
                {
                    lookBehindOffset = ReadByte(source);
                    lookBehindOffset |= ReadByte(source) << 8;
                    if (lookBehindOffset == 0)
                    {
                        // End of the compressed data
                        break;
                    }

                    lookBehindLength = lookBehindOffset & 7;
                    lookBehindOffset = (lookBehindOffset >> 3) | -0x2000;
                    if (lookBehindLength == 0)
                    {
                        lookBehindLength = ReadByte(source) + 1;
                    }
                    else
                    {
                        lookBehindLength += 2;
                    }
                }
                else
                {
                    lookBehindLength = 0;
                    lookBehindLength = (lookBehindLength << 1) | GetControlBit(ref bitPos, ref currentByte, source);
                    lookBehindLength = (lookBehindLength << 1) | GetControlBit(ref bitPos, ref currentByte, source);
                    lookBehindOffset = ReadByte(source) | -0x100;
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

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Compress(Stream source, Stream destination)
        {
            // Get the source length
            int sourceLength = (int)(source.Length - source.Position);

            byte[] sourceArray = new byte[sourceLength];
            var totalSourceBytesRead = 0;
            int sourceBytesRead;
            do
            {
                sourceBytesRead = source.Read(sourceArray, totalSourceBytesRead, sourceLength - totalSourceBytesRead);
                if (sourceBytesRead == 0)
                {
                    throw new IOException($"Unable to read all bytes in {nameof(source)}");
                }
                totalSourceBytesRead += sourceBytesRead;
            }
            while (totalSourceBytesRead < sourceLength);

            byte bitPos = 0;
            byte controlByte = 0;

            int position = 0;
            int currentLookBehindPosition, currentLookBehindLength;
            int lookBehindOffset, lookBehindLength;

            MemoryStream data = new MemoryStream();

            while (position < sourceLength)
            {
                currentLookBehindLength = 0;
                lookBehindOffset = 0;
                lookBehindLength = 0;

                for (currentLookBehindPosition = position - 1; (currentLookBehindPosition >= 0) && (currentLookBehindPosition >= position - 0x1FF0) && (lookBehindLength < 256); currentLookBehindPosition--)
                {
                    currentLookBehindLength = 1;
                    if (sourceArray[currentLookBehindPosition] == sourceArray[position])
                    {
                        do
                        {
                            currentLookBehindLength++;
                        } while ((currentLookBehindLength <= 256) &&
                            (position + currentLookBehindLength <= sourceArray.Length) &&
                            sourceArray[currentLookBehindPosition + currentLookBehindLength - 1] == sourceArray[position + currentLookBehindLength - 1]);

                        currentLookBehindLength--;
                        if (((currentLookBehindLength >= 2 && currentLookBehindPosition - position >= -0x100) || currentLookBehindLength >= 3) && currentLookBehindLength > lookBehindLength)
                        {
                            lookBehindOffset = currentLookBehindPosition - position;
                            lookBehindLength = currentLookBehindLength;
                        }
                    }
                }

                if (lookBehindLength == 0)
                {
                    data.WriteByte(sourceArray[position++]);
                    PutControlBit(1, ref controlByte, ref bitPos, data, destination);
                }
                else
                {
                    Copy(lookBehindOffset, lookBehindLength, ref controlByte, ref bitPos, data, destination);
                    position += lookBehindLength;
                }
            }

            PutControlBit(0, ref controlByte, ref bitPos, data, destination);
            PutControlBit(1, ref controlByte, ref bitPos, data, destination);
            if (bitPos != 0)
            {
                controlByte = (byte)((controlByte << bitPos) >> 8);
                Flush(ref controlByte, ref bitPos, data, destination);
            }

            destination.WriteByte(0);
            destination.WriteByte(0);
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
                return remainingLength > 3
                    && (reader.At(startPosition, x => x.ReadByte()) & 0x1) == 1
                    && reader.At(startPosition + remainingLength - 2, x => x.ReadInt16()) == 0;
            }
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