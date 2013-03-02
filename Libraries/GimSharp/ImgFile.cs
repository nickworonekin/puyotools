// ImgFile.cs
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
	public class ImgFile
	{
		// - Variables -
        private byte[] PngMagic  =       // Png Header Magic
			{ 0x89, 0x50, 0x4E, 0x47}; 
		private byte[] CompressedData;   // Png Data
		private byte[] DecompressedData; // Regular, Decompressed Data
        private int width;
        private int height;
        private ImageFormat fmt;
		
		// - Constructors -
		// PngFile(byte[] PngFile)
		// Parameters:
		//  PngFile: A byte array of the Png file
		// Description: Loads a Png from a byte array
		public ImgFile(byte[] PngFile)
		{
			SetCompressedData(PngFile);
        }
        // - Constructors -
        // PngFile(byte[] PngFile)
        // Parameters:
        //  PngFile: A byte array of ARGB8888
        // Description: Loads a raw ARGB8888 array
        public ImgFile(byte[] RgbaArray, int Width, int Height, ImageFormat OutputFormat)
        {
            fmt = OutputFormat;
            SetDecompressedData(RgbaArray, Width, Height);
        }
		
		// - Constructors -
		// PngFile(byte[] PngFile)
		// Parameters:
		//  PngFileName: A string to a filename of a Png
		// Description: Loads a Png from a file
		public ImgFile(string PngFileName)
		{
			FileStream File = new FileStream(PngFileName, FileMode.Open);
            byte [] Data = new byte[File.Length];
            File.Read(Data, 0, Data.Length);
            File.Close();

            if (Data.Length < 1) return;

			SetCompressedData(Data);
		}
		
		
		
		
		// public byte[] GetCompressedData()
		// Return Value: The byte array of the decompressed data
		// Description: Gets the decompressed data
		public byte[] GetDecompressedData()
		{
			return DecompressedData;
		}
		// public byte[] GetCompressedData()
		// Return Value: The byte array of the compressed data
		// Description: Gets the compressed Png data
		public byte[] GetCompressedData()
		{
			return CompressedData;
		}
		
		
		
		
		// - Data Input -
		// Description: Inputs compressed Png data (And unpacks it to ARGB8888)
		// Parameters:
		//  byte[] Compressed: Compressed Data to load
		// Return Value: True if the data was properly loaded.
		public bool SetCompressedData(byte[] Compressed)
		{
			CompressedData = Compressed;
			
			// Decompress
            Bitmap TmpBmp = new Bitmap(ImageConverter.byteArrayToImage(CompressedData, fmt));

            DecompressedData = new byte[TmpBmp.Width * TmpBmp.Height * 4];

            width = TmpBmp.Width;
            height = TmpBmp.Height;

			for(int y=0; y<TmpBmp.Height; y++)
			{
				for(int x=0; x<TmpBmp.Width; x++)
				{
					Color pxc = TmpBmp.GetPixel(x,y);
                    DecompressedData[(y * TmpBmp.Width + x) * 4 + 0] = pxc.A;
                    DecompressedData[(y * TmpBmp.Width + x) * 4 + 1] = pxc.R;
                    DecompressedData[(y * TmpBmp.Width + x) * 4 + 2] = pxc.G;
                    DecompressedData[(y * TmpBmp.Width + x) * 4 + 3] = pxc.B;
				}
			}
			
			return true;
		}
		// public bool SetDecompressedData(byte[] Decompressed, short FormatCode)
		// Parameters:
		//  byte[] Decompressed: Decompressed ARGB8888 image to load
		//  int Width: Width of the ARGB8888 image
		//  int Height: Height of the ARGB8888 image
		// Return Value: True if the data was properly loaded.
		// Description: Inputs decompressed, ARGB8888 data (And packs it to an image)
		public bool SetDecompressedData(byte[] Decompressed, int Width, int Height)
		{
            DecompressedData = Decompressed;

            width = Width;
            height = Height;

            // Compress
            Bitmap TmpBmp = new Bitmap(Width,Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData data = TmpBmp.LockBits(new Rectangle(0, 0, TmpBmp.Width, TmpBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imgPtr = (byte*)(data.Scan0);
                int junk = data.Stride - data.Width * 4;

                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        imgPtr[0] = DecompressedData[(y * Width + x) * 4 + 3];
                        imgPtr[1] = DecompressedData[(y * Width + x) * 4 + 2];
                        imgPtr[2] = DecompressedData[(y * Width + x) * 4 + 1];
                        imgPtr[3] = DecompressedData[(y * Width + x) * 4 + 0];
                        imgPtr += 4;
                    }
                    imgPtr += junk;
                }
            }
            TmpBmp.UnlockBits(data);
            Image TmpImg = TmpBmp;
            CompressedData = ImageConverter.imageToByteArray(TmpImg, fmt);
            
			
			return true;
		}
		
		
		
		
		// - File Property Retrieval -
		// public bool IsPng()
		// Return Value: True if the Data Magic is equivalant to that of a Png file
		//               False if not.
		// Description: This function will allow you to validate that the file you have is Png.
		public bool IsPng()
		{
            if (CompressedData.Length < PngMagic.Length) return false;
			
            for (int i = 0; i < PngMagic.Length; i++)
				if (CompressedData[CompressedData.Length] != PngMagic[i]) return false;

            return true;
		}
		public int CompressedLength()
		{
			return CompressedData.Length;
        }
        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }

        public static ImageFormat ImgFormatFromFilename(string InName)
        {
            string LowerName = InName.ToLower();
            if (LowerName.EndsWith(".bmp")) return ImageFormat.Bmp;
            if (LowerName.EndsWith(".emf")) return ImageFormat.Emf;
            if (LowerName.EndsWith(".exf")) return ImageFormat.Exif;
            if (LowerName.EndsWith(".gif")) return ImageFormat.Gif;
            if (LowerName.EndsWith(".ico")) return ImageFormat.Icon;
            if (LowerName.EndsWith(".jpg")) return ImageFormat.Jpeg;
            if (LowerName.EndsWith(".raw")) return ImageFormat.MemoryBmp;
            if (LowerName.EndsWith(".png")) return ImageFormat.Png;
            if (LowerName.EndsWith(".wmf")) return ImageFormat.Wmf;
            if (LowerName.EndsWith(".tiff")) return ImageFormat.Tiff;

            // PNG is by far the most widely used image format with it's level of capability and quality compression.
            // It will be default.
            return ImageFormat.Png;
        }
    }
}