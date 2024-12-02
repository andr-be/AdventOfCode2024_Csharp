namespace AdventOfCode2024.Day02
{
    internal class RedNosedReports : ISolution
    {
        public string Name => "Day 02: Red-Nosed Reports";

        public string PuzzleInput => "C:\\Users\\Ben\\source\\repos\\AdventOfCode2024\\Day02\\PuzzleInput";

        public string TestInput => "C:\\Users\\Ben\\source\\repos\\AdventOfCode2024\\Day02\\TestInput";

        public string Solve(Part part) => part switch
        {
            Part.One => Solution(Part.One, PuzzleInput),
            Part.Two => Solution(Part.Two, PuzzleInput),
            _ => "N/A"
        };

        public string Test(Part part) => part switch
        {
            Part.One => Solution(Part.One, TestInput),
            Part.Two => Solution(Part.Two, TestInput),
            _ => "N/A"
        };

        private string Solution(Part part, string input)
        {
            var puzzleInput = File.ReadAllLines(input);
            var safeCount = 0;
            foreach (var line in puzzleInput)
            {
                int[] array = SplitIntoIntArray(line);
                int[] deltas = CalculateDeltas(array);
                if (SafeSequence(deltas))
                {
                    Console.WriteLine($"{line} is safe.");
                    safeCount++;
                }
                else 
                {
                    var isUnsafe = true;
                    for (int i = 0; i < array.Length && part is Part.Two; i++)
                    {
                        var modifiedArray = array.Take(i).Concat(array.Skip(i + 1)).ToArray();
                        if (SafeSequence(CalculateDeltas(modifiedArray)))
                        {
                            safeCount++;
                            isUnsafe = false;
                            break;
                        }
                    }
                    Console.WriteLine(isUnsafe ? $"{line} is unsafe!" : $"{line} is safe with DAMPENER applied!");
                }
            }
            return safeCount.ToString();
        }

        private static bool SafeSequence(int[] deltas)
        {
            return deltas.All(d => d is <= -1 and >= -3)
                || deltas.All(d => d is >= +1 and <= +3);
        }

        private int[] CalculateDeltas(int[] array)
        {
            List<int> deltas = [];
            
            for (int i = 1; i < array.Length; i++)
                deltas.Add(array[i] - array[i - 1]);
            
            return [.. deltas];
        }

        private int[] SplitIntoIntArray(string line) => 
            line.Split(' ')
                .Select(x => int.Parse(x.Trim()))
                .ToArray();
    }
}
