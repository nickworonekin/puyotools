using System;
using System.IO;
using System.Drawing;
using VrSharp.PvrTexture;

namespace PuyoTools2.Texture
{
    public class PVR : TextureBase
    {
        public override void Read(byte[] source, long offset, out Bitmap destination, int length)
        {
            // Some PVR textures require an external clut, so we'll just pass this off to ReadWithCLUT
            ReadWithCLUT(source, offset, null, 0, out destination, length, 0);
        }

        public override void ReadWithCLUT(byte[] source, long offset, byte[] clut, long clutOffset, out Bitmap destination, int length, int clutLength)
        {
            // Reading PVR textures is done through VrSharp, so just pass it to that
            PvrTexture texture = new PvrTexture(source, offset, length);

            // Check to see if this texture requires an external CLUT.
            // If it does and none was set, throw an exception.
            if (texture.NeedsExternalClut())
            {
                if (clut != null && clutLength > 0)
                    texture.SetClut(new PvpClut(clut, clutOffset, clutLength));
                else
                    throw new PuyoTools.TextureFormatNeedsPalette();
            }

            destination = texture.GetTextureAsBitmap();
        }

        public override void Write(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && PvrTexture.IsPvrTexture(source, length));
        }

        public override bool CanWrite()
        {
            throw new NotImplementedException();
        }
    }
}