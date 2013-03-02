// ImageConverter.cs
// By Nmn / For PuyoNexus.net
// --
// This file is released under the New BSD license. See license.txt for details.
// This code comes with absolutely no warrenty.

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImgSharp
{
    public class ImageConverter
    {
        // A little cheating here - I took this code when my own was failing.
        // Turns out it wasn't the problem, but it's convinient to have this
        // in function form ;)
        static public byte[] imageToByteArray(Image imageIn, ImageFormat fmt)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, fmt);
            return ms.ToArray();
        }
        static public Image byteArrayToImage(byte[] byteArrayIn, ImageFormat fmt)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            fmt = returnImage.RawFormat;
            return returnImage;
        }
    }
}