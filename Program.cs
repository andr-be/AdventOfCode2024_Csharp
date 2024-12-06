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
        ];
        
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--single")
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
