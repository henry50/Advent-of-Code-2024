using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2024.Util;

namespace AdventOfCode2024
{
    internal class Day15 : Solution
    {
        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        readonly Grid<char> initWarehouse;
        readonly Direction[] movements;
        readonly Coordinate initLoc;

        public Day15(string input)
            : base(input)
        {
            var split = input.Split(
                ["\r\n\r\n", "\n\n"],
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            );
            initWarehouse = new Grid<char>(split[0]);
            var robotCell = initWarehouse.Find('@')!;
            robotCell.Value = '.';
            initLoc = robotCell.Loc;
            movements = split[1]
                .ToCharArray()
                .Where(x => x != '\n' && x != '\r')
                .Select(x =>
                    x switch
                    {
                        '^' => Direction.Up,
                        'v' => Direction.Down,
                        '<' => Direction.Left,
                        '>' => Direction.Right,
                        _ => throw new Exception("Unexpected instruction " + x),
                    }
                )
                .ToArray();
        }

        public override string Part1()
        {
            var warehouse = initWarehouse.Clone();
            return Solve(warehouse, false);
        }

        public override string Part2()
        {
            // expand grid
            var doubleWarehouse = new Grid<char>(
                '.',
                initWarehouse.Rowmax,
                initWarehouse.Colmax * 2
            );
            foreach (var row in initWarehouse)
            {
                foreach (var col in row)
                {
                    doubleWarehouse.Set(col.Row, col.Col * 2, col.Value == 'O' ? '[' : col.Value);
                    doubleWarehouse.Set(
                        col.Row,
                        (col.Col * 2) + 1,
                        col.Value == 'O' ? ']' : col.Value
                    );
                }
            }
            return Solve(doubleWarehouse, true);
        }

        private static GridCell<char> NextCell(Grid<char> grid, Direction direction, Coordinate loc)
        {
            // because the map is surrounded by #, the next cell will never be out of bounds
            return direction switch
            {
                Direction.Up => grid.UnsafeGet(loc.row - 1, loc.col),
                Direction.Down => grid.UnsafeGet(loc.row + 1, loc.col),
                Direction.Left => grid.UnsafeGet(loc.row, loc.col - 1),
                Direction.Right => grid.UnsafeGet(loc.row, loc.col + 1),
                _ => throw new InvalidOperationException(),
            };
        }

        private string Solve(Grid<char> warehouse, bool wide)
        {
            var currentLoc = initLoc.Clone();
            if (wide)
            {
                currentLoc.col *= 2;
            }
            foreach (Direction movement in movements)
            {
                var nextCell = NextCell(warehouse, movement, currentLoc);
                if (nextCell.Value == '#')
                {
                    // hit a wall, cannot move
                    continue;
                }
                if (nextCell.Value == '.')
                {
                    // empty space, occupy it
                    currentLoc = nextCell.Loc;
                }
                else
                {
                    // hit a box, try to move it and any adjacent to it

                    // mapping of new value to coordinates
                    // TODO: replace this with single list, move ] based on [
                    Dictionary<char, List<Coordinate>> move = [];
                    if (wide)
                    {
                        move.Add('[', []);
                        move.Add(']', []);
                    }
                    else
                    {
                        move.Add('O', []);
                    }
                    // cells to check in front of this round
                    HashSet<Coordinate> thisFrontier = [currentLoc];
                    // cells to check in front of next round
                    HashSet<Coordinate> nextFrontier = [];

                    // keep searching until a wall is hit or there is
                    bool spaceAhead = false;
                    bool wallHit = false;
                    while (!spaceAhead && !wallHit)
                    {
                        // we assume there is all space ahead until proven otherwise
                        spaceAhead = true;
                        // get the next frontier
                        foreach (var coord in thisFrontier)
                        {
                            var nextNext = NextCell(warehouse, movement, coord);
                            var nextValue = nextNext.Value;
                            var nextLoc = nextNext.Loc;
                            // more boxes :/
                            if (nextValue == 'O' || nextValue == '[' || nextValue == ']')
                            {
                                spaceAhead = false;
                                move[nextValue].Add(nextLoc);
                                // always include non-wide boxes in the next frontier
                                if (nextValue == 'O')
                                {
                                    nextFrontier.Add(nextLoc);
                                }
                                // consider right half of box
                                else if (nextValue == '[')
                                {
                                    // when travelling right, don't include the left side of the box in the frontier
                                    if (movement != Direction.Right)
                                    {
                                        nextFrontier.Add(nextLoc);
                                    }
                                    var rightNeighbour = new Coordinate(
                                        nextLoc.row,
                                        nextLoc.col + 1
                                    );
                                    nextFrontier.Add(rightNeighbour);
                                    move[']'].Add(rightNeighbour);
                                }
                                // consider left half of box
                                else if (nextValue == ']')
                                {
                                    // when travelling left, don't include the right side of the box in the frontier
                                    if (movement != Direction.Left)
                                    {
                                        nextFrontier.Add(nextLoc);
                                    }
                                    var leftNeigbour = new Coordinate(nextLoc.row, nextLoc.col - 1);
                                    nextFrontier.Add(leftNeigbour);
                                    move['['].Add(leftNeigbour);
                                }
                            }
                            else if (nextValue == '#')
                            {
                                spaceAhead = false;
                                wallHit = true;
                            }
                        }
                        // shift frontiers
                        thisFrontier = nextFrontier;
                        nextFrontier = [];
                    }
                    // move the boxes!
                    if (spaceAhead)
                    {
                        currentLoc = nextCell.Loc;
                        // clear old cells
                        foreach (var (_, boxes) in move)
                        {
                            boxes.ForEach(box =>
                            {
                                warehouse.Set(box, '.');
                            });
                        }
                        // write new ones
                        foreach (var (value, boxes) in move)
                        {
                            boxes.ForEach(box =>
                            {
                                NextCell(warehouse, movement, box).Value = value;
                            });
                        }
                    }
                }
            }
            // all movements are complete, calculate score
            return warehouse
                .FindAll(wide ? '[' : 'O')
                .Select(x => (100 * x.Row) + x.Col)
                .Sum()
                .ToString();
        }
    }
}
