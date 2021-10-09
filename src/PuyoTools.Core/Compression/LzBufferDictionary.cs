using System;
using System.Collections.Generic;

namespace PuyoTools.Core.Compression
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
            int[] match = new int[] { 0, 0 };
            int matchStart;
            int matchLength;
            int bufferPosition;

            for (int i = offsetList[decompressedData[offset]].Count - 1; i >= 0; i--)
            {
                matchStart = offsetList[decompressedData[offset]][i];
                bufferPosition = (bufferStart + matchStart) & (bufferSize - 1);
                matchLength = 1;

                while (matchLength < maxMatchAmount && matchLength < bufferSize && matchStart + matchLength < offset && offset + matchLength < length && decompressedData[offset + matchLength] == bufferData[(bufferPosition + matchLength) & (bufferSize - 1)])
                    matchLength++;

                if (matchLength >= minMatchAmount && matchLength > match[1]) // This is a good match
                {
                    match = new int[] { bufferPosition, matchLength };

                    if (matchLength == maxMatchAmount) // Don't look for more matches
                        break;
                }
            }

            // Return the match.
            // If no match was made, the distance & length pair will be zero
            return match;
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