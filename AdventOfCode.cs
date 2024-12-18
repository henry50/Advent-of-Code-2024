using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    public class AdventOfCode
    {
        public static int Main(string[] args)
        {
            // get classes implementing Solution
            Type solutionType = typeof(Solution);
            Type[] solutionTypes = solutionType
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(solutionType))
                .OrderBy(t => t.Name)
                .ToArray();
            int solutionCount = solutionTypes.Length;
            // check solutions exist
            if (solutionCount == 0)
            {
                Console.Error.WriteLine(
                    "ERROR: Couldn't find any classes that implement Solution, exiting..."
                );
                return 1;
            }
            // ask which days to run
            IEnumerable<int> daysToRun = [];
            bool useExample;
            while (true)
            {
                Console.Write(
                    $"Enter day to run (1-{solutionCount}) or \"ALL\" (leave blank for current day, append \"!\" to use example input): "
                );
                string? input = Console.ReadLine();
                if (input == null)
                {
                    Console.Error.WriteLine("ERROR: ReadLine failed");
                    return 1;
                }
                // validate input
                Regex validInput = new Regex(@"^(\d+|all)?(!?)$", RegexOptions.IgnoreCase);
                Match match = validInput.Match(input);
                if (match.Success)
                {
                    useExample = match.Groups[2].Value == "!";
                    string dayChoice = match.Groups[1].Value;
                    if (dayChoice == "all")
                    {
                        daysToRun = Enumerable.Range(0, solutionCount);
                        break;
                    }
                    // try and parse the number, failing that use today's date
                    int tryDay = int.TryParse(dayChoice, out tryDay) ? tryDay : DateTime.Today.Day;
                    // check the value is in range
                    if (tryDay > 0 && tryDay <= solutionCount)
                    {
                        daysToRun = [tryDay - 1];
                        break;
                    }
                    else
                    {
                        Console.Error.WriteLine("ERROR: Selected day not in range");
                    }
                }
                else
                {
                    Console.Error.WriteLine("ERROR: Invalid input");
                }
            }
            // run selected solution
            foreach (int day in daysToRun)
            {
                RunDay(day, solutionTypes[day], useExample);
            }
            return 0;
        }

        public static void RunDay(int n, Type solutionType, bool example)
        {
            string day = (n + 1).ToString();
            Console.WriteLine("=== Day " + day + " ===");
            string inputDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                example ? "example" : "input"
            );
            string input;
            try
            {
                input = File.ReadAllText(Path.Combine(inputDirectory, "day" + day + ".txt"));
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine("ERROR: Could not find input file for this day");
                return;
            }
            Solution solution = (Solution)Activator.CreateInstance(solutionType, input)!;
            Console.Write("Part 1: ");
            Console.WriteLine(solution.Part1());
            Console.Write("Part 2: ");
            Console.WriteLine(solution.Part2());
        }
    }
}
