# Primitive wrappers and bitwise operations

[Documentation home](index.md) ·
[Incomplete values](incomplete-values.md) ·
[API reference](api-reference.md)

The `NumeralSystems.Net.Type.Base` namespace wraps several .NET primitive types
with a shared bit-oriented API.

## Supported wrappers

| Wrapper | .NET value | Bit width |
| --- | --- | ---: |
| `Byte` | `byte` | 8 |
| `Char` | `char` | 16 |
| `Short` | `short` | 16 |
| `UShort` | `ushort` | 16 |
| `Int` | `int` | 32 |
| `UInt` | `uint` | 32 |
| `Long` | `long` | 64 |
| `ULong` | `ulong` | 64 |
| `Float` | `float` | 32 |
| `Double` | `double` | 64 |
| `Decimal` | `decimal` | 128 |

`String` is a mutable `IList<Char>` wrapper rather than a fixed-width numeric
value.

Because these names overlap with `System` types, aliases make examples clearer:

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var value = new IntValue { Value = 42 };
```

## Value, bytes, and bits

Numeric wrappers expose:

- `Value`: the underlying primitive;
- `Bytes`: its byte representation;
- `Binary`: a fixed-width `bool[]`;
- `BitLength`: the number of bits;
- an indexer for an individual bit.

```csharp
using ByteValue = NumeralSystems.Net.Type.Base.Byte;

var value = new ByteValue { Value = 0b1010_0101 };

Console.WriteLine(value.BitLength); // 8
Console.WriteLine(value.Binary[0]); // True: least-significant bit

value.Binary = new[]
{
    true, false, true, false,
    false, true, false, true
};

Console.WriteLine(value.Value); // 165
```

`Binary[0]` is the least-significant bit. `ToString()` reverses bits within each
byte for display, while multi-byte values retain their byte-array order. Do not
infer the in-memory byte order from that formatted output.

`Bytes` follows the conversion used by the underlying implementation and may be
platform-sensitive. Define an explicit byte order when serializing across
systems.

## Logical operations

Every numeric wrapper supports:

- `Not()`;
- `And(other)`;
- `Or(other)`;
- `Xor(other)`;
- `Nand(other)`.

```csharp
using ByteValue = NumeralSystems.Net.Type.Base.Byte;

var left = new ByteValue { Value = 0b1100_0011 };
var right = new ByteValue { Value = 0b1010_1010 };

Console.WriteLine(left.And(right).Value);  // 130
Console.WriteLine(left.Or(right).Value);   // 235
Console.WriteLine(left.Xor(right).Value);  // 105
Console.WriteLine(left.Nand(right).Value); // 125
Console.WriteLine(left.Not().Value);       // 60
```

Operations return a new wrapper; they do not mutate either operand.

## Mixing complete and incomplete values

The same methods accept the corresponding incomplete type:

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var complete = new IntValue { Value = 12 };
var unknown = complete.Incomplete();
unknown.Binary[0] = null;

var result = complete.Or(unknown);

Console.WriteLine(result.IsComplete); // depends on the other bits
```

The result uses three-valued Boolean logic. For example:

- `false AND unknown` is `false`;
- `true AND unknown` is unknown;
- `true OR unknown` is `true`;
- `false OR unknown` is unknown.

See [Incomplete values](incomplete-values.md) for candidate enumeration and
reverse operations.

## Floating-point and decimal wrappers

Logical operations on `Float`, `Double`, and `Decimal` act on their binary
representations, not on their mathematical values. This is useful for binary
analysis but is different from arithmetic:

```csharp
using FloatValue = NumeralSystems.Net.Type.Base.Float;

var left = new FloatValue { Value = 1.5f };
var right = new FloatValue { Value = -0.0f };
var combined = left.Xor(right);
```

The result may be a NaN, infinity, signed zero, or another special IEEE-754
pattern. Inspect `Binary` or `Bytes` when the representation is the intended
subject.

## Reverse operations

`ReverseAnd` and `ReverseOr` solve for a possible left operand:

```text
left AND right = result
left OR  right = result
```

The solution is an incomplete value because some bits may be unconstrained. A
Boolean return value reports whether any solution exists.

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var left = new IntValue { Value = 0b1100 };
var right = new IntValue { Value = 0b1010 };
var result = left.And(right);

if (result.ReverseAnd(right, out var candidates))
{
    Console.WriteLine(candidates.Contains(left)); // True
}
```
