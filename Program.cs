using AdventOfCode2024.Day01;
using AdventOfCode2024.Day02;
using AdventOfCode2024.Day03;
using AdventOfCode2024.Day04;

namespace AdventOfCode2024
{
    internal class Program
    {
        private static List<ISolution> solutions =
        [
            new HistorianHysteria(),
            new RedNosedReports(),
            new MullItOver(),
            new CeresSearch(),
        ];
        
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--single")
            {
                var solution = solutions.LastOrDefault();
                Console.WriteLine(solution.Name);

                Console.WriteLine("Part 1:");
                Console.WriteLine(solution.Test(Part.One));
                Console.WriteLine(solution.Solve(Part.One));

                Console.WriteLine("Part 2:");
                Console.WriteLine(solution.Test(Part.Two));
                Console.WriteLine(solution.Solve(Part.Two));
            }
            else foreach (var solution in solutions)
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
