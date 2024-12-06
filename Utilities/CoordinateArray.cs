namespace AdventOfCode2024.Utilities
{
    public class CoordinateArray
    {
        public Coordinate[,] Array { get; init; }
        public int Height { get; init; }
        public int Width { get; init; }

        public CoordinateArray(string characterGrid) 
            : this(characterGrid.Split(Environment.NewLine)
                  .Where(line => !string.IsNullOrWhiteSpace(line))
                  .ToArray()) { }

        public CoordinateArray(string[] characterLines)
        {
            Height = characterLines.Length;
            Width = characterLines[0].Length;

            Array = new Coordinate[Width, Height];
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    char c = characterLines[row][col];
                    Array[col, row] = new Coordinate(col, row, c);
                }
            }
        }

        public CoordinateArray(int height, int width)
        {
            Height = height;
            Width = width;
            Array = new Coordinate[width, height];
        }

        public Coordinate? GetPoint(int x, int y) =>
            WithinArray(x, y) ? Array[x, y] : null;

        public bool WithinArray(Coordinate c) =>
            WithinArray(c.X, c.Y);

        public bool WithinArray(int x, int y) =>
            x < Width && x >= 0
         && y < Height && y >= 0;

        public string Print(int? maxHeight = null, int? maxWidth = null)
        {
            maxHeight ??= 140; maxWidth ??= 50;

            string print = "   ";
            for (int x = 0; x < Width && x < maxWidth; x++)
            {
                print += $"{x,3} ";
                if (x == maxWidth - 1)
                    print += $"... ";
            }
            print += '\n';

            for (int i = 0; i < Height && i < maxHeight; i++)
            {
                string newLine = $"{i,3}  ";
                for (int j = 0; j < Width && j < maxWidth; j++)
                {
                    newLine += $"{Array[j, i].C}   ";
                    if (j == maxHeight - 1)
                        print += $"... ";
                }
                newLine += '\n';
                print += newLine;
            }
            return print;
        }

        public CoordinateArray Clone()
        {
            string cloned = string.Empty;
            for (int row = 0; row < Height; row++)
            {
                string cloneRow = string.Empty;
                for (int col = 0; col < Width; col++)
                {
                    cloneRow += Array[col, row].C;
                }
                cloned += cloneRow + "\r\n";
            }
            return new CoordinateArray(cloned);
        }
    }

    public record Coordinate(int X, int Y, char C)
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
