using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var array = SplitIntoIntArray(line);
                var safeGradient = ConsistentGradient(array, out var _);
                var safeDistances = CheckSafeDistances(array);
                if (safeGradient && safeDistances)
                {
                    Console.WriteLine($"{line} is SAFE");
                    safeCount++;
                }
                else
                {
                    var reason = (safeGradient, safeDistances) switch
                    {
                        (true, false) => "Consistent gradient; improper distances",
                        (false, true) => "Inconsistent gradient, safe distances",
                        (false, false) => "Both unsafe",
                        _ => ""
                    };
                    Console.WriteLine($"{line} is UNSAFE: {reason}");
                }
            }    
            return safeCount.ToString();
        }

        private bool CheckSafeDistances(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                var difference = Math.Abs(array[i] - array[i - 1]);
                if (difference >= 1 && difference <= 3) continue;
                else return false;
            }
            return true;
        }

        private int[] SplitIntoIntArray(string line) => 
            line.Split(' ')
                .Select(x => int.Parse(x.Trim()))
                .ToArray();

        private bool ConsistentGradient(int[] levels, out Direction direction)
        {
            direction = FindChangeDirection(levels);
            if (direction == Direction.Flat)
            {
                return false;
            }
            if (direction == Direction.Downwards)
            {
                for (int i = 1; i < levels.Length; i++)
                {
                    if (levels[i] >= levels[i - 1]) 
                        return false;
                }
            }
            if (direction == Direction.Upwards)
            {
                for (int i = 1; i < levels.Length; i++)
                {
                    if (levels[i] <= levels[i - 1])
                        return false;
                }
            }
            return true;
        }

        private Direction FindChangeDirection(int[] levels)
        {
            Direction direction = Direction.Flat;
            for (int i = 1; i < levels.Length; i++)
            {
                if (levels[i] < levels[i - 1])
                {
                    direction = Direction.Downwards;
                    break;
                }
                else if (levels[i] > levels[i - 1])
                {
                    direction = Direction.Upwards;
                    break;
                }
                else continue;
            }
            return direction;
        }

        private enum Direction
        {
            Upwards,
            Downwards,
            Flat
        }
    }
}
