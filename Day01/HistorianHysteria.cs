namespace AdventOfCode2024.Day01
{
    internal class HistorianHysteria : SolutionBase
    {
        public override string Name => "Day 01: Historian Hysteria";

        public override int Day => 1;

        public override string Solution(Part part, string input)
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
                leftList.Add(leftInt);

                var rightInt = int.Parse(split.Last());
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
