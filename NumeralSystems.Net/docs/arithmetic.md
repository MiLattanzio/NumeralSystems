# Arithmetic with `NumeralValue`

[Exact rational values](exact-rationals.md)

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Numeral systems](numeral-systems.md) ·
[API reference](api-reference.md) ·
[Cookbook](cookbook.md)

## Overview

`NumeralValue` supports signed arithmetic without first converting operands to
`int`, `long`, `decimal`, or `double`. This matters in two situations:

- integral values can be larger than every primitive integer type;
- operands can use different numeral bases.

The supported operations are:

| Operation | Method | Operator |
| --- | --- | --- |
| Addition | `Add` | `+` |
| Subtraction | `Subtract` | `-` |
| Multiplication | `Multiply` | `*` |
| Division | `Divide` | `/` |
| Sign inversion | `Negate` | unary `-` |
| Absolute value | `Abs` | — |
| Comparison | `CompareTo`, `NumericallyEquals` | `<`, `>`, `<=`, `>=` |

Every operation creates a new `NumeralValue`. The input objects and their digit
lists are not modified. In 5.0 the normalized `ExactValue` is authoritative;
the digits are only a projection selected by `NumeralConversionOptions`.

## Numeric model

A positional value is stored internally as an exact rational number, including
when its current base has an infinite repeating expansion.
For integral digits `I`, fractional digits `F`, base `b`, and `n` fractional
positions:

```text
value = sign × (I + F / bⁿ)
```

For example, the binary digits `10.101` mean:

```text
2 + 5 / 2³ = 2.625
```

The library reduces the rational numerator and denominator with
arbitrary-precision integers before performing an operation. A final rounded or
truncated projection does not replace that exact state. See
[Exact rational values and positional expansions](exact-rationals.md).

## Basic arithmetic

Factory methods are a convenient way to create base-10 operands:

```csharp
using NumeralSystems.Net;

var price = NumeralValue.FromDecimal(19.95m);
var shipping = NumeralValue.FromDecimal(4.50m);

var total = price.Add(shipping);

Console.WriteLine(total.ToDecimal()); // 24.45
Console.WriteLine(total.Base);        // 10
```

The result uses the base of the left operand unless another base is requested.

The operator form is shorter and follows the same rule:

```csharp
var subtotal = NumeralValue.FromDecimal(24.45m);
var discount = NumeralValue.FromDecimal(2.45m);

var result = subtotal - discount;

Console.WriteLine(result.ToDecimal()); // 22.00
```

## Arithmetic across different bases

Operands do not need the same base. This example adds one half in base 2 and
one quarter in base 4:

```csharp
var oneHalf = NumeralValue.FromRational(1, 2, baseValue: 2);
var oneQuarter = NumeralValue.FromRational(1, 4, baseValue: 4);

var sum = oneHalf + oneQuarter;

Console.WriteLine(sum.Base);                    // 2
Console.WriteLine(string.Concat(sum.Decimals)); // 11
Console.WriteLine(sum.ToDecimal());             // 0.75
```

`sum` is in base 2 because `oneHalf` is the left operand. Reversing the
operands produces an equivalent value represented in base 4.

## Choosing the result base

The precision-aware overloads accept `resultBase`:

```csharp
var one = NumeralValue.FromInt(1);
var four = NumeralValue.FromInt(4);

var finiteOnly = new NumeralConversionOptions(
    16,
    NumeralRoundingMode.ToNearestEven,
    true,
    InfiniteExpansionBehavior.Throw);
var binaryQuarter = one.Divide(four, finiteOnly, resultBase: 2);

Console.WriteLine(binaryQuarter.IsExactRepresentation); // True
Console.WriteLine(string.Concat(binaryQuarter.Decimals)); // 01
```

Choosing a suitable result base can turn a repeating expansion into a finite
one. One quarter terminates in bases 2, 4, 8, 10, 16, and many others, but one
third does not terminate in base 10.

## Exact, periodic, and bounded results

Use an immutable policy to state whether an infinite expansion should preserve
its period, throw, truncate, or round:

```csharp
var one = NumeralValue.FromInt(1);
var three = NumeralValue.FromInt(3);

var options = new NumeralConversionOptions(
    6,
    NumeralRoundingMode.ToZero,
    false,
    InfiniteExpansionBehavior.Truncate);
var result = one.Divide(three, options, resultBase: 10);

Console.WriteLine(result.IsExactRepresentation); // False
Console.WriteLine(string.Concat(result.Decimals)); // 333333
Console.WriteLine(result.ExactValue);              // 1/3
```

The digit list is truncated, but calculation, comparison, `ToDecimal`, and the
next base conversion use `ExactValue`. Select `Round` and a rounding mode for a
rounded projection. Select `PreservePeriod` to obtain exact cycle metadata.

The 4.x overloads with `out bool exact` remain for source compatibility. The
flag still reports whether the projected digits terminate within the supplied
limit, and those overloads use truncation toward zero.

Use these patterns:

- operators for concise calculations whose result terminates within the
  compatibility limit;
- methods with `NumeralConversionOptions` for persistence, validation, finance,
  protocol fields, and every explicit display boundary;
- a result base whose prime factors match the expected denominators when exact
  finite output is required.

## Addition and subtraction

Addition and subtraction use a common rational denominator. Signs are handled
without changing either operand:

```csharp
var positive = NumeralValue.FromDecimal(2.25m);
var negative = NumeralValue.FromDecimal(-3.5m);

var sum = positive + negative;
var difference = positive - negative;

Console.WriteLine(sum.ToDecimal());        // -1.25
Console.WriteLine(difference.ToDecimal()); // 5.75
```

Subtraction can create a negative result even if both operands are non-negative.
An exact zero result is normalized to a non-negative sign.

## Multiplication

Multiplication multiplies the exact rational numerators and denominators:

```csharp
var left = NumeralValue.FromDecimal(-1.5m);
var right = NumeralValue.FromDecimal(2m);

var product = left * right;

Console.WriteLine(product.ToDecimal()); // -3.0
```

Integral multiplication is not limited by `long` or `ulong`:

```csharp
using System.Numerics;

var magnitude = BigInteger.Pow(2, 300);
var large = NumeralValue.FromBigInteger(magnitude, baseValue: 16);

var product = large * NumeralValue.FromInt(8);

Console.WriteLine(product.ToBigInteger() == magnitude * 8); // True
```

## Division

Division throws `DivideByZeroException` when every digit of the divisor is zero:

```csharp
var numerator = NumeralValue.FromInt(10);
var zero = NumeralValue.FromInt(0);

// Throws DivideByZeroException.
var invalid = numerator / zero;
```

For non-zero divisors, the exact rational quotient is calculated first. The
fractional digit limit applies only while expressing that quotient in the
result base.

## Sign helpers

`Negate` and unary `-` return a value with the opposite sign:

```csharp
var value = NumeralValue.FromDecimal(-12.5m);

Console.WriteLine(value.Negate().ToDecimal()); // 12.5
Console.WriteLine((-value).ToDecimal());       // 12.5
```

`Abs` returns a non-negative magnitude:

```csharp
var magnitude = value.Abs();
Console.WriteLine(magnitude.ToDecimal()); // 12.5
```

Negating zero produces non-negative zero.

## Numeric comparison

Digit lists that look different can represent the same number. Use
`CompareTo` or `NumericallyEquals` for a base-independent comparison:

```csharp
var binaryHalf = NumeralValue.FromRational(1, 2, baseValue: 2);

var decimalHalf = NumeralValue.FromDecimal(0.5m);

Console.WriteLine(binaryHalf.NumericallyEquals(decimalHalf)); // True
Console.WriteLine(binaryHalf.CompareTo(decimalHalf));          // 0
Console.WriteLine(binaryHalf <= decimalHalf);                  // True
```

`NumericallyEquals` is intentionally separate from `object.Equals`.
`NumeralValue` does not replace reference equality, so existing dictionary and
set behavior remains unchanged.

## Error behavior

| Condition | Result |
| --- | --- |
| `other` is `null` | `ArgumentNullException` |
| `resultBase < 2` | `ArgumentOutOfRangeException` |
| `maxFractionalDigits < 0` | `ArgumentOutOfRangeException` |
| divisor is zero | `DivideByZeroException` |
| infinite expansion is forbidden | `InfiniteNumeralExpansionException` |
| period is not found within the exact limit | `NumeralExpansionLimitException` |
| explicit truncate/round reaches the limit | inexact digit projection; exact rational retained |
| conversion to `decimal` exceeds its range | `OverflowException` |

The arithmetic itself uses `BigInteger`; primitive overflow is only possible
when a caller explicitly requests a bounded primitive view such as
`ToDecimal()` or `ToInt()`.

## Performance notes

The cost depends on:

- the number of source digits;
- the size of the exact rational numerator and denominator;
- `maxFractionalDigits`;
- whether the destination expansion terminates early.

Avoid requesting thousands of fractional digits unless the application needs
them. Reuse operands instead of reconstructing digit lists in hot loops.

BenchmarkDotNet coverage is available in
`NumeralSystems.Net.Benchmarks/NumeralValueArithmeticBenchmarks.cs`.

Continue with the [cookbook](cookbook.md) for task-oriented examples or the
[API reference](api-reference.md) for the complete member catalog.
