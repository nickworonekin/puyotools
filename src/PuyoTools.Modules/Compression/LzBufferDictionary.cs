using System;
using System.Collections.Generic;

namespace PuyoTools.Modules.Compression
{
    class LzBufferDictionary
    {
        int minMatchAmount = 3;
        int maxMatchAmount = 18;
        int bufferSize = 0;
        int bufferStart = 0;
        int bufferPointer = 0;
        byte[] bufferData;
        List<int>[] offsetList;

        public LzBufferDictionary()
        {
            // Build the offset list, so Lz compression will become significantly faster
            offsetList = new List<int>[0x100];
            for (int i = 0; i < offsetList.Length; i++)
                offsetList[i] = new List<int>();

            // Build a new blank buffer
            bufferData = new byte[0];
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
            int BufferPos;

            for (int i = offsetList[decompressedData[offset]].Count - 1; i >= 0; i--)
            {
                MatchStart = offsetList[decompressedData[offset]][i];
                BufferPos = (bufferStart + MatchStart) & (bufferSize - 1);
                MatchSize = 1;

                while (MatchSize < maxMatchAmount && MatchSize < bufferSize && MatchStart + MatchSize < offset && offset + MatchSize < length && decompressedData[offset + MatchSize] == bufferData[(BufferPos + MatchSize) & (bufferSize - 1)])
                    MatchSize++;

                if (MatchSize >= minMatchAmount && MatchSize > Match[1]) // This is a good match
                {
                    Match = new int[] { BufferPos, MatchSize };

                    if (MatchSize == maxMatchAmount) // Don't look for more matches
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
            for (int i = 0; i < offsetList[index].Count; ) // Don't increment i
            {
                if (offsetList[index][i] >= bufferPointer - bufferSize)
                    break;
                else
                    offsetList[index].RemoveAt(0);
            }
        }

        // Set variables
        public void SetBufferSize(int size)
        {
            if (bufferSize != size)
            {
                bufferSize = size;
                bufferData = new byte[bufferSize];

                // Add the 0's to the buffer
                for (int i = 0; i < bufferSize; i++)
                {
                    bufferData[i] = 0;
                    offsetList[0].Add(i - bufferSize); // So the entries get deleted upon filling up
                }
            }
        }
        public void SetBufferStart(int pos)
        {
            bufferStart = pos;
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
            bufferData[(bufferStart + bufferPointer) & (bufferSize - 1)] = decompressedData[offset];
            offsetList[decompressedData[offset]].Add(bufferPointer);
            bufferPointer++;
        }
        public void AddEntryRange(byte[] decompressedData, int offset, int length)
        {
            for (int i = 0; i < length; i++)
                AddEntry(decompressedData, offset + i);
        }
    }
}