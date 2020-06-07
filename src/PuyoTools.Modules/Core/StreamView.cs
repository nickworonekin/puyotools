using System;
using System.IO;

namespace PuyoTools.Modules
{
    /// <summary>
    /// Provides a read-only view of a portion of a stream.
    /// </summary>
    public class StreamView : Stream
    {
        private Stream baseStream; // The base stream
        private long streamStart; // The starting offset of the stream
        private long streamLength; // The length of the stream view

        private bool disposed = false; // Is this StreamView disposed?

        /// <summary>
        /// Initializes a new instance of the StreamView class for the specified stream. 
        /// The starting position of this StreamView is the current position of the specified stream.
        /// The length is the number of bytes between the current position and the end of the stream.
        /// </summary>
        /// <param name="source">The stream to be read.</param>
        public StreamView(Stream source) : this(source, (source.Length - source.Position)) { }

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
        /// Does nothing.
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return streamLength; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream. The position cannot be outside the bounds of the stream.
        /// </summary>
        public override long Position
        {
            get { return baseStream.Position - streamStart; }
            set
            {
                if (value < 0 || value > streamLength)
                {
                    throw new ArgumentException("offset");
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

            return baseStream.Read(array, offset, count);
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

            return baseStream.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream. The position cannot be outside the bounds of the stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                // Seek from the beginning of the stream
                case SeekOrigin.Begin:
                    if (offset < 0 || offset > streamLength)
                    {
                        throw new ArgumentException("offset");
                    }

                    return baseStream.Seek(streamStart + offset, SeekOrigin.Begin);

                // Seek from the current position in the stream
                case SeekOrigin.Current:
                    if (baseStream.Position + offset < streamStart || baseStream.Position + offset > streamStart + streamLength)
                    {
                        throw new ArgumentException("offset");
                    }

                    return baseStream.Seek(offset, SeekOrigin.Current);

                // Seek from the end of the stream
                case SeekOrigin.End:
                    if (offset < -streamLength || offset > 0)
                    {
                        throw new ArgumentException("offset"); 
                    }

                    return baseStream.Seek(streamStart + streamLength + offset, SeekOrigin.Begin);
            }

            return baseStream.Position - streamStart;
        }

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        /// <param name="value">Not used</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        /// <param name="buffer">Not used</param>
        /// <param name="offset">Not used</param>
        /// <param name="count">Not used</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        /// <param name="value">Not used</param>
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Dispose the StreamView.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                base.Dispose(disposing);

                // Since we don't want to kill the baseStream, there's no need for disposing.
                baseStream = null;
                streamStart = -1;
                streamLength = -1;

                disposed = true;
            }
        }
    }
}