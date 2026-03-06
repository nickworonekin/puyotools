/*
 * This implementation is derived from the .NET source for System.Formats.Tar.SubReadStream and System.Formats.Tar.SeekableSubReadStream.
 * https://source.dot.net/#System.Formats.Tar/System/Formats/Tar/SubReadStream.cs
 * https://source.dot.net/#System.Formats.Tar/System/Formats/Tar/SeekableSubReadStream.cs
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuyoTools.Core
{
    // Stream that allows wrapping a super stream and specify the lower and upper limits that can be read from it.
    // It is meant to be used when the super stream is unseekable.
    // Does not support writing.
    public class SubReadStream : Stream
    {
        protected readonly long _startInSuperStream;
        protected long _positionInSuperStream;
        protected readonly long _endInSuperStream;
        protected readonly Stream _superStream;
        protected bool _isDisposed;

        public SubReadStream(Stream superStream)
            : this(superStream, superStream.Position, superStream.Length - superStream.Position)
        {
        }

        public SubReadStream(Stream superStream, long maxLength)
            : this(superStream, superStream.Position, maxLength)
        {
        }

        public SubReadStream(Stream superStream, long startPosition, long maxLength)
        {
            if (!superStream.CanRead)
            {
                throw new ArgumentException("The stream does not support reading.", nameof(superStream));
            }
            _startInSuperStream = startPosition;
            _positionInSuperStream = startPosition;
            _endInSuperStream = startPosition + maxLength;
            _superStream = superStream;
            _isDisposed = false;
        }

        public override long Length
        {
            get
            {
                ThrowIfDisposed();
                return _endInSuperStream - _startInSuperStream;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIfDisposed();
                return _positionInSuperStream - _startInSuperStream;
            }
            set
            {
                ThrowIfDisposed();
                ThrowIfUnseekable();
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _positionInSuperStream = _startInSuperStream + value;
            }
        }

        public override bool CanRead => !_isDisposed;

        public override bool CanSeek => !_isDisposed && _superStream.CanSeek;

        public override bool CanWrite => false;

        private long Remaining => _endInSuperStream - _positionInSuperStream;

        private int LimitByRemaining(int bufferSize) => (int)long.Clamp(Remaining, 0, bufferSize);

        protected void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
        }

        private void ThrowIfUnseekable()
        {
            if (!CanSeek)
            {
                throw new NotSupportedException("The stream does not support seeking.");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> destination)
        {
            ThrowIfDisposed();
            VerifyPositionInSuperStream();

            destination = destination[..LimitByRemaining(destination.Length)];

            int ret = _superStream.Read(destination);

            _positionInSuperStream += ret;

            return ret;
        }

        public override int ReadByte()
        {
            byte b = default;
            return Read(new Span<byte>(ref b)) == 1 ? b : -1;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }
            ValidateBufferArguments(buffer, offset, count);
            return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled<int>(cancellationToken);
            }
            ThrowIfDisposed();
            VerifyPositionInSuperStream();
            return ReadAsyncCore(buffer, cancellationToken);
        }

        protected async ValueTask<int> ReadAsyncCore(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            buffer = buffer[..LimitByRemaining(buffer.Length)];

            int ret = await _superStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            _positionInSuperStream += ret;

            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();
            ThrowIfUnseekable();

            long newPositionInSuperStream = origin switch
            {
                SeekOrigin.Begin => _startInSuperStream + offset,
                SeekOrigin.Current => _positionInSuperStream + offset,
                SeekOrigin.End => _endInSuperStream + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            if (newPositionInSuperStream < _startInSuperStream)
            {
                throw new IOException("An attempt was made to move the position before the beginning of the stream.");
            }

            _positionInSuperStream = newPositionInSuperStream;

            return _positionInSuperStream - _startInSuperStream;
        }

        private void VerifyPositionInSuperStream()
        {
            if (!CanSeek)
            {
                return;
            }

            if (_positionInSuperStream != _superStream.Position)
            {
                // Since we can seek, if the stream had its position pointer moved externally,
                // we must bring it back to the last read location on this stream
                _superStream.Seek(_positionInSuperStream, SeekOrigin.Begin);
            }
        }

        public override void SetLength(long value) => throw new NotSupportedException("The stream does not support both writing and seeking.");

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("The stream does not support writing.");

        public override void Flush() { }

        public override Task FlushAsync(CancellationToken cancellationToken) =>
            cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) :
            Task.CompletedTask;

        // Close the stream for reading.  Note that this does NOT close the superStream (since
        // the substream is just 'a chunk' of the super-stream
        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
