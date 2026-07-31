using System.Text.Json;
using NumeralSystems.Net;
using NumeralSystems.Net.Json;
using NumeralSystems.Net.Type.Incomplete;

var examples = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
{
    ["exact"] = ExactFractions,
    ["periods"] = PeriodTable,
    ["bits"] = UnknownBits,
    ["json"] = JsonRoundTrip
};

if (args.Length == 0 || string.Equals(args[0], "all", StringComparison.OrdinalIgnoreCase))
{
    foreach (var example in examples)
    {
        Console.WriteLine($"== {example.Key} ==");
        example.Value();
        Console.WriteLine();
    }
    return;
}

if (!examples.TryGetValue(args[0], out var selected))
{
    Console.Error.WriteLine($"Unknown example '{args[0]}'. Choose: {string.Join(", ", examples.Keys)}.");
    Environment.ExitCode = 2;
    return;
}

selected();

static void ExactFractions()
{
    var oneTenth = new RationalValue(1, 10);
    var options = new NumeralConversionOptions(
        maxFractionalDigits: 256,
        roundingMode: NumeralRoundingMode.ToNearestEven,
        detectRepeatingPeriod: true,
        infiniteBehavior: InfiniteExpansionBehavior.PreservePeriod);
    var binary = oneTenth.Expand(2, options);

    Console.WriteLine($"1/10 in base 2: {binary.ToString(NumeralAlphabet.Base2)}");
    Console.WriteLine($"period starts at {binary.RepeatingStartIndex}, length {binary.RepeatingLength}");
    Console.WriteLine($"1/3 in base 3: {new RationalValue(1, 3).Expand(3, options)}");
}

static void PeriodTable()
{
    var options = new NumeralConversionOptions(
        maxFractionalDigits: 4096,
        roundingMode: NumeralRoundingMode.ToNearestEven,
        detectRepeatingPeriod: true,
        infiniteBehavior: InfiniteExpansionBehavior.PreservePeriod);

    for (var denominator = 2; denominator <= 20; denominator++)
    {
        var expansion = new RationalValue(1, denominator).Expand(10, options);
        var graph = expansion.IsTerminating
            ? "terminating"
            : new string('#', expansion.RepeatingLength);
        Console.WriteLine($"1/{denominator,-2} {graph} ({expansion.RepeatingLength})");
    }
}

static void UnknownBits()
{
    var pattern = BitPattern.Parse("1100????");
    Console.WriteLine($"{pattern}: {pattern.UnknownBitCount} unknown, {pattern.CandidateCount} candidates");
    Console.WriteLine($"unsigned range {pattern.MinValue}..{pattern.MaxValue}");

    var solution = BitPattern.SolveAnd(
        BitPattern.Parse("10101010"),
        BitPattern.Parse("10001000"));
    Console.WriteLine($"x & 10101010 = 10001000 -> x = {solution}");
}

static void JsonRoundTrip()
{
    var options = new JsonSerializerOptions { WriteIndented = true }.AddNumeralSystems();
    var binary = Numeral.System.OfBase(2);
    binary.AdjustToFitIntegralLength = false;
    var numeral = Numeral.FromRational(
        binary,
        new RationalValue(1, 10),
        new NumeralConversionOptions(
            256,
            NumeralRoundingMode.ToNearestEven,
            true,
            InfiniteExpansionBehavior.PreservePeriod));
    var json = JsonSerializer.Serialize(numeral, options);
    var restored = JsonSerializer.Deserialize<Numeral>(json, options)
        ?? throw new InvalidOperationException("The numeral was not deserialized.");

    Console.WriteLine(json);
    Console.WriteLine($"exact round-trip: {restored.ExactValue == numeral.ExactValue}");
}
