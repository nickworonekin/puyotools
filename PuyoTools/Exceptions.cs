using System;

namespace PuyoTools
{
    class CompressionFormatNotSupported : Exception
    {
        public CompressionFormatNotSupported()
        {
        }
    }

    class ArchiveFormatNotSupported : Exception
    {
        public ArchiveFormatNotSupported()
        {
        }
    }

    class TextureFormatNotSupported : Exception
    {
        public TextureFormatNotSupported()
        {
        }
    }

    class IncorrectTextureFormat : Exception
    {
        public IncorrectTextureFormat()
        {
        }
    }

    class TextureFormatNeedsPalette : Exception
    {
        public TextureFormatNeedsPalette()
        {
        }
    }
}