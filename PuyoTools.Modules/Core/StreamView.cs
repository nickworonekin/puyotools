using System;
using System.IO;

namespace PuyoTools.Modules
{
    public class StreamView : Stream
    {
        private Stream baseStream; // The base stream
        private long streamStart; // The starting offset of the stream
        private long streamLength; // The length of the stream view

        /// <summary>
        /// Initializes a new instance of the StreamView class for the specified stream. 
        /// The starting position of this StreamView is the current position of the specified stream.
        /// </summary>
        /// <param name="source">The stream to be read.</param>
        /// <param name="length">Number of bytes that can be accessed by this StreamView.</param>
        public StreamView(Stream source, long length)
        {
            if (source.Position + length > source.Length)
            {
                baseStream = null;
                streamStart = -1;
                streamLength = -1;

                throw new ArgumentOutOfRangeException();
            }

            baseStream = source;
            streamStart = source.Position;
            streamLength = length;
        }

        /// <summary>
        /// Initializes a new instance of the StreamView class for the specified stream at the specified offset.
        /// </summary>
        /// <param name="source">The stream to be read.</param>
        /// <param name="offset">The starting position of this StreamView.</param>
        /// <param name="length">Number of bytes that can be accessed by this StreamView.</param>
        public StreamView(Stream source, long offset, long length)
        {
            if (offset + length > source.Length)
            {
                baseStream = null;
                streamStart = -1;
                streamLength = -1;

                throw new ArgumentOutOfRangeException();
            }

            source.Position = offset;

            baseStream = source;
            streamStart = source.Position;
            streamLength = length;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return baseStream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return baseStream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing. This will always return false.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device. 
        /// This will always throw a NotSupportedException.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return streamLength; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get { return baseStream.Position - streamStart; }
            set
            {
                if (value < 0 || value > streamLength)
                {
                    throw new ArgumentOutOfRangeException();
                }

                baseStream.Position = streamStart + value;
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="array">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] array, int offset, int count)
        {
            if (baseStream.Position >= streamStart + streamLength)
            {
                count = 0;
            }
            else if (baseStream.Position + count > streamStart + streamLength)
            {
                count = (int)(streamStart + streamLength - baseStream.Position);
            }

            return Read(array, offset, count);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            if (baseStream.Position >= streamStart + streamLength)
            {
                return -1;
            }

            return base.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if ((origin == SeekOrigin.Begin && (offset < 0 || offset > streamLength)) ||
                (origin == SeekOrigin.Current && (baseStream.Position + offset < streamStart || baseStream.Position + offset > streamStart + streamLength)) ||
                (origin == SeekOrigin.End && (streamLength + offset < 0 || streamLength + offset > streamLength)))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (origin == SeekOrigin.Begin)
            {
                return baseStream.Seek(streamStart + offset, origin);
            }
            else if (origin == SeekOrigin.End)
            {
                return baseStream.Seek(streamStart + streamLength + offset, origin);
            }

            return Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream. This will always throw a NotSupportedException.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written. 
        /// This will always throw a NotSupportedException.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte. 
        /// This will always throw a NotSupportedException.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
    }
}