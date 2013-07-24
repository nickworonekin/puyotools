using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class Lz10Compression : CompressionBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "LZ10"; }
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
            // Get the source and destination length
            int sourceLength      = (int)(source.Length - source.Position);
            int destinationLength = PTStream.ReadInt32(source) >> 8;

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x4;
            int destinationPointer = 0x0;
            int bufferPointer      = 0x0;

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = PTStream.ReadByte(source);
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x80) == 0) // Not compressed
                    {
                        byte value = PTStream.ReadByte(source);
                        sourcePointer++;

                        destination.WriteByte(value);
                        destinationPointer++;

                        buffer[bufferPointer] = value;
                        bufferPointer = (bufferPointer + 1) & 0xFFF;
                    }
                    else // Compressed
                    {
                        byte b1 = PTStream.ReadByte(source), b2 = PTStream.ReadByte(source);
                        sourcePointer += 2;

                        int matchDistance = (((b1 & 0xF) << 8) | b2) + 1;
                        int matchLength   = (b1 >> 4) + 3;

                        for (int j = 0; j < matchLength; j++)
                        {
                            destination.WriteByte(buffer[(bufferPointer - matchDistance) & 0xFFF]);
                            destinationPointer++;

                            buffer[bufferPointer] = buffer[(bufferPointer - matchDistance) & 0xFFF];
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

                    flag <<= 1;
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

            // LZ10 compression can only handle files smaller than 16MB
            if (sourceLength > 0xFFFFFF)
            {
                throw new Exception("Source is too large. LZ10 compression can only compress files smaller than 16MB.");
            }

            // Read the source data into an array
            byte[] sourceArray = new byte[sourceLength];
            source.Read(sourceArray, 0, sourceLength);

            // Set the source and destination pointers
            int sourcePointer = 0x0;
            int destinationPointer = 0x4;

            // Initalize the LZ dictionary
            LzWindowDictionary dictionary = new LzWindowDictionary();
            dictionary.SetWindowSize(0x1000);
            dictionary.SetMaxMatchAmount(0xF + 3);

            // Write out the header
            // Magic code & decompressed length
            PTStream.WriteInt32(destination, 0x10 | (sourceLength << 8));

            // Start compression
            while (sourcePointer < sourceLength)
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    byte flag = 0;

                    for (int i = 7; i >= 0; i--)
                    {
                        // Search for a match
                        int[] match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);

                        if (match[1] > 0) // There is a match
                        {
                            flag |= (byte)(1 << i);

                            buffer.WriteByte((byte)((((match[1] - 3) & 0xF) << 4) | (((match[0] - 1) & 0xFFF) >> 8)));
                            buffer.WriteByte((byte)((match[0] - 1) & 0xFF));

                            dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);
                            dictionary.SlideWindow(match[1]);

                            sourcePointer += match[1];
                        }
                        else // There is not a match
                        {
                            buffer.WriteByte(sourceArray[sourcePointer]);

                            dictionary.AddEntry(sourceArray, sourcePointer);
                            dictionary.SlideWindow(1);

                            sourcePointer++;
                        }

                        // Check to see if we reached the end of the file
                        if (sourcePointer >= sourceLength)
                            break;
                    }

                    // Flush the buffer and write it to the destination stream
                    destination.WriteByte(flag);

                    buffer.Position = 0;
                    while (buffer.Position < buffer.Length)
                    {
                        byte value = PTStream.ReadByte(buffer);
                        destination.WriteByte(value);
                    }

                    destinationPointer += (int)buffer.Length + 1;
                }
            }
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
            return (length > 4 && PTStream.Contains(source, 0, new byte[] { 0x10 }));
        }
    }
}