using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using GimSharp;
using ImgSharp;

namespace PuyoTools.Texture
{
    public class GimTexture : TextureBase
    {
        public override void Read(byte[] source, long offset, out Bitmap destination, int length)
        {
            // We may need to copy source to a new array. Let's check to see if we need to
            byte[] data = source;
            if (offset != 0 || length != source.Length)
            {
                data = new byte[length];
                Array.Copy(source, offset, data, 0, length);
            }

            // Do it like this until the GimSharp rewrite.
            GimFile textureInput = new GimFile(data);
            ImgFile textureOutput = new ImgFile(textureInput.GetDecompressedData(), textureInput.GetWidth(), textureInput.GetHeight(), ImageFormat.Png);

            destination = new Bitmap(new MemoryStream(textureOutput.GetCompressedData()));
        }

        public override void Write(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 12 && PTStream.Contains(source, 0, new byte[] { (byte)'M', (byte)'I', (byte)'G', (byte)'.', (byte)'0', (byte)'0', (byte)'.', (byte)'1', (byte)'P', (byte)'S', (byte)'P', 0 }));
        }

        public override bool CanWrite()
        {
            return false;
        }
    }
}