using System;
using System.Collections.Generic;

namespace PuyoTools.Modules.Compression
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
            int[] Match = new int[] { 0, 0 };
            int MatchStart;
            int MatchSize;

            for (int i = offsetList[decompressedData[offset]].Count - 1; i >= 0; i--)
            {
                MatchStart = offsetList[decompressedData[offset]][i];
                MatchSize = 1;

                while (MatchSize < maxMatchAmount && MatchSize < windowLength && MatchStart + MatchSize < offset && offset + MatchSize < length && decompressedData[offset + MatchSize] == decompressedData[MatchStart + MatchSize])
                    MatchSize++;

                if (MatchSize >= minMatchAmount && MatchSize > Match[1]) // This is a good match
                {
                    Match = new int[] { (int)(offset - MatchStart), MatchSize };

                    if (MatchSize == maxMatchAmount) // Don't look for more matches
                        break;
                }
            }

            // Return the match.
            // If no match was made, the distance & length pair will be zero
            return Match;
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