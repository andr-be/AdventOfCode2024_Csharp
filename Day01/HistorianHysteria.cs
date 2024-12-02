using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day01
{
    public class HistorianHysteria : ISolution
    {
        public string Name => "Day 01: Historian Hysteria";
        public string PuzzleInput => File.ReadAllText("C:\\Users\\Ben\\source\\repos\\AdventOfCode2024\\Day01\\PuzzleInput");
        private string TestInput => File.ReadAllText("C:\\Users\\Ben\\source\\repos\\AdventOfCode2024\\Day01\\TestInput");

        public string Solve(Part part)
        {
            if (part is Part.One)
            {
                var (left, right) = SplitInputIntoSortedLists(PuzzleInput);
                var solution = FindDistancesAndSum(left, right);
                return $"{solution}";
            }
            
            else return "";
        }

        public string Test(Part part)
        {
            if (part is Part.One)
            {
                var (leftList, rightList) = SplitInputIntoSortedLists(TestInput);
                var solution = FindDistancesAndSum(leftList, rightList);
                return $"{solution}";
            }

            else return "";
        }

        private (List<int> left, List<int> right) SplitInputIntoSortedLists(string input)
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

        private int FindDistancesAndSum(List<int> left, List<int> right) =>
            left.Zip(right, (l, r) => Math.Abs(l - r)).Sum();
    }
}
