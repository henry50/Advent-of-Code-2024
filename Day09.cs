using System.Linq;

namespace AdventOfCode2024
{
    internal class Day09(string input) : Solution(input)
    {
        readonly string input = input;

        public override string Part1()
        {
            int[] blocks = input.TrimEnd().Select(c => c - '0').ToArray();
            long total = 0;
            int pos = 0;
            int end = blocks.Length - 1;
            for (int i = 0; i < blocks.Length; i++)
            {
                // even index - file
                if (i % 2 == 0)
                {
                    int fileId = i / 2;
                    for (int j = 0; j < blocks[i]; j++)
                    {
                        total += ((pos++) * fileId);
                    }
                }
                // odd index - free space
                else
                {
                    for (int j = 0; j < blocks[i]; j++)
                    {
                        // if end is less than i all remaining space is empty
                        if (end < i)
                        {
                            return total.ToString();
                        }
                        // get the file id of the rightmost file
                        int fileId = end / 2;
                        // add it to the total
                        total += ((pos++) * fileId);

                        // decrement it's size
                        blocks[end]--;
                        // if it's now empty, move to the next rightmost file
                        if (blocks[end] == 0)
                        {
                            end -= 2;
                        }
                    }
                }
            }
            return total.ToString();
        }

        public override string Part2()
        {
            int[] blocks = input.TrimEnd().Select(c => c - '0').ToArray();
            long total = 0;
            int pos = 0;
            int end = blocks.Length - 1;
            for (int i = 0; i < blocks.Length; i++)
            {
                // even index - file
                if (i % 2 == 0)
                {
                    // negative size indicates moved file, skip it's position
                    if (blocks[i] < 0)
                    {
                        // blocks[i] is -1 * the amount to skip, so subtract it from pos
                        pos -= blocks[i];
                    }
                    else
                    {
                        int fileId = i / 2;
                        for (int j = 0; j < blocks[i]; j++)
                        {
                            total += ((pos++) * fileId);
                        }
                    }
                }
                // odd index - free space
                else
                {
                    // try and move every file from the end backwards into this empty space
                    // until it's full or nothing fits
                    ref int sizeOfEmptySpace = ref blocks[i];
                    for (int j = end; j > i; j -= 2)
                    {
                        ref int sizeOfFileToMove = ref blocks[j];
                        if (sizeOfFileToMove > 0 && sizeOfEmptySpace >= sizeOfFileToMove)
                        {
                            int fileId = j / 2;

                            // move file
                            for (int k = 0; k < sizeOfFileToMove; k++)
                            {
                                total += ((pos++) * fileId);
                            }
                            // remove the size of the moved file from the free space
                            sizeOfEmptySpace -= sizeOfFileToMove;
                            // store the negated size of the moved file in it's old location so pos can be skipped later
                            sizeOfFileToMove = -sizeOfFileToMove;
                            // if this space is now full, break
                            if (sizeOfEmptySpace == 0)
                            {
                                break;
                            }
                        }
                    }
                    // increment position past unfillable empty space
                    pos += sizeOfEmptySpace;
                }
            }
            return total.ToString();
        }
    }
}
