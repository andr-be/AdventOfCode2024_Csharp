namespace AdventOfCode2024
{
    internal abstract class SolutionBase : ISolution
    {
        public abstract string Name { get; }
        public abstract int Day { get; }

        public string PuzzleInput { get => _puzzleInput; }
        public string TestInput { get => _testInput; }

        private readonly string _baseDirectory = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName
            ?? throw new DirectoryNotFoundException("Couldn't find project root");

        private string _puzzleInput => File.ReadAllText(Path.Combine(_baseDirectory, $"Day{Day:D2}", "PuzzleInput"));
        private string _testInput => File.ReadAllText(Path.Combine(_baseDirectory, $"Day{Day:D2}", "TestInput"));

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

        public abstract string Solution(Part part, string input);
    }
}
