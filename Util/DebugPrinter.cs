using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Util
{
    internal class DebugPrinter
    {
        public static void Print(Grid<char> charGrid)
        {
            Console.WriteLine(
                string.Join('\n', charGrid.GetCellValues().Select(x => new string(x.ToArray())))
            );
        }

        public static void Print<T>(IEnumerable<T> x)
        {
            Console.WriteLine(string.Join(", ", x));
        }

        public static void Print<T>(IEnumerable<IEnumerable<T>> x)
        {
            foreach (IEnumerable<T> row in x)
            {
                Print(row);
            }
        }
    }
}
