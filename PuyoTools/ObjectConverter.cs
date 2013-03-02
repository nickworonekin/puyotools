using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools
{
    public static class StreamConverter
    {
        // Copy a stream to another stream
        public static Stream Copy(this Stream stream)
        {
            return Copy(stream, 0x0, (int)stream.Length);
        }

        // Copy part of stream
        public static Stream Copy(Stream stream, int offset, int length)
        {
            try
            {
                MemoryStream outputStream = new MemoryStream(length);

                stream.Position = offset;
                for (int i = 0; i < length; i++)
                    outputStream.WriteByte((byte)stream.ReadByte());

                return outputStream;
            }
            catch
            {
                return null;
            }
        }
        public static Stream Copy(Stream stream, uint offset, uint length)
        {
            return Copy(stream, (int)offset, (int)length);
        }
        public static Stream Copy(Stream stream, long offset, long length)
        {
            return Copy(stream, (int)offset, (int)length);
        }

        // Convert stream to string
        public static string ToString(Stream stream, long offset, uint maxLength, bool nullBytes)
        {
            try
            {
                string str = String.Empty;

                stream.Position = offset;
                for (int i = 0; i < maxLength && offset + i < stream.Length; i++)
                {
                    char strChar = (char)stream.ReadByte();
                    if (strChar == '\0' && !nullBytes)
                        break;
                    else
                        str += strChar;
                }

                return str;
            }
            catch
            {
                return null;
            }
        }
        public static string ToString(Stream stream, long offset, uint maxLength)
        {
            return ToString(stream, offset, maxLength, false);
        }
        public static string ToString(Stream stream, int offset, int maxLength)
        {
            return ToString(stream, offset, (uint)maxLength, false);
        }

        // Convert Stream to Byte Array
        public static byte[] ToByteArray(Stream stream, int offset, int length)
        {
            byte[] byteArray = new byte[length];
            stream.Position  = offset;
            stream.Read(byteArray, 0, length);
            return byteArray;
        }
        public static byte[] ToByteArray(Stream stream, uint offset, int length)
        {
            return ToByteArray(stream, (int)offset, length);
        }
        public static byte[] ToByteArray(Stream stream, uint offset, uint length)
        {
            return ToByteArray(stream, (int)offset, (int)length);
        }

        // Convert stream to byte list
        public static List<byte> ToByteList(Stream stream, int offset, int length)
        {
            List<byte> byteList = new List<byte>(length);
            stream.Position = offset;

            for (int i = 0; i < length; i++)
                byteList.Add((byte)stream.ReadByte());

            return byteList;
        }

        // Convert Stream to unsigned integer
        public static uint ToUInt(Stream stream, int offset)
        {
            return BitConverter.ToUInt32(ToByteArray(stream, offset, 4), 0);
        }
        public static uint ToUInt(Stream stream, uint offset)
        {
            return BitConverter.ToUInt32(ToByteArray(stream, (int)offset, 4), 0);
        }

        // Convert Stream to unsigned short
        public static ushort ToUShort(Stream stream, int offset)
        {
            return BitConverter.ToUInt16(ToByteArray(stream, offset, 2), 0);
        }

        // Convert Stream to Byte
        public static byte ToByte(Stream stream, long offset)
        {
            stream.Position = offset;
            return (byte)stream.ReadByte();
        }
    }

    public static class StringConverter
    {
        // Convert string to byte array
        public static byte[] ToByteArray(string str, int length)
        {
            byte[] byteArray = new byte[length];

            for (int i = 0; i < length; i++)
            {
                if (i < str.Length)
                    byteArray[i] = (byte)str[i];
                else
                    byteArray[i] = 0;
            }

            return byteArray;
        }

        // Convert string to byte list
        public static List<byte> ToByteList(string str, int strLength, int length)
        {
            List<byte> byteList = new List<byte>(length);

            for (int i = 0; i < length; i++)
            {
                if (i < str.Length && i < strLength)
                    byteList.Add((byte)str[i]);
                else
                    byteList.Add(0);
            }

            return byteList;
        }
        public static List<byte> ToByteList(string str, int length)
        {
            return ToByteList(str, length, length);
        }
    }

    public static class ByteConverter
    {
        // Convert byte array to string
        public static string ToString(byte[] byteArray, int maxLength)
        {
            string str = String.Empty;

            for (int i = 0; i < byteArray.Length && i < maxLength; i++)
                str += (char)byteArray[i];

            return str;
        }

        // Convert byte array to byte list
        public static List<byte> ToByteList(byte[] byteArray)
        {
            List<byte> byteList = new List<byte>(byteArray.Length);

            for (int i = 0; i < byteArray.Length; i++)
                byteList.Add(byteArray[i]);

            return byteList;
        }

        // Convert byte list to Stream
        public static Stream ToStream(List<byte> byteList)
        {
            Stream stream = new MemoryStream(byteList.Count);

            for (int i = 0; i < byteList.Count; i++)
                stream.WriteByte(byteList[i]);

            return stream;
        }
    }

    public static class NumberConverter
    {
        // Convert integer to byte list
        public static List<byte> ToByteList(this int number)
        {
            return ByteConverter.ToByteList(BitConverter.GetBytes(number));
        }
        public static List<byte> ToByteList(this uint number)
        {
            return ByteConverter.ToByteList(BitConverter.GetBytes(number));
        }

        // Convert short to byte list
        public static List<byte> ToByteList(short number)
        {
            return ByteConverter.ToByteList(BitConverter.GetBytes(number));
        }
        public static List<byte> ToByteList(ushort number)
        {
            return ByteConverter.ToByteList(BitConverter.GetBytes(number));
        }
    }
}