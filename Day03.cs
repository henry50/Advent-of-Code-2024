using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day03(string input) : Solution(input)
    {
        readonly string input = input;

        public override string Part1()
        {
            int total = 0;
            Regex validMul = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");
            foreach (Match match in validMul.Matches(input))
            {
                total += (int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value));
            }
            return total.ToString();
        }

        public override string Part2()
        {
            int total = 0;
            bool ignore = false;
            Regex validMul = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|(do\(\)|don't\(\))");
            foreach (Match match in validMul.Matches(input))
            {
                if (match.Value == "do()")
                {
                    ignore = false;
                }
                else if (match.Value == "don't()")
                {
                    ignore = true;
                }
                else if (!ignore)
                {
                    total += (int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value));
                }
            }
            return total.ToString();
        }
    }
}
