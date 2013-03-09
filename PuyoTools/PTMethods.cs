using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PuyoTools2
{
    public static class PTMethods
    {
        /// <summary>
        /// Compares two arrays to see if they are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns>True if the arrays are equal.</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        public static int RoundUp(int value, int roundUpTo)
        {
            // Return the same number if it is already a multiple
            if (value % roundUpTo == 0)
                return value;

            return value + (roundUpTo - (value % roundUpTo));
        }
    }

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
        /// Reads the bytes from the current stream and writes them to another stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        public static void CopyTo(Stream source, Stream destination)
        {
            CopyTo(source, destination, 4096);
        }

        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 4096.</param>
        public static void CopyTo(Stream source, Stream destination, int bufferSize)
        {
            int num;
            byte[] buffer = new byte[bufferSize];
            while ((num = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, num);
            }
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

            int copyLength = bufferSize;
            if (copyLength > length)
            {
                copyLength = length;
            }

            while ((num = source.Read(buffer, 0, copyLength)) != 0)
            {
                destination.Write(buffer, 0, num);

                if (source.Position + copyLength > source.Length)
                {
                    copyLength = (int)(source.Length - source.Position);
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
        /// <param name="inStream">Stream to check</param>
        /// <param name="offset">Offset to check (relative to the current stream position)</param>
        /// <param name="length">Number of bytes to check.</param>
        /// <param name="a">Byte array to compare to</param>
        /// <returns></returns>
        public static bool Contains(Stream inStream, long offset, byte[] a)
        {
            // First, let's do the logical thing and make sure a.Length > 0 and a != null
            if (a.Length > 0 && a == null)
                return false;

            // Go to the offset we want to check.
            // In this case, offset is relative to the position of the stream
            long oldPosition = inStream.Position;
            inStream.Position += offset;

            // Read in the buffer now
            byte[] buffer = new byte[a.Length];
            inStream.Read(buffer, 0, a.Length);

            // Reset the position of the stream back to oldPosition
            inStream.Position = oldPosition;

            // Now let's check to see if the stream contains a
            return PTMethods.ArraysEqual<byte>(buffer, a);
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

        public static void WriteInt16(Stream destination, short value)
        {
            destination.WriteByte((byte)(value & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public static void WriteInt32(Stream destination, int value)
        {
            destination.WriteByte((byte)(value & 0xFF));
            destination.WriteByte((byte)((value >> 8) & 0xFF));
            destination.WriteByte((byte)((value >> 16) & 0xFF));
            destination.WriteByte((byte)((value >> 24) & 0xFF));
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