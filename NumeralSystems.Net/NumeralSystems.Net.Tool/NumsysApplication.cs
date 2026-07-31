using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Tool;

/// <summary>Command dispatcher for the <c>numsys</c> global tool.</summary>
public static class NumsysApplication
{
    private static readonly Regex AndConstraint = new(
        @"^\s*x\s*&\s*([01?_\s]+?)\s*=\s*([01?_\s]+?)\s*$",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    /// <summary>Runs one command and returns a process exit code.</summary>
    public static int Run(
        IReadOnlyList<string> arguments,
        TextWriter output,
        TextWriter error)
    {
        if (arguments is null) throw new ArgumentNullException(nameof(arguments));
        if (output is null) throw new ArgumentNullException(nameof(output));
        if (error is null) throw new ArgumentNullException(nameof(error));

        try
        {
            if (arguments.Count == 0 || IsHelp(arguments[0]))
            {
                WriteHelp(output);
                return 0;
            }

            return arguments[0].ToLowerInvariant() switch
            {
                "convert" => Convert(arguments, output),
                "inspect" => Inspect(arguments, output),
                "solve" => Solve(arguments, output),
                _ => Fail(error, $"Unknown command '{arguments[0]}'. Run 'numsys --help'.")
            };
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException)
        {
            return Fail(error, exception.Message);
        }
    }

    private static int Convert(IReadOnlyList<string> arguments, TextWriter output)
    {
        if (arguments.Count < 2)
            throw new ArgumentException("convert requires a value.");

        var sourceBase = ReadIntegerOption(arguments, "--from");
        var destinationBase = ReadIntegerOption(arguments, "--to");
        var sourceAlphabet = NumeralAlphabet.CreateDefault(sourceBase);
        var destinationAlphabet = NumeralAlphabet.CreateDefault(destinationBase);
        var value = sourceAlphabet.Decode(arguments[1]);

        output.WriteLine(destinationAlphabet.Encode(value));
        return 0;
    }

    private static int Inspect(IReadOnlyList<string> arguments, TextWriter output)
    {
        if (arguments.Count < 2)
            throw new ArgumentException("inspect requires a bit pattern.");

        var typeName = ReadStringOption(arguments, "--type");
        var width = WidthOf(typeName);
        var pattern = BitPattern.Parse(arguments[1]);
        if (pattern.Count != width)
            throw new ArgumentException(
                $"Type '{typeName}' requires {width} bits, but the pattern contains {pattern.Count}.");

        output.WriteLine($"Pattern: {pattern}");
        output.WriteLine($"Type: {typeName.ToLowerInvariant()} ({width} bits)");
        output.WriteLine($"Unknown bits: {pattern.UnknownBitCount}");
        output.WriteLine($"Candidates: {pattern.CandidateCount}");
        output.WriteLine($"Unsigned range: {pattern.MinValue}..{pattern.MaxValue}");
        output.WriteLine($"Signed range: {pattern.SignedMinValue}..{pattern.SignedMaxValue}");
        output.WriteLine("First candidates:");
        foreach (var candidate in pattern.EnumerateCandidates(16))
            output.WriteLine($"  {candidate,4}  0x{FormatHex(candidate, width)}");
        return 0;
    }

    private static int Solve(IReadOnlyList<string> arguments, TextWriter output)
    {
        if (arguments.Count < 2)
            throw new ArgumentException("solve requires a quoted constraint such as \"x & 1010 = 1000\".");

        var match = AndConstraint.Match(arguments[1]);
        if (!match.Success)
            throw new FormatException("Supported syntax: x & BIT_PATTERN = BIT_PATTERN.");

        var mask = BitPattern.Parse(match.Groups[1].Value);
        var result = BitPattern.Parse(match.Groups[2].Value);
        if (mask.Count != result.Count)
            throw new ArgumentException("The mask and result must have the same width.");

        if (!BitPattern.TrySolveAnd(mask, result, out var solution))
        {
            output.WriteLine("No solution.");
            return 3;
        }

        output.WriteLine($"x = {solution}");
        output.WriteLine($"Candidates: {solution.CandidateCount}");
        output.WriteLine($"Unsigned range: {solution.MinValue}..{solution.MaxValue}");
        return 0;
    }

    private static int ReadIntegerOption(IReadOnlyList<string> arguments, string name)
    {
        var text = ReadStringOption(arguments, name);
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            throw new ArgumentException($"Option {name} must be an integer.");
        if (value < 2)
            throw new ArgumentOutOfRangeException(name, "A positional base must be at least 2.");
        return value;
    }

    private static string ReadStringOption(IReadOnlyList<string> arguments, string name)
    {
        for (var index = 0; index < arguments.Count; index++)
        {
            if (!string.Equals(arguments[index], name, StringComparison.OrdinalIgnoreCase)) continue;
            if (index + 1 >= arguments.Count)
                throw new ArgumentException($"Option {name} requires a value.");
            return arguments[index + 1];
        }
        throw new ArgumentException($"Missing required option {name}.");
    }

    private static int WidthOf(string typeName) => typeName.ToLowerInvariant() switch
    {
        "byte" or "sbyte" => 8,
        "short" or "ushort" or "int16" or "uint16" => 16,
        "int" or "uint" or "int32" or "uint32" => 32,
        "long" or "ulong" or "int64" or "uint64" => 64,
        _ => throw new ArgumentException(
            "Unknown type. Use byte, sbyte, short, ushort, int, uint, long, or ulong.")
    };

    private static string FormatHex(BigInteger value, int width)
    {
        var digits = (width + 3) / 4;
        var text = value.ToString("X", CultureInfo.InvariantCulture);
        while (text.Length > digits && text[0] == '0') text = text[1..];
        return text.PadLeft(digits, '0');
    }

    private static bool IsHelp(string value) =>
        value is "--help" or "-h" or "help";

    private static int Fail(TextWriter error, string message)
    {
        error.WriteLine($"error: {message}");
        return 2;
    }

    private static void WriteHelp(TextWriter output)
    {
        output.WriteLine("NumeralSystems.Net command line");
        output.WriteLine();
        output.WriteLine("Usage:");
        output.WriteLine("  numsys convert VALUE --from BASE --to BASE");
        output.WriteLine("  numsys inspect PATTERN --type TYPE");
        output.WriteLine("  numsys solve \"x & MASK = RESULT\"");
        output.WriteLine();
        output.WriteLine("Patterns use 0, 1, and ? from most-significant to least-significant bit.");
    }
}
