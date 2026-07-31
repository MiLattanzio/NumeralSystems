# Migrating to 4.7.0

[Documentation home](index.md) ·
[Arithmetic](arithmetic.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[BitPattern engine](bit-patterns.md) ·
[Troubleshooting](troubleshooting.md) ·
[Changelog](../../CHANGELOG.md)

This guide covers the behavioral corrections introduced in 4.6.0 and the
`BitPattern` and arithmetic APIs added in 4.7.0. Read all sections when
upgrading directly from 4.5.2 or earlier.

## Upgrade checklist

1. Update the package reference to `4.7.0`.
2. Run the complete test suite with representative fractional values.
3. Review code that reads fractional digits in bases other than 10.
4. Decide whether arithmetic operators' default 128-digit limit is acceptable.
5. Use precision-aware overloads where truncation must be observable.
6. Replace primitive intermediate conversions with `BigInteger` or
   `NumeralValue` arithmetic where appropriate.
7. Confirm persistent text uses an explicit alphabet and separators.
8. Replace new uses of unbounded `Incomplete*.Enumerable` with
   `EnumerateCandidates(limit)`.
9. Test ternary `AND`, `OR`, and `NAND` cases where one operand is unknown.
10. Replace `HashSet<string>` alphabets with `NumeralAlphabet`.
11. Use structured `ParseResult` diagnostics where invalid input is expected.

## Ordered alphabets in 4.7.0

`NumeralAlphabet` replaces implicit collection enumeration with an immutable
numeric order:

```csharp
var alphabet = new NumeralAlphabet(orderedSymbols);
var value = Value.FromString(text, alphabet);
```

The `Value.FromString(string, HashSet<string>)` overload remains binary/source
compatible but is marked `Obsolete`. Its remaining behavior sorts the set
ordinally; applications must not rely on a previous runtime-specific set order.

Construction now detects:

- duplicate or empty symbols;
- symbols that are prefixes of other symbols;
- fewer than two symbols.

Parsing/formatting with a `NumeralAlphabet` also detects conflicts with the
digit separator, negative sign, and decimal separator.

### Default text changes

`SerializationInfo.OfBase` now uses predefined alphabets for bases 2, 8, 10,
16, 32, 36, 58, 62, and 64. Other bases use deterministic fixed-width decimal
symbols. Text produced by parameterless `ToString()` can therefore differ from
older versions even though its numeric value is unchanged.

Persisted formats should always retain their explicit alphabet and formatting
tokens. To preserve an older mapping, construct `NumeralAlphabet` from the old
ordered `Identity` list.

### Structured parsing

Use:

```csharp
ParseResult parsed = system.TryParse(text, alphabet);
```

On failure, inspect `Reason`, `Position`, `ErrorLength`, and `Message`. Position
is a zero-based UTF-16 offset. The existing Boolean/out overload remains
available.

## Shared `BitPattern` engine in 4.7.0

All `Incomplete*` wrappers now inherit their new behavior from one immutable
engine:

```csharp
using NumeralSystems.Net.Type.Incomplete;

var pattern = BitPattern.Unknown(64);

Console.WriteLine(pattern.UnknownBitCount); // 64
Console.WriteLine(pattern.CandidateCount);  // 18446744073709551616

foreach (var candidate in pattern.EnumerateCandidates(limit: 10))
{
    // Explicitly bounded.
}
```

Existing wrapper members remain available. The following additions do not
require replacing `IncompleteInt`, `IncompleteLong`, or the other concrete
types:

- immutable `Pattern` snapshots;
- exact `BigInteger` candidate counts and bounds;
- `IsMatch` / `IsSignedMatch`;
- bounded enumeration;
- masks, shifts, rotations, compatibility, and intersection;
- reverse XOR/NAND and `TrySolveAnd`.

`Binary` setters are now public consistently across the incomplete family.

### Ternary logic correction

Nullable array operations now use set-based three-valued logic from the shared
engine:

```text
false AND unknown = false
true  OR  unknown = true
false NAND unknown = true
```

Several `IncompleteLong`, `IncompleteULong`, `IncompleteUInt`, `IncompleteFloat`,
and `IncompleteDouble` `Or` overloads previously delegated to `And`; 4.7.0
corrects those implementations. Add regression tests if an application worked
around the old result.

### Signed and unsigned bounds

`MinValue` and `MaxValue` are unsigned encoded bounds. Use `SignedMinValue`,
`SignedMaxValue`, and `IsSignedMatch` when a signed integer wrapper should use
two's-complement meaning.

The bounds of `IncompleteFloat`, `IncompleteDouble`, and `IncompleteDecimal`
describe raw encodings and are not floating-point numeric intervals.

## Fractional semantics corrected in 4.6.0

Fractional digits now have their standard positional meaning:

```text
0.1 in base 2 = 1 / 2
0.1 in base 3 = 1 / 3
0.1 in base 10 = 1 / 10
```

Earlier versions could interpret the fractional digit sequence as decimal text
regardless of its declared base. Applications that relied on that behavior will
observe different, now mathematically correct, values.

Add regression tests for:

- non-decimal fractions such as binary `0.1`, `0.01`, and `0.101`;
- conversions whose destination expansion repeats;
- negative fractional values;
- maximum accepted fractional length.

## Bounded fractional conversion

The original overload remains available:

```csharp
var converted = value.ToBase(2);
```

It generates at most `NumeralValue.DefaultMaxFractionalDigits` fractional
positions.

Use the new explicit contract when exactness matters:

```csharp
var exact = value.TryToBase(
    baseValue: 2,
    maxFractionalDigits: 256,
    result: out var converted);
```

Or use the `ToBase` overload when truncation is accepted but a custom limit is
needed:

```csharp
var converted = value.ToBase(
    baseValue: 2,
    maxFractionalDigits: 64);
```

## `BigInteger` support

Version 4.6.0 added arbitrary-precision integral paths:

```csharp
var numeral = Numeral.System.OfBase(16)[bigInteger];
BigInteger roundTrip = numeral.BigInteger;

var digits = Value.FromBigInteger(bigInteger, baseValue: 36);
BigInteger decoded = digits.ToBigInteger();
```

Prefer these APIs over a `decimal` or `long` intermediary for large integral
values.

`NumeralValue.ToBigInteger()` truncates fractional digits toward zero. This
matches normal conversion to an integer type.

## New arithmetic in 4.7.0

The following expressions are now supported:

```csharp
var sum = left + right;
var difference = left - right;
var product = left * right;
var quotient = left / right;
var opposite = -left;
```

The result uses the left operand's base and the default fractional digit limit.
The methods offer more control:

```csharp
var result = left.Divide(
    right,
    exact: out var exact,
    resultBase: 16,
    maxFractionalDigits: 128);
```

No operand is mutated.

## Comparison behavior

Use base-independent numeric comparison:

```csharp
var equal = left.NumericallyEquals(right);
var order = left.CompareTo(right);

if (left < right)
{
    // ...
}
```

`object.Equals` and `==` are not changed. They retain reference equality. This
choice avoids changing the behavior of existing dictionaries and hash sets.

## Result sign and zero

Arithmetic results and `Negate()` normalize zero to a non-negative sign:

```csharp
var zero = value - value;

Console.WriteLine(zero.IsZero);   // True
Console.WriteLine(zero.Negative); // False
```

A manually constructed `NumeralValue` can still contain zero digits with
`negative: true`; numeric comparison treats it as zero.

## Leading zeroes

Integral base conversion preserves the existing leading width unless
`removeFirstZeros` is `true`. All-zero values no longer gain an extra zero
during conversion.

If code depended on the accidental extra position, replace that dependency with
an explicit padding step.

## Signed primitive minimum values

Indexers now preserve the full magnitude of:

- `long.MinValue`;
- `int.MinValue`;
- `short.MinValue`;
- `sbyte.MinValue`.

The conversion no longer casts a negative value directly to its unsigned
counterpart.

## Exceptions to account for

| API | Condition | Exception |
| --- | --- | --- |
| arithmetic methods | `other` is `null` | `ArgumentNullException` |
| arithmetic result conversion | result base smaller than 2 | `ArgumentOutOfRangeException` |
| arithmetic result conversion | negative fractional limit | `ArgumentOutOfRangeException` |
| `Divide` or `/` | divisor is zero | `DivideByZeroException` |
| bounded primitive view | value outside target range | overflow exception from the target type |
| `BitPattern.FromUnsigned` | negative value, negative width, or value wider than the width | `ArgumentOutOfRangeException` |
| bit-pattern binary operation | operand widths differ | `ArgumentException` |
| `EnumerateCandidates` | negative limit | `ArgumentOutOfRangeException` |
| throwing intersection/reverse/solver | constraint has no solution | `InvalidOperationException` |
| `NumeralAlphabet` construction | duplicate, empty, or prefix-ambiguous symbols | `ArgumentException` |
| alphabet formatting | separator/sign conflicts with symbols | `ArgumentException` |
| `NumeralAlphabet.Decode` | invalid numeral text | `FormatException` |
| `NumeralSystem.Parse` with alphabet | unsuccessful `ParseResult` | `InvalidOperationException` |

A repeating result is not exceptional. It is returned with `exact == false`.

## Recommended migration tests

Add at least these cases to an application test suite:

```text
binary 0.1 -> decimal 0.5
decimal 10.625 -> binary 1010.101
base-3 0.1 -> base-10 repeating expansion
long.MinValue -> numeral -> BigInteger
large BigInteger -> base 36 -> BigInteger
cross-base addition
division by three with a bounded result
comparison of equal values in bases 2 and 10
zero sign after subtraction
bounded enumeration of a pattern with 64 unknown bits
false AND unknown and true OR unknown
compatible and contradictory pattern intersections
logical/arithmetic shifts and rotations
reverse XOR and reverse NAND
x & mask == result, including an impossible result
signed BigInteger encode/decode for every application alphabet
pairwise conversion among all predefined bases
duplicate and ambiguous-prefix alphabet rejection
parse error reason and UTF-16 position
```

See the repository's `NumeralValueArithmeticTests` and
`BitPatternTests` and `NumeralAlphabetTests` for executable examples.
