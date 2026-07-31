# Arithmetic with `NumeralValue`

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
lists are not modified.

## Numeric model

A finite positional value is treated internally as an exact rational number.
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
arbitrary-precision integers before performing an operation. Only the final
conversion to the requested result base can require truncation.

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
var oneHalf = new NumeralValue(
    integral: new List<int> { 0 },
    decimals: new List<int> { 1 },
    negative: false,
    baseValue: 2);

var oneQuarter = new NumeralValue(
    integral: new List<int> { 0 },
    decimals: new List<int> { 1 },
    negative: false,
    baseValue: 4);

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

var binaryQuarter = one.Divide(
    four,
    exact: out var exact,
    resultBase: 2,
    maxFractionalDigits: 16);

Console.WriteLine(exact);                              // True
Console.WriteLine(string.Concat(binaryQuarter.Decimals)); // 01
```

Choosing a suitable result base can turn a repeating expansion into a finite
one. One quarter terminates in bases 2, 4, 8, 10, 16, and many others, but one
third does not terminate in base 10.

## Exact and truncated results

The short methods and operators use
`NumeralValue.DefaultMaxFractionalDigits`, currently 128. Use an overload with
`out bool exact` when truncation must be observable:

```csharp
var one = NumeralValue.FromInt(1);
var three = NumeralValue.FromInt(3);

var result = one.Divide(
    three,
    exact: out var exact,
    resultBase: 10,
    maxFractionalDigits: 6);

Console.WriteLine(exact);                        // False
Console.WriteLine(string.Concat(result.Decimals)); // 333333
```

`exact == false` does not mean the operation failed. It means the exact rational
result has a non-terminating expansion, or needs more positions than the
supplied limit. The returned value contains the truncated expansion.

The conversion policy is truncation toward zero. It does not round the final
digit.

Use these patterns:

- operators for concise calculations where the default precision is accepted;
- methods with `out bool exact` for persistence, validation, finance, protocol
  fields, or any calculation where truncation must be handled explicitly;
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
var binaryHalf = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    false,
    2);

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
| result does not terminate within the limit | result returned with `exact == false` |
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
