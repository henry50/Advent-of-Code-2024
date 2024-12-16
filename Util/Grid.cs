using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Util
{
    internal class Grid<T>
    {
        private char[][] CharCells { get; }
        public GridCell<T>[][] Cells { get; }
        private int Rowmax { get; }
        private int Colmax { get; }

        public Grid(string grid, Func<char, T> parser)
        {
            string[] rows = grid.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            Rowmax = rows.Length;
            Colmax = rows[0].Length;
            CharCells = rows.Select(x => x.ToCharArray()).ToArray();
            Cells = CharCells
                .Select(
                    (x, r) => x.Select((y, c) => new GridCell<T>(this, r, c, parser(y))).ToArray()
                )
                .ToArray();
        }

        public Grid(string grid)
            : this(grid, x => (T)(object)x) { }

        public bool InGridRange(int row, int col)
        {
            return row >= 0 && row < Rowmax && col >= 0 && col < Colmax;
        }

        public bool InGridRange((int row, int col) loc)
        {
            return InGridRange(loc.row, loc.col);
        }

        public GridCell<T>? Find(T value)
        {
            for (int r = 0; r < Rowmax; r++)
            {
                for (int c = 0; c < Colmax; c++)
                {
                    if (Cells[r][c].Value!.Equals(value))
                    {
                        return Cells[r][c];
                    }
                }
            }
            return null;
        }

        public IEnumerable<GridCell<T>> FindAll(T value)
        {
            for (int r = 0; r < Rowmax; r++)
            {
                for (int c = 0; c < Colmax; c++)
                {
                    if (Cells[r][c].Value!.Equals(value))
                    {
                        yield return Cells[r][c];
                    }
                }
            }
            yield break;
        }

        public override string ToString()
        {
            return string.Join(
                '\n',
                Cells.Select(x => string.Join(" | ", x.Select(y => y.ToString())))
            );
        }
    }

    internal class GridCell<T>(Grid<T> grid, int row, int col, T value)
    {
        private Grid<T> grid = grid;
        public int Row { get; } = row;
        public int Col { get; } = col;
        public T Value { get; } = value;

        public IEnumerable<GridCell<T>> GetNeighbours(bool includeDiagonals)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (
                        grid.InGridRange(Row + i, Col + j) && (i == 0 || j == 0 || includeDiagonals)
                    )
                    {
                        // InGridRange check ensures non-null
                        yield return grid.Cells[Row + i][Col + j];
                    }
                }
            }
            yield break;
        }

        public override string ToString()
        {
            return string.Format("GridCell(row={0},col={1},value={2})", Row, Col, Value);
        }
    }
}
