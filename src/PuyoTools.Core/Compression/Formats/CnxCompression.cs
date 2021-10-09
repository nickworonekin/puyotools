using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PuyoTools.Core.Compression
{
    public class CnxCompression : CompressionBase
    {
        /*
         * CNX decompression support by drx (Luke Zapart)
         * <thedrx@gmail.com>
         */

        private static readonly byte[] magicCode = { (byte)'C', (byte)'N', (byte)'X', 0x2 };

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 8;

            // Get the source length and destination length
            int sourceLength      = PTStream.ReadInt32BE(source) + 16;
            int destinationLength = PTStream.ReadInt32BE(source);

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x10;
            int destinationPointer = 0x0;
            int bufferPointer      = 0x0;

            // Initalize the buffer
            byte[] buffer = new byte[0x800];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = PTStream.ReadByte(source);
                sourcePointer++;

                // If all bits are 0, this is the end of the compressed data.
                if (flag == 0)
                {
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    byte value;
                    ushort matchPair;
                    int matchDistance, matchLength;

                    switch (flag & 0x3)
                    {
                        // Jump to the next 0x800 boundary
                        case 0:
                            value = PTStream.ReadByte(source);

                            sourcePointer += value + 1;
                            source.Position += value;

                            i = 3;
                            break;

                        // Not compressed, single byte
                        case 1:
                            value = PTStream.ReadByte(source);
                            sourcePointer++;

                            destination.WriteByte(value);
                            destinationPointer++;

                            buffer[bufferPointer] = value;
                            bufferPointer = (bufferPointer + 1) & 0x7FF;
                            break;

                        // Compressed
                        case 2:
                            matchPair = PTStream.ReadUInt16BE(source);
                            sourcePointer += 2;

                            matchDistance = (matchPair >> 5) + 1;
                            matchLength = (matchPair & 0x1F) + 4;

                            for (int j = 0; j < matchLength; j++)
                            {
                                destination.WriteByte(buffer[(bufferPointer - matchDistance) & 0x7FF]);
                                destinationPointer++;

                                buffer[bufferPointer] = buffer[(bufferPointer - matchDistance) & 0x7FF];
                                bufferPointer = (bufferPointer + 1) & 0x7FF;
                            }
                            break;

                        // Not compressed, multiple bytes
                        case 3:
                            matchLength = PTStream.ReadByte(source);
                            sourcePointer++;

                            for (int j = 0; j < matchLength; j++)
                            {
                                value = PTStream.ReadByte(source);
                                sourcePointer++;

                                destination.WriteByte(value);
                                destinationPointer++;

                                buffer[bufferPointer] = value;
                                bufferPointer = (bufferPointer + 1) & 0x7FF;
                            }
                            break;
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

                    flag >>= 2;
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
            var destinationStartPosition = destination.Position;

            // Get the source length
            int sourceLength = (int)(source.Length - source.Position);

            // Read the source data into an array
            byte[] sourceArray = new byte[sourceLength];
            var totalSourceBytesRead = 0;
            int sourceBytesRead;
            do
            {
                sourceBytesRead = source.Read(sourceArray, totalSourceBytesRead, sourceLength - totalSourceBytesRead);
                if (sourceBytesRead == 0)
                {
                    throw new IOException($"Unable to read all bytes in {nameof(source)}");
                }
                totalSourceBytesRead += sourceBytesRead;
            }
            while (totalSourceBytesRead < sourceLength);

            // Set the source and destination pointers
            int sourcePointer = 0x0;

            // Initalize the LZ dictionary
            LzWindowDictionary dictionary = new LzWindowDictionary();
            dictionary.SetWindowSize(0x800);
            dictionary.SetMinMatchAmount(4);
            dictionary.SetMaxMatchAmount(0x1F + 4);

            using (var writer = destination.AsBinaryWriter())
            using (var buffer = new MemoryStream(256)) // Will never contain more than 256 bytes
            using (var bufferWriter = new BinaryWriter(buffer))
            {
                // Write out the header
                // Magic code
                writer.Write(magicCode);

                // Get the file extension, and adjust as necessary to get it as 3 bytes
                var fileExtension = source is FileStream fs
                    ? Path.GetExtension(fs.Name)
                    : string.Empty;
                writer.WriteString(fileExtension, 3);

                writer.WriteByte(0x10);
                writer.WriteInt32BigEndian(0); // Compressed length (will be filled in later)
                writer.WriteInt32BigEndian(sourceLength); // Decompressed length

                // Set the initial match
                int[] match = new[] { 0, 0 };

                // Start compression
                while (sourcePointer < sourceLength)
                {
                    byte flag = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        if (match[1] > 0) // There is a match
                        {
                            flag |= (byte)(2 << (i * 2));

                            bufferWriter.WriteUInt16BigEndian((ushort)((((match[0] - 1) & 0x7FF) << 5) | ((match[1] - 4) & 0x1F)));

                            dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);

                            sourcePointer += match[1];

                            // Search for a match
                            if (sourcePointer < sourceLength)
                            {
                                match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);
                            }
                        }
                        else // There is not a match
                        {
                            dictionary.AddEntry(sourceArray, sourcePointer);

                            byte matchLength = 1;

                            // Search for a match
                            while (sourcePointer + matchLength < sourceLength
                                && matchLength < 255
                                && (match = dictionary.Search(sourceArray, (uint)(sourcePointer + matchLength), (uint)sourceLength))[1] == 0)
                            {
                                dictionary.AddEntry(sourceArray, sourcePointer + matchLength);

                                matchLength++;
                            }

                            // Determine the type of flag to write based on the length of the match
                            if (matchLength > 1)
                            {
                                flag |= (byte)(3 << (i * 2));
                                bufferWriter.WriteByte(matchLength);
                            }
                            else
                            {
                                flag |= (byte)(1 << (i * 2));
                            }

                            bufferWriter.Write(sourceArray, sourcePointer, matchLength);

                            sourcePointer += matchLength;
                        }

                        // Check to see if we reached the end of the file
                        if (sourcePointer >= sourceLength)
                        {
                            break;
                        }
                    }

                    // Check to see if we reached the end of the file
                    if (sourcePointer >= sourceLength && ((flag >> 6) & 0x3) == 0)
                    {
                        // Write out values for this flag
                        bufferWriter.WriteByte(0);
                    }

                    // Flush the buffer and write it to the destination stream
                    writer.WriteByte(flag);

                    buffer.WriteTo(destination);
                    buffer.SetLength(0);
                }

                // Write the final flag of 0
                writer.WriteByte(0);

                // Go back to the beginning of the file and write out the compressed length
                var destinationLength = (int)(destination.Position - destinationStartPosition);
                writer.At(destinationStartPosition + 8, x => x.WriteInt32BigEndian(destinationLength - 16));
            }
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = source.AsBinaryReader())
            {
                return remainingLength > 16
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)
                    && reader.At(startPosition + 8, x => x.ReadInt32BigEndian()) == remainingLength - 16;
            }
        }
    }
}