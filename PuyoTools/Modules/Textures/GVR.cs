using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using VrSharp.GvrTexture;

namespace PuyoTools
{
    // Gvr Texture
    class GVR : TextureModule
    {
        public GVR()
        {
            Name      = "GVR";
            Extension = ".gvr";
            CanEncode = false;
            CanDecode = true;
        }

        // Convert the texture to a bitmap
        public override Bitmap Decode(Stream data)
        {
            try
            {
                GvrTexture TextureInput = new GvrTexture(data.Copy());
                if (TextureInput.NeedsExternalClut())
                {
                    if (PaletteData != null)
                        TextureInput.SetClut(new GvpClut(PaletteData.Copy())); // Texture has an external clut; set it
                    else
                        throw new TextureFormatNeedsPalette(); // Texture needs an external clut; throw an exception
                }

                return TextureInput.GetTextureAsBitmap();
            }
            catch (TextureFormatNeedsPalette)
            {
                throw new TextureFormatNeedsPalette(); // Throw it again
            }
            catch { return null; }
            finally { PaletteData = null; }
        }

        public override Stream Encode(Stream data)
        {
            return null;
        }

        // External Clut Filename
        public override string PaletteFilename(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename) + ".gvp";
        }

        // See if the texture is a Gvr
        public override bool Check(Stream input, string filename)
        {
            try   { return GvrTexture.IsGvrTexture(input.ToByteArray()); }
            catch { return false; }
        }
    }
}