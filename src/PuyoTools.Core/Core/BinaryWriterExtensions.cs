using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PuyoTools.Core
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

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(byte)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteByte(this BinaryWriter writer, byte value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(short)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16(this BinaryWriter writer, short value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(int)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(this BinaryWriter writer, int value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(long)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(this BinaryWriter writer, long value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(ushort)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16(this BinaryWriter writer, ushort value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(uint)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(this BinaryWriter writer, uint value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        /// <remarks>This method is an alias for <see cref="BinaryWriter.Write(ulong)"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(this BinaryWriter writer, ulong value) => writer.Write(value);

        /// <summary>
        /// Writes a two-byte signed integer in big-endian format to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        public static void WriteInt16BigEndian(this BinaryWriter writer, short value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a four-byte signed integer in big-endian format to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        public static void WriteInt32BigEndian(this BinaryWriter writer, int value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes an eight-byte signed integer in big-endian format to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        public static void WriteInt64BigEndian(this BinaryWriter writer, long value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a two-byte unsigned integer in big-endian format to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        public static void WriteUInt16BigEndian(this BinaryWriter writer, ushort value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a four-byte unsigned integer in big-endian format to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        public static void WriteUInt32BigEndian(this BinaryWriter writer, uint value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes an eight-byte unsigned integer in big-endian format to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        public static void WriteUInt64BigEndian(this BinaryWriter writer, ulong value) => writer.Write(BinaryPrimitives.ReverseEndianness(value));

        /// <summary>
        /// Writes a two-byte signed integer in the specified endianness to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        public static void WriteInt16(this BinaryWriter writer, short value, Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.Little: writer.WriteInt16(value); break;
                case Endianness.Big: writer.WriteInt16BigEndian(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(endianness));
            }
        }

        /// <summary>
        /// Writes a four-byte signed integer in the specified endianness to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        public static void WriteInt32(this BinaryWriter writer, int value, Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.Little: writer.WriteInt32(value); break;
                case Endianness.Big: writer.WriteInt32BigEndian(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(endianness));
            }
        }

        /// <summary>
        /// Writes an eight-byte signed integer the specified endianness to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        public static void WriteInt64(this BinaryWriter writer, long value, Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.Little: writer.WriteInt64(value); break;
                case Endianness.Big: writer.WriteInt64BigEndian(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(endianness));
            }
        }

        /// <summary>
        /// Writes a two-byte unsigned integer the specified endianness to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        public static void WriteUInt16(this BinaryWriter writer, ushort value, Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.Little: writer.WriteUInt16(value); break;
                case Endianness.Big: writer.WriteUInt16BigEndian(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(endianness));
            }
        }

        /// <summary>
        /// Writes a four-byte unsigned integer the specified endianness to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        public static void WriteUInt32(this BinaryWriter writer, uint value, Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.Little: writer.WriteUInt32(value); break;
                case Endianness.Big: writer.WriteUInt32BigEndian(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(endianness));
            }
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer the specified endianness to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        public static void WriteUInt64(this BinaryWriter writer, ulong value, Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.Little: writer.WriteUInt64(value); break;
                case Endianness.Big: writer.WriteUInt64BigEndian(value); break;
                default: throw new ArgumentOutOfRangeException(nameof(endianness));
            }
        }

        /// <summary>
        /// Writes a string to this stream, and advances the current position of the stream by the number of bytes specified in <paramref name="count"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The value to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <remarks><paramref name="value"/> will be truncated if the number of bytes is greater than <paramref name="count"/>.</remarks>
        public static void WriteString(this BinaryWriter writer, string value, int count) => WriteString(writer, value, count, Encoding.UTF8);

        /// <summary>
        /// Writes a string to this stream using the specified encoding, and advances the current position of the stream by the number of bytes specified in <paramref name="count"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">The value to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <remarks><paramref name="value"/> will be truncated if the number of bytes is greater than <paramref name="count"/>.</remarks>
        public static void WriteString(this BinaryWriter writer, string value, int count, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            if (bytes.Length >= count)
            {
                writer.Write(bytes, 0, count);
            }
            else
            {
                writer.Write(bytes);
                writer.Write(new byte[count - bytes.Length]);
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

        /// <summary>
        /// Aligns the position of this stream to the specified alignment.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="alignment">The byte alignment.</param>
        /// <param name="offset">The offset to relatively align this stream to.</param>
        /// <param name="paddingValue">The byte value to write as padding when alignment is needed.</param>
        /// <returns>The position of this stream after alignment.</returns>
        public static long Align(this BinaryWriter writer, int alignment, long offset = 0, byte paddingValue = default)
        {
            long position = writer.BaseStream.Position - offset;

            if (position % alignment != 0)
            {
                byte[] buffer = new byte[alignment - (position % alignment)];
                if (paddingValue != default)
                {
                    Array.Fill(buffer, paddingValue);
                }
                writer.Write(buffer);
            }

            return writer.BaseStream.Position;
        }
    }
}
