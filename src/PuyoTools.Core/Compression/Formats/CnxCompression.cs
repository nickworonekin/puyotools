using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PuyoTools.Core.Compression
{
    public class CnxCompression : CompressionBase
    {
        /*
         * CNX decompression support by drx (Luke Zapart)
         * <thedrx@gmail.com>
         */
        private const int SHIFT = 5;
        private const int LENGTH = 0x1F;

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
            int sourceLength = PTStream.ReadInt32BE(source) + 16;
            int destinationLength = PTStream.ReadInt32BE(source);

            // Set the source, destination, and buffer pointers
            int sourcePointer = 0x10;
            int destinationPointer = 0x0;
            int bufferPointer = 0x0;

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

                            matchDistance = (matchPair >> SHIFT) + 1;
                            matchLength = (matchPair & LENGTH) + 4;

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
            var minLength = 4;
            var maxLength = LENGTH + 4;
            dictionary.SetMinMatchAmount(minLength);
            dictionary.SetMaxMatchAmount(maxLength);

            using var writer = destination.AsBinaryWriter();
            using var buffer = new MemoryStream(256); // Will never contain more than 256 bytes
            using var bufferWriter = new BinaryWriter(buffer);

            try
            {

                // Write out the header
                // Magic code
                writer.Write(magicCode);

                // Get the file extension, and adjust as necessary to get it as 3 bytes
                var fileExtension = "bin";
                writer.WriteString(fileExtension, 3);

                writer.WriteByte(0x10);
                writer.WriteInt32BigEndian(0); // Compressed length (will be filled in later)
                writer.WriteInt32BigEndian(sourceLength); // Decompressed length

                // Set the initial match
                int[] match = new[] { 0, 0 };

                int code = 0;

                // Start compression
                while (sourcePointer <= sourceLength)
                {
                    byte flag = 0;
                   
                    for (int i = 0; i < 4; i++)
                    {
                        // Check to see if we reached the end of the file
                        if (sourcePointer >= sourceLength)
                        {
                            if (i == 0)
                            {
                                writer.WriteByte(0xFC);
                                writer.WriteByte(0x0);
                            }
                            else
                            {
                                flag |= (byte)(0 << (i * 2));
                                //i++;
                                for (i++; i < 4; i++)
                                    flag |= (byte)(0x3 << (i * 2));
                                bufferWriter.WriteByte(0x0);
                            }
                            sourcePointer++;
                            break;
                        }
                        else
                        {


                            byte matchLength = 0;

                            match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength-1);

                            //determine unfound runlength
                            if (match[1] == 0)
                            {
                                while (
                                    ((dictionary.Search(sourceArray, (uint)(sourcePointer + matchLength), (uint)sourceLength-1 ))[1] == 0)
                                    && sourcePointer + matchLength < sourceLength
                                    && matchLength < 255
                                    && sourcePointer > 0
                                    )

                                {
                                    if (sourcePointer + matchLength >= LENGTH)
                                    {
                                        dictionary.AddEntry(sourceArray, sourcePointer + matchLength);
                                    }
                                    matchLength++;
                                }
                                if (sourcePointer == 0)
                                {
                                    dictionary.AddEntryRange(sourceArray, sourcePointer + matchLength,LENGTH);
                                }
                                // Determine the type of flag to write based on the length of the match
                                if (matchLength > 1 && sourcePointer > 0)
                                {
                                    code = 3;
                                }
                                else
                                {
                                    code = 1;
                                    matchLength = 1;
                                }

                            }
                            else
                            {

                                // Search for a match
                                match = dictionary.Search(sourceArray, (uint)(sourcePointer + matchLength), (uint)sourceLength-1);

                                matchLength = (byte)match[1];
                                if (sourcePointer >= LENGTH)
                                {
                                    dictionary.AddEntryRange(sourceArray, sourcePointer, matchLength);
                                    
                                }
                                else
                                {
                                    var diff = LENGTH - sourcePointer;
                                    if((matchLength - diff) > 0)
                                     dictionary.AddEntryRange(sourceArray, sourcePointer + diff, matchLength - diff);
                                }
                                

                                code = 2;
                            }


                            if (((writer.BaseStream.Length & 0x7FF) + bufferWriter.BaseStream.Length + (code == 2 ? 2 : matchLength)) >= 0x7f7)
                            {
                                var remainder = (byte)(0x800 - ((writer.BaseStream.Length & 0x7FF) + bufferWriter.BaseStream.Length));
                            }
                            //current stream + flag + buffer + code3 count + matchlength
                            var len = ((writer.BaseStream.Length & 0x7FF) + 1 + bufferWriter.BaseStream.Length + (code == 3 ? 1 : 0) + (code == 2 ? 2 : matchLength));
                            if (len >= 0x7Fe + ((code == 2) ? 1 : 0))
                            {
                                if (code == 3)
                                {
                                    len -= matchLength;

                                    var newMatchLength = (0x7FE - len);
                                    if (newMatchLength >= 0)
                                    {
                                        if (newMatchLength > 0)
                                        {
                                            var leftover = (int)(matchLength - newMatchLength);
                                            dictionary.RemoveEntryRange(sourceArray, sourcePointer + matchLength - 1, leftover);
                                            matchLength = (byte)newMatchLength;
                                            len += matchLength;
                                            if (matchLength == 1)
                                            {
                                                code = 1;
                                                len--;//we need to take out the code3 count.
                                            }
                                            else
                                            {
                                                bufferWriter.WriteByte(matchLength);
                                            }
                                            flag |= (byte)(code << (i * 2));


                                            bufferWriter.Write(sourceArray, sourcePointer, matchLength);
                                            code = 0;
                                            i++;

                                        }
                                        else
                                        {
                                            len -= 1;//we need to take out the code3 count.
                                        }

                                    }
                                    else
                                    {
                                        len -= 1;//we need to take out the code3 count.
                                    }

                                }
                                else if (code == 2)
                                {
                                    len -= 2;
                                }
                                else
                                {
                                    dictionary.RemoveEntry(sourceArray, sourcePointer + 1 - 1);
                                    code = 0;
                                    matchLength = 0;
                                    len--;
                                }

                                var remainder = (byte)(0x800 - (len + 1));
                                if (i < 4)
                                {
                                    flag |= (byte)(0 << (i * 2));
                                    for (i++; i < 4; i++)
                                        flag |= (byte)(3 << (i * 2));
                                }
                                else
                                {
                                    //condition where we need to add a new flag, but we are beyond the 4 length limit
                                    bufferWriter.WriteByte(0xFC); //new flag with code 2
                                    remainder -= 1;//take a byte away from the remainder to allocate for the flag
                                }
                                bufferWriter.WriteByte(remainder);
                                for (var r = 0; r < remainder; r++)
                                    bufferWriter.WriteByte(0xCD);
                                writer.WriteByte(flag);
                                buffer.WriteTo(destination);
                                buffer.SetLength(0);
                                flag = 0;
                                i = 0;
                            }

                            if (code == 2) // There is a match
                            {
                                flag |= (byte)(2 << (i * 2));
                                bufferWriter.WriteUInt16BigEndian((ushort)((((match[0] - 1) & 0x7FF) << SHIFT) | ((match[1] - minLength) & LENGTH)));
                            }
                            else if (code == 0)
                            {
                                i -= 1; //This means there was nothing new to process, so run it again.
                            }
                            else // There is not a match
                            {
                                flag |= (byte)(code << (i * 2));
                                if (code == 3)
                                {
                                    bufferWriter.WriteByte(matchLength);
                                }
                                bufferWriter.Write(sourceArray, sourcePointer, matchLength);
                            }

                            sourcePointer += matchLength;
                        }


                    }
                    if (buffer.Length > 0)
                    {
                        // Flush the buffer and write it to the destination stream
                        writer.WriteByte(flag);
                        buffer.WriteTo(destination);
                        buffer.SetLength(0);
                    }

                }

                // Write the final flag of 0
                writer.WriteByte(0);

                // Go back to the beginning of the file and write out the compressed length
                var destinationLength = (int)(destination.Position - destinationStartPosition);
                writer.At(destinationStartPosition + 8, x => x.WriteInt32BigEndian(destinationLength - 16));
            }
            catch (Exception ex)
            {
                throw;
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