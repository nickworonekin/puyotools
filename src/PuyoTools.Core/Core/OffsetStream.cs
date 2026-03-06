using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuyoTools.Core
{
    /// <summary>
    /// A stream that starts at a specific position within the base stream.
    /// </summary>
    public class OffsetStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _startInBaseStream;
        private long _positionInBaseStream;
        private bool _isDisposed;

        public OffsetStream(Stream baseStream)
            : this(baseStream, baseStream.Position)
        {
        }

        public OffsetStream(Stream baseStream, long startPosition)
        {
            ArgumentNullException.ThrowIfNull(baseStream);
            ArgumentOutOfRangeException.ThrowIfNegative(startPosition);

            _baseStream = baseStream;
            _startInBaseStream = startPosition;
            _positionInBaseStream = startPosition;
            _isDisposed = false;
        }

        public override bool CanRead => !_isDisposed && _baseStream.CanRead;

        public override bool CanSeek => !_isDisposed && _baseStream.CanSeek;

        public override bool CanWrite => !_isDisposed && _baseStream.CanWrite;

        public override long Length
        {
            get
            {
                ThrowIfDisposed();
                ThrowIfUnseekable();

                return _baseStream.Length - _startInBaseStream;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIfDisposed();
                ThrowIfUnseekable();

                return _positionInBaseStream - _startInBaseStream;
            }
            set
            {
                ThrowIfDisposed();
                ThrowIfUnseekable();
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                _positionInBaseStream = _startInBaseStream + value;
                _baseStream.Position = _positionInBaseStream;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            ThrowIfUnreadable();
            VerifyPositionInBaseStream();

            int ret = _baseStream.Read(buffer, offset, count);

            _positionInBaseStream += ret;
            return ret;
        }

        public override int Read(Span<byte> buffer)
        {
            ThrowIfDisposed();
            ThrowIfUnreadable();
            VerifyPositionInBaseStream();

            int ret = _baseStream.Read(buffer);

            _positionInBaseStream += ret;
            return ret;
        }

        public override int ReadByte()
        {
            ThrowIfDisposed();
            ThrowIfUnreadable();
            VerifyPositionInBaseStream();

            int ret = _baseStream.ReadByte();

            if (ret != -1)
            {
                _positionInBaseStream++;
            }
            return ret;
        }

        public async override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfUnreadable();
            VerifyPositionInBaseStream();

            int ret = await _baseStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);

            _positionInBaseStream += ret;
            return ret;
        }

        public async override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfUnreadable();
            VerifyPositionInBaseStream();

            int ret = await _baseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            _positionInBaseStream += ret;
            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();
            ThrowIfUnseekable();

            long newPositionInSuperStream = origin switch
            {
                SeekOrigin.Begin => _startInBaseStream + offset,
                SeekOrigin.Current => _positionInBaseStream + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            if (newPositionInSuperStream < _startInBaseStream)
            {
                throw new IOException("An attempt was made to move the position before the beginning of the stream.");
            }

            _positionInBaseStream = newPositionInSuperStream;
            _baseStream.Position = _positionInBaseStream;

            return _positionInBaseStream - _startInBaseStream;
        }

        public override void SetLength(long value)
        {
            ThrowIfDisposed();
            ThrowIfUnseekable();
            ThrowIfUnwriteable();

            _baseStream.SetLength(_startInBaseStream + value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            ThrowIfUnwriteable();
            VerifyPositionInBaseStream();

            _baseStream.Write(buffer, offset, count);

            _positionInBaseStream += count;
        }

        public override void Write(ReadOnlySpan<byte> source)
        {
            ThrowIfDisposed();
            ThrowIfUnwriteable();
            VerifyPositionInBaseStream();

            _baseStream.Write(source);

            _positionInBaseStream += source.Length;
        }

        public override void WriteByte(byte value)
        {
            ThrowIfDisposed();
            ThrowIfUnwriteable();
            VerifyPositionInBaseStream();

            _baseStream.WriteByte(value);

            _positionInBaseStream++;
        }

        public async override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfUnwriteable();
            VerifyPositionInBaseStream();

            await _baseStream.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);

            _positionInBaseStream += count;
        }

        public async override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfUnwriteable();
            VerifyPositionInBaseStream();

            await _baseStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

            _positionInBaseStream += buffer.Length;
        }

        public override void Flush()
        {
            ThrowIfDisposed();

            _baseStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return _baseStream.FlushAsync(cancellationToken);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
        }

        private void ThrowIfUnreadable()
        {
            if (!CanRead)
            {
                throw new NotSupportedException("Stream does not support reading.");
            }
        }

        private void ThrowIfUnseekable()
        {
            if (!CanSeek)
            {
                throw new NotSupportedException("Stream does not support seeking.");
            }
        }

        private void ThrowIfUnwriteable()
        {
            if (!CanWrite)
            {
                throw new NotSupportedException("Stream does not support writing.");
            }
        }

        private void VerifyPositionInBaseStream()
        {
            // If this stream doesn't support seeking, don't attempt to verify the position
            // within the base stream.
            if (!CanSeek)
            {
                return;
            }

            if (_positionInBaseStream != _baseStream.Position)
            {
                // Since we can seek, if the stream had its position pointer moved externally,
                // we must bring it back to the last read location on this stream
                _baseStream.Seek(_positionInBaseStream, SeekOrigin.Begin);
            }
        }

        // Close the stream for reading and writing. Note that this does not close the base stream.
        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;
            base.Dispose(disposing);
        }

        public async override ValueTask DisposeAsync()
        {
            _isDisposed = true;
            await base.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}
