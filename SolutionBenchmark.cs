using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AdventOfCode2024
{
    public class SolutionBenchmark
    {
        private readonly ISolution _solution;

        [ParamsAllValues]
        public Part Part { get; set; }

        [Params(true, false)]
        public bool UseTestData { get; set; }

        public SolutionBenchmark()
        {
            // Default constructor needed for BenchmarkDotNet
        }

        public SolutionBenchmark(ISolution solution)
        {
            _solution = solution;
        }

        [Benchmark]
        public string RunSolution() =>
            UseTestData ? _solution.Test(Part) : _solution.Solve(Part);
    }

    public static class BenchmarkRunner
    {
        public static void RunBenchmarks(IEnumerable<ISolution> solutions, bool includeTestData = false)
        {
            foreach (var solution in solutions)
            {
                Console.WriteLine($"\nBenchmarking {solution.Name}");

                // Quick run with Stopwatch first
                RunQuickBenchmark(solution, Part.One, includeTestData);
                RunQuickBenchmark(solution, Part.Two, includeTestData);

                // Detailed benchmarks with BenchmarkDotNet
                var config = new BenchmarkDotNet.Configs.ManualConfig()
                    .WithOptions(BenchmarkDotNet.Configs.ConfigOptions.DisableOptimizationsValidator);

                var benchmark = new SolutionBenchmark(solution);
                BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(SolutionBenchmark), config);
            }
        }

        private static void RunQuickBenchmark(ISolution solution, Part part, bool includeTestData)
        {
            if (includeTestData)
            {
                Console.WriteLine($"\nPart {(int)part} (Test Data) - Quick Benchmark:");
                RunSingleQuickBenchmark(() => solution.Test(part));
            }

            Console.WriteLine($"\nPart {(int)part} (Puzzle Data) - Quick Benchmark:");
            RunSingleQuickBenchmark(() => solution.Solve(part));
        }

        private static void RunSingleQuickBenchmark(Func<string> action)
        {
            // Warmup
            action();

            var sw = Stopwatch.StartNew();
            var iterations = 10;

            for (int i = 0; i < iterations; i++)
                action();

            sw.Stop();

            Console.WriteLine($"Average time over {iterations} runs: {sw.ElapsedMilliseconds / (double)iterations:F2}ms");
            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }
    }
}