# Numeral systems and base conversion

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
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
`Numeral` stores a value in one system and exposes views such as `Integer`,
`Decimal`, `Double`, `Float`, `Char`, and `Bytes`.

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

Pass an ordered `IList<string>` whose position is the numeric value of a digit:

```csharp
var dozenal = Numeral.System.OfBase(12);
dozenal.AdjustToFitIntegralLength = false;

var identity = "0123456789XY"
    .Select(c => c.ToString())
    .ToList();

var numeral = dozenal[143];
var text = numeral.ToString(
    identity,
    separator: "",
    negativeSign: "-",
    numberDecimalSeparator: ".");

Console.WriteLine(text); // YY

var roundTrip = dozenal.Parse(
    text,
    identity,
    separator: "",
    negativeSign: "-",
    numberDecimalSeparator: ".");

Console.WriteLine(roundTrip.Integer); // 143
```

Symbols may contain more than one character. When symbols are ambiguous or
variable length, use a non-empty digit separator:

```csharp
var identity = new List<string> { "zero", "one", "two" };
var ternary = Numeral.System.OfBase(3);
ternary.AdjustToFitIntegralLength = false;

var text = ternary[5].ToString(identity, "|", "-", ".");
Console.WriteLine(text); // one|two
```

Avoid reusing the negative sign, decimal separator, or digit separator as a
digit symbol.

## Default serialization

`Numeral.ToString()` and `NumeralSystem.Parse(string)` call
`NumeralSystem.SerializationInfo.OfBase`. That method:

- builds an identity from printable characters;
- reads the negative sign and decimal separator from the current culture;
- chooses a separator when the base is larger than the printable identity.

This is convenient for display, but explicit settings are safer for files,
tests, hashes, and network protocols.

You can retain a configuration:

```csharp
var format = new NumeralSystem.SerializationInfo
{
    Identity = "0123456789ABCDEF".Select(c => c.ToString()).ToList(),
    Separator = "",
    NegativeSign = "-",
    NumberDecimalSeparator = "."
};

var hexadecimal = Numeral.System.OfBase(16);
var parsed = hexadecimal.Parse("7B", format);
```

## Converting a `Numeral`

`Numeral.To` converts through the numeric value and returns a new numeral in the
target system:

```csharp
var decimalSystem = Numeral.System.OfBase(10);
var hexadecimal = Numeral.System.OfBase(16);

var source = decimalSystem[65535];
var destination = source.To(hexadecimal);

Console.WriteLine(destination); // FFFF
```

Floating-point and decimal conversions are bounded by the range and precision
of the underlying .NET type. Test round trips for values where exact fractional
representation matters.

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

`Value.FromString(string, HashSet<string>)` creates an identity by enumerating
the set. That enumeration order defines the numeric value of each symbol. Use a
stable set construction and keep the same mapping for decoding.

`Value.FromString(string, bool)` treats UTF-16 character values as digit
indices. With `fit: true`, the instance uses the smallest base that can contain
the largest character index in that input.

`Value` does not preserve a sign or fractional digits.

## Using `NumeralValue`

`NumeralValue` keeps integral digits, fractional digits, a sign, and a base:

```csharp
var value = new NumeralValue(
    integral: new List<int> { 1, 0 },
    decimals: new List<int> { 6, 2, 5 },
    negative: false,
    baseValue: 10);

Console.WriteLine(value.ToDecimal()); // 10.625
```

Factory methods accept `decimal`, `BigInteger`, `int`, `float`, `double`, and
`Value`. Conversion methods return those primitive views or another base.

Use `NumeralValue` when the digit lists themselves matter. Use `Numeral` when
you also need custom symbol parsing and formatting.

## Validation and failure behavior

- The base must be at least 2.
- Every digit must be in the range `0..base-1`.
- `Parse` throws `InvalidOperationException` for an invalid textual numeral.
- `TryParse` returns `false` and still assigns a result object.
- `SkipUnknownValues` controls whether unknown input symbols are ignored or
  represented by zero while parsing.
- `TryFromIndices`, `TryIntegerOf`, and `TryCharOf` report whether conversion was
  exact through their Boolean result.
