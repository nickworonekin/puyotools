using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core
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
        /// Reads a 2-byte signed integer from the current stream in the specified endianness and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianness">The endianess of the value to read.</param>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        public static short ReadInt16(this BinaryReader reader, Endianness endianness) => endianness switch
        {
            Endianness.Little => reader.ReadInt16(),
            Endianness.Big => reader.ReadInt16BigEndian(),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness)),
        };

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream in the specified endianness and advances the current position of the stream by four bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianness">The endianess of the value to read.</param>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        public static int ReadInt32(this BinaryReader reader, Endianness endianness) => endianness switch
        {
            Endianness.Little => reader.ReadInt32(),
            Endianness.Big => reader.ReadInt32BigEndian(),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness)),
        };

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream in the specified endianness and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianness">The endianess of the value to read.</param>
        /// <returns>An 8-byte signed integer read from the current stream.</returns>
        public static long ReadInt64(this BinaryReader reader, Endianness endianness) => endianness switch
        {
            Endianness.Little => reader.ReadInt64(),
            Endianness.Big => reader.ReadInt64BigEndian(),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness)),
        };

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream in the specified endianness and advances the position of the stream by two bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianness">The endianess of the value to read.</param>
        /// <returns>A 2-byte unsigned integer read from this stream.</returns>
        public static ushort ReadUInt16(this BinaryReader reader, Endianness endianness) => endianness switch
        {
            Endianness.Little => reader.ReadUInt16(),
            Endianness.Big => reader.ReadUInt16BigEndian(),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness)),
        };

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream in the specified endianness and advances the position of the stream by four bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianness">The endianess of the value to read.</param>
        /// <returns>A 4-byte unsigned integer read from this stream.</returns>
        public static uint ReadUInt32(this BinaryReader reader, Endianness endianness) => endianness switch
        {
            Endianness.Little => reader.ReadUInt32(),
            Endianness.Big => reader.ReadUInt32BigEndian(),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness)),
        };

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream the specified endianness and advances the position of the stream by eight bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="endianness">The endianess of the value to read.</param>
        /// <returns>An 8-byte unsigned integer read from this stream.</returns>
        public static ulong ReadUInt64(this BinaryReader reader, Endianness endianness) => endianness switch
        {
            Endianness.Little => reader.ReadUInt64(),
            Endianness.Big => reader.ReadUInt64BigEndian(),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness)),
        };

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
