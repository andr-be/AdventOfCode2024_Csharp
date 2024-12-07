using System.Numerics;

namespace AdventOfCode2024.Day07;

internal class BridgeRepair : SolutionBase
{
    public override string Name => "Day 07: Bridge Repair";
    public override int Day => 7;

    public override string Solution(Part part, string input)
    {
        var calibrations = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
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
    private readonly string[] OperandStrings;
    private readonly int ResultLength;
    private bool? _isValid;

    public Calibration(string puzzleLine)
    {
        var parts = puzzleLine.Split(':');
        Result = BigInteger.Parse(parts[0]);
        ResultLength = parts[0].Length;
        OperandStrings = parts[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
        Operands = OperandStrings.Select(BigInteger.Parse).ToArray();
    }

    public bool IsValid(Part part) =>
        _isValid ??= CalculateValidity(part == Part.Two);

    private bool CalculateValidity(bool includeConcatenation)
    {
        if (Operands.Length == 1)
            return Operands[0] == Result;

        return TryFindSolution(0, Result, includeConcatenation);
    }

    private bool TryFindSolution(int operandIndex, BigInteger target, bool includeConcatenation)
    {
        if (operandIndex == Operands.Length - 1)
            return target == Operands[operandIndex];

        var current = Operands[operandIndex];
        var currentStr = OperandStrings[operandIndex];
        var targetStr = target.ToString();

        // Try addition (only if result would be smaller than target)
        if (target >= current)
            if (TryFindSolution(operandIndex + 1, target - current, includeConcatenation))
                return true;

        // Try multiplication (only if target is divisible by current)
        if (target % current == 0)
            if (TryFindSolution(operandIndex + 1, target / current, includeConcatenation))
                return true;

        // Try concatenation (only if target starts with current)
        if (includeConcatenation && targetStr.Length >= currentStr.Length)
        {
            var prefix = targetStr[..currentStr.Length];
            if (prefix == currentStr)
            {
                var remainingStr = targetStr[currentStr.Length..];
                if (remainingStr.Length > 0 && !remainingStr.StartsWith('0'))
                {
                    var remainingTarget = BigInteger.Parse(remainingStr);
                    if (TryFindSolution(operandIndex + 1, remainingTarget, includeConcatenation))
                        return true;
                }
            }
        }

        return false;
    }
}