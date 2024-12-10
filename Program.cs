namespace AdventOfCode2024
{
    internal class Program
    {
        private static List<ISolution> solutions =
        [
            new Day01.HistorianHysteria(),
            new Day02.RedNosedReports(),
            new Day03.MullItOver(),
            new Day04.CeresSearch(),
            new Day05.PrintQueue(),
            new Day06.GuardGallivant(),
            new Day07.BridgeRepair(),
            new Day08.ResonantCollinearity(),
            new Day09.DiskFragmenter(),
        ];

        static void Main(string[] args)
        {
            if (args.Contains("--benchmark"))
            {
                BenchmarkRunner.RunBenchmarks(solutions, args.Contains("--include-test"));
                return;
            }

            if (args.Contains("--single"))
            {
                var solution = solutions.LastOrDefault();
                Console.WriteLine(solution?.Name);

                Console.WriteLine("Part 1:");
                Console.WriteLine(solution?.Test(Part.One));
                Console.WriteLine(solution?.Solve(Part.One));

                Console.WriteLine("Part 2:");
                Console.WriteLine(solution?.Test(Part.Two));
                Console.WriteLine(solution?.Solve(Part.Two));
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