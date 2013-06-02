using System;
using System.IO;
using System.Drawing;
using VrSharp.PvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class PvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "PVR"; }
        }

        public override string FileExtension
        {
            get { return ".pvr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".pvp"; }
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
            // Reading PVR textures is done through VrSharp, so just pass it to that
            VrSharp.PvrTexture.PvrTexture texture = new VrSharp.PvrTexture.PvrTexture(source, length);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalClut())
            {
                if (settings != null && settings.PaletteStream != null)
                {
                    if (settings.PaletteLength == -1)
                    {
                        texture.SetClut(new PvpClut(settings.PaletteStream));
                    }
                    else
                    {
                        texture.SetClut(new PvpClut(settings.PaletteStream, settings.PaletteLength));
                    }
                }
                else
                {
                    throw new TextureNeedsPaletteException();
                }
            }

            MemoryStream destinationStream = texture.ToStream();
            destinationStream.Position = 0;
            PTStream.CopyTo(destinationStream, destination);
        }

        public override void Write(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && VrSharp.PvrTexture.PvrTexture.Is(source, length));
        }
    }
}