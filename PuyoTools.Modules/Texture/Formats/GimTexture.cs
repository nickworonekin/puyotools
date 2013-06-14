using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

//using GimSharp;
//using ImgSharp;

namespace PuyoTools.Modules.Texture
{
    public class GimTexture : TextureBase
    {
        public override string Name
        {
            get { return "GIM"; }
        }

        public override string FileExtension
        {
            get { return ".gim"; }
        }

        public override string PaletteFileExtension
        {
            get { return String.Empty; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="settings">Settings to use when decoding.</param>
        public override void Read(Stream source, Stream destination, int length, TextureReaderSettings settings)
        {
            // Reading GIM textures is done through GimSharp, so just pass it to that
            GimSharp.GimTexture texture = new GimSharp.GimTexture(source, length);

            texture.Save(destination);
        }

        public override void Write(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 24 && GimSharp.GimTexture.Is(source, length));
        }
    }
}