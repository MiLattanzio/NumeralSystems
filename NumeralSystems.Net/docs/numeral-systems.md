# Numeral systems and base conversion

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Formatting and JSON](formatting-and-serialization.md) ·
[Arithmetic](arithmetic.md) ·
[Cookbook](cookbook.md) ·
[API reference](api-reference.md)

## Representation model

A positional numeral has four pieces:

1. a base (`NumeralSystem.Size`);
2. integral digit indices;
3. fractional digit indices;
4. a sign (`Numeral.Positive`).

Each digit index must be in the range `0..Size - 1`. The numeric meaning of the
digits does not depend on the symbols used to print them.

For example, in base 12 the indices `[11, 11]` represent:

```text
11 × 12¹ + 11 × 12⁰ = 143
```

An alphabet maps those indices to strings. With
`0 1 2 3 4 5 6 7 8 9 X Y`, the same numeral is printed as `YY` if `X` is index
10 and `Y` is index 11.

## `NumeralSystem` and `Numeral`

Create systems with the factory or constructor:

```csharp
var base2 = Numeral.System.OfBase(2);
var base16 = new NumeralSystem(16);
```

`NumeralSystem` validates digit indices, parses text, formats digits, converts
indices to primitive values, and exposes indexers for common numeric types.
`Numeral` stores a value in one system and exposes views such as `BigInteger`,
`Integer`, `Decimal`, `Double`, `Float`, `Char`, and `Bytes`.

```csharp
var base16 = Numeral.System.OfBase(16);
var numeral = base16[255];

Console.WriteLine(numeral.Base.Size); // 16
Console.WriteLine(numeral.Integer);   // 255
Console.WriteLine(numeral);           // FF
```

## Width and leading zeroes

`AdjustToFitIntegralLength` defaults to `true`. When enabled, the constructor
pads the integral digits to `NumeralSystem.Length`, which is based on the number
of digits required to represent one byte in that base.

```csharp
var binary = Numeral.System.OfBase(2);

Console.WriteLine(binary[5]); // 00000101

binary.AdjustToFitIntegralLength = false;
Console.WriteLine(binary[5]); // 101
```

Set the property before constructing the numeral. Padding is part of the stored
integral indices, not only a display option.

## Custom alphabets

Use an ordered immutable `NumeralAlphabet` whose position is the numeric value
of a digit:

```csharp
var dozenal = Numeral.System.OfBase(12);
dozenal.AdjustToFitIntegralLength = false;

var alphabet = new NumeralAlphabet(
    "0123456789XY".Select(c => c.ToString()));

var numeral = dozenal[143];
var text = numeral.ToString(
    alphabet,
    separator: "",
    negativeSign: "-",
    numberDecimalSeparator: ".");

Console.WriteLine(text); // YY

var roundTrip = dozenal.Parse(
    text,
    alphabet,
    separator: "",
    negativeSign: "-",
    numberDecimalSeparator: ".");

Console.WriteLine(roundTrip.Integer); // 143
```

Symbols may contain more than one character. Duplicate and prefix-ambiguous
symbols are rejected at construction. A separator can still improve
readability:

```csharp
var alphabet = new NumeralAlphabet(new[] { "zero", "one", "two" });
var ternary = Numeral.System.OfBase(3);
ternary.AdjustToFitIntegralLength = false;

var text = ternary[5].ToString(alphabet, "|", "-", ".");
Console.WriteLine(text); // one|two
```

`ValidateFormat` and the parsing/formatting APIs reject conflicts between digit
symbols, the negative sign, decimal separator, and digit separator.

## Default serialization

`Numeral.ToString()` and `NumeralSystem.Parse(string)` call
`NumeralSystem.SerializationInfo.OfBase`. That method:

- uses a predefined `NumeralAlphabet` for bases 2, 8, 10, 16, 32, 36, 58, 62,
  and 64;
- generates deterministic fixed-width symbols for other bases;
- reads the negative sign and decimal separator from the current culture.

This is convenient for display, but explicit settings are safer for files,
tests, hashes, and network protocols.

You can retain a configuration:

```csharp
var format = new NumeralSystem.SerializationInfo
{
    Alphabet = NumeralAlphabet.Base16,
    Separator = "",
    NegativeSign = "-",
    NumberDecimalSeparator = "."
};

var hexadecimal = Numeral.System.OfBase(16);
var parsed = hexadecimal.Parse("7B", format);
```

## Converting a `Numeral`

`Numeral.To` converts the stored positional digits and returns a new numeral in
the target system. Integral values use arbitrary-precision arithmetic:

```csharp
var decimalSystem = Numeral.System.OfBase(10);
var hexadecimal = Numeral.System.OfBase(16);

var source = decimalSystem[65535];
var destination = source.To(hexadecimal, NumeralConversionOptions.Default);

Console.WriteLine(destination); // FFFF
```

Fractional conversions use immutable `NumeralConversionOptions`. The default
generates at most 128 digits and preserves a detected period exactly; explicit
policies can throw, truncate, or round. Primitive `decimal`, `double`, and
`float` views remain bounded by their underlying .NET type.

### Controlling fractional precision

Each fractional position has the normal positional meaning. For example,
`0.1` in base 2 is `1 × 2⁻¹`, or `0.5` in base 10:

```csharp
var oneHalf = NumeralValue.FromRational(1, 2, baseValue: 2);

Console.WriteLine(oneHalf.ToDecimal()); // 0.5
```

Some expansions repeat in the destination base. The expansion result exposes
both termination and period metadata:

```csharp
var oneThird = NumeralValue.FromRational(1, 3, baseValue: 3);
var expansion = oneThird.Expand(10, NumeralConversionOptions.Default);

Console.WriteLine(expansion.IsTerminating);       // False
Console.WriteLine(expansion.RepeatingStartIndex); // 0
Console.WriteLine(expansion.RepeatingLength);     // 1
Console.WriteLine(expansion.ToString(NumeralAlphabet.Base10)); // 0.(3)
```

## Using `Value`

`Value` stores only a non-negative integral digit sequence:

```csharp
var decimalDigits = new Value(
    indices: new List<int> { 2, 5, 5 },
    baseValue: 10);

var hexadecimalDigits = decimalDigits.ToBase(16, removeFirstZeros: true);
var text = string.Concat(hexadecimalDigits.Indices.Select(i => i.ToString("X")));

Console.WriteLine(text); // FF
```

An unordered set cannot define numeric symbol order. Version 4.8.1 exposes
only the ordered alphabet overload:

```csharp
var value = Value.FromString("00FF", NumeralAlphabet.Base16);
var text = value.ToString(NumeralAlphabet.Base16);
```

Use `FromUtf16String` for UTF-16 code units, or `FromRunes` on .NET 8 for
Unicode scalar values. With `fit: true`, both select `maxDigit + 1` with a
minimum base of 2. See [Text and binary encodings](string-encoding.md).

`Value` does not preserve a sign or fractional digits.

## Using `NumeralValue`

`NumeralValue` keeps an exact rational value and an immutable positional
projection:

```csharp
var value = NumeralValue.FromDigits(
    integral: new[] { 1, 0 },
    fractional: new[] { 6, 2, 5 },
    negative: false,
    baseValue: 10);

Console.WriteLine(value.ToDecimal()); // 10.625
```

Factory methods accept `decimal`, `BigInteger`, `int`, `float`, `double`, and
`Value`. `FromBigInteger` can create the digits directly in a requested base,
and `FromValue` preserves the source base. Conversion methods return primitive
views or another base.

`Value.FromBigInteger` and `Value.ToBigInteger` provide the same
arbitrary-precision round trip for non-negative integral digit sequences.

`NumeralValue` also supports cross-base addition, subtraction, multiplication,
division, sign helpers, and numeric comparison. These operations use an exact
rational intermediate and a bounded destination-base expansion. See
[Arithmetic](arithmetic.md) for the complete contract.

Use `NumeralValue` when the digit lists themselves matter. Use `Numeral` when
you also need custom symbol parsing and formatting.

## Validation and failure behavior

- The base must be at least 2.
- Every digit must be in the range `0..base-1`.
- `Parse` throws `InvalidOperationException` for an invalid textual numeral.
- `TryParse(value, NumeralAlphabet, ...)` returns `ParseResult` with an error
  reason and UTF-16 position.
- `Parse`/`TryParse` accept `IFormatProvider`; `NumeralFormatInfo` supplies a
  validated alphabet and tokens as one immutable object.
- `Numeral` implements standard `G` and invariant `R` formats. Span parsing and
  formatting are available in the .NET 8 asset.
- The legacy `TryParse` overload returns `false` and still assigns a result
  object.

See [Ordered numeral alphabets](numeral-alphabets.md) for all predefined
alphabets, exact integer round trips, and the complete diagnostic contract.
- `SkipUnknownValues` controls whether unknown input symbols are ignored or
  represented by zero while parsing.
- `TryFromIndices`, `TryIntegerOf`, and `TryCharOf` report whether conversion was
  exact through their Boolean result.
