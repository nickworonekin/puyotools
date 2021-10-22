using System;
using System.IO;

namespace PuyoTools.Core.Textures.Gvr
{
    public class GvrPaletteEncoder
    {
        #region Fields
        private GvrTextureEncoder textureEncoder;

        private int paletteEntries; // Number of palette entries in the palette data

        private static readonly byte[] magicCode = { (byte)'G', (byte)'V', (byte)'P', (byte)'L' };

        private byte[] encodedPaletteData;
        #endregion

        #region Constructors & Initalizers
        internal GvrPaletteEncoder(GvrTextureEncoder textureEncoder, byte[] palette, int count)
        {
            this.textureEncoder = textureEncoder;
            encodedPaletteData = palette;
            paletteEntries = count;
        }
        #endregion

        #region Encode Palette
        /// <summary>
        /// Saves the encoded palette to the specified file.
        /// </summary>
        /// <param name="file">Name of the file to save the data to.</param>
        public void Save(string file)
        {
            using (var stream = File.OpenWrite(file))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Saves the encoded palette to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the palette to.</param>
        public void Save(Stream destination)
        {
            var writer = new BinaryWriter(destination);

            writer.Write(magicCode);
            writer.WriteInt32(encodedPaletteData.Length + 8);
            writer.WriteByte(0);
            writer.WriteByte((byte)textureEncoder.PaletteFormat.Value);
            writer.WriteUInt32(0);
            writer.WriteUInt16BigEndian((ushort)paletteEntries);

            writer.Write(encodedPaletteData);
        }
        #endregion
    }
}