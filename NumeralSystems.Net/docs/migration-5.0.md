# Migrating from 4.8.1 to 5.0.0

Version 5.0 makes exact rational value state authoritative and treats positional
digits as a representation chosen by policy. It retains selected 4.x entry
points with `Obsolete` warnings so applications can migrate incrementally.

## Package and targets

The package continues to target .NET Standard 2.1 and .NET 8. The package ID
and namespace remain `NumeralSystems.Net`. No update to `Polecola.Primitive` is
required for 5.0; that dependency remains at 1.0.0 and continues to support the
primitive and incomplete-bit wrappers.

## Replace digit construction

Before:

```csharp
var value = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    negative: false,
    baseValue: 2);
```

After:

```csharp
var value = NumeralValue.FromDigits(
    integral: new[] { 0 },
    fractional: new[] { 1 },
    negative: false,
    baseValue: 2);
```

Use `FromRational` when the number is naturally a fraction:

```csharp
var value = NumeralValue.FromRational(1, 3, baseValue: 10);
```

## Replace implicit conversion precision

Before:

```csharp
var converted = value.ToBase(2);
var exact = value.TryToBase(2, 64, out converted);
```

After:

```csharp
var options = new NumeralConversionOptions(
    maxFractionalDigits: 64,
    roundingMode: NumeralRoundingMode.ToNearestEven,
    detectRepeatingPeriod: true,
    infiniteBehavior: InfiniteExpansionBehavior.PreservePeriod);

var converted = value.ToBase(2, options);
var expansion = value.Expand(2, options);
var terminating = expansion.IsTerminating;
var periodic = expansion.HasRepeatingPeriod;
```

## Compatibility map

| 4.x API retained with warning | 5.0 replacement |
| --- | --- |
| `new NumeralValue(integral, fractional, negative, base)` | `NumeralValue.FromDigits(...)` or `FromRational(...)` |
| `NumeralValue.ToBase(base, removeFirstZeros)` | `ToBase(base, options)` |
| `NumeralValue.ToBase(base, maxDigits, removeFirstZeros)` | `ToBase(base, new NumeralConversionOptions(...))` |
| `NumeralValue.TryToBase(...)` | `Expand(...).IsTerminating` and period metadata |
| `Numeral.To(system)` | `Numeral.To(system, options)` |
| `Numeral.TrySetValue(...)` | `WithExactValue(...)` |
| setters for `Positive`, digit lists, and primitive views | `FromRational(...)` or `WithExactValue(...)` |

The obsolete members are warnings, not errors. They continue to forward to a
4.x-compatible truncation policy. Projects that treat warnings as errors can
migrate one call site at a time or temporarily suppress `CS0618` around a
specific compatibility call.

## Important behavior change after truncation

In 4.x, converting `1/3` to six decimal digits produced an object whose numeric
state was `333333/1000000`. In 5.0, the visible digits can still be `333333`,
but `ExactValue` remains `1/3`:

```csharp
var truncate = new NumeralConversionOptions(
    6,
    NumeralRoundingMode.ToZero,
    false,
    InfiniteExpansionBehavior.Truncate);

var value = NumeralValue.FromRational(1, 3, 10, truncate);

Console.WriteLine(string.Join("", value.Decimals)); // 333333
Console.WriteLine(value.ExactValue);                 // 1/3
```

Consequently, `ToDecimal`, comparison, later base conversions, and arithmetic
use the exact rational value rather than the visible truncated digits. Code
that intentionally needs the truncated rational can construct it explicitly
from `Integral` and `Decimals` with `RationalValue.FromDigits`.

## `Numeral` collection semantics

`IntegralIndices` and `FractionalIndices` still have `List<int>` return types
for source compatibility, but getters return copies in 5.0. This no longer
mutates the object:

```csharp
var digits = numeral.IntegralIndices;
digits[0] = 9; // modifies only the copy
```

Create a replacement instead:

```csharp
var replacement = numeral.WithExactValue(new RationalValue(9, 1));
```

## Exceptions

Normal argument validation continues to use framework exceptions:

- `ArgumentNullException` for required null inputs;
- `ArgumentOutOfRangeException` for invalid bases, digits, limits, or enum
  values;
- `DivideByZeroException` for a zero rational denominator or divisor;
- `OverflowException` when converting outside a primitive target range.

Expansion policy failures use `NumeralExpansionException`:

- `InfiniteNumeralExpansionException` when infinite expansions are forbidden;
- `NumeralExpansionLimitException` when an exact period is not found within
  the safety limit.

## Nullable annotations

The new exact-value, conversion-option, expansion, and exception APIs carry
nullable reference metadata on both targets. Optional options parameters accept
`null` and select `NumeralConversionOptions.Default`; required values reject
`null` consistently.

## JSON migration

The .NET 8 converter adds two string properties:

```json
{
  "base": 10,
  "positive": true,
  "numerator": "1",
  "denominator": "3",
  "integral": [0],
  "fractional": [3]
}
```

The reader accepts old JSON without the rational fields. If either exact field
is present, both are required; the denominator must be positive. Serializing
and deserializing a 5.0 `Numeral` preserves its exact rational state.

## Recommended migration order

1. Replace `NumeralValue` constructors with `FromDigits`.
2. Define named conversion policies for calculation, display, and protocols.
3. Replace `ToBase` and `TryToBase` compatibility calls.
4. Replace `Numeral` mutation with `FromRational` and `WithExactValue`.
5. Review code that expected truncation to change the numeric value.
6. Regenerate JSON fixtures if exact numerator and denominator should be
   asserted.
7. Re-enable warnings as errors and remove local `CS0618` suppressions.

