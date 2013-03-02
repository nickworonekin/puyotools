using System;

namespace GimSharp
{
    public static class GimSwizzle
    {
        // Swizzle a GIM image
        public static void Swizzle(ref byte[] Buf, int Pointer, int Width, int Height)
        {
            // Make a copy of the unswizzled input
            byte[] UnSwizzled = new byte[Buf.Length - Pointer];
            Array.Copy(Buf, Pointer, UnSwizzled, 0, UnSwizzled.Length);

            int rowblocks = (Width / 16);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int blockx = x / 16;
                    int blocky = y / 8;

                    int block_index = blockx + ((blocky) * rowblocks);
                    int block_address = block_index * 16 * 8;

                    Buf[Pointer + block_address + (x - blockx * 16) + ((y - blocky * 8) * 16)] = UnSwizzled[x + (y * Width)];
                }
            }
        }

        // Unswizzle a GIM image
        public static void UnSwizzle(ref byte[] Buf, int Pointer, int Width, int Height)
        {
            // Make a copy of the unswizzled input
            byte[] Swizzled = new byte[Buf.Length - Pointer];
            Array.Copy(Buf, Pointer, Swizzled, 0, Swizzled.Length);

            int rowblocks = (Width / 16);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int blockx = x / 16;
                    int blocky = y / 8;

                    int block_index   = blockx + (blocky * rowblocks);
                    int block_address = block_index * 16 * 8;

                    Buf[Pointer + x + (y * Width)] = Swizzled[block_address + (x - blockx * 16) + ((y - blocky * 8) * 16)];
                }
            }
        }
    }
}