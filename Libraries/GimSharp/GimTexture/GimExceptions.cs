using System;

namespace GimSharp
{
    public class TextureNotInitalizedException : Exception
    {
        public TextureNotInitalizedException() { }

        public TextureNotInitalizedException(string message) : base(message) { }
    }

    public class NotAValidTextureException : Exception
    {
        public NotAValidTextureException() { }

        public NotAValidTextureException(string message) : base(message) { }
    }

    public class PixelFormatNotSupportedException : Exception
    {
        public PixelFormatNotSupportedException() { }

        public PixelFormatNotSupportedException(string message) : base(message) { }
    }

    public class DataFormatNotSupportedException : Exception
    {
        public DataFormatNotSupportedException() { }

        public DataFormatNotSupportedException(string message) : base(message) { }
    }

    public class InvalidTextureDimensionsException : Exception
    {
        public InvalidTextureDimensionsException() { }

        public InvalidTextureDimensionsException(string message) : base(message) { }
    }
}