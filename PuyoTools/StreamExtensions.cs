using System;
using System.IO;
using System.Text;

namespace PuyoTools.Old
{
    // Stream Reader Functions
    public static class StreamReaderExtensions
    {
        // Read a byte
        public static byte ReadByte(this Stream stream, long offset)
        {
            stream.Position = offset;

            return (byte)stream.ReadByte();
        }

        // Read bytes
        public static byte[] ReadBytes(this Stream stream, long offset, int length)
        {
            byte[] array = new byte[length];
            stream.Position = offset;
            stream.Read(array, 0, length);

            return array;
        }
        public static byte[] ReadBytes(this Stream stream, long offset, uint length)
        {
            return stream.ReadBytes(offset, (int)length);
        }

        // Read a short
        public static short ReadShort(this Stream stream, long offset)
        {
            byte[] array = new byte[2];
            stream.Position = offset;
            stream.Read(array, 0, 2);

            return BitConverter.ToInt16(array, 0);
        }
        public static ushort ReadUShort(this Stream stream, long offset)
        {
            byte[] array = new byte[2];
            stream.Position = offset;
            stream.Read(array, 0, 2);

            return BitConverter.ToUInt16(array, 0);
        }

        // Read an integer
        public static int ReadInt(this Stream stream, long offset)
        {
            byte[] array = new byte[4];
            stream.Position = offset;
            stream.Read(array, 0, 4);

            return BitConverter.ToInt32(array, 0);
        }
        public static uint ReadUInt(this Stream stream, long offset)
        {
            byte[] array = new byte[4];
            stream.Position = offset;
            stream.Read(array, 0, 4);

            return BitConverter.ToUInt32(array, 0);
        }

        // Read a string
        public static string ReadString(this Stream stream, long offset, int maxLength, bool nullTerminator)
        {
            string str = string.Empty;
            stream.Position = offset;

            for (int i = 0; i < maxLength; i++)
            {
                char chr = (char)stream.ReadByte();
                if (chr == '\0' && nullTerminator)
                    break;
                else
                    str += chr;
            }

            return str;
        }
        public static string ReadString(this Stream stream, long offset, int maxLength)
        {
            return stream.ReadString(offset, maxLength, true);
        }

        // Read a string with a specific encoding
        public static string ReadString(this Stream stream, long offset, int maxLength, Encoding encoding, bool nullTerminator)
        {
            stream.Position = offset;

            byte[] array = new byte[maxLength];
            stream.Read(array, 0x00, maxLength);

            string str = encoding.GetString(array);
            if (nullTerminator)
                str = str.TrimEnd('\0');

            return str;
        }

        public static string ReadString(this Stream stream, long offset, int maxLength, Encoding encoding)
        {
            return stream.ReadString(offset, maxLength, encoding, true);
        }

        // Copy to a byte array
        public static byte[] ToByteArray(this Stream stream)
        {
            byte[] array = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(array, 0, array.Length);

            return array;
        }
    }

    // Stream Writer Functions
    public static class StreamWriterExtensions
    {
        // Write an byte
        public static void Write(this Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        // Write a short
        public static void Write(this Stream stream, short value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 2);
        }
        public static void Write(this Stream stream, ushort value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        // Write an integer
        public static void Write(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }
        public static void Write(this Stream stream, uint value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        // Write a byte array
        public static void Write(this Stream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }

        // Write a string
        public static void Write(this Stream stream, string value)
        {
            for (int i = 0; i < value.Length; i++)
                stream.WriteByte((byte)value[i]);
        }
        public static void Write(this Stream stream, string value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                    stream.WriteByte((byte)value[i]);
                else
                    stream.WriteByte(0x0);
            }
        }
        public static void Write(this Stream stream, string value, int strLength, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length && i < strLength)
                    stream.WriteByte((byte)value[i]);
                else
                    stream.WriteByte(0x0);
            }
        }

        // Write a string with a specific encoding
        public static void Write(this Stream stream, string value, int strLength, int length, Encoding encoding)
        {
            byte[] strArray = encoding.GetBytes(value);

            int trimAmount = 0;
            while (strArray.Length > strLength)
            {
                trimAmount++;
                strArray = encoding.GetBytes(value.Substring(0, value.Length - trimAmount));
            }

            for (int i = 0; i < length; i++)
            {
                if (i < strArray.Length && i < strLength)
                    stream.WriteByte(strArray[i]);
                else
                    stream.WriteByte(0x00);
            }
        }

        // Write a stream
        public static void Write(this Stream output, Stream input)
        {
            byte[] buffer  = new byte[4096];
            input.Position = 0;

            int bytes;
            while ((bytes = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, bytes);
        }
        public static void Write(this Stream output, MemoryStream input)
        {
            input.Position = 0;
            input.WriteTo(output);
        }
        public static void Write(this Stream output, Stream input, long offset, long length)
        {
            byte[] buffer  = new byte[4096];
            input.Position = offset;

            int bytes;
            while ((bytes = input.Read(buffer, 0, (int)(input.Position + buffer.Length > offset + length ? offset + length - input.Position : buffer.Length))) > 0)
                output.Write(buffer, 0, bytes);
        }

        // Copy stream
        public static MemoryStream Copy(this Stream input)
        {
            MemoryStream output = new MemoryStream((int)input.Length);
            byte[] buffer  = new byte[4096];
            input.Position = 0;

            int bytes;
            while ((bytes = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, bytes);

            return output;
        }
        public static MemoryStream Copy(this Stream input, long offset, int length)
        {
            MemoryStream output = new MemoryStream(length);
            byte[] buffer  = new byte[4096];
            input.Position = offset;

            int bytes;
            while ((bytes = input.Read(buffer, 0, (int)(input.Position + buffer.Length > offset + length ? offset + length - input.Position : buffer.Length))) > 0)
                output.Write(buffer, 0, bytes);

            return output;
        }
        public static MemoryStream Copy(this Stream input, long offset, uint length)
        {
            MemoryStream output = new MemoryStream((int)length);
            byte[] buffer  = new byte[4096];
            input.Position = offset;

            int bytes;
            while ((bytes = input.Read(buffer, 0, (int)(input.Position + buffer.Length > offset + length ? offset + length - input.Position : buffer.Length))) > 0)
                output.Write(buffer, 0, bytes);

            return output;
        }
    }
}