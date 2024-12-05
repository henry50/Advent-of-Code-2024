
using System;

namespace AdventOfCode2024
{
    internal class Day04 : Solution
    {
        public override string Part1(string input)
        {
            string[] grid = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int matches = 0;
            // iterate each cell
            int rowmax = grid.Length;
            for (int row = 0; row < rowmax; row++)
            {
                int colmax = grid[row].Length;
                for (int col = 0; col < colmax; col++)
                {
                    // all matches must begin with X
                    if (grid[row][col] == 'X')
                    {
                        // consider neighbours
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                // check if MAS appears in one direction
                                if (
                                    Match(grid, 'M', row + i, col + j, rowmax, colmax) &&
                                    Match(grid, 'A', row + (2 * i), col + (2 * j), rowmax, colmax) &&
                                    Match(grid, 'S', row + (3 * i), col + (3 * j), rowmax, colmax)
                                )
                                {
                                    matches++;
                                }
                            }
                        }
                    }
                }
            }
            return matches.ToString();
        }
        public override string Part2(string input)
        {
            string[] grid = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int matches = 0;
            int[] corners = { -1, 1 };
            // iterate each cell
            int rowmax = grid.Length;
            for (int row = 0; row < rowmax; row++)
            {
                int colmax = grid[row].Length;
                for (int col = 0; col < colmax; col++)
                {
                    // all matches must begin with M
                    if (grid[row][col] == 'M')
                    {
                        // consider diagonal neighbours
                        foreach (int i in corners)
                        {
                            foreach (int j in corners)
                            {
                                if (
                                    // check MAS appears diagonally in one direction
                                    Match(grid, 'A', row + i, col + j, rowmax, colmax) &&
                                    Match(grid, 'S', row + (2 * i), col + (2 * j), rowmax, colmax) &&
                                    ((
                                        // either M is the far row and S is the far col
                                        Match(grid, 'M', row + (2 * i), col, rowmax, colmax) &&
                                        Match(grid, 'S', row, col + (2 * j), rowmax, colmax)
                                    ) || (
                                        // or S is the far row and M is the far row
                                        Match(grid, 'S', row + (2 * i), col, rowmax, colmax) &&
                                        Match(grid, 'M', row, col + (2 * j), rowmax, colmax)
                                    ))
                                )
                                {
                                    matches++;
                                }
                            }
                        }
                    }
                }
            }
            // every X-MAS should be found twice, once for each M
            return (matches / 2).ToString();
        }
        private bool Match(string[] grid, char chr, int row, int col, int rowmax, int colmax)
        {
            return (0 <= row) && // upper row valid
                   (row < rowmax) && // lower row valid
                   (0 <= col) && // left col valid
                   (col < colmax) && // right col valid
                   grid[row][col] == chr; // cell has desired value
        }
    }
}