# Cookbook

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Arithmetic](arithmetic.md) ·
[BitPattern engine](bit-patterns.md) ·
[Troubleshooting](troubleshooting.md) ·
[API reference](api-reference.md)

This page collects task-oriented examples. Each recipe states which abstraction
to use and calls out precision or representation details that are easy to miss.

## Convert a normal integer to hexadecimal

Use a `NumeralSystem` when formatting is part of the task:

```csharp
using NumeralSystems.Net;

var hexadecimal = Numeral.System.OfBase(16);
hexadecimal.AdjustToFitIntegralLength = false;

var value = hexadecimal[65535];

Console.WriteLine(value); // FFFF
```

The default alphabet is culture-aware. For a persistent format, use an explicit
alphabet as shown below.

## Create deterministic hexadecimal text

```csharp
var hexadecimal = Numeral.System.OfBase(16);
hexadecimal.AdjustToFitIntegralLength = false;

var format = new NumeralSystem.SerializationInfo
{
    Identity = "0123456789ABCDEF".Select(c => c.ToString()).ToList(),
    Separator = "",
    NegativeSign = "-",
    NumberDecimalSeparator = "."
};

var text = hexadecimal[48879].ToString(
    format.Identity,
    format.Separator,
    format.NegativeSign,
    format.NumberDecimalSeparator);

Console.WriteLine(text); // BEEF
```

Retain the same `SerializationInfo` when parsing.

## Convert an integer larger than `ulong`

Use `BigInteger` with a `NumeralSystem`, `NumeralValue`, or `Value`:

```csharp
using System.Numerics;

var number = BigInteger.Pow(2, 256) + 1;
var base36 = Numeral.System.OfBase(36);
base36.AdjustToFitIntegralLength = false;

var numeral = base36[number];

Console.WriteLine(numeral.BigInteger == number); // True
```

Use `Value.FromBigInteger(number, 36)` when symbols and formatting are not
needed.

## Preserve leading zeroes during an integral conversion

`Value.ToBase` preserves leading width by default:

```csharp
var source = new Value(
    indices: new List<int> { 0, 0, 2, 5, 5 },
    baseValue: 10);

var binary = source.ToBase(2);

Console.WriteLine(binary.Indices[0]); // 0
Console.WriteLine(binary.Indices[1]); // 0
```

Pass `removeFirstZeros: true` for the canonical minimal-width result.

## Interpret fractional digits correctly

Digits after the separator use negative powers of their declared base:

```csharp
var binaryHalf = new NumeralValue(
    integral: new List<int> { 0 },
    decimals: new List<int> { 1 },
    negative: false,
    baseValue: 2);

Console.WriteLine(binaryHalf.ToDecimal()); // 0.5
```

Do not read `Decimals` as decimal text unless `Base == 10`.

## Detect a repeating conversion

```csharp
var oneThird = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    false,
    3);

var exact = oneThird.TryToBase(
    baseValue: 10,
    maxFractionalDigits: 12,
    result: out var converted);

Console.WriteLine(exact); // False
Console.WriteLine(string.Concat(converted.Decimals)); // 333333333333
```

Store the `exact` flag or reject the conversion when truncation is not allowed.

## Add values written in different bases

```csharp
var binaryHalf = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    false,
    2);

var decimalQuarter = NumeralValue.FromDecimal(0.25m);

var sum = binaryHalf.Add(decimalQuarter, out var exact);

Console.WriteLine(exact);           // True
Console.WriteLine(sum.Base);        // 2
Console.WriteLine(sum.ToDecimal()); // 0.75
```

The result base defaults to the base of the left operand.

## Divide in a base where the result terminates

One third repeats in base 10 but terminates in base 3:

```csharp
var one = NumeralValue.FromInt(1);
var three = NumeralValue.FromInt(3);

var result = one.Divide(
    three,
    exact: out var exact,
    resultBase: 3,
    maxFractionalDigits: 8);

Console.WriteLine(exact);                         // True
Console.WriteLine(string.Concat(result.Decimals)); // 1
```

This technique is useful when a protocol or domain naturally uses a
non-decimal radix.

## Compare equivalent values from different bases

```csharp
var binaryHalf = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    false,
    2);

var decimalHalf = NumeralValue.FromDecimal(0.5m);

Console.WriteLine(binaryHalf.NumericallyEquals(decimalHalf)); // True
Console.WriteLine(binaryHalf.CompareTo(decimalHalf));          // 0
```

Do not use reference equality to answer a numeric question.

## Parse custom multi-character digits

Use a non-empty separator when symbols have variable length:

```csharp
var ternary = Numeral.System.OfBase(3);
ternary.AdjustToFitIntegralLength = false;

var identity = new List<string> { "zero", "one", "two" };
var parsed = ternary.Parse(
    "one|two",
    identity,
    separator: "|",
    negativeSign: "-",
    numberDecimalSeparator: ".");

Console.WriteLine(parsed.Integer); // 5
```

Without a separator, overlapping symbols can be ambiguous.

## Use a stable alphabet for data exchange

```csharp
var base32 = Numeral.System.OfBase(32);
var alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ"
    .Select(c => c.ToString())
    .ToList();

var encoded = base32[123456789];
var text = encoded.ToString(alphabet, "", "-", ".");
var decoded = base32.Parse(text, alphabet, "", "-", ".");

Console.WriteLine(decoded.BigInteger); // 123456789
```

The example alphabet omits visually ambiguous characters, but it is not a
standard Base32 implementation.

## Inspect and modify primitive bits

```csharp
using IntBits = NumeralSystems.Net.Type.Base.Int;

var value = new IntBits { Value = 10 };

Console.WriteLine(value.Binary[1]); // True

value[0] = true;
Console.WriteLine(value.Value);     // 11
```

`Binary[0]` is the least-significant bit.

## Recover a possible operand of `AND`

```csharp
using ByteBits = NumeralSystems.Net.Type.Base.Byte;

var knownRight = new ByteBits { Value = 0b1010 };
var result = new ByteBits { Value = 0b1000 };

if (result.ReverseAnd(knownRight, out var possibleLeft))
{
    var expected = new ByteBits { Value = 0b1100 };
    Console.WriteLine(possibleLeft.Contains(expected)); // True
}
```

The reverse result is incomplete because multiple left operands can satisfy the
equation.

## Enumerate unknown bits safely

```csharp
var incomplete = new NumeralSystems.Net.Type.Base.Byte
{
    Value = 0b0010
}.Incomplete();

incomplete.Binary[0] = null;
incomplete.Binary[2] = null;

Console.WriteLine(incomplete.CandidateCount); // 4

foreach (var candidate in incomplete.EnumerateCandidates(limit: 16))
{
    Console.WriteLine(candidate);
}
```

Candidate count grows as `2^unknownBits`. `CandidateCount` uses `BigInteger`,
and `EnumerateCandidates` never returns more than its explicit limit.

## Combine two partial bit constraints

```csharp
using NumeralSystems.Net.Type.Incomplete;

var header = new BitPattern(new bool?[]
{
    true, null, false, null, null, null, true, false
});

var subtype = new BitPattern(new bool?[]
{
    null, true, false, null, true, null, null, false
});

if (header.TryIntersect(subtype, out var combined))
{
    Console.WriteLine(combined);
    Console.WriteLine(combined.CandidateCount);
}
```

Use `IsCompatibleWith` when only the Boolean answer is required.

## Solve an AND mask constraint

```csharp
using NumeralSystems.Net.Type.Incomplete;

var mask = BitPattern.FromUnsigned(0b1111_0000, 8);
var required = BitPattern.FromUnsigned(0b1010_0000, 8);

if (BitPattern.TrySolveAnd(mask, required, out var input))
{
    foreach (var candidate in input.EnumerateCandidates(limit: 8))
    {
        Console.WriteLine(candidate);
    }
}
```

The solution is `1010????`: the low four input bits are unconstrained.

## Shift or rotate an incomplete pattern

```csharp
var pattern = new NumeralSystems.Net.Type.Incomplete.BitPattern(
    new bool?[] { true, null, false, true, false, false, false, false });

var logical = pattern.LogicalShiftRight(1);
var arithmetic = pattern.ArithmeticShiftRight(1);
var rotated = pattern.RotateRight(1);
```

All results retain the original width. Arithmetic right shift copies the
highest bit, including an unknown highest bit.

## Convert string code units to another base

```csharp
var encoded = NumeralSystems.Net.Type.Base.String.EncodeToBase(
    "Hello",
    destinationBase: 16,
    size: out var width);

var decoded = NumeralSystems.Net.Type.Base.String.DecodeFromBase(
    encoded,
    sourceBase: 16,
    size: width);

Console.WriteLine(decoded); // Hello
```

This API converts UTF-16 code units. It is not Base64 and should not be used as
a cryptographic or standardized transport encoding.

## Validate the package before a release

From `NumeralSystems.Net/`:

```bash
dotnet restore NumeralSystems.Net/NumeralSystems.Net.csproj
dotnet test NumeralSystems.Net.sln --configuration Release
dotnet pack NumeralSystems.Net/NumeralSystems.Net.csproj \
  --configuration Release \
  --no-restore \
  --output artifacts
```

Inspect the generated `.nupkg` and `.snupkg`, then follow the
[release guide](releasing.md).
