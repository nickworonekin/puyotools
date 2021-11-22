using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.DataCodecs
{
    internal static class DataSwizzler
    {
        public static byte[] Swizzle(byte[] source, int width, int height, int bitsPerPixel)
        {
            if (bitsPerPixel == 4)
            {
                // 4-bit textures are swizzled only if both their width and height are at least 128 pixels.
                if (width < 128 || height < 128)
                {
                    return source;
                }

                byte[] destination = new byte[source.Length];

                int sourceIndex;
                int destinationIndex;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sourceIndex = ((y * width) + x) / 2;
                        destinationIndex = Get4bppPosition(x, y, width); // Swizzing is always based off of the width

                        byte pixel = (byte)((source[sourceIndex] >> ((x & 0x1) * 4)) & 0xF);
                        pixel <<= (byte)(((y >> 1) & 0x01) * 4);

                        destination[destinationIndex] |= pixel;
                    }
                }

                return destination;
            }

            else if (bitsPerPixel == 8)
            {
                // 8-bit textures are swizzled only if their width is at least 128 pixels and height is at least 64 pixels.
                if (width < 128 || height < 64)
                {
                    return source;
                }

                byte[] destination = new byte[source.Length];

                int sourceIndex = 0;
                int destinationIndex;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        destinationIndex = Get8bppPosition(x, y, width); // Swizzing is always based off of the width

                        destination[destinationIndex] = source[sourceIndex];

                        sourceIndex++;
                    }
                }

                return destination;
            }

            else if (bitsPerPixel == 16)
            {
                // 16-bit textures are swizzled only if both their width and height are at least 64 pixels.
                if (width < 64 || height < 64)
                {
                    return source;
                }

                byte[] destination = new byte[source.Length];

                int sourceIndex = 0;
                int destinationIndex;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        destinationIndex = Get16bppPosition(x, y, width); // Swizzing is always based off of the width

                        destination[destinationIndex + 0] = source[sourceIndex + 0];
                        destination[destinationIndex + 1] = source[sourceIndex + 1];

                        sourceIndex += 2;
                    }
                }

                return destination;
            }

            // 32-bit textures are never swizzled. Just return the data as-is.
            else if (bitsPerPixel == 32)
            {
                return source;
            }
            
            throw new ArgumentOutOfRangeException(nameof(bitsPerPixel));
        }

        public static byte[] UnSwizzle(byte[] source, int width, int height, int bitsPerPixel)
        {
            if (bitsPerPixel == 4)
            {
                // 4-bit textures are swizzled only if both their width and height are at least 128 pixels.
                if (width < 128 || height < 128)
                {
                    return source;
                }

                byte[] destination = new byte[source.Length];

                int sourceIndex;
                int destinationIndex;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sourceIndex = Get4bppPosition(x, y, width); // Swizzing is always based off of the width
                        destinationIndex = ((y * width) + x) / 2;

                        byte pixel = (byte)((source[sourceIndex] >> ((y >> 1) & 0x01) * 4) & 0x0F);
                        pixel <<= (byte)((x & 0x1) * 4);

                        destination[destinationIndex] |= pixel;
                    }
                }

                return destination;
            }

            else if (bitsPerPixel == 8)
            {
                // 8-bit textures are swizzled only if their width is at least 128 pixels and height is at least 64 pixels.
                if (width < 128 || height < 64)
                {
                    return source;
                }

                byte[] destination = new byte[source.Length];

                int sourceIndex;
                int destinationIndex = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sourceIndex = Get8bppPosition(x, y, width); // Swizzing is always based off of the width

                        destination[destinationIndex] = source[sourceIndex];

                        destinationIndex++;
                    }
                }

                return destination;
            }

            else if (bitsPerPixel == 16)
            {
                // 16-bit textures are swizzled only if both their width and height are at least 64 pixels.
                if (width < 64 || height < 64)
                {
                    return source;
                }

                byte[] destination = new byte[source.Length];

                int sourceIndex;
                int destinationIndex = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sourceIndex = Get16bppPosition(x, y, width); // Swizzing is always based off of the width

                        destination[destinationIndex + 0] = source[sourceIndex + 0];
                        destination[destinationIndex + 1] = source[sourceIndex + 1];

                        destinationIndex += 2;
                    }
                }

                return destination;
            }

            // 32-bit textures are never swizzled. Just return the data as-is.
            else if (bitsPerPixel == 32)
            {
                return source;
            }

            throw new ArgumentOutOfRangeException(nameof(bitsPerPixel));
        }

        private static int Get4bppPosition(int x, int y, int size)
        {
            int pageX = x & (~0x7f);
            int pageY = y & (~0x7f);
            int pagesH = (size + 127) / 128;
            int pagesV = (size + 127) / 128;
            int pageNum = (pageY / 128) * pagesH + (pageX / 128);

            int page32Y = (pageNum / pagesV) * 32;
            int page32X = (pageNum % pagesV) * 64;

            int pagePos = page32Y * size * 2 + page32X * 4;

            int locX = x & 0x7f;
            int locY = y & 0x7f;

            int blockPos = ((locX & (~0x1f)) >> 1) * size + (locY & (~0xf)) * 2;
            int swapSel = (((y + 2) >> 2) & 0x1) * 4;
            int yPos = (((y & (~3)) >> 1) + (y & 1)) & 0x7;
            int coloumPos = yPos * size * 2 + ((x + swapSel) & 0x7) * 4;

            int byteNum = (x >> 3) & 3;     // 0,1,2,3

            return pagePos + blockPos + coloumPos + byteNum;
        }

        private static int Get8bppPosition(int x, int y, int size)
        {
            int blockPos = (y & (~0xf)) * size + (x & (~0xf)) * 2;
            int swapSel = (((y + 2) >> 2) & 0x1) * 4;
            int yPos = (((y & (~3)) >> 1) + (y & 1)) & 0x7;
            int coloumPos = yPos * size * 2 + ((x + swapSel) & 0x7) * 4;
            int byteNum = ((y >> 1) & 1) + ((x >> 2) & 2); // 0, 1, 2, 3

            return blockPos + coloumPos + byteNum;
        }

        private static int Get16bppPosition(int x, int y, int size)
        {
            int pageX = x & (~0x3f);
            int pageY = y & (~0x3f);
            int pagesH = (size + 63) / 64;
            int pagesV = (size + 63) / 64;
            int pageNum = (pageY / 64) * pagesH + (pageX / 64);

            int page32Y = (pageNum / pagesV) * 32;
            int page32X = (pageNum % pagesV) * 64;

            int pagePos = (page32Y * size + page32X) * 2;

            int locX = x & 0x3f;
            int locY = y & 0x3f;

            int blockPos = (locX & (~0xf)) * size + (locY & (~0x7)) * 2;
            int coloumPos = ((y & 0x7) * size + (x & 0x7)) * 2;

            int byteNum = (x >> 3) & 1;       // 0,1

            return (pagePos + blockPos + coloumPos + byteNum) * 2;
        }
    }
}
