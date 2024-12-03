namespace AdventOfCode2024
{
    public class AdventOfCode
    {
        public static void Main(string[] args)
        {
            // there must be a better way to do this
            string inputDirectory = @"..\..\..\input";
            List<Solution> solutions = [
                new Day1(),
                new Day2(),
                new Day3(),
            ];
            for (int i = 0; i < solutions.Count; i++)
            {
                string day = (i + 1).ToString();
                Console.WriteLine("=== Day " + day + " ===");
                string input = File.ReadAllText(Path.Combine(inputDirectory, "day" + day + ".txt"));
                Solution solution = solutions[i];
                Console.Write("Part 1: ");
                Console.WriteLine(solution.Part1(input));
                Console.Write("Part 2: ");
                Console.WriteLine(solution.Part2(input));
            }
        }
    }
}