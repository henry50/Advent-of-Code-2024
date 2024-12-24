using System.Collections.Generic;
using System.Linq;
using AdventOfCode2024.Util;

namespace AdventOfCode2024
{
    internal class Day12(string input) : Solution(input)
    {
        readonly Grid<char> grid = new(input);

        public override string Part1()
        {
            return Solve(false);
        }

        public override string Part2()
        {
            return Solve(true);
        }

        private string Solve(bool bulkDiscount)
        {
            int total = 0;
            HashSet<GridCell<char>> unexplored = new(grid.GetAll());
            while (unexplored.Count > 0)
            {
                // find the region for the first unexplored grid item
                var regionBase = unexplored.First();
                // this will be either perimeter or edges depending on bulk discount
                int regionOutside = 0;
                HashSet<GridCell<char>> region = [regionBase];
                Queue<GridCell<char>> frontier = [];
                frontier.Enqueue(regionBase);
                // Expand region as far as possible
                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();
                    var neighbours = current.GetNeighbours(false, true);
                    int thisPerimeter = 0;

                    // explore neighbours
                    foreach (var neighbour in neighbours)
                    {
                        // check if neighbour is in region
                        if (neighbour != null && neighbour.Value == current.Value)
                        {
                            // only add to region/frontier if it isn't already in region
                            if (region.Contains(neighbour))
                            {
                                continue;
                            }
                            region.Add(neighbour);
                            frontier.Enqueue(neighbour);
                        }
                        else
                        {
                            // any out-of-region neighbours increase this cell's perimeter
                            thisPerimeter++;
                        }
                    }
                    // calculate sides
                    if (bulkDiscount)
                    {
                        // single cell surrounded by other regions, 4 sides
                        if (thisPerimeter == 4)
                        {
                            regionOutside = 4;
                        }
                        /* single cell surrounded by 3 other regions
                         * --+
                         * AA|
                         * --+
                         * 1 full side and 2 half sides = 2 sides
                         */
                        else if (thisPerimeter == 3)
                        {
                            regionOutside += 2;
                        }
                        // corner detection time!
                        // one corner = two half sides = 1 side
                        else
                        {
                            int[] offsets = [-1, 1];
                            int row = current.Row;
                            int col = current.Col;
                            char reg = current.Value;
                            foreach (int dr in offsets)
                            {
                                foreach (int dc in offsets)
                                {
                                    // L-shape
                                    if (
                                        InRegion(row + dr, col, reg) && InRegion(row, col + dc, reg)
                                    )
                                    {
                                        // Inside corner pattern
                                        if (!InRegion(row + dr, col + dc, reg))
                                        {
                                            regionOutside++;
                                        }
                                        // Outside corner pattern
                                        if (
                                            !InRegion(row - dr, col, reg)
                                            && !InRegion(row, col - dc, reg)
                                        )
                                        {
                                            regionOutside++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // calculate perimeter
                    else
                    {
                        regionOutside += thisPerimeter;
                    }
                    // this cell has now been explored
                    unexplored.Remove(current);
                }
                // at this point the whole region has been explored, add to total
                total += region.Count * regionOutside;
            }
            return total.ToString();
        }

        private bool InRegion(int row, int col, char region)
        {
            var cell = grid.Get(row, col);
            return cell != null && cell.Value == region;
        }
    }
}
