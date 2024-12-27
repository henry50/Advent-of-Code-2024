using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Day13 : Solution
    {
        readonly ClawMachine[] machines;

        public Day13(string input)
            : base(input)
        {
            Regex parser = new Regex(
                @"Button A: X\+(\d+), Y\+(\d+)\s+Button B: X\+(\d+), Y\+(\d+)\s+Prize: X=(\d+), Y=(\d+)"
            );
            machines = parser.Matches(input).Select(x => new ClawMachine(x)).ToArray();
        }

        public override string Part1()
        {
            return Solve(false);
        }

        public override string Part2()
        {
            return Solve(true);
        }

        private string Solve(bool fixError)
        {
            long total = 0;
            long error = fixError ? 10000000000000 : 0;
            foreach (var machine in machines)
            {
                /* find matrix inverse to solve simultaneous equations
                 *  (ax bx)(a) = (px) --> (a) =        1       ( by -bx)(px)
                 *  (ay by)(b)   (py) --> (b)   (ax*by)-(bx*ay)(-ay  ax)(py)
                 */
                double determinant = (machine.ax * machine.by) - (machine.bx * machine.ay);
                long errorPx = machine.px + error;
                long errorPy = machine.py + error;
                long multA = (machine.by * errorPx) + (-machine.bx * errorPy);
                long multB = (-machine.ay * errorPx) + (machine.ax * errorPy);
                double solutionA = multA / determinant;
                double solutionB = multB / determinant;
                // integer solutions = possible
                if (solutionA % 1 == 0 && solutionB % 1 == 0)
                {
                    total += (3 * (long)solutionA) + (long)solutionB;
                }
            }
            return total.ToString();
        }
    }

    class ClawMachine
    {
        public int ax;
        public int ay;
        public int bx;
        public int by;
        public int px;
        public int py;

        public ClawMachine(Match m)
        {
            var groups = m.Groups;
            ax = int.Parse(groups[1].Value);
            ay = int.Parse(groups[2].Value);
            bx = int.Parse(groups[3].Value);
            by = int.Parse(groups[4].Value);
            px = int.Parse(groups[5].Value);
            py = int.Parse(groups[6].Value);
        }
    }
}
