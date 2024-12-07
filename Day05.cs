
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AdventOfCode2024
{
    internal class Day05 : Solution
    {
        int part1 = 0;
        int part2 = 0;
        bool solved = false;
        public override string Part1(string input)
        {
            Solve(input);
            return part1.ToString();
        }
        public override string Part2(string input)
        {
            Solve(input);
            return part2.ToString();
        }
        private void Solve(string input)
        {
            if (solved)
            {
                return;
            }
            // parse input
            string[] parts = input.Split(["\r\n\r\n", "\r\r", "\n\n"], StringSplitOptions.TrimEntries);
            int[][] rules = parts[0].Split('\n').Select(x => x.Split('|').Select(int.Parse).ToArray()).ToArray();
            int[][] pagesList = parts[1].Split('\n').Select(x => x.Split(',').Select(int.Parse).ToArray()).ToArray();

            // check each page
            foreach (int[] pages in pagesList)
            {
                // get only the rules that apply to these pages
                int[][] relevantRules = rules.Where(x => pages.Contains(x[0]) && pages.Contains(x[1])).ToArray();
                // sort pages by rules
                int[] sorted = pages.OrderBy(x => x, new RuleComparer(relevantRules)).ToArray();
                // get middle page
                int middle = sorted[sorted.Length / 2];
                // check the pages are in the correct order
                if (sorted.SequenceEqual(pages))
                {
                    part1 += middle;
                }
                else
                {
                    part2 += middle;
                }
            }
            solved = true;
        }
    }
    class RuleComparer(int[][] rules) : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            int[]? rule = rules.FirstOrDefault(r => r[0] == x && r[1] == y);
            return rule == null ? 1 : -1;
        }
    }
}

/* Part 2 attempt (I didn't realise the rules weren't universal, I also didn't think there would be a rule for every pair)
        public void Solve(string input)
        {
            \/* Solution idea:
             *   Create mapping of (Page number) : (All first items of rules in which Page number is second item)
             *   The "weakest" item won't be the second item of any rule (it never appears on the right hand side
             *     of any rule, in the mapping it will have an empty set)
             *   Remove the weakest item from the mapping
             *   Add the weakest item to another mapping of (Page number) : ("strength rank")
             *   Continue the above until there are no items left
             *\/
        // if the problem has already been solved then return
        if (solved)
        {
            return;
        }
        // parse input
        string[] parts = input.Split(["\r\n\r\n", "\r\r", "\n\n"], StringSplitOptions.TrimEntries);
        int[][] rules = parts[0].Split('\n').Select(x => x.Split('|').Select(int.Parse).ToArray()).ToArray();
        int[][] pages = parts[1].Split('\n').Select(x => x.Split(',').Select(int.Parse).ToArray()).ToArray();

        var lastFirstMap = rules
            // get distinct page numbers from rules
            .SelectMany(x => x)
            .Distinct()
            // create mapping (pageNum : {pageNums which appear on left hand side of pageNum in rules})
            .ToDictionary(
                x => x,
                x => rules
                    .Where(y => y[1] == x)
                    .Select(y => y[0])
                    .ToHashSet()
            );



        // remove empty last set until all page numbers are ordered
        int idx = 0;
        Dictionary<int, int> indexMap = [];
        for (int i = 0; i < lastFirstMap.Count; i++)
        {
        loop:
            foreach (int k in lastFirstMap.Keys)
            {
                // if there are no rules in which this page number appears on the right
                if (lastFirstMap[k].Count == 0)
                {
                    Console.WriteLine("now removing " + k);
                    // add it to the index map
                    indexMap[k] = idx++;
                    // remove it from this map
                    lastFirstMap.Remove(k);
                    // remove it from all existing sets
                    foreach (int kk in lastFirstMap.Keys)
                    {
                        lastFirstMap[kk].Remove(k);
                    }
                    // continue
                    Console.WriteLine("about to break");
                    goto loop;
                }
            }
            Console.WriteLine(i);
        }
        Console.WriteLine("immediately quit loop");
        Console.WriteLine(String.Join(',', indexMap.Keys));

        // sort each page list according to indexMap
        foreach (int[] page in pages)
        {
            int[] sorted = page.OrderBy(x => indexMap[x]).ToArray();
            // add the middle item to the relevant part
            int middle = sorted[sorted.Length / 2];
            if (page.SequenceEqual(sorted))
            {
                part1 += middle;
            }
            else
            {
                part2 += middle;
            }
        }
        solved = true;
    }
*/