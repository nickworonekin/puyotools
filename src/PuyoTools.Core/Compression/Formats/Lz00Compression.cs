using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class Lz00Compression : CompressionBase
    {
        /*
         * LZ00 decompression support by QPjDDYwQLI thanks to the author of ps2dis
         */

        private static readonly byte[] magicCode = { (byte)'L', (byte)'Z', (byte)'0', (byte)'0' };

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /*public override void Decompress(Stream source, Stream destination)
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
        }*/

        public override void Decompress(Stream source, Stream destination)
        {
            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                source.Position += 4;

                // Get the source length, destination length, and encryption key
                int sourceLength = reader.ReadInt32();

                source.Position += 40;

                int destinationLength = reader.ReadInt32();
                uint key = reader.ReadUInt32();

                source.Position += 8;

                // Set the source, destination, and buffer pointers
                int sourcePointer = 0x40;
                int destinationPointer = 0x0;
                int bufferPointer = 0xFEE;

                // Initalize the buffer
                byte[] buffer = new byte[0x1000];

                // Start decompression
                while (sourcePointer < sourceLength)
                {
                    byte flag = Transform(reader.ReadByte(), ref key);
                    sourcePointer++;

                    for (int i = 0; i < 8; i++)
                    {
                        if ((flag & 0x1) != 0) // Not compressed
                        {
                            byte value = Transform(reader.ReadByte(), ref key);
                            sourcePointer++;

                            destination.WriteByte(value);
                            destinationPointer++;

                            buffer[bufferPointer] = value;
                            bufferPointer = (bufferPointer + 1) & 0xFFF;
                        }
                        else // Compressed
                        {
                            ushort matchPair = Transform(reader.ReadUInt16(), ref key);
                            sourcePointer += 2;

                            int matchOffset = ((matchPair >> 4) & 0xF00) | (matchPair & 0xFF);
                            int matchLength = ((matchPair >> 8) & 0xF) + 3;

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
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Compress(Stream source, Stream destination)
        {
            var destinationStartPosition = destination.Position;

            // Get the source length, and read it in to an array
            int sourceLength = (int)(source.Length - source.Position);
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

            // Get the encryption key
            // Since the original files appear to use the time the file was compressed (as Unix time), we will do the same.
            uint key = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Set the source and destination pointers
            int sourcePointer = 0x0;

            // Initalize the LZ dictionary
            LzBufferDictionary dictionary = new LzBufferDictionary();
            dictionary.SetBufferSize(0x1000);
            dictionary.SetBufferStart(0xFEE);
            dictionary.SetMaxMatchAmount(0xF + 3);

            using (var writer = destination.AsBinaryWriter())
            using (var buffer = new MemoryStream(16)) // Will never contain more than 16 bytes
            using (var bufferWriter = new BinaryWriter(buffer))
            {
                // Write out the header
                // Magic code
                writer.Write(magicCode);
                writer.WriteInt32(0); // Compressed length (will be filled in later)
                writer.WriteInt32(0);
                writer.WriteInt32(0);

                // Filename (or null bytes)
                if (destination is FileStream fs)
                {
                    writer.WriteString(Path.GetFileName(fs.Name), 32, EncodingExtensions.ShiftJIS);
                }
                else
                {
                    writer.Write(new byte[32]); // Elements in array default to 0
                }

                writer.WriteInt32(sourceLength); // Decompressed length
                writer.WriteUInt32(key); // Encryption key
                writer.WriteInt32(0);
                writer.WriteInt32(0);

                // Start compression
                while (sourcePointer < sourceLength)
                {
                    byte flag = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        // Search for a match
                        int[] match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);

                        if (match[1] > 0) // There is a match
                        {
                            bufferWriter.WriteUInt16((ushort)((match[0] & 0xFF) | (match[0] & 0xF00) << 4 | ((match[1] - 3) & 0xF) << 8));

                            dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);

                            sourcePointer += match[1];
                        }
                        else // There is not a match
                        {
                            flag |= (byte)(1 << i);

                            bufferWriter.WriteByte(sourceArray[sourcePointer]);

                            dictionary.AddEntry(sourceArray, sourcePointer);

                            sourcePointer++;
                        }

                        // Check to see if we reached the end of the file
                        if (sourcePointer >= sourceLength)
                            break;
                    }

                    // Flush the buffer and write it to the destination stream
                    writer.WriteByte(Transform(flag, ref key));

                    // Loop through the buffer and encrypt the contents before writing it to the destination
                    var backingBuffer = buffer.GetBuffer();
                    for (var i = 0; i < buffer.Length; i++)
                    {
                        backingBuffer[i] = Transform(backingBuffer[i], ref key);
                    }

                    buffer.WriteTo(destination);
                    buffer.SetLength(0);
                }

                // Go back to the beginning of the file and write out the compressed length
                var destinationLength = (int)(destination.Position - destinationStartPosition);
                writer.At(destinationStartPosition + 4, x => x.WriteInt32(destinationLength));
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
                return remainingLength > 64
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)
                    && reader.At(startPosition + 4, x => x.ReadInt32()) == remainingLength;
            }
        }

        private byte Transform(byte value, ref uint key)
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

        private ushort Transform(ushort value, ref uint key)
        {
            return (ushort)(Transform((byte)(value & 0xFF), ref key) | (Transform((byte)(value >> 8), ref key) << 8));
        }
    }
}