using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2024.Util;

namespace AdventOfCode2024
{
    internal class Day10 : Solution
    {
        public override string Part1(string input)
        {
            return Solve(input, false);
        }

        public override string Part2(string input)
        {
            return Solve(input, true);
        }

        private static string Solve(string input, bool distinctPaths)
        {
            int trails = 0;
            Grid<int> grid = new(input, c => (int)char.GetNumericValue(c));
            var trailheads = grid.FindAll(0);
            foreach (GridCell<int> trailhead in trailheads)
            {
                Queue<GridCell<int>> frontier = [];
                HashSet<GridCell<int>> ends = [];
                frontier.Enqueue(trailhead);
                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();
                    var neighbours = current
                        .GetNeighbours(false)
                        .Where(x => x.Value == current.Value + 1)
                        .ToLookup(x => x.Value == 9);
                    if (distinctPaths)
                    {
                        // include every path that reaches the ends
                        trails += neighbours[true].Count();
                    }
                    else
                    {
                        // only include each end point once
                        foreach (var end in neighbours[true])
                        {
                            ends.Add(end);
                        }
                    }
                    // expand the frontier
                    foreach (var item in neighbours[false])
                    {
                        if (distinctPaths || !frontier.Contains(item))
                        {
                            frontier.Enqueue(item);
                        }
                    }
                }
                trails += ends.Count;
            }

            return trails.ToString();
        }
    }
}
