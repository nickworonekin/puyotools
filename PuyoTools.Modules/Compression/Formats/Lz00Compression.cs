using System;
using System.IO;
using System.Text;

namespace PuyoTools.Modules.Compression
{
    public class Lz00Compression : CompressionBase
    {
        /*
         * LZ00 decompression support by QPjDDYwQLI thanks to the author of ps2dis
         */

        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "LZ00"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 4;

            // Get the source length, destination length, and encryption key
            int sourceLength = PTStream.ReadInt32(source);

            source.Position += 40;

            int destinationLength = PTStream.ReadInt32(source);
            uint key = PTStream.ReadUInt32(source);

            source.Position += 8;

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x40;
            int destinationPointer = 0x0;
            int bufferPointer      = 0xFEE;

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = ReadByte(source, ref key);
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x1) != 0) // Not compressed
                    {
                        byte value = ReadByte(source, ref key);
                        sourcePointer++;

                        destination.WriteByte(value);
                        destinationPointer++;

                        buffer[bufferPointer] = value;
                        bufferPointer = (bufferPointer + 1) & 0xFFF;
                    }
                    else // Compressed
                    {
                        byte b1 = ReadByte(source, ref key), b2 = ReadByte(source, ref key);
                        sourcePointer += 2;

                        int matchOffset = (((b2 >> 4) & 0xF) << 8) | b1;
                        int matchLength = (b2 & 0xF) + 3;

                        for (int j = 0; j < matchLength; j++)
                        {
                            destination.WriteByte(buffer[(matchOffset + j) & 0xFFF]);
                            destinationPointer++;

                            buffer[bufferPointer] = buffer[(matchOffset + j) & 0xFFF];
                            bufferPointer = (bufferPointer + 1) & 0xFFF;
                        }
                    }

                    // Check to see if we reached the end of the source
                    if (sourcePointer >= sourceLength)
                    {
                        break;
                    }

                    // Check to see if we wrote too much data to the destination
                    if (destinationPointer > destinationLength)
                    {
                        throw new Exception("Too much data written to the destination.");
                    }

                    flag >>= 1;
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
            long destinationStart = destination.Position;

            // Get the source length, and read it in to an array
            int sourceLength = (int)(source.Length - source.Position);
            byte[] sourceArray = new byte[sourceLength];
            source.Read(sourceArray, 0, sourceLength);

            // Get the encryption key
            uint key = (uint)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

            // Set the source and destination pointers
            int sourcePointer      = 0x0;
            int destinationPointer = 0x40;

            // Initalize the LZ dictionary
            LzBufferDictionary dictionary = new LzBufferDictionary();
            dictionary.SetBufferSize(0x1000);
            dictionary.SetBufferStart(0xFEE);
            dictionary.SetMaxMatchAmount(0xF + 3);

            // Write out the header
            // Magic code
            destination.WriteByte((byte)'L');
            destination.WriteByte((byte)'Z');
            destination.WriteByte((byte)'0');
            destination.WriteByte((byte)'0');
            PTStream.WriteInt32(destination, 0); // Compressed length (will be filled in later)
            PTStream.WriteInt32(destination, 0);
            PTStream.WriteInt32(destination, 0);

            PTStream.WriteCString(destination, Path.GetFileName(DestinationPath), 32, Encoding.GetEncoding("Shift_JIS")); // File name
            PTStream.WriteInt32(destination, sourceLength); // Decompressed length
            PTStream.WriteUInt32(destination, key); // Encryption key
            PTStream.WriteInt32(destination, 0);
            PTStream.WriteInt32(destination, 0);

            // Start compression
            while (sourcePointer < sourceLength)
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    byte flag = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        // Search for a match
                        int[] match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);

                        if (match[1] > 0) // There is a match
                        {
                            buffer.WriteByte((byte)(match[0] & 0xFF));
                            buffer.WriteByte((byte)(((match[0] & 0xF00) >> 4) | ((match[1] - 3) & 0xF)));

                            dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);

                            sourcePointer += match[1];
                        }
                        else // There is not a match
                        {
                            flag |= (byte)(1 << i);

                            buffer.WriteByte(sourceArray[sourcePointer]);

                            dictionary.AddEntry(sourceArray, sourcePointer);

                            sourcePointer++;
                        }

                        // Check to see if we reached the end of the file
                        if (sourcePointer >= sourceLength)
                            break;
                    }

                    // Flush the buffer and write it to the destination stream
                    WriteByte(destination, flag, ref key);

                    buffer.Position = 0;
                    while (buffer.Position < buffer.Length)
                    {
                        byte value = PTStream.ReadByte(buffer);
                        WriteByte(destination, value, ref key);
                    }

                    destinationPointer += (int)buffer.Length + 1;
                }
            }

            // Go back to the beginning of the file and write out the compressed length
            long currentPosition = destination.Position;
            destination.Position = destinationStart + 4;
            PTStream.WriteInt32(destination, destinationPointer);
            destination.Position = currentPosition;
        }

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 64 &&
                PTStream.Contains(source, 0, new byte[] { (byte)'L', (byte)'Z', (byte)'0', (byte)'0' }) &&
                PTStream.ReadInt32At(source, source.Position + 4) == length);
        }

        private byte ReadByte(Stream source, ref uint key)
        {
            return Get(PTStream.ReadByte(source), ref key);
        }

        private void WriteByte(Stream destination, byte value, ref uint key)
        {
            destination.WriteByte(Get(value, ref key));
        }

        private byte Get(byte value, ref uint key)
        {
            // Generate a new key
            uint x = (((((((key << 1) + key) << 5) - key) << 5) + key) << 7) - key;
            x = (x << 6) - x;
            x = (x << 4) - x;

            key = ((x << 2) - x) + 12345;

            // Now return the value since we have the key
            uint t = (key >> 16) & 0x7FFF;
            return (byte)(value ^ ((((t << 8) - t) >> 15)));
        }
    }
}