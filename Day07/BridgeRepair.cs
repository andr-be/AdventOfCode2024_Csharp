using System.Numerics;

namespace AdventOfCode2024.Day07;

internal class BridgeRepair : SolutionBase
{
    public override string Name => "Day 07: Bridge Repair";
    public override int Day => 7;

    public override string Solution(Part part, string input)
    {
        var calibrations = input
            .Split(Environment.NewLine)
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(l => new Calibration(l))
            .ToList();

        // Convert BigInteger to decimal for final sum
        decimal sum = calibrations
            .Where(c => c.IsValid(part))
            .Sum(c => (decimal)c.Result);

        return sum.ToString();
    }
}

internal class Calibration
{
    public BigInteger Result { get; }
    private BigInteger[] Operands { get; }
    private bool? _isValid;

    public Calibration(string puzzleLine)
    {
        var parts = puzzleLine.Split(':');
        Result = BigInteger.Parse(parts[0]);
        Operands = parts[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(BigInteger.Parse)
            .ToArray();
    }

    public bool IsValid(Part part) =>
        _isValid ??= CalculateValidity(part == Part.Two);

    private bool CalculateValidity(bool includeConcatenation)
    {
        if (Operands.Length == 1)
            return Operands[0] == Result;

        var combinations = GenerateAllCombinations(includeConcatenation);
        foreach (var ops in combinations)
        {
            try
            {
                if (EvaluateCombination(ops) == Result)
                    return true;
            }
            catch (OverflowException)
            {
                continue;
            }
        }

        return false;
    }

    private BigInteger EvaluateCombination(List<Operator> operators)
    {
        var result = Operands[0];
        for (int i = 0; i < operators.Count; i++)
        {
            result = operators[i] switch
            {
                Operator.Add => result + Operands[i + 1],
                Operator.Multiply => result * Operands[i + 1],
                Operator.Concatenate => ConcatenateNumbers(result, Operands[i + 1]),
                _ => throw new ArgumentException($"Unknown operator: {operators[i]}")
            };
        }
        return result;
    }

    private static BigInteger ConcatenateNumbers(BigInteger left, BigInteger right) =>
        BigInteger.Parse(left.ToString() + right.ToString());

    private IEnumerable<List<Operator>> GenerateAllCombinations(bool includeConcatenation)
    {
        var operatorCount = Operands.Length - 1;
        var states = includeConcatenation ? 3 : 2; // 3 operators if including concatenation
        var combinationCount = (int)Math.Pow(states, operatorCount);

        for (int i = 0; i < combinationCount; i++)
        {
            var combination = new List<Operator>();
            var value = i;

            for (int pos = 0; pos < operatorCount; pos++)
            {
                var op = (value % states) switch
                {
                    0 => Operator.Add,
                    1 => Operator.Multiply,
                    2 => Operator.Concatenate,
                    _ => throw new ArgumentException("Invalid operator state")
                };
                combination.Add(op);
                value /= states;
            }

            yield return combination;
        }
    }

    private enum Operator
    {
        Add,
        Multiply,
        Concatenate
    }
}