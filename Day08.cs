using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024
{
    internal class Day08 : Solution
    {
        Dictionary<char, (int y, int x)[]> freqAntennae = [];

        int maxX;
        int maxY;
        bool parsed = false;

        public override string Part1(string input)
        {
            return Solve(input, false);
        }

        public override string Part2(string input)
        {
            return Solve(input, true);
        }

        private string Solve(string input, bool resonantHarmonics)
        {
            if (!parsed)
            {
                Parse(input);
            }

            bool InMapRange((int x, int y) p) => (p.x >= 0 && p.x < maxX && p.y >= 0 && p.y < maxY);

            HashSet<(int, int)> antinodes = [];

            foreach (var freq in freqAntennae.Keys)
            {
                var antennae = freqAntennae[freq];
                // create all antenna pairs
                for (int i = 0; i < antennae.Length; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        var (iy, ix) = antennae[i];
                        var (jy, jx) = antennae[j];
                        if (resonantHarmonics)
                        {
                            // for part 2 the antenna are always antinodes
                            antinodes.Add((ix, iy));
                            antinodes.Add((jx, jy));
                        }
                        // get a_i -> a_j vector
                        (int dx, int dy) = (ix - jx, iy - jy);
                        // find antinodes
                        (int x, int y) ai = (ix + dx, iy + dy);
                        while (InMapRange(ai))
                        {
                            antinodes.Add(ai);
                            ai = (ai.x + dx, ai.y + dy);
                            if (!resonantHarmonics)
                            {
                                break;
                            }
                        }
                        (int x, int y) aj = (jx - dx, jy - dy);
                        while (InMapRange(aj))
                        {
                            antinodes.Add(aj);
                            aj = (aj.x - dx, aj.y - dy);
                            if (!resonantHarmonics)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return antinodes.Count.ToString();
        }

        private void Parse(string input)
        {
            string[] split = input.Split(
                '\n',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            );
            // mapping of frequency -> array of antenna positions
            freqAntennae = split
                .SelectMany((s, r) => s.Select((i, c) => (i, pos: (r, c)))) // convert grid to (char, (row, column))
                .GroupBy(x => x.i, x => x.pos) // group by char
                .Where(x => x.Key != '.') // remove empty spaces
                .ToDictionary(x => x.Key, x => x.ToArray()); // convert to dictionary
            maxX = split.Length;
            maxY = split[0].Length;
            parsed = true;
        }
    }
}
