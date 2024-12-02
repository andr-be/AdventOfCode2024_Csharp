namespace AdventOfCode2024.Day01
{
    public class HistorianHysteria : ISolution
    {
        public string Name => "Day 01: Historian Hysteria";
        public string PuzzleInput => File.ReadAllText("C:\\Users\\Ben\\source\\repos\\AdventOfCode2024\\Day01\\PuzzleInput");
        public string TestInput => File.ReadAllText("C:\\Users\\Ben\\source\\repos\\AdventOfCode2024\\Day01\\TestInput");

        public string Solve(Part part) => part switch
        {
            Part.One => Solution(PuzzleInput, Part.One),
            Part.Two => Solution(PuzzleInput, Part.Two),
            _ => "N/A"
        };

        public string Test(Part part) => part switch
        {
            Part.One => Solution(TestInput, Part.One),
            Part.Two => Solution(TestInput, Part.Two),
            _ => "N/A"
        };

        private string Solution(string input, Part part)
        {
            var (left, right) = SplitInputIntoSortedLists(input);

            return part switch
            {
                Part.One => $"{FindDistancesAndSum(left, right)}",
                Part.Two => $"{FindSimilarityScoreAndSum(left, right)}",
                _ => "N/A"
            };
        }

        private static (List<int> left, List<int> right) SplitInputIntoSortedLists(string input)
        {
            List<int> leftList = [];
            List<int> rightList = [];

            foreach (string line in input.Split('\n'))
            {
                if (string.IsNullOrEmpty(line)) continue;
                var split = line.Split(' ');
                var leftInt = int.Parse(split.First());
                var rightInt = int.Parse(split.Last());
                leftList.Add(leftInt);
                rightList.Add(rightInt);
            }

            leftList.Sort();
            rightList.Sort();

            return (leftList, rightList);
        }

        private static int FindDistancesAndSum(List<int> left, List<int> right) =>
            left.Zip(right, (l, r) => Math.Abs(l - r)).Sum();

        private static int FindSimilarityScoreAndSum(List<int> left, List<int> right)
        {
            var score = 0;
            foreach (var number in left)
            {
                var count = right.FindAll(x => x == number).Count;
                score += number * count;
            }
            return score;
        }
    }
}
