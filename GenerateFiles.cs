using System;
using System.IO;

class GenerateFiles
{
    private static string Template(string dayNumber)
    {
        return $@"namespace AdventOfCode2024
{{
    internal class Day{dayNumber} : Solution
    {{
        public Day{dayNumber}(string input)
            : base(input) {{ }}

        public override string Part1()
        {{
            return ""Not implemented"";
        }}

        public override string Part2()
        {{
            return ""Not implemented"";
        }}
    }}
}}
";
    }

    public static void Main(string[] args)
    {
        Console.WriteLine(args.Length);
        string outDir = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        if (
            args.Length < 2
            || !int.TryParse(args[1], out int startDay)
            || startDay < 1
            || startDay > 25
        )
        {
            startDay = 1;
        }
        Console.WriteLine($"Generating templates in \"{outDir}\" starting at Day {startDay}");
        for (int i = startDay; i <= 25; i++)
        {
            string dayNumber = i.ToString().PadLeft(2, '0');
            string template = Template(dayNumber);
            string filePath = Path.Combine(outDir, $"Day{dayNumber}.cs");

            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                outputFile.Write(template);
            }

            Console.WriteLine($"Wrote {filePath}");
        }
        Console.WriteLine("Templates generated successfully!");
    }
}
