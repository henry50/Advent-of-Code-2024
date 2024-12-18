using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024
{
    internal class Day02(string input) : Solution(input)
    {
        readonly string input = input;

        public override string Part1()
        {
            int safe = 0;
            foreach (var line in input.Split('\n'))
            {
                int[] levels = line.Split(' ').Select(int.Parse).ToArray();
                bool asc = levels[0] < levels[1];
                bool is_safe = true;
                for (int i = 0; i < levels.Length - 1; i++)
                {
                    int diff = Math.Abs(levels[i] - levels[i + 1]);
                    if (
                        diff < 1
                        || diff > 3
                        || (asc && (levels[i] > levels[i + 1]))
                        || (!asc && (levels[i] < levels[i + 1]))
                    )
                    {
                        is_safe = false;
                        break;
                    }
                }
                if (is_safe)
                {
                    safe++;
                }
            }
            return safe.ToString();
        }

        public override string Part2()
        {
            int safe = 0;
            foreach (var line in input.Split('\n'))
            {
                int[] levels = line.Split(' ').Select(int.Parse).ToArray();
                // try removing each item and no items
                for (int remove = -1; remove < levels.Length; remove++)
                {
                    List<int> dampened = new(levels);
                    // if remove == -1 don't remove anything
                    if (remove > -1)
                    {
                        dampened.RemoveAt(remove);
                    }
                    // same as part 1
                    bool asc = dampened[0] < dampened[1];
                    bool is_safe = true;
                    for (int i = 0; i < dampened.Count - 1; i++)
                    {
                        int diff = Math.Abs(dampened[i] - dampened[i + 1]);
                        if (
                            diff < 1
                            || diff > 3
                            || (asc && (dampened[i] > dampened[i + 1]))
                            || (!asc && (dampened[i] < dampened[i + 1]))
                        )
                        {
                            is_safe = false;
                            break;
                        }
                    }
                    if (is_safe)
                    {
                        safe++;
                        break;
                    }
                }
            }
            return safe.ToString();
        }
    }
}
