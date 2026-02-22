using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core
{
    /// <summary>
    /// Provides a lazy-loaded read-only <see cref="FileStream"/>, where the underlying file handle is not opened until a read operation occurs.
    /// </summary>
    public class LazyFileReadStream : Stream
    {
        private readonly string _sourcePath;

        private FileStream? _sourceStream;
        private long _position = 0;
        private bool _isDisposed = false;

        public LazyFileReadStream(string path)
        {
            _sourcePath = Path.GetFullPath(path);
        }

        public override bool CanRead => !_isDisposed;

        public override bool CanSeek => !_isDisposed;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                ThrowIfDisposed();

                if (_sourceStream is null)
                {
                    return new FileInfo(_sourcePath).Length;
                }

                return _sourceStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIfDisposed();

                if (_sourceStream is null)
                {
                    return _position;
                }

                return _sourceStream.Position;
            }
            set
            {
                ThrowIfDisposed();

                if (_sourceStream is null)
                {
                    _position = value;
                }
                else
                {
                    _sourceStream.Position = value;
                }
            }
        }

        /// <summary>Gets the path that was passed to the constructor.</summary>
        public string Name => _sourcePath;

        public override void Flush()
        {
            ThrowIfDisposed();

            _sourceStream?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            return GetSourceStream().Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();

            if (_sourceStream is null)
            {
                _position = origin switch
                {
                    SeekOrigin.Begin => offset,
                    SeekOrigin.Current => _position + offset,
                    SeekOrigin.End => Length + offset,
                    _ => throw new ArgumentOutOfRangeException(nameof(origin)),
                };

                return _position;
            }
            else
            {
                return _sourceStream.Seek(offset, origin);
            }
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotSupportedException"/> exception.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">The current stream does not support writing.</exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotSupportedException"/> exception.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">The current stream does not support writing.</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
        }

        private FileStream GetSourceStream()
        {
            ThrowIfDisposed();

            if (_sourceStream is null)
            {
                try
                {
                    _sourceStream = new FileStream(_sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    if (_position != 0)
                    {
                        _sourceStream.Position = _position;
                        _position = 0;
                    }
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            return _sourceStream;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _sourceStream?.Dispose();
                _sourceStream = null;

                _isDisposed = true;
            }
            
            base.Dispose(disposing);
        }

        public override int ReadByte()
        {
            return GetSourceStream().ReadByte();
        }
    }
}
