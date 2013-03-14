using System;
using System.Collections.Generic;
using GimSharp;
using VrSharp.PvrTexture;

namespace PuyoTools.Old
{
    public abstract class TextureFormatSettings
    {
        // Abstract Functions
        public Dictionary<object, string> PixelFormats { get; protected set; }
        public Dictionary<object, string> DataFormats  { get; protected set; }

        public class GIM : TextureFormatSettings
        {
            public GIM()
            {
                // Add Pixel Formats
                PixelFormats = new Dictionary<object, string>();
                PixelFormats.Add(GimPixelFormat.Rgb565,   "RGB565");
                PixelFormats.Add(GimPixelFormat.Argb1555, "ARGB1555");
                PixelFormats.Add(GimPixelFormat.Argb4444, "ARGB4444");
                PixelFormats.Add(GimPixelFormat.Argb8888, "ARGB8888");

                // Add Data Formats
                DataFormats = new Dictionary<object, string>();
                PixelFormats.Add(GimDataFormat.Rgb565,   "RGB565");
                PixelFormats.Add(GimDataFormat.Argb1555, "ARGB1555");
                PixelFormats.Add(GimDataFormat.Argb4444, "ARGB4444");
                PixelFormats.Add(GimDataFormat.Argb8888, "ARGB8888");
                PixelFormats.Add(GimDataFormat.Index4,   "4-bit Palettized");
                PixelFormats.Add(GimDataFormat.Index8,   "8-bit Palettized");
                PixelFormats.Add(GimDataFormat.Index16,  "16-bit Palettized");
                PixelFormats.Add(GimDataFormat.Index32,  "32-bit Palettized");
            }
        }

        public class PVR : TextureFormatSettings
        {
            public PVR()
            {
                // Add Pixel Formats
                PixelFormats = new Dictionary<object, string>();
                PixelFormats.Add(PvrPixelFormat.Argb1555, "ARGB1555");
                PixelFormats.Add(PvrPixelFormat.Rgb565,   "RGB565");
                PixelFormats.Add(PvrPixelFormat.Argb4444, "ARGB4444");

                // Add Data Formats
                DataFormats = new Dictionary<object, string>();
                DataFormats.Add(PvrDataFormat.SquareTwiddled,    "Square Twiddled");
                DataFormats.Add(PvrDataFormat.Rectangle,         "Rectangle");
                DataFormats.Add(PvrDataFormat.RectangleTwiddled, "Rectangular Twiddled");
            }
        }

        // Convert index to pixel or data format, based on current dictionary
        // Note that it returns an object, you must use a cast
        // to return the correct format
        public object IndexToPixelFormat(int index)
        {
            int i = 0;
            foreach (KeyValuePair<object, string> value in PixelFormats)
            {
                if (i == index)
                    return value.Key;

                i++;
            }

            return null;
        }
        public object IndexToDataFormat(int index)
        {
            int i = 0;
            foreach (KeyValuePair<object, string> value in DataFormats)
            {
                if (i == index)
                    return value.Key;

                i++;
            }

            return null;
        }
    }
}