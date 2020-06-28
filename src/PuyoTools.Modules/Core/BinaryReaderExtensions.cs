using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PuyoTools.Modules
{
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Invokes <paramref name="func"/> with the position of the stream set to the given value.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="position"></param>
        /// <param name="func"></param>
        /// <remarks>The position of the stream is set to the previous position after <paramref name="func"/> is invoked.</remarks>
        public static void At(this BinaryReader reader, long position, Action<BinaryReader> func)
        {
            var origPosition = reader.BaseStream.Position;
            if (origPosition != position)
            {
                reader.BaseStream.Position = position;
            }

            try
            {
                func(reader);
            }
            finally
            {
                reader.BaseStream.Position = origPosition;
            }
        }

        /// <summary>
        /// Invokes <paramref name="func"/> with the position of the stream set to the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="position"></param>
        /// <param name="func"></param>
        /// <returns>The value returned by <paramref name="func"/>.</returns>
        /// <remarks>The position of the stream is set to the previous position after <paramref name="func"/> is invoked.</remarks>
        public static T At<T>(this BinaryReader reader, long position, Func<BinaryReader, T> func)
        {
            var origPosition = reader.BaseStream.Position;
            if (origPosition != position)
            {
                reader.BaseStream.Position = position;
            }

            T value;
            try
            {
                value = func(reader);
            }
            finally
            {
                reader.BaseStream.Position = origPosition;
            }

            return value;
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream using the specified endianess and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianess">The endianess to use.</param>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public static short ReadInt16(this BinaryReader reader, Endianess endianess)
        {
            var value = reader.ReadInt16();

            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream using the specified endianess and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianess">The endianess to use.</param>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public static int ReadInt32(this BinaryReader reader, Endianess endianess)
        {
            var value = reader.ReadInt32();

            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream using the specified endianess and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianess">The endianess to use.</param>
        /// <returns>An 8-byte signed integer read from the current stream.</returns>
        public static long ReadInt64(this BinaryReader reader, Endianess endianess)
        {
            var value = reader.ReadInt64();

            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream using the specified endianess and advances the position of the stream by two bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianess">The endianess to use.</param>
        /// <returns>A 2-byte unsigned integer read from this stream.</returns>
        public static ushort ReadUInt16(this BinaryReader reader, Endianess endianess)
        {
            var value = reader.ReadUInt16();

            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream using the specified endianess and advances the position of the stream by four bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianess">The endianess to use.</param>
        /// <returns>A 4-byte unsigned integer read from this stream.</returns>
        public static uint ReadUInt32(this BinaryReader reader, Endianess endianess)
        {
            var value = reader.ReadUInt32();

            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream using the specified endianess and advances the position of the stream by eight bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianess">The endianess to use.</param>
        /// <returns>An 8-byte unsigned integer read from this stream.</returns>
        public static ulong ReadUInt64(this BinaryReader reader, Endianess endianess)
        {
            var value = reader.ReadUInt64();

            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream in big-endian format and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public static short ReadInt16BigEndian(this BinaryReader reader) => BinaryPrimitives.ReverseEndianness(reader.ReadInt16());

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream in big-endian format and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public static int ReadInt32BigEndian(this BinaryReader reader) => BinaryPrimitives.ReverseEndianness(reader.ReadInt32());

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream in big-endian format and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>An 8-byte signed integer read from the current stream.</returns>
        public static long ReadInt64BigEndian(this BinaryReader reader) => BinaryPrimitives.ReverseEndianness(reader.ReadInt64());

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream in big-endian format and advances the position of the stream by two bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>A 2-byte unsigned integer read from this stream.</returns>
        public static ushort ReadUInt16BigEndian(this BinaryReader reader) => BinaryPrimitives.ReverseEndianness(reader.ReadUInt16());

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream in big-endian format and advances the position of the stream by four bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>A 4-byte unsigned integer read from this stream.</returns>
        public static uint ReadUInt32BigEndian(this BinaryReader reader) => BinaryPrimitives.ReverseEndianness(reader.ReadUInt32());

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream in big-endian format and advances the position of the stream by eight bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>An 8-byte unsigned integer read from this stream.</returns>
        public static ulong ReadUInt64BigEndian(this BinaryReader reader) => BinaryPrimitives.ReverseEndianness(reader.ReadUInt64());

        /// <summary>
        /// Reads a string from the current stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns></returns>
        public static string ReadString(this BinaryReader reader, int count) => ReadString(reader, count, Encoding.UTF8);

        /// <summary>
        /// Reads a string from the current stream with the specified encoding.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="count">The number of bytes to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns></returns>
        public static string ReadString(this BinaryReader reader, int count, Encoding encoding)
        {
            var s = encoding.GetString(reader.ReadBytes(count));
            var indexOfNull = s.IndexOf('\0');

            if (indexOfNull != -1)
            {
                s = s.Remove(indexOfNull);
            }

            return s;
        }

        /// <summary>
        /// Reads a null-terminated string from the current stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadNullTerminatedString(this BinaryReader reader) => ReadNullTerminatedString(reader, Encoding.UTF8);

        /// <summary>
        /// Reads a null-terminated string from the current stream with the specified encoding.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns></returns>
        public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
        {
            var bytes = new List<byte>();

            byte c;
            while ((c = reader.ReadByte()) != 0)
            {
                bytes.Add(c);
            }

            return encoding.GetString(bytes.ToArray());
        }
    }
}
