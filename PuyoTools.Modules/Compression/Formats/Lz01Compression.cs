using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class Lz01Compression : CompressionBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "LZ01"; }
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

            // Get the source length and the destination length
            int sourceLength      = PTStream.ReadInt32(source);
            int destinationLength = PTStream.ReadInt32(source);

            source.Position += 4;

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x10;
            int destinationPointer = 0x0;
            int bufferPointer      = 0xFEE;

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = PTStream.ReadByte(source);
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x1) != 0) // Not compressed
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

            // Set the source and destination pointers
            int sourcePointer = 0x0;
            int destinationPointer = 0x10;

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
            destination.WriteByte((byte)'1');
            PTStream.WriteInt32(destination, 0); // Compressed length (will be filled in later)
            PTStream.WriteInt32(destination, sourceLength); // Decompressed length
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
            return (length > 16 &&
                PTStream.Contains(source, 0, new byte[] { (byte)'L', (byte)'Z', (byte)'0', (byte)'1' }) &&
                PTStream.ReadInt32At(source, source.Position + 4) == length);
        }
    }
}