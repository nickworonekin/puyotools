using System;
using System.IO;

using VrSharp;
using VrSharp.SvrTexture;

namespace PuyoTools.Modules.Texture
{
    public class SvrTexture : TextureBase
    {
        public override string Name
        {
            get { return "SVR"; }
        }

        public override string FileExtension
        {
            get { return ".svr"; }
        }

        public override string PaletteFileExtension
        {
            get { return ".svp"; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public SvrTexture()
        {
            // Set default values
            HasGlobalIndex = true;
            GlobalIndex = 0;

            PixelFormat = SvrPixelFormat.Rgb5a3;
            DataFormat = SvrDataFormat.Rectangle;
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Read(Stream source, Stream destination)
        {
            // Reading SVR textures is done through VrSharp, so just pass it to that
            VrSharp.SvrTexture.SvrTexture texture = new VrSharp.SvrTexture.SvrTexture(source);

            // Check to see if this texture requires an external palette and throw an exception
            // if we do not have one defined
            if (texture.NeedsExternalPalette)
            {
                if (PaletteStream != null)
                {
                    if (PaletteLength == -1)
                    {
                        texture.SetPalette(new SvpPalette(PaletteStream));
                    }
                    else
                    {
                        texture.SetPalette(new SvpPalette(PaletteStream, PaletteLength));
                    }

                    PaletteStream = null;
                    PaletteLength = -1;
                }
                else
                {
                    throw new TextureNeedsPaletteException();
                }
            }

            texture.Save(destination);
        }

        #region Writer Settings
        /// <summary>
        /// Sets whether or not this texture has a global index when encoding. If false, the texture will not include a GBIX header. The default value is true.
        /// </summary>
        public bool HasGlobalIndex { get; set; }

        /// <summary>
        /// Sets the texture's global index when encoding. This only matters if HasGlobalIndex is true. The default value is 0.
        /// </summary>
        public uint GlobalIndex { get; set; }

        /// <summary>
        /// Sets the texture's pixel format for encoding. The default value is SvrPixelFormat.Rgb5a3.
        /// </summary>
        public SvrPixelFormat PixelFormat { get; set; }

        /// <summary>
        /// Sets the texture's data format for encoding. The default value is SvrDataFormat.Rectangle.
        /// </summary>
        public SvrDataFormat DataFormat { get; set; }
        #endregion

        public override void Write(Stream source, Stream destination)
        {
            // Writing SVR textures is done through VrSharp, so just pass it to that
            SvrTextureEncoder texture = new SvrTextureEncoder(source, PixelFormat, DataFormat);

            if (!texture.Initalized)
            {
                throw new TextureNotInitalizedException("Unable to initalize texture.");
            }

            texture.HasGlobalIndex = HasGlobalIndex;
            if (texture.HasGlobalIndex)
            {
                texture.GlobalIndex = GlobalIndex;
            }

            // If we have an external palette file, save it
            if (texture.NeedsExternalPalette)
            {
                needsExternalPalette = true;

                PaletteStream = new MemoryStream();
                texture.PaletteEncoder.Save(PaletteStream);
            }
            else
            {
                needsExternalPalette = false;
            }

            texture.Save(destination);
        }

        public override ModuleSettingsControl GetModuleSettingsControl()
        {
            return new SvrWriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && VrSharp.SvrTexture.SvrTexture.Is(source, length));
        }
    }
}