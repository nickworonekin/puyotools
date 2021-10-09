using System;
using System.IO;

namespace PuyoTools.Core
{
    /// <summary>
    /// Provides a read-only view over a portion of a stream.
    /// </summary>
    public class StreamView : Stream
    {
        private bool disposed;
        private readonly Stream baseStream;
        private readonly long startInBaseStream;
        private readonly long endInBaseStream;
        private long positionInBaseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamView"/> class based on the specified stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public StreamView(Stream stream)
            : this(stream, stream.Length - stream.Position)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamView"/> class based on the specified region of a stream starting at its current position.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="length">The length of the stream in bytes.</param>
        public StreamView(Stream stream, long length)
            : this(stream, stream.Position, length)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamView"/> class based on the specified region of a stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="offset">The offset into the input stream at which the stream begins.</param>
        /// <param name="length">The length of the stream in bytes.</param>
        public StreamView(Stream stream, long offset, long length)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (length < 0 || offset + length > stream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            baseStream = stream;
            startInBaseStream = offset;
            endInBaseStream = offset + length;
            positionInBaseStream = startInBaseStream;

            disposed = false;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => !disposed && baseStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => !disposed && baseStream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the stream supports writing.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// Gets the length of the stream in bytes.
        /// </summary>
        public override long Length
        {
            get
            {
                ThrowIfDisposed();
                
                return endInBaseStream - startInBaseStream;
            }
        }

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        public override long Position
        {
            get
            {
                ThrowIfDisposed();

                return positionInBaseStream - startInBaseStream;
            }
            set
            {
                ThrowIfDisposed();

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                positionInBaseStream = startInBaseStream + value;
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        /// <summary>
        /// This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        public override void Flush()
        {
            ThrowIfDisposed();
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a byte from the current stream.
        /// </summary>
        /// <returns>The byte cast to a <see cref="int"/>, or -1 if the end of the stream has been reached.</returns>
        public override int ReadByte()
        {
            ThrowIfDisposed();

            if (positionInBaseStream <= endInBaseStream && positionInBaseStream != baseStream.Position)
            {
                baseStream.Seek(positionInBaseStream, SeekOrigin.Begin);
            }

            if (positionInBaseStream >= endInBaseStream)
            {
                return -1;
            }

            positionInBaseStream++;
            return baseStream.ReadByte();
        }

        /// <summary>
        /// Reads a block of bytes from the current stream and writes the data to a buffer.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the characters read from the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing data from the current stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes written into the buffer. This can be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached before any bytes are read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();

            if (positionInBaseStream <= endInBaseStream && positionInBaseStream != baseStream.Position)
            {
                baseStream.Seek(positionInBaseStream, SeekOrigin.Begin);
            }

            if (positionInBaseStream >= endInBaseStream)
            {
                return 0;
            }
            if (positionInBaseStream + count > endInBaseStream)
            {
                count = (int)(endInBaseStream - positionInBaseStream);
            }

            var bytesRead = baseStream.Read(buffer, offset, count);
            positionInBaseStream += bytesRead;

            return bytesRead;
        }

        /// <summary>
        /// Sets the position within the current stream to the specified value.
        /// </summary>
        /// <param name="offset">The new position within the stream. This is relative to the <paramref name="origin"/> parameter, and can be positive or negative.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/>, which acts as the seek reference point.</param>
        /// <returns>The new position within the stream, calculated by combining the initial reference point and the offset.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();

            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        if (offset < 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(offset));
                        }

                        positionInBaseStream = startInBaseStream + offset;
                    }
                    break;

                case SeekOrigin.Current:
                    {
                        if (positionInBaseStream + offset < startInBaseStream)
                        {
                            throw new ArgumentOutOfRangeException(nameof(offset));
                        }

                        positionInBaseStream += offset;
                    }
                    break;

                case SeekOrigin.End:
                    {
                        if (endInBaseStream + offset < startInBaseStream)
                        {
                            throw new ArgumentOutOfRangeException(nameof(offset));
                        }

                        positionInBaseStream = endInBaseStream + offset;
                    }
                    break;

                default:
                    throw new ArgumentException(nameof(origin));
            }

            return positionInBaseStream - startInBaseStream;
        }

        /// <summary>
        /// This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        public override void SetLength(long value)
        {
            ThrowIfDisposed();
            throw new NotSupportedException();
        }

        /// <summary>
        /// This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            throw new NotSupportedException();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="StreamView"/> class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
