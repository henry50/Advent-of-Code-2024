﻿using System;
using System.Collections.Generic;

namespace AdventOfCode2024
{
    internal class Day01 : Solution
    {
        readonly List<int> left = [];
        readonly List<int> right = [];

        public Day01(string input)
            : base(input)
        {
            foreach (var line in input.Split('\n'))
            {
                string[] cols = line.Split("   ");
                if (cols.Length == 2)
                {
                    left.Add(int.Parse(cols[0]));
                    right.Add(int.Parse(cols[1]));
                }
            }
        }

        public override string Part1()
        {
            left.Sort();
            right.Sort();
            int total = 0;
            for (int i = 0; i < left.Count; i++)
            {
                total += Math.Abs(left[i] - right[i]);
            }
            return total.ToString();
        }

        public override string Part2()
        {
            int total = 0;
            int multiplier = 0;
            foreach (var l in left)
            {
                foreach (var r in right)
                {
                    if (l == r)
                    {
                        multiplier++;
                    }
                }
                total += (l * multiplier);
                multiplier = 0;
            }
            return total.ToString();
        }
    }
}
