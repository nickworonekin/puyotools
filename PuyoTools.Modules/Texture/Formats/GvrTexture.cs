using System;
using System.IO;
using System.Drawing;
using VrSharp.GvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class GvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "GVR"; }
        }

        public override string FileExtension
        {
            get { return ".gvr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".gvp"; }
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
            // Reading GVR textures is done through VrSharp, so just pass it to that
            VrSharp.GvrTexture.GvrTexture texture = new VrSharp.GvrTexture.GvrTexture(source, length);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalClut())
            {
                if (settings != null && settings.PaletteStream != null)
                {
                    if (settings.PaletteLength == -1)
                    {
                        texture.SetClut(new GvpClut(settings.PaletteStream));
                    }
                    else
                    {
                        texture.SetClut(new GvpClut(settings.PaletteStream, settings.PaletteLength));
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
            return (length > 16 && VrSharp.GvrTexture.GvrTexture.Is(source, length));
        }
    }
}