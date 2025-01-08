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

                    // cells to move
                    List<Coordinate> move = [];
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
                        // find the next frontier
                        foreach (var coord in thisFrontier)
                        {
                            var nextNext = NextCell(warehouse, movement, coord);
                            var nextValue = nextNext.Value;
                            var nextLoc = nextNext.Loc;
                            switch (nextValue)
                            {
                                case 'O':
                                    spaceAhead = false;
                                    move.Add(nextLoc);
                                    nextFrontier.Add(nextLoc);
                                    break;
                                case '[':
                                    spaceAhead = false;
                                    // when travelling right, don't include the left side of the box in the frontier
                                    if (movement != Direction.Right)
                                    {
                                        nextFrontier.Add(nextLoc);
                                    }
                                    var rightHalf = new Coordinate(nextLoc.row, nextLoc.col + 1);
                                    nextFrontier.Add(rightHalf);
                                    move.Add(nextLoc);
                                    break;
                                case ']':
                                    spaceAhead = false;
                                    // when travelling left, don't include the right side of the box in the frontier
                                    if (movement != Direction.Left)
                                    {
                                        nextFrontier.Add(nextLoc);
                                    }
                                    var leftHalf = new Coordinate(nextLoc.row, nextLoc.col - 1);
                                    nextFrontier.Add(leftHalf);
                                    move.Add(leftHalf);
                                    break;
                                case '#':
                                    spaceAhead = false;
                                    wallHit = true;
                                    break;
                            }
                        }
                        // shift frontiers
                        thisFrontier = nextFrontier;
                        nextFrontier = [];
                    }
                    // move the boxes!
                    if (spaceAhead)
                    {
                        // move robot forward
                        currentLoc = nextCell.Loc;
                        // clear old cells
                        move.ForEach(box =>
                        {
                            warehouse.Set(box, '.');
                            if (wide)
                            {
                                // clear the other half of the box as well
                                warehouse.Set(box.row, box.col + 1, '.');
                            }
                        });

                        // write new ones
                        move.ForEach(box =>
                        {
                            var next = NextCell(warehouse, movement, box);
                            next.Value = wide ? '[' : 'O';
                            if (wide)
                            {
                                // add the other half
                                warehouse.Set(next.Row, next.Col + 1, ']');
                            }
                        });
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

/* Original part 1 implementation

var currentLoc = initLoc.Clone();
foreach (Direction movement in movements)
{
    var nextCell = NextCell(warehouse, movement, currentLoc);
    if (nextCell == null || nextCell.Value == '#')
    {
        continue;
    }
    if (nextCell.Value == '.')
    {
        currentLoc = nextCell.Loc;
    }
    else if (nextCell.Value == 'O')
    {
        // check if boxes can be pushed
        bool canMove = false;
        List<Coordinate> newLocs = [];
        GridCell<char>? cell = nextCell;
        while (!canMove)
        {
            cell = NextCell(warehouse, movement, cell.Loc);
            // give up if a wall is encountered
            if (cell == null || cell.Value == '#')
                break;
            // store the location for pushing into
            newLocs.Add(cell.Loc);
            // when a free space is found, the boxes can be moved
            canMove = cell.Value == '.';
        }
        // number of boxes = number of spaces, boxes can be pushed
        if (canMove)
        {
            // move robot forward
            currentLoc = nextCell.Loc;
            // replace robot's new position with empty space
            nextCell.Value = '.';
            // shift boxes
            newLocs.ForEach(loc => warehouse.Set(loc, 'O'));
        }
    }
}
*/
