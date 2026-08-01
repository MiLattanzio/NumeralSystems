using System.Globalization;
using System.Numerics;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Tool;

/// <summary>Command dispatcher for the <c>numsys</c> global tool.</summary>
public static class NumsysApplication
{
    private static readonly BigInteger MaximumCliEnumeration = new(10_000);

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
        catch (TimeoutException exception)
        {
            return Fail(error, exception.Message, 4);
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
            throw new ArgumentException(
                "solve requires one or more quoted constraints such as \"x & 1010 = 1000\".");

        var enumerationLimit = ReadOptionalBigIntegerOption(arguments, "--limit");
        if (enumerationLimit > MaximumCliEnumeration)
            throw new ArgumentOutOfRangeException(
                "--limit",
                $"The CLI candidate limit cannot exceed {MaximumCliEnumeration}.");

        var timeoutMilliseconds = ReadOptionalIntegerOption(arguments, "--timeout") ?? 5_000;
        var options = new BitConstraintSolverOptions(
            maximumConstraints: 256,
            maximumBitWidth: 4_096,
            maximumEnumeratedCandidates: enumerationLimit ?? BigInteger.Zero,
            timeout: TimeSpan.FromMilliseconds(timeoutMilliseconds));
        var constraints = BitConstraintSet.Parse(arguments[1]);
        var solution = constraints.Solve(options);
        var explain = HasOption(arguments, "--explain");

        if (!solution.IsSatisfiable)
        {
            output.WriteLine("No solution.");
            if (explain) WriteExplanations(output, solution);
            return 3;
        }

        var pattern = solution.GetPatternOrThrow();
        output.WriteLine($"{constraints.VariableName} = {pattern}");
        output.WriteLine($"Candidates: {solution.CandidateCount}");
        output.WriteLine($"Unsigned range: {pattern.MinValue}..{pattern.MaxValue}");
        if (explain) WriteExplanations(output, solution);
        if (enumerationLimit.HasValue)
        {
            output.WriteLine($"First candidates (limit {enumerationLimit.Value}):");
            foreach (var candidate in solution.EnumerateCandidates(enumerationLimit.Value))
                output.WriteLine($"  {candidate}");
        }
        return 0;
    }

    private static void WriteExplanations(TextWriter output, BitConstraintSolution solution)
    {
        output.WriteLine("Explanation (MSB to LSB):");
        foreach (var explanation in solution.Explanations.Reverse())
        {
            var state = explanation.IsContradiction
                ? "!"
                : explanation.RequiredValue.HasValue
                    ? explanation.RequiredValue.Value ? "1" : "0"
                    : "?";
            output.WriteLine($"  bit {explanation.BitIndex,4}: {state}  {explanation.Message}");
        }
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

    private static int? ReadOptionalIntegerOption(IReadOnlyList<string> arguments, string name)
    {
        var text = ReadOptionalStringOption(arguments, name);
        if (text is null) return null;
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            throw new ArgumentException($"Option {name} must be a non-negative integer.");
        return value;
    }

    private static BigInteger? ReadOptionalBigIntegerOption(IReadOnlyList<string> arguments, string name)
    {
        var text = ReadOptionalStringOption(arguments, name);
        if (text is null) return null;
        if (!BigInteger.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            throw new ArgumentException($"Option {name} must be a non-negative integer.");
        return value;
    }

    private static string? ReadOptionalStringOption(IReadOnlyList<string> arguments, string name)
    {
        for (var index = 0; index < arguments.Count; index++)
        {
            if (!string.Equals(arguments[index], name, StringComparison.OrdinalIgnoreCase)) continue;
            if (index + 1 >= arguments.Count)
                throw new ArgumentException($"Option {name} requires a value.");
            return arguments[index + 1];
        }

        return null;
    }

    private static bool HasOption(IReadOnlyList<string> arguments, string name) =>
        arguments.Any(argument => string.Equals(argument, name, StringComparison.OrdinalIgnoreCase));

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

    private static int Fail(TextWriter error, string message, int exitCode = 2)
    {
        error.WriteLine($"error: {message}");
        return exitCode;
    }

    private static void WriteHelp(TextWriter output)
    {
        output.WriteLine("NumeralSystems.Net command line");
        output.WriteLine();
        output.WriteLine("Usage:");
        output.WriteLine("  numsys convert VALUE --from BASE --to BASE");
        output.WriteLine("  numsys inspect PATTERN --type TYPE");
        output.WriteLine("  numsys solve \"CONSTRAINT[; CONSTRAINT...]\" [--explain] [--limit COUNT] [--timeout MS]");
        output.WriteLine();
        output.WriteLine("Patterns use 0, 1, and ? from most-significant to least-significant bit.");
        output.WriteLine("Constraint operators are &, |, ^, and nand. Exit code 4 means timeout.");
    }
}
