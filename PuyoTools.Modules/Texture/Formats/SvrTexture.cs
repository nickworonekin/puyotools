using System;
using System.IO;
using System.Drawing;
using VrSharp.SvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class SvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "SVR"; }
        }

        public override string FileExtension
        {
            get { return ".svr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".svp"; }
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
            // Reading SVR textures is done through VrSharp, so just pass it to that
            VrSharp.SvrTexture.SvrTexture texture = new VrSharp.SvrTexture.SvrTexture(source, length);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (settings != null && settings.PaletteStream != null)
                {
                    if (settings.PaletteLength == -1)
                    {
                        texture.SetPalette(new SvpPalette(settings.PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new SvpPalette(settings.PaletteStream, settings.PaletteLength));
                    }
                }
                else
                {
                    throw new TextureNeedsPaletteException();
                }
            }

            texture.Save(destination);
        }

        public override void Write(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && VrSharp.SvrTexture.SvrTexture.Is(source, length));
        }
    }
}