using System.Security;
using AdventOfCode2024.Utilities;

namespace AdventOfCode2024.Day04
{
    internal class CeresSearch : SolutionBase
    {
        public override string Name => "Day 04: Ceres Search";

        public override int Day => 4;

        public override string Solution(Part part, string input)
        {
            var coordinateArray = new WordsearchArray(input);
            //Console.Write(coordinateArray.Print());

            if (part is Part.One)
            {
                var sum = coordinateArray
                    .AllXCoordinates()
                    .Sum(coordinateArray.SpellsXmas);

                return sum.ToString();
            }
            else
            {
                // should be lower than 2067!!!
                // and higher than 1583...
                var sum = coordinateArray
                    .AllACoordinates()
                    .Sum(coordinateArray.IsX_Mas);
                
                return sum.ToString();
            }
        }
    }

    internal class WordsearchArray(string characterGrid) 
        : CoordinateArray(characterGrid)
    {
        #region Part 1
        public int SpellsXmas(Coordinate c)
        {
            var count = 0;

            for (int i = 0; i < 8; i++)
            {
                if (CheckXmas(c, (Direction)i))
                {
                    count++;
                    //Console.WriteLine($"XMAS found! Starting @ {c} going {(Direction)i} ({count} found for ({c.X},{c.Y}))");
                }
            }

            return count;
        }

        public List<Coordinate> AllXCoordinates() =>
            Array.ToEnumerable<Coordinate>()
                 .Where(c => c.C is 'X')
                 .ToList();

        private enum Direction
        {
            West = 0, East = 1, North = 2, South = 3,
            NW = 4, NE = 5, SW = 6, SE = 7
        }

        private bool CheckXmasSequence(List<Coordinate> candidates)
        {
            var sequence = new string(candidates.Select(c => c.C).ToArray());
            return sequence is "XMAS";
        }

        private List<Coordinate> GetXmasCharacters(Coordinate c, Direction d)
        {
            List<Coordinate> list = [];
            switch (d)
            {
                case Direction.West:
                    for (int col = c.X; col > c.X - 4; col--)
                        if (GetPoint(col, c.Y) is Coordinate p)
                            list.Add(p);

                    break;

                case Direction.East:
                    for (int col = c.X; col < c.X + 4; col++)
                        if (GetPoint(col, c.Y) is Coordinate p)
                            list.Add(p);

                    break;

                case Direction.North:
                    for (int row = c.Y; row > c.Y - 4; row--)
                        if (GetPoint(c.X, row) is Coordinate p)
                            list.Add(p);

                    break;

                case Direction.South:
                    for (int row = c.Y; row < c.Y + 4; row++)
                        if (GetPoint(c.X, row) is Coordinate p)
                            list.Add(p);

                    break;

                case Direction.NW:
                    for (int row = c.Y, col = c.X; row > c.Y - 4 && col > c.X - 4; row--, col--)
                        if (GetPoint(col, row) is Coordinate p)
                            list.Add(p);
                    break;

                case Direction.NE:
                    for (int row = c.Y, col = c.X; row > c.Y - 4 && col < c.X + 4; row--, col++)
                        if (GetPoint(col, row) is Coordinate p)
                            list.Add(p);

                    break;

                case Direction.SW:
                    for (int row = c.Y, col = c.X; row < c.Y + 4 && col < c.X + 4; row++, col--)
                        if (GetPoint(col, row) is Coordinate p)
                            list.Add(p);

                    break;

                case Direction.SE:
                    for (int row = c.Y, col = c.X; row < c.Y + 4 && col > c.X - 4; row++, col++)
                        if (GetPoint(col, row) is Coordinate p)
                            list.Add(p);

                    break;

                default:
                    break;
            }

            return list;
        }

        private bool CheckXmas(Coordinate c, Direction d)
        {
            // If the character isn't an X, or the coordinate is out of bounds, it can't spell XMAS
            if (c.C is not 'X' || WithinArray(c) is not true) 
                return false;

            List<Coordinate> candidates = GetXmasCharacters(c, d);

            return CheckXmasSequence(candidates);
        }

        #endregion

        #region Part 2
        public List<Coordinate> AllACoordinates() =>
            Array.ToEnumerable<Coordinate>()
                 .Where(c => c.C is 'A')
                 .ToList();

        public int IsX_Mas(Coordinate c) => CheckX_Mas(c) ? 1 : 0;

        private bool CheckX_Mas(Coordinate c)
        {
            // If the character isn't an A, or the coordinate is out of bounds, it can't make an X-MAS
            if (c.C is not 'A' || WithinArray(c) is not true)
                return false;

            List<Coordinate> candidates = GetX_MasCharacters(c);

            //Console.WriteLine($"Candidate: {c}:");
            //foreach (Coordinate candidate in candidates)
                //Console.WriteLine("\t" + candidate);

            return CheckX_MasSSequence(candidates);
        }

        private bool CheckX_MasSSequence(List<Coordinate> candidates)
        {
            var sequence = new string(candidates.Select(c => c.C).ToArray());

            if (candidates.Count < 4)
            {
                //Console.WriteLine($"{sequence} is invalid length");
                return false;
            }

            if (candidates.Where(c => c.C is 'M').Count() != 2 
            ||  candidates.Where(c => c.C is 'S').Count() != 2)
            {
                //Console.WriteLine($"{sequence} contains incorrect letters");
                return false;
            }

            if (sequence is "SMMS" or "MSSM")
            {
                //Console.WriteLine($"{sequence} is MAM/SAS");
                return false;
            }

            //Console.WriteLine($"{sequence} is valid X-MAS!");

            return true;
        }

        private List<Coordinate> GetX_MasCharacters(Coordinate c)
        {
            List<(int, int)> checkCoords = [
                (c.X - 1, c.Y - 1),
                (c.X + 1, c.Y - 1),
                (c.X - 1, c.Y + 1),
                (c.X + 1, c.Y + 1),
            ];

            return checkCoords
                .Select(x => GetPoint(x.Item1, x.Item2))
                .Where(x => x is not null)
                .ToList()!;
        }
        #endregion
    }
}
