using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using GimSharp;
using ImgSharp;

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
            // Temporary until the GimSharp rewrite is done and complete.
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            GimFile textureInput = new GimFile(buffer);
            ImgFile textureOutput = new ImgFile(textureInput.GetDecompressedData(), textureInput.GetWidth(), textureInput.GetHeight(), ImageFormat.Png);

            buffer = textureOutput.GetCompressedData();
            destination.Write(buffer, 0, buffer.Length);
        }

        public override void Write(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 12 && PTStream.Contains(source, 0, new byte[] { (byte)'M', (byte)'I', (byte)'G', (byte)'.', (byte)'0', (byte)'0', (byte)'.', (byte)'1', (byte)'P', (byte)'S', (byte)'P', 0 }));
        }
    }
}