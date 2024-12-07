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
            .Where(c => c.IsValid(part is Part.Two))
            .Sum(c => (decimal)c.Result);

        return sum.ToString();
    }
}

internal class Calibration
{
    public BigInteger Result { get; }
    private BigInteger[] Operands { get; }
    private readonly string[] OperandStrings;
    private bool? _isValid;

    public Calibration(string puzzleLine)
    {
        var parts = puzzleLine.Split(':');

        Result = BigInteger.Parse(parts[0]);

        OperandStrings = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Operands = [.. OperandStrings.Select(BigInteger.Parse)];
    }

    public bool IsValid(bool partTwo) =>
        _isValid ??= CalculateValidity(partTwo);

    private bool CalculateValidity(bool includeConcatenation)
    {
        if (Operands.Length == 1)
            return Operands[0] == Result;

        var maxOps = includeConcatenation ? 3 : 2;
        var operandCount = Operands.Length - 1;

        // Instead of generating all combinations upfront,
        // try each operator sequence one at a time
        for (int i = 0; i < Math.Pow(maxOps, operandCount); i++)
        {
            var operators = new List<Operator>();
            var value = i;

            // Convert number to sequence of operators
            for (int pos = 0; pos < operandCount; pos++)
            {
                operators.Add((value % maxOps) switch
                {
                    0 => Operator.Add,
                    1 => Operator.Multiply,
                    2 => Operator.Concatenate,
                    _ => throw new ArgumentException("Invalid operator state")
                });
                value /= maxOps;
            }

            try
            {
                if (EvaluateCombination_Backwards(operators, Result))
                    return true;
            }
            catch (OverflowException)
            {
                continue;
            }
        }

        return false;
    }

    private bool EvaluateCombination(List<Operator> operators, BigInteger result)
    {
        var candidate = Operands[0];
        for (int i = 0; i < operators.Count; i++)
        {
            candidate = operators[i] switch
            {
                Operator.Add => candidate + Operands[i + 1],
                Operator.Multiply => candidate * Operands[i + 1],
                Operator.Concatenate => ConcatenateNumbers(candidate, Operands[i + 1]),
                _ => throw new ArgumentException($"Unknown operator: {operators[i]}")
            };
        }
        return candidate == result;
    }

    private bool EvaluateCombination_Backwards(List<Operator> operators, BigInteger candidate)
    {
        var result = Operands[0];
        bool valid = true;
        for (int i = Operands.Length - 1; i > 0 && valid; i--)
        {
            candidate = operators[i - 1] switch 
            { 
                Operator.Add => candidate - Operands[i],
                Operator.Multiply => DeMultiplyNumbers(candidate, Operands[i], out valid),
                Operator.Concatenate => DeconcatenateNumbers(candidate, Operands[i], out valid)
            };
        }
        return valid && result == candidate;
    }

    private BigInteger DeMultiplyNumbers(BigInteger candidate, BigInteger operand, out bool valid)
    {
        valid = candidate % operand == 0;
        return valid ? candidate / operand : 0;
    }

    private BigInteger DeconcatenateNumbers(BigInteger candidate, BigInteger operand, out bool valid)
    {
        // Find the original string for this operand
        var operandString = OperandStrings[Array.IndexOf(Operands, operand)];
        var candidateString = candidate.ToString();

        if (candidateString.Length < operandString.Length)
        {
            valid = false;
            return 0;
        }

        var endOfCandidate = candidateString[^operandString.Length..];
        valid = (endOfCandidate == operandString);

        var result = candidateString[..^operandString.Length];
        try
        {
            return BigInteger.Parse(result is not "" ? result : "0");
        }
        catch (FormatException)
        {
            valid = false;
            return 0;
        }
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