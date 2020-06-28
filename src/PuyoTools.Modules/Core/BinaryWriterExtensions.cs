using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Modules
{
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Invokes <paramref name="func"/> with the position of the stream set to the given value.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="position"></param>
        /// <param name="func"></param>
        /// <remarks>The position of the stream is set to the previous position after <paramref name="func"/> is invoked.</remarks>
        public static void At(this BinaryWriter writer, long position, Action<BinaryWriter> func)
        {
            var origPosition = writer.BaseStream.Position;
            if (origPosition != position)
            {
                writer.BaseStream.Position = position;
            }

            try
            {
                func(writer);
            }
            finally
            {
                writer.BaseStream.Position = origPosition;
            }
        }

        /// <summary>
        /// Invokes <paramref name="func"/> with the position of the stream set to the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="position"></param>
        /// <param name="func"></param>
        /// <returns>The value returned by <paramref name="func"/>.</returns>
        /// <remarks>The position of the stream is set to the previous position after <paramref name="func"/> is invoked.</remarks>
        public static T At<T>(this BinaryWriter writer, long position, Func<BinaryWriter, T> func)
        {
            var origPosition = writer.BaseStream.Position;
            if (origPosition != position)
            {
                writer.BaseStream.Position = position;
            }

            T value;
            try
            {
                value = func(writer);
            }
            finally
            {
                writer.BaseStream.Position = origPosition;
            }

            return value;
        }

        /// <summary>
        /// Writes a two-byte signed integer using the specified endianess to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The two-byte signed integer to write.</param>
        /// <param name="endianess">The endianess to use.</param>
        public static void Write(this BinaryWriter writer, short value, Endianess endianess)
        {
            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            writer.Write(value);
        }

        /// <summary>
        /// Writes a four-byte signed integer using the specified endianess to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The four-byte signed integer to write.</param>
        /// <param name="endianess">The endianess to use.</param>
        public static void Write(this BinaryWriter writer, int value, Endianess endianess)
        {
            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            writer.Write(value);
        }

        /// <summary>
        /// Writes an eight-byte signed integer using the specified endianess to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The eight-byte signed integer to write.</param>
        /// <param name="endianess">The endianess to use.</param>
        public static void Write(this BinaryWriter writer, long value, Endianess endianess)
        {
            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            writer.Write(value);
        }

        /// <summary>
        /// Writes a two-byte unsigned integer using the specified endianess to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        /// <param name="endianess">The endianess to use.</param>
        public static void Write(this BinaryWriter writer, ushort value, Endianess endianess)
        {
            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            writer.Write(value);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer using the specified endianess to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        /// <param name="endianess">The endianess to use.</param>
        public static void Write(this BinaryWriter writer, uint value, Endianess endianess)
        {
            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            writer.Write(value);
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer using the specified endianess to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        /// <param name="endianess">The endianess to use.</param>
        public static void Write(this BinaryWriter writer, ulong value, Endianess endianess)
        {
            if (endianess == Endianess.Big || (endianess == Endianess.Native && !BitConverter.IsLittleEndian))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            writer.Write(value);
        }

        /// <summary>
        /// Writes a two-byte signed integer in big-endian format to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The two-byte signed integer to write.</param>
        public static void WriteInt16BigEndian(this BinaryWriter writer, short value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a four-byte signed integer in big-endian format to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The four-byte signed integer to write.</param>
        public static void WriteInt32BigEndian(this BinaryWriter writer, int value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes an eight-byte signed integer in big-endian format to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The eight-byte signed integer to write.</param>
        public static void WriteInt64BigEndian(this BinaryWriter writer, long value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a two-byte unsigned integer in big-endian format to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        public static void WriteUInt16BigEndian(this BinaryWriter writer, ushort value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a four-byte unsigned integer in big-endian format to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        public static void WriteUInt32BigEndian(this BinaryWriter writer, uint value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes an eight-byte unsigned integer in big-endian format to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        public static void WriteUInt64BigEndian(this BinaryWriter writer, ulong value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a string to this stream, and advances the current position of the stream by the number of bytes specified in <paramref name="count"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The value to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <remarks><paramref name="value"/> will be truncated if the number of bytes is greater than <paramref name="count"/>.</remarks>
        public static void Write(this BinaryWriter writer, string value, int count) => Write(writer, value, count, Encoding.UTF8);

        /// <summary>
        /// Writes a string to this stream using the specified encoding, and advances the current position of the stream by the number of bytes specified in <paramref name="count"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The value to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <remarks><paramref name="value"/> will be truncated if the number of bytes is greater than <paramref name="count"/>.</remarks>
        public static void Write(this BinaryWriter writer, string value, int count, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            if (bytes.Length >= count)
            {
                writer.Write(bytes, 0, count);
            }
            else
            {
                writer.Write(bytes);
                writer.Write(Enumerable.Repeat((byte)0, count - bytes.Length).ToArray());
            }
        }

        /// <summary>
        /// Writes a null-terminated string to this stream, and advances the current position of the stream by the length of <paramref name="value"/> in bytes plus one.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The value to write.</param>
        /// <returns>The number of bytes written to the stream.</returns>
        public static int WriteNullTerminatedString(this BinaryWriter writer, string value) => WriteNullTerminatedString(writer, value, Encoding.UTF8);

        /// <summary>
        /// Writes a null-terminated string to this stream using the specified encoding, and advances the current position of the stream by the length of <paramref name="value"/> in bytes plus one.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The value to write.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>The number of bytes written to the stream.</returns>
        public static int WriteNullTerminatedString(this BinaryWriter writer, string value, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            writer.Write(bytes);
            writer.Write((byte)0);

            return bytes.Length + 1;
        }
    }
}
