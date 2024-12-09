using BenchmarkDotNet.ConsoleArguments.ListBenchmarks;

namespace AdventOfCode2024.Day08;

internal class ResonantCollinearity : SolutionBase
{
    public override string Name => "Day 08: Resonant Collinearity";
    public override int Day => 8;

    public override string Solution(Part part, string input)
    {
        var antennaMap = GenerateArrayFromString(input);
        var antennaList = LocateAntennas(antennaMap);
        var frequencyList = antennaList.GroupBy(c => c.Frequency).Select(g => g.Key);

        List<Antenna> antinodes = [];
        foreach (var frequency in frequencyList)
        {
            var tunedAntennas = antennaList
                .Where(a => a.Frequency == frequency)
                .ToArray();

            var newAntinodes = Enumerable.Range(0, tunedAntennas.Length - 1)
            .SelectMany(i => Enumerable.Range(i + 1, tunedAntennas.Length - i - 1)
                .Select(j => part is Part.One
                    ? antennaMap.GenerateSimpleAntinodes(tunedAntennas[i], tunedAntennas[j])
                    : antennaMap.GenerateComplexAntinodes(tunedAntennas[i], tunedAntennas[j])
                )
            )
            .SelectMany(x => x)
            .ToList();

            antinodes.AddRange(newAntinodes);
        }

        List<(int, int)> solution = [.. antinodes
            .OrderBy(a => a.Column)
            .OrderBy(a => a.Row)
            .Select(a => (a.Row, a.Column))
            .Distinct()
        ];

        return $"{solution.Count}";
    }

    private AntennaMap GenerateArrayFromString(string input)
    {
        var splitIntoLines = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        var rowNumber = splitIntoLines.Length;
        var rowLength = splitIntoLines.First().Length;
        var array = new char[rowNumber, rowLength];

        for (int row = 0; row < rowNumber; row++)
            for (int col = 0; col < rowLength; col++)
                array[row, col] = splitIntoLines[row][col];

        return new(array, rowNumber, rowLength);
    }

    private List<Antenna> LocateAntennas(AntennaMap map)
    {
        List<Antenna> antennaList = [];

        for (int row = 0; row < map.RowCount; row++)
        {
            for (int col = 0; col < map.ColumnCount; col++)
            {
                var frequency = map.Array[row, col];

                if (frequency is not '.')
                    antennaList.Add(new (row, col, map.Array[row, col]));
            }
        }

        return antennaList;
    }

    record Antenna(int Row, int Column, char Frequency);

    record AntennaMap(char[,] Array, int RowCount, int ColumnCount)
    {
        public List<Antenna> GenerateSimpleAntinodes(Antenna a, Antenna b)
        {
            List<Antenna> antinodes = [];

            (int row, int col) = (b.Row - a.Row, b.Column - a.Column);

            Antenna first = new(a.Row - row, a.Column - col, a.Frequency);
            if (WithinArray(first.Row, first.Column)) antinodes.Add(first);

            Antenna second = new(b.Row + row, b.Column + col, a.Frequency);
            if (WithinArray(second.Row, second.Column)) antinodes.Add(second);

            return antinodes;
        }

        public List<Antenna> GenerateComplexAntinodes(Antenna a, Antenna b)
        {
            List<Antenna> antinodes = [];

            var (row, col) = (b.Row - a.Row, b.Column - a.Column);
            
            for (int i = a.Row, j = a.Column; WithinArray(i, j); i -= row, j -= col)
                antinodes.Add(new(i, j, a.Frequency));

            for (int i = b.Row, j = b.Column; WithinArray(i, j); i += row, j += col)
                antinodes.Add(new(i, j, a.Frequency));

            return antinodes;
        }

        public bool WithinArray(int row, int col) => 
            row < RowCount && row >= 0 
         && col < ColumnCount && col >= 0;
    };
}

