# Incomplete values and reverse operations

[Documentation home](index.md) ·
[Bitwise values](bitwise-values.md) ·
[BitPattern engine](bit-patterns.md) ·
[Cookbook](cookbook.md) ·
[Troubleshooting](troubleshooting.md) ·
[API reference](api-reference.md)

An incomplete value represents a set of primitive values with one ternary bit
array:

- `false`: the bit is known to be zero;
- `true`: the bit is known to be one;
- `null`: either value is allowed.

This model is useful for partial binary data and for the result of solving a
logical operation backwards.

Version 4.7 centralizes these rules in the immutable
[`BitPattern`](bit-patterns.md) engine. The existing `Incomplete*` classes keep
their constructors and legacy members while inheriting the new shared API.

## Supported types

The `NumeralSystems.Net.Type.Incomplete` namespace contains:

| Incomplete type | Complete counterpart |
| --- | --- |
| `IncompleteByte` | `Type.Base.Byte` |
| `IncompleteChar` | `Type.Base.Char` |
| `IncompleteShort` | `Type.Base.Short` |
| `IncompleteUShort` | `Type.Base.UShort` |
| `IncompleteInt` | `Type.Base.Int` |
| `IncompleteUInt` | `Type.Base.UInt` |
| `IncompleteLong` | `Type.Base.Long` |
| `IncompleteULong` | `Type.Base.ULong` |
| `IncompleteFloat` | `Type.Base.Float` |
| `IncompleteDouble` | `Type.Base.Double` |
| `IncompleteDecimal` | `Type.Base.Decimal` |
| `IncompleteByteArray` | a sequence of incomplete bytes |

## Create a pattern

Start from a complete wrapper:

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var complete = new IntValue { Value = 10 };
var pattern = complete.Incomplete();

pattern.Binary[0] = null;
pattern.Binary[1] = null;

Console.WriteLine(pattern.IsComplete);   // False
Console.WriteLine(pattern.Permutations); // 4
Console.WriteLine(pattern.UnknownBitCount); // 2
Console.WriteLine(pattern.CandidateCount);  // 4 as BigInteger
Console.WriteLine(pattern.ToString("?"));
```

Or construct an incomplete type directly:

```csharp
using NumeralSystems.Net.Type.Incomplete;

var pattern = new IncompleteByte
{
    Binary = new bool?[]
    {
        null, true, false, false,
        false, false, false, false
    }
};
```

As with complete wrappers, bit index zero is the least-significant bit.

## Test membership

`Contains` answers whether a complete or incomplete value is compatible with a
pattern:

```csharp
using ByteValue = NumeralSystems.Net.Type.Base.Byte;
using NumeralSystems.Net.Type.Incomplete;

var pattern = new IncompleteByte
{
    Binary = new bool?[]
    {
        null, true, false, false,
        false, false, false, false
    }
};

Console.WriteLine(pattern.Contains(new ByteValue { Value = 2 })); // True
Console.WriteLine(pattern.Contains(new ByteValue { Value = 3 })); // True
Console.WriteLine(pattern.Contains(new ByteValue { Value = 0 })); // False
```

Two incomplete patterns are compatible when no position requires conflicting
known bits. New code can make that intent explicit with `IsCompatibleWith` and
can combine constraints with `TryIntersect` or `Intersect`.

## Enumerate candidates

`CandidateCount` is the exact count implied by the unknown bits and uses
`BigInteger`. Unlike the older fixed-width `Permutations` property, it cannot
overflow at 32 or 64 bits.

New code should enumerate with an explicit upper bound:

```csharp
foreach (var encodedCandidate in pattern.EnumerateCandidates(limit: 100))
{
    Console.WriteLine(encodedCandidate);
}
```

The method yields at most `limit` unsigned encoded `BigInteger` values. The
candidate count grows as `2^unknownBits`, so a bounded API prevents an
accidental traversal of billions of combinations.

The indexer and `Enumerable` remain available for compatibility and materialize
the wrapper's complete counterpart. Prefer `IsMatch`, `IsSignedMatch`, or
`Contains` when only membership is needed.

## Bounds

Every incomplete wrapper exposes both encoded and signed bounds:

```csharp
Console.WriteLine(pattern.MinValue);
Console.WriteLine(pattern.MaxValue);
Console.WriteLine(pattern.SignedMinValue);
Console.WriteLine(pattern.SignedMaxValue);
```

`MinValue` and `MaxValue` interpret the bits as an unsigned encoding.
`SignedMinValue` and `SignedMaxValue` use two's complement. For floating-point
and decimal wrappers, these are bit-pattern bounds rather than mathematical
numeric bounds.

## Reverse `AND`

Given a result and a known right operand, reverse `AND` recovers the set of
possible left operands:

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var originalLeft = new IntValue { Value = 0b1100 };
var right = new IntValue { Value = 0b1010 };
var result = originalLeft.And(right);

var solved = result.ReverseAnd(right, out var possibleLeft);

Console.WriteLine(solved);                     // True
Console.WriteLine(possibleLeft.Contains(originalLeft)); // True
```

For each bit:

- if `right` is one, the left bit must equal the result bit;
- if `right` is zero, the result must be zero and the left bit is unconstrained;
- a result bit of one with a right bit of zero is impossible.

When no solution exists, the method returns `false` and the output result is
`null`.

## Reverse `OR`

Reverse `OR` uses the complementary rules:

- if `right` is zero, the left bit must equal the result bit;
- if `right` is one, the result must be one and the left bit is unconstrained;
- a result bit of zero with a right bit of one is impossible.

```csharp
var result = originalLeft.Or(right);

if (result.ReverseOr(right, out var possibleLeft))
{
    Console.WriteLine(possibleLeft.Contains(originalLeft)); // True
}
```

Both the result and right operand can be incomplete. The library propagates
unknown bits through its three-valued logical rules.

## Reverse `XOR` and `NAND`

The shared wrapper API also solves:

```text
left XOR right = result
left NAND right = result
```

```csharp
var left = new NumeralSystems.Net.Type.Base.Int { Value = 0b1100 }.Incomplete();
var right = new NumeralSystems.Net.Type.Base.Int { Value = 0b1010 }.Incomplete();
var xorResult = left.Xor(right);

if (xorResult.ReverseXor(right, out var possibleLeft))
{
    Console.WriteLine(possibleLeft.IsMatch(0b1100)); // True
}
```

`ReverseNand` has the same Boolean-returning shape. The direct `BitPattern`
engine additionally offers throwing `ReverseXor` and `ReverseNand` methods.

## Shifts, rotations, and masks

Each `Incomplete*` wrapper now exposes:

- `LogicalShiftLeft` and `ArithmeticShiftLeft`;
- `LogicalShiftRight` and `ArithmeticShiftRight`;
- `RotateLeft` and `RotateRight`;
- `ApplyMask`.

The width never changes. Logical shifts fill with known zeroes, arithmetic right
shift extends the highest bit, and rotations wrap bits. Unknown states move
with their positions.

```csharp
var mask = pattern;
var shifted = pattern.LogicalShiftRight(3);
var rotated = pattern.RotateLeft(5);
var masked = pattern.ApplyMask(mask);
```

## Solve mask constraints

Treat the current wrapper as the desired result and call `TrySolveAnd`:

```csharp
var mask = new NumeralSystems.Net.Type.Base.Int
{
    Value = unchecked((int)0xFFFF0000)
}.Incomplete();

var result = new NumeralSystems.Net.Type.Base.Int
{
    Value = unchecked((int)0x12340000)
}.Incomplete();

if (result.TrySolveAnd(mask, out var x))
{
    Console.WriteLine(x.UnknownBitCount); // 16
}
```

This represents `x & mask == result`. It returns `false` when the result
requires a one where the complete mask has a zero.

## Logical operations on patterns

Incomplete values support `Not`, `And`, `Or`, `Xor`, `Nand`, reverse
`And`/`Or`/`Xor`/`Nand`, masks, shifts, rotations, and pattern intersection.
Operations return new objects.

When a protocol or algorithm gives unknown bits a meaning other than "either
zero or one," convert that state before using these APIs.

For the complete engine reference, safety rules, exception behavior, and the
independent-bit expressiveness limit, see [BitPattern engine](bit-patterns.md).
