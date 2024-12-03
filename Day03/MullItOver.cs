using System.Text.RegularExpressions;

namespace AdventOfCode2024.Day03
{
    internal partial class MullItOver : ISolution
    {
        public string Name => "Day 03: Mull It Over";

        private readonly string _baseDirectory = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName
            ?? throw new DirectoryNotFoundException("Couldn't find project root");

        public string PuzzleInput => File.ReadAllText(Path.Combine(_baseDirectory, "Day03", "PuzzleInput"));
        public string TestInput => File.ReadAllText(Path.Combine(_baseDirectory, "Day03", "TestInput"));
        public string TestInput2 => File.ReadAllText(Path.Combine(_baseDirectory, "Day03", "TestInput2"));

        public string Solve(Part part) => part switch
        {
            Part.One => Solution(Part.One, PuzzleInput).ToString(),
            Part.Two => Solution(Part.Two, PuzzleInput).ToString(),
            _ => "N/A"
        };

        public string Test(Part part) => part switch
        {
            Part.One => Solution(Part.One, TestInput).ToString(),
            Part.Two => Solution(Part.Two, TestInput2).ToString(),
            _ => "N/A"
        };

        private int Solution(Part part, string input) => part switch
        {
            Part.One => ProcessSimpleMultiplications(input),
            Part.Two => ProcessStateAwareMultiplications(input),
            _ => throw new ArgumentException("Invalid part", nameof(part))
        };

        private int ProcessSimpleMultiplications(string input)
        {
            return input.Split(Environment.NewLine)
                .SelectMany(line => MulPattern().Matches(line))
                .Select(match => ParseMultiplication(match))
                .Sum();
        }

        private int ProcessStateAwareMultiplications(string input)
        {
            var enabledRanges = GetEnabledRanges(input);
            return MulPattern().Matches(input)
                .Where(match => IsInEnabledRange(match, enabledRanges))
                .Select(match => ParseMultiplication(match))
                .Sum();
        }

        // Extracts the logic for determining enabled ranges based on state changes
        private List<Range> GetEnabledRanges(string input)
        {
            var stateChanges = GetOrderedStateChanges(input);
            var ranges = new List<Range>();
            var startPos = 0;
            var isEnabled = true;

            foreach (var change in stateChanges)
            {
                if (isEnabled && !change.IsEnabled)
                {
                    ranges.Add(new(startPos, change.Position));
                    isEnabled = false;
                }
                else if (!isEnabled && change.IsEnabled)
                {
                    startPos = change.Position;
                    isEnabled = true;
                }
            }

            if (isEnabled)
            {
                ranges.Add(new Range(startPos, input.Length));
            }

            return ranges;
        }

        // Helper methods to improve readability
        private IOrderedEnumerable<StateChange> GetOrderedStateChanges(string input) =>
            StatePattern().Matches(input)
                .Select(match => new StateChange(match.Index, match.Value == "do()"))
                .OrderBy(x => x.Position);

        private bool IsInEnabledRange(Match match, List<Range> enabledRanges) =>
            enabledRanges.Any(range =>
                match.Index >= range.Start.Value &&
                match.Index < range.End.Value);

        private int ParseMultiplication(Match match)
        {
            var (first, second) = (
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value)
            );
            return first * second;
        }

        // Record to represent state changes more clearly
        private record StateChange(int Position, bool IsEnabled);

        [GeneratedRegex("mul\\((\\d+),(\\d+)\\)")]
        public static partial Regex MulPattern();

        [GeneratedRegex("(?:do|don't)\\(\\)")]
        public static partial Regex StatePattern();
    }
}
