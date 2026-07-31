# BitPattern engine

[Documentation home](index.md) ·
[Incomplete values](incomplete-values.md) ·
[Bitwise values](bitwise-values.md) ·
[Cookbook](cookbook.md) ·
[API reference](api-reference.md)

`BitPattern` is the shared, immutable engine behind the `IncompleteByte`,
`IncompleteInt`, `IncompleteLong`, and other incomplete primitive wrappers. It
models a fixed-width set of binary values without enumerating that set.

```csharp
using System.Numerics;
using NumeralSystems.Net.Type.Incomplete;

var pattern = new BitPattern(new bool?[]
{
    null, true, false, null // least-significant bit first
});

Console.WriteLine(pattern);                 // ?0?1
Console.WriteLine(pattern.UnknownBitCount); // 2
Console.WriteLine(pattern.CandidateCount);  // 4
Console.WriteLine(pattern.MinValue);        // 2
Console.WriteLine(pattern.MaxValue);        // 11
Console.WriteLine(pattern.IsMatch(10));     // True
```

## Representation and set semantics

Each position has one of three states:

| Stored value | Meaning |
| --- | --- |
| `false` | only zero is allowed |
| `true` | only one is allowed |
| `null` | both zero and one are allowed |

Arrays passed to the constructor are least-significant bit first:
`pattern[0]` is the bit with weight `2^0`. `ToString()` displays the familiar
most-significant-bit-first form and writes unknown bits as `?`.

The constructor copies its input, `ToArray()` returns a defensive copy, and
every operation creates a new pattern. A `BitPattern` is therefore safe to
share as an immutable value. The legacy `Incomplete*` wrappers remain mutable;
their `Pattern` property returns an immutable snapshot.

Complete values can be created without manually expanding bits:

```csharp
var mask = BitPattern.FromUnsigned(0b1111_0000, width: 8);
var anyByte = BitPattern.Unknown(width: 8);
var readable = BitPattern.Parse("1100_????");
```

`FromUnsigned` rejects negative values and values that do not fit the requested
width. `Parse` and `TryParse` read the same MSB-first form produced by
`ToString`; whitespace and `_` separators are ignored, while any other
character is rejected.

## Size, bounds, and membership

| Member | Meaning |
| --- | --- |
| `Count` | fixed bit width |
| `UnknownBitCount` | number of independent unknown positions |
| `CandidateCount` | exact `2^UnknownBitCount` as `BigInteger` |
| `MinValue` / `MaxValue` | unsigned encoded bounds |
| `SignedMinValue` / `SignedMaxValue` | two's-complement bounds |
| `IsMatch(value)` | membership using unsigned encoding |
| `IsSignedMatch(value)` | membership using two's complement |

The unsigned and signed methods deliberately have different names. For an
eight-bit all-one pattern, `MinValue` is `255`, while `SignedMinValue` is `-1`.
For `Float`, `Double`, and `Decimal` wrappers, bounds describe encoded bits,
not a floating-point numeric interval.

`CandidateCount` never wraps at 32 or 64 bits:

```csharp
var pattern128 = BitPattern.Unknown(128);
BigInteger count = pattern128.CandidateCount; // 2^128
```

## Bounded candidate enumeration

Use `EnumerateCandidates(limit)` when concrete values are required:

```csharp
foreach (BigInteger candidate in pattern128.EnumerateCandidates(limit: 10))
{
    // At most ten iterations, even though CandidateCount is 2^128.
}
```

The limit is an upper bound, not a prerequisite:

- zero returns an empty sequence;
- a limit smaller than `CandidateCount` returns exactly that many candidates;
- a larger limit returns every candidate;
- a negative limit throws `ArgumentOutOfRangeException`.

Enumeration is lazy and deterministic. Unknown positions form a binary counter
from the lowest unknown bit to the highest. Use `EnumerateBitArrays(limit)` when
a complete `bool[]` is more convenient.

The older `Incomplete*.Enumerable` member is retained for compatibility and can
still traverse the whole set. New code should prefer the bounded API.

## Three-valued bitwise operations

`Not`, `And`, `Or`, `Xor`, and `Nand` operate on sets of possible bits. A result
bit is known only when every permitted input combination gives the same output.

| Left | Right | `AND` | `OR` | `XOR` | `NAND` |
| --- | --- | --- | --- | --- | --- |
| `0` | `?` | `0` | `?` | `?` | `1` |
| `1` | `?` | `?` | `1` | `?` | `?` |
| `?` | `?` | `?` | `?` | `?` | `?` |

Operands must have the same width. A mismatch throws `ArgumentException`.
`ApplyMask(mask)` is the named equivalent of `And(mask)`.

## Logical shifts and rotations

All transformations preserve the fixed width:

| Method | High/low fill behavior |
| --- | --- |
| `ShiftLeft` / `LogicalShiftLeft` | introduces known zeroes at the low end |
| `ArithmeticShiftLeft` | same bit behavior as logical left shift |
| `LogicalShiftRight` | introduces known zeroes at the high end |
| `ArithmeticShiftRight` | repeats the highest, sign-position bit |
| `RotateLeft` | wraps overflowing high bits to the low end |
| `RotateRight` | wraps discarded low bits to the high end |

Unknown bits move or repeat exactly like known bits. A shift at least as large
as the width produces all zeroes, except arithmetic right shift, which fills
the result with the sign-position state. Rotation counts are reduced modulo the
width. Negative counts throw `ArgumentOutOfRangeException`.

```csharp
var source = new BitPattern(new bool?[]
{
    true, null, false, true
});

var logical = source.LogicalShiftRight(1);
var arithmetic = source.ArithmeticShiftRight(1);
var rotated = source.RotateLeft(1);
```

## Compatibility and intersection

`IsCompatibleWith` answers whether two patterns share at least one concrete
value. `Intersect` returns the most specific pattern that satisfies both:

```csharp
var first = new BitPattern(new bool?[] { true, null, false, null });
var second = new BitPattern(new bool?[] { null, true, false, false });

if (first.TryIntersect(second, out var intersection))
{
    Console.WriteLine(intersection); // 0011
}
```

`TryIntersect` returns `false` for contradictory known bits. `Intersect` throws
`InvalidOperationException` for the same case.

## Reverse XOR and NAND

Reverse operations treat the current pattern as the desired result and solve
for the left operand:

```text
left XOR  right = result
left NAND right = result
```

```csharp
var left = BitPattern.FromUnsigned(0b1100, 4);
var right = BitPattern.FromUnsigned(0b1010, 4);

var xorResult = left.Xor(right);
var possibleLeft = xorResult.ReverseXor(right);

Console.WriteLine(possibleLeft.IsMatch(0b1100)); // True
```

`TryReverseXor` and `TryReverseNand` return `false` instead of throwing when a
constraint has no solution. Reverse `AND` and `OR` are also available directly
on the engine as `TryReverseAnd` and `TryReverseOr`.

The matching `Incomplete*` wrappers expose `ReverseXor` and `ReverseNand` and
return the same concrete wrapper type. Complete primitive wrappers expose the
same reverse methods with either a complete or incomplete right operand.

## Solving `x & mask == result`

Use `TrySolveAnd` for an explicit mask constraint:

```csharp
var mask = BitPattern.FromUnsigned(0b1111_0000, 8);
var result = BitPattern.FromUnsigned(0b1010_0000, 8);

if (BitPattern.TrySolveAnd(mask, result, out var x))
{
    Console.WriteLine(x);                      // 1010????
    Console.WriteLine(x.CandidateCount);       // 16
    Console.WriteLine(x.IsMatch(0b1010_0110)); // True
}
```

There is also an encoded-value overload:

```csharp
BitPattern.TrySolveAnd(
    mask: 0b1111_0000,
    result: 0b1010_0000,
    width: 8,
    solution: out var x);
```

`SolveAnd` returns the solution or throws `InvalidOperationException` when the
constraint is contradictory. On a legacy wrapper, `result.TrySolveAnd(mask,
out solution)` provides the same behavior and preserves the wrapper type.

## Expressiveness limit

A `BitPattern` represents independent choices at each bit position. It cannot
represent correlations between different positions, such as "bit 2 must equal
bit 7." Reverse operations return the exact per-bit projection of their
solutions.

When the mask itself contains unknown bits, solving uses existential set
semantics: an `x` bit is permitted when some allowed mask bit can produce an
allowed result bit. Use a complete mask when the mask is a fixed protocol or
hardware value.

## Wrapper compatibility

Every legacy incomplete wrapper derives its new behavior from
`IncompleteBitPattern<TSelf>` and exposes:

- `Pattern`, `UnknownBitCount`, and `CandidateCount`;
- unsigned `MinValue` / `MaxValue` and signed counterparts;
- `IsMatch` and `IsSignedMatch`;
- bounded `EnumerateCandidates`;
- compatibility and intersection;
- masks, shifts, rotations, reverse XOR/NAND, and `TrySolveAnd`.

Existing members such as `Binary`, `Permutations`, the permutation indexer,
`Enumerable`, `Contains`, and reverse `AND`/`OR` remain available. `Binary`
setters are public consistently across the incomplete wrapper family.

## Verification strategy

The repository test suite checks the engine at three levels:

- every one of the 65,536 ordered pairs of complete byte values for forward
  AND/OR/XOR/NAND, all four reverse operations, and AND solving;
- every byte value with every shift count from zero through seven for logical
  shift, arithmetic right shift, and both rotations;
- deterministic property loops over complete 64-bit values and incomplete
  32-bit patterns for set membership, intersection, and operation closure.

These tests complement focused cases for `BigInteger` cardinality, bounded
enumeration, signed bounds, contradictory constraints, and wrapper delegation.
