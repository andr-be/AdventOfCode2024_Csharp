using AdventOfCode2024.Day01;
using AdventOfCode2024.Day02;

namespace AdventOfCode2024
{
    internal class Program
    {
        private static List<ISolution> solutions =
        [
            new HistorianHysteria(),
            new RedNosedReports(),
        ];
        
        static void Main(string[] args)
        {
            foreach (var solution in solutions)
            {
                Console.WriteLine(solution.Name);
                Console.WriteLine("Part 1:");
                Console.WriteLine(solution.Test(Part.One));
                Console.WriteLine(solution.Solve(Part.One));

                Console.WriteLine("Part 2:");
                Console.WriteLine(solution.Test(Part.Two));
                Console.WriteLine(solution.Solve(Part.Two));
                Console.WriteLine('\n');
            }
        }
    }
}
