using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using PuyoTools.Modules.Texture;

namespace PuyoTools
{
    public static class Texture
    {
        public static Dictionary<TextureFormat, TextureBase> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<TextureFormat, TextureBase>();

            Formats.Add(TextureFormat.Gim, new GimTexture());
            Formats.Add(TextureFormat.Gvr, new GvrTexture());
            Formats.Add(TextureFormat.Pvr, new PvrTexture());
            Formats.Add(TextureFormat.Svr, new SvrTexture());
        }

        public static TextureFormat Read(Stream source, Stream destination, int length, string fname)
        {
            TextureFormat format = GetFormat(source, length, fname);

            if (format == TextureFormat.Unknown)
                return format;

            Formats[format].Read(source, destination, length);

            return format;
        }

        public static TextureFormat Read(Stream source, out Bitmap destination, int length, string fname)
        {
            TextureFormat format = GetFormat(source, length, fname);

            if (format == TextureFormat.Unknown)
            {
                destination = null;
                return format;
            }

            Formats[format].Read(source, out destination, length);

            return format;
        }

        public static void Read(Stream source, Stream destination, int length, TextureFormat format)
        {
            Formats[format].Read(source, destination, length);
        }

        public static void Read(Stream source, out Bitmap destination, int length, TextureFormat format)
        {
            Formats[format].Read(source, out destination, length);
        }

        public static void ReadWithPalette(Stream source, Stream palette, Stream destination, int length, int paletteLength, TextureFormat format)
        {
            Formats[format].ReadWithPalette(source, palette, destination, length, paletteLength);
        }

        public static void ReadWithPalette(Stream source, Stream palette, out Bitmap destination, int length, int paletteLength, TextureFormat format)
        {
            Formats[format].ReadWithPalette(source, palette, out destination, length, paletteLength);
        }

        public static void Write(Stream source, Stream destination, string fname, TextureFormat format)
        {
            return;
        }

        public static TextureFormat GetFormat(Stream source, int length, string fname)
        {
            foreach (KeyValuePair<TextureFormat, TextureBase> format in Formats)
            {
                if (format.Value.Is(source, length, fname))
                    return format.Key;
            }

            return TextureFormat.Unknown;
        }
    }

    public enum TextureFormat
    {
        Unknown,
        Gim,
        Gvr,
        Pvr,
        Svr,
        Plugin,
    }
}