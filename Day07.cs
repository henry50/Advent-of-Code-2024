using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024
{
    internal class Day07 : Solution
    {
        readonly (long target, long[] numbers)[] equations;

        public Day07(string input)
            : base(input)
        {
            equations = input
                .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(": "))
                .Select(x =>
                    (long.Parse(x[0]), x[1].Split(' ').Select(y => long.Parse(y)).ToArray())
                )
                .ToArray();
        }

        public override string Part1()
        {
            Func<long, long, long>[] operators = [(x, y) => x + y, (x, y) => x * y];
            return Solve(operators);
        }

        public override string Part2()
        {
            Func<long, long, long>[] operators =
            [
                (x, y) => x + y,
                (x, y) => x * y,
                (x, y) => long.Parse(x.ToString() + y.ToString()),
            ];
            return Solve(operators);
        }

        private string Solve(Func<long, long, long>[] operators)
        {
            long possiblyTrue = 0;
            foreach (var (target, numbers) in equations)
            {
                List<long> lhs = [numbers[0]];
                for (int i = 1; i < numbers.Length; i++)
                {
                    List<long> newLhs = [];
                    // calculate all (lhs op numbers[i]) combinations
                    foreach (long l in lhs)
                    {
                        foreach (var op in operators)
                        {
                            newLhs.Add(op(l, numbers[i]));
                        }
                    }
                    lhs = newLhs;
                }
                if (lhs.Contains(target))
                {
                    possiblyTrue += target;
                }
            }
            return possiblyTrue.ToString();
        }
    }
}
