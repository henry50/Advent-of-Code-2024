using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day14(string input) : Solution(input)
    {
        readonly Robot[] robots = input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => new Robot(x))
            .ToArray();
        const int width = 101;
        const int height = 103;

        private static int Mod(int a, int b)
        {
            int r = a % b;
            return r < 0 ? r + b : r;
        }

        public override string Part1()
        {
            int quad1 = 0,
                quad2 = 0,
                quad3 = 0,
                quad4 = 0;
            const int seconds = 100;
            int midX = width / 2;
            int midY = height / 2;
            foreach (Robot robot in robots)
            {
                int endX = Mod(robot.px + (seconds * robot.vx), width);
                int endY = Mod(robot.py + (seconds * robot.vy), height);
                if (endX < midX && endY < midY)
                {
                    quad1++;
                }
                else if (endX < midX && endY > midY)
                {
                    quad2++;
                }
                else if (endX > midX && endY < midY)
                {
                    quad3++;
                }
                else if (endX > midX && endY > midY)
                {
                    quad4++;
                }
            }
            // find product
            return (quad1 * quad2 * quad3 * quad4).ToString();
        }

        public override string Part2()
        {
            const int seconds = 10000;
            // find the tree by looking for a straight line
            string straightLine = new String('\u2593', 15);
            for (int t = 1; t <= seconds; t++)
            {
                char[][] grid = Enumerable
                    .Range(0, height)
                    .Select(x => Enumerable.Range(0, width).Select(y => '\u2591').ToArray())
                    .ToArray();
                foreach (Robot robot in robots)
                {
                    int nextX = Mod(robot.px + (t * robot.vx), width);
                    int nextY = Mod(robot.py + (t * robot.vy), height);
                    grid[nextY][nextX] = '\u2593';
                }
                foreach (char[] row in grid)
                {
                    if (string.Join("", row).Contains(straightLine))
                    {
                        return t + "\n" + string.Join('\n', grid.Select(r => string.Join("", r)));
                    }
                }
            }
            return "Couldn't find tree :(";
        }
    }

    class Robot
    {
        static readonly Regex parser = new Regex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)");
        public readonly int px;
        public readonly int py;
        public readonly int vx;
        public readonly int vy;

        public Robot(string s)
        {
            var groups = parser.Match(s).Groups;
            px = int.Parse(groups[1].Value);
            py = int.Parse(groups[2].Value);
            vx = int.Parse(groups[3].Value);
            vy = int.Parse(groups[4].Value);
        }
    }
}
