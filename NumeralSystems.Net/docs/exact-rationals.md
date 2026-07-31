# Exact rational values and positional expansions

Version 5.0 separates a number from one of its written representations. The
number is stored as a normalized `RationalValue`; a `NumeralExpansion` is a
projection of that value in a requested base under an explicit conversion
policy.

This distinction matters whenever a fraction does not terminate. The value
`1/3` remains exactly `1/3` even if a decimal projection is limited to six
digits and displays `0.333333`.

## The exact value model

`RationalValue` is immutable. Its numerator and denominator are arbitrary-size
`BigInteger` values. Construction enforces these invariants:

- the denominator is positive;
- numerator and denominator are relatively prime;
- zero is normalized to `0/1`;
- a zero denominator throws `DivideByZeroException`.

```csharp
using System.Numerics;
using NumeralSystems.Net;

var value = new RationalValue(new BigInteger(6), new BigInteger(-8));

Console.WriteLine(value.Numerator);   // -3
Console.WriteLine(value.Denominator); // 4
Console.WriteLine(value);             // -3/4
```

Factories preserve the exact source model:

```csharp
var integer = RationalValue.FromInteger(BigInteger.Pow(2, 256));
var money = RationalValue.FromDecimal(12.50m); // 25/2
var ieee = RationalValue.FromDouble(0.1d);     // exact IEEE 754 value, not 1/10
var digits = RationalValue.FromDigits(
    integral: new[] { 1, 0 },
    fractional: new[] { 1 },
    negative: false,
    baseValue: 2);                              // 5/2
```

`FromSingle` and `FromDouble` reject NaN and infinities. They decode the IEEE
754 bits directly. `FromDecimal` decodes the 96-bit coefficient and scale
directly.

## Exact examples across bases

No conversion in this section passes through `decimal`, `double`, or another
primitive type.

```csharp
var half = NumeralValue.FromRational(1, 2, baseValue: 10);
var binaryHalf = half.ToBase(2, NumeralConversionOptions.Default);

Console.WriteLine(binaryHalf.Decimals[0]); // 1: 0.1 base 2
Console.WriteLine(binaryHalf.ExactValue);  // 1/2

var thirdInBaseThree = NumeralValue.FromRational(1, 3, baseValue: 3);
Console.WriteLine(thirdInBaseThree.Decimals[0]); // 1: 0.1 base 3
```

Decimal one tenth has a finite decimal spelling but a repeating binary
spelling:

```csharp
var oneTenth = NumeralValue.FromDecimal(0.1m);
var binary = oneTenth.Expand(2);

Console.WriteLine(binary.IsTerminating);       // False
Console.WriteLine(binary.RepeatingStartIndex); // 1
Console.WriteLine(binary.RepeatingLength);     // 4
Console.WriteLine(binary.ToString(NumeralAlphabet.Base2)); // 0.0(0011)
```

The parenthesized digits repeat. Therefore `0.0(0011)` in base 2 is exactly
`1/10`, not a rounded approximation.

## Conversion options

`NumeralConversionOptions` is immutable and contains every policy that can
change an expansion:

| Property | Meaning |
| --- | --- |
| `MaxFractionalDigits` | Safety limit for generated fractional digits |
| `RoundingMode` | Rule used by the `Round` behavior |
| `DetectRepeatingPeriod` | Track remainders to identify a cycle |
| `InfiniteBehavior` | Preserve, reject, truncate, or round an infinite expansion |

The default is exact-first: at most 128 fractional digits, nearest-even
rounding when requested, period detection enabled, and
`InfiniteExpansionBehavior.PreservePeriod`. If a terminating tail or a period
cannot be completed within 128 digits, the default throws
`NumeralExpansionLimitException` rather than silently losing information.

Create a policy explicitly for protocol or user-interface boundaries:

```csharp
var displayPolicy = new NumeralConversionOptions(
    maxFractionalDigits: 8,
    roundingMode: NumeralRoundingMode.ToNearestEven,
    detectRepeatingPeriod: false,
    infiniteBehavior: InfiniteExpansionBehavior.Round);

var display = NumeralValue.FromRational(1, 3, 10, displayPolicy);
Console.WriteLine(string.Join("", display.Decimals)); // 33333333
Console.WriteLine(display.WasRounded);                 // True
Console.WriteLine(display.ExactValue);                 // still 1/3
```

Use `WithMaxFractionalDigits`, `WithRoundingMode`, and
`WithInfiniteBehavior` to derive a new policy. The original options instance is
never modified.

## Infinite-expansion behavior

`PreservePeriod`

: Stops when a remainder repeats and stores the period start and length. This
  is exact. If no period is found within the digit limit, it throws
  `NumeralExpansionLimitException`.

`Throw`

: Throws `InfiniteNumeralExpansionException` when the expansion has not
  terminated. Use this for formats that only allow finite positional values.

`Truncate`

: Stops at the digit limit. When period detection is disabled, the visible
  digits are inexact but `ExactValue` remains unchanged.

`Round`

: Stops at the digit limit and applies `RoundingMode`. The exact rational value
  remains available separately from the rounded digit projection.

## Rounding modes

The available modes are:

- `ToZero`;
- `AwayFromZero`;
- `ToNegativeInfinity`;
- `ToPositiveInfinity`;
- `ToNearestEven`;
- `ToNearestAwayFromZero`.

Rounding propagates carries through fractional and integral digits. For
example, rounding `999/1000` to two base-10 fractional digits produces `1.00`,
while `ExactValue` remains `999/1000`.

## Expansion metadata

`NumeralExpansion` exposes:

- `Value`, the immutable exact rational;
- `Base`;
- `IntegralDigits` and `FractionalDigits` as read-only collections;
- `IsTerminating` and `IsExact`;
- `RepeatingStartIndex`, `RepeatingLength`, and `HasRepeatingPeriod`;
- `WasRounded`.

Call `RationalValue.Expand` or `NumeralValue.Expand` when metadata is more
important than obtaining another `NumeralValue`.

## Arithmetic stays exact

All `RationalValue` arithmetic normalizes its result. `NumeralValue` arithmetic
uses `ExactValue`, not the currently displayed digit prefix.

```csharp
var truncate = new NumeralConversionOptions(
    6,
    NumeralRoundingMode.ToZero,
    false,
    InfiniteExpansionBehavior.Truncate);

var decimalThird = NumeralValue.FromRational(1, 3, 10, truncate);
var exactOne = decimalThird.Multiply(
    NumeralValue.FromRational(3, 1),
    NumeralConversionOptions.Default,
    resultBase: 10);

Console.WriteLine(exactOne.ExactValue); // 1
```

The compatibility arithmetic overloads that return `out bool exact` keep
their 4.x meaning: the flag reports whether the displayed fractional digits
terminated within the requested limit. The returned object still retains the
exact rational value in 5.0.

## `Numeral` and immutable replacement

Use `Numeral.FromRational` to create a formatting-oriented numeral while
retaining the exact value:

```csharp
var system = Numeral.System.OfBase(10);
system.AdjustToFitIntegralLength = false;

var numeral = Numeral.FromRational(system, new RationalValue(1, 3));
var changed = numeral.WithExactValue(new RationalValue(2, 3));

Console.WriteLine(numeral.ExactValue); // 1/3
Console.WriteLine(changed.ExactValue); // 2/3
```

The 4.x mutating setters remain available with `Obsolete` migration warnings.
Digit getters return copies, so modifying a returned `List<int>` no longer
changes the numeral. Prefer replacement through `FromRational`,
`WithExactValue`, and `To(system, options)`.

## Decimal conversion

`RationalValue.ToDecimal` scales and rounds the ratio before constructing a
`decimal`. It does not cast a potentially huge numerator and denominator
separately. Values outside the final `decimal` range throw `OverflowException`.
The optional rounding mode uses the same enum as positional expansion.

## JSON on .NET 8

`NumeralJsonConverter` writes the exact numerator and denominator as decimal
strings in addition to the base, sign, and digit arrays. Strings avoid JSON
number precision limits. The reader still accepts 4.8 JSON that contains only
digits; 5.0 JSON round-trips a rational value even when its displayed digits
are a finite projection of a repeating expansion.

## Safety and performance

Period detection stores one dictionary entry per distinct remainder and is
therefore bounded by `MaxFractionalDigits`. Choose limits deliberately for
untrusted denominators. `Truncate` and `Round` with detection disabled use
constant auxiliary state. Integral conversion and arithmetic remain
arbitrary-precision operations and can consume memory proportional to input
size.

