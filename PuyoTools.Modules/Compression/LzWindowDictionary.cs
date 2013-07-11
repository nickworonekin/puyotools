using System;
using System.Collections.Generic;

namespace PuyoTools.Modules.Compression
{
    class LzWindowDictionary
    {
        int WindowSize = 0x1000;
        int WindowStart = 0;
        int WindowLength = 0;
        int MinMatchAmount = 3;
        int MaxMatchAmount = 18;
        int BlockSize = 0;
        List<int>[] OffsetList;

        public LzWindowDictionary()
        {
            // Build the offset list, so Lz compression will become significantly faster
            OffsetList = new List<int>[0x100];
            for (int i = 0; i < OffsetList.Length; i++)
                OffsetList[i] = new List<int>();
        }

        public int[] Search(byte[] DecompressedData, uint offset, uint length)
        {
            RemoveOldEntries(DecompressedData[offset]); // Remove old entries for this index

            if (offset < MinMatchAmount || length - offset < MinMatchAmount) // Can't find matches if there isn't enough data
                return new int[] { 0, 0 };

            // Start finding matches
            int[] Match = new int[] { 0, 0 };
            int MatchStart;
            int MatchSize;

            for (int i = OffsetList[DecompressedData[offset]].Count - 1; i >= 0; i--)
            {
                MatchStart = OffsetList[DecompressedData[offset]][i];
                MatchSize = 1;

                while (MatchSize < MaxMatchAmount && MatchSize < WindowLength && MatchStart + MatchSize < offset && offset + MatchSize < length && DecompressedData[offset + MatchSize] == DecompressedData[MatchStart + MatchSize])
                    MatchSize++;

                if (MatchSize >= MinMatchAmount && MatchSize > Match[1]) // This is a good match
                {
                    Match = new int[] { (int)(offset - MatchStart), MatchSize };

                    if (MatchSize == MaxMatchAmount) // Don't look for more matches
                        break;
                }
            }

            // Return the match.
            // If no match was made, the distance & length pair will be zero
            return Match;
        }

        // Slide the window
        public void SlideWindow(int Amount)
        {
            if (WindowLength == WindowSize)
                WindowStart += Amount;
            else
            {
                if (WindowLength + Amount <= WindowSize)
                    WindowLength += Amount;
                else
                {
                    Amount -= (WindowSize - WindowLength);
                    WindowLength = WindowSize;
                    WindowStart += Amount;
                }
            }
        }

        // Slide the window to the next block
        public void SlideBlock()
        {
            WindowStart += BlockSize;
        }

        // Remove old entries
        private void RemoveOldEntries(byte index)
        {
            for (int i = 0; i < OffsetList[index].Count; ) // Don't increment i
            {
                if (OffsetList[index][i] >= WindowStart)
                    break;
                else
                    OffsetList[index].RemoveAt(0);
            }
        }

        // Set variables
        public void SetWindowSize(int size)
        {
            WindowSize = size;
        }
        public void SetMinMatchAmount(int amount)
        {
            MinMatchAmount = amount;
        }
        public void SetMaxMatchAmount(int amount)
        {
            MaxMatchAmount = amount;
        }
        public void SetBlockSize(int size)
        {
            BlockSize = size;
            WindowLength = size; // The window will work in blocks now
        }

        // Add entries
        public void AddEntry(byte[] DecompressedData, int offset)
        {
            OffsetList[DecompressedData[offset]].Add(offset);
        }
        public void AddEntryRange(byte[] DecompressedData, int offset, int length)
        {
            for (int i = 0; i < length; i++)
                AddEntry(DecompressedData, offset + i);
        }
    }
}