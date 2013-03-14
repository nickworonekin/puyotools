using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace PuyoTools.Texture
{
    public static class PTTexture
    {
        public static Dictionary<TextureFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<TextureFormat, FormatEntry>();

            Formats.Add(TextureFormat.GVR, new FormatEntry(new GVR(), "GVR", ".gvr", ".gvp"));
            Formats.Add(TextureFormat.PVR, new FormatEntry(new PVR(), "PVR", ".pvr", ".pvp"));
            Formats.Add(TextureFormat.SVR, new FormatEntry(new SVR(), "SVR", ".svr", ".svp"));
        }

        public static TextureFormat Read(Stream source, Stream destination, int length, string fname)
        {
            TextureFormat format = GetFormat(source, length, fname);

            if (format == TextureFormat.Unknown)
                return format;

            Formats[format].Class.Read(source, destination, length);

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

            Formats[format].Class.Read(source, out destination, length);

            return format;
        }

        public static void Read(Stream source, Stream destination, int length, TextureFormat format)
        {
            Formats[format].Class.Read(source, destination, length);
        }

        public static void Read(Stream source, out Bitmap destination, int length, TextureFormat format)
        {
            Formats[format].Class.Read(source, out destination, length);
        }

        public static void ReadWithPalette(Stream source, Stream palette, Stream destination, int length, int paletteLength, TextureFormat format)
        {
            Formats[format].Class.ReadWithPalette(source, palette, destination, length, paletteLength);
        }

        public static void ReadWithPalette(Stream source, Stream palette, out Bitmap destination, int length, int paletteLength, TextureFormat format)
        {
            Formats[format].Class.ReadWithPalette(source, palette, out destination, length, paletteLength);
        }

        public static void Write(Stream source, Stream destination, string fname, TextureFormat format)
        {
            return;
        }

        public static TextureFormat GetFormat(Stream source, int length, string fname)
        {
            foreach (KeyValuePair<TextureFormat, FormatEntry> format in Formats)
            {
                if (format.Value.Class.Is(source, length, fname))
                    return format.Key;
            }

            return TextureFormat.Unknown;
        }

        public struct FormatEntry
        {
            public TextureBase Class;
            public string Name;
            public string Extension;
            public string PaletteExtension;

            public FormatEntry(TextureBase instance, string name, string extension, string paletteExt)
            {
                Class = instance;
                Name = name;
                Extension = extension;
                PaletteExtension = paletteExt;
            }
        }
    }

    public enum TextureFormat
    {
        Unknown,
        GVR,
        PVR,
        SVR,
    }
}