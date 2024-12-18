using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024
{
    internal class Day11(string input) : Solution(input)
    {
        readonly Dictionary<(long stone, int depth), long> CountCache = [];
        readonly List<long> stones = input.Split(' ').Select(long.Parse).ToList();

        public override string Part1()
        {
            CountCache.Clear();
            return Solve(25);
        }

        public override string Part2()
        {
            CountCache.Clear();
            return Solve(75);
        }

        private string Solve(int depth)
        {
            long result = 0;
            foreach (long stone in stones)
            {
                result += CountStones(stone, 0, depth);
            }
            return result.ToString();
        }

        private long CountStones(long stone, int depth, int maxDepth)
        {
            if (depth == maxDepth)
            {
                return 1;
            }
            if (CountCache.TryGetValue((stone, depth), out var lookup))
            {
                return lookup;
            }
            if (stone == 0)
            {
                long result = CountStones(1, depth + 1, maxDepth);
                CountCache.Add((stone, depth), result);
                return result;
            }

            string stoneStr = stone.ToString();
            if (stoneStr.Length % 2 == 0)
            {
                int halfStr = stoneStr.Length / 2;
                long result =
                    CountStones(long.Parse(stoneStr.Substring(0, halfStr)), depth + 1, maxDepth)
                    + CountStones(
                        long.Parse(stoneStr.Substring(halfStr, halfStr)),
                        depth + 1,
                        maxDepth
                    );
                CountCache.Add((stone, depth), result);
                return result;
            }
            else
            {
                long result = CountStones(stone * 2024, depth + 1, maxDepth);
                CountCache.Add((stone, depth), result);
                return result;
            }
        }
    }
}
