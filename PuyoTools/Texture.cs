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
        public static void Read(Stream source, Stream destination, TextureFormat format)
        {
            Formats[format].Read(source, destination);
        }

        // Reads a texture with the specified texture format and returns a bitmap
        public static void Read(Stream source, out Bitmap destination, TextureFormat format)
        {
            Formats[format].Read(source, out destination);
        }

        // Writes a texture to the specified texture format
        public static void Write(Stream source, Stream destination, TextureFormat format)
        {
            Formats[format].Write(source, destination);
        }

        // Returns the format used by the source texture.
        public static TextureFormat GetFormat(Stream source, string fname)
        {
            foreach (KeyValuePair<TextureFormat, TextureBase> format in Formats)
            {
                if (format.Value.Is(source, fname))
                    return format.Key;
            }

            return TextureFormat.Unknown;
        }

        // Returns the module for this texture format.
        public static TextureBase GetModule(TextureFormat format)
        {
            return Formats[format];
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