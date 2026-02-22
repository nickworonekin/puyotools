/*
 * This implementation is derived from the .NET source for System.Formats.Tar.SubReadStream.
 * https://source.dot.net/#System.Formats.Tar/System/Formats/Tar/SubReadStream.cs
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PuyoTools.Core
{
    // Stream that allows wrapping a super stream and specify the lower and upper limits that can be read from it.
    // It is meant to be used when the super stream is unseekable.
    // Does not support writing.
    public class SubReadStream : Stream
    {
        protected bool _hasReachedEnd;
        protected readonly long _startInSuperStream;
        protected long _positionInSuperStream;
        protected readonly long _endInSuperStream;
        protected readonly Stream _superStream;
        protected bool _isDisposed;

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
            _hasReachedEnd = false;
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
                throw new InvalidOperationException("The stream does not support seeking.");
            }
        }

        public override bool CanRead => !_isDisposed;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        internal bool HasReachedEnd
        {
            get
            {
                if (!_hasReachedEnd && _positionInSuperStream > _endInSuperStream)
                {
                    _hasReachedEnd = true;
                }
                return _hasReachedEnd;
            }
            set
            {
                if (value) // Don't allow revert to false
                {
                    _hasReachedEnd = true;
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
        }

        private void ThrowIfBeyondEndOfStream()
        {
            if (HasReachedEnd)
            {
                throw new EndOfStreamException();
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
            ThrowIfBeyondEndOfStream();

            // parameter validation sent to _superStream.Read
            int origCount = destination.Length;
            int count = destination.Length;

            if (_positionInSuperStream + count > _endInSuperStream)
            {
                count = (int)(_endInSuperStream - _positionInSuperStream);
            }

            Debug.Assert(count >= 0);
            Debug.Assert(count <= origCount);

            int ret = _superStream.Read(destination.Slice(0, count));

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
            ThrowIfBeyondEndOfStream();
            return ReadAsyncCore(buffer, cancellationToken);
        }

        protected async ValueTask<int> ReadAsyncCore(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            Debug.Assert(!_hasReachedEnd);

            cancellationToken.ThrowIfCancellationRequested();

            if (_positionInSuperStream > _endInSuperStream - buffer.Length)
            {
                buffer = buffer.Slice(0, (int)(_endInSuperStream - _positionInSuperStream));
            }

            int ret = await _superStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            _positionInSuperStream += ret;
            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException("The stream does not support seeking.");

        public override void SetLength(long value) => throw new NotSupportedException("The stream does not support seeking.");

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
