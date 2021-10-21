using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr
{
    public class PvrMipmapDecoder
    {
        private readonly PvrTextureDecoder textureDecoder;
        private readonly byte[] textureData;
        private byte[] decodedData;

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        internal PvrMipmapDecoder(
            PvrTextureDecoder textureDecoder,
            byte[] textureData,
            int width,
            int height)
        {
            this.textureDecoder = textureDecoder;
            this.textureData = textureData;

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Saves the decoded texture mipmap to the specified file as a PNG.
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
        /// Saves the decoded texture mipmap to the specified stream as a PNG.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            var image = Image.LoadPixelData<Bgra32>(GetPixelData(), Width, Height);
            image.Save(destination, new PngEncoder());
        }

        // Decodes a texture
        private byte[] DecodeTexture()
        {
            return textureDecoder.DecodeTexture(textureData, Width, Height);
        }

        /// <summary>
        /// Decodes the texture mipmap and returns the pixel data.
        /// </summary>
        /// <returns>The pixel data as a byte array.</returns>
        public byte[] GetPixelData()
        {
            if (decodedData == null)
            {
                decodedData = DecodeTexture();
            }

            return decodedData;
        }
    }
}
