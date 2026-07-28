# Incomplete values and reverse operations

[Documentation home](index.md) ·
[Bitwise values](bitwise-values.md) ·
[API reference](api-reference.md)

An incomplete value represents a set of primitive values with one ternary bit
array:

- `false`: the bit is known to be zero;
- `true`: the bit is known to be one;
- `null`: either value is allowed.

This model is useful for partial binary data and for the result of solving a
logical operation backwards.

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
known bits.

## Enumerate candidates

`Permutations` is the count implied by the unknown bits. The indexer selects one
candidate, and `Enumerable` walks all candidates:

```csharp
foreach (var candidate in pattern.Enumerable)
{
    Console.WriteLine(candidate.Value);
}
```

The candidate count grows as `2^unknownBits`. Enumerating a pattern with many
unknown bits is expensive and can be impractical. Prefer `Contains` when you
only need to test a known value.

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

## Logical operations on patterns

Incomplete values support `Not`, `And`, `Or`, `Xor`, reverse `And`, and reverse
`Or`. Most complete wrappers also support `Nand` with an incomplete operand.
Operations return new objects.

When a protocol or algorithm gives unknown bits a meaning other than "either
zero or one," convert that state before using these APIs.
