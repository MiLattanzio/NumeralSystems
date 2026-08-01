using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Tool;

/// <summary>Command dispatcher for the <c>numsys</c> global tool.</summary>
public static class NumsysApplication
{
    private const string JsonSchemaVersion = "1.0";
    private static readonly BigInteger MaximumCliEnumeration = new(10_000);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    /// <summary>Runs one command and returns a process exit code.</summary>
    public static int Run(
        IReadOnlyList<string> arguments,
        TextWriter output,
        TextWriter error) =>
        Run(arguments, null, output, error);

    /// <summary>
    /// Runs one command with an optional redirected input stream and returns a process exit code.
    /// </summary>
    public static int Run(
        IReadOnlyList<string> arguments,
        TextReader? input,
        TextWriter output,
        TextWriter error)
    {
        if (arguments is null) throw new ArgumentNullException(nameof(arguments));
        if (output is null) throw new ArgumentNullException(nameof(output));
        if (error is null) throw new ArgumentNullException(nameof(error));

        var outputFormat = OutputFormat.Text;
        var commandName = "command";
        try
        {
            var normalized = NormalizeGlobalOptions(arguments, out outputFormat);
            if (normalized.Count == 0 || IsHelp(normalized[0]))
            {
                WriteResult(output, outputFormat, HelpResult());
                return 0;
            }

            commandName = normalized[0].ToLowerInvariant();
            var result = commandName switch
            {
                "convert" => Convert(normalized, input),
                "inspect" => Inspect(normalized),
                "solve" => Solve(normalized),
                _ => throw new ArgumentException(
                    $"Unknown command '{normalized[0]}'. Run 'numsys --help'.")
            };

            WriteResult(output, outputFormat, result);
            return result.ExitCode;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return WriteFailure(error, outputFormat, commandName, exception.Message, 2, "invalid_input");
        }
        catch (TimeoutException exception)
        {
            return WriteFailure(error, outputFormat, commandName, exception.Message, 4, "timeout");
        }
    }

    private static CommandResult Convert(IReadOnlyList<string> arguments, TextReader? input)
    {
        ValidateOptions(arguments, new[] { "--from", "--to", "--input" }, new[] { "--explain" });
        var sourceBase = ReadIntegerOption(arguments, "--from");
        var destinationBase = ReadIntegerOption(arguments, "--to");
        var explain = HasOption(arguments, "--explain");
        var sourceAlphabet = NumeralAlphabet.CreateDefault(sourceBase);
        var destinationAlphabet = NumeralAlphabet.CreateDefault(destinationBase);
        var values = ReadConversionInputs(arguments, input).ToArray();
        if (values.Length == 0)
            throw new ArgumentException(
                "convert requires a value, --input FILE, or redirected standard input.");

        var converted = values.Select((value, index) =>
        {
            try
            {
                var numericValue = sourceAlphabet.Decode(value);
                return new ConversionItem(
                    value,
                    destinationAlphabet.Encode(numericValue),
                    numericValue.ToString(CultureInfo.InvariantCulture));
            }
            catch (FormatException exception)
            {
                throw new FormatException(
                    $"Cannot convert input {index + 1} ({JsonSerializer.Serialize(value)}): {exception.Message}",
                    exception);
            }
        }).ToArray();

        using var text = new StringWriter(CultureInfo.InvariantCulture);
        foreach (var item in converted)
        {
            text.WriteLine(item.Result);
            if (!explain) continue;
            text.WriteLine(
                $"  {item.Input} (base {sourceBase}) = {item.DecimalValue} (base 10) = " +
                $"{item.Result} (base {destinationBase})");
        }

        var payload = new
        {
            fromBase = sourceBase,
            toBase = destinationBase,
            count = converted.Length,
            results = converted.Select(item => new
            {
                input = item.Input,
                result = item.Result,
                decimalValue = item.DecimalValue,
                explanation = explain
                    ? $"Decoded with the ordered base-{sourceBase} alphabet and encoded with the ordered base-{destinationBase} alphabet."
                    : null
            })
        };
        return new CommandResult("convert", 0, text.ToString(), payload);
    }

    private static CommandResult Inspect(IReadOnlyList<string> arguments)
    {
        ValidateOptions(arguments, new[] { "--type" }, new[] { "--explain" });
        var patternText = ReadRequiredPositional(arguments, 1, "inspect requires a bit pattern.");
        var typeName = ReadStringOption(arguments, "--type");
        var width = WidthOf(typeName);
        var pattern = BitPattern.Parse(patternText);
        if (pattern.Count != width)
            throw new ArgumentException(
                $"Type '{typeName}' requires {width} bits, but the pattern contains {pattern.Count}.");

        var explain = HasOption(arguments, "--explain");
        var candidates = pattern.EnumerateCandidates(16).Select(candidate => new
        {
            value = candidate.ToString(CultureInfo.InvariantCulture),
            hexadecimal = FormatHex(candidate, width)
        }).ToArray();

        using var text = new StringWriter(CultureInfo.InvariantCulture);
        text.WriteLine($"Pattern: {pattern}");
        text.WriteLine($"Type: {typeName.ToLowerInvariant()} ({width} bits)");
        text.WriteLine($"Unknown bits: {pattern.UnknownBitCount}");
        text.WriteLine($"Candidates: {pattern.CandidateCount}");
        text.WriteLine($"Unsigned range: {pattern.MinValue}..{pattern.MaxValue}");
        text.WriteLine($"Signed range: {pattern.SignedMinValue}..{pattern.SignedMaxValue}");
        text.WriteLine("First candidates:");
        foreach (var candidate in candidates)
            text.WriteLine($"  {candidate.value,4}  0x{candidate.hexadecimal}");
        if (explain)
        {
            text.WriteLine("Explanation (MSB to LSB):");
            foreach (var bit in BuildPatternExplanation(pattern))
                text.WriteLine($"  bit {bit.Index,4}: {bit.State}  {bit.Message}");
        }

        var payload = new
        {
            pattern = pattern.ToString(),
            type = typeName.ToLowerInvariant(),
            width,
            unknownBitCount = pattern.UnknownBitCount,
            candidateCount = pattern.CandidateCount.ToString(CultureInfo.InvariantCulture),
            unsignedRange = new
            {
                minimum = pattern.MinValue.ToString(CultureInfo.InvariantCulture),
                maximum = pattern.MaxValue.ToString(CultureInfo.InvariantCulture)
            },
            signedRange = new
            {
                minimum = pattern.SignedMinValue.ToString(CultureInfo.InvariantCulture),
                maximum = pattern.SignedMaxValue.ToString(CultureInfo.InvariantCulture)
            },
            candidates,
            explanation = explain
                ? BuildPatternExplanation(pattern).Select(bit => new
                {
                    bitIndex = bit.Index,
                    state = bit.State,
                    message = bit.Message
                })
                : null
        };
        return new CommandResult("inspect", 0, text.ToString(), payload);
    }

    private static CommandResult Solve(IReadOnlyList<string> arguments)
    {
        ValidateOptions(arguments, new[] { "--limit", "--timeout" }, new[] { "--explain" });
        var expression = ReadRequiredPositional(
            arguments,
            1,
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
        var constraints = BitConstraintSet.Parse(expression);
        var solution = constraints.Solve(options);
        var explain = HasOption(arguments, "--explain");
        var candidates = enumerationLimit.HasValue
            ? solution.EnumerateCandidates(enumerationLimit.Value)
                .Select(candidate => candidate.ToString(CultureInfo.InvariantCulture))
                .ToArray()
            : Array.Empty<string>();

        using var text = new StringWriter(CultureInfo.InvariantCulture);
        if (!solution.IsSatisfiable)
        {
            text.WriteLine("No solution.");
            if (explain) WriteExplanations(text, solution);
        }
        else
        {
            var pattern = solution.GetPatternOrThrow();
            text.WriteLine($"{constraints.VariableName} = {pattern}");
            text.WriteLine($"Candidates: {solution.CandidateCount}");
            text.WriteLine($"Unsigned range: {pattern.MinValue}..{pattern.MaxValue}");
            if (explain) WriteExplanations(text, solution);
            if (enumerationLimit.HasValue)
            {
                text.WriteLine($"First candidates (limit {enumerationLimit.Value}):");
                foreach (var candidate in candidates) text.WriteLine($"  {candidate}");
            }
        }

        var solvedPattern = solution.Pattern;
        var payload = new
        {
            variable = constraints.VariableName,
            width = constraints.Width,
            constraints = constraints.Select(constraint => constraint.ToString()),
            satisfiable = solution.IsSatisfiable,
            pattern = solvedPattern?.ToString(),
            candidateCount = solution.CandidateCount.ToString(CultureInfo.InvariantCulture),
            unsignedRange = solvedPattern is null
                ? null
                : new
                {
                    minimum = solvedPattern.MinValue.ToString(CultureInfo.InvariantCulture),
                    maximum = solvedPattern.MaxValue.ToString(CultureInfo.InvariantCulture)
                },
            candidates,
            explanation = explain
                ? solution.Explanations.Reverse().Select(ToJsonExplanation)
                : null
        };
        return new CommandResult("solve", solution.IsSatisfiable ? 0 : 3, text.ToString(), payload);
    }

    private static object ToJsonExplanation(BitConstraintBitExplanation explanation) => new
    {
        bitIndex = explanation.BitIndex,
        state = explanation.IsContradiction
            ? "contradiction"
            : explanation.RequiredValue.HasValue
                ? explanation.RequiredValue.Value ? "1" : "0"
                : "unknown",
        canBeZero = explanation.CanBeZero,
        canBeOne = explanation.CanBeOne,
        message = explanation.Message,
        sources = explanation.Sources.Select(source => source.ToString())
    };

    private static IReadOnlyList<PatternBitExplanation> BuildPatternExplanation(BitPattern pattern)
    {
        var result = new List<PatternBitExplanation>(pattern.Count);
        for (var bitIndex = pattern.Count - 1; bitIndex >= 0; bitIndex--)
        {
            var bit = pattern[bitIndex];
            result.Add(bit switch
            {
                true => new PatternBitExplanation(bitIndex, "1", "This bit is fixed to one."),
                false => new PatternBitExplanation(bitIndex, "0", "This bit is fixed to zero."),
                null => new PatternBitExplanation(bitIndex, "?", "Both zero and one are possible.")
            });
        }

        return result;
    }

    private static IEnumerable<string> ReadConversionInputs(
        IReadOnlyList<string> arguments,
        TextReader? input)
    {
        var positionalValues = ReadPositionals(
            arguments,
            1,
            "--from",
            "--to",
            "--input",
            "--limit",
            "--timeout");
        var inputPath = ReadOptionalStringOption(arguments, "--input");
        if (positionalValues.Count > 0 && inputPath is not null)
            throw new ArgumentException("A positional value cannot be combined with --input.");
        if (positionalValues.Count > 1)
            throw new ArgumentException("convert accepts one positional value; use standard input or --input for batches.");
        if (positionalValues.Count == 1)
        {
            yield return positionalValues[0];
            yield break;
        }

        IEnumerable<string> lines;
        if (inputPath is not null && inputPath != "-")
        {
            lines = File.ReadLines(Path.GetFullPath(inputPath));
        }
        else
        {
            if (input is null)
                throw new ArgumentException(
                    "No redirected standard input is available. Pass VALUE or --input FILE.");
            lines = ReadLines(input);
        }

        foreach (var line in lines)
        {
            // Windows PowerShell 5 may prefix redirected native-command input
            // with U+FEFF even when $OutputEncoding reports ASCII.
            var value = line.Trim().TrimStart('\uFEFF');
            if (value.Length > 0) yield return value;
        }
    }

    private static IEnumerable<string> ReadLines(TextReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) is not null) yield return line;
    }

    private static IReadOnlyList<string> NormalizeGlobalOptions(
        IReadOnlyList<string> arguments,
        out OutputFormat outputFormat)
    {
        outputFormat = OutputFormat.Text;
        var normalized = new List<string>(arguments.Count);
        for (var index = 0; index < arguments.Count; index++)
        {
            if (!string.Equals(arguments[index], "--output", StringComparison.OrdinalIgnoreCase))
            {
                normalized.Add(arguments[index]);
                continue;
            }

            if (index + 1 >= arguments.Count)
                throw new ArgumentException("Option --output requires text or json.");
            outputFormat = arguments[++index].ToLowerInvariant() switch
            {
                "text" => OutputFormat.Text,
                "json" => OutputFormat.Json,
                var value => throw new ArgumentException(
                    $"Unknown output format '{value}'. Use text or json.")
            };
        }

        return normalized;
    }

    private static IReadOnlyList<string> ReadPositionals(
        IReadOnlyList<string> arguments,
        int startIndex,
        params string[] optionsWithValues)
    {
        var values = new List<string>();
        for (var index = startIndex; index < arguments.Count; index++)
        {
            if (optionsWithValues.Any(option =>
                    string.Equals(arguments[index], option, StringComparison.OrdinalIgnoreCase)))
            {
                index++;
                continue;
            }
            if (arguments[index].StartsWith("--", StringComparison.Ordinal)) continue;
            values.Add(arguments[index]);
        }

        return values;
    }

    private static void ValidateOptions(
        IReadOnlyList<string> arguments,
        IReadOnlyCollection<string> optionsWithValues,
        IReadOnlyCollection<string> flags)
    {
        for (var index = 1; index < arguments.Count; index++)
        {
            if (!arguments[index].StartsWith("--", StringComparison.Ordinal)) continue;
            if (flags.Contains(arguments[index], StringComparer.OrdinalIgnoreCase)) continue;
            if (optionsWithValues.Contains(arguments[index], StringComparer.OrdinalIgnoreCase))
            {
                if (index + 1 >= arguments.Count)
                    throw new ArgumentException($"Option {arguments[index]} requires a value.");
                index++;
                continue;
            }

            throw new ArgumentException($"Unknown option '{arguments[index]}'.");
        }
    }

    private static string ReadRequiredPositional(
        IReadOnlyList<string> arguments,
        int startIndex,
        string errorMessage)
    {
        var values = ReadPositionals(arguments, startIndex, "--type", "--limit", "--timeout", "--input");
        if (values.Count == 0) throw new ArgumentException(errorMessage);
        if (values.Count > 1) throw new ArgumentException("Only one positional expression is accepted.");
        return values[0];
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
        var value = ReadOptionalStringOption(arguments, name);
        return value ?? throw new ArgumentException($"Missing required option {name}.");
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

    private static bool IsHelp(string value) => value is "--help" or "-h" or "help";

    private static CommandResult HelpResult()
    {
        const string help = """
            NumeralSystems.Net command line

            Usage:
              numsys [--output text|json] convert [VALUE] --from BASE --to BASE [--input FILE|-] [--explain]
              numsys [--output text|json] inspect PATTERN --type TYPE [--explain]
              numsys [--output text|json] solve "CONSTRAINT[; CONSTRAINT...]" [--explain] [--limit COUNT] [--timeout MS]

            Omit VALUE to read one value per line from redirected standard input. Blank lines are ignored.
            Patterns use 0, 1, and ? from most-significant to least-significant bit.
            Constraint operators are &, |, ^, and nand. Exit code 3 means unsatisfiable; 4 means timeout.
            """;
        return new CommandResult(
            "help",
            0,
            help + Environment.NewLine,
            new
            {
                usage = new[]
                {
                    "numsys [--output text|json] convert [VALUE] --from BASE --to BASE [--input FILE|-] [--explain]",
                    "numsys [--output text|json] inspect PATTERN --type TYPE [--explain]",
                    "numsys [--output text|json] solve \"CONSTRAINT[; CONSTRAINT...]\" [--explain] [--limit COUNT] [--timeout MS]"
                }
            });
    }

    private static void WriteResult(TextWriter output, OutputFormat format, CommandResult result)
    {
        if (format == OutputFormat.Text)
        {
            output.Write(result.Text);
            return;
        }

        output.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = JsonSchemaVersion,
            command = result.Command,
            success = result.ExitCode == 0,
            exitCode = result.ExitCode,
            result = result.Payload
        }, JsonOptions));
    }

    private static int WriteFailure(
        TextWriter error,
        OutputFormat format,
        string command,
        string message,
        int exitCode,
        string code)
    {
        if (format == OutputFormat.Text)
        {
            error.WriteLine($"error: {message}");
        }
        else
        {
            error.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = JsonSchemaVersion,
                command,
                success = false,
                exitCode,
                error = new { code, message }
            }, JsonOptions));
        }

        return exitCode;
    }

    private enum OutputFormat
    {
        Text,
        Json
    }

    private sealed record CommandResult(string Command, int ExitCode, string Text, object Payload);

    private sealed record ConversionItem(string Input, string Result, string DecimalValue);

    private sealed record PatternBitExplanation(int Index, string State, string Message);
}
