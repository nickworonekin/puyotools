using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using GimSharp;
using ImgSharp;

namespace PuyoTools.Old
{
    public class GIM : TextureModule
    {
        // GIM Images
        public GIM()
        {
            Name      = "GIM";
            Extension = ".gim";
            CanEncode = false;
            CanDecode = true;
        }

        // Unpack a GIM into a Bitmap
        public override Bitmap Decode(Stream data)
        {
            // Convert the GIM to an image
            try
            {
                GimFile imageInput  = new GimFile(data.ReadBytes(0, (int)data.Length));
                ImgFile imageOutput = new ImgFile(imageInput.GetDecompressedData(), imageInput.GetWidth(), imageInput.GetHeight(), ImageFormat.Png);

                return new Bitmap(new MemoryStream(imageOutput.GetCompressedData()));
            }
            catch
            {
                return null;
            }
        }

        public override Stream Encode(Stream data)
        {
            return null;
        }

        // Check to see if this is a GIM
        public override bool Check(Stream input, string filename)
        {
            try
            {
                return (input.ReadString(0x0, 12, false) == TextureHeader.MIG ||
                    input.ReadString(0x0, 12, false) == TextureHeader.GIM);
            }
            catch
            {
                return false;
            }
        }
    }
}