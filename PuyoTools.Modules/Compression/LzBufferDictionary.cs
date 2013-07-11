using System;
using System.Collections.Generic;

namespace PuyoTools.Modules.Compression
{
    class LzBufferDictionary
    {
        int MinMatchAmount = 3;
        int MaxMatchAmount = 18;
        int BufferSize = 0;
        int BufferStart = 0;
        int BufferPointer = 0;
        byte[] BufferData;
        List<int>[] OffsetList;

        public LzBufferDictionary()
        {
            // Build the offset list, so Lz compression will become significantly faster
            OffsetList = new List<int>[0x100];
            for (int i = 0; i < OffsetList.Length; i++)
                OffsetList[i] = new List<int>();

            // Build a new blank buffer
            BufferData = new byte[0];
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
            int BufferPos;

            for (int i = OffsetList[DecompressedData[offset]].Count - 1; i >= 0; i--)
            {
                MatchStart = OffsetList[DecompressedData[offset]][i];
                BufferPos = (BufferStart + MatchStart) & (BufferSize - 1);
                MatchSize = 1;

                while (MatchSize < MaxMatchAmount && MatchSize < BufferSize && MatchStart + MatchSize < offset && offset + MatchSize < length && DecompressedData[offset + MatchSize] == BufferData[(BufferPos + MatchSize) & (BufferSize - 1)])
                    MatchSize++;

                if (MatchSize >= MinMatchAmount && MatchSize > Match[1]) // This is a good match
                {
                    Match = new int[] { BufferPos, MatchSize };

                    if (MatchSize == MaxMatchAmount) // Don't look for more matches
                        break;
                }
            }

            // Return the match.
            // If no match was made, the distance & length pair will be zero
            return Match;
        }

        // Remove old entries
        private void RemoveOldEntries(byte index)
        {
            for (int i = 0; i < OffsetList[index].Count; ) // Don't increment i
            {
                if (OffsetList[index][i] >= BufferPointer - BufferSize)
                    break;
                else
                    OffsetList[index].RemoveAt(0);
            }
        }

        // Set variables
        public void SetBufferSize(int size)
        {
            if (BufferSize != size)
            {
                BufferSize = size;
                BufferData = new byte[BufferSize];

                // Add the 0's to the buffer
                for (int i = 0; i < BufferSize; i++)
                {
                    BufferData[i] = 0;
                    OffsetList[0].Add(i - BufferSize); // So the entries get deleted upon filling up
                }
            }
        }
        public void SetBufferStart(int pos)
        {
            BufferStart = pos;
        }
        public void SetMinMatchAmount(int amount)
        {
            MinMatchAmount = amount;
        }
        public void SetMaxMatchAmount(int amount)
        {
            MaxMatchAmount = amount;
        }

        // Add entries
        public void AddEntry(byte[] DecompressedData, int offset)
        {
            BufferData[(BufferStart + BufferPointer) & (BufferSize - 1)] = DecompressedData[offset];
            OffsetList[DecompressedData[offset]].Add(BufferPointer);
            BufferPointer++;
        }
        public void AddEntryRange(byte[] DecompressedData, int offset, int length)
        {
            for (int i = 0; i < length; i++)
                AddEntry(DecompressedData, offset + i);
        }
    }
}