using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2024.Util;

namespace AdventOfCode2024
{
    internal class Day10(string input) : Solution(input)
    {
        readonly Grid<int> grid = new(input, c => (int)char.GetNumericValue(c));

        public override string Part1()
        {
            return Solve(false);
        }

        public override string Part2()
        {
            return Solve(true);
        }

        private string Solve(bool distinctPaths)
        {
            int trails = 0;
            var trailheads = grid.FindAll(0);
            foreach (GridCell<int> trailhead in trailheads)
            {
                Queue<GridCell<int>> frontier = [];
                HashSet<Coordinate> ends = [];
                frontier.Enqueue(trailhead);
                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();
                    var neighbours = current
                        .GetNeighbours(false)
                        .Where(x => x.Value == current.Value + 1)
                        .ToLookup(x => x.Value == 9);
                    var trailEnds = neighbours[true]; // cells with value 9 - trail ends
                    var correctGradients = neighbours[false]; // cells with correct gradient that aren't ends
                    if (distinctPaths)
                    {
                        // include every path that reaches the ends
                        trails += trailEnds.Count();
                    }
                    else
                    {
                        // only include each end point once
                        ends.UnionWith(trailEnds.Select(end => end.Loc));
                    }
                    // expand the frontier
                    foreach (var cell in correctGradients)
                    {
                        if (distinctPaths || !frontier.Contains(cell))
                        {
                            frontier.Enqueue(cell);
                        }
                    }
                }
                trails += ends.Count;
            }

            return trails.ToString();
        }
    }
}
