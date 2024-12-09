using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2024
{
    public class AdventOfCode
    {
        public static void Main(string[] args)
        {
            // create instances of classes implementing Solution
            Type solutionType = typeof(Solution);
            Solution[] solutions = solutionType
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(solutionType))
                .OrderBy(t => t.Name)
                .Select(t => (Solution)Activator.CreateInstance(t)!)
                .ToArray();

            Console.Write("Enter day to run (1-25) or \"ALL\" (leave blank for current day): ");
            string? dayOption = Console.ReadLine();
            switch (dayOption)
            {
                case null:
                    Console.WriteLine("ERROR: ReadLine failed");
                    break;
                case "":
                    int today = DateTime.Today.Day - 1;
                    RunDay(today, solutions[today]);
                    break;
                case "all":
                case "ALL":
                    for (int n = 0; n < solutions.Length; n++)
                    {
                        RunDay(n, solutions[n]);
                    }
                    break;
                default:
                    if (int.TryParse(dayOption, out int i))
                    {
                        i--; // convert day number to index
                        if (i >= 0 && i < solutions.Length)
                        {
                            RunDay(i, solutions[i]);
                        }
                        else
                        {
                            Console.WriteLine("Day number out of range");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                    break;
            }
        }

        public static void RunDay(int n, Solution solution)
        {
            string day = (n + 1).ToString();
            Console.WriteLine("=== Day " + day + " ===");
            // there must be a better way to do this
            string inputDirectory = @"..\..\..\input";
            string input;
            try
            {
                input = File.ReadAllText(Path.Combine(inputDirectory, "day" + day + ".txt"));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: Could not find input file for this day");
                return;
            }
            Console.Write("Part 1: ");
            Console.WriteLine(solution.Part1(input));
            Console.Write("Part 2: ");
            Console.WriteLine(solution.Part2(input));
        }
    }
}
