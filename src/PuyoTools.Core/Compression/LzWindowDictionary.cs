using System;
using System.Collections.Generic;

namespace PuyoTools.Core.Compression
{
    class LzWindowDictionary
    {
        int windowSize = 0x1000;
        int windowStart = 0;
        int windowLength = 0;
        int minMatchAmount = 3;
        int maxMatchAmount = 18;
        List<int>[] offsetList;

        public LzWindowDictionary()
        {
            // Build the offset list, so Lz compression will become significantly faster
            offsetList = new List<int>[0x100];
            for (int i = 0; i < offsetList.Length; i++)
                offsetList[i] = new List<int>();
        }

        public int[] Search(byte[] decompressedData, uint offset, uint length)
        {
            RemoveOldEntries(decompressedData[offset]); // Remove old entries for this index

            if (offset < minMatchAmount || length - offset < minMatchAmount) // Can't find matches if there isn't enough data
                return new int[] { 0, 0 };

            // Start finding matches
            int[] match = new int[] { 0, 0 };
            int matchStart;
            int matchLength;

            for (int i = offsetList[decompressedData[offset]].Count - 1; i >= 0; i--)
            {
                matchStart = offsetList[decompressedData[offset]][i];
                matchLength = 1;

                while (matchLength < maxMatchAmount && matchLength < windowLength && matchStart + matchLength < offset && offset + matchLength < length && decompressedData[offset + matchLength] == decompressedData[matchStart + matchLength])
                    matchLength++;

                if (matchLength >= minMatchAmount && matchLength > match[1]) // This is a good match
                {
                    match = new int[] { (int)(offset - matchStart), matchLength };

                    if (matchLength == maxMatchAmount) // Don't look for more matches
                        break;
                }
            }

            // Return the match.
            // If no match was made, the distance & length pair will be zero
            return match;
        }

        // Slide the window
        private void SlideWindow(int amount)
        {
            if (windowLength == windowSize)
                windowStart += amount;
            else
            {
                if (windowLength + amount <= windowSize)
                    windowLength += amount;
                else
                {
                    amount -= (windowSize - windowLength);
                    windowLength = windowSize;
                    windowStart += amount;
                }
            }
        }

        // Remove old entries
        private void RemoveOldEntries(byte index)
        {
            for (int i = 0; i < offsetList[index].Count; ) // Don't increment i
            {
                if (offsetList[index][i] >= windowStart)
                    break;
                else
                    offsetList[index].RemoveAt(0);
            }
        }

        // Set variables
        public void SetWindowSize(int size)
        {
            windowSize = size;
        }
        public void SetMinMatchAmount(int amount)
        {
            minMatchAmount = amount;
        }
        public void SetMaxMatchAmount(int amount)
        {
            maxMatchAmount = amount;
        }

        // Add entries
        public void AddEntry(byte[] decompressedData, int offset)
        {
            offsetList[decompressedData[offset]].Add(offset);
            SlideWindow(1);
        }
        public void AddEntryRange(byte[] decompressedData, int offset, int length)
        {
            for (int i = 0; i < length; i++)
                AddEntry(decompressedData, offset + i);
        }
    }
}