using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace PuyoTools2.Texture
{
    public static class Texture
    {
        public static Dictionary<TextureFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<TextureFormat, FormatEntry>();

            Formats.Add(TextureFormat.GVR, new FormatEntry(new GVR(), "GVR", "*.gvr"));
            Formats.Add(TextureFormat.PVR, new FormatEntry(new PVR(), "PVR", "*pvr;*.pvz"));
            Formats.Add(TextureFormat.SVR, new FormatEntry(new SVR(), "SVR", "*.svr"));
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
            public string Filter;

            public FormatEntry(TextureBase instance, string name, string filter)
            {
                Class = instance;
                Name = name;
                Filter = filter;
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