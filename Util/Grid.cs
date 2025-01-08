using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.Util
{
    internal class Grid<T> : IEnumerable<IEnumerable<GridCell<T>>>
    {
        private readonly T[][] cells;
        public int Rowmax { get; private set; }
        public int Colmax { get; private set; }

        public Grid(string grid, Func<char, T> parser)
        {
            string[] rows = grid.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            Rowmax = rows.Length;
            Colmax = rows[0].Length;
            cells = rows.Select((x, r) => x.Select((y, c) => parser(y)).ToArray()).ToArray();
        }

        public Grid(string grid)
            : this(grid, x => (T)(object)x) { }

        public Grid(T[][] cells)
        {
            this.cells = cells;
            Rowmax = cells.Length;
            Colmax = cells[0].Length;
        }

        public Grid(IEnumerable<IEnumerable<T>> cells)
        {
            this.cells = cells.Select(x => x.ToArray()).ToArray();
            Rowmax = cells.Count();
            Colmax = cells.First().Count();
        }

        public Grid(T item, int rowmax, int colmax)
        {
            cells = Enumerable
                .Range(0, rowmax)
                .Select(x => Enumerable.Range(0, colmax).Select(y => item).ToArray())
                .ToArray();
            this.Rowmax = rowmax;
            this.Colmax = colmax;
        }

        public bool InGridRange(int row, int col)
        {
            return row >= 0 && row < Rowmax && col >= 0 && col < Colmax;
        }

        public bool InGridRange(Coordinate loc)
        {
            return InGridRange(loc.row, loc.col);
        }

        public GridCell<T> UnsafeGet(int row, int col)
        {
            // may throw IndexOutOfRangeException
            return new GridCell<T>(this, row, col);
        }

        public GridCell<T> UnsafeGet(Coordinate loc)
        {
            return UnsafeGet(loc.row, loc.col);
        }

        public bool TryGetCell(int row, int col, out GridCell<T>? cell)
        {
            if (InGridRange(row, col))
            {
                cell = new GridCell<T>(this, row, col);
            }
            else
            {
                cell = null;
            }
            return cell != null;
        }

        public bool TryGetCell(Coordinate loc, out GridCell<T>? cell)
        {
            return TryGetCell(loc.row, loc.col, out cell);
        }

        public GridCell<T>? Get(int row, int col)
        {
            if (InGridRange(row, col))
            {
                return new GridCell<T>(this, row, col);
            }
            return null;
        }

        public GridCell<T>? Get(Coordinate loc)
        {
            return Get(loc.row, loc.col);
        }

        public T UnsafeGetValue(int row, int col)
        {
            return cells[row][col];
        }

        public T UnsafeGetValue(Coordinate loc)
        {
            return cells[loc.row][loc.col];
        }

        public bool TryGetValue(int row, int col, out T? value)
        {
            if (InGridRange(row, col))
            {
                value = cells[row][col];
            }
            else
            {
                value = default;
            }
            return value != null;
        }

        public bool TryGetValue(Coordinate loc, out T? value)
        {
            return TryGetValue(loc.row, loc.col, out value);
        }

        public void Set(int row, int col, T value)
        {
            cells[row][col] = value;
        }

        public void Set(Coordinate loc, T value)
        {
            cells[loc.row][loc.col] = value;
        }

        public IEnumerable<GridCell<T>> GetFlattened()
        {
            return cells.SelectMany((x, r) => x.Select((_, c) => UnsafeGet(r, c)));
        }

        public IEnumerable<IEnumerable<GridCell<T>>> GetCells()
        {
            return cells.Select((x, r) => x.Select((_, c) => UnsafeGet(r, c)));
        }

        public IEnumerable<IEnumerable<T>> GetCellValues()
        {
            return cells;
        }

        public GridCell<T>? Find(T value)
        {
            for (int r = 0; r < Rowmax; r++)
            {
                for (int c = 0; c < Colmax; c++)
                {
                    if (TryGetValue(r, c, out var cell) && cell != null && cell.Equals(value))
                    {
                        return UnsafeGet(r, c);
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
                    if (TryGetValue(r, c, out var cell) && cell != null && cell.Equals(value))
                    {
                        yield return UnsafeGet(r, c);
                    }
                }
            }
            yield break;
        }

        public IEnumerable<GridCell<T>> GetNeighbours(int row, int col, bool includeDiagonals)
        {
            // When includeNull = false, InGridRange check ensures non-null
            return GetNeighbours(row, col, includeDiagonals, false)!;
        }

        public IEnumerable<GridCell<T>> GetNeighbours(Coordinate loc, bool includeDiagonals)
        {
            return GetNeighbours(loc.row, loc.col, includeDiagonals);
        }

        public IEnumerable<GridCell<T>?> GetNeighbours(
            int row,
            int col,
            bool includeDiagonals,
            bool includeNull = false
        )
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (
                        (i == 0 || j == 0 || includeDiagonals) // diagonal check
                        && !(i == 0 && j == 0) // self check
                        && (TryGetCell(row + i, col + j, out var cell) || includeNull)
                    )
                    {
                        yield return cell;
                    }
                }
            }
            yield break;
        }

        public IEnumerable<GridCell<T>?> GetNeighbours(
            Coordinate loc,
            bool includeDiagonals,
            bool includeNull = false
        )
        {
            return GetNeighbours(loc.row, loc.col, includeDiagonals, includeNull);
        }

        public IEnumerable<Coordinate> GetCoords()
        {
            return Enumerable
                .Range(0, Rowmax)
                .SelectMany(r => Enumerable.Range(0, Colmax).Select(c => new Coordinate(r, c)));
        }

        public override string ToString()
        {
            return string.Join(
                '\n',
                this.Select(x => string.Join(" | ", x.Select(y => y.ToString())))
            );
        }

        public Grid<T> Clone()
        {
            return new Grid<T>(cells.Select(x => x.Select(y => y).ToArray()).ToArray());
        }

        public IEnumerator<IEnumerable<GridCell<T>>> GetEnumerator()
        {
            return GetCells().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class GridCell<T>(Grid<T> grid, int row, int col)
    {
        private readonly Grid<T> grid = grid;
        public int Row { get; } = row;
        public int Col { get; } = col;
        public Coordinate Loc { get; } = new Coordinate(row, col);
        public T Value
        {
            get => grid.UnsafeGetValue(Loc);
            set => grid.Set(Row, Col, value);
        }

        public IEnumerable<GridCell<T>?> GetNeighbours(
            bool includeDiagonals,
            bool includeNull = false
        )
        {
            return grid.GetNeighbours(Row, Col, includeDiagonals, includeNull);
        }

        public IEnumerable<GridCell<T>> GetNeighbours(bool includeDiagonals)
        {
            return grid.GetNeighbours(Row, Col, includeDiagonals);
        }

        public override string ToString()
        {
            return string.Format("GridCell(row={0},col={1},value={2})", Row, Col, Value);
        }

        public override int GetHashCode()
        {
            // Grid cells can be uniquely identified by their coordinates
            return Loc.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is GridCell<T> cell)
            {
                return cell.Loc.Equals(Loc);
            }
            return false;
        }
    }

    class Coordinate(int row, int col)
    {
        public int row = row;
        public int col = col;

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.row + b.row, a.col + b.col);
        }

        public override string ToString()
        {
            return "(" + row + "," + col + ")";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Coordinate c)
            {
                return c.row == row && c.col == col;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(row, col);
        }

        public Coordinate Clone()
        {
            return new Coordinate(row, col);
        }
    }
}
