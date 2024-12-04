using System.Security;

namespace AdventOfCode2024.Day04
{
    internal class CeresSearch : ISolution
    {
        public string Name => "Day 04: Ceres Search";

        private readonly string _baseDirectory = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName
            ?? throw new DirectoryNotFoundException("Couldn't find project root");

        public string PuzzleInput => File.ReadAllText(Path.Combine(_baseDirectory, "Day04", "PuzzleInput"));
        public string TestInput => File.ReadAllText(Path.Combine(_baseDirectory, "Day04", "TestInput"));

        public string Solve(Part part) => part switch
        {
            Part.One => Solution(Part.One, PuzzleInput).ToString(),
            Part.Two => Solution(Part.Two, PuzzleInput).ToString(),
            _ => "N/A"
        };

        public string Test(Part part) => part switch
        {
            Part.One => Solution(Part.One, TestInput).ToString(),
            Part.Two => Solution(Part.Two, TestInput).ToString(),
            _ => "N/A"
        };

        private int Solution(Part part, string input)
        {
            var coordinateArray = new CoordinateArray(input);
            Console.Write(coordinateArray.Print());

            if (part is Part.One)
            {
                var sum = coordinateArray
                    .AllXCoordinates()
                    .Sum(coordinateArray.SpellsXmas);

                return sum;
            }
            else
            {
                // should be lower than 2067!!!
                // and higher than 1583...
                var sum = coordinateArray
                    .AllACoordinates()
                    .Sum(coordinateArray.IsX_Mas);
                
                return sum;
            }
        }
    }

    internal class CoordinateArray
    {
        #region CoordinateArray
        public Coordinate[,] Array { get; init; }
        public int Height { get; init; }
        public int Width { get; init; }

        public CoordinateArray(string characterGrid)
        {
            var splitGrid = characterGrid.Split(Environment.NewLine)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            Height = splitGrid.Length;
            Width = splitGrid[0].Length;

            Array = new Coordinate[Width, Height];
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    char c = splitGrid[row][col];
                    Array[col, row] = new Coordinate(col, row, c, c is 'X');
                }
            }
        }

        public Coordinate? GetPoint(int x, int y) => 
            WithinArray(x, y) ? Array[x, y] : null;

        public bool WithinArray(Coordinate c) =>
            WithinArray(c.X, c.Y);

        public bool WithinArray(int x, int y) =>
            x < Width && x >= 0 
         && y < Height && y >= 0;

        public string Print()
        {
            string print = "   ";
            for (int x = 0; x < Width; x++)
            {
                print += $"{x,3} ";
            }
            print += '\n';
            for (int i = 0; i < Height; i++)
            {
                string newLine = $"{i,3}  ";
                for (int j = 0; j < Width; j++)
                {
                    newLine += $"{Array[j, i].C}   ";
                }
                newLine += '\n';
                print += newLine;
            }
            return print;
        }
        #endregion

        #region Part 1
        public int SpellsXmas(Coordinate c)
        {
            var count = 0;

            for (int i = 0; i < 8; i++)
            {
                if (CheckXmas(c, (Direction)i))
                {
                    count++;
                    Console.WriteLine($"XMAS found! Starting @ {c} going {(Direction)i} ({count} found for ({c.X},{c.Y}))");
                }
            }

            return count;
        }

        public List<Coordinate> AllXCoordinates() =>
            Array.ToEnumerable<Coordinate>()
                 .Where(c => c.Start)
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
            if (c.Start is not true || WithinArray(c) is not true) 
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

            Console.WriteLine($"Candidate: {c}:");
            foreach (Coordinate candidate in candidates)
                Console.WriteLine("\t" + candidate);

            return CheckX_MasSSequence(candidates);
        }

        private bool CheckX_MasSSequence(List<Coordinate> candidates)
        {
            var sequence = new string(candidates.Select(c => c.C).ToArray());

            if (candidates.Count < 4)
            {
                Console.WriteLine($"{sequence} is invalid length");
                return false;
            }

            if (candidates.Where(c => c.C is 'M').Count() != 2 
            ||  candidates.Where(c => c.C is 'S').Count() != 2)
            {
                Console.WriteLine($"{sequence} contains incorrect letters");
                return false;
            }

            if (sequence is "SMMS" or "MSSM")
            {
                Console.WriteLine($"{sequence} is MAM/SAS");
                return false;
            }

            Console.WriteLine($"{sequence} is valid X-MAS!");

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

    internal record Coordinate(int X, int Y, char C, bool Start)
    {
        public override string ToString() => $"({X},{Y}):[{C}]";
    }

    public static class ArrayExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this Array target)
        {
            foreach (var item in target)
                yield return (T)item;
        }
    }
}
