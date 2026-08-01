# Troubleshooting

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Arithmetic](arithmetic.md) ·
[Cookbook](cookbook.md) ·
[Releasing](releasing.md)

## `ArgumentOutOfRangeException`: base must be at least 2

Positional bases 0 and 1 are not accepted. The validation applies consistently
to:

- `NumeralSystem`;
- `Value` and `NumeralValue`;
- base-conversion helpers;
- arithmetic result bases.

Use base 2 or greater:

```csharp
var binary = Numeral.System.OfBase(2);
```

Unary or non-positional representations need a different model.

## A digit is rejected

Every digit is an integer index in `0..base-1`. In base 16, for example, valid
indices are 0 through 15:

```csharp
var valid = new Value(new List<int> { 15, 15 }, 16);

// Throws: digit 16 does not exist in base 16.
var invalid = new Value(new List<int> { 1, 16 }, 16);
```

An alphabet changes how a digit is printed; it does not change its numeric
index.

## Binary output contains leading zeroes

`NumeralSystem.AdjustToFitIntegralLength` defaults to `true`. It pads newly
constructed numerals to the system's byte-oriented width:

```csharp
var binary = Numeral.System.OfBase(2);
binary.AdjustToFitIntegralLength = false;

Console.WriteLine(binary[5]); // 101
```

Set the option before constructing or parsing the numeral.

For `Value.ToBase`, pass `removeFirstZeros: true` when a canonical minimal-width
digit list is required.

## A fraction is much longer than expected

The fraction may repeat in the destination base. For example, one third repeats
in base 10, while one tenth repeats in base 2.

Use an explicit policy and inspect the expansion metadata:

```csharp
var options = new NumeralConversionOptions(
    32,
    NumeralRoundingMode.ToNearestEven,
    true,
    InfiniteExpansionBehavior.PreservePeriod);
var expansion = value.Expand(10, options);
```

Inspect `IsTerminating`, `HasRepeatingPeriod`, and `RepeatingLength`. Increase
the limit, select a more suitable base, or select `Truncate`, `Round`, or
`Throw` according to application requirements.

## A compatibility arithmetic call returned `exact == false`

Arithmetic is performed on an exact rational value first. The flag describes
only the final representation in the result base.

Common causes:

- division produced a repeating fraction;
- operands in different bases created a denominator that does not terminate in
  the chosen result base;
- `maxFractionalDigits` is smaller than a terminating expansion requires.

Prefer an explicit result base and policy:

```csharp
var result = left.Divide(
    right,
    NumeralConversionOptions.Default,
    resultBase: 3);
```

See [Arithmetic](arithmetic.md) for the complete precision contract.

## The result of `a + b` uses an unexpected base

Operators and short arithmetic methods use the left operand's base:

```csharp
var result = left + right;
Console.WriteLine(result.Base == left.Base); // True
```

Use the option-aware method when a specific result base is required:

```csharp
var result = left.Add(
    right,
    NumeralConversionOptions.Default,
    resultBase: 16);
```

## `ToBigInteger()` lost the fractional part

This is intentional. `BigInteger` cannot represent a fractional component, so
conversion truncates toward zero:

```csharp
var value = NumeralValue.FromDecimal(-12.75m);
Console.WriteLine(value.ToBigInteger()); // -12
```

Use `ToDecimal`, `ToDouble`, the digit lists, or rational arithmetic when the
fraction must be retained.

## `ToDecimal()` throws `OverflowException`

`NumeralValue` and integral arithmetic can exceed the range of `decimal`.
`ToDecimal()` is a bounded view, not the storage limit.

For an integral value, use:

```csharp
BigInteger integer = value.ToBigInteger();
```

For a very large fractional value, keep it as `NumeralValue` or
`RationalValue`; `ExactValue` is not bounded by the primitive target range.

## `NumeralExpansionLimitException` is thrown

`PreservePeriod` refuses to return an inexact value. The terminating tail or
repeating cycle was not completed before `MaxFractionalDigits`. Increase the
limit for trusted input, choose `Truncate` or `Round` for display, or choose
`Throw` when the output format forbids repeating fractions.

## Two numerically equal values do not pass `object.Equals`

`NumeralValue` retains normal reference equality. Use the numeric APIs:

```csharp
var same = left.NumericallyEquals(right);
var order = left.CompareTo(right);
```

This avoids changing dictionary and set behavior for existing applications.

## Division throws `DivideByZeroException`

A divisor is zero when all integral and fractional digits are zero. Its stored
sign does not matter.

Check `IsZero` before dividing when zero is an expected input:

```csharp
if (!divisor.IsZero)
{
    var quotient = dividend / divisor;
}
```

## Parsing works on one machine but fails on another

The convenient `Parse(string)` and `ToString()` methods use
`CultureInfo.CurrentCulture` for the negative sign and decimal separator.

For stable files, tests, and protocols, pass an explicit:

- `NumeralAlphabet`;
- digit separator;
- negative sign;
- decimal separator.

The [numeral systems guide](numeral-systems.md) contains a complete
`SerializationInfo` example.

## A custom alphabet is rejected

`NumeralAlphabet` rejects duplicate, empty, and prefix-ambiguous symbols:

```csharp
// "a" is a prefix of "ab".
var invalid = new NumeralAlphabet(new[] { "a", "ab" });
```

Symbols also cannot conflict with the configured digit separator, negative
sign, or decimal separator. Use distinct tokens:

```csharp
var alphabet = new NumeralAlphabet(new[] { "zero", "one", "two" });
var parsed = ternary.Parse("one|two", alphabet, "|", "-", ".");
```

## Parsing failed but a Boolean is not enough

Use the structured overload:

```csharp
var result = hexadecimal.TryParse("1G", NumeralAlphabet.Base16);

Console.WriteLine(result.Reason);   // UnknownSymbol
Console.WriteLine(result.Position); // 1
Console.WriteLine(result.Message);
```

`Position` is a zero-based UTF-16 index. `ErrorLength` identifies how much text
belongs to the error.

## A HashSet alphabet overload no longer exists

`Value.FromString(string, HashSet<string>)` was removed in 4.8.1. A set does not
define which symbol means zero, one, or any other digit.

Create the alphabet from an ordered sequence:

```csharp
var alphabet = new NumeralAlphabet(orderedSymbols);
var value = Value.FromString(text, alphabet);
```

## `Binary[0]` appears to be reversed

Primitive wrappers store bit arrays least-significant bit first:

```text
Binary[0] = least-significant bit
Binary[BitLength - 1] = most-significant bit
```

`ToString()` produces the normal human-readable order.

## Incomplete-value enumeration is too slow or large

Each unknown bit doubles the candidate count:

```text
candidate count = 2^unknownBits
```

Inspect `Permutations` before iterating `Enumerable`. Prefer `Contains` when the
task is only to test whether one known value is compatible.

## A bit constraint parses but has no solution

Syntax validity and mathematical satisfiability are separate. Parse a single
expression with `BitConstraintParser.Parse` to inspect `ErrorReason` and
`ErrorPosition`. For a valid set, inspect `BitConstraintSolution.IsSatisfiable`:

```csharp
var solution = BitConstraintSet.Parse(
    "x ^ 0000 = 0000; x | 0000 = 0001").Solve();

foreach (var bit in solution.Explanations.Where(item => item.IsContradiction))
    Console.WriteLine(bit.Message);
```

`Pattern` is `null` and `CandidateCount` is zero for a contradiction. This is
not a parser failure.

## Constraint composition rejects a rule

All rules in one `BitConstraintSet` must use the same variable and width. Check
for accidental padding differences and variable spelling. Variable comparison
is case-insensitive, so `x` and `X` are compatible.

## Constraint solving exceeded a limit or timed out

`BitConstraintLimitException` identifies count, width, or enumeration policy
through `LimitName`. Increase only the relevant
`BitConstraintSolverOptions` value after validating the input source.

`BitConstraintTimeoutException` means the configured elapsed-time budget was
reached. Candidate enumeration has its own timer and always requires an
explicit limit. Cancellation produces `OperationCanceledException` instead.
The CLI maps timeout to exit code 4 and never accepts `--limit` above 10,000.

## String encoding produced control characters

`CharacterRadixTransform.EncodeUtf16` maps character values to raw positional
digits. The encoded string is not designed to be printable, URL-safe, or
interoperable with Base64.

Retain both:

- the base;
- the width returned by `EncodeUtf16`.

Use `StandardBaseCodec.EncodeBase64` when a standardized text transport is
required.

## `NumeralAlphabet.Base64` does not match Base64 output

`NumeralAlphabet.Base64` represents an integer with 64 ordered digit symbols.
It does not group bytes into six-bit chunks or apply RFC padding.

Choose based on the input model:

```csharp
var numericText = NumeralAlphabet.Base64.Encode(bigInteger);
var binaryText = StandardBaseCodec.EncodeBase64(bytes);
```

The same distinction applies to Base16 and Base32.

## Emoji became two values

UTF-16 APIs intentionally expose surrogate code units, so many supplementary
characters contribute two digits. On .NET 8, choose the Rune API when one
Unicode scalar should be one value:

```csharp
var utf16 = Value.FromUtf16String("😀"); // two digits
var runes = Value.FromRunes("😀");       // one digit
```

Rune APIs reject unpaired surrogates. Fix or sanitize malformed input before
retrying; replacement is never silent.

## `GetSmallestBase` is one larger after upgrading

This is the corrected contract. A digit equal to its base is invalid, so the
smallest legal base is `maximum digit + 1`. Do not increment the 4.8 result a
second time. Empty input returns base 2.

## JSON did not preserve my custom alphabet

The .NET 8 `Numeral` JSON converter persists numeric structure—base, sign, and
digit arrays—not presentation. Send a separate alphabet or alphabet identifier
when a protocol needs a custom mapping. The digit arrays and leading zeros are
preserved exactly.

## Rune, Span, or JSON APIs are missing

These members are exposed by the `net8.0` package asset. A project targeting
only .NET Standard 2.1 receives the portable API, including UTF-16,
`IFormatProvider`, standard codecs, and streaming, but not framework types that
are unavailable in that target. Retarget the consuming application to .NET 8
or multi-target it when these APIs are required.

## NuGet publishing fails before contacting NuGet.org

If `NuGet/login@v1` reports:

```text
Input required and not supplied: user
```

create the GitHub Actions variable `NUGET_USER` and set it to the NuGet.org
profile name, not an email address. Then rerun the failed job.

For OIDC policy mismatches and the complete release checklist, see
[Releasing](releasing.md).

## Build fails while treating warnings as errors

The library, tests, and benchmark projects enable `TreatWarningsAsErrors`.
Resolve the first compiler warning rather than suppressing the build globally.

Run:

```bash
dotnet build NumeralSystems.Net.sln \
  --configuration Release \
  --verbosity minimal
```

The library intentionally suppresses only missing public XML comments
(`CS1591`); malformed XML documentation remains an error.
