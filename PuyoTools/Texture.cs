using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using PuyoTools.Modules.Texture;

namespace PuyoTools
{
    public static class Texture
    {
        // Texture format dictionary
        public static Dictionary<TextureFormat, TextureBase> Formats;

        // Initalize the texture format dictionary
        public static void Initalize()
        {
            Formats = new Dictionary<TextureFormat, TextureBase>();

            Formats.Add(TextureFormat.Gim, new GimTexture());
            Formats.Add(TextureFormat.Gvr, new GvrTexture());
            Formats.Add(TextureFormat.Pvr, new PvrTexture());
            Formats.Add(TextureFormat.Svr, new SvrTexture());
        }

        // Reads a texture with the specified texture format
        public static void Read(Stream source, Stream destination, int length, TextureFormat format)
        {
            Formats[format].Read(source, destination, length);
        }

        public static void Read(Stream source, out Bitmap destination, int length, TextureFormat format)
        {
            Formats[format].Read(source, out destination, length);
        }

        // Reads a texture with the specified texture format and texture reader settings
        public static void Read(Stream source, Stream destination, int length, TextureReaderSettings settings, TextureFormat format)
        {
            Formats[format].Read(source, destination, length, settings);
        }
        
        public static void Read(Stream source, out Bitmap destination, int length, TextureReaderSettings settings, TextureFormat format)
        {
            Formats[format].Read(source, out destination, length, settings);
        }

        // Writes a texture to the specified texture format and texture writer settings
        public static void Write(Stream source, Stream destination, int length, TextureWriterSettings settings, TextureFormat format)
        {
            Formats[format].Write(source, destination, length, settings);
        }

        // Returns the format used by the source texture.
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

    // List of texture formats
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