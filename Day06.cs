using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024
{
    internal class Day06 : Solution
    {
        (int row, int col) initPosition;
        readonly bool[][] obstacles;

        public Day06(string input)
            : base(input)
        {
            string[] split = input.Split(
                '\n',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            );
            initPosition = (-1, -1);
            for (int r = 0; r < split.Length; r++)
            {
                for (int c = 0; c < split[r].Length; c++)
                {
                    if (split[r][c] == '^')
                    {
                        initPosition = (r, c);
                        break;
                    }
                }
            }
            if (initPosition == (-1, -1))
            {
                throw new Exception("^ does not appear in input file");
            }
            obstacles = split.Select(x => x.Select(y => y == '#').ToArray()).ToArray();
        }

        public override string Part1()
        {
            return Solve(false).ToString();
        }

        public override string Part2()
        {
            int total = 0;
            // try adding an obstacle everywhere
            for (int r = 0; r < obstacles.Length; r++)
            {
                for (int c = 0; c < obstacles[r].Length; c++)
                {
                    // only add obstacles where they aren't already and don't add to the start position
                    if (!obstacles[r][c] && !(r == initPosition.row && c == initPosition.col))
                    {
                        obstacles[r][c] = true;
                        total += Solve(true);
                        obstacles[r][c] = false;
                    }
                }
            }
            return total.ToString();
        }

        private int Solve(bool checkLoop)
        {
            (int row, int col) position = initPosition;
            (int row, int col) nextPosition;
            HashSet<(int, int)> visited = [initPosition];
            HashSet<(int, int, int, int)> directedVisited =
            [
                (initPosition.row, initPosition.col, -1, 0),
            ];
            (int row, int col) facing = (-1, 0); // up
            while (true)
            {
                // go forward as long as possible
                nextPosition = (position.row + facing.row, position.col + facing.col);
                // left the map?
                if (!InMapRange(nextPosition, obstacles))
                {
                    break;
                }
                // hit an obstacle?
                if (obstacles[nextPosition.row][nextPosition.col])
                {
                    // turn
                    facing = facing switch
                    {
                        (-1, 0) => (0, 1), // up -> right
                        (0, 1) => (1, 0), // right -> down
                        (1, 0) => (0, -1), // down -> left
                        (0, -1) => (-1, 0), // left -> up
                        _ => throw new NotImplementedException(),
                    };
                }
                // keep going forward
                else
                {
                    if (checkLoop)
                    {
                        // if the same place is reached in the same direction there must be a loop
                        var directed = (nextPosition.row, nextPosition.col, facing.row, facing.col);
                        if (directedVisited.Contains(directed))
                        {
                            return 1;
                        }
                        directedVisited.Add(directed);
                    }
                    else
                    {
                        visited.Add(nextPosition);
                    }
                    position = nextPosition;
                }
            }
            if (checkLoop)
            {
                return 0;
            }
            return visited.Count;
        }

        private bool InMapRange((int row, int col) position, bool[][] map)
        {
            return position.row >= 0
                && position.row < map.Length
                && position.col >= 0
                && position.col < map[0].Length;
        }
    }
}
