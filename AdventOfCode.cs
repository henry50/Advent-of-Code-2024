using System;
using AdventOfCode2024;

public class AdventOfCode
{
    public static void Main(string[] args)
    {
        // there must be a better way to do this
        string inputDirectory = @"..\..\..\input";
        Day1 day1 = new Day1();
        Console.WriteLine("Day 1");
        string input = File.ReadAllText(Path.Combine(inputDirectory, "day1.txt"));
        Console.Write("Part 1: ");
        Console.WriteLine(day1.Part1(input));
        Console.Write("Part 2: ");
        Console.WriteLine(day1.Part2(input));
    }
}