using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using VrSharp.PvrTexture;

namespace PuyoTools.Old
{
    // Pvr Texture
    class PVR : TextureModule
    {
        public PvrPixelFormat PixelFormat = PvrPixelFormat.Argb1555;
        public PvrDataFormat DataFormat = PvrDataFormat.SquareTwiddled;
        public PvrCompressionFormat CompressionFormat = PvrCompressionFormat.None;
        public bool GbixHeader = true;
        public uint GlobalIndex = 0;

        public PVR()
        {
            Name      = "PVR";
            Extension = ".pvr";
            CanEncode = true;
            CanDecode = true;
        }

        // Convert the texture to a bitmap
        public override Bitmap Decode(Stream data)
        {
            try
            {
                PvrTexture TextureInput = new PvrTexture(data.Copy());
                if (TextureInput.NeedsExternalClut())
                {
                    if (PaletteData != null)
                        TextureInput.SetClut(new PvpClut(PaletteData.Copy())); // Texture has an external clut; set it
                    else
                        throw new TextureFormatNeedsPalette(); // Texture needs an external clut; throw an exception
                }

                return TextureInput.GetTextureAsBitmap();
            }
            catch (TextureFormatNeedsPalette)
            {
                throw new TextureFormatNeedsPalette(); // Throw it again
            }
            //catch   { return null; }
            finally { PaletteData = null; }
        }

        public override Stream Encode(Stream data)
        {
            // Convert the bitmap to a pvr
            try
            {
                PvrTextureEncoder TextureEncoder = new PvrTextureEncoder(data, PixelFormat, DataFormat);
                TextureEncoder.EnableGbix(GbixHeader);
                if (GbixHeader)
                    TextureEncoder.WriteGbix(GlobalIndex);
                if (CompressionFormat != PvrCompressionFormat.None)
                    TextureEncoder.SetCompressionFormat(CompressionFormat);

                return TextureEncoder.GetTextureAsStream();
            }
            catch { return null; }
        }

        // External Clut Filename
        public override string PaletteFilename(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename) + ".pvp";
        }

        // See if the texture is a Pvr
        public override bool Check(Stream input, string filename)
        {
            try   { return PvrTexture.IsPvrTexture(input.ToByteArray()); }
            catch { return false; }
        }
    }
}