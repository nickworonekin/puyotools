using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core
{
    public static class PTStream
    {
        /// <summary>
        /// Reads a byte from the file and advances the read position one byte.
        /// Similar to Stream.ReadByte but returns a byte and throws an exception if the end of the stream was reached.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>The byte that was read.</returns>
        public static byte ReadByte(Stream source)
        {
            int value = source.ReadByte();
            if (value == -1)
            {
                throw new EndOfStreamException();
            }

            return (byte)value;
        }

        /// <summary>
        /// Writes a byte to the current stream a specified amount of times.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="value">Value to write to the stream.</param>
        /// <param name="amount">Number of times to write the value to the stream.</param>
        public static void WriteByteRepeated(Stream destination, byte value, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                destination.WriteByte(value);
            }
        }

        public static void CopyToPadded(Stream source, Stream destination, int blockSize, byte paddingByte)
        {
            //CopyPartToPadded(source, destination, (int)(source.Length - source.Position), blockSize, paddingByte);

            var sourceLength = source.Length - source.Position;
            source.CopyTo(destination);

            if (sourceLength % blockSize == 0)
            {
                return;
            }

            var bytes = new byte[blockSize - (sourceLength % blockSize)];
            if (paddingByte != default) // Only set the values if the padding byte isn't 0
            {
                for (var i = 0; i < bytes.Length; i++) // Shame we can't use Array.Fill
                {
                    bytes[i] = paddingByte;
                }
            }
            destination.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Reads some of the bytes from the current stream and writes them to another stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <param name="length">Maximum number of bytes to copy to the other stream.</param>
        public static void CopyPartTo(Stream source, Stream destination, int length)
        {
            CopyPartTo(source, destination, length, 4096);
        }

        /// <summary>
        /// Reads some of the bytes from the current stream and writes them to another stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <param name="length">Maximum number of bytes to copy to the other stream.</param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 4096.</param>
        public static void CopyPartTo(Stream source, Stream destination, int length, int bufferSize)
        {
            int num;
            byte[] buffer = new byte[bufferSize];

            if (source.Position + length > source.Length)
            {
                length = (int)(source.Length - source.Position);
            }

            int amountToRead = length;

            int copyLength = bufferSize;
            if (copyLength > amountToRead)
            {
                copyLength = amountToRead;
            }

            while ((num = source.Read(buffer, 0, copyLength)) != 0)
            {
                destination.Write(buffer, 0, num);

                amountToRead -= num;
                if (copyLength > amountToRead)
                {
                    copyLength = amountToRead;
                }
            }
        }

        public static void CopyPartToPadded(Stream source, Stream destination, int length, int blockSize)
        {
            CopyPartToPadded(source, destination, length, blockSize, 0);
        }

        public static void CopyPartToPadded(Stream source, Stream destination, int length, int blockSize, byte paddingByte)
        {
            CopyPartTo(source, destination, length);

            while (length % blockSize != 0)
            {
                destination.WriteByte(paddingByte);
                length++;
            }
        }

        /// <summary>
        /// Checks to see if the stream contains the specified sequence of bytes.
        /// </summary>
        /// <param name="source">Stream to check</param>
        /// <param name="offset">Offset to check (relative to the current stream position)</param>
        /// <param name="length">Number of bytes to check.</param>
        /// <param name="a">Byte array to compare to</param>
        /// <returns></returns>
        public static bool Contains(Stream source, long offset, byte[] a)
        {
            // First, let's do the logical thing and make sure a.Length > 0 and a != null
            if (a == null || a.Length == 0)
                return false;

            // Go to the offset we want to check.
            // In this case, offset is relative to the position of the stream
            long oldPosition = source.Position;
            source.Position += offset;

            // Read in the buffer now
            byte[] buffer = new byte[a.Length];
            source.Read(buffer, 0, a.Length);

            // Reset the position of the stream back to oldPosition
            source.Position = oldPosition;

            // Now let's check to see if the stream contains a
            return buffer.SequenceEqual(a);
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public static short ReadInt16(Stream source)
        {
            return (short)(ReadByte(source) | ReadByte(source) << 8);
        }

        /// <summary>
        /// Reads a 2-byte signed integer as big endian from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public static short ReadInt16BE(Stream source)
        {
            return (short)(ReadByte(source) << 8 | ReadByte(source));
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public static ushort ReadUInt16(Stream source)
        {
            return (ushort)(ReadByte(source) | ReadByte(source) << 8);
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer as big endian from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 2-byte unsigned integer read from the current stream.</returns>
        public static ushort ReadUInt16BE(Stream source)
        {
            return (ushort)(ReadByte(source) << 8 | ReadByte(source));
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public static int ReadInt32(Stream source)
        {
            return (ReadByte(source) | ReadByte(source) << 8 | ReadByte(source) << 16 | ReadByte(source) << 24);
        }

        /// <summary>
        /// Reads a 4-byte signed integer as big endian from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public static int ReadInt32BE(Stream source)
        {
            return (ReadByte(source) << 24 | ReadByte(source) << 16 | ReadByte(source) << 8 | ReadByte(source));
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public static uint ReadUInt32(Stream source)
        {
            return (uint)(ReadByte(source) | ReadByte(source) << 8 | ReadByte(source) << 16 | ReadByte(source) << 24);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer as big endian from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A 4-byte unsigned integer read from the current stream.</returns>
        public static uint ReadUInt32BE(Stream source)
        {
            return (uint)(ReadByte(source) << 24 | ReadByte(source) << 16 | ReadByte(source) << 8 | ReadByte(source));
        }

        /// <summary>
        /// Writes a 2-byte integer to the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="value">A 2-byte signed integer to write to the current stream.</param>
        public static void WriteInt16(Stream destination, short value)
        {
            destination.WriteByte((byte)(value & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
        }

        /// <summary>
        /// Writes a 4-byte integer to the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="value">A 4-byte signed integer to write to the current stream.</param>
        public static void WriteInt32(Stream destination, int value)
        {
            destination.WriteByte((byte)(value & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
            destination.WriteByte((byte)((value >> 16) & 0xFF));
            destination.WriteByte((byte)((value >> 24) & 0xFF));
        }

        /// <summary>
        /// Writes a 4-byte integer as big endian to the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="value">A 4-byte signed integer to write to the current stream.</param>
        public static void WriteInt32BE(Stream destination, int value)
        {
            destination.WriteByte((byte)((value >> 24) & 0xFF));
            destination.WriteByte((byte)((value >> 16) & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
            destination.WriteByte((byte)(value & 0xFF));
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer to the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="value">A 2-byte unsigned integer to write to the current stream.</param>
        public static void WriteUInt16(Stream destination, ushort value)
        {
            destination.WriteByte((byte)(value & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void WriteUInt16BE(Stream destination, ushort value)
        {
            destination.WriteByte((byte)((value >> 8) & 0xFF));
            destination.WriteByte((byte)(value & 0xFF));
        }

        public static void WriteUInt32(Stream destination, uint value)
        {
            destination.WriteByte((byte)(value & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
            destination.WriteByte((byte)((value >> 16) & 0xFF));
            destination.WriteByte((byte)((value >> 24) & 0xFF));
        }

        public static void WriteUInt32BE(Stream destination, uint value)
        {
            destination.WriteByte((byte)((value >> 24) & 0xFF));
            destination.WriteByte((byte)((value >> 16) & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
            destination.WriteByte((byte)(value & 0xFF));
        }

        public static int ReadInt32At(Stream source, long offset)
        {
            long oldPosition = source.Position;
            source.Position = offset;
            int value = ReadInt32(source);
            source.Position = oldPosition;

            return value;
        }

        public static int ReadInt32BEAt(Stream source, long offset)
        {
            long oldPosition = source.Position;
            source.Position = offset;
            int value = ReadInt32BE(source);
            source.Position = oldPosition;

            return value;
        }

        public static uint ReadUInt32At(Stream source, long offset)
        {
            long oldPosition = source.Position;
            source.Position = offset;
            uint value = ReadUInt32(source);
            source.Position = oldPosition;

            return value;
        }

        public static uint ReadUInt32BEAt(Stream source, long offset)
        {
            long oldPosition = source.Position;
            source.Position = offset;
            uint value = ReadUInt32BE(source);
            source.Position = oldPosition;

            return value;
        }

        /// <summary>
        /// Reads an ASCII encoded null terminated C string from a stream.
        /// The stream is read until a null byte is reached.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>The string read from the stream.</returns>
        public static string ReadCString(Stream source)
        {
            List<byte> buffer = new List<byte>();
            byte value;

            while ((value = ReadByte(source)) != 0)
            {
                buffer.Add(value);
            }

            return Encoding.ASCII.GetString(buffer.ToArray());
        }

        /// <summary>
        /// Reads an ASCII encoded null terminated C string from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>The string read from the stream.</returns>
        public static string ReadCString(Stream source, int length)
        {
            return ReadCString(source, length, Encoding.ASCII);
        }

        /// <summary>
        /// Reads a null terminated C string from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="encoding">Encoding of the string.</param>
        /// <returns>The string read from the stream.</returns>
        public static string ReadCString(Stream source, int length, Encoding encoding)
        {
            // Read in the bytes
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Now, look for the null terminator (if it has one)
            int index = Array.IndexOf<byte>(buffer, 0);
            if (index == -1)
            {
                index = length;
            }

            return encoding.GetString(buffer, 0, index);
        }

        public static string ReadCStringAt(Stream source, int offset, int? length = null, Encoding encoding = null)
        {
            long oldPosition = source.Position;
            try
            {
                source.Position = offset;

                if (length == null)
                {
                    return ReadCString(source);
                }
                else
                {
                    return ReadCString(source, length.Value, encoding ?? Encoding.ASCII);
                }
            }
            finally
            {
                source.Position = oldPosition;
            }
        }

        /// <summary>
        /// Writes an ASCII encoded null terminated C string to a stream.
        /// </summary>
        /// <param name="source">The stream to write to.</param>
        /// <param name="str">The string to write.</param>
        /// <param name="length">Number of bytes to write, including the null terminator and any null bytes.</param>
        public static void WriteCString(Stream source, string str, int length)
        {
            WriteCString(source, str, length, Encoding.ASCII);
        }

        /// <summary>
        /// Writes a null terminated C string to a stream.
        /// </summary>
        /// <param name="source">The stream to write to.</param>
        /// <param name="str">The string to write.</param>
        /// <param name="length">Number of bytes to write, including the null terminator and any null bytes.</param>
        /// <param name="encoding">Encoding of the string to write.</param>
        public static void WriteCString(Stream source, string str, int length, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(str);
            int bytesToWrite = buffer.Length;

            if (bytesToWrite >= length)
            {
                bytesToWrite = length - 1;
            }

            int i = 0;
            for (; i < bytesToWrite; i++)
            {
                source.WriteByte(buffer[i]);
            }
            for (; i < length; i++)
            {
                source.WriteByte(0);
            }
        }
    }
}